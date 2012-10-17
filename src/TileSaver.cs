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
using System.Threading;
using System.Management;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using ICSharpCode.SharpZipLib.Zip;

using FreeImageAPI;
using FreeImageIcs;
using ThreadingSystem;

namespace ImageStitching
{
	/// <summary>
	/// Save stiched images on export. Save *.mos file.
	/// </summary>
	public class TileSaver
	{
        private ThreadController threadController;
        private MosaicWindow window;
        private ZipOutputStream oZipStream = null;
        private SaveDialog saveDialog = null;
        private bool saveInfoPanel;
        private FreeImageAlgorithmsBitmap infoBitmap;
        private string filePath;

        internal TileSaver(MosaicWindow window)
        {
            this.window = window;
        }

        internal TileSaver(ThreadController threadController, MosaicWindow window)
        {
            this.threadController = threadController;
            this.window = window;
            this.filePath = null;
        }

        public MosaicWindow Window
        {
            get
            {
                return this.window;
            }
        }

        private FreeImageAlgorithmsBitmap Stitch(int stitchWidth, int stitchHeight)
		{
            float zoom = 1.0f;
            Point origin = new Point(0, 0);

            List<Tile> tiles = null;

            RoiTool roiPlugin = this.Window.GetTool("Region") as RoiTool;

            Rectangle roi = Rectangle.Empty;

            if(roiPlugin.Active == true)
                roi = roiPlugin.TransformedRegionOfInterest;

            if (roi != null && roi != Rectangle.Empty)
            {
                tiles = new List<Tile>();

                foreach (Tile tile in MosaicWindow.MosaicInfo.Items)
                {
                    if (roi.IntersectsWith(tile.Bounds))
                    {
                        tiles.Add(tile);
                    }
                }

                zoom = (float)stitchWidth / (float)(roi.Width);
                origin = roi.Location;
            }
            else
            {
                tiles = new List<Tile>(MosaicWindow.MosaicInfo.Items);
                zoom = (float)stitchWidth / (float)(MosaicWindow.MosaicInfo.TotalWidth);
            }

           // origin = Tile.GetOriginOfTiles(tiles);

            //int width = Tile.GetHorizontalRangeOfTiles(tiles);
            //int height = Tile.GetVerticalRangeOfTiles(tiles);

            int width, height;

            if (roi == Rectangle.Empty)
            {
                // Whole mosaic Width / Height
                width = MosaicWindow.MosaicInfo.TotalWidth;
                height = MosaicWindow.MosaicInfo.TotalHeight;
            }
            else
            {
                width = roi.Width;
                height = roi.Height;
            }

            FreeImageAlgorithmsBitmap section = null;

            try
            {
                section = new FreeImageAlgorithmsBitmap((int)(width * zoom), (int)(height * zoom),
                    MosaicWindow.MosaicInfo.FreeImageType, MosaicWindow.MosaicInfo.ColorDepth);
            }
            catch (FreeImageException)
            {
                return null;
            }

            FreeImageAlgorithmsBitmap tmpBitmap = null;

            int count = 1;

            foreach (Tile tile in tiles)
            {
                Point position = tile.GetTilePositionRelativeToPoint(origin);

                position.X = (int)(position.X * zoom);
                position.Y = (int)(position.Y * zoom);

                try
                {
                    tmpBitmap = tile.LoadFreeImageBitmap((int)(tile.Width * zoom), (int)(tile.Height * zoom));
                }
                catch (FreeImageException e)
                {
                    MessageBox.Show(e.Message);
                }

                section.PasteFromTopLeft(tmpBitmap, position, this.Window.BlendingEnabled);

                tmpBitmap.Dispose();

                this.threadController.ReportThreadPercentage(this, "Saving Tiles",
                    count, tiles.Count);
                
                count++;
            }

            return section;
		}

