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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageStitching
{
    public partial class SaveDialog : Form
    {
        //static private SaveDialog instance = null;
        //static readonly object padlock = new object();

        private Rectangle imageRegion = Rectangle.Empty;
        private RoiTool tool;
        private decimal aspectRatio;
        private const int MaxPossibleWidth = 20000;
        private const int MaxPossibleHeight = 20000;
        private int MaxWidth;
        private int MaxHeight;
        
        public event EventHandler SaveButtonClicked;

        public SaveDialog(MosaicInfo mosaicInfo, RoiTool tool)
        {
            InitializeComponent();

            this.tool = tool;

            this.imageRegion = Rectangle.Empty;

            if (tool.Active == true)
                this.imageRegion = tool.TransformedRegionOfInterest;

            tool.RoiToolDrawnHandler += new RoiToolHandler<RoiTool, RoiToolEventArgs>(OnRoiToolDrawnHandler);
            tool.PluginActiveStatusChanged +=
                new MosaicPluginEventHandler<MosaicPlugin, EventArgs>(OnRoiPluginActiveStatusChanged);

            this.widthNumeric.Minimum = 100;
            this.widthNumeric.Maximum = 1000000;
            this.heightNumeric.Minimum = 100;
            this.heightNumeric.Maximum = 1000000;

            if (MosaicWindow.MosaicInfo.TotalWidth > SaveDialog.MaxPossibleWidth)
                this.MaxWidth = MaxPossibleWidth;
            else
                this.MaxWidth = MosaicWindow.MosaicInfo.TotalWidth;

            if (MosaicWindow.MosaicInfo.TotalHeight > SaveDialog.MaxPossibleWidth)
                this.MaxHeight = MaxPossibleHeight;
            else
                this.MaxHeight = MosaicWindow.MosaicInfo.TotalHeight;

            CalculateAspectRatio();

            this.filePathCombo.Items.Add(MosaicWindow.MosaicInfo.DirectoryPath);
            this.filePathCombo.Text = MosaicWindow.MosaicInfo.DirectoryPath;

            this.OnExportSizeNumericValidating(this.widthNumeric, new CancelEventArgs());

            this.widthNumeric.ValueChanged += new EventHandler(this.OnNumericValueChanged);
            this.heightNumeric.ValueChanged += new EventHandler(this.OnNumericValueChanged);
        }

        /*
        static internal SaveDialog GetSaveDialog(TileLoadInfo info, RoiTool tool)
        {
            lock (padlock)
            {
                if (SaveDialog.instance == null)
                {
                    SaveDialog.instance = new SaveDialog(info, tool);
                }

                return SaveDialog.instance;
            }
        }
        */

        private void Validate(NumericUpDown control)
        {
            // Pin controls to between their min and max values
            decimal width, height;

            width = this.widthNumeric.Value;

            if (width > this.MaxWidth)
            {
                this.textBox.Visible = true;
                // Reset the timer
                this.timer.Stop();
                this.timer.Start();
                width = MaxWidth;
            }
            
            width = Math.Max(width, this.widthNumeric.Minimum);

            height = this.heightNumeric.Value;

            if (height > this.MaxHeight)
            {
                this.textBox.Visible = true;
                this.timer.Stop();
                this.timer.Start();
                height = this.MaxHeight;
            }

            height = Math.Max(height, this.heightNumeric.Minimum);

            if (control == this.widthNumeric)
            {
                // Changed width numeric so I we must chabged the height
                height = Convert.ToInt32(width / this.aspectRatio);
                height = Math.Min(height, this.MaxHeight);
                height = Math.Max(height, this.heightNumeric.Minimum);
                this.widthNumeric.Value = width;
                this.heightNumeric.Value = height;
            }
            else
            {
                width = Convert.ToInt32(height * this.aspectRatio);
                width = Math.Min(width, this.MaxWidth);
                width = Math.Max(width, this.widthNumeric.Minimum);
                this.widthNumeric.Value = width;
                this.heightNumeric.Value = height;
            }

            if (this.imageRegion == Rectangle.Empty)
            {
                this.PercentNumeric.Value = this.widthNumeric.Value / Convert.ToDecimal(this.MaxWidth) * 100.0M;
                this.MicronsPerPixelNumeric.Value = Convert.ToDecimal(MosaicWindow.MosaicInfo.OriginalMicronsPerPixel * this.MaxWidth) / this.widthNumeric.Value;
            }
            else
            {
                this.PercentNumeric.Value = this.widthNumeric.Value / Convert.ToDecimal(imageRegion.Width) * 100.0M;
                this.MicronsPerPixelNumeric.Value = Convert.ToDecimal(MosaicWindow.MosaicInfo.OriginalMicronsPerPixel * imageRegion.Width) / this.widthNumeric.Value;
            }

            this.widthNumeric.Refresh();
            this.heightNumeric.Refresh();
            this.PercentNumeric.Refresh();
            this.MicronsPerPixelNumeric.Refresh();
        }

        private void OnExportSizeNumericValidating(object sender, CancelEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            Validate(control);
        }

        private void OnNumericValueChanged(object sender, EventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            Validate(control);
        }

        public int ExportWidth
        {
            get
            {
                return Convert.ToInt32(this.widthNumeric.Value);
            }
        }

        public int ExportHeight
        {
            get
            {
                return Convert.ToInt32(this.heightNumeric.Value);
            }
        }

        private bool IsFilePathValid()
        {
            string dir = null;

            if (this.filePathCombo.Text == "")
                return false;

            dir = System.IO.Path.GetDirectoryName(this.filePathCombo.Text);
          
            if (!System.IO.Directory.Exists(dir))
            {
                this.filePathCombo.Select();
                return false;
            }

            string fileName = System.IO.Path.GetFileName(this.filePathCombo.Text);

            if (fileName == "")            
                return false;

            return true;
        }

        private void CalculateAspectRatio()
        {
            this.imageRegion = this.tool.TransformedRegionOfInterest;

            if (this.imageRegion == Rectangle.Empty)
            {
                this.Text = "Exporting entire image";
                this.aspectRatio = (decimal)MosaicWindow.MosaicInfo.TotalWidth / MosaicWindow.MosaicInfo.TotalHeight;
                this.widthNumeric.Value = MosaicWindow.MosaicInfo.TotalWidth;
                this.heightNumeric.Value = MosaicWindow.MosaicInfo.TotalHeight;

                this.PercentNumeric.Value = 100.0M;
                this.MicronsPerPixelNumeric.Value = Convert.ToDecimal(MosaicWindow.MosaicInfo.OriginalMicronsPerPixel);
            }
            else
            {
                this.Text = "Exporting defined region";

                if (this.imageRegion.Width < this.widthNumeric.Minimum ||
                    this.imageRegion.Width > MosaicWindow.MosaicInfo.TotalWidth)
                {
                    return;
                }

                if (this.imageRegion.Height < this.heightNumeric.Minimum ||
                    this.imageRegion.Height > MosaicWindow.MosaicInfo.TotalHeight)
                {
                    return;
                }

                this.aspectRatio = (decimal)imageRegion.Width / imageRegion.Height;
                this.widthNumeric.Value = imageRegion.Width;
                this.heightNumeric.Value = imageRegion.Height;

                this.PercentNumeric.Value = 100.0M;
                this.MicronsPerPixelNumeric.Value = Convert.ToDecimal(MosaicWindow.MosaicInfo.OriginalMicronsPerPixel);
            }

            Validate(this.widthNumeric);
        }

        void OnRoiPluginActiveStatusChanged(MosaicPlugin sender, EventArgs args)
        {
            this.imageRegion = Rectangle.Empty;
            CalculateAspectRatio();
        }

        void OnRoiToolDrawnHandler(RoiTool sender, RoiToolEventArgs args)
        {
            
            CalculateAspectRatio();
        }

        public bool SaveInfo
        {
            get
            {
                return this.saveInfoCheckbox.Checked;
            }
        }

        public int NativeRegionWidth
        {
            get
            {
                if (this.imageRegion != Rectangle.Empty)
                    return this.imageRegion.Width;
                else
                    return MosaicWindow.MosaicInfo.TotalWidth;
            }
        }

        public int NativeRegionHeight
        {
            get
            {
                if (this.imageRegion != Rectangle.Empty)
                    return this.imageRegion.Height;
                else
                    return MosaicWindow.MosaicInfo.TotalHeight;
            }
        }

            

        /*
        public float Zoom
        {
            get
            {
                if (this.imageRegion != Rectangle.Empty)
                    return (float)(this.widthNumeric.Value / this.imageRegion.Width);
                else
                    return (float)(this.widthNumeric.Value / this.mosaicInfo.TotalWidth);
            }
        }
        */

        public string FilePath
        {
            get
            {
                return this.filePathCombo.Text;
            }
        }

        private void OnFileSelectionBrowse(object sender, EventArgs e)
        {
            string filePath = Utilities.SaveDialog(true);

            if (String.IsNullOrEmpty(filePath))
                return;

            this.filePathCombo.Items.Add(filePath);
            this.filePathCombo.Text = filePath;

            if (IsFilePathValid())
                this.saveButton.Enabled = true;
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (!IsFilePathValid())
                return;

            if (System.IO.Path.GetExtension(this.filePathCombo.Text) == "")
                this.filePathCombo.Text = this.filePathCombo.Text + ".ics";

            if (this.SaveButtonClicked != null)
                this.SaveButtonClicked(this, e);
        }

        private void OnHelpImageHover(object sender, EventArgs e)
        {
            string text = "The maximum image size is restricted to 20000 pixels in either direction.\n" + 
                          "The aspect ratio will be maintained for any selected region or the entire stitched\n" +
                          "image if no particular region is selected.";

            this.toolTip.Show(text, this.helpLabel, 50000);
        }

        private void OnHelpImageLeave(object sender, EventArgs e)
        {
            this.toolTip.Hide(this.helpLabel);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.textBox.Visible = false;
        }
    }
}