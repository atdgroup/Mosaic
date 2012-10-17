/*
*   Copyright 2007-2011 Glenn Pierce, Paul Barber,
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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;

using ExtensionMethods;

using FreeImageAPI;
using FreeImageIcs;

namespace ImageStitching
{
	/// <summary>
	/// Represent each image that we wish to stitch.
	/// </summary>
    public class Tile : IComparable<Tile>, IEquatable<Tile>
	{
        private readonly object thumbnailLock = new object();

        // static values - for all tiles
        private static string filedir = null;
        private static bool isCompositeRGB = false;    // false is normal case arrays below are not used then.
        public static int nCompositeImages = 1;
        public static FREE_IMAGE_COLOR_CHANNEL[] channel = new FREE_IMAGE_COLOR_CHANNEL[3] { FREE_IMAGE_COLOR_CHANNEL.FICC_RED, FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN, FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE };
        public static double[] scaleMin = new double[3] { 0.0, 0.0, 0.0 };
        public static double[] scaleMax = new double[3] { 255.0, 255.0, 255.0 };
        public static Point[] channelShift = new Point[3];
//        public static double[] TotalMaxIntensity = new double[3] { 255.0, 255.0, 255.0 };  // max intensity present in each channel - comes from seq file
        public static string[] channelPrefix = new string[3] { "", "", "" };
        private static bool usingAdjustedPositionsForAllTiles = false;

        // tile specific values
        private FreeImageAlgorithmsBitmap thumbnail = null;
        private string[] filename = new string[3] { "","","" };  // element [0] is normally used, others are used for composite images, see above
        private bool dummyTile;
        private int number;
        private int width;
        private int height;
        private FreeImageAPI.FREE_IMAGE_TYPE type;
        private int colorDepth;
        private Point originalPosition;
        private Point adjustedPosition;
        private bool useAdjustedPosition = false;
        private bool isAdjusted;      // indicates a seccessful correlation
        private double minIntensity;
        private double maxIntensity;

        public static int ThumbnailWidth = 200;
        public static int ThumbnailHeight;

        public static bool IsCompositeRGB
        {
            get
            {
                return Tile.isCompositeRGB;
            }
            set
            {
                Tile.isCompositeRGB = value;
            }
        }




        public static void SetUseAjustedPositionForAllTiles(List<Tile> tiles, bool use)
        {
            foreach (Tile t in tiles)
            {
                t.UseAdjustedPosition = use;
            }

            Tile.usingAdjustedPositionsForAllTiles = use;
        }
  
        public static void ResetAjustedPositionForAllTiles(List<Tile> tiles)
        {
            foreach (Tile t in tiles)
            {
                t.IsAdjusted = false;
                t.AdjustedPosition = t.OriginalPosition;
            }

            Tile.usingAdjustedPositionsForAllTiles = false;
        }

        public static bool UsingAdjustedPositionsForAllTiles
        {
            get
            {
                return Tile.usingAdjustedPositionsForAllTiles;
            }
        }

        public bool UseAdjustedPosition
        {
            set
            {
                this.useAdjustedPosition = value;
            }
            get
            {
                return this.useAdjustedPosition;
            }
        }

        private void SetupThumbnailScaleFactor(int width, int height)
        {
            this.width = width;
            this.height = height;

            float zoomFactor = (float)ThumbnailWidth / width;
            Tile.ThumbnailHeight = (int)(zoomFactor * height);
        }

        private void Initialise(string filepath, int width, int height)
        {
            Tile.filedir = System.IO.Path.GetDirectoryName(filepath);

            this.filename[0] = System.IO.Path.GetFileName(filepath);

            this.SetupThumbnailScaleFactor(width, height);
        }


        internal Tile(Tile tile)
        {
            this.dummyTile = tile.dummyTile;
            this.number = tile.number;
            this.originalPosition = tile.originalPosition;
            this.ColorDepth = tile.colorDepth;
            this.type = tile.type;

            Initialise(tile.FilePath, tile.Width, tile.Height);
        }

        internal Tile(string filepath, int number, Point position, int width, int height,
            int colorDepth, FREE_IMAGE_TYPE type)
        {
            this.dummyTile = false;
            this.number = number;
            this.originalPosition = position;
            this.ColorDepth = colorDepth;
            this.type = type;

            Initialise(filepath, width, height);
        }

        internal Tile(Stream imageStream, string filepath, int number, Point position, int width, int height,
            int pixelFormat, int minIntensity, int maxIntensity, FREE_IMAGE_TYPE type)
            :this(filepath, number, position, width, height, pixelFormat, type)
        {

            FreeImageAlgorithmsBitmap fib = new FreeImageAlgorithmsBitmap(imageStream);

            this.thumbnail = fib;

            GC.KeepAlive(this.thumbnail);  

            this.minIntensity = minIntensity;
            this.maxIntensity = maxIntensity;
        }

        internal Tile(string filepath, int number, int width, int height)
        {
            this.number = number;

            Initialise(filepath, width, height);
        }

        internal Tile(string filepath, Point position, int width, int height)
        {
            this.OriginalPosition = position;

            Initialise(filepath, width, height);
		}

        /*
        public static bool operator ==(Tile left, Tile right)
        {
            return (left.FileName == right.FileName);
        }

        public static bool operator !=(Tile left, Tile right)
        {
            return (left.FileName != right.FileName);
        }
        */

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((obj is Tile) && (this == ((Tile)obj)));
        }

        public bool Equals(Tile other)
        {
            return (this.FileName == other.FileName);
        }

        public int CompareTo(Tile other)
        {
            if (this.Position.Y != other.Position.Y)
                return this.Position.Y.CompareTo(other.Position.Y);

            return this.Position.X.CompareTo(other.Position.X);
        }

        public object ThumbnailLock
        {
            get
            {
                return this.thumbnailLock;
            }
        }

        public float ThumbnailToFullWidthScaleFactor
        {
            get
            {
                return (float) Tile.ThumbnailWidth / this.Width;
            }
        }

        public float ThumbnailToFullHeightScaleFactor
        {
            get
            {
                return (float) Tile.ThumbnailHeight / this.Height;
            }
        }

        public static void ResizeToThumbnail(FreeImageAlgorithmsBitmap fib)
        {
            int height = Tile.ThumbnailHeight;

            // Scale to a width of around 200 pixels
            if (fib.Width <= Tile.ThumbnailWidth)
                return;

            if (height == 0)
            {
                height = (int)(((float)ThumbnailWidth / fib.Width) * fib.Height);
            }

            fib.Rescale(Tile.ThumbnailWidth, height, FREE_IMAGE_FILTER.FILTER_BILINEAR);
        }

        public int MemorySizeInBytes
        {
            get
            {
                return (this.ColorDepth * this.height * this.width) / 8;
            }
        }

        public float ThumbnailImageSizeRatio
        {
            get
            {
                return (float) Tile.ThumbnailWidth / this.Width;
            }
        }


        public Point GetTilePositionRelativeToPoint(Point point)
        {
            return new Point(this.Position.X - point.X, this.Position.Y - point.Y);
        }

        public Rectangle GetTileRectangleRelativeToPoint(Point point)
        {
            return new Rectangle(GetTilePositionRelativeToPoint(point), this.Size);
        }

        public bool IsDummy
        {
            get
            {
                return this.dummyTile;
            }
            set
            {
                this.dummyTile = value;
            }
        }

        public bool IsAdjusted
        {
            get
            {
                return this.isAdjusted;
            }
            set
            {
                this.isAdjusted = value;
            }
        } 

        public int TileNumber
        {
            get
            {
                return this.number;
            }       
        }

        public FreeImageAlgorithmsBitmap Thumbnail
        {
            get
            {
                return this.thumbnail;
            }
            set
            {
                if (value == null)
                    MessageBox.Show("Error");

                this.thumbnail = value;
                GC.KeepAlive(this.thumbnail);  
            }
        }

        public static FreeImageAlgorithmsBitmap LoadFreeImageBitmapFromFile(string filepath)
        {  // This method is independant of Tile
            IcsFile icsFile;
            FreeImageAlgorithmsBitmap fib = null;

            try
            {
                if (IcsFile.IsIcsFile(filepath))
                {
                    icsFile = new IcsFile(filepath);
                    fib = icsFile.FreeImageAlgorithmsBitmap;
                    icsFile.Close();
                }
                else
                {
                    fib = new FreeImageAlgorithmsBitmap(filepath);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            // Resample seems to only work for uint16 not int16 currently
            if (fib.ImageType == FREE_IMAGE_TYPE.FIT_INT16)
                fib.ConvertInt16ToUInt16();

            return fib;
        }

        public FreeImageAlgorithmsBitmap LoadFreeImageBitmap(string filepath)
        {   // all overloaded functions come through this method

            FreeImageAlgorithmsBitmap fib = null;

            if (!System.IO.File.Exists(filepath))
            {
                this.dummyTile = true;
                return new FreeImageAlgorithmsBitmap(this.Width, this.Height,
                        this.FreeImageType, this.colorDepth);
            }
            
            if (!(Tile.isCompositeRGB)) {
                fib = LoadFreeImageBitmapFromFile(filepath);
            }
            else {   //// Composite RGB: Insert 3 grey images into 1 24-bit colour image
                FreeImageAlgorithmsBitmap fib8 = null;
                int n = Tile.nCompositeImages;

                // Crop images differently to apply the channel shift
                // shift for channel 0 will always be 0,0 but
                // crop for the channel with the min shift will be 0 to width-range and
                // crop for the channel with the max shift will be range to width.

                int minX, minY, midX, midY, maxX, maxY, rX, rY;
                int[] orderX = new int[3], orderY = new int[3];
                int[] x = new int[3], y = new int[3];
                x[0] = -Tile.channelShift[0].X;
                x[1] = -Tile.channelShift[1].X;
                x[2] = -Tile.channelShift[2].X;
                y[0] = -Tile.channelShift[0].Y;
                y[1] = -Tile.channelShift[1].Y;
                y[2] = -Tile.channelShift[2].Y;

                OrderShifts(x, orderX, out minX, out midX, out maxX);
                OrderShifts(y, orderY, out minY, out midY, out maxY);

                // ranges
                rX = maxX - minX;
                rY = maxY - minY;

                int[] shiftXl = new int[3], shiftYt = new int[3];
                int[] shiftXr = new int[3], shiftYb = new int[3];
                shiftXl[0] = 0; // the shift for min shift channel
                shiftXr[0] = rX; // the shift for min shift channel
                shiftXl[1] = midX - minX;
                shiftXr[1] = maxX - midX;
                shiftXl[2] = rX; // the shift for max shift channel
                shiftXr[2] = 0; // the shift for max shift channel
                shiftYt[0] = 0; // the shift for min shift channel
                shiftYb[0] = rY; // the shift for min shift channel
                shiftYt[1] = midY - minY;
                shiftYb[1] = maxY - midY;
                shiftYt[2] = rY; // the shift for max shift channel
                shiftYb[2] = 0; // the shift for max shift channel

                fib8 = LoadFreeImageBitmapFromFile(this.getFilePath(0));
                fib8.LinearScaleToStandardType(Tile.scaleMin[0], Tile.scaleMax[0]);
 //               fib8.Crop(0, 2, fib8.Width - 1 - 4, fib8.Height - 1);
                fib8.Crop(shiftXl[orderX[0]], shiftYt[orderY[0]], fib8.Width - 1 - shiftXr[orderX[0]], fib8.Height - 1 - shiftYb[orderY[0]]);
                fib = new FreeImageAlgorithmsBitmap(fib8.Width, fib8.Height, FREE_IMAGE_TYPE.FIT_BITMAP, 24);
                FreeImage.SetChannel(fib.Dib, fib8.Dib, Tile.channel[0]);
                fib8.Dispose();

                fib8 = LoadFreeImageBitmapFromFile(this.getFilePath(1));
                fib8.LinearScaleToStandardType(Tile.scaleMin[1], Tile.scaleMax[1]);
//                fib8.Crop(4, 0, fib8.Width - 1, fib8.Height - 1 - 2);
                fib8.Crop(shiftXl[orderX[1]], shiftYt[orderY[1]], fib8.Width - 1 - shiftXr[orderX[1]], fib8.Height - 1 - shiftYb[orderY[1]]);
                FreeImage.SetChannel(fib.Dib, fib8.Dib, Tile.channel[1]);
                fib8.Dispose();

                if (n == 3)  // may not have 3 files but should have at least 2
                {
                    fib8 = LoadFreeImageBitmapFromFile(this.getFilePath(2));
                    fib8.LinearScaleToStandardType(Tile.scaleMin[2], Tile.scaleMax[2]);
//                    fib8.Crop(2, 1, fib8.Width - 1 - 2, fib8.Height - 1 - 1);
                    fib8.Crop(shiftXl[orderX[2]], shiftYt[orderY[2]], fib8.Width - 1 - shiftXr[orderX[2]], fib8.Height - 1 - shiftYb[orderY[2]]);
                    FreeImage.SetChannel(fib.Dib, fib8.Dib, Tile.channel[2]);
                    fib8.Dispose();
                }
            }


            // Resample seems to only work for uint16 not int16 currently
            if (fib.ImageType == FREE_IMAGE_TYPE.FIT_INT16)
                fib.ConvertInt16ToUInt16();

            return fib;
        }

        private static void OrderShifts(int[] x, int[] orderX, out int minX, out int midX, out int maxX)
        {   // Take in an array of 3 values, x
            // Return the order in orderX[], orderX[chan]=0 for the smallest, orderX[chan]=2 the largest
            // The min and max values found are also returned.

            if (x[0] >= x[1] && x[0] >= x[2]) // try channel 0 largest
            {
                maxX = x[0];
                orderX[0] = 2;  // largest
                if (x[1] >= x[2])
                {
                    orderX[1] = 1;
                    orderX[2] = 0;  // smallest
                    midX = x[1];
                    minX = x[2];
                }
                else
                {
                    orderX[2] = 1;
                    orderX[1] = 0; // smallest
                    midX = x[2];
                    minX = x[1];
                }
            }
            else if (x[1] >= x[0] && x[1] >= x[2]) // try channel 1 largest
            {
                maxX = x[1];
                orderX[1] = 2;  // largest
                if (x[0] >= x[2])
                {
                    orderX[0] = 1;
                    orderX[2] = 0;  // smallest
                    midX = x[0];
                    minX = x[2];
                }
                else
                {
                    orderX[2] = 1;
                    orderX[0] = 0; // smallest
                    midX = x[2];
                    minX = x[0];
                }
            }
            else  // channel 2 largest
            {
                maxX = x[2];
                orderX[2] = 2;  // largest
                if (x[0] >= x[1])
                {
                    orderX[0] = 1;
                    orderX[1] = 0;  // smallest
                    midX = x[0];
                    minX = x[1];
                }
                else
                {
                    orderX[1] = 1;
                    orderX[0] = 0; // smallest
                    midX = x[1];
                    minX = x[0];
                }
            }
        }

        public FreeImageAlgorithmsBitmap LoadFreeImageBitmap()
        {
            FreeImageAlgorithmsBitmap fib = this.LoadFreeImageBitmap(this.FilePath);

            return fib;
        }

        public FreeImageAlgorithmsBitmap LoadFreeImageBitmap(int width, int height)
        {
            FreeImageAlgorithmsBitmap fib = this.LoadFreeImageBitmap(this.FilePath);

            if (width != 0 || height != 0)
                fib.Rescale(width, height, FREE_IMAGE_FILTER.FILTER_BOX);

            return fib;
        }

        /*        public Bitmap LoadBitmap(string filepath, double minIntensity, double maxIntensity)
                {
                    try
                    {
                        using (FreeImageAlgorithmsBitmap fib = this.LoadFreeImageBitmap(filepath))
                        {
                            if (fib.IsGreyScale)
                            {
                                fib.LinearScaleToStandardType(minIntensity, maxIntensity);
                                fib.SetGreyLevelPalette();
                            }

                            return fib.ToBitmap();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Failed to load the image for tile path " + filepath);

                        return null;
                    }
                }
        */

        public string FilePath
        {
            get
            {
                return (Tile.filedir + System.IO.Path.DirectorySeparatorChar + this.filename[0]);
            }
        }

        public string getFilePath (int c)
        {
            return (Tile.filedir + System.IO.Path.DirectorySeparatorChar + this.filename[c]);
        }

        public string FileName
        {
            get
            {
                return this.filename[0];
            }
        }

        public string getFileName(int c)
        {
            return this.filename[c];
        }

        public void setFileName(int c, string path)
        {
            this.filename[c] = path;
        }

        public string FileNameWithoutExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.filename[0]);
            }
        }

        public double MinIntensity
        {
            get
            {
                return this.minIntensity;
            }
            set
            {
                this.minIntensity = value;
            }
        }

        public double MaxIntensity
        {
            get
            {
                return this.maxIntensity;
            }
            set
            {
                this.maxIntensity = value;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                this.SetupThumbnailScaleFactor(this.width, this.height);
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
                this.SetupThumbnailScaleFactor(this.width, this.height);
            }
        }

        public Size Size
        {
            get
            {
                return new Size(this.width, this.height);
            }
        }

        public int ColorDepth
        {
            get
            {
                return this.colorDepth;
            }
            set
            {
                this.colorDepth = value;
            }
        }

        public FREE_IMAGE_TYPE FreeImageType
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public virtual Point Position
        {
            get
            {
                if (this.UseAdjustedPosition && this.IsAdjusted)
                    return this.AdjustedPosition;

                return this.originalPosition;
            }
        }

        public Point OriginalPosition
        {
            get
            {
                return this.originalPosition;
            }
            set
            {
                this.originalPosition = value;
                // Set the tile adjusted position to its normal position by default.
                this.AdjustedPosition = value;
            }
        }

        public Point AdjustedPosition
        {
            get
            {
                return this.adjustedPosition;
            }
            set
            {
                this.adjustedPosition = value;
            }
        }

        public Point AdjustedPositionShift
        {
            get
            {
                return new Point(this.adjustedPosition.X - this.originalPosition.X,
                                 this.adjustedPosition.Y - this.originalPosition.Y);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.Position, this.Size);
            }
        }

        public Rectangle AdjustedBounds
        {
            get
            {
                return new Rectangle(this.AdjustedPosition, this.Size);
            }
        }

        public PointF Centre
        {
            get
            {
                return this.Bounds.Centre();
            }
        }

        public Size ThumbnailSize
        {
            get
            {
                return new Size(Tile.ThumbnailWidth, Tile.ThumbnailHeight);
            }
        }

        public Point ThumbnailPosition
        {
            get
            {
                return new Point((int)(this.Position.X * ThumbnailToFullWidthScaleFactor),
                    (int)(this.Position.Y * ThumbnailToFullHeightScaleFactor));
            }
        }

        

        public Rectangle ThumbnailBounds
        {
            get
            {
                return new Rectangle(this.ThumbnailPosition, this.ThumbnailSize);
            }
        }

        public PointF ThumbnailCentre
        {
            get
            {
                Point pos = this.ThumbnailPosition;
                Rectangle bounds = this.ThumbnailBounds;
                return new PointF(pos.X + bounds.Width / 2.0f, pos.Y + bounds.Height / 2.0f);
            }
        }

        public static int SmallestTileWidth(List<Tile> tiles)
        {
            int smallest = tiles[0].Width;

            foreach (Tile tile in tiles)
            {
                if (tile.Width < smallest)
                    smallest = tile.Width;
            }

            return smallest;
        }

        public static int SmallestTileHeight(List<Tile> tiles)
        {
            int smallest = tiles[0].Height;

            foreach (Tile tile in tiles)
            {
                if (tile.Height < smallest)
                    smallest = tile.Height;
            }

            return smallest;
        }

        public static Tile GetMostLeftTile(List<Tile> tiles)
        { 
            Tile leftMostTile = tiles[0];

            foreach (Tile tile in tiles)
            {   
                if (tile.Position.X < leftMostTile.Position.X)
                    leftMostTile = tile;
            }

            return leftMostTile;
        }

        public static Tile GetMostRightTile(List<Tile> tiles)
        {
            Tile rightMostTile = tiles[0];

            foreach (Tile tile in tiles)
            {
                if (tile.Position.X > rightMostTile.Position.X)
                    rightMostTile = tile;
            }

            return rightMostTile;
        }

        public static Tile GetMostTopTile(List<Tile> tiles)
        {
            Tile topMostTile = tiles[0];

            foreach (Tile tile in tiles)
            {
                if (tile.Position.Y < topMostTile.Position.Y)
                    topMostTile = tile;
            }

            return topMostTile;
        }

        public static Tile GetMostBottomTile(List<Tile> tiles)
        {
            Tile bottomMostTile = tiles[0];

            foreach (Tile tile in tiles)
            {
                if (tile.Position.Y > bottomMostTile.Position.Y)
                    bottomMostTile = tile;
            }

            return bottomMostTile;
        }

        public static int GetMostLeftPosition(List<Tile> tiles)
        {
            return Tile.GetMostLeftTile(tiles).Position.X;
        }

        public static int GetMostRightPosition(List<Tile> tiles)
        {
            return Tile.GetMostRightTile(tiles).Bounds.Right;
        }

        public static int GetMostTopPosition(List<Tile> tiles)
        {
            return Tile.GetMostTopTile(tiles).Position.Y;
        }

        public static int GetMostBottomPosition(List<Tile> tiles)
        {
            return Tile.GetMostBottomTile(tiles).Bounds.Bottom;
        }

        public static int GetHorizontalRangeOfTiles(List<Tile> tiles)
        {
            return (Tile.GetMostRightPosition(tiles) - Tile.GetMostLeftPosition(tiles));
        }

        public static int GetVerticalRangeOfTiles(List<Tile> tiles)
        {
            return (Tile.GetMostBottomPosition(tiles) - Tile.GetMostTopPosition(tiles));
        }

        public static PointF GetCentreOfTiles(List<Tile> tiles)
        {
            int left = Tile.GetMostLeftPosition(tiles);
            int top = Tile.GetMostTopPosition(tiles);
            int width = Tile.GetMostRightPosition(tiles) - left;
            int height = Tile.GetMostBottomPosition(tiles) - top;

            return new PointF(left + width / 2.0f, top + height / 2.0f);
        }


        // Finds the left most and the top most position 
        // that is occupied bt the tiles. This is our top left origin.
        public static Point GetOriginOfTiles(List<Tile> tiles)
        {
            Point origin = new Point(tiles[0].Position.X, tiles[0].Position.Y);
   
            foreach (Tile tile in tiles)
            {
                if (tile.Position.X < origin.X)
                    origin.X = tile.Position.X;

                if (tile.Position.Y < origin.Y)
                    origin.Y = tile.Position.Y;
            }

            return origin;
        }

        public static List<Tile> GetTilesIntersectingRectangle(List<Tile> tiles, Rectangle rectangle)
        {
            Point origin = Tile.GetOriginOfTiles(tiles);

            List<Tile> list = new List<Tile>();

            Rectangle rect;

            foreach (Tile tile in tiles)
            {
                rect = tile.GetTileRectangleRelativeToPoint(origin);

                if (rectangle.IntersectsWith(rect))
                    list.Add(tile);
            }

            return list;
        }

        public static Region GetRegion(List<Tile> tiles)
        {
            Region region = new Region(tiles[0].Bounds);

            for (int i = 1; i < tiles.Count; i++ )
            {
                // Creates a region by forming the union of region and a tiles bounds.
                region.Union(tiles[i].Bounds);
            }

            return region;
        }
	}

    /// <summary>
    /// CircularComparer orders tile according to which ones are closest to
    /// some centraol value.
    /// </summary>
    internal class CircularComparer : IComparer<Tile>
    {
        private PointF centre;

        public CircularComparer(PointF centre)
        {
            this.centre = centre;
        }

        private float DistanceBetweenCentresMeasure(PointF centre1, PointF centre2)
        {
            return (((centre1.X - centre2.X) * (centre1.X - centre2.X)) + ((centre1.Y - centre2.Y) * (centre1.Y - centre2.Y)));
        }

        public int Compare(Tile lhs, Tile rhs)
        {
            float lhsSpacing = DistanceBetweenCentresMeasure(lhs.Centre, this.centre);
            float rhsSpacing = DistanceBetweenCentresMeasure(rhs.Centre, this.centre);

            return lhsSpacing.CompareTo(rhsSpacing);   
        }
    }
}
