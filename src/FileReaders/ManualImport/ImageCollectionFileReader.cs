using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

using FreeImageAPI;
using FreeImageIcs;

using DiagramDesigner;

using ThreadingSystem;

namespace ImageStitching
{
    /// <summary>
    /// Reads tiles from a directory which contains a sequence file.
    /// This format is outputed by the new microscopy code. It is in an ini format
    /// </summary>
    internal class ImageCollectionFileReader : TileReader
    {
        //private ThreadController imageCollectionThreadController;
        private ImageCollectionArrangerForm arranger;
        private string dataSetName;

        public ImageCollectionFileReader(string[] filePaths, MosaicWindow window)
            : base(filePaths, window)
        {
        }

        public override bool CanRead()
        {
            if (this.FilePaths.Length <= 0)
                return false;

            string extension = Path.GetExtension(this.FilePaths[0]);

            List<string> allowedExtensions = new List<string>();
            allowedExtensions.Add(".ics");
            allowedExtensions.Add(".jpg");
            allowedExtensions.Add(".jpeg");
            allowedExtensions.Add(".png");
            allowedExtensions.Add(".tif");
            allowedExtensions.Add(".tiff");
            allowedExtensions.Add(".bmp");

            if (!allowedExtensions.Contains(extension.ToLower()))  // case insensitive with ToLower
                return false;

            return true;
        }

        public override string GetCacheFilePath()
        {
            DirectoryInfo dir = new DirectoryInfo(this.FilePaths[0]);
            string prefix = this.dataSetName;
            return this.DirectoryPath + Path.DirectorySeparatorChar + prefix + ".mos";
        }

        private void DisplayThumbnails()
        {     
            foreach (string filePath in this.FilePaths)
            {
               // if (this.imageCollectionThreadController.ThreadAborted)
                //    return;

                try
                {
                    this.arranger.AddThumbail(filePath);
                }
                catch (Exception e)
                {
                    throw;
                }

                //this.threadController.ReportThreadPercentage(this, "Creating Thumbnails", count++, info.Items.Count);   
            }
        }

        protected override void ReadHeader(TileLoadInfo info)
        {
            info.OriginalPixelsPerMicron = 1.0;
            info.OverLapPercentageX = 0.0;
            info.OverLapPercentageY = 0.0;

            this.arranger = new ImageCollectionArrangerForm();

            TextStringDialog nameDialog = new TextStringDialog();

            nameDialog.Text = "Mosaic Name";
            nameDialog.Description = "Please enter a name for this Mosaic.";

            nameDialog.ShowDialog();

            this.dataSetName = nameDialog.TextString;
            info.Prefix = this.dataSetName;

            if (String.IsNullOrEmpty(this.dataSetName))
            {
                MessageBox.Show("Invalid Dataset name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Tile.IsCompositeRGB = false;
            Tile.ThumbnailHeight = 0;  // reset this so image aspect is correct

            //this.imageCollectionThreadController = new ThreadController(this.arranger);

            try
            {
                DisplayThumbnails();
            }
            catch (Exception e)
            {
                throw;
            }

            this.arranger.ShowDialog();

            List<DesignerItem> items = this.arranger.GetItemPositionsSortedFromMostTopLeft();

            float x, y, top, left;
            Point position;
            FreeImageAlgorithmsWPFImage wpfImage;

            wpfImage = items[0].Content as FreeImageAlgorithmsWPFImage;

            if (wpfImage == null)
            {
                throw new MosaicReaderException("UIElement is not a FreeImageAlgorithmsWPFImage type");
            }

            // Initialise left and top
            left = (float)DesignerCanvas.GetLeft(items[0]);
            top = (float)DesignerCanvas.GetTop(items[0]);

            foreach (DesignerItem item in items)
            {
                wpfImage = item.Content as FreeImageAlgorithmsWPFImage;

                if (wpfImage == null)
                {
                    throw new MosaicReaderException("UIElement is not a FreeImageAlgorithmsWPFImage type");
                }

                // Get position of item relative to the most top left
                x = (float)DesignerCanvas.GetLeft(item);
                y = (float)DesignerCanvas.GetTop(item);

                if (x < left)
                    left = x;

                if (y < top)
                    top = y;
            }

            // We now have the most top left position.

            foreach (DesignerItem item in items)
            {
                wpfImage = item.Content as FreeImageAlgorithmsWPFImage;

                if(wpfImage == null) 
                {
                    throw new MosaicReaderException("UIElement is not a FreeImageAlgorithmsWPFImage type");
                }

                // Get position of item relative to the most top left
                x = (float)DesignerCanvas.GetLeft(item) - left;
                y = (float)DesignerCanvas.GetTop(item) - top;

                
                position = new Point((int)x, (int)y);

                Tile tile = new Tile(wpfImage.FilePath, position, wpfImage.Fib.Width, wpfImage.Fib.Height);

                x = (int)(x / tile.ThumbnailToFullWidthScaleFactor);
                y = (int)(y / tile.ThumbnailToFullHeightScaleFactor);

                position = new Point((int)x, (int)y);

                tile.OriginalPosition = position;
                tile.ColorDepth = wpfImage.Fib.ColorDepth;
                tile.FreeImageType = wpfImage.Fib.ImageType;
                
                info.Items.Add(tile);
            }
        }
    }


}
