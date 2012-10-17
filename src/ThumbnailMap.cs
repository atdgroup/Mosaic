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
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;

using FreeImageAPI;

namespace ImageStitching
{
    // Save and Retrieve thumbnails from a larger image.
    // Thumbnails are stored top to bottom / left to right.
    public class ThumbnailMap : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")] 
        extern static public int BitBlt(IntPtr dest, int xdest, int ydest, int width, int height, IntPtr src, int xsrc, int ysrc, uint op); 

        [System.Runtime.InteropServices.DllImport("user32.dll")] 
        internal static extern IntPtr GetDC(IntPtr hWnd); 

        [System.Runtime.InteropServices.DllImport("user32.dll")] 
        internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        private bool disposed;
        private bool colour = false;
        private const int widthMax = 10;
        private const int heightMax = 10;
        private int count = 0;
        private int thumbnailWidth = 0;
        private int thumbnailHeight = 0;
        private int lastPosition = 0;
        private FreeImageAlgorithmsBitmap bmp;
        public const int MaxSize = ThumbnailMap.widthMax * ThumbnailMap.heightMax;

        public ThumbnailMap(int thumbnailWidth, int thumbnailHeight, bool colour)
        {
            this.thumbnailWidth = thumbnailWidth;
            this.thumbnailHeight = thumbnailHeight;

            // Create Large Bitmap to hold all the thumbnals
            this.colour = colour;

            int bpp = 8;

            if(this.colour) {
                bpp = 24;
            }

            this.bmp = new FreeImageAlgorithmsBitmap(thumbnailWidth * ThumbnailMap.widthMax, thumbnailHeight * ThumbnailMap.heightMax, bpp);

            if (bpp == 8)
                this.bmp.SetGreyLevelPalette();
        }

        public ThumbnailMap(int thumbnailWidth, int thumbnailHeight, Stream imageStream)
        {
            this.thumbnailWidth = thumbnailWidth;
            this.thumbnailHeight = thumbnailHeight;

            // Create Large Bitmap to hold all the thumbnals
            this.bmp = new FreeImageAlgorithmsBitmap(imageStream, FREE_IMAGE_FORMAT.FIF_BMP);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.bmp.Dispose();

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

        private Point RealPosition(int position)
        {
            Point pt = new Point();
            int x = position % ThumbnailMap.widthMax;
            int y = position / ThumbnailMap.widthMax;

            pt.X = x * this.thumbnailWidth;
            pt.Y = y * this.thumbnailHeight;

            return pt;
        }

        public void AddImage(FreeImageAlgorithmsBitmap image)
        {
            //Graphics g = Graphics.FromImage(this.bmp);

            if (this.thumbnailWidth != 0 && this.thumbnailWidth != image.Width
                && this.thumbnailHeight != image.Height)
            {
                throw new MosaicException("Thumbnails must all be the same size");
            }

            this.thumbnailWidth = image.Width;
            this.thumbnailHeight = image.Height;

            Point pt = this.RealPosition(this.lastPosition++);

            this.bmp.PasteFromTopLeft(image, pt);

            this.count++;
        }

        public FreeImageAlgorithmsBitmap GetImage(int position)
        {
            Point pt = this.RealPosition(position);

            FreeImageAlgorithmsBitmap fib = this.bmp.Copy(pt.X, pt.Y, pt.X + this.thumbnailWidth - 1, pt.Y + this.thumbnailHeight - 1);

            return fib;
        }

        public int MapSizeInTiles
        {
            get
            {
                return ThumbnailMap.widthMax * ThumbnailMap.heightMax;
            }
        }

        public int MapSizeInBytes
        {
            get
            {
                return Image.GetPixelFormatSize(this.bmp.PixelFormat) * this.bmp.Width * this.bmp.Height;
            }
        }

        public bool Full
        {
            get
            {
                return this.count >= ThumbnailMap.MaxSize;
            }
        }

        public void Save(Stream stream, FREE_IMAGE_FORMAT format)
        {
            this.bmp.Save(stream, format);
        }
    }

    public class ThumbnailMapCollectionEventArgs : EventArgs
    {
        private ThumbnailMap map;

        public ThumbnailMapCollectionEventArgs(ThumbnailMap map)
        {
            this.map = map;
        }

        public ThumbnailMap Map
        {
            get
            {
                return this.map;
            }
        }
    }

    public delegate void ThumbnailMapCollectionEventHandler(object sender, ThumbnailMapCollectionEventArgs e);    

    /// <summary>
    /// This class is for placing thumbnails into maps (Images of 100x100 thumbnails)
    /// To reduce disk access times.
    /// </summary>
    public class ThumbnailMapCollection : IDisposable
    {  
        private bool disposed;
        List<ThumbnailMap> maps;
        private int thumbnailWidth = 0;
        private int thumbnailHeight = 0;

        public event ThumbnailMapCollectionEventHandler MapFilled;
        public event ThumbnailMapCollectionEventHandler MapCreated;

        public ThumbnailMapCollection(int thumbnailWidth, int thumbnailHeight)
        {
            this.thumbnailWidth = thumbnailWidth;
            this.thumbnailHeight = thumbnailHeight;

            this.maps = new List<ThumbnailMap>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                //if (disposing)
                //{
                // We are not in the destructor, OK to ref other objects

                //}

                foreach(ThumbnailMap map in maps) {
                    map.Dispose();
                }

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

        // Invoke the NewMapCreated event; called whenever list changes
        protected virtual void OnMapFilled(ThumbnailMapCollectionEventArgs e)
        {
            if (MapFilled != null)
                MapFilled(this, e);
        }

        protected virtual void OnMapCreated(ThumbnailMapCollectionEventArgs e)
        {
            if (MapCreated != null)
                MapCreated(this, e);
        }


        public ThumbnailMap Last
        {
            get
            {
                return this.maps[this.maps.Count - 1];
            }
        }

        public void AddMap(ThumbnailMap map)
        {
            this.maps.Add(map);
        }

        public List<ThumbnailMap> Maps
        {
            get
            {
                return this.maps;
            }
        }

        public int MapSizeInBytes
        {
            get
            {
                return this.Last.MapSizeInBytes;
            }
        }

        public void AddThumbnail(Tile tile)
        {
            if (this.maps.Count == 0 || this.Last.Full)
            {
                bool colour = false;
        
                if (tile.FreeImageType == FREE_IMAGE_TYPE.FIT_BITMAP && tile.ColorDepth >= 24)
                    colour = true;

                if (this.maps.Count != 0)
                    OnMapFilled(new ThumbnailMapCollectionEventArgs(this.Last));

                // Create a new Map Image and add it to the list
                ThumbnailMap map = new ThumbnailMap(this.thumbnailWidth, this.thumbnailHeight, colour);
                this.maps.Add(map);
                OnMapCreated(new ThumbnailMapCollectionEventArgs(map));
            }

            // Add the thumbnail to the MapImage
            if(tile.Thumbnail != null)
                this.Last.AddImage(tile.Thumbnail);
        }

        public FreeImageAlgorithmsBitmap GetImage(int position)
        {
            // Find what map the thumbnail is in.
            int mapNumber = position / ThumbnailMap.MaxSize;

            // Work out the real position.
            return this.maps[mapNumber].GetImage(mapNumber);
        }
    }
}
