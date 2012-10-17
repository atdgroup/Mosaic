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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using GrayLabs.Windows.Forms;

using FreeImageAPI;
using ThreadingSystem;

namespace ImageStitching
{
    public delegate void TileOverViewHandler<S, A>(S sender, A args);

    /// <summary>
    /// Summary description for TileViewer.
    /// </summary>
    internal class TileOverView : System.Windows.Forms.UserControl
    {
        public event TileOverViewHandler<TileOverView, TileOverViewEventArgs>
            TileOverViewCompletedLoadHandler;

        private MosaicWindow window;
        private ThreadController threadController;
        private int scaledWidth;
        private int scaledHeight;
        private FreeImageAlgorithmsBitmap scaledImage;
        private TileView imageViewer;
        private FreeImageAlgorithmsBitmap bitmap;
        private volatile bool abort = false;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public TileOverView()
        {

        }

        public void SetMosaicWindow(MosaicWindow window)
        {
            this.window = window;
            this.imageViewer = window.TileView;
     
            InitializeComponent();

            this.SetStyle(
                                ControlStyles.AllPaintingInWmPaint |
                                ControlStyles.UserPaint |
                                ControlStyles.ResizeRedraw |
                                ControlStyles.Opaque |
                                ControlStyles.DoubleBuffer, true);

            this.imageViewer.ZoomChanged += new EventHandler(OnZoomChanged);
            this.imageViewer.VertScrollChanged += new ScrollEventHandler(OnVerticalScrollChanged);
            this.imageViewer.HorzScrollChanged += new ScrollEventHandler(OnHorizontalScrollChanged);
            this.imageViewer.PanningOccurred += new TileViewHandler<TileView, EventArgs>(OnPanningOccurred);
            this.imageViewer.SizeChanged += new EventHandler(OnImageViewSizeChanged);
            this.imageViewer.OnScrollChanged += new ScrollEventHandler(OnScrollChanged);

            this.window.BlendingChanged += 
                new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnBlendingChanged);

            this.Paint += new PaintEventHandler(OnPaint);

            threadController = new ThreadController(this);
            threadController.SetThreadCompletedCallback(TileOverviewThreadCompleted);
        }

        ~TileOverView()
        {
        }

        public void AbortOverviewCreation()
        {
            this.abort = true;
        }

        private void ThreadCreateOverview()
        {
            List<Tile> items = new List<Tile>(MosaicWindow.MosaicInfo.Items);

            while (items.Count > 0)
            {
                if(abort)
                    return;

                foreach (Tile tile in items)
                {
                    if (tile.Thumbnail != null)
                    {
                        TileImage(tile);
                        items.Remove(tile);
                        break;
                    }
                }

                Thread.Sleep(5);
            }

            if (this.threadController != null)
                this.threadController.ReportThreadCompleted(this, "TileOverview Completed", false);
        }

        private void TileOverviewThreadCompleted(object sender, string updateText, bool aborted)
        {
            OnTileOverViewCompletedLoadHandler(this, new TileOverViewEventArgs());   
        }

        private void OnImageViewSizeChanged(object sender, EventArgs e)
        {
            if (this.imageViewer != null)
                this.Invalidate();
        }

        private void OnZoomChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        protected void OnTileOverViewCompletedLoadHandler(TileOverView sender,
                                               TileOverViewEventArgs args)
        {
            if (TileOverViewCompletedLoadHandler != null)
                TileOverViewCompletedLoadHandler(this, args);
        }

        public void CreateOverview()
        {
            float aspectRatio = (float)(MosaicWindow.MosaicInfo.TotalWidth) / MosaicWindow.MosaicInfo.TotalHeight;

            // Largest dimension should be 300 pixels
            if(MosaicWindow.MosaicInfo.TotalWidth >= (MosaicWindow.MosaicInfo.TotalHeight)) {
                this.scaledWidth = Math.Min(MosaicWindow.MosaicInfo.TotalWidth, 300);
                this.scaledHeight = (int)(this.scaledWidth / aspectRatio + 0.5);
            }
            else {
                this.scaledHeight = Math.Min(MosaicWindow.MosaicInfo.TotalHeight, 300);
                this.scaledWidth = (int)(this.scaledHeight * aspectRatio + 0.5);
            }

            this.scaledImage = new FreeImageAlgorithmsBitmap(this.scaledWidth, this.scaledHeight,
                MosaicWindow.MosaicInfo.ColorDepth);

      //      int xBorderSize = this.Size.Width - this.ClientRectangle.Width;
      //      int yBorderSize = this.Size.Height - this.ClientRectangle.Height;

            this.bitmap = new FreeImageAlgorithmsBitmap(this.scaledWidth, this.scaledHeight, 24);

            this.Size = new Size(this.scaledWidth, this.scaledHeight);

            this.threadController.ThreadStart("TileOverview", new ThreadStart(this.ThreadCreateOverview));
        }

