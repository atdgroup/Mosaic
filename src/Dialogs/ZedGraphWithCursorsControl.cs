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

namespace ImageStitching
{
    public delegate void ZedGraphCursorHandler<S, A>(S sender, A args);

    public partial class ZedGraphWithCursorsControl : ZedGraph.ZedGraphControl
    {
        public event ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>
            ZedGraphCursorChangingHandler;

        public event ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>
            ZedGraphCursorChangedHandler;

        private List<ZedGraphCursor> cursors = new List<ZedGraphCursor>();
        private ZedGraphCursor selectedCursor;
        private Point oldPoint;
        private Boolean hit = false;
        private Boolean mouseDown = false;

        public ZedGraphWithCursorsControl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public void AddVerticalCursor(VerticalZedGraphCursor cursor)
        {
            cursors.Add(cursor);
        }

        protected void OnCursorPositionChanging(ZedGraphCursorEventArgs args)
        {
            if(ZedGraphCursorChangingHandler != null)
                ZedGraphCursorChangingHandler(this, args);
        }

        protected void OnCursorPositionChanged(ZedGraphCursorEventArgs args)
        {
            if(ZedGraphCursorChangedHandler != null)
                ZedGraphCursorChangedHandler(this, args);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.cursors.Count == 0)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            PointF pt = new PointF();

            pt.X = (float)e.Location.X;
            pt.Y = (float)e.Location.Y;

            double x, y;

            this.GraphPane.ReverseTransform(pt, out x, out y);

            y = this.GraphPane.YAxis.Scale.Max - y;

            foreach (ZedGraphCursor cursor in this.cursors)
            {
                float tolerance = 5.0F;  // some tolerance in picking up the cursor
                RectangleF r = new RectangleF((float)x - tolerance / 2, (float)y - tolerance / 2, tolerance, tolerance);

//                if (cursor.GraphRect.Contains((float)x, (float)y))
                if (cursor.GraphRect.IntersectsWith(r))
                {
                    oldPoint = e.Location;
                    selectedCursor = cursor;
                    mouseDown = true;
                    hit = true;
                    return;
                }
            }

            return;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.cursors.Count == 0)
                return;

            mouseDown = false;
            hit = false;

            if (this.selectedCursor != null)
            {
                this.OnCursorPositionChanged(new ZedGraphCursorEventArgs(this.selectedCursor, this.selectedCursor.Position));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.cursors.Count == 0)
                return;

            if (mouseDown && hit)
            {
                PointF pt = new PointF();

                pt.X = (float)e.Location.X;
                pt.Y = (float)e.Location.Y;

                double x, y;

                this.GraphPane.ReverseTransform(pt, out x, out y);

                this.selectedCursor.Position = x;
             
                int left = (int) this.GraphPane.Chart.Rect.Left - 10;
                int top = (int) this.GraphPane.Chart.Rect.Top - 10;
                int width = (int) this.GraphPane.Chart.Rect.Width + 20;
                int height = (int) this.GraphPane.Chart.Rect.Height + 20;

                Rectangle chart_rect = new Rectangle(left, top, width, height);

                this.Invalidate(chart_rect);

                this.OnCursorPositionChanging(new ZedGraphCursorEventArgs(this.selectedCursor, this.selectedCursor.Position));
            }
        }
    }

    public class ZedGraphCursorEventArgs : EventArgs
    {
        private ZedGraphCursor cursor;
        private double position;

        public ZedGraphCursorEventArgs(ZedGraphCursor cursor, double position)
        {
            this.cursor = cursor;
            this.position = position;
        }

        public ZedGraphCursor Cursor
        {
            get
            {
                return this.cursor;
            }
        }

        public double Position
        {
            get
            {
                return this.position;
            }
        }
    }

    public abstract class ZedGraphCursor : BoxObj
    {     
        private int cursorSizeInPixels = 2;
        private RectangleF graphRect;

        public ZedGraphCursor() : base() { }

        public int CursorSizeInPixels
        {
            get
            {
                return this.cursorSizeInPixels;
            }
            set
            {
                this.cursorSizeInPixels = value;
            }
        }

        public abstract double CursorSize
        {
            get;
        }

        public RectangleF GraphRect
        {
            get
            {
                return graphRect;
            }
            set
            {
                this.graphRect = value;
            }
        }

        public abstract double Position
        {
            get;
            set;
        }     
    }

    public class VerticalZedGraphCursor : ZedGraphCursor
    {
        ZedGraphWithCursorsControl zedGraph;
        Color color;
        private double x;

        public VerticalZedGraphCursor(ZedGraphWithCursorsControl zedGraph, Color color)
        {
            this.zedGraph = zedGraph;
            this.color = color;

            GraphPane pane = this.zedGraph.GraphPane;

            this.x = pane.XAxis.Scale.Min;
            this.SetupFromGraph();
            pane.AxisChangeEvent += new GraphPane.AxisChangeEventHandler(OnAxisChangeEvent);
        }

        void OnAxisChangeEvent(GraphPane pane)
        {
            double ymin = pane.YAxis.Scale.Min;
            double ymax = pane.YAxis.Scale.Max;

            if(this.x < pane.XAxis.Scale.Min)
                this.x = pane.XAxis.Scale.Min;

            if (this.x > pane.XAxis.Scale.Max)
                this.x = pane.XAxis.Scale.Max;

            this.GraphRect = new RectangleF((float)x, (float)ymin,
                (float)this.CursorSize, (float)(ymax - ymin));

            this.Location = new Location(x, ymax, this.CursorSize, ymax - ymin,
                       CoordType.AxisXYScale, AlignH.Center, AlignV.Bottom);

            this.Border.Color = Color.Empty;
            this.Fill.Color = this.color;
            this.ZOrder = ZOrder.A_InFront;

            this.zedGraph.Invalidate();
        }

        private void SetupFromGraph()
        {
            this.zedGraph.GraphPane.GraphObjList.Add(this);
        }

        public override double CursorSize
        {
            get
            {
                double chartWidth = this.zedGraph.GraphPane.Chart.Rect.Width;

                double percent = (double)this.CursorSizeInPixels / chartWidth;

                double range = this.zedGraph.GraphPane.XAxis.Scale.Max - this.zedGraph.GraphPane.XAxis.Scale.Min;
                return (percent * range);
            }
        }

        public override double Position
        {
            get
            {
                return this.x;
            }
            set
            {
                double tmp;

                if (this.x == value)
                    return;

                tmp = value;

                if (value > this.zedGraph.GraphPane.XAxis.Scale.Max)
                    tmp = this.zedGraph.GraphPane.XAxis.Scale.Max;

                if (value < this.zedGraph.GraphPane.XAxis.Scale.Min)
                    tmp = this.zedGraph.GraphPane.XAxis.Scale.Min;

                this.x = tmp;
                this.Location.X = this.x;

                RectangleF rect = this.GraphRect;
                rect.X = (float) this.x;
                this.GraphRect = rect;
                //this.Location.X = rect.X;

                //this.this.zedGraph.GraphPane.AxisChange();
                //this.OnPositionChanged(this, new ZedGraphCursorEventArgs(rect.X));
            }
        }
    }
}

