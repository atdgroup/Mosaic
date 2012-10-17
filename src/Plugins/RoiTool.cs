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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ZedGraph;
using FreeImageAPI;
using FreeImageIcs;

namespace ImageStitching
{
    public delegate void RoiToolHandler<S, A>(S sender, A args);

    public class RoiToolEventArgs : EventArgs
    {
        private Rectangle rect;

        public RoiToolEventArgs(Rectangle rect)
        {
            this.rect = rect;
        }

        public Rectangle Rectangle
        {
            get
            {
                return this.rect;
            }
        }
    }

    public class RoiTool : MosaicToggleButtonTool
    {
        private bool disableUserInteraction = false;
        private bool mouseDown = false;
        private Rectangle roi;
        private ToolStripMenuItem menuItem;

        public event RoiToolHandler<RoiTool, RoiToolEventArgs> RoiToolDrawnHandler;
		
        public RoiTool(string name, MosaicWindow window)
            :
            base(name, window, "Resources.roi_icon.ico")
        {
            this.roi = Rectangle.Empty;

            this.menuItem = AddMenuItem("Display", "Region", null, new EventHandler(OnRegionMenuCheckedChanged));

            this.menuItem.Enabled = false;
        }

        protected override void OnMosaicLoaded(MosaicWindowEventArgs args)
        {
            this.Enabled = true;
            this.menuItem.Enabled = true;
        }

        #region TODO_FEATURE

        private static int CalculateMoveDistance(int mouse_dist_from_window)
        {
            int dist;

            if (mouse_dist_from_window < 16)
                dist = 2;
            else if (mouse_dist_from_window < 48)
                dist = 8;
            else
                dist = 20;

            return dist;
        }

        /*
                // Specify what you want to happen when the Elapsed event is raised.
                private void OnTimedEvent(object source, ElapsedEventArgs e)
                {
                    int x_scroll_distance = 0, y_scroll_distance = 0;
                    Point pt = new Point();

                    GetCursorPos(ref pt);

                    if (pt.X > this.ClientRectangle.Right)
                        x_scroll_distance = CalculateMoveDistance(pt.X - this.ClientRectangle.Right);

                    if (pt.X < this.ClientRectangle.Left)
                    {
                        x_scroll_distance = CalculateMoveDistance(this.ClientRectangle.Left - pt.X);
                        x_scroll_distance = -x_scroll_distance;
                    }

                    if (pt.Y > this.ClientRectangle.Bottom)
                        y_scroll_distance = CalculateMoveDistance(pt.Y - this.ClientRectangle.Bottom);

                    if (pt.Y < this.ClientRectangle.Top)
                    {
                        y_scroll_distance = CalculateMoveDistance(this.ClientRectangle.Top - pt.Y);
                        y_scroll_distance = -y_scroll_distance;
                    }

                    //TimedScrollEventArgs args = new TimedScrollEventArgs(x_scroll_distance, y_scroll_distance);
                    //this.BeginInvoke(new TimedScrollEventHandler(this.TimerScroll), this, args);         
                }
                */
        #endregion

        public bool DisableUserInteraction
        {
            set
            {
                this.disableUserInteraction = value;
            }
            get
            {
                return this.disableUserInteraction;
            }
        }


        protected virtual void OnRoiToolDrawnHandler(RoiToolEventArgs e)
        {
            if (RoiToolDrawnHandler != null)
                RoiToolDrawnHandler(this, e);
        }

        protected override void OnViewMouseDown(MouseEventArgs e)
        {
            if (this.DisableUserInteraction)
                return;

            List<Tile> tiles = this.MosaicWindow.TileView.GetVisibleTiles();

            if (tiles == null)
                return;

            this.mouseDown = true;

            if (e.Button == MouseButtons.Left)
            {
                this.MosaicWindow.TileView.Cursor = Cursors.SizeAll;
                this.roi.Location = e.Location;

                Cursor.Clip = this.MosaicWindow.TileView.RectangleToScreen(
                    this.MosaicWindow.TileView.ClientRectangle);
            }
        }

