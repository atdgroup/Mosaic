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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace ImageStitching
{
    public class ScalebarTool : MosaicPlugin
    {
        private const int EarLength = 10;
        private const int VerticalScalebarOffset = 32;
        private const int BoundaryWidth = 300;
        private const int BoundaryHeight = 50;
        private const int DistanceFromEdge = 50;
        private Point scalebarStartPoint;
        private Point scalebarEndPoint;
        private string text;
        private Brush brush;
        private Font font;
        private Rectangle boundary;
        private ToolStripMenuItem menuItem;

        public ScalebarTool(string name, MosaicWindow window)
            :
            base(name, window)
        {
            this.menuItem = AddMenuItem("Options", "Show Scalebar", null, new EventHandler(OnCheckedChanged));

            this.font = new Font("Arial", 12);
            this.brush = new SolidBrush(Color.Red);

            this.MosaicWindow.SizeChanged += new EventHandler(OnMosaicWindowSizeChanged);
            this.MosaicWindow.TileView.ZoomChanged += new EventHandler(OnZoomChanged);
        }

        public override bool Enabled
        {
            get
            {
                return this.menuItem.Enabled;
            }
            set
            {
                this.menuItem.Enabled = value;
            }
        }

        private void CalculateBoundary()
        {
            Point boundaryTopLeft = new Point();

            boundaryTopLeft.X = this.MosaicWindow.TileView.Width
                - ScalebarTool.BoundaryWidth - ScalebarTool.DistanceFromEdge;

            boundaryTopLeft.Y = this.MosaicWindow.TileView.Height
                - ScalebarTool.BoundaryHeight - ScalebarTool.DistanceFromEdge;

            Size size = new Size(ScalebarTool.BoundaryWidth, ScalebarTool.BoundaryHeight);

            boundary = new Rectangle(boundaryTopLeft, size);
        }

        private void CalculateScalebarSize()
        {
            int scalebarLengthInPixels;
            int scalebarLengthInMicrons;

            if (MosaicWindow.MosaicInfo == null) return;

            scalebarLengthInPixels = 200;

            double micronsPerPixel = MosaicWindow.MosaicInfo.MicronsPerPixel;

            // Work out what this is in microns
            scalebarLengthInMicrons = (int)(micronsPerPixel * scalebarLengthInPixels);

            // Round the micron value to nearest powr of ten.
            double log = Math.Log10(scalebarLengthInMicrons);
            scalebarLengthInMicrons = (int)Math.Pow(10, (int)log);

            // Work out length of scalebar
            scalebarLengthInPixels = (int)(scalebarLengthInMicrons / micronsPerPixel);

            CalculateBoundary();

            int offset = (boundary.Width - scalebarLengthInPixels) / 2;
            
            this.scalebarStartPoint = new Point(boundary.Left + offset,
                boundary.Top + ScalebarTool.VerticalScalebarOffset);

            this.scalebarEndPoint = new Point(boundary.Left + boundary.Width - offset,
                boundary.Top + ScalebarTool.VerticalScalebarOffset);

            this.text = String.Format(CultureInfo.CurrentCulture, "{0} microns", scalebarLengthInMicrons);
        }

        /*
        public Bitmap InfoBitmap
        {
            get
            {
                Bitmap bitmap = new Bitmap(this.Width, this.Height);

                // For DrawToBitmap to work the control needs to be visible.
                // To prevent the control flashing up when not visible 
                // I hide the control beneith the image view.
                // I should draw the text on the panel and not use labels I guess.
                this.SendToBack();

                bool oldVisible = this.Visible;

                if (!this.Visible)
                    this.Visible = true;

                this.DrawToBitmap(bitmap, this.ClientRectangle);

                this.Visible = oldVisible;
                this.BringToFront();

                return bitmap;
            }
        }
        */

        protected override void OnMosaicLoaded(MosaicWindowEventArgs args)
        {
            if (MosaicWindow.MosaicInfo.MetaData.Count == 0)
            {
                this.Enabled = false;
                return;
            }

            CalculateScalebarSize();
        }

        protected void OnZoomChanged(object sender, EventArgs args)
        {
            CalculateScalebarSize();
        }

        void OnMosaicWindowSizeChanged(object sender, EventArgs e)
        {
             CalculateScalebarSize();
        }

        protected override void OnSavingScreenShot(Graphics g, MosaicWindowEventArgs args)
        {
            //if (this.Active)
            //    g.DrawImage(this.infoControl.InfoBitmap, this.infoControl.Location);
        }

        protected void OnCheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked)
            {
                this.Active = true;
            }
            else
            {
                this.Active = false;
            }

            this.MosaicWindow.TileView.Invalidate();
        }

        protected override void OnViewPainted(PaintEventArgs e)
        {
            base.OnViewPainted(e);

            e.Graphics.DrawLine(new Pen(Color.Red, 3), this.scalebarStartPoint, this.scalebarEndPoint);

            e.Graphics.DrawLine(new Pen(Color.Red, 3),
                this.scalebarStartPoint.X, this.scalebarStartPoint.Y - ScalebarTool.EarLength,
                this.scalebarStartPoint.X, this.scalebarStartPoint.Y + ScalebarTool.EarLength);

            e.Graphics.DrawLine(new Pen(Color.Red, 3),
                this.scalebarEndPoint.X, this.scalebarEndPoint.Y - ScalebarTool.EarLength,
                this.scalebarEndPoint.X, this.scalebarEndPoint.Y + ScalebarTool.EarLength);
     
            SizeF size = e.Graphics.MeasureString(this.text, this.font);

            PointF scalebarTextStartPoint = new PointF();

            scalebarTextStartPoint = new PointF();

            scalebarTextStartPoint.X = this.boundary.Left + (this.boundary.Width / 2) - (size.Width / 2);
            scalebarTextStartPoint.Y = this.boundary.Bottom;
    
            e.Graphics.DrawString(text, this.font, this.brush, scalebarTextStartPoint);
        }
    }
}