        void OnBlendingChanged(MosaicWindow sender, MosaicWindowEventArgs args)
        {
            CreateOverview();
        }

        public void TileImage(Tile tile)
        {
            lock (tile)
            {
                if (tile.Thumbnail == null)
                    return;

                float xscaleFactor = (float)this.scaledWidth / MosaicWindow.MosaicInfo.TotalWidth;
                float yscaleFactor = (float)this.scaledHeight / MosaicWindow.MosaicInfo.TotalHeight;

                Size scaledSize = new Size((int)(tile.Width * xscaleFactor + 0.5),
                                           (int)(tile.Height * yscaleFactor + 0.5));

                Point scaledPosition = new Point();
                scaledPosition.X = (int)(tile.Position.X * xscaleFactor + 0.5);
                scaledPosition.Y = (int)(tile.Position.Y * yscaleFactor + 0.5);

                Rectangle dstRect = new Rectangle(scaledPosition, scaledSize);

                FreeImageAlgorithmsBitmap thumb = null;

                lock (tile.ThumbnailLock)
                {
                    thumb = new FreeImageAlgorithmsBitmap(tile.Thumbnail);
                }

                thumb.ConvertToStandardType(true);

                if (thumb.IsGreyScale)
                {
                    thumb.LinearScaleToStandardType(MosaicWindow.MosaicInfo.ThumbnailScaleMinIntensity,
                        MosaicWindow.MosaicInfo.ThumbnailScaleMaxIntensity);

                    thumb.SetGreyLevelPalette();
                }

                if (thumb.ImageType != FREE_IMAGE_TYPE.FIT_BITMAP)
                {
                    MessageBox.Show("Failed to convert tile thumbnail to a standard type ?");
                }

                thumb.Rescale(dstRect.Size, FREE_IMAGE_FILTER.FILTER_BOX);

                thumb.ConvertTo24Bits();

                this.bitmap.PasteFromTopLeft(thumb, dstRect.Location, this.imageViewer.BlendingEnabled);

                thumb.Dispose();
            }

            this.Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (this.bitmap == null)
                return;

            e.Graphics.DrawImage(this.bitmap.ToBitmap(), 0, 0, bitmap.Width, bitmap.Height);

            e.Graphics.DrawRectangle(new Pen(Color.Red, 2), this.ScaledViewedRectangle);
        }


        private Rectangle ScaledViewedRectangle
        {
            get
            {
                float scaleFactor = (float)this.scaledWidth / imageViewer.ImageSize.Width;

                Rectangle rect = Utilities.ZoomRectangle(this.imageViewer.ViewedImageSectionRect, scaleFactor);

                return rect;
            }
        }

        private void OnScrollChanged(object sender, ScrollEventArgs e)
        {
            this.Refresh();
        }

        private void OnVerticalScrollChanged(object sender, ScrollEventArgs e)
        {
            this.Refresh();
        }

        private void OnHorizontalScrollChanged(object sender, ScrollEventArgs e)
        {
            this.Refresh();
        }

        private void OnPanningOccurred(TileView sender, EventArgs args)
        {
            this.Refresh();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileOverView));
            this.SuspendLayout();
            // 
            // TileOverView
            // 
            //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(443, 426);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TileOverView";
            this.Text = "Image Overview";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnOverviewMouseDown);
            //this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnOverviewFormClosing);
            this.ResumeLayout(false);

        }
        #endregion

        private void OnOverviewFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void OnOverviewMouseDown(object sender, MouseEventArgs e)
        {
            Point newPosition = new Point(e.Location.X - (this.ScaledViewedRectangle.Size.Width / 2),
                                          e.Location.Y - (this.ScaledViewedRectangle.Size.Height / 2));

            float scaleFactor = (float)this.scaledWidth / imageViewer.ImageSize.Width;

            newPosition.X = (int) (newPosition.X / scaleFactor + 0.5);
            newPosition.Y = (int) (newPosition.Y / scaleFactor + 0.5);

            // Causes scroll messages.
            this.imageViewer.SetAutoScrollPosition(Utilities.ZoomPoint(newPosition, this.imageViewer.Zoom));

            this.imageViewer.Redraw();
            this.Refresh();
        }
    }

    public class TileOverViewEventArgs : EventArgs
    {
        public TileOverViewEventArgs()
        {
        }
    }
}
