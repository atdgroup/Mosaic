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
using System.Timers;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Permissions;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ThreadingSystem;
using FreeImageAPI;

namespace ImageStitching
{
    public delegate void TimedScrollEventHandler(object sender, TimedScrollEventArgs e);

    public delegate void TileViewHandler<S, A>(S sender, A args);

    /// <summary>
    /// ImageView - Scrollable Picture Box.
    /// This is very simple it does not paint the image depending
    /// on where you have scroll to. That's for the client code.
    /// </summary>
    public class TileView : UserControl
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(int hWnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point pt);

        // If there are more than 6 tiles visible use thumbnails instead
        private const int PointAtWhichToUseThumbnails = 10;

        private TileViewEditToolBox toolbox = null;
        private MosaicInfo mosaicInfo;
        private List<Tile> tiles;
        private List<Tile> idleTiles;

        private Size imageSize;
        private float zoom = 1.0f;

        private bool blendingEnabled = false;
        private bool preventIdleProcessing = false;
        private bool highResBitmapReady = false;
        private bool showFileNames = false;
        private bool showJoins = false;

        private Rectangle defaultClip;

        private bool allowMouseWheelZoom = true;
        private bool panning = true;
        private bool leftMouseButtonDown;
        private Point mouseDownPoint;
        private bool dontDraw = false;
        private int currentTileIdleProcess;
        private bool intermediateBitmapDrawn = false;
        private FreeImageAlgorithmsBitmap intermediateBitmap = null;

        private bool highResBitmapIsReset = true;
        private FreeImageAlgorithmsBitmap highResBitmap;

        private List<Rectangle> lastIdleTileRectangles = new List<Rectangle>();
        private Size lastIdleTileSize;

        private Tile editTile;
        private Point editTileOriginalPosition;

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_MOUSEWHEEL = 0x020A;
        private System.Windows.Forms.Timer keyTimer;
        private IContainer components;
        private Keys pressedKey;

        private ThreadController threadController = null;

        public event EventHandler ZoomChanged;

        /// <summary>
        /// Horizontal scroll position has changed event
        /// </summary>
        public event ScrollEventHandler HorzScrollChanged;

        /// <summary>
        /// Vertical scroll position has changed event
        /// </summary>
        public event ScrollEventHandler VertScrollChanged;

        /// <summary>
        /// Horizontal scroll position has changed event
        /// </summary>
        public event ScrollEventHandler OnScrollChanged;

        public event TileViewHandler<TileView, EventArgs> PanningOccurred;

