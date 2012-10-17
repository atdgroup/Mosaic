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
	internal class SequenceFileReader : TileReader
	{
        private List<TilePosition> tilePositions = new List<TilePosition>();

        public SequenceFileReader(string[] filePaths, MosaicWindow window)
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
                    if (count++ > 5)
                        return false;
                }
            }

            return true;
        }

        public override string GetCacheFilePath()
        {
            string prefix = System.IO.Path.GetFileNameWithoutExtension(this.FilePath);
            return this.DirectoryPath + Path.DirectorySeparatorChar + prefix + ".mos";
        }

        protected override void ReadHeader(TileLoadInfo info)
        {
            int count = 0;
            string extension = null;
            decimal OverLapMicrons=0M;

            info.Prefix = System.IO.Path.GetFileNameWithoutExtension(this.FilePath);
            info.DirectoryPath = System.IO.Path.GetDirectoryName(this.FilePath);

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

                    // Get the overlap in microns
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

                    // Get the extension of the files
                    extension = sr.ReadLine();
                }
                catch (IOException e)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot parse file: " + e.Message);
                }
            }

            DirectoryInfo dir = new DirectoryInfo(this.DirectoryPath);
            FileInfo[] filesInDir = dir.GetFiles(info.Prefix + "*" + extension);

            count = info.WidthInTiles * info.HeightInTiles;

            Tile[] validTiles = new Tile[count];

            Tile.IsCompositeRGB = false;

            // Check whether all the filenames will have are valid
            for (int i = 1; i <= count; i++)
            {
                string filename = BuildExpectedFilename(info.Prefix, i, extension);
                string fullpath = this.DirectoryPath + System.IO.Path.DirectorySeparatorChar + filename;

                validTiles[i - 1] = new Tile(fullpath, i, 0, 0);
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

                // The original seq spec only allowed for one overlap in microns (not X and Y)
                decimal overlapXInpixels = ((decimal)info.OverLapPercentageX / 100.0M) * (decimal)width;
                decimal overlapYInpixels = ((decimal)info.OverLapPercentageY / 100.0M) * (decimal)height;

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

            foreach(char c in intString) 
            {
                if( c > 0 )
                    break;
				
                zeros++;
            }

            return int.Parse(intString.Substring(zeros), CultureInfo.InvariantCulture);
        }
	}
}
