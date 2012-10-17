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
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace ImageStitching
{
    public delegate void ImageViewMouseDelegate<S, A>(S sender, A args);

    public partial class ImageView : ScrollableControl
    {
        public event ImageViewMouseDelegate<ImageView, ImageViewMouseEventArgs>
            ImageViewMouseDownHandler;

        public event ImageViewMouseDelegate<ImageView, ImageViewMouseEventArgs>
            ImageViewMouseMoveHandler;

        private bool allowMouseWheelZoom = true;
        private Image image;
        private bool zoomToFit;
        float _zoom = 1.0f;
        private InterpolationMode interpolationMode = InterpolationMode.High;

        [Category("Appearance"), Description("The image to be displayed")]
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;

                if (this.ZoomToFit)
                    this.ZoomToFitFactor();

                UpdateScaleFactor();

                Invalidate();
            }
        }

        public bool ZoomToFit
        {
            set
            {
                this.zoomToFit = value;
            }
            get
            {
                return this.zoomToFit;
            }
        }

        private void ZoomToFitFactor()
        {
            this.Zoom = Math.Min((float)this.Width / this.image.Width, (float)this.Height / this.image.Height);
        }

        [Category("Appearance"), Description("The zoom factor. Less than 1 to reduce. More than 1 to magnify.")]
        public float Zoom
        {
            get
            {
                return _zoom; 
            }
            set
            {
                if (value < 0 || value < 0.00001)
                    value = 0.00001f;

                _zoom = value;

                UpdateScaleFactor();

                Invalidate();
            }

        }

        /// <summary>
        /// Calculates the effective size of the image 
        /// after zooming and updates the AutoScrollSize accordingly
        /// </summary>
        private void UpdateScaleFactor()
        {
            if (image == null)
            {
                this.AutoScrollMinSize = this.Size;
            }
            else
            {
                this.AutoScrollMinSize = new Size((int)(this.image.Width * _zoom + 0.5f),
                                                  (int)(this.image.Height * _zoom + 0.5f));
            }
        }

        [Category("Appearance"), Description("The interpolation mode used to smooth the drawing")]
        public InterpolationMode InterpolationMode
        {

            get
            {
                return interpolationMode;
            }
            set
            {
                interpolationMode = value;
            }
        }

        private Point CalculateImageCenter()
        {
            // Keep Image Centered
            int top = (int)((this.ClientSize.Height - (this.Image.Size.Height * this.Zoom)) / 2.0);
            int left = (int)((this.ClientSize.Width - (this.Image.Size.Width * this.Zoom)) / 2.0);

            if (top < 0)
                top = 0;

            if (left < 0)
                left = 0;

            return new Point(left, top);
        }

        protected Matrix PaintTransform()
        {
            if (this.Image == null)
                return new Matrix();

            // Set up a zoom matrix
            Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0);

            // Now translate the matrix into position for the scrollbars
            Point centerPosition = CalculateImageCenter();

            if (centerPosition.X != 0)
                mx.Translate(centerPosition.X / _zoom, 0);
            else
                mx.Translate(this.AutoScrollPosition.X / _zoom, 0);

            if (centerPosition.Y != 0)
                mx.Translate(0, centerPosition.Y / _zoom);
            else
                mx.Translate(0, this.AutoScrollPosition.Y / _zoom);

            return mx;
        }

        protected virtual void OnImageMouseDown(ImageViewMouseEventArgs e)
        {
            if (this.ImageViewMouseDownHandler != null)
            {
                ImageViewMouseDownHandler(this, e);
            }  
        }

        protected virtual void OnImageMouseMove(ImageViewMouseEventArgs e)
        {
            if (this.ImageViewMouseMoveHandler != null)
            {
                ImageViewMouseMoveHandler(this, e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.image == null)
                return;

            Matrix mx = PaintTransform();

            mx.Invert();

            Point[] points = new Point[] {e.Location};

            mx.TransformPoints(points);
           
            Rectangle rect = new Rectangle(0, 0, this.image.Width, this.image.Height);

            if (!rect.Contains(points[0]))
            {
                return;
            }

            OnImageMouseDown(new ImageViewMouseEventArgs(e.Location, points[0], e));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.image == null)
                return;

            Matrix mx = PaintTransform();

            mx.Invert();

            Point[] points = new Point[] { e.Location };

            mx.TransformPoints(points);

            Rectangle rect = new Rectangle(0, 0, this.image.Width, this.image.Height);

            if (!rect.Contains(points[0]))
            {
                return;
            }

            OnImageMouseMove(new ImageViewMouseEventArgs(e.Location, points[0], e));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // If no image, don't bother
            if (image == null)
            {
                base.OnPaintBackground(e);

                return;
            }

            Matrix mx = PaintTransform();

            e.Graphics.Transform = mx;

            e.Graphics.InterpolationMode = interpolationMode;

            //Draw the image ignoring the images resolution settings.
            e.Graphics.DrawImage(image, new Rectangle(0, 0, this.image.Width, this.image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            base.OnPaint(e);

        }

        public ImageView()
        {
            //Double buffer the control

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer, true);

            this.AutoScroll = true;
        }

        public bool AllowMouseWheelZoom
        {
            set
            {
                this.allowMouseWheelZoom = value;
            }
            get
            {
                return this.allowMouseWheelZoom;
            }
        }


        private void ScrollBy(int dx, int dy)
        {
            if (dx != 0)  // If dx == 0 we don't do anything
            {
                // Ensure dx will not be 0 by adding or subtracting 1
                if (dx > 0)
                    dx = (int)((dx / this.Zoom) + 1);
                else
                    dx = (int)((dx / this.Zoom) - 1);

                // convert scroll distance to int multiple of zoom factor. So one scroll move = 1 pixel
                if (this.Zoom > 1)
                    dx = dx * (int)this.Zoom;

                int horzValue = this.HorizontalScroll.Value + dx;

                if (horzValue < 0)
                    horzValue = 0;

                if (horzValue > this.HorizontalScroll.Maximum)
                    horzValue = this.HorizontalScroll.Maximum;

                this.HorizontalScroll.Value = horzValue;
            }

            if (dy != 0)
            {
                if (dy > 0)
                    dy = (int)((dy / this.Zoom) + 1);
                else
                    dy = (int)((dy / this.Zoom) - 1);

                // convert scroll distance to int multiple of zoom factor. So one scroll move = 1 pixel
                if (this.Zoom > 1)
                    dy = dy * (int)this.Zoom;

                int vertValue = this.VerticalScroll.Value + dy;

                if (vertValue < 0)
                    vertValue = 0;

                if (vertValue > this.VerticalScroll.Maximum)
                    vertValue = this.VerticalScroll.Maximum;

                this.VerticalScroll.Value = vertValue;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!this.AllowMouseWheelZoom)
                return;

            if (e.Delta > 0)
                this.Zoom += (0.1F * this.Zoom);
            else
                this.Zoom -= (1.0F / 11.0F * this.Zoom);

            Point point = new Point(e.X + this.AutoScrollPosition.X,
                                    e.Y + this.AutoScrollPosition.Y);

            Point centre = new Point(this.AutoScrollPosition.X + this.Width / 2, this.AutoScrollPosition.Y + this.Height / 2);
            
            int xDiff = point.X - centre.X;
            int yDiff = point.Y - centre.Y;

            this.ScrollBy(xDiff, yDiff);
        }
    }

    public class ImageViewEventArgs : EventArgs
    {
        private Point imagePosition;
        private Point clientPosition;


        public ImageViewEventArgs(Point clientPosition, Point imagePosition)
        {
            this.clientPosition = clientPosition;
            this.imagePosition = imagePosition;
        }

        public Point ImagePosition
        {
            get
            {
                return this.imagePosition;
            }
        }

        public Point ClientPosition
        {
            get
            {
                return this.clientPosition;
            }
        }
    }

    public class ImageViewMouseEventArgs : EventArgs
    {
        private MouseEventArgs mouseArgs;
        private Point imagePosition;
        private Point clientPosition;

        public ImageViewMouseEventArgs(Point clientPosition, Point imagePosition, MouseEventArgs mouseArgs)
        {
            this.mouseArgs = mouseArgs;
            this.clientPosition = clientPosition;
            this.imagePosition = imagePosition;
        }

        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return this.mouseArgs;
            }
        }

        public Point ImagePosition
        {
            get
            {
                return this.imagePosition;
            }
        }

        public Point ClientPosition
        {
            get
            {
                return this.clientPosition;
            }
        }
    }
}
