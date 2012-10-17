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
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

using FreeImageAPI;
using FreeImageIcs;

namespace ImageStitching
{
    /// <summary>
    /// Reads tiles from a directory which contains a sequence file.
    /// This format is outputed by the new microscopy code. It is in an ini format.
    /// This multi version is designed to read 3 grey mosaics and combine into one RGB mosaic.
    /// </summary>
    internal class MultiSequenceFileReader : TileReader
    {
        private const string info_ini_name = "regionscan sequence info";
        private string dataSetName;
 
        public MultiSequenceFileReader(string[] filePaths, MosaicWindow window)
            : base(filePaths, window)
        {
        }

        public override bool CanRead()
        {
            if (this.FilePaths.Length > 3)
                return false;

            if (this.FilePaths.Length < 2)
                return false;

            string extension = Path.GetExtension(this.FilePaths[0]);

            if (extension != ".seq")
                return false;

            try {
                string version = IniParser.IniFile.GetIniFileString(this.FilePaths[0], info_ini_name, "Version", "-1");
                if (version != "2")
                    return false;

                version = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Version", "-1");
                if (version != "2")
                    return false;

                if (this.FilePaths.Length == 3)
                {
                    version = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Version", "-1");
                    if (version != "2")
                        return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override string GetCacheFilePath()
        {
            return this.DirectoryPath + Path.DirectorySeparatorChar + this.dataSetName + ".mos";
        }

        protected override void ReadHeader(TileLoadInfo info)
        {
            // TODO check all seqs have the same format

            int count = 0;
            List<TilePosition> tilePositions = new List<TilePosition>();

            string prefix = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "file format", "");
            string prefix2 = "", prefix3 = "";  // may not have 3 files, but should have 2
            if (this.FilePaths.Length > 1)
            {
                prefix2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "file format", "");
                Tile.nCompositeImages = 2;
            }
            if (this.FilePaths.Length > 2)
            {
                prefix3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "file format", "");
                Tile.nCompositeImages = 3;
            }

            string iniValue, iniValue2, iniValue3;

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "roi left", "0.0");
            double roiLeft = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "roi top", "0.0");
            double roiTop = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Roi Height", "0.0");
            double roiHeight = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Horizontal Overlap", "0.0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Horizontal Overlap", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Horizontal Overlap.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Horizontal Overlap", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Horizontal Overlap.");
            }
            // This is overlap in %
            decimal overlapX = Convert.ToDecimal(iniValue);
            info.OverLapPercentageX = (double)overlapX;

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Vertical Overlap", "0.0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Vertical Overlap", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Vertical Overlap.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Vertical Overlap", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Vertical Overlap.");
            }
            // This is overlap in %
            decimal overlapY = Convert.ToDecimal(iniValue);
            info.OverLapPercentageY = (double)overlapY;

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Horizontal Frames", "0.0");
            info.WidthInTiles = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);
            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Vertical Frames", "0.0");
            info.HeightInTiles = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Max Intensity", "0.0");
            info.TotalMinIntensity = 0;
            info.TotalMaxIntensity = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);
            info.ScaleMinIntensity = info.TotalMinIntensity;
            info.ScaleMaxIntensity = info.TotalMaxIntensity;
