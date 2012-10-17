/*
*   Copyright 2007-2010 Glenn Pierce, Paul Barber,
*   Oxford University (Gray Institute for Radiation Oncology and Biology) 
*
*   This file is part of MosaicStitcher.
*
*   MosaicStitcher is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*   
*   MosaicStitcher is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with MosaicStitcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using ExtensionMethods;

using FreeImageAPI;

// The Known Overlap Correlator
// Correlates two images based on a known overlap
// Written for mosaics captured with a motorised stage

// This correlator does this:
// Adjusts the default position of the tile for the average shift of the surrounding tiles.
// Correlates a strip from the new tile (kernel) with the composite of surrounding tiles (background)
// over a search area determined here.
// The maximum correlation is found as a position for the new tile wrt the background.

namespace ImageStitching
{
    public enum FiltersEnum
    {
        [EnumDescription("None")]
        None,

        [EnumDescription("Edge Detection")]
        GenericEdgeDetection
    };

    class KernalBasedOverlapCorrelator : Correlator
    {
        private Rectangle backgroundIntersectArea;
        private Rectangle searchArea;
        private Rectangle searchAreaWithinMosaic;
        private Rectangle kernelArea;
        private Rectangle kernelAreaWithinBackground;
        private int minKernelSizeMicrons;
        private int minSearchSizeMicrons;
        private PointF startPosition;
        private CorrelationTile startTile;
        private FREE_IMAGE_COLOR_CHANNEL channel=FREE_IMAGE_COLOR_CHANNEL.FICC_RGB;
        private KernelBasedOverlapCorrelatorOptionPanel knownOverlapCorrelatorOptionPanel;
        protected static CorrelationPrefilter prefilter = null;

        
        private Rectangle searchPos;

        public const float GoodCorrelation = 0.80f;

        public KernalBasedOverlapCorrelator(MosaicInfo mosaicInfo, TiledImageView correlationDisplayViewer)
            : base(mosaicInfo, correlationDisplayViewer)
        {
            this.knownOverlapCorrelatorOptionPanel = new KernelBasedOverlapCorrelatorOptionPanel(this);
            
            this.CorrelationDisplayViewer.TileImageViewMouseDownHandler += 
                new TileImageViewMouseDelegate<TiledImageView, TiledImageViewMouseEventArgs>(OnCorrelationTileImageViewMouseDownHandler);

            this.CorrelationDisplayViewer.TileImageViewMouseMoveHandler +=
                new TileImageViewMouseDelegate<TiledImageView, TiledImageViewMouseEventArgs>(OnCorrelationTileImageViewMouseMoveHandler);
            
            this.CorrelationDisplayViewer.Paint += new PaintEventHandler(OnCorrelationTileImageViewPaint);
        }

        public override MosaicInfo MosaicInfo
        {
            set
            {
                this.knownOverlapCorrelatorOptionPanel.Reset(value);

                base.MosaicInfo = value;

                this.startPosition = CorrelationTile.GetCentreOfTiles(this.CorrelationTiles);
            }
            get
            {
                return base.MosaicInfo;
            }
        }

        public override CorrelatorOptionPanel OptionPanel
        {
            get
            {
                return knownOverlapCorrelatorOptionPanel;
            }
        }

        public static CorrelationPrefilter Prefilter
        {
            set
            {
                KernalBasedOverlapCorrelator.prefilter = value;
            }
        }

        public int KernelMinSizeMicrons
        {
            set
            {
                this.minKernelSizeMicrons = value;
            }
        }

        public int KernelMinSizePixels
        {
            get
            {
                return (int)(this.minKernelSizeMicrons / this.MosaicInfo.OriginalMicronsPerPixel);
            }
        }

        public int SearchMinSizeMicrons
        {
            set
            {
                this.minSearchSizeMicrons = value;
            }
        }

        public int SearchMinSizePixels
        {
            get
            {
                return (int)(this.minSearchSizeMicrons / this.MosaicInfo.OriginalMicronsPerPixel);
            }
        }

        // This get the size of the search area which is the size on the smallest dimesion 
        // of the overlaps. It also pins to a maximum value
        private Size GetSearchAreaSize(Rectangle intersectRect)
        {
            int minDimension = Math.Min(Math.Min(intersectRect.Width, intersectRect.Height), (int) this.SearchMinSizePixels);
            return new Size(minDimension, minDimension);
        }

        // This function gets the positions that the search bounds could possibly be placed.
        // It may well be placed randomly in this region.
        private Rectangle PossibleSearchBounds(Rectangle intersectRect)
        {
            Rectangle rect = intersectRect;

            Size size = GetSearchAreaSize(intersectRect);

            rect = rect.RectangleChangeLargestDimensionToSize(size.Width);

            this.searchPos = rect;

            return rect;
        }

        private Rectangle SearchBounds(Rectangle intersectRect, bool randomise)
        {
            Rectangle possibleSearchBounds = PossibleSearchBounds(intersectRect);
            Size size = GetSearchAreaSize(intersectRect);

            if (randomise)
                return possibleSearchBounds.RectangleExtractRandomSize(size);
            else
                return possibleSearchBounds.RectangleExtractSizeFromCentre(size);
        }

        private Size GetKernelSize(Rectangle intersectRect, Rectangle searchRect)
        {
            Size size;

            if (intersectRect.Width < intersectRect.Height)
            {
                size = new Size(Math.Min((int)intersectRect.Width, this.KernelMinSizePixels), intersectRect.Height);
                size.Height -= (searchRect.Height / 2);
            }
            else
            {
                size = new Size(intersectRect.Width, Math.Min((int)intersectRect.Height, this.KernelMinSizePixels));             
                size.Width -= (searchRect.Width / 2);              
            }

            return size;
        }

        private Rectangle KernelBounds(Rectangle intersectRect, Rectangle searchRect, bool randomise)
        {
            // First find the intersection between the tile and the background drawn bounds.
            //Rectangle intersectRect = backgroundRect;
            Size size = GetKernelSize(intersectRect, searchRect);  
            Rectangle rect = new Rectangle(new Point(), size);

            return rect.RectanglePositionAroundCentre(searchRect.Centre());
        }

        public double Correlate(FreeImageAlgorithmsBitmap backgroundImage, FreeImageAlgorithmsBitmap fib,
            CorrelationTile tile1, CorrelationTile tile2,
            bool randomise, out Point pt)
        {
            this.backgroundIntersectArea = Rectangle.Empty;
            this.searchArea = Rectangle.Empty;
            this.searchAreaWithinMosaic = Rectangle.Empty;

            double measure = 0.0;
            pt = new Point(); 

            // Find the intersection between the tile and the background drawn bounds.
            this.backgroundIntersectArea = Rectangle.Intersect(tile1.CorrelatedBoundsRelativeToOrigin, tile2.CorrelatedBoundsRelativeToOrigin);

            if (this.backgroundIntersectArea == Rectangle.Empty)
            {
                SendFeedback(String.Format("{0} and {1} did not intersect ?", tile1.Tile.FileName, tile2.Tile.FileName)
                    + Environment.NewLine, Color.Red);

                return 0.0;
            }

            // determine the area over which to perform the correlation
            // defines the maximum shift allowed of the new tile.
            this.searchArea = this.SearchBounds(this.backgroundIntersectArea, randomise);
           
            // find the kernel area in the bg image
            this.kernelAreaWithinBackground = this.KernelBounds(this.backgroundIntersectArea, this.searchArea, randomise);

            // locate the area in the new image
            this.kernelArea = this.kernelAreaWithinBackground;

            this.kernelArea.X -= tile1.CorrelatedBoundsRelativeToOrigin.X;
            this.kernelArea.Y -= tile1.CorrelatedBoundsRelativeToOrigin.Y;

            // Call start correlation delegate, for screen update
            this.SendTileCorrelatationBegin(tile1, fib);

            if (KernalBasedOverlapCorrelator.prefilter == null)
            {
                backgroundImage.KernelCorrelateImageRegions(fib, backgroundIntersectArea, kernelArea,
                    this.searchArea, out pt, out measure);
            }
            else
            {
                backgroundImage.KernelCorrelateImageRegions(fib, backgroundIntersectArea, kernelArea, this.searchArea,
                    KernalBasedOverlapCorrelator.prefilter, out pt, out measure);
            }

            if (measure > KernalBasedOverlapCorrelator.GoodCorrelation)
            {
                pt = CorrelationTile.TranslateBackgroundPointToMosaicPoint(pt);

                return measure;
            }

            if (tile1.NumberOfAtemptedCorrelations < CorrelationTile.MaxNumberOfCorrelationAttempts)
            {
                tile1.NumberOfAtemptedCorrelations++;

                return this.Correlate(backgroundImage, fib, tile1, tile2, 
                    this.knownOverlapCorrelatorOptionPanel.RandomKernelSearch, out pt);
            }
            else
            {
                // If we are debugging lets pause the thread for closer inspection
                #if DEBUG
                this.CorrelationPause();
                #endif
            }

            return measure;
        }

        public override void DebugBackgroundPaint(CorrelationDebugForm debugForm, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 255, 0, 255)), this.backgroundIntersectArea);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 255)), this.kernelAreaWithinBackground);
            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 255, 255, 255)), this.searchPos);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 0, 255, 0)), this.searchArea);
        }

        public override void DebugCurrentImagePaint(CorrelationDebugForm debugForm, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 255)), this.kernelArea);
        }
        
        private List<CorrelationTile> GetFourBorderingTiles(CorrelationTile tile)
        {
            // Here we filter the previous placed tile list so they are sorted to the overlap
            // with the largest area is first.
            // The overalp must greater than 25% in one direction
            // We than return the first 4, which are probably the ones above, below, left and right
            List<CorrelationTile> tiles = new List<CorrelationTile>();

            foreach (CorrelationTile t in this.CorrelationTiles)
            {
                if (tile.Equals(t))
                    continue;

                if (t.PerformedCorrelation == false)
                    continue;

                if (t.Tile.IsAdjusted == false)
                    continue;

                if (t.Tile.IsDummy)
                    continue;

                if (t.Tile.Bounds.IntersectsWith(tile.Tile.Bounds))
                {
                    Rectangle intersection = t.Tile.Bounds;

                    intersection.Intersect(tile.Tile.Bounds);

                    if (intersection.Width < 0.25 * tile.Tile.Width &&
                       intersection.Height < 0.25 * tile.Tile.Height)
                    {
                        continue;
                    }

                    tiles.Add(t);

                }
            }
         
            if (tiles.Count == 0)
                return null;

            tiles.Sort(new TileIntersectionComparer(tile));

            // We now, after this line, will have the tiles with the largest intersection areas
            tiles = tiles.GetRange(0, Math.Min(tiles.Count, 4));

            return tiles;
        }

        private int FindMaxIndex(double[] array)
        {
            double largest = array[0];
            int index = 0;

            for (int i = 1; i < array.Length; i++)
            {
                if(Double.IsNaN(array[i]))
                {
                    continue;
                }

                if (largest < array[i])
                {
                    largest = array[i];
                    index=i;       
                }
            }

            return index;
        }

        private FreeImageAlgorithmsBitmap TileImage(Tile tile)
        {
            FreeImageAlgorithmsBitmap fib = tile.LoadFreeImageBitmap();
            fib.ConvertToStandardType(true);
            if (!fib.IsGreyScale)
            {
                if (this.channel == FREE_IMAGE_COLOR_CHANNEL.FICC_RGB)
                    fib.ConvertToGreyscale();
                else
                    fib.GetChannel(this.channel);
            }

            return fib;
        }

        // If quick is true the the tile is correlated with only the previously placed 
        // tile that is overlapping by the largest area. quick is opposite to SearchAllEdges
        private bool CorrelateTileWithPreviousPlacedTiles(CorrelationTile tile, bool quick)
        {
            bool success = false;

            // Here we keep a reference to the one that correlates best.
            double measure = 0.0;
            int count = 0;
            double[] measures = new double[4];
            Point[] points = new Point[4];

            if (tile.DontCorrelate)
                return success;

            // Return max of four tiles
            List<CorrelationTile> overLappingTiles = GetFourBorderingTiles(tile);

            tile.PerformedCorrelation = true;

//            FreeImageAlgorithmsBitmap fib = tile.Tile.LoadFreeImageBitmap();
//            fib.ConvertToStandardType(true);
            FreeImageAlgorithmsBitmap fib = TileImage(tile.Tile);

            if (overLappingTiles == null || overLappingTiles.Count < 1)
            {
                success = false;
                this.NumberOfFailedCorrelations++;
                tile.CorrelationFailed = true;
                this.SendTileCorrelated(tile, tile.Tile.Bounds, fib, false);
                return false;
            }

            if (quick)  // just use the first of these 4 bordering tiles
            {
                overLappingTiles = overLappingTiles.GetRange(0, 1);
            }

            if (overLappingTiles != null)
            {          
                foreach (CorrelationTile t in overLappingTiles)
                {
                    success = false;
                    Point pt = new Point();

                    if (this.knownOverlapCorrelatorOptionPanel.AdjustPosBeforeCorrelate)
                    {
                        // We are trying to correlate tile with tile (t)
                        // Tile t should have been correlated so here we first adjust the position of 
                        // tile so it is moved the same amount that tile t was.
                        pt = new Point(tile.Tile.OriginalPosition.X + t.Tile.AdjustedPositionShift.X,
                                       tile.Tile.OriginalPosition.Y + t.Tile.AdjustedPositionShift.Y);

                        tile.CorrelationPosition = pt;
                    }

                    measure = this.Correlate(this.BackgroundImage, fib, tile, t, false, out pt);

                    measures[count] = measure;
                    points[count] = pt;

                    count++;
                }
            }

            // Search the array of measures, one value for each correlation performed, and find the largest
            int maxIndex = FindMaxIndex(measures);

            Color colour = Color.Red;

            if (measures[maxIndex] > KernalBasedOverlapCorrelator.GoodCorrelation)
            {
                colour = Color.Blue;
                NumberOfCorrelatedTiles++;

                // We have a good correlation so we need to store the correlation point
                tile.CorrelationPosition = tile.Tile.AdjustedPosition = points[maxIndex];
                tile.CorrelationFailed = false;
                tile.Tile.IsAdjusted = true;
                this.SendTileCorrelated(tile, tile.Tile.AdjustedBounds, fib, true);

                success = true;

                SendFeedback("Correlating ");

                SendFeedback(String.Format("{0} x {1} ", tile.Tile.FileName, overLappingTiles[maxIndex].Tile.FileName), Color.Green);
                SendFeedback("resulted in a factor of  ");
                SendFeedback(String.Format(" {0:0.00}", measures[maxIndex]), colour);
                SendFeedback(String.Format("   Δ {0},{1}", tile.Tile.AdjustedPositionShift.X, tile.Tile.AdjustedPositionShift.Y));
                SendFeedback(Environment.NewLine, Color.Red);
            }
            else
            {
                // Set the tile adjusted position to its normal position by default.
                tile.Tile.AdjustedPosition = tile.CorrelationPosition;

                success = false;
                this.NumberOfFailedCorrelations++;
                tile.Tile.IsAdjusted = false;
                tile.CorrelationFailed = true;
                this.SendTileCorrelated(tile, tile.Tile.Bounds, fib, false);

                SendFeedback("Correlating ");
                SendFeedback(String.Format("{0} x {1} ", tile.Tile.FileName, overLappingTiles[maxIndex].Tile.FileName), Color.Green);
                SendFeedback("resulted in a factor of  ");
                SendFeedback(String.Format(" {0:0.00}", measures[maxIndex]), colour);
                SendFeedback(String.Format("  Δ {0},{1}", tile.Tile.AdjustedPositionShift.X, tile.Tile.AdjustedPositionShift.Y));
                SendFeedback(Environment.NewLine, Color.Red);
            }

            this.ThreadController.ReportThreadPercentage(this, null, NumberOfCorrelatedTiles, this.CorrelationTiles.Count);

            return success;
        }

        private void PlaceFirstTile()
        {
            // puts the first tile in the sorted list at its default place
            CorrelationTile firstTile = this.CorrelationTiles[0];

            CorrelationTile.Origin = firstTile.BackgroundBoundsWithinMosaic.Location;
//            FreeImageAlgorithmsBitmap fib = firstTile.Tile.LoadFreeImageBitmap();
            FreeImageAlgorithmsBitmap fib = TileImage(firstTile.Tile);

            firstTile.Tile.IsAdjusted = true;
            firstTile.PerformedCorrelation = true;
            NumberOfCorrelatedTiles++;
            firstTile.Tile.AdjustedPosition = firstTile.Tile.OriginalPosition;
            this.SendTileCorrelatationBegin(firstTile, fib);
            this.SendTileCorrelated(firstTile, firstTile.Tile.Bounds, fib, true);

            this.ThreadController.ReportThreadPercentage(this, "Placed  Tile " + firstTile.Tile.FileName + Environment.NewLine,
                NumberOfCorrelatedTiles, CorrelationTiles.Count);

            //fib.Dispose();
        }

        private void DrawPreviouslyPlacedTiles(CorrelationTile tile)
        {
            this.BackgroundImage.Clear();

            // Paste the previous placed images (correlated) onto the temporary (bg) image
            foreach (CorrelationTile t in this.CorrelationTiles)
            {
                if (t.PerformedCorrelation == false)
                    continue;

                if (!t.BoundsRelativeToMosaic.IntersectsWith(tile.BoundsRelativeToMosaic))
                    continue;

 //               FreeImageAlgorithmsBitmap fib = Tile.LoadFreeImageBitmapFromFile(t.Tile.FilePath);
 //               fib.ConvertToStandardType(true);
                FreeImageAlgorithmsBitmap fib = TileImage(t.Tile);

                // Paste first image. We blend as it may improve the correlation.
                try
                {
                    this.BackgroundImage.PasteFromTopLeft(fib, t.CorrelatedBoundsRelativeToOrigin.Location, true);
                }
                catch (FreeImageException)
                {
                    this.SendFeedback("Tile positions do not intersect for pasting together" + Environment.NewLine
                        , Color.Red);
                }
                finally
                {
                    fib.Dispose();
                }
            }
        }

        private void CircularSortTiles(List<CorrelationTile> tiles)
        {
            //List<CorrelationTile> tileCopy = new List<Tile>(tiles);

            // Find centre of tiles
            PointF center = CorrelationTile.GetCentreOfTiles(tiles);
            // Sort the tile from the centre of the mosaic.
            tiles.Sort(new CorrelationTileCircularComparer(center));
        }

        private CorrelationTile FindTileAroundPoint(Point pt)
        {
            CorrelationTile tile = null;

            // Find the tile that contains the centre point
            foreach (CorrelationTile t in this.CorrelationTiles)
            {
                if (t.Tile.Bounds.Contains(pt))
                {
                    tile = t;
                    break;
                }
            }

            return tile;
        }

        void OnCorrelationTileImageViewMouseDownHandler(TiledImageView sender, TiledImageViewMouseEventArgs args)
        {
            if (!this.knownOverlapCorrelatorOptionPanel.SelectStartTile.Checked)
                return;

            this.startPosition = args.VirtualAreaPosition;
            Point pt = new Point((int) this.startPosition.X, (int) this.startPosition.Y);
            this.startTile = FindTileAroundPoint(pt);
            this.knownOverlapCorrelatorOptionPanel.SelectStartTile.Checked = false;
            sender.Invalidate();
        }

        void OnCorrelationTileImageViewMouseMoveHandler(TiledImageView sender, TiledImageViewMouseEventArgs args)
        {
            if (!this.knownOverlapCorrelatorOptionPanel.SelectStartTile.Checked)
                return;

            this.startPosition = args.VirtualAreaPosition;
            Point pt = new Point((int) this.startPosition.X, (int) this.startPosition.Y);
            this.startTile = FindTileAroundPoint(pt);
            sender.Invalidate();
        }

        void OnCorrelationTileImageViewPaint(object sender, PaintEventArgs e)
        {
            TiledImageView tiledImageView = (TiledImageView) sender;
            e.Graphics.Transform = tiledImageView.GetTransform();

            // Draw Rectangle Bounds
            if (this.startTile != null)
            {
                int width = (int)(5.0f / (Math.Min(tiledImageView.XScaleFactor, tiledImageView.YScaleFactor)));

                e.Graphics.DrawRectangle(new Pen(Color.DarkGreen, width), this.startTile.Tile.Bounds);
            }
        }

        protected override void Correlate(List<CorrelationTile> items)
        {
            try
            {
                items.Sort(new CorrelationTileCircularComparer(this.startPosition));

                // The entry point for a new correlation
                NumberOfCorrelatedTiles = 0;
                NumberOfFailedCorrelations = 0;

                SendFeedback("Started Correlation." + Environment.NewLine, Color.Green);

                PlaceFirstTile();

                // For all the other tiles, do ...
                List<CorrelationTile> tileListwithoutFirst = this.CorrelationTiles.GetRange(1, this.CorrelationTiles.Count - 1);
                foreach (CorrelationTile tile in tileListwithoutFirst)
                {
                    this.ProcessPause();

                    if (this.ThreadController.ThreadAborted)
                    {
                        if (this.ThreadController != null)
                        {
                            this.ThreadController.ReportThreadCompleted(this,
                                "Correlation Completed" + Environment.NewLine, false);
                        }

                        return;
                    }

                    // If the tile is a dummy tile (Black) we can't correlate so we ignore it
                    if (tile.Tile.IsDummy)
                    {
                        tile.Tile.IsAdjusted = true;
                        NumberOfCorrelatedTiles++;
                        continue;
                    }

                    // Finds the origin of the background image (mini mosaic of surrounding tiles)
                    // relative to the full mosaic.
                    CorrelationTile.Origin = tile.BackgroundBoundsWithinMosaic.Location;

                    // Draws previously correlated, surrounding tiles into the bg image
                    this.DrawPreviouslyPlacedTiles(tile);

                    // We have now placed down the first tile and we have the bounds of all previous
                    // drawing to the background image.
                    // We need to determine a section of the background image to correlate with out new 
                    // tile.
                    this.CorrelateTileWithPreviousPlacedTiles(tile,
                        !this.knownOverlapCorrelatorOptionPanel.SearchAllEdges);

                    Thread.Sleep(10);
                }

                // Lets try the tiles we could correlate again.
                //CorrelationTile.KernelMinSize = CorrelationTile.KernelSizeEnum.Medium;
                //CorrelationTile.SearchMinSize = CorrelationTile.SearchSizeEnum.Medium;
                //this.RandomiseKernelRegion = true;

                while (NumberOfCorrelatedTiles < this.CorrelationTiles.Count)
                {
                    SendFeedback("Trying to recorrelate previous failed attempts." + Environment.NewLine, Color.Red);

                    this.ProcessPause();

                    if (this.ThreadController.ThreadAborted)
                    {
                        if (this.ThreadController != null)
                            this.ThreadController.ReportThreadCompleted(this, "Correlation Completed", false);

                        return;
                    }

                    foreach (CorrelationTile tile in this.CorrelationTiles)
                    {
                        this.ProcessPause();

                        if (this.ThreadController.ThreadAborted)
                        {
                            if (this.ThreadController != null)
                                this.ThreadController.ReportThreadCompleted(this, "Correlation Completed", false);

                            return;
                        }

                        if (tile.Tile.IsAdjusted || tile.DontCorrelate)
                            continue;

                        if (tile.PerformedCorrelation == false)
                            continue;

                        CorrelationTile.Origin = tile.BackgroundBoundsWithinMosaic.Location;

                        this.DrawPreviouslyPlacedTiles(tile);

                        this.CorrelateTileWithPreviousPlacedTiles(tile, false);
                    }
                }

                if (this.ThreadController != null)
                {
                    this.ThreadController.ReportThreadCompleted(this,
                        "Correlation Completed" + Environment.NewLine, false);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        protected override void CorrelationStarted()
        {
            if (this.knownOverlapCorrelatorOptionPanel.Filter == FiltersEnum.GenericEdgeDetection)
                KernalBasedOverlapCorrelator.Prefilter = new CorrelationPrefilter(FreeImageAlgorithmsBitmap.EdgeDetect);

            this.KernelMinSizeMicrons = this.knownOverlapCorrelatorOptionPanel.StripSize;
            this.SearchMinSizeMicrons = this.knownOverlapCorrelatorOptionPanel.SearchSize;

            this.channel = this.knownOverlapCorrelatorOptionPanel.Channel;

            this.knownOverlapCorrelatorOptionPanel.SelectStartTile.Checked = false;
            this.startTile = null;
        }

        protected override void CorrelationStopped()
        {
            this.knownOverlapCorrelatorOptionPanel.SelectStartTile.Enabled = true;
        }
    }
}
