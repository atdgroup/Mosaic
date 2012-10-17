using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using FreeImageAPI;

namespace ImageStitching
{
    class KernelBasedOverlapCorrelatorOptionPanel : CorrelatorOptionPanel
    {
        private System.Windows.Forms.CheckBox searchAllEdgesCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox preFilter;
        private System.Windows.Forms.Label overLapLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown stripSizeNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown searchSizeNumeric;
        private System.Windows.Forms.NumericUpDown overlapNumeric;
        private Label label4;
        private Label label5;
        private Label label6;
        private NumericUpDown overlapNumericPixels2;
        private Label label7;
        private NumericUpDown stripSizeNumericPixels;
        private Label label8;
        private NumericUpDown searchSizeNumericPixels;
        private Label label9;
        private CheckBox adjustPosBeforeCorrelateCheckBox;
        private Button resetButton;
        private System.Windows.Forms.CheckBox randomSearchCheckBox;
        
        public CheckBox SelectStartTile;
        private ComboBox channel;
        private Label label10;
        KernalBasedOverlapCorrelator correlator;

        private void InitializeComponent()
        {
            this.searchAllEdgesCheckBox = new System.Windows.Forms.CheckBox();
            this.randomSearchCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.preFilter = new System.Windows.Forms.ComboBox();
            this.overLapLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.stripSizeNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.searchSizeNumeric = new System.Windows.Forms.NumericUpDown();
            this.overlapNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.overlapNumericPixels2 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.stripSizeNumericPixels = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.searchSizeNumericPixels = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.adjustPosBeforeCorrelateCheckBox = new System.Windows.Forms.CheckBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.SelectStartTile = new System.Windows.Forms.CheckBox();
            this.channel = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.stripSizeNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchSizeNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlapNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlapNumericPixels2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stripSizeNumericPixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchSizeNumericPixels)).BeginInit();
            this.SuspendLayout();
            // 
            // searchAllEdgesCheckBox
            // 
            this.searchAllEdgesCheckBox.AutoSize = true;
            this.searchAllEdgesCheckBox.Location = new System.Drawing.Point(13, 230);
            this.searchAllEdgesCheckBox.Name = "searchAllEdgesCheckBox";
            this.searchAllEdgesCheckBox.Size = new System.Drawing.Size(105, 17);
            this.searchAllEdgesCheckBox.TabIndex = 13;
            this.searchAllEdgesCheckBox.Text = "Search all edges";
            this.searchAllEdgesCheckBox.UseVisualStyleBackColor = true;
            // 
            // randomSearchCheckBox
            // 
            this.randomSearchCheckBox.AutoSize = true;
            this.randomSearchCheckBox.Location = new System.Drawing.Point(121, 230);
            this.randomSearchCheckBox.Name = "randomSearchCheckBox";
            this.randomSearchCheckBox.Size = new System.Drawing.Size(101, 17);
            this.randomSearchCheckBox.TabIndex = 12;
            this.randomSearchCheckBox.Text = "Random search";
            this.randomSearchCheckBox.UseVisualStyleBackColor = true;
            this.randomSearchCheckBox.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Pre filter";
            // 
            // preFilter
            // 
            this.preFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preFilter.FormattingEnabled = true;
            this.preFilter.Location = new System.Drawing.Point(10, 199);
            this.preFilter.Name = "preFilter";
            this.preFilter.Size = new System.Drawing.Size(105, 21);
            this.preFilter.TabIndex = 14;
            // 
            // overLapLabel
            // 
            this.overLapLabel.AutoSize = true;
            this.overLapLabel.Location = new System.Drawing.Point(7, 15);
            this.overLapLabel.Name = "overLapLabel";
            this.overLapLabel.Size = new System.Drawing.Size(92, 13);
            this.overLapLabel.TabIndex = 16;
            this.overLapLabel.Text = "Expected Overlap";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Strip size";
            // 
            // stripSizeNumeric
            // 
            this.stripSizeNumeric.Location = new System.Drawing.Point(102, 67);
            this.stripSizeNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.stripSizeNumeric.Name = "stripSizeNumeric";
            this.stripSizeNumeric.Size = new System.Drawing.Size(46, 20);
            this.stripSizeNumeric.TabIndex = 18;
            this.stripSizeNumeric.ValueChanged += new System.EventHandler(this.OnStripSizeValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Search size";
            // 
            // searchSizeNumeric
            // 
            this.searchSizeNumeric.Location = new System.Drawing.Point(102, 130);
            this.searchSizeNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.searchSizeNumeric.Name = "searchSizeNumeric";
            this.searchSizeNumeric.Size = new System.Drawing.Size(46, 20);
            this.searchSizeNumeric.TabIndex = 20;
            this.searchSizeNumeric.ValueChanged += new System.EventHandler(this.OnSearchSizeValueChanged);
            // 
            // overlapNumeric
            // 
            this.overlapNumeric.Enabled = false;
            this.overlapNumeric.Location = new System.Drawing.Point(102, 8);
            this.overlapNumeric.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.overlapNumeric.Name = "overlapNumeric";
            this.overlapNumeric.ReadOnly = true;
            this.overlapNumeric.Size = new System.Drawing.Size(47, 20);
            this.overlapNumeric.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(150, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "um";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(150, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "um";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "um";
            // 
            // overlapNumericPixels2
            // 
            this.overlapNumericPixels2.Enabled = false;
            this.overlapNumericPixels2.Location = new System.Drawing.Point(102, 33);
            this.overlapNumericPixels2.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.overlapNumericPixels2.Name = "overlapNumericPixels2";
            this.overlapNumericPixels2.ReadOnly = true;
            this.overlapNumericPixels2.Size = new System.Drawing.Size(47, 20);
            this.overlapNumericPixels2.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(150, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "pixels";
            // 
            // stripSizeNumericPixels
            // 
            this.stripSizeNumericPixels.Enabled = false;
            this.stripSizeNumericPixels.Location = new System.Drawing.Point(103, 93);
            this.stripSizeNumericPixels.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.stripSizeNumericPixels.Name = "stripSizeNumericPixels";
            this.stripSizeNumericPixels.ReadOnly = true;
            this.stripSizeNumericPixels.Size = new System.Drawing.Size(46, 20);
            this.stripSizeNumericPixels.TabIndex = 27;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(150, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "pixels";
            // 
            // searchSizeNumericPixels
            // 
            this.searchSizeNumericPixels.Enabled = false;
            this.searchSizeNumericPixels.Location = new System.Drawing.Point(103, 156);
            this.searchSizeNumericPixels.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.searchSizeNumericPixels.Name = "searchSizeNumericPixels";
            this.searchSizeNumericPixels.ReadOnly = true;
            this.searchSizeNumericPixels.Size = new System.Drawing.Size(46, 20);
            this.searchSizeNumericPixels.TabIndex = 29;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(150, 163);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "pixels";
            // 
            // adjustPosBeforeCorrelateCheckBox
            // 
            this.adjustPosBeforeCorrelateCheckBox.AutoSize = true;
            this.adjustPosBeforeCorrelateCheckBox.Checked = true;
            this.adjustPosBeforeCorrelateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.adjustPosBeforeCorrelateCheckBox.Location = new System.Drawing.Point(121, 201);
            this.adjustPosBeforeCorrelateCheckBox.Name = "adjustPosBeforeCorrelateCheckBox";
            this.adjustPosBeforeCorrelateCheckBox.Size = new System.Drawing.Size(128, 17);
            this.adjustPosBeforeCorrelateCheckBox.TabIndex = 31;
            this.adjustPosBeforeCorrelateCheckBox.Text = "Pre adjust tile position";
            this.adjustPosBeforeCorrelateCheckBox.UseVisualStyleBackColor = true;
            this.adjustPosBeforeCorrelateCheckBox.Visible = false;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(110, 313);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 32;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.OnResetSettingsClicked);
            // 
            // SelectStartTile
            // 
            this.SelectStartTile.Appearance = System.Windows.Forms.Appearance.Button;
            this.SelectStartTile.AutoSize = true;
            this.SelectStartTile.Enabled = false;
            this.SelectStartTile.Location = new System.Drawing.Point(10, 287);
            this.SelectStartTile.Name = "SelectStartTile";
            this.SelectStartTile.Size = new System.Drawing.Size(72, 23);
            this.SelectStartTile.TabIndex = 33;
            this.SelectStartTile.Text = "Set start tile";
            this.SelectStartTile.UseVisualStyleBackColor = true;
            // 
            // channel
            // 
            this.channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channel.FormattingEnabled = true;
            this.channel.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Blue",
            "Intensity"});
            this.channel.Location = new System.Drawing.Point(70, 254);
            this.channel.Name = "channel";
            this.channel.Size = new System.Drawing.Size(105, 21);
            this.channel.TabIndex = 34;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(10, 255);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Channel";
            // 
            // KernelBasedOverlapCorrelatorOptionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.channel);
            this.Controls.Add(this.SelectStartTile);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.adjustPosBeforeCorrelateCheckBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.searchSizeNumericPixels);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.stripSizeNumericPixels);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.overlapNumericPixels2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.overlapNumeric);
            this.Controls.Add(this.searchSizeNumeric);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.stripSizeNumeric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.overLapLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.preFilter);
            this.Controls.Add(this.searchAllEdgesCheckBox);
            this.Controls.Add(this.randomSearchCheckBox);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximumSize = new System.Drawing.Size(264, 465);
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "KernelBasedOverlapCorrelatorOptionPanel";
            this.Size = new System.Drawing.Size(252, 339);
            ((System.ComponentModel.ISupportInitialize)(this.stripSizeNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchSizeNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlapNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlapNumericPixels2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stripSizeNumericPixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchSizeNumericPixels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public KernelBasedOverlapCorrelatorOptionPanel(KernalBasedOverlapCorrelator correlator) : base()
        {
            this.InitializeComponent();

            this.preFilter.DisplayMember = "Value";
            this.preFilter.ValueMember = "Key";
            this.preFilter.DataSource = typeof(FiltersEnum).ToList();
            this.preFilter.SelectedIndex = 0;

            KernalBasedOverlapCorrelator.Prefilter = null;

            this.overLapLabel.Text = "Expected" + Environment.NewLine + "Overlap";

            this.channel.SelectedItem = "Intensity";  // default value

            this.correlator = correlator;
        }

        public override void Reset(MosaicInfo info)
        {
            this.MosaicInfo = info;

            double pixels = 1.0;

            if (this.MosaicInfo.HasConstantOverlap)
            {
                pixels = (Math.Min(this.MosaicInfo.OverLapPercentageX, this.MosaicInfo.OverLapPercentageY) / 100.0)
                     * this.MosaicInfo.Items[0].Width;
            }
            else
            {
                // There is no constant overlap
                // Lets default to 500 pixels or the image width which ever the smaller.
                pixels = (Math.Min(500, Tile.SmallestTileWidth(this.MosaicInfo.Items)));
                pixels = (Math.Min(pixels, Tile.SmallestTileHeight(this.MosaicInfo.Items)));
            }

            this.OverlapInMicrons = (int)(pixels * this.MosaicInfo.OriginalMicronsPerPixel);

            // By default set strip size to 20% of overlap
            this.StripSize = (int)(this.OverlapInMicrons * 0.2);

            // By default set the search size to 40 % of the overlap
            this.SearchSize = (int)(this.OverlapInMicrons * 0.4);

            this.searchAllEdgesCheckBox.Checked = true;

            this.preFilter.SelectedIndex = 0;
        }

        public FREE_IMAGE_COLOR_CHANNEL Channel
        {
            get
            {
                string selection = (string)this.channel.SelectedItem;

                if (selection.Equals("Red"))
                    return FREE_IMAGE_COLOR_CHANNEL.FICC_RED;
                else if (selection.Equals("Green"))
                    return FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN;
                else if (selection.Equals("Blue"))
                    return FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE;
                else
                    return FREE_IMAGE_COLOR_CHANNEL.FICC_RGB;
            }
        }

        public bool AdjustPosBeforeCorrelate
        {
            get
            {
                // Do we move the tile by a the same amount that the prvious tile
                // got moved by ?
                return this.adjustPosBeforeCorrelateCheckBox.Checked;
            }
        }

        public bool SearchAllEdges
        {   // Do we want to perforn 2 correlations per tile with surrounding tiles, and combime results in some way
            // Or just 1, selected by most area (probably)
            get
            {
                return this.searchAllEdgesCheckBox.Checked;
            }
        }

        public bool RandomKernelSearch
        {
            get
            {
                return this.randomSearchCheckBox.Checked;
            }
        }

        public FiltersEnum Filter
        {
            get
            {
                return (FiltersEnum) this.preFilter.SelectedValue;
            }
        }

        public int OverlapInMicrons
        {
            set
            {
                this.overlapNumeric.Value = Convert.ToInt32(value);
                this.overlapNumericPixels2.Value = Convert.ToInt32((value / this.MosaicInfo.MicronsPerPixel));
            }
            get
            {
                return Convert.ToInt32(this.overlapNumeric.Value);
            }
        }

        private void SetControlValues(int stripSize, int searchSize)
        {
            this.stripSizeNumeric.Value = stripSize;
            this.stripSizeNumericPixels.Value = Convert.ToInt32((stripSize / this.MosaicInfo.MicronsPerPixel));

            this.searchSizeNumeric.Value = searchSize;
            this.searchSizeNumericPixels.Value = Convert.ToInt32((searchSize / this.MosaicInfo.MicronsPerPixel));
        }

        public int StripSize
        {
            set
            {
                int stripSize = Convert.ToInt32(value);
                int searchSize = this.SearchSize;

                if (stripSize > this.OverlapInMicrons - 1)
                {
                    stripSize = this.OverlapInMicrons - 1;
                }

                if((stripSize + SearchSize) > this.OverlapInMicrons) {
                    searchSize = this.OverlapInMicrons - stripSize;
                }

                SetControlValues(stripSize, searchSize);               
            }
            get
            {
                return Convert.ToInt32(this.stripSizeNumeric.Value);
            }
        }

        public int SearchSize
        {
            set
            {
                int searchSize = Convert.ToInt32(value);
                int stripSize = this.StripSize;

                if (searchSize > this.OverlapInMicrons - 1)
                {
                    searchSize = this.OverlapInMicrons - 1;
                }

                if ((searchSize + StripSize) > this.OverlapInMicrons)
                {
                    stripSize = this.OverlapInMicrons - searchSize;
                }

                SetControlValues(stripSize, searchSize);
            }
            get
            {
                return Convert.ToInt32(this.searchSizeNumeric.Value);
            }
        }

        private void OnStripSizeValueChanged(object sender, EventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            this.StripSize = (int)control.Value;
        }

        private void OnSearchSizeValueChanged(object sender, EventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            this.SearchSize = (int)control.Value;
        }

        private void OnResetSettingsClicked(object sender, EventArgs e)
        {
            this.Reset(this.MosaicInfo);
        }
    }
}
