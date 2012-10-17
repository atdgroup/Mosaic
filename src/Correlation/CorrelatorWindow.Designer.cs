namespace ImageStitching
{
    partial class CorrelatorWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrelatorWindow));
            this.logSplitContainer = new System.Windows.Forms.SplitContainer();
            this.optionsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.correlationTypeComboBox = new System.Windows.Forms.ComboBox();
            this.correlatorPauseButton = new System.Windows.Forms.Button();
            this.correlateButton = new System.Windows.Forms.Button();
            this.correlatorOptionPanel = new System.Windows.Forms.Panel();
            this.useCorrelationCheckBox = new System.Windows.Forms.CheckBox();
            this.tiledImageViewer = new ImageStitching.TiledImageView();
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timeTakenStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.logSplitContainer)).BeginInit();
            this.logSplitContainer.Panel1.SuspendLayout();
            this.logSplitContainer.Panel2.SuspendLayout();
            this.logSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionsSplitContainer)).BeginInit();
            this.optionsSplitContainer.Panel1.SuspendLayout();
            this.optionsSplitContainer.Panel2.SuspendLayout();
            this.optionsSplitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // logSplitContainer
            // 
            this.logSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.logSplitContainer.Name = "logSplitContainer";
            this.logSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // logSplitContainer.Panel1
            // 
            this.logSplitContainer.Panel1.Controls.Add(this.optionsSplitContainer);
            this.logSplitContainer.Panel1MinSize = 400;
            // 
            // logSplitContainer.Panel2
            // 
            this.logSplitContainer.Panel2.Controls.Add(this.textBox);
            this.logSplitContainer.Panel2.Controls.Add(this.statusStrip);
            this.logSplitContainer.Size = new System.Drawing.Size(883, 774);
            this.logSplitContainer.SplitterDistance = 563;
            this.logSplitContainer.TabIndex = 1;
            // 
            // optionsSplitContainer
            // 
            this.optionsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.optionsSplitContainer.Name = "optionsSplitContainer";
            // 
            // optionsSplitContainer.Panel1
            // 
            this.optionsSplitContainer.Panel1.AutoScroll = true;
            this.optionsSplitContainer.Panel1.Controls.Add(this.label1);
            this.optionsSplitContainer.Panel1.Controls.Add(this.stopButton);
            this.optionsSplitContainer.Panel1.Controls.Add(this.label2);
            this.optionsSplitContainer.Panel1.Controls.Add(this.correlationTypeComboBox);
            this.optionsSplitContainer.Panel1.Controls.Add(this.correlatorPauseButton);
            this.optionsSplitContainer.Panel1.Controls.Add(this.correlateButton);
            this.optionsSplitContainer.Panel1.Controls.Add(this.correlatorOptionPanel);
            this.optionsSplitContainer.Panel1.Controls.Add(this.useCorrelationCheckBox);
            this.optionsSplitContainer.Panel1MinSize = 235;
            // 
            // optionsSplitContainer.Panel2
            // 
            this.optionsSplitContainer.Panel2.Controls.Add(this.tiledImageViewer);
            this.optionsSplitContainer.Size = new System.Drawing.Size(883, 563);
            this.optionsSplitContainer.SplitterDistance = 235;
            this.optionsSplitContainer.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Correlation Options";
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopButton.Location = new System.Drawing.Point(74, 475);
            this.stopButton.MinimumSize = new System.Drawing.Size(60, 0);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(60, 23);
            this.stopButton.TabIndex = 9;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.OnCorrelateStopButtonClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Correlation Types";
            // 
            // correlationTypeComboBox
            // 
            this.correlationTypeComboBox.FormattingEnabled = true;
            this.correlationTypeComboBox.Items.AddRange(new object[] {
            "None",
            "Kernel Correlation"});
            this.correlationTypeComboBox.Location = new System.Drawing.Point(12, 27);
            this.correlationTypeComboBox.Name = "correlationTypeComboBox";
            this.correlationTypeComboBox.Size = new System.Drawing.Size(145, 21);
            this.correlationTypeComboBox.TabIndex = 7;
            this.correlationTypeComboBox.Text = "Kernel Correlation";
            this.correlationTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.OnCorrelationTypeChanged);
            // 
            // correlatorPauseButton
            // 
            this.correlatorPauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.correlatorPauseButton.Enabled = false;
            this.correlatorPauseButton.Location = new System.Drawing.Point(140, 475);
            this.correlatorPauseButton.MinimumSize = new System.Drawing.Size(60, 0);
            this.correlatorPauseButton.Name = "correlatorPauseButton";
            this.correlatorPauseButton.Size = new System.Drawing.Size(60, 23);
            this.correlatorPauseButton.TabIndex = 6;
            this.correlatorPauseButton.Text = "Pause";
            this.correlatorPauseButton.UseVisualStyleBackColor = true;
            this.correlatorPauseButton.Click += new System.EventHandler(this.OnCorrelatePauseButtonToggled);
            // 
            // correlateButton
            // 
            this.correlateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.correlateButton.Location = new System.Drawing.Point(15, 475);
            this.correlateButton.Name = "correlateButton";
            this.correlateButton.Size = new System.Drawing.Size(54, 23);
            this.correlateButton.TabIndex = 2;
            this.correlateButton.Text = "Start";
            this.correlateButton.UseVisualStyleBackColor = true;
            this.correlateButton.Click += new System.EventHandler(this.OnCorrelateStartButtonClicked);
            // 
            // correlatorOptionPanel
            // 
            this.correlatorOptionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.correlatorOptionPanel.AutoScroll = true;
            this.correlatorOptionPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.correlatorOptionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.correlatorOptionPanel.Location = new System.Drawing.Point(12, 119);
            this.correlatorOptionPanel.MinimumSize = new System.Drawing.Size(100, 100);
            this.correlatorOptionPanel.Name = "correlatorOptionPanel";
            this.correlatorOptionPanel.Size = new System.Drawing.Size(211, 350);
            this.correlatorOptionPanel.TabIndex = 4;
            // 
            // useCorrelationCheckBox
            // 
            this.useCorrelationCheckBox.AutoSize = true;
            this.useCorrelationCheckBox.Location = new System.Drawing.Point(15, 61);
            this.useCorrelationCheckBox.Name = "useCorrelationCheckBox";
            this.useCorrelationCheckBox.Size = new System.Drawing.Size(103, 17);
            this.useCorrelationCheckBox.TabIndex = 2;
            this.useCorrelationCheckBox.Text = "Use Correlations";
            this.useCorrelationCheckBox.UseVisualStyleBackColor = true;
            this.useCorrelationCheckBox.CheckedChanged += new System.EventHandler(this.OnUseCorrelationsChanged);
            // 
            // tiledImageViewer
            // 
            this.tiledImageViewer.AllowMouseWheelZoom = true;
            this.tiledImageViewer.AutoScroll = true;
            this.tiledImageViewer.AutoScrollMinSize = new System.Drawing.Size(859, 694);
            this.tiledImageViewer.BackColor = System.Drawing.SystemColors.ControlText;
            this.tiledImageViewer.BlendingEnabled = false;
            this.tiledImageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tiledImageViewer.Image = null;
            this.tiledImageViewer.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            this.tiledImageViewer.Location = new System.Drawing.Point(0, 0);
            this.tiledImageViewer.Name = "tiledImageViewer";
            this.tiledImageViewer.Size = new System.Drawing.Size(644, 563);
            this.tiledImageViewer.TabIndex = 2;
            this.tiledImageViewer.Text = "imageView";
            this.tiledImageViewer.Zoom = 1F;
            this.tiledImageViewer.ZoomToFit = false;
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.SystemColors.MenuBar;
            this.textBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(3, 3);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(877, 263);
            this.textBox.TabIndex = 2;
            this.textBox.Text = "";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.toolStripStatusLabel,
            this.timeTakenStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 182);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(883, 25);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 19);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(183, 20);
            this.toolStripStatusLabel.Text = "Number of files correlated 0 / 200";
            // 
            // timeTakenStatusLabel
            // 
            this.timeTakenStatusLabel.Name = "timeTakenStatusLabel";
            this.timeTakenStatusLabel.Size = new System.Drawing.Size(0, 20);
            // 
            // CorrelatorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 774);
            this.Controls.Add(this.logSplitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CorrelatorWindow";
            this.Text = "Correlation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.logSplitContainer.Panel1.ResumeLayout(false);
            this.logSplitContainer.Panel2.ResumeLayout(false);
            this.logSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logSplitContainer)).EndInit();
            this.logSplitContainer.ResumeLayout(false);
            this.optionsSplitContainer.Panel1.ResumeLayout(false);
            this.optionsSplitContainer.Panel1.PerformLayout();
            this.optionsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.optionsSplitContainer)).EndInit();
            this.optionsSplitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer logSplitContainer;
        private System.Windows.Forms.SplitContainer optionsSplitContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private TiledImageView tiledImageViewer;
        private System.Windows.Forms.CheckBox useCorrelationCheckBox;
        private System.Windows.Forms.Panel correlatorOptionPanel;
        private System.Windows.Forms.RichTextBox textBox;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button correlateButton;
        private System.Windows.Forms.Button correlatorPauseButton;
        private System.Windows.Forms.ToolStripStatusLabel timeTakenStatusLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox correlationTypeComboBox;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label1;

    }
}