        /*
        private void GetInfoPanelBitmap()
        {
            this.infoBitmap = new FreeImageAlgorithmsBitmap();

            Graphics g = this.Window.CreateGraphics();

            this.infoBitmap.LoadFromBitmap(g.GetHdc(), this.Window.InfoControl.InfoBitmap);

            if (this.TileLoadInfo.IsGreyScale) {

                this.infoBitmap.ConvertToGreyscale();
            }
            else
                this.infoBitmap.ConvertTo24Bits();

            g.ReleaseHdc();
            g.Dispose();
        }
        */

        private int GetVideoRamInMB()
        {
            int ram = 0;

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("select AdapterRAM from Win32_VideoController");

            foreach (ManagementObject mo in searcher.Get())
            {
                PropertyData property = mo.Properties["AdapterRAM"];

                if (property != null)
                {
                    ram = ((int)property.Value / 1048576);
                }
            }

            return ram;
        }

        // Save mosaic data.
        internal void Export()
        {
            if (MosaicWindow.MosaicInfo == null)
                return;

            RoiTool roiPlugin = this.Window.GetTool("Region") as RoiTool;

            this.saveDialog = new SaveDialog(MosaicWindow.MosaicInfo, roiPlugin);
            this.saveDialog.SaveButtonClicked += new EventHandler(this.OnSaveDialogSaveButtonClicked);
            this.saveDialog.FormClosed += new FormClosedEventHandler(OnSaveDialogFormClosed);
            this.saveDialog.Show();

            this.Window.TileView.AllowMouseWheelZoom = false;
            roiPlugin.DisableUserInteraction = true;
            this.Window.EnableNonPluginToolStrip = false;
        }

        void OnSaveDialogFormClosed(object sender, FormClosedEventArgs e)
        {
            RoiTool roiPlugin = this.Window.GetTool("Region") as RoiTool;

            this.Window.TileView.AllowMouseWheelZoom = true;
            roiPlugin.DisableUserInteraction = false;
            this.Window.EnableNonPluginToolStrip = true;
        }
        
        // This is the Mos format 
        internal void SaveCacheFile()
        {
            if (MosaicWindow.MosaicInfo == null)
                return;

            this.threadController.ThreadStart("TileSaver", new ThreadStart(this.SaveMosaicCacheFile));
        }

        void OnSaveDialogSaveButtonClicked(object sender, EventArgs e)
        {
            this.filePath = this.saveDialog.FilePath;
            //this.zoom = this.saveDialog.Zoom;
            this.saveInfoPanel = this.saveDialog.SaveInfo;
            this.saveDialog.Close();

           // if (this.saveInfoPanel)
           //     this.GetInfoPanelBitmap();

            if (this.threadController != null)
                this.threadController.ThreadStart("TileSaver", new ThreadStart(this.SaveStitchedImage));
        }

