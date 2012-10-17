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
    public partial class LinearScaleForm : Form
    {
        private MosaicWindow mosaicWindow;
        private VerticalZedGraphCursor cursor1;
        private VerticalZedGraphCursor cursor2;
        private double min;
        private double max;
        private double max_possible;
        private PointPairList list;
        private LineItem curve;
        private LineItem histogramCurve;

        public LinearScaleForm(MosaicWindow window)
        {
            InitializeComponent();

            mosaicWindow = window;

            // Set the Titles
            this.zedGraphControl.GraphPane.Title.Text = "LUT";
            this.zedGraphControl.GraphPane.XAxis.Title.Text = "Input Intensity";
            this.zedGraphControl.GraphPane.YAxis.Title.Text = "Output Intensity";

            this.list = new PointPairList();

            this.cursor1 =
               new VerticalZedGraphCursor(this.zedGraphControl, Color.Red);

            this.cursor2 =
                new VerticalZedGraphCursor(this.zedGraphControl, Color.Green);

            zedGraphControl.ZedGraphCursorChangingHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangingHandler);

            zedGraphControl.ZedGraphCursorChangedHandler += new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangedHandler);

            this.cursor1.Position = this.min;
            this.cursor2.Position = this.max;

            this.zedGraphControl.AddVerticalCursor(cursor1);
            this.zedGraphControl.AddVerticalCursor(cursor2);
        }

        private void LinearScale_Load(object sender, EventArgs e)
        {
            UpdateGraphForLoadedImage();      
  
        }

        public void UpdateGraphForLoadedImage()
        {         
            TileView viewer = this.mosaicWindow.TileView;

//            CalculateGreyLevelHistogramShape();

            double min_in_images = Double.MaxValue, max_in_images = Double.MinValue;
            double max_posible_intensity = 0.0;

            if (MosaicWindow.MosaicInfo.IsColour)
            {
                return;
            }

            if (viewer.CacheImageAvailiable)
            {
                max_posible_intensity = viewer.CacheImage.MaximumPossibleIntensityValue;
                viewer.CacheImage.FindMinMaxIntensity(out min_in_images, out max_in_images);
            }
            else
            {

                List<Tile> tiles = viewer.GetVisibleTiles();

                if (tiles == null)
                    return;

                foreach (Tile tile in tiles)
                {
                    if (tile.IsDummy)
                        continue;

                    FreeImageAlgorithmsBitmap dib = tile.LoadFreeImageBitmap();

                    if (max_posible_intensity == 0.0)
                        max_posible_intensity = dib.MaximumPossibleIntensityValue;

                    double min;
                    double max;

                    dib.FindMinMaxIntensity(out min, out max);

                    if (min < min_in_images)
                        min_in_images = min;

                    if (max > max_in_images)
                        max_in_images = max;

                    dib.Dispose();
                }
            }

            this.min = min_in_images;
            this.max = max_in_images;
            this.max_possible = max_posible_intensity;

            this.minNumericUpDown.Minimum = Convert.ToDecimal(0);
            this.maxNumericUpDown.Minimum = Convert.ToDecimal(1);
            this.minNumericUpDown.Maximum = Convert.ToDecimal(this.max_possible - 1);
            this.maxNumericUpDown.Maximum = Convert.ToDecimal(this.max_possible);
            this.minNumericUpDown.Value = Convert.ToDecimal(this.min);
            this.maxNumericUpDown.Value = Convert.ToDecimal(this.max);

            this.zedGraphControl.GraphPane.XAxis.Scale.Min = 0;
            this.zedGraphControl.GraphPane.XAxis.Scale.Max = this.max;
            this.zedGraphControl.GraphPane.YAxis.Scale.Min = 0;
            this.zedGraphControl.GraphPane.YAxis.Scale.Max = 255;

            this.cursor1.Position = this.min;
            this.cursor2.Position = this.max;

            this.DisplayPlot();
           
            this.zedGraphControl.AxisChange();
        }

        #region TODO
        /*
        private void CaculateColourHistogramShape()
        {
            TileView viewer = this.mosaicWindow.TileView;

            List<Tile> tiles = this.mosaicWindow.MosaicInfo.Items;

            if (tiles == null)
                return;

            int[] total_red_hist = new int[256];
            int[] total_green_hist = new int[256];
            int[] total_blue_hist = new int[256];
            ulong[] total_hist = new ulong[256];

            int[] red_hist = null;
            int[] green_hist = null;
            int[] blue_hist = null;
            ulong[] hist = null;
            bool colourImage = false;

            if (mosaicWindow.MosaicInfo.ColorDepth >= 24)
                colourImage = true;

            for (int i = 0; i < total_red_hist.Length; i++)
            {
                total_red_hist[i] = 0;
                total_green_hist[i] = 0;
                total_blue_hist[i] = 0;
            }

            double max_posible_intensity_in_images = 0.0;

            foreach (Tile tile in tiles)
            {

                if (tile.IsDummy)
                    continue;

                FreeImageAlgorithmsBitmap dib = tile.LoadFreeImageBitmap(tile.FilePath);

                if (dib.MaximumPossibleIntensityValue > max_posible_intensity_in_images)
                    max_posible_intensity_in_images = dib.MaximumPossibleIntensityValue;

                if (colourImage)
                {
                    dib.GetHistogram(FREE_IMAGE_COLOR_CHANNEL.FICC_RED, out red_hist);
                    dib.GetHistogram(FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN, out green_hist);
                    dib.GetHistogram(FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE, out blue_hist);
                }
                else
                {
                    dib.GetGreyLevelHistogram(256, out hist);
                }

                dib.Dispose();

                for (int i = 0; i < total_red_hist.Length; i++)
                {
                    if (colourImage)
                    {
                        total_red_hist[i] += red_hist[i];
                        total_green_hist[i] += green_hist[i];
                        total_blue_hist[i] += blue_hist[i];
                    }
                    else
                    {
                        total_hist[i] += hist[i];
                    }
                }
            }

            double range_per_bin = max_posible_intensity_in_images / 256;

            double x, y;
            PointPairList red_list = new PointPairList();
            PointPairList green_list = new PointPairList();
            PointPairList blue_list = new PointPairList();

            if (colourImage)
            {
                for (int i = 0; i < total_red_hist.Length; i++)
                {
                    x = (double)i;
                    y = (double)total_red_hist[i];
                    red_list.Add(x, y);

                    y = (double)total_green_hist[i];
                    green_list.Add(x, y);

                    y = (double)total_blue_hist[i];
                    blue_list.Add(x, y);
                }

                // Generate a red curve with diamond
                LineItem red_curve = myPane.AddCurve("",
                      red_list, Color.Red, SymbolType.None);

                LineItem green_curve = myPane.AddCurve("",
                      green_list, Color.Green, SymbolType.None);

                LineItem blue_curve = myPane.AddCurve("",
                      blue_list, Color.Blue, SymbolType.None);
            }
            else
            {
                for (int i = 0; i < total_red_hist.Length; i++)
                {
                    x = (double)(i * range_per_bin);
                    y = (double)total_hist[i];
                    red_list.Add(x, y);
                }

                // Generate a red curve with diamond
                // symbols, and "Porsche" in the legend
                BarItem curve = myPane.AddBar("",
                      red_list, Color.Black);
            }

            zgc.AxisChange();
        }
        */
        #endregion

        private void CalculateGreyLevelHistogramShape()
        {
            if (MosaicWindow.MosaicInfo.ColorDepth >= 24)
                return;

            TileView viewer = this.mosaicWindow.TileView;

            List<Tile> tiles = MosaicWindow.MosaicInfo.Items;

            if (tiles == null)
                return;

            int[] hist = null;
            int[] total_hist = new int[256];

            if (viewer.CacheImageAvailiable)
            {
                viewer.CacheImage.GetHistogram(FREE_IMAGE_COLOR_CHANNEL.FICC_BLACK, out total_hist);
            }
            else
            {
                foreach (Tile tile in tiles)
                {
                    tile.Thumbnail.GetHistogram(FREE_IMAGE_COLOR_CHANNEL.FICC_BLACK, out hist);

                    for (int i = 0; i < 256; i++)
                    {
                        total_hist[i] = hist[i];
                    }
                }
            }

            int hist_peak_intensity = 0;

            for (int i = 0; i < 256; i++)
            {
                if (total_hist[i] > hist_peak_intensity)
                    hist_peak_intensity = total_hist[i];
            }

            PointPairList hist_list = new PointPairList();
            double x, y;
            double max_intensity = MosaicWindow.MosaicInfo.TotalMaxIntensity;

            for (int i = 0; i < 256; i++)
            {
                x = (double)(i * max_intensity / 256.0);
                y = ((double) total_hist[i] / hist_peak_intensity * 256.0);
                hist_list.Add(x, y);
            }

            if(this.histogramCurve != null)
                this.zedGraphControl.GraphPane.CurveList.Remove(this.histogramCurve);

            this.histogramCurve =
                this.zedGraphControl.GraphPane.AddCurve("Histogram", hist_list, Color.Black, SymbolType.None);

            this.histogramCurve.Line.Fill = new Fill(Color.White, Color.LightSkyBlue, -45F);
        }

        public void DisplayPlot(double min, double max)
        {
            double range;

            if (min == 0.0 && max == 0.0)
            {
                min = this.min;
                max = this.max;
            }

            if ((range = (max - min)) <= 0)
                return;

            double scale = 255.0 / range;

            this.list.Clear();

            int int_range = (int)Math.Ceiling(range);

            for (int i = 0; i <= int_range; i++)
            {
                list.Add((double)i + min, (scale * i));
            }

            if (this.curve != null)
                this.zedGraphControl.GraphPane.CurveList.Remove(this.curve);

            this.curve = new LineItem("LUT", list, Color.Blue, SymbolType.None);

            this.zedGraphControl.GraphPane.CurveList.Insert(0, curve);

            this.zedGraphControl.AxisChange();      
        }

        public void DisplayPlot()
        {
            this.DisplayPlot(this.cursor1.Position, this.cursor2.Position);
        }

        void OnCursorChangingHandler(ZedGraphWithCursorsControl sender, ZedGraphCursorEventArgs args)
        {
            // Don't allow the two cursors to overlap.
            ZedGraphCursor cursor = args.Cursor;

            if (cursor == this.cursor1)
            {
                if (this.cursor1.Position > this.cursor2.Position - this.cursor2.CursorSize)
                    this.cursor1.Position = this.cursor2.Position - this.cursor2.CursorSize;
            }
            else
            {
                if (this.cursor2.Position < this.cursor1.Position + this.cursor1.CursorSize)
                    this.cursor2.Position = this.cursor1.Position + this.cursor1.CursorSize;
            }
        }

        void OnCursorChangedHandler(ZedGraphWithCursorsControl sender, ZedGraphCursorEventArgs args)
        {
            ZedGraphCursor cursor = args.Cursor;

            if (this.cursor1.Position < Convert.ToDouble(this.minNumericUpDown.Minimum))
            {
                this.cursor1.Position = Convert.ToDouble(this.minNumericUpDown.Minimum);
                this.zedGraphControl.Invalidate();
            }

            if (this.cursor2.Position > Convert.ToDouble(this.maxNumericUpDown.Maximum))
            {
                this.cursor2.Position = Convert.ToDouble(this.maxNumericUpDown.Maximum);
                this.zedGraphControl.Invalidate();
            }

            MosaicWindow.MosaicInfo.ScaleMinIntensity = this.cursor1.Position;
            MosaicWindow.MosaicInfo.ScaleMaxIntensity = this.cursor2.Position;

            this.minNumericUpDown.Value = Convert.ToDecimal(MosaicWindow.MosaicInfo.ScaleMinIntensity);
            this.maxNumericUpDown.Value = Convert.ToDecimal(MosaicWindow.MosaicInfo.ScaleMaxIntensity);

            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }

        private void OnNormaliseClicked(object sender, EventArgs e)
        {
            this.cursor1.Position = this.min;
            MosaicWindow.MosaicInfo.ScaleMinIntensity = this.cursor1.Position;
            this.minNumericUpDown.Value = Convert.ToDecimal(this.cursor1.Position);

            this.cursor2.Position = this.max;
            MosaicWindow.MosaicInfo.ScaleMaxIntensity = this.cursor2.Position;
            this.maxNumericUpDown.Value = Convert.ToDecimal(this.cursor2.Position);

            this.DisplayPlot();
            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }

        private void OnMinMaxNumericValueChanged(object sender, EventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            if (control == this.maxNumericUpDown)
            {
                // Don't let max control be set to lower than min
                if (control.Value <= this.minNumericUpDown.Value)
                    control.Value = Math.Min(this.minNumericUpDown.Value + 1, control.Maximum);
            }

            if (control == this.minNumericUpDown)
            {
                // Don't let max control be set to lower than min
                if (control.Value >= this.maxNumericUpDown.Value)
                {
                    control.Value = Math.Max(this.maxNumericUpDown.Value - 1, control.Minimum);
                }
            }
            
            double minValue = Convert.ToDouble(this.minNumericUpDown.Value);
            double maxValue = Convert.ToDouble(this.maxNumericUpDown.Value);

            if(MosaicWindow.MosaicInfo.ScaleMinIntensity != minValue)
                MosaicWindow.MosaicInfo.ScaleMinIntensity = minValue;

            if (MosaicWindow.MosaicInfo.ScaleMaxIntensity != maxValue)
                MosaicWindow.MosaicInfo.ScaleMaxIntensity = maxValue;

            if (this.cursor1.Position != MosaicWindow.MosaicInfo.ScaleMinIntensity)
                this.cursor1.Position = MosaicWindow.MosaicInfo.ScaleMinIntensity;
            
            if(this.cursor2.Position != MosaicWindow.MosaicInfo.ScaleMaxIntensity)
                this.cursor2.Position = MosaicWindow.MosaicInfo.ScaleMaxIntensity;

            this.DisplayPlot();
            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }
    }

    public class LinearScaleTool : MosaicToggleButtonTool
    {
        private LinearScaleForm form;
        private ToolStripMenuItem menuItem;

        public LinearScaleTool(string name, MosaicWindow window)
            :
            base(name, window, "Resources.scale_icon.ico")
        {
            this.form = new LinearScaleForm(window);
            this.form.FormClosing += new FormClosingEventHandler(OnLinearScalingFormClosing);

            this.MosaicWindow.MosaicLoaded += new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnMosaicLoaded);
        
            this.menuItem = AddMenuItem("Display", "Linear Scale", null, new EventHandler(OnCheckedChanged));

            this.menuItem.Enabled = false;
        }

        void OnMosaicLoaded(MosaicWindow sender, MosaicWindowEventArgs args)
        {
            if (MosaicWindow.MosaicInfo.IsColour)
            {
                this.Enabled = false;
                this.menuItem.Enabled = false;
            }
            else
            {
                this.Enabled = true;
                this.menuItem.Enabled = true;
            }

            if(this.Active)
                this.form.UpdateGraphForLoadedImage();
        }

        public void Display()
        {
            this.Active = true;
            this.Button.Checked = true;
            this.menuItem.Checked = true;
            this.form.UpdateGraphForLoadedImage();
            this.form.DisplayPlot();
            this.form.Show();
        }

        public void Hide()
        {
            this.Active = false;
            this.Button.Checked = false;
            this.menuItem.Checked = false;
            this.form.Hide();
        }

        private void OnLinearScalingFormClosing(object sender, FormClosingEventArgs e)
        {
            this.Active = false;
            e.Cancel = true;
            this.form.Hide();
        }

        protected void OnCheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (MosaicWindow.MosaicInfo == null || MosaicWindow.MosaicInfo.IsColour)
                return;

            if (item.Checked)
            {
                Display();
            }
            else
            {
                Hide();
            }
        }

        protected override void OnToggleButtonToolSelected(object sender, EventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;

            if (MosaicWindow.MosaicInfo == null || MosaicWindow.MosaicInfo.IsColour)
                return;

            if (item.Checked)
            {
                Display();
            }
            else
            {
                Hide();
            }
        }
    }
}