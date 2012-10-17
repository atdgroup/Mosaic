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
    public partial class ProfileForm : Form
    {
        private MosaicWindow mosaicWindow;
        private LineItem curve;

        public ProfileForm(MosaicWindow window)
        {
            InitializeComponent();

            mosaicWindow = window;
        }

        private void CreateCurveForGreylevelImageLineData(Point start, Point end)
        {
            TileView viewer = this.mosaicWindow.TileView;

            List<Tile> tiles = viewer.GetVisibleTiles();

            if (tiles == null)
                return;

            if (MosaicWindow.MosaicInfo.ColorDepth >= 24)
                return;

            int max_pixels = Math.Abs((end.X - start.X)) + Math.Abs((end.Y - start.Y));

            double[] values = new double[max_pixels];
             
            int len = 0;

            foreach (Tile tile in tiles)
            {
                if (tile.IsDummy)
                    continue;

                FreeImageAlgorithmsBitmap dib = tile.LoadFreeImageBitmap();
             
                len = dib.GetGreyScalePixelValuesAsDoublesForLine(start, end, out values);
            
                dib.Dispose();          
            }

            PointPairList list = new PointPairList();

            double x, y;

            for (int i = 0; i < len; i++)
            {
                x = (double)i;
                y = (double)values[i];
                list.Add(x, y);
            }

            if (this.curve != null)
                this.zedGraphControl.GraphPane.CurveList.Remove(this.curve);

            this.curve = new LineItem("Profile", list, Color.Black, SymbolType.None);

            this.zedGraphControl.GraphPane.CurveList.Insert(0, curve);

            this.zedGraphControl.AxisChange();     
        }

        public void ShowProfile(Point start, Point end)
        {
            // Get a reference to the GraphPane
            GraphPane pane = this.zedGraphControl.GraphPane;

            // Set the Titles
            pane.Title.Text = "Intensity Histogram";
            pane.XAxis.Title.Text = "Intensity";
            pane.YAxis.Title.Text = "Count";

            CreateCurveForGreylevelImageLineData(start, end);

            this.zedGraphControl.AxisChange();
        }
    }


    public class ProfileTool : MosaicToggleButtonTool
    {
        private bool drawProfile;
        private bool mouseDown;
        private Point mouseDownLocation;
        private Point mouseEndLocation;
        private ProfileForm form;

        public ProfileTool(string name, MosaicWindow window)
            :
            base(name, window, "Resources.profile_icon.ico")
        {
            this.form = new ProfileForm(window);
            this.form.Hide();
        }

        protected override void OnViewMouseDown(MouseEventArgs e)
        {
            List<Tile> tiles = this.MosaicWindow.TileView.GetVisibleTiles();

            if (tiles == null)
                return;

            this.mouseDown = true;
            this.mouseDownLocation = e.Location;

        }

        protected override void OnViewMouseUp(MouseEventArgs e)
        {
            this.mouseDown = false;
            this.form.ShowProfile(this.mouseDownLocation, e.Location);
            this.form.Show();
        }

        protected override void OnViewMouseMove(MouseEventArgs e)
        {
            if (!this.mouseDown) return;
            
            this.drawProfile = true;
            this.mouseEndLocation = e.Location;
            this.MosaicWindow.TileView.Invalidate();
        }

        protected override void OnViewPainted(PaintEventArgs e)
        {
            bool activelyDrawing = this.MouseDownLocationIsValid && this.mouseDown;
            bool displayProfile = this.drawProfile && this.MouseDownLocationIsValid;

            if (activelyDrawing || displayProfile)
            {
                e.Graphics.DrawLine(Pens.Blue, this.mouseDownLocation, this.mouseEndLocation);
            }
        }   

        private bool MouseDownLocationIsValid
        {
            get
            {
                return (this.mouseDownLocation != new Point(-1,-1));
            }
        }

        protected override void OnToggleButtonToolSelected(object sender, EventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;

            this.drawProfile = false;
            this.mouseDown = false;
            this.mouseDownLocation = new Point(-1, -1);

            if (item.Checked)
            {
                MosaicPlugin.AllPlugins["Region"].Active = false;
                this.MosaicWindow.TileView.AllowPanning = false;

                this.Active = true;
            }
            else
            {
                this.MosaicWindow.TileView.AllowPanning = true;
                this.Active = false;
            }
        }
    }
}