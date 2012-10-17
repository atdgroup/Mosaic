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
    /// </summary>
    internal class RosMosaicSequenceFileReader : TileReader
    {
        public RosMosaicSequenceFileReader(string[] filePaths, MosaicWindow window)
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

            // Try to read first line
            // The first line is four doubles \t seperated
            StreamReader sr = new StreamReader(this.FilePath);

            string line = sr.ReadLine();

            if (line == null)
                return false;

            string[] fields = line.Split(new char[] { ' ', '\t' });

            if (fields.Length != 4)
                return false;

            using (sr = new StreamReader(this.FilePath))
            {
                int count = 0;

                // Read extents but don't use
                while (sr.ReadLine() != null)
                {
                    count++;
                }

                if (count < 5)
                    return false;
            }

            return true;
        }

        public override string GetCacheFilePath()
        {
            DirectoryInfo dir = new DirectoryInfo(this.FilePath);
            string prefix = System.IO.Path.GetFileNameWithoutExtension(this.FilePath);
            return this.DirectoryPath + Path.DirectorySeparatorChar + prefix + ".mos";
        }

        protected override void ReadHeader(TileLoadInfo info)
        {
            int count = 0;
            decimal OverLapMicrons = 0M;

            FileInfo[] filesInDir = null;
            List<TilePosition> tilePositions = new List<TilePosition>();
    
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(this.FilePath))
            {
                String line;
                string[] fields;

                try
                {
                    // Read extents but don't use
                    line = sr.ReadLine();

                    // Get the overlap
                    line = sr.ReadLine();
                    OverLapMicrons = Convert.ToDecimal(line);

                    // Get the number of frames in each direction
                    line = sr.ReadLine();
                    fields = line.Split('\t');

                    info.WidthInTiles = Convert.ToInt32(fields[0], CultureInfo.InvariantCulture);
                    info.HeightInTiles = Convert.ToInt32(fields[1], CultureInfo.InvariantCulture);
                 
                    // Get the microns per pixel factor
                    line = sr.ReadLine();
                    info.OriginalPixelsPerMicron = Convert.ToDouble(line, CultureInfo.InvariantCulture);

                    // By definition Ros's TileLoadInfo files consist of correlated data.
                    info.IsCorrelated = true;

                    // Get the extension of the files
                    line = sr.ReadLine();

                    filesInDir = new FileInfo[info.NumberOfTiles];
                    count = 0;

                    while ((line = sr.ReadLine()) != null)
                    {
                        fields = line.Split('\t');

                        tilePositions.Add(new TilePosition(fields[0],
                            Convert.ToInt32(fields[1], CultureInfo.InvariantCulture),
                            Convert.ToInt32(fields[2], CultureInfo.InvariantCulture)));

                        filesInDir[count++] = new FileInfo(this.DirectoryPath
                            + "\\" + fields[0]);
                    }
                }
                catch (IOException e)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot parse file: " + e.Message);
                }
            }

            Tile.IsCompositeRGB = false;

            count = 0;
            int width = 0;
            int height = 0;
            int bpp = 8;
            FREE_IMAGE_TYPE type = FREE_IMAGE_TYPE.FIT_BITMAP;

            // Find the first tile that exists to get the sizes and color depth etc
            // but check all
            bool oneNotFound = false, atLeastOneFound = false;

            foreach (FileInfo file in filesInDir)
            {
                if (System.IO.File.Exists(file.FullName))
                {
                    if (!atLeastOneFound)
                    {
                        FreeImageAlgorithmsBitmap fib = Tile.LoadFreeImageBitmapFromFile(file.FullName);
                        width = fib.Width;
                        height = fib.Height;
                        bpp = fib.ColorDepth;
                        type = fib.ImageType;
                        fib.Dispose();

                        // Set the overlap percentage
                        double widthInMicrions = width / info.OriginalPixelsPerMicron;
                        double heightInMicrions = height / info.OriginalPixelsPerMicron;

                        info.OverLapPercentageX =
                            (double)((double)OverLapMicrons / widthInMicrions) * 100.0;

                        info.OverLapPercentageY =
                            (double)((double)OverLapMicrons / heightInMicrions) * 100.0;

                        atLeastOneFound = true;
                    }
                }
                else
                    oneNotFound = true;
            }

            if (!atLeastOneFound)  // No images at all!
                throw (new MosaicReaderException("No images found."));

            if (oneNotFound)
                MessageBox.Show("At least 1 image is missing from the mosaic.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            foreach (FileInfo file in filesInDir)
            {
                if (this.ThreadController.ThreadAborted)
                    return;

                if (!this.AllowedExtensions.Contains(file.Extension))
                    continue;

                Point position = new Point();

                position.X = tilePositions[count].X;
                position.Y = tilePositions[count].Y;

                Tile tile = new Tile(file.FullName, position, width, height);

                tile.Width = width;
                tile.Height = height;
                tile.ColorDepth = bpp;
                tile.FreeImageType = (FREE_IMAGE_TYPE) type;

                info.Items.Add(tile);
                
                count++;
            }
        }  
    }
}
