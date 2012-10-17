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
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using ExtensionMethods;

using FreeImageAPI;
using ThreadingSystem;

namespace ImageStitching
{
    public partial class CorrelatorWindow : Form
    {
        private static CorrelatorWindow instance = null;
        private Correlator correlator = null;   // Identifies the method for correlation and can be replace by another
      
        private MosaicWindow mosaicWindow;

        #if DEBUG
        private CorrelationDebugForm debugForm;
        #endif

        private CorrelationTile currentTile;

        public static CorrelatorWindow GetCorrelatorWindowInstance(MosaicWindow mosaicWindow)
        {
            if(CorrelatorWindow.instance == null)
                CorrelatorWindow.instance = new CorrelatorWindow(mosaicWindow);

            return CorrelatorWindow.instance;
        }

        private void Initialise()
        {
            #if DEBUG
            this.debugForm = new CorrelationDebugForm();
            this.debugForm.BackgroundImageView.ZoomToFit = true;
            this.debugForm.TileImageView.ZoomToFit = true;
            this.debugForm.Show();
            #endif

            this.mosaicWindow.MosaicLoaded += new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnMosaicLoaded);
            this.tiledImageViewer.TileImageViewMouseDownHandler += new TileImageViewMouseDelegate<TiledImageView, TiledImageViewMouseEventArgs>(OnTileImageViewMouseDownHandler);
            this.mosaicWindow.UsingCorrelationsChanged += new MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs>(OnUsingCorrelationsChanged);
            
            this.useCorrelationCheckBox.Checked = MosaicWindow.MosaicInfo.IsCorrelated;

            this.tiledImageViewer.Initialise(MosaicWindow.MosaicInfo, new Rectangle(0, 0, MosaicWindow.MosaicInfo.TotalWidth, MosaicWindow.MosaicInfo.TotalHeight),
                                                                new Size(1000, 1000), true);  // forcing the tiledimageviewer to be grayscale here

            this.tiledImageViewer.Paint += new PaintEventHandler(OnImageViewPaint);

            #if DEBUG
            this.debugForm.BackgroundImageView.Paint += new PaintEventHandler(OnDebugBackgroundImageViewPaint);
            this.debugForm.TileImageView.Paint += new PaintEventHandler(OnDebugTileImageImageViewPaint);
            #endif
      
            this.correlationTypeComboBox.SelectedIndex = 1;
            SetCorrelationForCombobox();
        }

        private CorrelatorWindow(MosaicWindow mosaicWindow)
        {
            this.mosaicWindow = mosaicWindow;

            InitializeComponent();

            this.Initialise();
        }

        void OnUsingCorrelationsChanged(MosaicWindow sender, MosaicWindowEventArgs args)
        {
            this.useCorrelationCheckBox.Checked = sender.CorrelationEnabled;
        }

        void OnTileImageViewMouseDownHandler(TiledImageView sender, TiledImageViewMouseEventArgs args)
        {
            // Causes scroll messages.
            if (args.ImageViewMouseEventArgs.MouseEventArgs.Button != MouseButtons.Left)
                return;

            Point pt = args.VirtualAreaPosition;

            pt = Utilities.ZoomPoint(pt, this.mosaicWindow.TileView.Zoom);

            pt.X -= this.mosaicWindow.TileView.ClientRectangle.Width / 2;
            pt.Y -= this.mosaicWindow.TileView.ClientRectangle.Height / 2;

            this.mosaicWindow.TileView.SetAutoScrollPosition(pt);

            this.mosaicWindow.TileView.Redraw();
            this.mosaicWindow.TileView.Refresh();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            StopCorrelation();

            e.Cancel = true;
 
            CorrelatorWindow.instance.Hide();
        }

        void OnDebugBackgroundImageViewPaint(object sender, PaintEventArgs e)
        {
            #if DEBUG

            correlator.DebugBackgroundPaint(this.debugForm, e);

            #endif
        }

        void OnDebugTileImageImageViewPaint(object sender, PaintEventArgs e)
        {
            #if DEBUG
            if (correlator == null || this.currentTile == null)
                return;

            correlator.DebugCurrentImagePaint(this.debugForm, e);

            e.Graphics.Transform = new Matrix();

            // Draw Tile Name
            Font drawFont = new Font("Arial", 20);
            SolidBrush drawBrush = new SolidBrush(Color.Red);

            e.Graphics.DrawString(this.currentTile.Tile.FileName, drawFont, drawBrush, new PointF(20, 20));
            #endif
        }