        #region Constructor
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.keyTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // keyTimer
            // 
            this.keyTimer.Interval = 20;
            this.keyTimer.Tick += new System.EventHandler(this.OnKeyTimerTick);
            // 
            // TileView
            // 
            this.Name = "TileView";
            this.Size = new System.Drawing.Size(729, 519);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDoubleClick);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.ResumeLayout(false);
        }

        internal TileView()
        {
            this.InitializeComponent();

            this.SetStyle(
                        ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint |
                        ControlStyles.ResizeRedraw |
                        ControlStyles.Opaque |
                        ControlStyles.DoubleBuffer, true);

            this.imageSize = new Size(0, 0);

            this.AutoScroll = false;

            this.defaultClip = Cursor.Clip;

            Application.Idle += new EventHandler(OnIdle);

            //this.timer = new System.Timers.Timer();
            //this.timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //timer.Interval = 50;
            //timer.Enabled = false;
        }

        #endregion

        public TileViewEditToolBox Toolbox
        {
            set
            {
                this.toolbox = value;
                this.toolbox.Visible = false;
                this.toolbox.TickButtonPressed += new TileViewEditToolBoxHandler<TileViewEditToolBox, EventArgs>(OnToolBoxTickButtonPressed);
                this.toolbox.CrossButtonPressed += new TileViewEditToolBoxHandler<TileViewEditToolBox, EventArgs>(OnToolBoxCrossButtonPressed);
            }
            get
            {
                return this.toolbox;
            }
        }

        public Rectangle DefaultClip
        {
            get
            {
                return this.defaultClip;
            }
        }

        public void SetThreadController(ThreadController threadController)
        {
            this.threadController = threadController;
        }

        internal void LoadMosaicInfo(MosaicInfo mosaicInfo)
        {
            this.mosaicInfo = mosaicInfo;

            Tile tile = mosaicInfo.Items[0];

            this.DestroyIntermediateBitmap();
 
            this.Clear();
            this.CalculateVisibleTiles();
        }

        #region ScrollEventCode

        /// <summary>
        /// Intercept scroll messages to send notifications
        /// </summary>
        /// <param name="m">Message parameters</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            // Let the control process the message
            base.WndProc(ref m);

            // Was this a horizontal scroll message?
            if (m.Msg == WM_HSCROLL)
            {
                //this.Clear();

                if (HorzScrollChanged != null)
                {
                    uint wParam = (uint)m.WParam.ToInt32();
                    HorzScrollChanged(this,
                        new ScrollEventArgs(
                            GetEventType(wParam & 0xffff), (int)(wParam >> 16)));
                }

                if (OnScrollChanged != null)
                {
                    uint wParam = (uint)m.WParam.ToInt32();
                    OnScrollChanged(this,
                        new ScrollEventArgs(
                            GetEventType(wParam & 0xffff), (int)(wParam >> 16)));
                }
            }
            // or a vertical scroll message?
            else if (m.Msg == WM_VSCROLL /* || m.Msg == WM_MOUSEWHEEL */ )
            {
                //this.Clear();

                if (VertScrollChanged != null)
                {
                    uint wParam = (uint)m.WParam.ToInt32();
                    VertScrollChanged(this,
                        new ScrollEventArgs(
                        GetEventType(wParam & 0xffff), (int)(wParam >> 16)));
                }

                if (OnScrollChanged != null)
                {
                    uint wParam = (uint)m.WParam.ToInt32();
                    OnScrollChanged(this,
                        new ScrollEventArgs(
                            GetEventType(wParam & 0xffff), (int)(wParam >> 16)));
                }
            }

            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL /* || m.Msg == WM_MOUSEWHEEL */ )
            {
                this.Redraw();
                this.Invalidate();
            }
        }

        // Send a scroll event when someone modifies the AutoScrollPosition.
        internal void SetAutoScrollPosition(Point location)
        {
            this.AutoScrollPosition = location;

            if (OnScrollChanged != null)
            {
                OnScrollChanged(this, null);
            }

            this.Redraw();
        }

        // Based on SB_* constants
        private static ScrollEventType[] _events =
            new ScrollEventType[] {
									  ScrollEventType.SmallDecrement,
									  ScrollEventType.SmallIncrement,
									  ScrollEventType.LargeDecrement,
									  ScrollEventType.LargeIncrement,
									  ScrollEventType.ThumbPosition,
									  ScrollEventType.ThumbTrack,
									  ScrollEventType.First,
									  ScrollEventType.Last,
									  ScrollEventType.EndScroll
								  };
        /// <summary>
        /// Decode the type of scroll message
        /// </summary>
        /// <param name="wParam">Lower word of scroll notification</param>
        /// <returns></returns>
        private static ScrollEventType GetEventType(uint wParam)
        {
            if (wParam < _events.Length)
                return _events[wParam];
            else
                return ScrollEventType.EndScroll;
        }

        #endregion ScrollEventCode

        protected void OnPanningOccurred(EventArgs args)
        {
            if (PanningOccurred != null)
                PanningOccurred(this, args);
        }

        public void Clear()
        {
            DestroyIntermediateBitmap();
            this.highResBitmap.Clear();
        }

        private void ResetHighResolutionBitmap()
        {
            if (highResBitmapIsReset)
                return;

            this.preventIdleProcessing = false;
            this.highResBitmapReady = false;

            lastIdleTileRectangles.Clear();
            highResBitmap.Clear();

            this.currentTileIdleProcess = 0;

            highResBitmapIsReset = true;
        }

        /// <summary>
        /// Specifies the Image to be loaded
        /// </summary>		
        private void CalculateVisibleTiles()
        {
            this.tiles = this.GetVisibleTiles();

            return;
        }

        public List<Tile> GetVisibleTiles()
        {
            Rectangle viewRect = this.ViewedImageSectionRect;

            if (viewRect.IsEmpty)
                return null;

            return Tile.GetTilesIntersectingRectangle(MosaicWindow.MosaicInfo.Items, viewRect);
        }

        public void Redraw()
        {
            this.CalculateVisibleTiles();
            this.ResetHighResolutionBitmap();
        }

        private void CreateHighResBitmap()
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
                return;

            if (this.highResBitmap != null)
            {
                if (this.highResBitmap.Width == this.ClientRectangle.Width &&
                    this.highResBitmap.Height == this.ClientRectangle.Height)
                {
                    this.highResBitmap.Clear();
                    return;
                }
            }

            this.highResBitmap =
                new FreeImageAlgorithmsBitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, 24);
        }

        private Graphics HighResGraphics(Bitmap bitmap)
        {
            return Graphics.FromImage(bitmap);
        }

        // This property returns a rectangle representing the area of the image currently 
        // viewed on the control.
        internal Rectangle ViewedImageSectionRect
        {
            get
            {
                int left = 0, top = 0;
                int width = (int)(this.imageSize.Width * this.zoom);
                int height = (int)(this.imageSize.Height * this.zoom);

                if (width < this.ClientRectangle.Width)
                {
                    width = this.imageSize.Width;
                }
                else
                {
                    left = (int)(-this.AutoScrollPosition.X / this.zoom);
                    width = (int)(this.ClientRectangle.Width / this.zoom);
                }

                if (height < this.ClientRectangle.Height)
                {
                    height = this.imageSize.Height;
                }
                else
                {
                    top = (int)(-this.AutoScrollPosition.Y / this.zoom);
                    height = (int)(this.ClientRectangle.Height / this.zoom);
                }

                return new Rectangle(left, top, width, height);
            }
        }

        public bool BlendingEnabled
        {
            set
            {
                this.blendingEnabled = value;

                this.Clear();

                CreateIntermediateBitmapIfNeccessary();

                this.SetupScrollbars();
                this.Invalidate();
            }
            get
            {
                return this.blendingEnabled;
            }
        }

        public bool ShowFileNames
        {
            set
            {
                this.showFileNames = value;

                if (this.mosaicInfo == null)
                    return;

                this.Clear();

                CreateIntermediateBitmapIfNeccessary();

                this.SetupScrollbars();
                this.Invalidate();
            }
            get
            {
                return this.showFileNames;
            }
        }

        public bool ShowJoins
        {
            set
            {
                this.showJoins = value;

                if (this.mosaicInfo == null)
                    return;

                this.Clear();

                CreateIntermediateBitmapIfNeccessary();

                this.SetupScrollbars();
                this.Invalidate();
            }
            get
            {
                return this.showJoins;
            }
        }

        internal Size ImageSize
        {
            get
            {
                return this.imageSize;
            }
            set
            {
                this.imageSize = value;
                this.SetupScrollbars();
            }
        }

        private void DestroyIntermediateBitmap()
        {
            if (this.intermediateBitmap != null)
            {
                this.intermediateBitmap.Dispose();
                this.intermediateBitmap = null;
                this.intermediateBitmapDrawn = false;
            }
        }

        private void DrawIntermediateBitmap()
        {
            if (!this.intermediateBitmapDrawn)
            {
                this.threadController.ReportThreadStarted(this, "Started Zoom");

                float aspectRatio = (float)(this.mosaicInfo.TotalWidth) / this.mosaicInfo.TotalHeight;

                int scaledWidth = Screen.PrimaryScreen.Bounds.Size.Width;
                int scaledHeight = (int)(scaledWidth / aspectRatio + 0.5);

                if (this.mosaicInfo.IsGreyScale)
                {
                    this.intermediateBitmap = new FreeImageAlgorithmsBitmap(scaledWidth, scaledHeight, 8);
                }
                else
                {
                    this.intermediateBitmap = new FreeImageAlgorithmsBitmap(scaledWidth, scaledHeight, 24);
                }

                float xscaleFactor = (float)scaledWidth / this.mosaicInfo.TotalWidth;
                float yscaleFactor = (float)scaledHeight / this.mosaicInfo.TotalHeight;

                if (this.intermediateBitmap == null)
                {
                    MessageBox.Show("Failed to create intermediate bitmap");
                }

                float scale = (float)scaledWidth / this.mosaicInfo.TotalWidth;
      
                Point scaledPosition = new Point();

                int count = 0;

                foreach (Tile tile in this.mosaicInfo.Items)
                {
                    if (tile.Thumbnail == null)
                        MessageBox.Show("Error thumnail is null");

                    //FreeImageAlgorithmsBitmap thumb = null;

                    //lock (tile.ThumbnailLock)
                    //{
                    //    thumb = new FreeImageAlgorithmsBitmap(tile.Thumbnail);
                    //}

                    FreeImageAlgorithmsBitmap fib = tile.LoadFreeImageBitmap();

                    fib.Rescale(new Size((int)(xscaleFactor * fib.Width), (int)(yscaleFactor * fib.Height)), FREE_IMAGE_FILTER.FILTER_BILINEAR);

                    if (fib.IsGreyScale)
                    {
                        fib.LinearScaleToStandardType(mosaicInfo.ScaleMinIntensity, mosaicInfo.ScaleMaxIntensity);

                        fib.SetGreyLevelPalette();
                    }
                    else
                    {
                        fib.ConvertToStandardType(true);
                    }

                    if (fib.ImageType != FREE_IMAGE_TYPE.FIT_BITMAP)
                    {
                        MessageBox.Show("Failed to convert tile thumbnail to a standard type ?");
                    }

                    Size scaledSize = new Size((int)(tile.Width * xscaleFactor + 0.5),
                                               (int)(tile.Height * yscaleFactor + 0.5));

                    scaledPosition.X = (int)(tile.Position.X * xscaleFactor + 0.5);
                    scaledPosition.Y = (int)(tile.Position.Y * yscaleFactor + 0.5);

                    Rectangle dstRect = new Rectangle(scaledPosition, scaledSize);

                    intermediateBitmap.PasteFromTopLeft(fib, scaledPosition, this.BlendingEnabled);

                    fib.Dispose();

                    this.threadController.ReportThreadPercentage(this,
                        "Performing Zoom", count++, this.tiles.Count);
                }
            }

            //intermediateBitmap.ConvertTo24Bits();

            this.intermediateBitmapDrawn = true;
            this.preventIdleProcessing = false;
            this.dontDraw = false;

            this.threadController.ReportThreadCompleted(this, "Zoom Completed", false);

            this.Redraw();
            this.Invalidate();
        }

        private void ThreadCreateIntermediateBitmap()
        {
            DrawIntermediateBitmap();
        }

        public bool NumberOfTilesExceedCacheThreshold
        {
            get
            {
                // Dont bother with cache images if the total number of tiles is less than fifty.
                if ((this.tiles.Count > (this.mosaicInfo.NumberOfTiles / 2)) && this.mosaicInfo.NumberOfTiles > 50)
                    return true;

                return false;
            }
        }

        public bool CacheImageAvailiable
        {
            get
            {
                return this.intermediateBitmapDrawn && NumberOfTilesExceedCacheThreshold;
            }
        }

        public FreeImageAlgorithmsBitmap CacheImage
        {
            get
            {
                return this.intermediateBitmap;
            }
        }

        private void CreateIntermediateBitmapIfNeccessary()
        {
            if(this.tiles == null)
                return;

            this.CalculateVisibleTiles();

            ResetHighResolutionBitmap();

            if (this.NumberOfTilesExceedCacheThreshold)
            {
                this.dontDraw = true;
                this.preventIdleProcessing = true;

                this.threadController.ThreadStart("ThreadCreateIntermediateBitmap",
                    new ThreadStart(this.ThreadCreateIntermediateBitmap));

                this.ThreadCreateIntermediateBitmap();
            }
        }

        public bool AllowPanning
        {
            get
            {
                return this.panning;
            }
            set
            {
                this.panning = value;
            }
        }

        internal float Zoom
        {
            set
            {
                if (this.zoom != value)
                {
                    if (value > 400.0)
                        return;

                    Point centre = new Point(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2);

                    double percentX = (double)(-this.AutoScrollPosition.X + centre.X) / this.zoom;
                    double percentY = (double)(-this.AutoScrollPosition.Y + centre.Y) / this.zoom;

                    this.zoom = value;
                    this.SetupScrollbars();

                    int x = Convert.ToInt32(percentX * this.zoom);
                    int y = Convert.ToInt32(percentY * this.zoom);

                    this.AutoScrollPosition = new Point(x - centre.X, y - centre.Y);

                    this.Clear();
                    CreateIntermediateBitmapIfNeccessary();

                    this.Redraw();
                    this.Invalidate();

                    OnZoomChanged(new EventArgs());
                }
            }
            get
            {
                return this.zoom;
            }
        }

        protected virtual void OnZoomChanged(EventArgs e)
        {
            if (this.ZoomChanged != null)
                this.ZoomChanged(this, e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.mosaicInfo != null)
            {
                this.Redraw();
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            CreateHighResBitmap();

            base.OnSizeChanged(e);
        }

        //protected override void On

        private Point CalculateImageCenter()
        {
            // Keep Image Centered
            int top = (int)((this.ClientSize.Height - (this.ImageSize.Height * this.Zoom)) / 2.0);
            int left = (int)((this.ClientSize.Width - (this.ImageSize.Width * this.Zoom)) / 2.0);

            if (top < 0)
                top = 0;

            if (left < 0)
                left = 0;

            return new Point(left, top);
        }

        private Matrix SetupTransform()
        {
            Point centerPosition = CalculateImageCenter();

            Matrix matrix = new Matrix();
            matrix.Reset();

            // Now translate the matrix into position for the scrollbars
            if (centerPosition.X != 0)
                matrix.Translate(centerPosition.X, 0);
            else
                matrix.Translate(this.AutoScrollPosition.X, 0);

            if (centerPosition.Y != 0)
                matrix.Translate(0, centerPosition.Y);
            else
                matrix.Translate(0, this.AutoScrollPosition.Y);

            matrix.Scale(this.zoom, this.zoom);

            return matrix;
        }

        private void SetupGraphicsTransform(Graphics graphics)
        {
            Matrix matrix = SetupTransform();

            // Use the transform
            graphics.Transform = matrix;
        }

        private void DrawThumbnailImage(Graphics graphics, Tile tile)
        {
            if (tile.Thumbnail != null)
            {
                lock (tile.ThumbnailLock)
                {
                    //tile.Thumbnail.ConvertToStandardType();
                    FreeImageAlgorithmsBitmap thumb = new FreeImageAlgorithmsBitmap(tile.Thumbnail);

                    if (thumb.IsGreyScale)
                    {
                        thumb.LinearScaleToStandardType(
                            mosaicInfo.ThumbnailScaleMinIntensity, mosaicInfo.ThumbnailScaleMaxIntensity);
                        thumb.SetGreyLevelPalette();
                    }

                    if (thumb.ImageType != FREE_IMAGE_TYPE.FIT_BITMAP)
                    {
                        MessageBox.Show("Failed to convert tile thumbnail to a standard type ?");
                    }

                    graphics.DrawImage(thumb.ToBitmap(), tile.Bounds);

                    thumb.Dispose();
                }
            }

            /*
            if (this.ShowFileNames)
            {
                // Create font and brush.
                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                Point location = new Point(tile.Bounds.Location.X + 100, tile.Bounds.Location.Y + 100);

                SizeF size = graphics.MeasureString(tile.FileName, drawFont);

                Brush brush = new SolidBrush(Color.FromArgb(50, Color.Gray));

                graphics.FillRectangle(brush, location.X - 2, location.Y - 2,
                    size.Width + 4, size.Height + 4);

                graphics.DrawString(tile.FileName, drawFont, drawBrush,
                    new PointF(location.X, location.Y));
            }
            */

            if (this.ShowJoins)
            {
                Pen pen = new Pen(Color.Red, 2.0f);
                graphics.DrawRectangle(pen, tile.Bounds);
            }
        }

        private Rectangle MaxRectangle(Rectangle rect1, Rectangle rect2)
        {
            Rectangle rect = new Rectangle();

            rect.Location = new Point(Math.Min(rect1.Left, rect2.Left), Math.Min(rect1.Top, rect2.Top));
            rect.Size = new Size(Math.Max(rect1.Width, rect2.Width), Math.Max(rect1.Height, rect2.Height));

            return rect;
        }

        private void OnIdle(object sender, EventArgs e)
        {
            // Nothing todo.
            if (this.preventIdleProcessing == true)
                return;

            if (this.highResBitmapReady == true)
                return;

            if (this.imageSize.IsEmpty)
                return;

            if (this.highResBitmap == null)
                return;

            if (this.intermediateBitmapDrawn == true && this.NumberOfTilesExceedCacheThreshold)
                return;

            this.Cursor = Cursors.WaitCursor;

            Rectangle viewRect = ViewedImageSectionRect;
 
            this.idleTiles = Tile.GetTilesIntersectingRectangle(MosaicWindow.MosaicInfo.Items, viewRect);

            // Go through the tiles and draw them at the highest resolution.
            // This is where gradient blending should go also.
            if (this.highResBitmapReady == false && this.currentTileIdleProcess < this.idleTiles.Count)
            {
                Tile tile = this.idleTiles[this.currentTileIdleProcess];

                FreeImageAlgorithmsBitmap fib = tile.LoadFreeImageBitmap();
         
                if (fib.IsGreyScale)
                {
                    fib.LinearScaleToStandardType(mosaicInfo.ScaleMinIntensity, mosaicInfo.ScaleMaxIntensity);

                    fib.SetGreyLevelPalette();
                }

                fib.ConvertTo24Bits();

                if (this.zoom < 1.0)
                {
                    fib.Rescale(new Size((int)(fib.Width * this.zoom), (int)(fib.Height * this.zoom)), FREE_IMAGE_FILTER.FILTER_BILINEAR);
                }

                Point pos = new Point();

                // We may be using the thumbnails to display so 
                // we need to work out the scaled coordinates if the zoom is < 1.0
                // For 1.0 and above we draw in native res just the area we can see on the screen.
                if (this.zoom < 1.0f)
                {
                    pos = tile.Position;

                    pos.X = (int)(tile.Position.X - viewRect.Location.X + 0.5);
                    pos.Y = (int)(tile.Position.Y - viewRect.Location.Y + 0.5);

                    pos.X = (int)(this.zoom * pos.X);
                    pos.Y = (int)(this.zoom * pos.Y);
                }
                else
                {
                    pos.X = tile.Position.X - viewRect.Location.X;
                    pos.Y = tile.Position.Y - viewRect.Location.Y;
                }

                lastIdleTileRectangles.Add(new Rectangle(pos, fib.Size));

                if (highResBitmap.Bounds.IntersectsWith(new Rectangle(pos, fib.Size)))
                {
                    highResBitmap.PasteFromTopLeft(fib, pos, this.blendingEnabled);
                }

                this.highResBitmapIsReset = false;

                fib.Dispose();

                // We have completed the last tile
                if (this.currentTileIdleProcess == this.idleTiles.Count - 1)
                {
                    // We are finished with the highres tiles 
                    // Draw the joins if neccessary
                    if (this.ShowJoins)
                    {
                        int i = 0;
                        foreach(Tile t in this.idleTiles) {
                           highResBitmap.DrawColourRect(lastIdleTileRectangles[i++], Color.Red, 2);
                        }
                    }

                    lastIdleTileSize.Width = (int) (Tile.GetHorizontalRangeOfTiles(idleTiles) * this.zoom);
                    lastIdleTileSize.Height = (int)(Tile.GetVerticalRangeOfTiles(idleTiles) * this.zoom);

                    this.Cursor = Cursors.Default;
                    this.highResBitmapReady = true;
                }

                this.currentTileIdleProcess++;

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.dontDraw == true)
            {
                base.OnPaintBackground(e);
                base.OnPaint(e);

                return;
            }

            // Here with have generated a highres image, so we use that.
            if (this.highResBitmapReady)
            {
                if (this.idleTiles == null)
                {
                    base.OnPaintBackground(e);
                    base.OnPaint(e);

                    return;
                }

                Matrix matrix = new Matrix();

                if (this.zoom < 1.0f)
                {
                    matrix.Scale(1.0f / this.zoom, 1.0f / this.zoom);
                }
       
                matrix.Scale(this.zoom, this.zoom);

                Point translate = new Point();

                if (this.lastIdleTileSize.Width < this.ClientRectangle.Width)
                {
                    translate.X = (this.ClientRectangle.Width - this.lastIdleTileSize.Width) / 2;
                }

                if (this.lastIdleTileSize.Height < this.ClientRectangle.Height)
                {
                    translate.Y = (this.ClientRectangle.Height - this.lastIdleTileSize.Height) / 2;
                }

                matrix.Translate(translate.X, translate.Y, MatrixOrder.Append);

                e.Graphics.Transform = matrix;
                e.Graphics.DrawImage(this.highResBitmap.ToBitmap(), this.ClientRectangle);

                this.SetupGraphicsTransform(e.Graphics);  
            }
            else if (this.intermediateBitmapDrawn == true && this.NumberOfTilesExceedCacheThreshold)
            {
                this.SetupGraphicsTransform(e.Graphics);

                if (this.intermediateBitmap != null)
                {
                    e.Graphics.DrawImage(this.intermediateBitmap.ToBitmap(),
                        new Rectangle(0, 0, this.mosaicInfo.TotalWidth, this.mosaicInfo.TotalHeight));
                }
            }
            else
            {
                if (this.tiles == null)
                {
                    base.OnPaintBackground(e);
                    base.OnPaint(e);

                    return;
                }

                this.SetupGraphicsTransform(e.Graphics);

                foreach (Tile tile in this.tiles)
                {
                    this.DrawThumbnailImage(e.Graphics, tile);
                }
            }

            if (this.editTile != null)
            {
                // Tile is being edited. Mark it will an alpha blended rectangle
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 0, 0, 255)), this.editTile.Bounds);
            }
             
            e.Graphics.ResetTransform();

            base.OnPaint(e);
        }

        public Bitmap ScreenBitmap
        {
            get
            {
                Bitmap bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);

                this.DrawToBitmap(bitmap, this.ClientRectangle);

                return bitmap;
            }
        }

        public void TransformPoints(Point[] points)
        {
            Matrix matrix = this.SetupTransform();

            matrix.Invert();
            matrix.TransformPoints(points);
        }

        private void SetupScrollbars()
        {
            if (this.imageSize.IsEmpty)
            {
                this.AutoScroll = false;
            }
            else
            {
                this.AutoScroll = true;

                this.AutoScrollMinSize = new Size(
                    (int)(this.imageSize.Width * this.Zoom),
                    (int)(this.imageSize.Height * this.Zoom));
            }
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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!this.AllowMouseWheelZoom)
                return;

            if (e.Delta > 0)
                this.Zoom += (0.1F * this.Zoom);
            else
                this.Zoom -= (1.0F / 11.0F * this.Zoom);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!this.AllowPanning)
                return;

            if (e.Button == MouseButtons.Left)
            {
                this.leftMouseButtonDown = true;

                mouseDownPoint = new Point(e.X + this.AutoScrollPosition.X,
                                           e.Y + this.AutoScrollPosition.Y);

            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!this.AllowPanning)
                return;

            this.leftMouseButtonDown = false;
        }

        private void ScrollBy(int dx, int dy)
        {
            if (dx != 0)  // If dx == 0 we don't do anything
            {
                // Ensure dx will not be 0 by adding or subtracting 1
                if (dx > 0)
                    dx = (int)((dx / this.zoom) + 1);
                else
                    dx = (int)((dx / this.zoom) - 1);

                // convert scroll distance to int multiple of zoom factor. So one scroll move = 1 pixel
                if (this.Zoom > 1)
                    dx = dx * (int)this.zoom;

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
                    dy = (int)((dy / this.zoom) + 1);
                else
                    dy = (int)((dy / this.zoom) - 1);

                // convert scroll distance to int multiple of zoom factor. So one scroll move = 1 pixel
                if (this.Zoom > 1)
                    dy = dy * (int)this.zoom;

                int vertValue = this.VerticalScroll.Value + dy;

                if (vertValue < 0)
                    vertValue = 0;

                if (vertValue > this.VerticalScroll.Maximum)
                    vertValue = this.VerticalScroll.Maximum;

                this.VerticalScroll.Value = vertValue;
            }

            this.Redraw();

            OnPanningOccurred(new EventArgs());
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!this.AllowPanning)
                return;

            if (this.tiles == null)
                return;

            // If fit to window exit


            if (this.leftMouseButtonDown == false)
                return;

            // Image is smaller than window so we can't pan
            //if (size.cx <= viewer_width && size.cy <= viewer_height)
            //    return 0;

            //if (x < 0 || x > viewer_width || y < 0 || y > viewer_height)
            //    return 0;

            Point point = new Point(e.X + this.AutoScrollPosition.X,
                                    e.Y + this.AutoScrollPosition.Y);

            int xDiff = point.X - this.mouseDownPoint.X;
            int yDiff = point.Y - this.mouseDownPoint.Y;

            this.ScrollBy(-xDiff, -yDiff);

            mouseDownPoint = point = new Point(e.X + this.AutoScrollPosition.X,
                                    e.Y + this.AutoScrollPosition.Y);

        }

        public Tile FindTileForPoint(Point point)
        {
            if (this.mosaicInfo == null)
                return null;

            Point[] points = {point};

            this.TransformPoints(points);

            Tile t = null;

            foreach (Tile tile in this.mosaicInfo.Items)
            {
                if (tile.Bounds.Contains(points[0]))
                {
                    t = tile;
                    break;
                }
            }

            if (t != null)
            {
                return t;
            }

            return null;
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            // we have double clicked the view.
            // Find the tile we are over and mark that as the one we shall edit.
            this.editTile = FindTileForPoint(e.Location);
            this.editTileOriginalPosition = this.editTile.Position;

            // Only do this if the toolbox is embedded as a control
            // As we then have paint programs
            if (this.Toolbox != null)
                this.Toolbox.Visible = true;

            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            switch(keyData)
            {
                case Keys.Left:
                {
                    this.pressedKey = keyData;
                    this.keyTimer.Enabled = true;
                    this.highResBitmapReady = false;
                    return true;
                }

                case Keys.Right:
                {
                    this.pressedKey = keyData;
                    this.keyTimer.Enabled = true;
                    this.highResBitmapReady = false;
                    return true;
                }

                case Keys.Up:
                {
                    this.pressedKey = keyData;
                    this.keyTimer.Enabled = true;
                    this.highResBitmapReady = false;
                    return true;
                }

                case Keys.Down:
                {
                    this.pressedKey = keyData;
                    this.keyTimer.Enabled = true;
                    this.highResBitmapReady = false;
                    return true;
                }
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        private List<Keys> ValidKeys = new List<Keys> { Keys.Enter, Keys.Escape, Keys.Left, Keys.Right, Keys.Up, Keys.Down };

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            this.keyTimer.Enabled = false;

            if(ValidKeys.Contains(e.KeyData))
            {
                this.Redraw();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (this.editTile == null)
                return;

            if (!ValidKeys.Contains(e.KeyData))
                return;

            if (e.KeyData == Keys.Enter)
            {
                // The images have been moved so we need to work out the new extents of the whole image.
                this.ImageSize = new Size(MosaicWindow.MosaicInfo.TotalWidth, MosaicWindow.MosaicInfo.TotalHeight);
                this.editTile = null;
                this.keyTimer.Enabled = false;
            }
            else if (e.KeyData == Keys.Escape)
            {
                this.editTile.AdjustedPosition = this.editTileOriginalPosition;
                this.editTile = null;
                this.keyTimer.Enabled = false;
            }

            if (this.Toolbox != null)
                this.Toolbox.Visible = false;
        }

        void OnToolBoxTickButtonPressed(TileViewEditToolBox sender, EventArgs args)
        {
            if (this.editTile == null)
                return;

            // The images have been moved so we need to work out the new extents of the whole image.
            this.ImageSize = new Size(MosaicWindow.MosaicInfo.TotalWidth, MosaicWindow.MosaicInfo.TotalHeight);
            this.editTile = null;
            this.keyTimer.Enabled = false;

            if (this.Toolbox != null)
                this.Toolbox.Visible = false;

            this.Redraw();

            return;
        }

        void OnToolBoxCrossButtonPressed(TileViewEditToolBox sender, EventArgs args)
        {
            if (this.editTile == null)
                return;

            this.editTile.AdjustedPosition = this.editTileOriginalPosition;
            this.editTile = null;
            this.keyTimer.Enabled = false;

            if (this.Toolbox != null)
                this.Toolbox.Visible = false;

            this.Redraw();

            return;
        }

        private void OnKeyTimerTick(object sender, EventArgs e)
        {
            if (this.editTile == null)
                return;

            Point pt = this.editTile.AdjustedPosition;
            
            switch (this.pressedKey)
            {
                case Keys.Left:
                {
                    pt.X--;
                    break;
                }

                case Keys.Right:
                {
                    pt.X++;
                    break;
                }

                case Keys.Up:
                {
                    pt.Y--;
                    break;
                }

                case Keys.Down:
                {
                    pt.Y++;
                    break;
                }
            }

            this.editTile.OriginalPosition = pt;
            this.Invalidate();
        }
    }

    public class TimedScrollEventArgs : EventArgs
    {
        private int x;
        private int y;

        public TimedScrollEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get
            {
                return this.x;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
        }
    }
}