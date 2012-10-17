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
using System.Text;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using FreeImageAPI;

namespace ImageStitching
{
    public sealed class Utilities
    {
        private Utilities() { }

        public static Point ZoomPoint(Point point, float zoom)
        {
            Point zoomedPoint = new Point(point.X, point.Y);

            zoomedPoint.X = (int)(zoomedPoint.X * zoom);
            zoomedPoint.Y = (int)(zoomedPoint.Y * zoom);

            return zoomedPoint;
        }

        public static Rectangle ZoomRectangle(Rectangle rect, float zoom)
        {
            Rectangle zoomedRect = new Rectangle(rect.Location, rect.Size);

            zoomedRect.X = (int)(zoomedRect.X * zoom);
            zoomedRect.Y = (int)(zoomedRect.Y * zoom);
            zoomedRect.Width = (int)(zoomedRect.Width * zoom);
            zoomedRect.Height = (int)(zoomedRect.Height * zoom);

            return zoomedRect;
        }

        public static string SaveDialog(bool export)
        {
            SaveFileDialog fileChooser = new SaveFileDialog();

            if(export)
                fileChooser.Filter = "Ics File (*.ics)|*.ics|Jpeg File (*.jpg)|*.jpg|Png File (*.png)|*.png|Tiff File (*.tiff)|*.tiff";
            else
                fileChooser.Filter = "Mos File (*.mos)|*.mos";

            DialogResult result = fileChooser.ShowDialog();
            string fileName;

            // Allow user to create file
            fileChooser.CheckFileExists = false;

            if(result == DialogResult.Cancel)
                return null;

            fileName = fileChooser.FileName;

            if(String.IsNullOrEmpty(fileName)) {

                MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; 
            }

            return fileName;
        }

        private static Boolean checkFileSelection(string[] files)
        {   // can have 1 mos file, or upto 3 seq files, or many image files

            if (files.Length <= 0)
                return false;  // no files

            if (files.Length == 1)
                return true;  // one file of anything is ok

            List<string> list = new List<string>(10);

            foreach (string filename in files)
            {
                list.Add(Path.GetExtension(filename));
            }
            
            if (files.Length > 1)
            {
                if (list.Contains(".mos"))
                {
                    MessageBox.Show("You can only select one *.mos file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (files.Length > 3)
            {
                if (list.Contains(".seq"))
                {
                    MessageBox.Show("You can only select upto three *.seq files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            
            return true;  // all ok
        }

        public static string[] OpenDialog()
        {  // Can select mos file or upto 3 seq files
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Title = "Open Mosaic";

            fileChooser.Filter = "Mosaic Files (*.mos)|*.mos|Sequence Files (*.seq)|*.seq";

            fileChooser.Multiselect = true;

            DialogResult result = fileChooser.ShowDialog();

            if (result == DialogResult.Cancel)
                return null;

            if (!checkFileSelection(fileChooser.FileNames))
                return null;

            return fileChooser.FileNames;
        }

        public static string[] OpenMultiSeqDialog()
        {   // this is now the same as above but just has different title
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Title = "Import seq file, or 2 or 3 Seq files for a Composite Mosaic";

            fileChooser.Filter = "Sequence Files (*.seq)|*.seq|Mosaic Files (*.mos)|*.mos";

            fileChooser.Multiselect = true;

            DialogResult result = fileChooser.ShowDialog();

            if (result == DialogResult.Cancel)
                return null;

            if (!checkFileSelection(fileChooser.FileNames))
                return null;

            return fileChooser.FileNames;
        }

        public static string[] OpenImagesDialog()
        {
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Title = "Import Image Files";

            fileChooser.Filter = "Image Files (*.ics;*.jpg;*.png;*.tiff;*.tif;*.bmp)|*.ics;*.jpg;*.png;*.tiff;*.tif;*.bmp";

            fileChooser.Multiselect = true;

            DialogResult result = fileChooser.ShowDialog();

            if (result == DialogResult.Cancel)
                return null;

            if (!checkFileSelection(fileChooser.FileNames))
                return null;

            return fileChooser.FileNames;
        }


        public static void PasteTile(FreeImageAlgorithmsBitmap dst, 
                                     FreeImageAlgorithmsBitmap src, Point location, bool blending)
        {
            if (blending)
            {
                if (!dst.GradientBlendPasteFromTopLeft(src, location))
                {
                    string errorStr = String.Format(
                        "Can not paste freeimage. Dst image bpp {0}, Src image bpp {1}",
                        dst.ColorDepth, src.ColorDepth);

                    throw new FormatException(errorStr);
                }
            }
            else
            {
                if (!dst.PasteFromTopLeft(src, location))
                {
                    string errorStr = String.Format(
                        "Can not paste freeimage. Dst image bpp {0}, Src image bpp {1}",
                        dst.ColorDepth, src.ColorDepth);

                    throw new FormatException(errorStr);
                }
            }
        }

        public static int guessFibMaxValue(FreeImageAlgorithmsBitmap fib)
        {
            uint bpp = FreeImage.GetBPP(fib.Dib);
            FREE_IMAGE_TYPE type = FreeImage.GetImageType(fib.Dib);
            double min, max;

            if (bpp == 8)
                return 256;

            if (bpp >= 24 && type == FREE_IMAGE_TYPE.FIT_BITMAP)  // colour image
                return 256;

            fib.FindMinMaxIntensity(out min, out max);

            if (max < 256)  // 8 bit
                return 256;
            if (max < 4096)  // 12 bit
                return 4096;
            if (max < 65536)  // 16 bit
                return 65536;
            
            return 100000;  // who knows!
        }
    }
}
