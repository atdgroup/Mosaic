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
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

using FreeImageAPI;
using FreeImageIcs;

using ICSharpCode.SharpZipLib.Zip;

namespace ImageStitching
{
    public class MosaicCacheReaderException : MosaicException
    {
        public MosaicCacheReaderException(string message)
            : base(message)
        {
        }

        //public MosaicCacheReaderException(Exception e, string message)
        //    : base(message)
        //{
        //    this.InnerException = e;
        //}
    }

    /// <summary>
    /// Reads tiles from a directory which contains a sequence file.
    /// </summary>
    internal class MosaicFileReader : TileReader
    {
        private ZipInputStream s;
        private StreamReader sr;
        private string[] tileData;
        private int numberOfTiles;
        private int thumbnailWidth;
        private int thumbnailHeight;

        public MosaicFileReader(string[] filePaths, MosaicWindow window)
            : base(filePaths, window)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // Dispose of the unmanaged resources   
                if (this.s != null)
                {
                    this.s.Close();
                    this.s.Dispose();
                }

                if (this.sr != null)
                {
                    this.sr.Close();
                    this.sr.Dispose();
                }

                this.disposed = true;
            }
        }

        public override bool CanRead()
        {
            if (this.FilePaths.Length > 1)
                return false;

            string extension = Path.GetExtension(this.FilePaths[0]);

            if (extension != ".mos")
                return false;

            return true;
        }

        private bool CheckOriginalDataDirectory(string directory, string firstFilePath, bool isDummy)
        {
            if (isDummy)
                return true;

            if (!Directory.Exists(directory))
                return false;

            // Ok check the first file in the directory.
            if (!File.Exists(firstFilePath))
                return false;

            return true;
        }

        public override string GetCacheFilePath()
        {
            return this.FilePath;
        }

        /*
        public override void SetPrefix(TileLoadInfo info)
        {
            this.s = new ZipInputStream(File.OpenRead(this.FilePath));
            this.sr = new StreamReader(s);
            this.s.GetNextEntry(); // Get the first entry ie the info file.

            string text = sr.ReadToEnd();

            this.tileData = Regex.Split(text, "\r\n");
            string[] line = this.tileData[0].Split('\u0000');

            // Get the prefix of the tiles
            info.Prefix = this.tileData[1].Split('\u0000')[0];

            this.s.Close();
            this.sr.Close();
        }
        */

        protected override void ReadHeader(TileLoadInfo info)
        {
            try
            {
                this.s = new ZipInputStream(File.OpenRead(this.FilePath));
                this.sr = new StreamReader(s);
                this.s.GetNextEntry(); // Get the first entry ie the info file.

                string text = sr.ReadToEnd();
                double version = 1.0;

                this.tileData = Regex.Split(text, "\r\n");
                string[] line = this.tileData[0].Split('\u0000');
                int j = 0;  // version number at start
                version = Convert.ToDouble(line[j]); j++;
                info.WidthInTiles = Convert.ToInt32(line[j]); j++;
                info.HeightInTiles = Convert.ToInt32(line[j]); j++;
                info.TotalWidth = Convert.ToInt32(line[j]); j++;
                info.TotalHeight = Convert.ToInt32(line[j]); j++;
                info.ColorDepth = Convert.ToInt32(line[j]); j++;
                info.OriginalPixelsPerMicron = Convert.ToDouble(line[j]); j++;
                info.OverLapPercentageX = Convert.ToDouble(line[j]); j++;
                info.OverLapPercentageY = Convert.ToDouble(line[j]); j++;
                double dummyOverlap = Convert.ToDouble(line[j]); j++;
                info.TotalMinIntensity = Convert.ToDouble(line[j]); j++;
                info.TotalMaxIntensity = Convert.ToDouble(line[j]); j++;
                info.ScaleMinIntensity = info.TotalMinIntensity;
                info.ScaleMaxIntensity = info.TotalMaxIntensity;
                info.FreeImageType = (FREE_IMAGE_TYPE)Convert.ToInt32(line[j]); j++;
                this.numberOfTiles = Convert.ToInt32(line[j]); j++;
                this.thumbnailWidth = Convert.ToInt32(line[j]); j++;
                this.thumbnailHeight = Convert.ToInt32(line[j]); j++;
                info.IsCorrelated = Convert.ToBoolean(line[j]); j++;

                info.Prefix = this.tileData[1].Split('\u0000')[0];

                string filepath, filename, filename2="", filename3="";

                int i = 1;

                // Read the thumbnails
                while (i <= this.numberOfTiles)
                {
                    if (this.ThreadController.ThreadAborted)
                        return;

                    line = this.tileData[i + 1].Split('\u0000');
                    j = 0; 
                    filename = line[j].ToString(); j++;
                    if (version >= 1.5)  // composite images
                    {
                        filename2 = line[j].ToString(); j++;
                        filename3 = line[j].ToString(); j++;
                    }
                    filepath = this.DirectoryPath + Path.DirectorySeparatorChar + filename;

                    int x = Convert.ToInt32(line[j]); j++;
                    int y = Convert.ToInt32(line[j]); j++;
                    int width = Convert.ToInt32(line[j]); j++;
                    int height = Convert.ToInt32(line[j]); j++;
                    int bpp = Convert.ToInt32(line[j]); j++;
                    int minIntensity = Convert.ToInt32(line[j]); j++;
                    int maxIntensity = Convert.ToInt32(line[j]); j++;
                    int type = Convert.ToInt32(line[j]); j++;
                    bool isDummy = Convert.ToBoolean(line[j]); j++;
                    int correlatedx = Convert.ToInt32(line[j]); j++;
                    int correlatedy = Convert.ToInt32(line[j]); j++;
                    bool isCorrelated = Convert.ToBoolean(line[j]); j++;

                    Point position = new Point(x, y);
                    Point correlatedPosition = new Point(correlatedx, correlatedy);

                    Tile tile = new Tile(filepath, position, width, height);

                    tile.ColorDepth = bpp;
                    tile.FreeImageType = (FREE_IMAGE_TYPE)type;
                    tile.MinIntensity = minIntensity;
                    tile.MaxIntensity = maxIntensity;
                    tile.IsDummy = isDummy;
                    tile.AdjustedPosition = correlatedPosition;
                    tile.IsAdjusted = isCorrelated;
                    if (version >= 1.5)  // composite images
                    {
                        tile.setFileName(1, filename2);
                        tile.setFileName(2, filename3);
                    }

                    info.Items.Add(tile);

                    i++;
                }

                if (version >= 1.5) // composite images
                {
                    line = this.tileData[i + 1].Split('\u0000'); i++;
                    j = 0;
                    Tile.IsCompositeRGB = Convert.ToBoolean(line[j]); j++;
                    info.IsCompositeRGB = Tile.IsCompositeRGB;
                    Tile.nCompositeImages = Convert.ToInt32(line[j]); j++;
                    Tile.channel[0] = (FREE_IMAGE_COLOR_CHANNEL)Convert.ToInt32(line[j]); j++;
                    Tile.channel[1] = (FREE_IMAGE_COLOR_CHANNEL)Convert.ToInt32(line[j]); j++;
                    Tile.channel[2] = (FREE_IMAGE_COLOR_CHANNEL)Convert.ToInt32(line[j]); j++;
                    Tile.scaleMin[0] = Convert.ToDouble(line[j]); j++;
                    Tile.scaleMin[1] = Convert.ToDouble(line[j]); j++;
                    Tile.scaleMin[2] = Convert.ToDouble(line[j]); j++;
                    Tile.scaleMax[0] = Convert.ToDouble(line[j]); j++;
                    Tile.scaleMax[1] = Convert.ToDouble(line[j]); j++;
                    Tile.scaleMax[2] = Convert.ToDouble(line[j]); j++;
                    if (version >= 1.6)  // composite rgb channel shifts
                    {
                        Tile.channelShift[0].X = Convert.ToInt32(line[j]); j++;
                        Tile.channelShift[0].Y = Convert.ToInt32(line[j]); j++;
                        Tile.channelShift[1].X = Convert.ToInt32(line[j]); j++;
                        Tile.channelShift[1].Y = Convert.ToInt32(line[j]); j++;
                        Tile.channelShift[2].X = Convert.ToInt32(line[j]); j++;
                        Tile.channelShift[2].Y = Convert.ToInt32(line[j]); j++;
                    }
                    Tile.channelPrefix[0] = this.tileData[i + 1].Split('\u0000')[0]; i++;
                    Tile.channelPrefix[1] = this.tileData[i + 1].Split('\u0000')[0]; i++;
                    Tile.channelPrefix[2] = this.tileData[i + 1].Split('\u0000')[0]; i++;
                }
                else
                {
                    Tile.IsCompositeRGB = false;
                    info.IsCompositeRGB = false;
                }

                this.s.Close();
                this.sr.Close();
            }
            catch (System.FormatException)
            {
                // probably an old mos file
                // just continue to 'finally' section
                MessageBox.Show("Old mos file imported.");
            }
            catch (Exception e)
            {
                throw new MosaicCacheReaderException("Failed to read cache file format: " + e.Message);
            }
            finally
            {
                this.s.Close();
                this.sr.Close();
            }
        }

        internal override void ReadThumbnails(TileLoadInfo info)
        {
            this.ThreadController.ReportThreadStarted(this, "Loading files");

            ZipEntry zipEntry;

            int totalCount = 0;
            int count = 0;

            ThumbnailMapCollection mapCollection =
                new ThumbnailMapCollection(this.thumbnailWidth, this.thumbnailHeight);

            this.s.Close();
            this.s = new ZipInputStream(File.OpenRead(this.FilePath));

            totalCount = 0;

            try
            {

                // First go through the zip file and get the image maps
                while ((zipEntry = this.s.GetNextEntry()) != null)
                {
                    if (zipEntry.Name.StartsWith("Thumbnail"))
                    {
                        if (this.ThreadController.ThreadAborted)
                            return;

                        count = 0;

                        ThumbnailMap map = new ThumbnailMap(this.thumbnailWidth, this.thumbnailHeight, this.s);

                        Tile tile = null;

                        for (int i = 0; i < map.MapSizeInTiles && totalCount < MosaicWindow.MosaicInfo.Items.Count; i++)
                        {
                            tile = MosaicWindow.MosaicInfo.Items[totalCount];

                            FreeImageAlgorithmsBitmap fib = map.GetImage(count);
                            tile.Thumbnail = fib;

                            if (tile.Thumbnail == null)
                            {
                                throw new Exception("Failed to retrive tumbnail from map");
                            }

                            this.ThreadController.ReportThreadPercentage(this, "Creating Thumbnails",
                                totalCount++, MosaicWindow.MosaicInfo.Items.Count);

                            count++;
                        }

                        map.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                FailedToReadFormat = true;
                this.ThreadController.ReportThreadCompleted(this, "Loaded file", false);
                return;
            }

            this.s.Close();
            this.sr.Close();

            info.FreeImageType = MosaicWindow.MosaicInfo.Items[0].FreeImageType;
            info.ColorDepth = MosaicWindow.MosaicInfo.Items[0].ColorDepth;

            this.ThreadController.ReportThreadCompleted(this, "Loaded file", false);
        }
    }
}