        void OnImageViewPaint(object sender, PaintEventArgs e)
        {
            if (correlator == null || this.currentTile == null)
                return;

            e.Graphics.Transform = this.tiledImageViewer.GetTransform();

            // Mark all the tiles we will not correlate
            foreach (CorrelationTile t in this.correlator.CorrelationTiles)
            {
                if (t.CorrelationFailed)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(80, 255, 0, 0)), t.Tile.Bounds);
                }
            }
            
            int width = (int)(5.0f / (Math.Min(this.tiledImageViewer.XScaleFactor, this.tiledImageViewer.YScaleFactor)));

            // Draw Rectangle Bounds
            if(this.currentTile != null)
                e.Graphics.DrawRectangle(new Pen(Color.LightGreen, width), this.currentTile.Tile.Bounds);
        }

        private void  OnTileCorrelationBegin(CorrelationTile tile, FreeImageAlgorithmsBitmap fib)
        {
            this.useCorrelationCheckBox.Checked = false;
            this.currentTile = tile;
        
            #if DEBUG
     
            FreeImageAlgorithmsBitmap fg = new FreeImageAlgorithmsBitmap(fib);

            fg.ConvertToStandardType(true);
            fg.ConvertTo24Bits();
      
            this.debugForm.TileImageView.Image = fg.ToBitmap();

            this.debugForm.BackgroundImageView.Refresh();
            this.debugForm.TileImageView.Refresh();
   
            fg.Dispose();

            #endif
        }

        private void OnTileCorrelated(CorrelationTile tile, Rectangle bounds, FreeImageAlgorithmsBitmap fib, bool success)
        {
            this.tiledImageViewer.AddTile(bounds, fib);

            this.tiledImageViewer.Refresh();

            #if DEBUG
            if (success == false)
            {
                FreeImageAlgorithmsBitmap bg = new FreeImageAlgorithmsBitmap(this.correlator.BackgroundImage);
                bg.ConvertTo24Bits();
                bg.DrawColourRect(tile.OriginalBoundsRelativeToOrigin, Color.Red, 2);
                this.debugForm.BackgroundImageView.Image = bg.ToBitmap();      
                bg.Dispose();
            }

            this.debugForm.Refresh();
            #endif
        }

        private void TextSent(string text, Color color)
        {
            this.textBox.AppendColouredText(text, color);
            // Causes Issues with stopping thread and focus lost.
            //this.textBox.Focus();
            //this.textBox.SelectionStart = textBox.Text.Length;
            //this.textBox.ScrollToCaret();
        }

        private void TileCorrelationCompleted(string updateText, bool aborted)
        {
            this.currentTile = null;

            this.tiledImageViewer.Invalidate();

            this.progressBar.Value = 0;

            if (updateText != null)
                this.textBox.AppendText(updateText);

            TimeSpan duration = this.correlator.TimeTaken();

            this.timeTakenStatusLabel.Text = "Time Taken: " +
                duration.Hours.ToString("00") + ":" +
                duration.Minutes.ToString("00") + ":" +
                duration.Seconds.ToString("00");

            MosaicWindow.MosaicInfo.IsCorrelated = true;

            this.useCorrelationCheckBox.Checked = true;
            this.mosaicWindow.CorrelationEnabled = true;

            // Enable start button again
            this.correlateButton.Enabled = true;

            // Update the cache file with correlation positions
            this.mosaicWindow.SaveCache();
        }

        private void TileCorrelationProgressUpdate(string updateText, int percentage)
        {
            if (updateText != null)
                TextSent(updateText, Color.Black);

            this.progressBar.Value = percentage;

            this.tiledImageViewer.ReloadImage();

            this.toolStripStatusLabel.Text = String.Format("Number of files correlated {0} / {1} Failed correlations = {2}",
                correlator.NumberOfCorrelatedTiles, MosaicWindow.MosaicInfo.Items.Count, correlator.NumberOfFailedCorrelations);       
        }

        private void OnUseCorrelationsChanged(object sender, EventArgs e)
        {       
            this.mosaicWindow.CorrelationEnabled = this.useCorrelationCheckBox.Checked;
        }

        private void CorrlationPausedOrResumed(bool paused)
        {
            if (paused)
            {
                this.correlatorPauseButton.Text = "Resume";           
            }
            else
            {
                this.correlatorPauseButton.Text = "Pause ";
            }
        }

        private void OnCorrelatePauseButtonToggled(object sender, EventArgs e)
        {
            Button item = (Button)sender;

            if (correlator == null)
                return;

            if (correlator.ThreadController.ThreadPaused)
            {
                correlator.CorrelationResume();
                item.Text = "Pause ";                    
            }
            else
            {
                correlator.CorrelationPause();
                item.Text = "Resume";  
            }
        }

        private void OnCorrelateStartButtonClicked(object sender, EventArgs e)
        {
            if (correlator == null)
                return;

            this.textBox.Clear();
            this.tiledImageViewer.Reset();

            correlator.StartCorrelation();

            this.correlateButton.Enabled = false;
            this.correlatorPauseButton.Enabled = true;
        }

        public void StopCorrelation()
        {
            if (correlator == null)
                return;

            correlator.StopCorrelation();
            this.correlateButton.Enabled = true;

            this.correlatorPauseButton.Text = "Pause ";
            this.correlatorPauseButton.Enabled = false;
        }

        private void OnCorrelateStopButtonClicked(object sender, EventArgs e)
        {
            StopCorrelation();
        }

        private void SetCorrelationForCombobox()
        {
            switch (this.correlationTypeComboBox.SelectedIndex)
            {
                case 0:
                    {
                        correlator = null;
                        this.correlatorOptionPanel.Controls.Clear();
                        this.correlateButton.Enabled = false;
                        this.correlatorPauseButton.Enabled = false;
                        return;
                    }

                case 1:
                    {
                        correlator = new KernalBasedOverlapCorrelator(MosaicWindow.MosaicInfo, this.tiledImageViewer);

                        /*
                        if (!MosaicWindow.MosaicInfo.HasConstantOverlap)
                        {
                            MessageBox.Show("The currently loaded mosaic contains no information about a " +
                                "regular overlap and so cannot at this time be used to perform correlation.");

                            return;
                        }
                        */

                        break;
                    }
            }

            correlator.MosaicInfo = MosaicWindow.MosaicInfo;
            correlator.ThreadController = new ThreadController(this);
            correlator.TextFeedbackDelegate = new CorrelatorTextFeedbackDelegate(this.TextSent);
            correlator.TileCorrelatedHandler = new CorrelatorTileCorrelatedDelegate(this.OnTileCorrelated);
            correlator.TileCorrelationBeginHandler = new CorrelatorTileCorrelationBeginDelegate(OnTileCorrelationBegin);
            correlator.TileCorrelatedCompletedHandler = new CorrelatorTileCorrelatedCompletedDelegate(TileCorrelationCompleted);
            correlator.TileCorrelatedProgressHandler = new CorrelatorTileCorrelationProgressDelegate(TileCorrelationProgressUpdate);
            correlator.CorrelationPausedOrResumedHandler = new CorrelatorTileCorrelationPausedOrResumedDelegate(CorrlationPausedOrResumed);

            if (this.correlator.OptionPanel != null)
            {
                this.correlatorOptionPanel.Controls.Add(this.correlator.OptionPanel);

                if (this.correlatorOptionPanel.Controls.Count > 1)
                    this.correlatorOptionPanel.Controls.RemoveAt(0);

                this.correlator.OptionPanel.Show();
            }

            this.correlateButton.Enabled = true;
        }

        private void OnCorrelationTypeChanged(object sender, EventArgs e)
        {
            SetCorrelationForCombobox();
        }

        void OnMosaicLoaded(MosaicWindow sender, MosaicWindowEventArgs args)
        {
            this.currentTile = null;
            this.Initialise();
            SetCorrelationForCombobox();
            this.tiledImageViewer.Reset();

            if (this.correlator != null && this.correlator.OptionPanel != null)
                this.correlator.OptionPanel.Reset(this.correlator.MosaicInfo);
        }
    }
}
