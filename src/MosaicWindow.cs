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
using System.Text;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using GrayLabs.Windows.Forms;
using ThreadingSystem;
using FreeImageAPI;
using FreeImageIcs;

namespace ImageStitching
{
    public delegate void MosaicWindowHandler<S, A>(S sender, A args);
    public delegate void MosaicWindowGraphicsHandler<S, G, A>(S sender, G graphics, A args);

    public partial class MosaicWindow : Form
    {
        #region Private Members

        public static MosaicInfo MosaicInfo;

        public event MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs> MosaicLoaded;
        public event MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs> ZoomChanged;
        public event MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs> BlendingChanged;
        public event MosaicWindowHandler<MosaicWindow, MosaicWindowEventArgs> UsingCorrelationsChanged;
        public event MosaicWindowGraphicsHandler<MosaicWindow, Graphics, MosaicWindowEventArgs> SavingScreenShot;

        private List<MosaicPlugin> plugins;
        private TileView imageView;
        private ThreadController saveCacheThreadController;
        private ThreadController openThreadController;
        private ThreadController tileViewThreadController;
        private CorrelatorWindow correlateWindow;
        private TileReader tileReader = null;
        private TileReader originalTileReader;
        private string[] originalFilePaths;

        // this is the file to open first
        // This is passed in from args. We can't open the file before application.Run
        // so we saved the path here
        private bool preventTextChangedHandler;

        #endregion

        #region Constructor
        public MosaicWindow()
        {
            this.imageView = new TileView();

            tileViewThreadController = new ThreadController(this);
            tileViewThreadController.SetThreadCompletedCallback(TileViewThreadCompleted);
            tileViewThreadController.SetThreadProgressCallback(TileViewProgressIndicator);
            this.TileView.SetThreadController(tileViewThreadController);

            InitializeComponent();

            this.imageView.BackColor = Color.Black;
            this.imageView.HandleCreated += new EventHandler(OnTileViewerHandleCreated);
            this.imageView.MouseMove += new MouseEventHandler(OnTileViewerMouseMove);
            this.imageView.ZoomChanged += new EventHandler(OnTileViewerZoomChanged);
        
            this.imageView.Dock = DockStyle.Fill;
            
            this.viewPanel.Controls.Add(this.imageView);

            this.imageView.Toolbox = this.tileViewEditToolBox;

            this.tileOverView.SetMosaicWindow(this);

            this.tileOverView.TileOverViewCompletedLoadHandler +=
                new TileOverViewHandler<TileOverView,
                                        TileOverViewEventArgs>(OnTileOverViewCompletedLoadHandler);

            this.toolStripZoomComboBox.SelectedIndex = 3;

            plugins = new List<MosaicPlugin>();
            //plugins.Add(new ProfileTool("Profile", this));
            plugins.Add(new RoiTool("Region", this));
            plugins.Add(new LinearScaleTool("Linear Scale", this));
            plugins.Add(new RGBBalanceTool("RGB Balance", this));
            plugins.Add(new ScalebarTool("Scalebar", this));

            MosaicPlugin.AllPlugins["Linear Scale"].Enabled = false;
            MosaicPlugin.AllPlugins["RGB Balance"].Enabled = false;
            MosaicPlugin.AllPlugins["Region"].Enabled = false;

            //MosaicPlugin.AllPlugins["Profile"].Enabled = false;
        }

        #endregion

        #region AccessProperties

        public MosaicPlugin GetTool(string name)
        {
            return MosaicPlugin.AllPlugins[name];
        }

        public TileView TileView
        {
            get
            {
                return this.imageView;
            }
        }

        public ToolStrip ToolStrip
        {
            get
            {
                return this.mosaicToolStrip;
            }
        }

        public bool EnableNonPluginToolStrip
        {
            set
            {
                this.toolStrip.Enabled = value;
            }
        }

        public MenuStrip MenuStrip
        {
            get
            {
                return this.menuStrip;
            }
        }

        public TileReader TileReader
        {
            get
            {
                return this.tileReader;
            }
        }

        internal TileOverView TileOverView
        {
            get
            {
                return this.tileOverView;
            }
        }

        #endregion

        #region DisableEnableUI

        private void DisableUI()
        {
          
        }

        #endregion

