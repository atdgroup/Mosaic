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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

using FreeImageAPI;
using ThreadingSystem;

namespace ImageStitching
{
    public delegate void TileImageViewMouseDelegate<S, A>(S sender, A args);

    public class TiledImageView : ImageView
    {
        public event TileImageViewMouseDelegate<TiledImageView, TiledImageViewMouseEventArgs>
            TileImageViewMouseDownHandler;

        public event TileImageViewMouseDelegate<TiledImageView, TiledImageViewMouseEventArgs>
            TileImageViewMouseMoveHandler;

        private FreeImageAlgorithmsBitmap image;
        private MosaicInfo info;
        private float xScaleFactor;
        private float yScaleFactor;
        private bool blending;
        private bool forceGreyscale;
        private Rectangle virtualArea;

        public void Initialise(MosaicInfo info, Rectangle virtualArea, Size imageSize, bool forceGreyscale)
        {
            this.info = info;
            this.virtualArea = virtualArea;
            this.forceGreyscale = forceGreyscale;

            float aspectRatio = (float)(virtualArea.Width) / virtualArea.Height;

            if (imageSize.Width < imageSize.Height)
            {
                imageSize.Width = (int)(imageSize.Height * aspectRatio + 0.5);
            }
            else
            {
                imageSize.Height = (int)(imageSize.Width / aspectRatio + 0.5);
            }
 
            if (this.info != null)
            {
                xScaleFactor = (float)imageSize.Width / virtualArea.Width;
                yScaleFactor = (float)imageSize.Height / virtualArea.Height;

                // Set the view zoom factor so the large background image is fit to window
                // by default.
                float factor = Math.Min((float)this.Width / imageSize.Width,
                                        (float)this.Height / imageSize.Height);
                this.Zoom = factor;
            }

            if (this.forceGreyscale || info.IsGreyScale)
                this.image = new FreeImageAlgorithmsBitmap(imageSize.Width, imageSize.Height, 8);
            else
                this.image = new FreeImageAlgorithmsBitmap(imageSize.Width, imageSize.Height, 24);
        }

        public TiledImageView() {}

        public TiledImageView(MosaicInfo info, Rectangle virtualArea, Size imageSize, bool forceGreyscale)
        {
            Initialise(info, virtualArea, imageSize, forceGreyscale);
        }

        public TiledImageView(MosaicInfo info, Rectangle virtualArea, Size imageSize)
        {
            Initialise(info, virtualArea, imageSize, false);
        }

        public float XScaleFactor
        {
            get
            {
                return this.xScaleFactor;
            }
        }

        public float YScaleFactor
        {
            get
            {
                return this.yScaleFactor;
            }
        }

        public void Reset()
        {
            this.image.Clear();
            this.ReloadImage();
        }

        public bool BlendingEnabled
        {
            get
            {
                return this.blending;
            }
            set
            {
                this.blending = value;
            }
        }

        public Rectangle TranslateVirtualRectangleToImageRectangle(Rectangle virtualRect)
        {
            int width = Math.Min((int)(xScaleFactor * virtualRect.Width), virtualRect.Width);
            int height = Math.Min((int)(yScaleFactor * virtualRect.Height), virtualRect.Height);

            // Get left / top offset from top left of virtual rect
            virtualRect.Offset(-this.virtualArea.Left, -this.virtualArea.Top);

            int x = (int)(xScaleFactor * virtualRect.Left);
            int y = (int)(yScaleFactor * virtualRect.Top);

            Size size = new Size(width, height);
            Point pos = new Point(x, y);
            return new Rectangle(pos, size);
        }

        // Set the transform up that scales and translates from the virtual space to the image space.
        public Matrix GetTransform()
        {
            Matrix viewPaintMatrix = base.PaintTransform();
            Matrix matrix = new Matrix();

            matrix.Translate(-this.virtualArea.Left, -this.virtualArea.Top);

            matrix.Scale(xScaleFactor, yScaleFactor);
            matrix.Scale(this.Zoom, this.Zoom);

            matrix.Translate(viewPaintMatrix.OffsetX, viewPaintMatrix.OffsetY, MatrixOrder.Append);

            return matrix;
        }

        public void AddTile(Rectangle bounds, FreeImageAlgorithmsBitmap fib)
        {
            if(fib == null)
                return;

            try
            {
                Rectangle imageBounds = TranslateVirtualRectangleToImageRectangle(bounds);

                if (this.xScaleFactor < 1.0f || this.yScaleFactor < 1.0f)
                    fib.Rescale(imageBounds.Size, FREE_IMAGE_FILTER.FILTER_BILINEAR);

                fib.ConvertToStandardType(true);

                if (this.forceGreyscale && !fib.IsGreyScale)
                    fib.ConvertToGreyscale();

                this.image.PasteFromTopLeft(fib, imageBounds.Location, this.blending);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void AddTile(Tile tile, FreeImageAlgorithmsBitmap fib)
        {
            AddTile(tile.Bounds, fib);
        }

        public void AddTile(Tile tile)
        {
            FreeImageAlgorithmsBitmap fib = tile.LoadFreeImageBitmap();

            AddTile(tile, fib);
        }

        public void ReloadImage()
        {
            this.Image = this.image.ToBitmap();
            this.Invalidate();
        }


        protected override void OnImageMouseDown(ImageViewMouseEventArgs e)
        {
            base.OnImageMouseDown(e);

            int x = (int) ((double) e.ImagePosition.X / xScaleFactor);
            int y = (int) ((double) e.ImagePosition.Y / yScaleFactor);

            if (this.TileImageViewMouseDownHandler != null)
            {
                TileImageViewMouseDownHandler(this, new TiledImageViewMouseEventArgs(new Point(x, y), e));
            }    
        }

        protected override void OnImageMouseMove(ImageViewMouseEventArgs e)
        {
            base.OnImageMouseMove(e);

            int x = (int)((double)e.ImagePosition.X / xScaleFactor);
            int y = (int)((double)e.ImagePosition.Y / yScaleFactor);

            if (this.TileImageViewMouseDownHandler != null)
            {
                TileImageViewMouseMoveHandler(this, new TiledImageViewMouseEventArgs(new Point(x, y), e));
            }    
        }
    }

    public class TiledImageViewEventArgs : EventArgs
    {
        private Point joinedImagePosition;

        public TiledImageViewEventArgs(Point joinedImagePosition)
        {
            this.joinedImagePosition = joinedImagePosition;
        }

        public Point VirtualAreaPosition
        {
            get
            {
                return this.joinedImagePosition;
            }
        }
    }

    public class TiledImageViewMouseEventArgs : EventArgs
    {
        private ImageViewMouseEventArgs imageViewEventArgs;
        private Point joinedImagePosition;

        public TiledImageViewMouseEventArgs(Point joinedImagePosition, ImageViewMouseEventArgs mouseArgs)
        {
            this.imageViewEventArgs = mouseArgs;
            this.joinedImagePosition = joinedImagePosition;
        }

        public ImageViewMouseEventArgs ImageViewMouseEventArgs
        {
            get
            {
                return this.imageViewEventArgs;
            }
        }

        public Point VirtualAreaPosition
        {
            get
            {
                return this.joinedImagePosition;
            }
        }
    }
}
