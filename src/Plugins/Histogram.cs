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
    public partial class Histogram : Form
    {
        private MosaicWindow mosaicWindow;

        public Histogram(MosaicWindow window)
        {
            InitializeComponent();

            mosaicWindow = window;
        }

        private void Histogram_Load(object sender, EventArgs e)
        {
            CreateGraph(zedGraphControl);
            // Size the control to fill the form with a margin

            SetSize();
        }

        // SetSize() is separate from Resize() so we can 
        // call it independently from the Form1_Load() method
        // This leaves a 10 px margin around the outside of the control
        // Customize this to fit your needs
        private void SetSize()
        {
            zedGraphControl.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control

            zedGraphControl.Size = new Size(ClientRectangle.Width - 20,
                                    ClientRectangle.Height - 20);
        }

        private void Histogram_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void CreateGraph(ZedGraphControl zgc)
        {
            // Get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;

            // Set the Titles
            myPane.Title.Text = "Intensity Histogram";
            myPane.XAxis.Title.Text = "Intensity";
            myPane.YAxis.Title.Text = "Count";

            TileView viewer = this.mosaicWindow.TileView;

            List<Tile> tiles = viewer.GetVisibleTiles();

            if(tiles == null)
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

            if (MosaicWindow.MosaicInfo.ColorDepth >= 24)
                colourImage = true;

            for (int i = 0; i < total_red_hist.Length; i++)
            {
                total_red_hist[i] = 0;
                total_green_hist[i] = 0;
                total_blue_hist[i] = 0;
            }

            double max_posible_intensity_in_images = 0.0;

            foreach(Tile tile in tiles) {

                if (tile.IsDummy)
                    continue;

                FreeImageAlgorithmsBitmap dib = tile.LoadFreeImageBitmap();

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
                    x = (double) (i * range_per_bin);
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
    }
}