        #region Opening Files

        public void SetStartupFiles(string[] filepaths)
        {
            this.originalFilePaths = filepaths;
        }

        public void Open(string[] originalFilePaths)
        {
            this.originalFilePaths = originalFilePaths;
            string[] cachefilePaths = new string[1];

            MosaicPlugin.AllPlugins["Linear Scale"].Enabled = false;
            MosaicPlugin.AllPlugins["RGB Balance"].Enabled = false; 
            
            TileReader[] tileReaders = new TileReader[6];

            tileReaders[0] = new MosaicFileReader(originalFilePaths, this);
            tileReaders[1] = new Version2SequenceFileReader(originalFilePaths, this);
            tileReaders[2] = new SequenceFileReader(originalFilePaths, this);
            tileReaders[3] = new RosMosaicSequenceFileReader(originalFilePaths, this);
            tileReaders[4] = new ImageCollectionFileReader(originalFilePaths, this);
            tileReaders[5] = new MultiSequenceFileReader(originalFilePaths, this);

            foreach (TileReader t in tileReaders)
            {
                if (t.CanRead())
                {
                    originalTileReader = tileReader = t;

                    break;
                }
            }

            if(tileReader == null) {
                MessageBox.Show("Unknown file type");
                return;
            }

            // Check if there is cache availiable for this file
            if (tileReader.HasCache())
            {
                cachefilePaths[0] = tileReader.GetCacheFilePath();

                if (tileReader.GetType() != typeof(MosaicFileReader))   // create a new reader to read this cache, if we opened a mos file, we already have the reader
                    tileReader = new MosaicFileReader(cachefilePaths, this);
            }

            this.menuStrip.Enabled = false;
            this.toolStrip.Enabled = false;

            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            openThreadController = new ThreadController(this);
            openThreadController.SetThreadCompletedCallback(OpenFileThreadCompleted);
            openThreadController.SetThreadProgressCallback(OpenFileProgressIndicator);
            tileReader.SetThreadController(openThreadController);

            progressbar.Value = 0;
            this.statusStrip.Refresh();

            try
            {
                tileReader.ReadHeader();
            }
            catch (MosaicException e)
            {
                // The cache file failed to be read.
                // Probably as the format has changed. Here we delete the cache file and recall the open function

                if (tileReader.GetType() == typeof(MosaicFileReader))
                {
                    if (originalTileReader.GetType() != typeof(MosaicFileReader))
                    {
                        File.Delete(cachefilePaths[0]);
                        this.Open(originalFilePaths);
                    }
                    else
                    {
                        // The user has tried to open the mos (cache) file directly 
                        // We don't know what other files to fall back too so we just error and return.
                        // File.Delete(cachefilePaths[0]);   // DO NOT DELETE THE FILE THEY JUST TRIED TO OPEN!
                        MessageBox.Show("Unable to load file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to read header information correctly. " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                this.menuStrip.Enabled = true;
                this.toolStrip.Enabled = true;
            }

            MosaicWindow.MosaicInfo = new MosaicInfo(tileReader);
            
            this.tileOverView.CreateOverview();

            this.tileOverView.Location = new Point(25, this.TileView.Bottom - this.tileOverView.Height - 25);

            this.tileOverView.Show();

            // TileLoadInfo should now be ready as we have read the header.
            this.BlendingEnabled = true;
            this.CorrelationEnabled = true;

            // Threaded Operation
            tileReader.CreateThumbnails();   
        }

        void OnTileOverViewCompletedLoadHandler(TileOverView sender, TileOverViewEventArgs args)
        {
            // Save cache for furture Loads
            if (tileReader.GetType() != typeof(MosaicFileReader))
                this.SaveCache();
        }

        void OnTileViewerZoomChanged(object sender, EventArgs e)
        {
            if (MosaicWindow.MosaicInfo == null)
                return;

            MosaicWindow.MosaicInfo.Zoom = this.imageView.Zoom;
            double value = 100.0 * this.imageView.Zoom;

            preventTextChangedHandler = true;
            this.toolStripZoomComboBox.Text = String.Format("{0:0.0}%", value);
            preventTextChangedHandler = false;
        }

        void OnTileViewerMouseMove(object sender, MouseEventArgs e)
        {
            Tile tile = this.TileView.FindTileForPoint(e.Location);

            if (tile == null)
                return;
     
            if (MosaicWindow.MosaicInfo.IsCompositeRGB)
                this.feedbackStatusStrip.Items[0].Text =
                     String.Format(CultureInfo.CurrentCulture, "{0} {3} {4} Pos {1} Adj Pos {2}",
                     tile.FileName, tile.OriginalPosition, tile.AdjustedPosition, tile.getFileName(1), tile.getFileName(2));
            else
               this.feedbackStatusStrip.Items[0].Text =
                    String.Format(CultureInfo.CurrentCulture, "Filename {0} Pos {1} Adj Pos {2}",
                    tile.FileName, tile.OriginalPosition, tile.AdjustedPosition);
        }

        private void OnOpenMenuClicked(object sender, EventArgs e)
        {
            string[] fileNames = Utilities.OpenDialog();

            if (fileNames == null)
                return;

            this.Open(fileNames);
        }

        private void OnOpenMultiSeqClicked(object sender, EventArgs e)
        {
            string[] fileNames = Utilities.OpenMultiSeqDialog();

            if (fileNames == null)
                return;

            this.Open(fileNames);

        }

        private void OnOpenImagesClicked(object sender, EventArgs e)
        {
            string[] fileNames = Utilities.OpenImagesDialog();

            if (fileNames == null)
                return;

            this.Open(fileNames);

        }

        private void OnToolStripOpenButtonClicked(object sender, EventArgs e)
        {
            string[] fileNames = Utilities.OpenDialog();

            if (fileNames == null)
                return;

            this.Open(fileNames);
        }

        public void OpenFileProgressIndicator(object sender, string updateText, int percentage)
        {
            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            this.feedbackStatusStrip.Items[0].Text = String.Format(CultureInfo.CurrentCulture, updateText);

            progressbar.Value = percentage;

            this.imageView.Invalidate();
        }

        public void OpenFileThreadCompleted(object sender, string updateText, bool aborted)
        {
            TileReader reader = (TileReader) sender;
            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            // Try to read cache
            string cacheFilePath = tileReader.GetCacheFilePath();

            if (tileReader.FailedToReadFormat && reader.GetType() == typeof(MosaicFileReader))
            {
                // The cache file failed to be read.
                // Probably as the format is corrupt

                try
                {          
                    File.Delete(tileReader.GetCacheFilePath());
                    tileReader.Dispose();
                    this.Open(this.originalFilePaths);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            this.menuStrip.Enabled = true;
            this.toolStrip.Enabled = true;

            tileReader.ReadMetaData();

            tileReader.Dispose();

            this.statusStrip.Items[0].Text =
                String.Format(CultureInfo.CurrentCulture, "Total Size {0} x {1}", MosaicWindow.MosaicInfo.TotalWidth, MosaicWindow.MosaicInfo.TotalHeight);

            this.feedbackStatusStrip.Items[0].Text = "";

            this.imageView.ImageSize = new Size(MosaicInfo.TotalWidth, MosaicInfo.TotalHeight);
            this.imageView.Clear();
            this.imageView.LoadMosaicInfo(MosaicWindow.MosaicInfo);

            // set the window title
            this.Text = MosaicWindow.MosaicInfo.Prefix + " - Mosaic";

            // Send a scroll message to update the display.
            this.imageView.SetAutoScrollPosition(new Point(0, 0));   

            this.menuStrip.Enabled = true;
            this.toolStrip.Enabled = true;
            MosaicPlugin.AllPlugins["Linear Scale"].Enabled = true;
            MosaicPlugin.AllPlugins["RGB Balance"].Enabled = true; 

            progressbar.Value = 0;
            this.feedbackStatusStrip.Items[0].Text = "";

            OnMosaicLoaded(new MosaicWindowEventArgs (MosaicWindow.MosaicInfo));
        }

        public void TileViewProgressIndicator(object sender, string updateText, int percentage)
        {
            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            this.feedbackStatusStrip.Items[0].Text = String.Format(CultureInfo.CurrentCulture, updateText);

            progressbar.Value = percentage;

            this.imageView.Invalidate();
        }

        public void TileViewThreadCompleted(object sender, string updateText, bool aborted)
        {
            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            progressbar.Value = 0;
            this.feedbackStatusStrip.Items[0].Text = "";
        }
        #endregion

        #region Saving

        private void SaveScreenShot()
        {
            string filePath = Utilities.SaveDialog(true);

            if (filePath == null)
                return;

            Bitmap bitmap = this.imageView.ScreenBitmap;   
            Graphics g = Graphics.FromImage(bitmap);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            OnSavingScreenShot(g, new MosaicWindowEventArgs(MosaicWindow.MosaicInfo));

            FreeImageAlgorithmsBitmap fib = new FreeImageAlgorithmsBitmap(bitmap);

            fib.ConvertTo24Bits();

            g.Dispose();

            bitmap.Dispose();

            string extension = Path.GetExtension(filePath);

            if (extension != ".ics")
            {
                fib.SaveToFile(filePath);
            }
            else {
                IcsFile.SaveToFile(fib, filePath, true);
            }

            fib.Dispose();
        }

        private void OnScreenShotMenuClicked(object sender, EventArgs e)
        {
            this.SaveScreenShot();
        }

        private void SaveFileProgressIndicator(object sender, string updateText, int percentage)
        {
            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            this.feedbackStatusStrip.Items[0].Text = String.Format(CultureInfo.CurrentCulture, updateText);

            progressbar.Value = percentage;
        }

        private void SaveFileThreadCompleted(object sender, string updateText, bool aborted)
        {
            ToolStripProgressBar progressbar = (ToolStripProgressBar)this.feedbackStatusStrip.Items[1];

            this.feedbackStatusStrip.Items[0].Text = String.Format(CultureInfo.CurrentCulture, updateText);

            progressbar.Value = 0;
        }

        public void SaveCache()
        {
            saveCacheThreadController = new ThreadController(this);
            saveCacheThreadController.SetThreadCompletedCallback(SaveFileThreadCompleted);
            saveCacheThreadController.SetThreadProgressCallback(SaveFileProgressIndicator);

            TileSaver saver = new TileSaver(saveCacheThreadController, this);
            saver.SaveCacheFile();
        }

        private void Export()
        {
            ThreadController threadController = new ThreadController(this);
            threadController.SetThreadCompletedCallback(SaveFileThreadCompleted);
            threadController.SetThreadProgressCallback(SaveFileProgressIndicator);

            TileSaver saver = new TileSaver(threadController, this);
            saver.Export();
        }

        private void OnExportToolButton(object sender, EventArgs e)
        {
            this.Export();
        }

        private void OnExportClicked(object sender, EventArgs e)
        {
            this.Export();
        }

        #endregion

        #region Misc UI Events

        private void OnShowOverviewClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked == true)
            {
                this.tileOverView.Hide();
                item.Checked = false;
            }
            else
            {
                this.tileOverView.Show();
                item.Checked = true;
            }
        }

        private void showFilenamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked == true)
            {
                this.imageView.ShowFileNames = false;
                item.Checked = false;
            }
            else
            {
                this.imageView.ShowFileNames = true;
                item.Checked = true;
            }

            this.imageView.Redraw();
        }

        private void OnInfoPanelMenuItemClicked(object sender, EventArgs e)
        {
            
        }

        private void OnShowCorrelationWindow(object sender, EventArgs e)
        {
            if (MosaicWindow.MosaicInfo == null)
                return;

            correlateWindow = CorrelatorWindow.GetCorrelatorWindowInstance(this);
            correlateWindow.Show();          
        }

        private void OnAboutMenuItemClicked(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();

            aboutBox.ShowDialog();
        }

        private void Shutdown()
        {
            // Update the cache file with correlation positions
            this.SaveCache();

            if (this.openThreadController != null)
            {
                if (this.tileOverView != null)
                    this.tileOverView.AbortOverviewCreation();

                this.openThreadController.AbortThread();
                this.openThreadController.ThreadJoin();

                if(this.saveCacheThreadController != null)
                    this.saveCacheThreadController.ThreadJoin();
            }

            if(this.correlateWindow != null)
                this.correlateWindow.StopCorrelation();
        }

        private void OnWindowClosing(object sender, FormClosingEventArgs e)
        {
            this.Shutdown();

            e.Cancel = false;
        }

        private void OnTileViewerHandleCreated(object sender, EventArgs e)
        {
            if(this.originalFilePaths != null)
                this.Open(this.originalFilePaths);
        }

        private void OnExitMenuItemClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnHistogramShowClicked(object sender, EventArgs e)
        {
            Histogram histogram = new Histogram(this);

            histogram.Show();
        }

        private void OnImageInformationClicked(object sender, EventArgs e)
        {
            ImageInformation info = new ImageInformation();

            if (MosaicWindow.MosaicInfo == null)
                return;

            info.AddMetaData(MosaicWindow.MosaicInfo.MetaData);

            info.Show();
        }

        private void linearScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearScaleTool tool = (LinearScaleTool)MosaicToggleButtonTool.AllPlugins["Linear Scale"];
            tool.Display();
        }

