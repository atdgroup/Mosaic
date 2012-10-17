/*
*   Copyright 2007-2011 Glenn Pierce, Paul Barber,
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
    public partial class RGBBalanceForm : Form
    {
        private MosaicWindow mosaicWindow;
        private VerticalZedGraphCursor cursor1_1;
        private VerticalZedGraphCursor cursor1_2;
        private VerticalZedGraphCursor cursor2_1;
        private VerticalZedGraphCursor cursor2_2;
        private VerticalZedGraphCursor cursor3_1;
        private VerticalZedGraphCursor cursor3_2;
        private double min;
        private double max;
        private double max_possible;
        private PointPairList list1;
        private PointPairList list2;
        private PointPairList list3;
//        private PointPairList hist_list1;          // Plotting the histograms has been disabled
//        private PointPairList hist_list2;           // - the histograms would come from the thumbnails but these will depend here on 
//        private PointPairList hist_list3;           //the min and max scale values chosen and may not represent the histograms well at all, even if max value is known
        private LineItem curve1;
        private LineItem curve2;
        private LineItem curve3;
//        private LineItem histogramCurve1;
//        private LineItem histogramCurve2;
//        private LineItem histogramCurve3;

        public RGBBalanceForm(MosaicWindow window)
        {
            InitializeComponent();

            mosaicWindow = window;

            // Setup graphs
            this.zedGraphControl1.GraphPane.Title.IsVisible = false;
            this.zedGraphControl2.GraphPane.Title.IsVisible = false;
            this.zedGraphControl3.GraphPane.Title.IsVisible = false;

            this.zedGraphControl1.GraphPane.Legend.IsVisible = false;
            this.zedGraphControl2.GraphPane.Legend.IsVisible = false;
            this.zedGraphControl3.GraphPane.Legend.IsVisible = false;

            this.zedGraphControl1.GraphPane.XAxis.Title.IsVisible = false;
            this.zedGraphControl1.GraphPane.YAxis.Title.IsVisible = false;
            this.zedGraphControl2.GraphPane.XAxis.Title.IsVisible = false;
            this.zedGraphControl2.GraphPane.YAxis.Title.IsVisible = false;
            this.zedGraphControl3.GraphPane.XAxis.Title.IsVisible = false;
            this.zedGraphControl3.GraphPane.YAxis.Title.IsVisible = false;

            this.zedGraphControl1.GraphPane.XAxis.Scale.IsVisible = true;
            this.zedGraphControl2.GraphPane.XAxis.Scale.IsVisible = true;
            this.zedGraphControl3.GraphPane.XAxis.Scale.IsVisible = true;

            this.zedGraphControl1.GraphPane.XAxis.Scale.FontSpec.Size = 30.0F;
            this.zedGraphControl2.GraphPane.XAxis.Scale.FontSpec.Size = 30.0F;
            this.zedGraphControl3.GraphPane.XAxis.Scale.FontSpec.Size = 30.0F;

            this.zedGraphControl1.GraphPane.YAxis.Scale.IsVisible = false;
            this.zedGraphControl2.GraphPane.YAxis.Scale.IsVisible = false;
            this.zedGraphControl3.GraphPane.YAxis.Scale.IsVisible = false;

            this.zedGraphControl1.GraphPane.XAxis.MajorTic.Size = 0;
            this.zedGraphControl1.GraphPane.YAxis.MajorTic.Size = 0;
            this.zedGraphControl2.GraphPane.XAxis.MajorTic.Size = 0;
            this.zedGraphControl2.GraphPane.YAxis.MajorTic.Size = 0;
            this.zedGraphControl3.GraphPane.XAxis.MajorTic.Size = 0;
            this.zedGraphControl3.GraphPane.YAxis.MajorTic.Size = 0;

            this.zedGraphControl1.GraphPane.XAxis.MinorTic.Size = 0;
            this.zedGraphControl1.GraphPane.YAxis.MinorTic.Size = 0;
            this.zedGraphControl2.GraphPane.XAxis.MinorTic.Size = 0;
            this.zedGraphControl2.GraphPane.YAxis.MinorTic.Size = 0;
            this.zedGraphControl3.GraphPane.XAxis.MinorTic.Size = 0;
            this.zedGraphControl3.GraphPane.YAxis.MinorTic.Size = 0;

            this.list1 = new PointPairList();
            this.list2 = new PointPairList();
            this.list3 = new PointPairList();
//            this.hist_list1 = new PointPairList();
//            this.hist_list2 = new PointPairList();
//            this.hist_list3 = new PointPairList();

            this.cursor1_1 = new VerticalZedGraphCursor(this.zedGraphControl1, Color.Red);
            this.cursor1_2 = new VerticalZedGraphCursor(this.zedGraphControl1, Color.Green);
            this.cursor2_1 = new VerticalZedGraphCursor(this.zedGraphControl2, Color.Red);
            this.cursor2_2 = new VerticalZedGraphCursor(this.zedGraphControl2, Color.Green);
            this.cursor3_1 = new VerticalZedGraphCursor(this.zedGraphControl3, Color.Red);
            this.cursor3_2 = new VerticalZedGraphCursor(this.zedGraphControl3, Color.Green);

            zedGraphControl1.ZedGraphCursorChangingHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangingHandler);
            zedGraphControl2.ZedGraphCursorChangingHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangingHandler);
            zedGraphControl3.ZedGraphCursorChangingHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangingHandler);

            zedGraphControl1.ZedGraphCursorChangedHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangedHandler);
            zedGraphControl2.ZedGraphCursorChangedHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangedHandler);
            zedGraphControl3.ZedGraphCursorChangedHandler +=
                new ZedGraphCursorHandler<ZedGraphWithCursorsControl, ZedGraphCursorEventArgs>(OnCursorChangedHandler);

            this.cursor1_1.Position = this.min;
            this.cursor1_2.Position = this.max;
            this.cursor2_1.Position = this.min;
            this.cursor2_2.Position = this.max;
            this.cursor3_1.Position = this.min;
            this.cursor3_2.Position = this.max;

            this.zedGraphControl1.AddVerticalCursor(this.cursor1_1);
            this.zedGraphControl1.AddVerticalCursor(this.cursor1_2);
            this.zedGraphControl2.AddVerticalCursor(this.cursor2_1);
            this.zedGraphControl2.AddVerticalCursor(this.cursor2_2);
            this.zedGraphControl3.AddVerticalCursor(this.cursor3_1);
            this.zedGraphControl3.AddVerticalCursor(this.cursor3_2);
        }

        private void RGBBalance_Load(object sender, EventArgs e)
        {
            UpdateUIForLoadedImage();      
  
        }

        private void setComboBoxForFICC(ComboBox comboBox, FREE_IMAGE_COLOR_CHANNEL ficc)
        {
            if (ficc == FREE_IMAGE_COLOR_CHANNEL.FICC_RED)
                comboBox.Text = "Red";

            if (ficc == FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN)
                comboBox.Text = "Green";

            if (ficc == FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE)
                comboBox.Text = "Blue";
        }

        public void UpdateUIForLoadedImage()
        {         
            TileView viewer = this.mosaicWindow.TileView;
/*
            CalculateGreyLevelHistogramShape(Tile.channel[0], hist_list1, Tile.TotalMaxIntensity[0]);
            if (this.histogramCurve1 != null)
                this.zedGraphControl1.GraphPane.CurveList.Remove(this.histogramCurve1);
            this.histogramCurve1 = this.zedGraphControl1.GraphPane.AddCurve("Histogram", hist_list1, Color.Black, SymbolType.None);
            this.histogramCurve1.Line.Fill = new Fill(Color.White, Color.LightSkyBlue, -45F);

            CalculateGreyLevelHistogramShape(Tile.channel[1], hist_list2, Tile.TotalMaxIntensity[1]);
            if (this.histogramCurve2 != null)
                this.zedGraphControl2.GraphPane.CurveList.Remove(this.histogramCurve2);
            this.histogramCurve2 = this.zedGraphControl2.GraphPane.AddCurve("Histogram", hist_list2, Color.Black, SymbolType.None);
            this.histogramCurve2.Line.Fill = new Fill(Color.White, Color.LightSkyBlue, -45F);

            CalculateGreyLevelHistogramShape(Tile.channel[2], hist_list3, Tile.TotalMaxIntensity[2]);
            if (this.histogramCurve3 != null)
                this.zedGraphControl3.GraphPane.CurveList.Remove(this.histogramCurve3);
            this.histogramCurve3 = this.zedGraphControl3.GraphPane.AddCurve("Histogram", hist_list3, Color.Black, SymbolType.None);
            this.histogramCurve3.Line.Fill = new Fill(Color.White, Color.LightSkyBlue, -45F);
*/

