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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using FreeImageAPI;
using ThreadingSystem;

namespace ImageStitching
{
    internal class TileNameComparer : System.Collections.Generic.IComparer<System.IO.FileInfo>
    {
        private string prefix;
 
        public string FileNamePrefix
        {
            set
            {
                this.prefix = value;
            }
        }

        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        public int Compare(System.IO.FileInfo x, System.IO.FileInfo y)
        {
            string val1_str = TileReader.ExtractTileNumber(this.prefix, x).ToString();
            string val2_str = TileReader.ExtractTileNumber(this.prefix, y).ToString();

            int val1 = Convert.ToInt32(val1_str);
            int val2 = Convert.ToInt32(val2_str);

            return val1.CompareTo(val2);
        }
    }

    internal class TileNameArrayComparer : System.Collections.IComparer
    {
        private string prefix;

        public string FileNamePrefix
        {
            set
            {
                this.prefix = value;
            }
        }

        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        public int Compare(Object x, Object y)
        {
            FileInfo a = (FileInfo) x;
            FileInfo b = (FileInfo) y;
        
            string val1_str = TileReader.ExtractTileNumber(this.prefix, a).ToString();
            string val2_str = TileReader.ExtractTileNumber(this.prefix, b).ToString();

            int val1 = Convert.ToInt32(val1_str);
            int val2 = Convert.ToInt32(val2_str);

            return val1.CompareTo(val2);
        }
    }

    internal struct TilePosition
    {
        public TilePosition(string name, int x, int y)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
        }