        private void RGBBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RGBBalanceTool tool = (RGBBalanceTool)MosaicToggleButtonTool.AllPlugins["RGB Balance"];
            tool.Display();
        }

        private void OnShowJoinsMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked == true)
            {
                this.imageView.ShowJoins = false;
                item.Checked = false;
            }
            else
            {
                this.imageView.ShowJoins = true;
                item.Checked = true;
            }

            this.imageView.Redraw();
        }

        #endregion

        #region Signals

        protected void OnMosaicLoaded(MosaicWindowEventArgs args)
        {
            if (MosaicLoaded != null)
                MosaicLoaded(this, args);
        }

        protected void OnZoomChanged(MosaicWindowEventArgs args)
        {
            if (ZoomChanged != null)
                ZoomChanged(this, args);
        }

        protected void OnBlendingChanged(MosaicWindowEventArgs args)
        {
            if (BlendingChanged != null)
                BlendingChanged(this, args);
        }

        protected void OnUsingCorrelationsChanged(MosaicWindowEventArgs args)
        {
            if (UsingCorrelationsChanged != null)
                UsingCorrelationsChanged(this, args);
        }

        protected void OnSavingScreenShot(Graphics g, MosaicWindowEventArgs args)
        {
            if (SavingScreenShot != null)
                SavingScreenShot(this, g, args);
        }     

        #endregion

        private void UpdateZoom()
        {
            if (this.preventTextChangedHandler)
                return;

            if (MosaicWindow.MosaicInfo == null)
                return;

            float zoom = 1.0f;

            switch (this.toolStripZoomComboBox.Text)
            {
                case "500%":
                    {
                        zoom = 5.0f;
                        break;
                    }

                case "300%":
                    {
                        zoom = 3.0f;
                        break;
                    }

                case "250%":
                    {
                        zoom = 2.5f;
                        break;
                    }

                case "100%":
                    {
                        zoom = 1.0f;
                        break;
                    }

                case "75%":
                    {
                        zoom = 0.75f;
                        break;
                    }

                case "50%":
                    {
                        zoom = 0.5f;
                        break;
                    }

                case "25%":
                    {
                        zoom = 0.25f;
                        break;
                    }

                case "Fit":
                    {
                        float aspectRatio = (float)MosaicWindow.MosaicInfo.TotalHeight / MosaicWindow.MosaicInfo.TotalWidth;

                        int width = this.imageView.Width;
                        int height = (int)(aspectRatio * width);

                        if (height > this.imageView.Height)
                        {
                            height = this.imageView.Height;
                            width = (int)(height / aspectRatio);
                        }

                        float wfactor = (float)width / MosaicWindow.MosaicInfo.TotalWidth;
                        float hfactor = (float)height / MosaicWindow.MosaicInfo.TotalHeight;

                        float factor = Math.Min(wfactor, hfactor);

                        zoom = factor;


                        break;
                    }

                default:
                    {
                        // Maybe the user has entered a specific zoom ie 0.3 %
                        // Lets assume thet enter a float value, anything else is dis regarded     
                        try
                        {
                            string zoomStr;

                            if (this.toolStripZoomComboBox.Text.Contains("%"))
                                zoomStr = this.toolStripZoomComboBox.Text.Split('%')[0];
                            else
                                zoomStr = this.toolStripZoomComboBox.Text;

                            zoom = (float)Convert.ToDouble(zoomStr);
                            zoom /= 100.0F;

                            if (zoom <= 0.0) return;
                        }
                        catch (FormatException)
                        {
                            return;
                        }

                        break;
                    }
            }

            this.menuStrip.Enabled = false;
            this.toolStrip.Enabled = false;

            MosaicWindow.MosaicInfo.Zoom = zoom;
            this.imageView.Zoom = zoom;
            this.imageView.Refresh();

            OnZoomChanged(new MosaicWindowEventArgs(MosaicWindow.MosaicInfo));

            this.imageView.Invalidate();

            this.tileOverView.Refresh();

            this.menuStrip.Enabled = true;
            this.toolStrip.Enabled = true;

            this.toolStripZoomComboBox.Focus();
            this.toolStripZoomComboBox.Select(this.toolStripZoomComboBox.SelectionLength, 0);
        }

        private void OnZoomComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            this.UpdateZoom();
        }

        private void OnZoomComboBoxSelectedChanged(object sender, EventArgs e)
        {
            this.UpdateZoom();
        }

        public bool CorrelationEnabled
        {
            set
            {
                if (MosaicWindow.MosaicInfo.IsCorrelated && value)
                {
                    if (value)
                        this.correlationStatusLabel.Font = new Font(this.correlationStatusLabel.Font, FontStyle.Bold);
                    else
                        this.correlationStatusLabel.Font = new Font(this.correlationStatusLabel.Font, FontStyle.Regular);

                    Tile.SetUseAjustedPositionForAllTiles(MosaicWindow.MosaicInfo.Items, value);
                }
                else
                {
                    this.correlationStatusLabel.Font = new Font(this.correlationStatusLabel.Font, FontStyle.Regular);
                    Tile.SetUseAjustedPositionForAllTiles(MosaicWindow.MosaicInfo.Items, false);
                }

                this.TileView.Redraw();

                this.OnUsingCorrelationsChanged(new MosaicWindowEventArgs(MosaicWindow.MosaicInfo));
            }
            get
            {
                return Tile.UsingAdjustedPositionsForAllTiles;
            }
        }

        public bool BlendingEnabled
        {
            set
            {
                this.TileView.BlendingEnabled = value;

                if (value) {
                    this.blendedStatusLabel.Font = new Font(this.blendedStatusLabel.Font, FontStyle.Bold);
                    this.performBlendingToolStripMenuItem.Checked = true;
                }
                else {
                    this.blendedStatusLabel.Font = new Font(this.blendedStatusLabel.Font, FontStyle.Regular);
                    this.performBlendingToolStripMenuItem.Checked = false;
                }

                this.imageView.Redraw();

                this.OnBlendingChanged(new MosaicWindowEventArgs(MosaicWindow.MosaicInfo));     
            }
            get
            {
                return this.TileView.BlendingEnabled;
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            this.tileOverView.Location = new Point(25, this.TileView.Bottom - this.tileOverView.Height - 25);

            this.TileView.Refresh();
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            string executableName = Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            string file = null;

            if (Debugger.IsAttached == false)
            {
                file = Path.Combine(executableDirectoryName, "Mosaic_help.htm");
            }
            else
            {
                file = Path.Combine(executableDirectoryName, "..\\Help\\Mosaic_help.htm");
            }

            try
            {
                Process.Start(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnBlendingClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (item.Checked == true)
            {
                this.BlendingEnabled = false;
            }
            else
            {
                this.BlendingEnabled = true;     
            }
        }

        private void OnBlendedLabelClick(object sender, EventArgs e)
        {
            if (MosaicWindow.MosaicInfo == null)
                return;

            this.BlendingEnabled = !this.BlendingEnabled;
        }

        private void OnCorrelatedLabelClicked(object sender, EventArgs e)
        {
            if (MosaicWindow.MosaicInfo == null)
                return;

            this.CorrelationEnabled = !this.CorrelationEnabled;
        }

        private void OnReloadThumbnailsClicked(object sender, EventArgs e)
        {
            tileReader.ReCreateThumbnails();
        }
    }

    public class MosaicWindowEventArgs : EventArgs
    {
        private MosaicInfo info;

        public MosaicWindowEventArgs(MosaicInfo info)
        {
            this.info = info;
        }

        public MosaicInfo MosaicInfo
        {
            get
            {
                return this.info;
            }
        }
    }
}