        internal static void SaveMosaicHeader(MosaicInfo info, StreamWriter sw)
        {
            sw.Write("1.6\u0000");
            sw.Write(info.WidthInTiles + "\u0000");
            sw.Write(info.HeightInTiles + "\u0000"); 
            sw.Write(info.TotalWidth + "\u0000");
            sw.Write(info.TotalHeight + "\u0000");
            sw.Write(info.ColorDepth + "\u0000");
            sw.Write(info.OriginalPixelsPerMicron + "\u0000");
            sw.Write(info.OverLapPercentageX + "\u0000");
            sw.Write(info.OverLapPercentageY + "\u0000");
            sw.Write("0.0\u0000");  // dummy - was used for the overlap
            sw.Write(info.TotalMinIntensity + "\u0000");
            sw.Write(info.TotalMaxIntensity + "\u0000");
            sw.Write((int)info.FreeImageType + "\u0000");
            sw.Write(info.Items.Count + "\u0000");
            sw.Write(info.Items[0].Thumbnail.Width + "\u0000");
            sw.Write(info.Items[0].Thumbnail.Height + "\u0000");

            sw.Write(info.IsCorrelated + "\u0000\r\n");
            sw.Write(info.Prefix + "\u0000\r\n");

            foreach (Tile tile in info.Items)
            {
 /*               string filename, filename2, filename3;

                filename = tile.getFileName(0); // should always have one

                try { filename2 = tile.getFileName(1); }
                catch { filename2 = ""; }
                try { filename3 = tile.getFileName(2); }
                catch { filename3 = ""; }
   */             
                
                sw.WriteLine(String.Format(
                    "{0}\u0000{1}\u0000{2}\u0000{3}\u0000{4}\u0000{5}\u0000{6}\u0000{7}\u0000{8}\u0000{9}"
                    + "\u0000{10}\u0000{11}\u0000{12}\u0000{13}\u0000{14}",
                    tile.getFileName(0).ToString(),
                    tile.getFileName(1).ToString(),
                    tile.getFileName(2).ToString(),
                    tile.OriginalPosition.X,
                    tile.OriginalPosition.Y,
                    tile.Width,
                    tile.Height,
                    tile.ColorDepth,
                    tile.MinIntensity,
                    tile.MaxIntensity,
                    (int)tile.FreeImageType,
                    tile.IsDummy,
                    tile.AdjustedPosition.X,
                    tile.AdjustedPosition.Y,
                    tile.IsAdjusted
                    ));
            }

            // composite image stuff
            sw.Write(Tile.IsCompositeRGB + "\u0000");
            sw.Write(Tile.nCompositeImages + "\u0000");
            sw.Write((int)Tile.channel[0] + "\u0000");
            sw.Write((int)Tile.channel[1] + "\u0000");
            sw.Write((int)Tile.channel[2] + "\u0000");
            sw.Write(Tile.scaleMin[0] + "\u0000");
            sw.Write(Tile.scaleMin[1] + "\u0000");
            sw.Write(Tile.scaleMin[2] + "\u0000");
            sw.Write(Tile.scaleMax[0] + "\u0000");
            sw.Write(Tile.scaleMax[1] + "\u0000");
            sw.Write(Tile.scaleMax[2] + "\u0000");
            sw.Write(Tile.channelShift[0].X + "\u0000");
            sw.Write(Tile.channelShift[0].Y + "\u0000");
            sw.Write(Tile.channelShift[1].X + "\u0000");
            sw.Write(Tile.channelShift[1].Y + "\u0000");
            sw.Write(Tile.channelShift[2].X + "\u0000");
            sw.Write(Tile.channelShift[2].Y + "\u0000");
            sw.Write(Tile.channelPrefix[0] + "\u0000\r\n");
            sw.Write(Tile.channelPrefix[1] + "\u0000\r\n");
            sw.Write(Tile.channelPrefix[2] + "\u0000\r\n");

            sw.Flush();
        }

