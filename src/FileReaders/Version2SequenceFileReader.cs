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
    /// </summary>
    internal class Version2SequenceFileReader : TileReader
    {
        private const string info_ini_name = "regionscan sequence info";
 
        public Version2SequenceFileReader(string[] filePaths, MosaicWindow window)
            : base(filePaths, window)
        {
        }

        public override bool CanRead()
        {            
            if (this.FilePaths.Length > 1)
                return false;

            string extension = Path.GetExtension(this.FilePaths[0]);

            if (extension != ".seq")
                return false;

            try {
                string version = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Version", "-1");

                if (version != "2")
                    return false;

            }
            catch
            {
                return false;
            }

            return true;
        }

        public override string GetCacheFilePath()
        {
            string prefix = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "file format", "");
            return this.DirectoryPath + Path.DirectorySeparatorChar + prefix + ".mos";
        }

        protected override void ReadHeader(TileLoadInfo info)
        {
            int count = 0;
            List<TilePosition> tilePositions = new List<TilePosition>();

            string prefix = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "file format", "");
            info.Prefix = prefix;

            string iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "roi left", "0.0");
            double roiLeft = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "roi top", "0.0");
            double roiTop = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Roi Height", "0.0");
            double roiHeight = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Horizontal Overlap", "0.0");
            // This is overlap in %
            decimal overlapX = Convert.ToDecimal(iniValue);
            info.OverLapPercentageX = (double) overlapX;

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Vertical Overlap", "0.0");
            // This is overlap in %
            decimal overlapY = Convert.ToDecimal(iniValue);
            info.OverLapPercentageY = (double) overlapY;

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Horizontal Frames", "0.0");
            info.WidthInTiles = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);
            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Vertical Frames", "0.0");
            info.HeightInTiles = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Max Intensity", "0.0");
            info.TotalMinIntensity = 0;
            info.TotalMaxIntensity = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);
            info.ScaleMinIntensity = info.TotalMinIntensity;
            info.ScaleMaxIntensity = info.TotalMaxIntensity;

            string extension = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Extension", ".ics");

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Width", "0");
            int tileWidth = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Height", "0");
            int tileHeight = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Bits Per Pixel", "0");
            info.ColorDepth = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Tile Image Type", "0");
            int tileImageType = Convert.ToInt32(iniValue, CultureInfo.InvariantCulture);

            iniValue = IniParser.IniFile.GetIniFileString(this.FilePath, info_ini_name, "Pixels Per Micron", "1.0");
            info.OriginalPixelsPerMicron = Convert.ToDouble(iniValue, CultureInfo.InvariantCulture);

            // This file format provides a list of tiles and their position relative 
            // to the left, top of the first region of interest.

            DirectoryInfo dir = new DirectoryInfo(this.DirectoryPath);

            FileInfo[] filesInDir = dir.GetFiles(prefix + "*" + extension);

            count = info.WidthInTiles * info.HeightInTiles;

            Tile[] validTiles = new Tile[count];

            Tile.IsCompositeRGB = false;

            // Check whether all the filenames will have are valid
            for (int i = 1; i <= count; i++)
            {
                string filename = BuildExpectedFilename(prefix, i, extension);
                string fullpath = this.DirectoryPath + Path.DirectorySeparatorChar + filename;

                validTiles[i - 1] = new Tile(fullpath, i, tileWidth, tileHeight);
            }

            int width = 0;
            int height = 0;
            int bpp = 8;
            FREE_IMAGE_TYPE type = FREE_IMAGE_TYPE.FIT_BITMAP;

            // Find the first tile that exists to get the sizes and color depth etc
            // but check all
            bool oneNotFound = false, atLeastOneFound = false;

            foreach (Tile tile in validTiles)
            {
                if (System.IO.File.Exists(tile.FilePath))
                {
                    if (!atLeastOneFound)
                    {
                        FreeImageAlgorithmsBitmap fib = tile.LoadFreeImageBitmap();
                        width = fib.Width;
                        height = fib.Height;
                        bpp = fib.ColorDepth;
                        type = fib.ImageType;
                        fib.Dispose();
                        atLeastOneFound = true;
                    }
                }
                else
                    oneNotFound = true;
            }

            if (!atLeastOneFound)  // No images at all!
                throw (new MosaicReaderException("No images found. Expecting prefix " + prefix + " as in seq file."));

            if (oneNotFound)
                MessageBox.Show("At least 1 image is missing from the mosaic.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            foreach (Tile tile in validTiles)
            {
                if (this.ThreadController.ThreadAborted)
                    return;

                Point position = new Point();

                int row, col;

                //   fileWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filepath);

                // Ignore files in the directory that have shorter names than we expect.
                //   if (fileWithoutExtension.Length < mosaicInfo.Fileinfo.Prefix.Length)
                //       continue;

                //  fileWithoutPrefix = fileWithoutExtension.Substring(prefix.Length);
                //   fileNumber = Version2SequenceFileReader.ParseIntStringWithZeros(fileWithoutPrefix);

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

                info.Items.Add(tile);
            }
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