//            double min_in_images = Double.MaxValue, max_in_images = Double.MinValue;
            double max_posible_intensity = 0.0;

            if (!MosaicWindow.MosaicInfo.IsColour)
            {
                return;
            }

            // load 1 image to get the max possible grey value for the channels
            List<Tile> tiles = viewer.GetVisibleTiles();
            
            if (tiles == null)
                return;

            FreeImageAlgorithmsBitmap dib = Tile.LoadFreeImageBitmapFromFile(tiles[0].FilePath);  // get on e image from one of the channels
//            max_posible_intensity = dib.MaximumPossibleIntensityValue;
            max_posible_intensity = Utilities.guessFibMaxValue(dib);
            dib.Dispose();

            this.min = 0;
            this.max = max_posible_intensity;
            this.max_possible = max_posible_intensity;

            this.minNumericUpDown1.Minimum = Convert.ToDecimal(0);
            this.maxNumericUpDown1.Minimum = Convert.ToDecimal(1);
            this.minNumericUpDown1.Maximum = Convert.ToDecimal(this.max_possible - 1);
            this.maxNumericUpDown1.Maximum = Convert.ToDecimal(this.max_possible);
            this.minNumericUpDown2.Minimum = Convert.ToDecimal(0);
            this.maxNumericUpDown2.Minimum = Convert.ToDecimal(1);
            this.minNumericUpDown2.Maximum = Convert.ToDecimal(this.max_possible - 1);
            this.maxNumericUpDown2.Maximum = Convert.ToDecimal(this.max_possible);
            this.minNumericUpDown3.Minimum = Convert.ToDecimal(0);
            this.maxNumericUpDown3.Minimum = Convert.ToDecimal(1);
            this.minNumericUpDown3.Maximum = Convert.ToDecimal(this.max_possible - 1);
            this.maxNumericUpDown3.Maximum = Convert.ToDecimal(this.max_possible);

            this.minNumericUpDown1.Value = Convert.ToDecimal(Tile.scaleMin[0]);
            this.minNumericUpDown2.Value = Convert.ToDecimal(Tile.scaleMin[1]);
            this.minNumericUpDown3.Value = Convert.ToDecimal(Tile.scaleMin[2]);
            this.maxNumericUpDown1.Value = Convert.ToDecimal(Tile.scaleMax[0]);
            this.maxNumericUpDown2.Value = Convert.ToDecimal(Tile.scaleMax[1]);
            this.maxNumericUpDown3.Value = Convert.ToDecimal(Tile.scaleMax[2]);

            this.numericUpDown1X.Value = Convert.ToDecimal(Tile.channelShift[1].X);
            this.numericUpDown1Y.Value = Convert.ToDecimal(Tile.channelShift[1].Y);
            this.numericUpDown2X.Value = Convert.ToDecimal(Tile.channelShift[2].X);
            this.numericUpDown2Y.Value = Convert.ToDecimal(Tile.channelShift[2].Y);

            this.channelTitle1.Text = Tile.channelPrefix[0];
            this.channelTitle2.Text = Tile.channelPrefix[1];
            this.channelTitle3.Text = Tile.channelPrefix[2];

            setComboBoxForFICC(this.comboBox1, Tile.channel[0]);
            setComboBoxForFICC(this.comboBox2, Tile.channel[1]);
            setComboBoxForFICC(this.comboBox3, Tile.channel[2]);

            this.zedGraphControl1.GraphPane.XAxis.Scale.Min = 0;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Max = this.max;
            this.zedGraphControl1.GraphPane.YAxis.Scale.Min = 0;
            this.zedGraphControl1.GraphPane.YAxis.Scale.Max = 255;
            this.zedGraphControl2.GraphPane.XAxis.Scale.Min = 0;
            this.zedGraphControl2.GraphPane.XAxis.Scale.Max = this.max;
            this.zedGraphControl2.GraphPane.YAxis.Scale.Min = 0;
            this.zedGraphControl2.GraphPane.YAxis.Scale.Max = 255;
            this.zedGraphControl3.GraphPane.XAxis.Scale.Min = 0;
            this.zedGraphControl3.GraphPane.XAxis.Scale.Max = this.max;
            this.zedGraphControl3.GraphPane.YAxis.Scale.Min = 0;
            this.zedGraphControl3.GraphPane.YAxis.Scale.Max = 255;

            this.cursor1_1.Position = Tile.scaleMin[0];
            this.cursor1_2.Position = Tile.scaleMax[0];
            this.cursor2_1.Position = Tile.scaleMin[1];
            this.cursor2_2.Position = Tile.scaleMax[1];
            this.cursor3_1.Position = Tile.scaleMin[2];
            this.cursor3_2.Position = Tile.scaleMax[2];

            this.DisplayPlot();

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl2.AxisChange();
            this.zedGraphControl3.AxisChange();
        }