        public string Name;
        public int X;
        public int Y;
    }

	/// <summary>
	/// Reads tile from a directory.
	/// </summary>
    public abstract partial class TileReader : IDisposable
	{
        private delegate void ThreadExceptionMarshallerDelegate(Exception e);
   
        private TileLoadInfo info = new TileLoadInfo();
        private MosaicWindow window;
        protected bool disposed;
        private string[] filePaths;
        private ThreadController threadController;
        private bool failedToReadFormat = false;

        private List<string> allowedExtensions = new List<string>(5);

        protected TileReader(MosaicWindow window)
        {
            this.window = window;
            this.allowedExtensions.Add(".ics");
            this.allowedExtensions.Add(".bmp");
            this.allowedExtensions.Add(".png");
            this.allowedExtensions.Add(".jpg");
            this.allowedExtensions.Add(".tif");
            this.allowedExtensions.Add(".tiff");
        }

        protected TileReader(string[] filePaths,  MosaicWindow window)
            : this(window)
        {
            this.filePaths = filePaths;
        }

        ~TileReader()
        {
            // call Dispose with false.  Since we're in the
            // destructor call, the managed resources will be
            // disposed of anyways.
            Dispose(false);	
        }

        protected virtual void Dispose( bool disposing )
        {
            if(!this.disposed) 
            {
                //if (disposing)
                //{
                    // We are not in the destructor, OK to ref other objects
                          
                //}

                // Dispose of the unmanaged resources    
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            // Tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        public bool FailedToReadFormat
        {
            set
            {
                this.failedToReadFormat = value;
            }
            get
            {
                return this.failedToReadFormat;
            }
        }

        protected void ThrowTileReaderException(Exception exception)
        {
            Object[] objects = {exception};

            this.window.BeginInvoke(new ThreadExceptionMarshallerDelegate(ThreadExceoptionMarshaller), objects);
        }

        private void ThreadExceoptionMarshaller(Exception e)
        {
            throw new MosaicCacheReaderException(e.Message);
        }

        protected List<string> AllowedExtensions
        {
            get
            {
                return this.allowedExtensions;
            }
        }

        public ThreadController ThreadController
        {
            get
            {
                return this.threadController;
            }
        }

        public void SetThreadController(ThreadController threadController)
        {
            this.threadController = threadController; 
        }

        static internal string BuildExpectedFilename(string prefix, int number, string extension)
        {
            string name = String.Format(@"{0}{1}{2}", prefix, number, extension);
           
            return name;
        }

        static internal int ExtractTileNumber(string prefix, System.IO.FileInfo info)
        {
            Regex re = TileReader.GetTileFilenameRegEx(prefix, info);
            Match match = re.Match(info.ToString());

            // 2 because match.Groups[0] is entire match
            if (match.Groups.Count != 2)
                return -1;

            return Convert.ToInt32(match.Groups[1].Captures[0].Value);
        }

        static internal Regex GetTileFilenameRegEx(string prefix, FileInfo fileinfo)
        {
            string pattern = String.Format(@"{0}{1}(\d+){2}", prefix, "{1}", fileinfo.Extension);
            Regex re = new Regex(pattern);

            Match match = re.Match(fileinfo.ToString());
            return re;
        }

        static internal Regex GetTileFilenameRegEx(string prefix, string filename, string extension)
        {
            string pattern = String.Format(@"{0}{1}(\d+){2}", prefix, "{1}", extension);
            Regex re = new Regex(pattern);

            Match match = re.Match(filename);
            return re;
        }

        protected bool IsTileFilenameValid(string prefix, FileInfo fileinfo)
        {
            if (!this.AllowedExtensions.Contains(fileinfo.Extension))
                return false;

            Regex re = TileReader.GetTileFilenameRegEx(prefix, fileinfo);
            return re.IsMatch(fileinfo.ToString());
        }

        internal string[] FilePaths
        {
            get
            {
                return this.filePaths;
            }
        }

        internal string FilePath
        {
            get
            {
 //               if (this.filePaths.Length > 1)
 //                 throw new Exception("Multiple files loaded use FilePaths instead.");

                return this.filePaths[0];
            }
        }

        internal string DirectoryPath
        {
            get
            {
                return Path.GetDirectoryName(this.filePaths[0]);
            }
        }
   
        public bool HasCache()
        {
            string path = GetCacheFilePath();

            return File.Exists(path);
        }

        public abstract string GetCacheFilePath();

        internal void CreateThumbnails()
        {
            this.threadController.ThreadStart("TileReader", new ThreadStart(this.ReadThumbnails));
        }

        internal void ReCreateThumbnails()
        {
            this.threadController.ThreadStart("TileReader", new ThreadStart(this.RegenerateThumbnails));
        }


        public abstract bool CanRead();

        public void ReadHeader()
        {
            ReadHeader(this.info);
        }

        protected abstract void ReadHeader(TileLoadInfo info);

        private bool ReadMetaData(TileLoadInfo info, string filepath)
        {
            if (!File.Exists(filepath))
                return false;

            using (StreamReader file = System.IO.File.OpenText(filepath))
            {
                // Read file
                while (!file.EndOfStream)
                {
                    String line = file.ReadLine();

                    // Ignore empty lines
                    if (line.Length > 0)
                    {
                        string[] lineData = line.Split('\t');

                        info.AddMetaData(lineData[0], lineData[1]);
                    }
                }

                // Remove any silly values - Why do we export these from the scope ?
                info.MetaData.Remove("stage pos");
                info.MetaData.Remove("stage positionx");
                info.MetaData.Remove("stage positiony");
                info.MetaData.Remove("stage positionz");
            }

            return true;
        }

        internal virtual bool ReadMetaData()
        {
            info.MetaData.Clear();

            if (Tile.IsCompositeRGB)
                return AddCompositeMetaData();
            
            // MetaData is stored in a file call prefix_MetaData.txt
            string metaDataFilePath = this.DirectoryPath + Path.DirectorySeparatorChar + info.Prefix + "_MetaData.txt";

            if (this.ReadMetaData(info, metaDataFilePath) == true)
                return true;

            metaDataFilePath = info.DirectoryPath + "MetaData.txt";

            // Falied to read metadata file. Try to read a file just called "MetaData.txt"
            if (this.ReadMetaData(this.info, metaDataFilePath) == true)
                return true;

            return false;
        }

        internal bool AddCompositeMetaData()
        {
            int n = Tile.nCompositeImages;

            this.LoadInfo.AddMetaData("type", "Composite Mosaic");
            this.LoadInfo.AddMetaData("mosaic number", Tile.nCompositeImages.ToString());
            this.LoadInfo.AddMetaData("mosaic title1", Tile.channelPrefix[0]);
            this.LoadInfo.AddMetaData("mosaic title2", Tile.channelPrefix[1]);
            if (n > 2) this.LoadInfo.AddMetaData("mosaic title3", Tile.channelPrefix[2]);
            this.LoadInfo.AddMetaData("mosaic channel1", Tile.channel[0].ToString());
            this.LoadInfo.AddMetaData("mosaic channel2", Tile.channel[1].ToString());
            if (n > 2) this.LoadInfo.AddMetaData("mosaic channel3", Tile.channel[2].ToString());
            this.LoadInfo.AddMetaData("mosaic min1", Tile.scaleMin[0].ToString());
            this.LoadInfo.AddMetaData("mosaic max1", Tile.scaleMax[0].ToString());
            this.LoadInfo.AddMetaData("mosaic min2", Tile.scaleMin[1].ToString());
            this.LoadInfo.AddMetaData("mosaic max2", Tile.scaleMax[1].ToString());
            if (n > 2) this.LoadInfo.AddMetaData("mosaic min3", Tile.scaleMin[2].ToString());
            if (n > 2) this.LoadInfo.AddMetaData("mosaic max3", Tile.scaleMax[2].ToString());
            this.LoadInfo.AddMetaData("mosaic shift1.x", Tile.channelShift[0].X.ToString());
            this.LoadInfo.AddMetaData("mosaic shift1.y", Tile.channelShift[0].Y.ToString());
            this.LoadInfo.AddMetaData("mosaic shift2.x", Tile.channelShift[1].X.ToString());
            this.LoadInfo.AddMetaData("mosaic shift2.y", Tile.channelShift[1].Y.ToString());
            if (n > 2) this.LoadInfo.AddMetaData("mosaic shift3.x", Tile.channelShift[2].X.ToString());
            if (n > 2) this.LoadInfo.AddMetaData("mosaic shift3.y", Tile.channelShift[2].Y.ToString());
            return true;
        }

        internal TileReader.TileLoadInfo LoadInfo
        {
            get
            {
                return this.info;
            }
        }

        internal virtual void ReadThumbnails()
        {
            this.ReadThumbnails(this.info);
        }

        internal virtual void ReadThumbnails(TileLoadInfo info)
        {  // this may be overridden by a TileReader
            RegenerateThumbnails(info);
        }

        internal void RegenerateThumbnails()
        {
            this.RegenerateThumbnails(this.info);
            this.window.TileOverView.CreateOverview();
        }

        internal void RegenerateThumbnails(TileLoadInfo info)
        {   // this is never overidden by a TileReader
            double min = Double.MaxValue, max = Double.MinValue;
            double tmp_min = 0.0, tmp_max = 0.0;
            int count = 0;

            this.threadController.ReportThreadStarted(this, "Creating Thumbnails");

            foreach (Tile tile in info.Items)
            {
                if (this.threadController.ThreadAborted)
                    return;

                using (FreeImageAlgorithmsBitmap fib = tile.LoadFreeImageBitmap())
                {
                    Tile.ResizeToThumbnail(fib);

                    if (info.TotalMaxIntensity == 0.0)
                    {
                        // We have to find the max intensity of all the images then linear scale later
                        fib.FindMinMaxIntensity(out tmp_min, out tmp_max);

                        if (tmp_min < min)
                            min = tmp_min;

                        if (tmp_max > max)
                            max = tmp_max;

                        info.TotalMinIntensity = min;
                        info.TotalMaxIntensity = max;
                        info.ScaleMinIntensity = min;
                        info.ScaleMaxIntensity = max;
                    }

                    lock (tile.ThumbnailLock)
                    {
                        tile.Thumbnail = new FreeImageAlgorithmsBitmap(fib);

                        if (tile.Thumbnail == null)
                        {
                            MessageBox.Show("Error creating thumbnail.");
                        }

                        if (fib.IsGreyScale)
                        {
                            tile.Thumbnail.LinearScaleToStandardType(info.ScaleMinIntensity, info.ScaleMaxIntensity);
                            tile.Thumbnail.SetGreyLevelPalette();
                        }
                    }

                    this.threadController.ReportThreadPercentage(this, "Creating Thumbnails", count++, info.Items.Count);
                }
            }

            // Have to scale the bitmaps for Ros's formats
            if (info.TotalMaxIntensity != 0.0)
            {
                info.FreeImageType = info.Items[0].FreeImageType;
                info.ColorDepth = info.Items[0].ColorDepth;
            }
            else
            {

                info.ScaleMinIntensity = min;
                info.ScaleMaxIntensity = max;

                count = 0;

                info.FreeImageType = info.Items[0].FreeImageType;
                info.ColorDepth = info.Items[0].ColorDepth;
            }

            this.threadController.ReportThreadCompleted(this, "Loaded Thumbnails", false);
        }

    
    
    }
}