/*            Tile.TotalMaxIntensity[0] = info.TotalMaxIntensity;
            iniValue = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Max Intensity", "0.0");
            Tile.TotalMaxIntensity[1] = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);
            if (Tile.nCompositeImages == 3)
            {
                iniValue = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Max Intensity", "0.0");
                Tile.TotalMaxIntensity[2] = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);
            }
*/
            string extension = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Extension", ".ics");
            string extension2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Extension", ".ics");
            if (extension != extension2) throw new MosaicReaderException("Seq files have different File types.");
            if (Tile.nCompositeImages == 3)
            {
                string extension3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Extension", ".ics");
                if (extension != extension3) throw new MosaicReaderException("Seq files have different File types.");
            }

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Width", "0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Tile Width", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Tile Width.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Tile Width", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Tile Width.");
            }
            int tileWidth = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Height", "0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Tile Height", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Tile Height.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Tile Height", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Tile Height.");
            }
            int tileHeight = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Bits Per Pixel", "0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Tile Bits Per Pixel", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Tile Bits Per Pixel.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Tile Bits Per Pixel", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Tile Bits Per Pixel.");
            }
            info.ColorDepth = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Image Type", "0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Tile Image Type", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Tile Image Type.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Tile Image Type", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Tile Image Type.");
            }
            int tileImageType = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Pixels Per Micron", "1.0");
            iniValue2 = IniParser.IniFile.GetIniFileString(this.FilePaths[1], info_ini_name, "Pixels Per Micron", "0.0");
            if (iniValue != iniValue2) throw new MosaicReaderException("Seq files have different Pixels Per Micron.");
            if (Tile.nCompositeImages == 3)
            {
                iniValue3 = IniParser.IniFile.GetIniFileString(this.FilePaths[2], info_ini_name, "Pixels Per Micron", "0.0");
                if (iniValue != iniValue3) throw new MosaicReaderException("Seq files have different Pixels Per Micron.");
            }
            info.OriginalPixelsPerMicron = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            // This file format provides a list of tiles and their position relative 
            // to the left, top of the first region of interest.

            // these prefix's are needed below, but let's ask for the prefix for the composite mosaic
            TextStringDialog nameDialog = new TextStringDialog();
            nameDialog.Text = "Composite Mosaic Name";
            nameDialog.Description = "Please enter a name for this composite Mosaic.";
            nameDialog.TextString = prefix + "_" + prefix2 + "_" + prefix3;
  //          if (Tile.nCompositeImages==3)
    //            nameDialog.TextString.
            nameDialog.ShowDialog();
            this.dataSetName = nameDialog.TextString;
            Tile.channelPrefix[0] = prefix;
            Tile.channelPrefix[1] = prefix2;
            Tile.channelPrefix[2] = prefix3;
            info.Prefix = this.dataSetName;

            DirectoryInfo dir = new DirectoryInfo(this.DirectoryPath);

            FileInfo[] filesInDir = dir.GetFiles(prefix + "*" + extension);

            count = info.WidthInTiles * info.HeightInTiles;

            Tile[] validTiles = new Tile[count];

            Tile.IsCompositeRGB = true;
            info.IsCompositeRGB = Tile.IsCompositeRGB;

            // Check whether all the filenames we have are valid
            for (int i = 1; i <= count; i++)
            {
                string filename = BuildExpectedFilename(prefix, i, extension);
                string fullpath = this.DirectoryPath + Path.DirectorySeparatorChar + filename;

                validTiles[i - 1] = new Tile(fullpath, i, tileWidth, tileHeight);
            }

            int width = 0;
            int height = 0;
            int bpp = 24;   // always 24 bit colour for this reader
            FREE_IMAGE_TYPE type = FREE_IMAGE_TYPE.FIT_BITMAP;
            int max_posible_intensity = 256;

            {    // pick a tile from the middle of the stack to get some info, this is a single channel
                Tile tile = validTiles[validTiles.Length / 2];
                FreeImageAlgorithmsBitmap fib = Tile.LoadFreeImageBitmapFromFile(tile.FilePath);  // use fn that loads fib directly from filename as tiles are not properly set up yet
                width = fib.Width;
                height = fib.Height;
                // bpp = fib.ColorDepth;
                // type = fib.ImageType;
                max_posible_intensity = Utilities.guessFibMaxValue(fib);
                fib.Dispose();
            }
            
            foreach (Tile tile in validTiles)
            {
                if (this.ThreadController.ThreadAborted)
                    return;

                Point position = new Point();

                int row, col;

                row = ((tile.TileNumber - 1) / info.WidthInTiles) + 1;

                if ((row % 2) > 0)
                {
                    // Odd row reversed order
                    col = ((tile.TileNumber - 1) % info.WidthInTiles) + 1;
                }
                else
                {
                    // Even row nornal order 
                    col = info.WidthInTiles - ((tile.TileNumber - 1) % info.WidthInTiles);
                }

                decimal overlapXInpixels = (overlapX / 100.0M) * (decimal)tileWidth;
                decimal overlapYInpixels = (overlapY / 100.0M) * (decimal)tileHeight;

                position.X = (int)Math.Round(((decimal)width - overlapXInpixels) * ((decimal)col - 1.0M));
                position.Y = (int)Math.Round(((decimal)height - overlapYInpixels) * ((decimal)row - 1.0M));

                tile.OriginalPosition = position;
                tile.Width = width;
                tile.Height = height;
                tile.ColorDepth = bpp;
                tile.FreeImageType = type;

                // multi seq specific stuff

                string filename = BuildExpectedFilename(prefix2, tile.TileNumber, extension);
                tile.setFileName(1, filename);

                if (Tile.nCompositeImages == 3)
                {
                    filename = BuildExpectedFilename(prefix3, tile.TileNumber, extension);
                    tile.setFileName(2, filename);
                }

                info.Items.Add(tile);
            }

            Tile.scaleMin[0] = 0.0;
            Tile.scaleMax[0] = (double)max_posible_intensity;
            Tile.scaleMin[1] = 0.0;
            Tile.scaleMax[1] = (double)max_posible_intensity;
            Tile.scaleMin[2] = 0.0;
            Tile.scaleMax[2] = (double)max_posible_intensity;

            Tile.channelShift[0] = new Point(0, 0);
            Tile.channelShift[1] = new Point(0, 0);
            Tile.channelShift[2] = new Point(0, 0);

            // check all files
            bool oneNotFound = false, atLeastOneFound = false;

            foreach (Tile tile in validTiles)
            {
                if (System.IO.File.Exists(tile.FilePath))
                {
                    if (System.IO.File.Exists(tile.getFilePath(1)))
                    {
                        if (Tile.nCompositeImages == 3)
                        {
                            if (System.IO.File.Exists(tile.getFilePath(2)))
                                atLeastOneFound = true;
                            else
                                oneNotFound = true;
                        }
                        else
                            atLeastOneFound = true;
                    }
                    else
                        oneNotFound = true;
                }
                else
                    oneNotFound = true;
            }

            if (!atLeastOneFound)  // No images at all!
                throw (new MosaicReaderException("No tiles have all channels, images missing."));

            if (oneNotFound)
                MessageBox.Show("At least 1 image is missing from the mosaic.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      
        }

        private static int ParseIntStringWithZeros(string intString)
        {
            int zeros = 0;

            foreach (char c in intString)
            {
                if (c > 0)
                    break;

                zeros++;
            }

            return int.Parse(intString.Substring(zeros), CultureInfo.InvariantCulture);
        }
    }
}