        protected override void OnViewMouseUp(MouseEventArgs e)
        {
            if (this.DisableUserInteraction)
                return;

            Cursor.Clip = this.MosaicWindow.TileView.DefaultClip;
            this.MosaicWindow.TileView.Cursor = Cursors.Default;

            List<Tile> tiles = this.MosaicWindow.TileView.GetVisibleTiles();

            if (tiles == null)
                return;

            this.roi = NormaliseRectangle(this.roi);

            if (roi.Width < 20 && roi.Height < 20)
            {
                this.roi = Rectangle.Empty;
                this.MosaicWindow.TileView.Invalidate();
            }

            OnRoiToolDrawnHandler(new RoiToolEventArgs(this.NormaliseRectangle(this.roi)));
       
            this.mouseDown = false;
        }

        #region TODO_FEATURE
        //private void TimerScroll(object sender, TimedScrollEventArgs e)
        //{
        //    MyScroll(e.X, e.Y);
        //}

        //private void MyScroll(int x, int y)
        //{      
        //    x = Math.Abs(this.AutoScrollPosition.X) - x;
        //    y = Math.Abs(this.AutoScrollPosition.Y) - y;
        //
        //    this.SetAutoScrollPosition(new Point(x, y));
        //}
        #endregion

        protected override void OnViewMouseMove(MouseEventArgs e)
        {
            if (this.DisableUserInteraction)
                return;

            List<Tile> tiles = this.MosaicWindow.TileView.GetVisibleTiles();

            if (tiles == null)
                return;

            if (this.mouseDown)
            {
                #region TODO_FEATURE
                /*
                        MyScroll(e.X - this.roi.X, e.Y - this.roi.Y);

                        if (!this.ClientRectangle.Contains(e.Location))
                        {
                            timer.Enabled = true;
                        }
                        else
                        {
                            
                            timer.Enabled = false;
                        }
                        */
                #endregion

                this.roi.Size = new Size(e.X - this.roi.X + 1,
                                         e.Y - this.roi.Y + 1);

                this.MosaicWindow.TileView.Invalidate();
            }
        }


        private void EnableROI()
        {
            this.MosaicWindow.TileView.AllowPanning = false;
            this.DisableUserInteraction = false;
            this.Active = true;
            this.Button.Checked = true;
            this.menuItem.Checked = true;
        }

        private void DisableROI()
        {
            this.Active = false;
            this.Button.Checked = false;
            this.menuItem.Checked = false;
            this.MosaicWindow.TileView.AllowPanning = true;
            this.roi = Rectangle.Empty;
            
        }

        protected void OnRegionMenuCheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (MosaicWindow.MosaicInfo == null)
                return;

            if (item.Checked)
            {
                EnableROI();
            }
            else
            {
                DisableROI();
            }
        }

        protected override void OnToggleButtonToolSelected(object sender, EventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;

            if (item.Checked)
            {
                EnableROI();            
            }
            else
            {
                DisableROI();
            }

            this.MosaicWindow.TileView.Invalidate();

            base.OnToggleButtonToolSelected(sender, e);
        }

        private Rectangle NormaliseRectangle(Rectangle rect)
        {
            Rectangle rc = new Rectangle(Math.Min(rect.Left, rect.Right), Math.Min(rect.Top, rect.Bottom),
                Math.Abs(rect.Width), Math.Abs(rect.Height));

            return rc;
        }

        public Rectangle TransformedRegionOfInterest
        {
            get
            {
                if (this.roi.Size.Width < 20 && this.roi.Size.Height < 20)
                    return Rectangle.Empty;

                Point p1 = new Point(this.roi.Left, this.roi.Top);
                Point p2 = new Point(this.roi.Right, this.roi.Bottom);
                Point[] points = { p1, p2 };

                this.MosaicWindow.TileView.TransformPoints(points);

                Size size = new Size(Math.Abs(points[1].X - points[0].X + 1),
                                     Math.Abs(points[1].Y - points[0].Y + 1));

                Rectangle rect = new Rectangle(points[0], size);

                return rect;
            }
        }

        protected override void OnViewPainted(PaintEventArgs e)
        {
            if (this.roi != Rectangle.Empty)
            {
                e.Graphics.DrawRectangle(Pens.Red, NormaliseRectangle(this.roi));
            }
        }   
    }
}