/*
        private void CalculateGreyLevelHistogramShape(FREE_IMAGE_COLOR_CHANNEL ficc, PointPairList list, double max_intensity)
        {
            TileView viewer = this.mosaicWindow.TileView;

            List<Tile> tiles = MosaicWindow.MosaicInfo.Items;

            if (tiles == null)
                return;

            int[] hist = null;
            int[] total_hist = new int[256];

            if (viewer.CacheImageAvailiable)
            {
                viewer.CacheImage.GetHistogram(ficc, out total_hist);
            }
            else
            {
                foreach (Tile tile in tiles)
                {
                    tile.Thumbnail.GetHistogram(ficc, out hist);

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

            double x=0.0, y;

            list.Clear();

            for (int i = 0; i < 256; i++)
            {
                x = (double)(i * max_intensity / 256.0);
                y = ((double)total_hist[i] / hist_peak_intensity * 256.0);
                list.Add(x, y);
            }
        }
*/
        public void DisplayPlot(double min, double max, PointPairList list)
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

            list.Clear();

            int int_range = (int)Math.Ceiling(range);

            for (int i = 0; i <= int_range; i++)
            {
                list.Add((double)i + min, (scale * i));
            }
        }

        public void DisplayPlot(int c)
        {
            switch (c)
            {
                case 1:
                    this.DisplayPlot(this.cursor1_1.Position, this.cursor1_2.Position, this.list1); 
                    if (this.curve1 != null)
                        this.zedGraphControl1.GraphPane.CurveList.Remove(this.curve1);
                    this.curve1 = new LineItem("LUT", list1, Color.Blue, SymbolType.None);
                    this.zedGraphControl1.GraphPane.CurveList.Insert(0, this.curve1);
                    this.zedGraphControl1.AxisChange();

                    break;
                case 2:
                    this.DisplayPlot(this.cursor2_1.Position, this.cursor2_2.Position, this.list2);
                    if (this.curve2 != null)
                        this.zedGraphControl2.GraphPane.CurveList.Remove(this.curve2);
                    this.curve2 = new LineItem("LUT", list2, Color.Blue, SymbolType.None);
                    this.zedGraphControl2.GraphPane.CurveList.Insert(0, this.curve2);
                    this.zedGraphControl2.AxisChange();

                    break;
                case 3:
                    this.DisplayPlot(this.cursor3_1.Position, this.cursor3_2.Position, this.list3);
                    if (this.curve3 != null)
                        this.zedGraphControl3.GraphPane.CurveList.Remove(this.curve3);
                    this.curve3 = new LineItem("LUT", list3, Color.Blue, SymbolType.None);
                    this.zedGraphControl3.GraphPane.CurveList.Insert(0, this.curve3);
                    this.zedGraphControl3.AxisChange();

                    break;
            }
        }

        public void DisplayPlot()
        {
            this.DisplayPlot(1);
            this.DisplayPlot(2);
            this.DisplayPlot(3);
        }

        void OnCursorChangingHandler(ZedGraphWithCursorsControl sender, ZedGraphCursorEventArgs args)
        {
            // Don't allow the two cursors to overlap.
            ZedGraphCursor cursor = args.Cursor;

            if (cursor == this.cursor1_1)
            {
                if (this.cursor1_1.Position > this.cursor1_2.Position - this.cursor1_2.CursorSize)
                    this.cursor1_1.Position = this.cursor1_2.Position - this.cursor1_2.CursorSize;
                DisplayPlot(1);
            }
            else if (cursor == this.cursor1_2)
            {
                if (this.cursor1_2.Position < this.cursor1_1.Position + this.cursor1_1.CursorSize)
                    this.cursor1_2.Position = this.cursor1_1.Position + this.cursor1_1.CursorSize;
                DisplayPlot(1);
            }
            else if (cursor == this.cursor2_1)
            {
                if (this.cursor2_1.Position > this.cursor2_2.Position + this.cursor2_2.CursorSize)
                    this.cursor2_1.Position = this.cursor2_2.Position + this.cursor2_2.CursorSize;
                DisplayPlot(2);
            }
            else if (cursor == this.cursor2_2)
            {
                if (this.cursor2_2.Position < this.cursor2_1.Position + this.cursor2_1.CursorSize)
                    this.cursor2_2.Position = this.cursor2_1.Position + this.cursor2_1.CursorSize;
                DisplayPlot(2);
            }
            else if (cursor == this.cursor3_1)
            {
                if (this.cursor3_1.Position > this.cursor3_2.Position + this.cursor3_2.CursorSize)
                    this.cursor3_1.Position = this.cursor3_2.Position + this.cursor3_2.CursorSize;
                DisplayPlot(3);
            }
            else if (cursor == this.cursor3_2)
            {
                if (this.cursor3_2.Position < this.cursor3_1.Position + this.cursor3_1.CursorSize)
                    this.cursor3_2.Position = this.cursor3_1.Position + this.cursor3_1.CursorSize;
                DisplayPlot(3);
            }
        }

        void OnCursorChangedHandler(ZedGraphWithCursorsControl sender, ZedGraphCursorEventArgs args)
        {
            ZedGraphCursor cursor = args.Cursor;

            if (this.cursor1_1.Position < Convert.ToDouble(this.minNumericUpDown1.Minimum))
            {
                this.cursor1_1.Position = Convert.ToDouble(this.minNumericUpDown1.Minimum);
                this.zedGraphControl1.Invalidate();
            }

            if (this.cursor1_2.Position > Convert.ToDouble(this.maxNumericUpDown1.Maximum))
            {
                this.cursor1_2.Position = Convert.ToDouble(this.maxNumericUpDown1.Maximum);
                this.zedGraphControl1.Invalidate();
            }

            if (this.cursor2_1.Position < Convert.ToDouble(this.minNumericUpDown2.Minimum))
            {
                this.cursor2_1.Position = Convert.ToDouble(this.minNumericUpDown2.Minimum);
                this.zedGraphControl2.Invalidate();
            }

            if (this.cursor2_2.Position > Convert.ToDouble(this.maxNumericUpDown2.Maximum))
            {
                this.cursor2_2.Position = Convert.ToDouble(this.maxNumericUpDown2.Maximum);
                this.zedGraphControl2.Invalidate();
            }

            if (this.cursor3_1.Position < Convert.ToDouble(this.minNumericUpDown3.Minimum))
            {
                this.cursor3_1.Position = Convert.ToDouble(this.minNumericUpDown3.Minimum);
                this.zedGraphControl3.Invalidate();
            }

            if (this.cursor3_2.Position > Convert.ToDouble(this.maxNumericUpDown3.Maximum))
            {
                this.cursor3_2.Position = Convert.ToDouble(this.maxNumericUpDown3.Maximum);
                this.zedGraphControl3.Invalidate();
            }

            Tile.scaleMin[0] = this.cursor1_1.Position;
            Tile.scaleMax[0] = this.cursor1_2.Position;
            Tile.scaleMin[1] = this.cursor2_1.Position;
            Tile.scaleMax[1] = this.cursor2_2.Position;
            Tile.scaleMin[2] = this.cursor3_1.Position;
            Tile.scaleMax[2] = this.cursor3_2.Position;

            this.minNumericUpDown1.Value = Convert.ToDecimal(Tile.scaleMin[0]);
            this.maxNumericUpDown1.Value = Convert.ToDecimal(Tile.scaleMax[0]);
            this.minNumericUpDown2.Value = Convert.ToDecimal(Tile.scaleMin[1]);
            this.maxNumericUpDown2.Value = Convert.ToDecimal(Tile.scaleMax[1]);
            this.minNumericUpDown3.Value = Convert.ToDecimal(Tile.scaleMin[2]);
            this.maxNumericUpDown3.Value = Convert.ToDecimal(Tile.scaleMax[2]);

            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }

        private void OnMinMaxNumericValueChanged(object sender, EventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            if (control == this.maxNumericUpDown1)
            {
                // Don't let max control be set to lower than min
                if (control.Value <= this.minNumericUpDown1.Value)
                    control.Value = Math.Min(this.minNumericUpDown1.Value + 1, control.Maximum);

                Tile.scaleMax[0] = Convert.ToDouble(this.maxNumericUpDown1.Value);
                this.cursor1_2.Position = Tile.scaleMax[0];
                this.DisplayPlot(1);
            }

            if (control == this.minNumericUpDown1)
            {
                // Don't let max control be set to lower than min
                if (control.Value >= this.maxNumericUpDown1.Value)
                {
                    control.Value = Math.Max(this.maxNumericUpDown1.Value - 1, control.Minimum);
                }

                Tile.scaleMin[0] = Convert.ToDouble(this.minNumericUpDown1.Value);
                this.cursor1_1.Position = Tile.scaleMin[0];
                this.DisplayPlot(1);
            }

            if (control == this.maxNumericUpDown2)
            {
                // Don't let max control be set to lower than min
                if (control.Value <= this.minNumericUpDown2.Value)
                    control.Value = Math.Min(this.minNumericUpDown2.Value + 1, control.Maximum);

                Tile.scaleMax[1] = Convert.ToDouble(this.maxNumericUpDown2.Value);
                this.cursor2_2.Position = Tile.scaleMax[1];
                this.DisplayPlot(2);
            }

            if (control == this.minNumericUpDown2)
            {
                // Don't let max control be set to lower than min
                if (control.Value >= this.maxNumericUpDown2.Value)
                {
                    control.Value = Math.Max(this.maxNumericUpDown2.Value - 1, control.Minimum);
                }

                Tile.scaleMin[1] = Convert.ToDouble(this.minNumericUpDown2.Value);
                this.cursor2_1.Position = Tile.scaleMin[1];
                this.DisplayPlot(2);
            }

            if (control == this.maxNumericUpDown3)
            {
                // Don't let max control be set to lower than min
                if (control.Value <= this.minNumericUpDown3.Value)
                    control.Value = Math.Min(this.minNumericUpDown3.Value + 1, control.Maximum);

                Tile.scaleMax[2] = Convert.ToDouble(this.maxNumericUpDown3.Value);
                this.cursor3_2.Position = Tile.scaleMax[2];
                this.DisplayPlot(3);
            }

            if (control == this.minNumericUpDown3)
            {
                // Don't let max control be set to lower than min
                if (control.Value >= this.maxNumericUpDown3.Value)
                {
                    control.Value = Math.Max(this.maxNumericUpDown3.Value - 1, control.Minimum);
                }

                Tile.scaleMin[2] = Convert.ToDouble(this.minNumericUpDown3.Value);
                this.cursor3_1.Position = Tile.scaleMin[2];
                this.DisplayPlot(3);
            }

            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }

        private void checkChannelAssignment(int c)
        {
            // Check that if 2 channels are the same, change one so that it is different to the other 2
            // param c indicates the channel that has just been set by the user and therefore should not be changed
            // never expect 3 the same

            int cToChange=-1, i, theThirdc;

            for (i = 0; i < 3; i++)  // find which need to be changed
            {
                if (i != c)
                {
                    if (Tile.channel[i] == Tile.channel[c])
                        cToChange = i;    
                }
            }

            if (cToChange < 0)   // nothing to do
                return;

            // find the 3rd one
            theThirdc = 3 - c - cToChange;

            // now change the right one to the missing channel - there must be a better way ...
            if (Tile.channel[c] == FREE_IMAGE_COLOR_CHANNEL.FICC_RED)
            {
                if (Tile.channel[theThirdc] == FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN)
                    Tile.channel[cToChange] = FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE;
                else
                    Tile.channel[cToChange] = FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN;
            }
            else if (Tile.channel[c] == FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN)
            {
                if (Tile.channel[theThirdc] == FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE)
                    Tile.channel[cToChange] = FREE_IMAGE_COLOR_CHANNEL.FICC_RED;
                else
                    Tile.channel[cToChange] = FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE;
            }
            else if (Tile.channel[c] == FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE)
            {
                if (Tile.channel[theThirdc] == FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN)
                    Tile.channel[cToChange] = FREE_IMAGE_COLOR_CHANNEL.FICC_RED;
                else
                    Tile.channel[cToChange] = FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN;
            }
        }

        private void OnRGBComboIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selection = (string)comboBox.SelectedItem;

            int channel=0;   // default assume first channel, comboBox1
            if (comboBox == this.comboBox2)
                channel = 1;
            else if (comboBox == this.comboBox3)
                channel = 2;

            if  (selection.Equals("Red"))
                Tile.channel[channel] = FREE_IMAGE_COLOR_CHANNEL.FICC_RED;
            else if  (selection.Equals("Green"))
                Tile.channel[channel] = FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN;
            else if  (selection.Equals("Blue"))
                Tile.channel[channel] = FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE;

            checkChannelAssignment(channel);

            setComboBoxForFICC(this.comboBox1, Tile.channel[0]);
            setComboBoxForFICC(this.comboBox2, Tile.channel[1]);
            setComboBoxForFICC(this.comboBox3, Tile.channel[2]);

            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }

        private void OnChannelShiftChanged(object sender, EventArgs e)
        {
            Tile.channelShift[0].X = 0;
            Tile.channelShift[0].Y = 0;
            Tile.channelShift[1].X = Convert.ToInt32(this.numericUpDown1X.Value);
            Tile.channelShift[1].Y = Convert.ToInt32(this.numericUpDown1Y.Value);
            Tile.channelShift[2].X = Convert.ToInt32(this.numericUpDown2X.Value);
            Tile.channelShift[2].Y = Convert.ToInt32(this.numericUpDown2Y.Value);

            this.mosaicWindow.TileView.Clear(); // This possibly clears the intermidiate image
            this.mosaicWindow.TileView.Redraw();
        }

    }

    public class RGBBalanceTool : MosaicToggleButtonTool
    {
        private RGBBalanceForm form;
        private ToolStripMenuItem menuItem;

        public RGBBalanceTool(string name, MosaicWindow window)
            :
            base(name, window, "Resources.scale_icon.ico")
        {
            this.form = new RGBBalanceForm(window);
            this.form.FormClosing += new FormClosingEventHandler(OnLinearScalingFormClosing);

            this.MosaicWindow.MosaicLoaded += new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnMosaicLoaded);
        
            this.menuItem = AddMenuItem("Display", "RGB Balance", null, new EventHandler(OnCheckedChanged));

            this.menuItem.Enabled = false;
        }

        void OnMosaicLoaded(MosaicWindow sender, MosaicWindowEventArgs args)
        {
            if (MosaicWindow.MosaicInfo.IsCompositeRGB)
            {
                this.Enabled = true;
                this.menuItem.Enabled = true;
            }
            else
            {
                this.Enabled = false;
                this.menuItem.Enabled = false;
            }

            if(this.Active)
                this.form.UpdateUIForLoadedImage();
        }

        public void Display()
        {
            this.Active = true;
            this.Button.Checked = true;
            this.menuItem.Checked = true;
            this.form.UpdateUIForLoadedImage();
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

            if (MosaicWindow.MosaicInfo == null || !MosaicWindow.MosaicInfo.IsCompositeRGB)
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