        // Save mosaic data.
        internal void SaveMosaicCacheFile()
        {
            if (this.threadController != null)
                this.threadController.ReportThreadStarted(this, "Saving Tiles");

            string tempFilePath = Path.GetTempFileName();

            try
            {
                this.oZipStream = new ZipOutputStream(System.IO.File.Create(tempFilePath));
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            this.oZipStream.SetLevel(2); // 9 = maximum compression level
  
            ZipEntry oZipEntry = new ZipEntry("info.txt");
            this.oZipStream.PutNextEntry(oZipEntry);

            StreamWriter sw = new StreamWriter(oZipStream);
            TileSaver.SaveMosaicHeader(MosaicWindow.MosaicInfo, sw);
            
            ThumbnailMapCollection mapCollection = new ThumbnailMapCollection(MosaicWindow.MosaicInfo.Items[0].Thumbnail.Width,
                                                MosaicWindow.MosaicInfo.Items[0].Thumbnail.Height);

            mapCollection.MapFilled += new ThumbnailMapCollectionEventHandler(OnMapFilled);

            int count = 1;
            int requiredMaps = (MosaicWindow.MosaicInfo.Items.Count / ThumbnailMap.MaxSize) + 1;

            foreach (Tile tile in MosaicWindow.MosaicInfo.Items) // For each file, generate a zipentry
            {
                if (this.threadController.ThreadAborted)
                    return;

                mapCollection.AddThumbnail(tile);

                if (this.threadController != null)
                    threadController.ReportThreadPercentage(this, "Creating Tile Maps",
                        count++, MosaicWindow.MosaicInfo.Items.Count);
            }

            // Final map may not get completly filled
            // if not then save it here

            if(mapCollection.Maps.Count <= requiredMaps) {
                ThumbnailMapCollectionEventArgs thumbnailEventArgs
                    = new ThumbnailMapCollectionEventArgs(mapCollection.Last);

                OnMapFilled(mapCollection, thumbnailEventArgs);
            }

            mapCollection.Dispose();
            oZipStream.Finish();
            oZipStream.Dispose();
            oZipStream.Close();

            try
            {
                File.Delete(MosaicWindow.MosaicInfo.TileReader.GetCacheFilePath());
                File.Copy(tempFilePath, MosaicWindow.MosaicInfo.TileReader.GetCacheFilePath());
            }
            catch (Exception)
            {

            }
            
            //File.SetAttributes(this.filePath, FileAttributes.Hidden);

            if (this.threadController != null)
                this.threadController.ReportThreadCompleted(this, "Cache Saved", false);

            GC.Collect();
        }

        private void OnMapFilled(object sender, ThumbnailMapCollectionEventArgs e)
        {
            ThumbnailMapCollection mapCollection = sender as ThumbnailMapCollection;

            ZipEntry oZipEntry = new ZipEntry("ThumbnailMap" + mapCollection.Maps.Count + ".bmp");
            this.oZipStream.PutNextEntry(oZipEntry);

            int size = mapCollection.MapSizeInBytes;

            byte[] buffer = new byte[size];
            MemoryStream ms = new MemoryStream(buffer);

            e.Map.Save(ms, FREE_IMAGE_FORMAT.FIF_BMP);

            oZipStream.Write(buffer, 0, size);

            e.Map.Dispose();
            ms.Dispose();
        }

        private void FileToLargeError()
        {
            MessageBox.Show("Unable to save file.\nThis is probably due to the file " +
                               "size being too large for your system.\n" +
                               "Try saving at a lower zoom level or in tiff format. ",
                               "File Save Error");
        }

        private void SaveStitchedImage()
        {
            if (System.IO.Path.GetExtension(this.filePath) != ".ics")
            {
                MessageBox.Show("Saving in formats other than ics may result " +
                                "in a loss of infomation.", "Warning");
            }

            if (this.threadController != null)
                this.threadController.ReportThreadStarted(this, "Started Image Export");

            FreeImageAlgorithmsBitmap stitchedImage = this.Stitch(this.saveDialog.ExportWidth,
                                                        this.saveDialog.ExportHeight);

            if (stitchedImage == null)
            {
                   this.FileToLargeError();
                   return;
            }

            try
            {
                if (System.IO.Path.GetExtension(this.filePath) != ".ics")
                {            
                    if (stitchedImage.ImageType != FREE_IMAGE_TYPE.FIT_BITMAP)                    
                        stitchedImage.ConvertToStandardType(true);  // PRB convert takes double the memory, only do it when necessary

                    if (this.saveInfoPanel)
                        stitchedImage.Paste(this.infoBitmap, new Point(20, 20), 256);

                    //stitchedImage.SaveToFile(this.filePath);  // uses many converts and clones in FIA
                    stitchedImage.Save(this.filePath);  // PRB use a simpler native free_image save

                }
                else
                {
                    // infoBitmap is 8bit for greyscale or 24bit colour
                    // If we are saving a greyscale image with > 8 bits 
                    // we need to scale the infoBitmap to the min and max
                    // possible values of the stitchedImage.
                    if (this.saveInfoPanel)
                    {
                        if (MosaicWindow.MosaicInfo.IsGreyScale && MosaicWindow.MosaicInfo.FreeImageType != this.infoBitmap.ImageType)
                        {
                            double min, max;
                            stitchedImage.FindMinMaxIntensity(out min, out max);
                            this.infoBitmap.StretchImageToType(MosaicWindow.MosaicInfo.FreeImageType, max);
                        }

                        stitchedImage.Paste(this.infoBitmap, new Point(20, 20));
                    }

                    IcsFile.SaveToFile(stitchedImage, this.filePath, true);

                    if (MosaicWindow.MosaicInfo.MetaData != null)
                    {
                        IcsFile icsFile = new IcsFile(this.filePath);

                        // We have to add the metadata to reflect the 
                        // exported image.
 //                       if(MosaicWindow.MosaicInfo.MetaData.ContainsKey("extents"))
                        {
                            string extentString = String.Format("{0:#.###e+000} {1:#.###e+000}",
                                    this.saveDialog.NativeRegionWidth * MosaicWindow.MosaicInfo.OriginalMicronsPerPixel,
                                    this.saveDialog.NativeRegionHeight * MosaicWindow.MosaicInfo.OriginalMicronsPerPixel);

                            MosaicWindow.MosaicInfo.MetaData["extents"] = extentString;
                            MosaicWindow.MosaicInfo.MetaData["units"] = "um um";
                        }

//                        if (MosaicWindow.MosaicInfo.MetaData.ContainsKey("image physical_sizex"))
                        {
                            string extentString = String.Format("{0:#.###e+000}",
                                    this.saveDialog.NativeRegionWidth * MosaicWindow.MosaicInfo.OriginalMicronsPerPixel);

                            MosaicWindow.MosaicInfo.MetaData["image physical_sizex"] = extentString;
                        }

 //                       if (MosaicWindow.MosaicInfo.MetaData.ContainsKey("image physical_sizey"))
                        {
                            string extentString = String.Format("{0:#.###e+000}",
                                    this.saveDialog.NativeRegionHeight * MosaicWindow.MosaicInfo.OriginalMicronsPerPixel);

                            MosaicWindow.MosaicInfo.MetaData["image physical_sizey"] = extentString;
                        }

//                        if (MosaicWindow.MosaicInfo.MetaData.ContainsKey("image sizex"))
                        {
                            MosaicWindow.MosaicInfo.MetaData["image sizex"] = stitchedImage.Width.ToString();
                        }

//                        if (MosaicWindow.MosaicInfo.MetaData.ContainsKey("image sizey"))
                        {
                            MosaicWindow.MosaicInfo.MetaData["image sizey"] = stitchedImage.Height.ToString();
                        }

 //                       if (MosaicWindow.MosaicInfo.MetaData.ContainsKey("dimensions"))
                        {
                            string extentString = String.Format("{0} {1}",
                                    stitchedImage.Width,
                                    stitchedImage.Height);

                            MosaicWindow.MosaicInfo.MetaData["dimensions"] = extentString;
                        }

                        // Add some metadata
                        MosaicWindow.MosaicInfo.MetaData["processed by"] = String.Format("{0} {1}", 
                            Application.ProductName, Application.ProductVersion);
                        
                        icsFile.AppendHistory(MosaicWindow.MosaicInfo.MetaData);

                        // calculate the um per pixel scale of the saved image
                        double xscale = this.saveDialog.NativeRegionWidth * MosaicWindow.MosaicInfo.OriginalMicronsPerPixel / stitchedImage.Width; 
                        double yscale = this.saveDialog.NativeRegionHeight * MosaicWindow.MosaicInfo.OriginalMicronsPerPixel / stitchedImage.Height;

                        icsFile.SetNativeScale(0, 0.0, xscale, "microns");
                        icsFile.SetNativeScale(1, 0.0, yscale, "microns");

                        icsFile.Close();
                    }
                }

                stitchedImage.Dispose();
                
                if(this.infoBitmap != null)
                    this.infoBitmap.Dispose();

                if (this.threadController != null)
                    this.threadController.ReportThreadCompleted(this, "Exported file", false);

                this.Window.TileView.AllowMouseWheelZoom = true;
                this.Window.ToolStrip.Enabled = true;
            }
            catch (FreeImageException)
            {
                this.FileToLargeError();

                stitchedImage.Dispose();

      //          this.Export();
            }
        }
	}
}
