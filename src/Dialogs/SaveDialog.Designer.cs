namespace ImageStitching
{
    partial class SaveDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveDialog));
            this.widthNumeric = new System.Windows.Forms.NumericUpDown();
            this.heightNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveInfoCheckbox = new System.Windows.Forms.CheckBox();
            this.filePathCombo = new System.Windows.Forms.ComboBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.helpIcon = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.PercentNumeric = new System.Windows.Forms.NumericUpDown();
            this.MicronsPerPixelNumeric = new System.Windows.Forms.NumericUpDown();
            this.helpLabel = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.widthNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PercentNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MicronsPerPixelNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // widthNumeric
            // 
            this.widthNumeric.Location = new System.Drawing.Point(68, 49);
            this.widthNumeric.Name = "widthNumeric";
            this.widthNumeric.Size = new System.Drawing.Size(68, 20);
            this.widthNumeric.TabIndex = 1;
            this.toolTip.SetToolTip(this.widthNumeric, "Width of saved image in pixels");
            // 
            // heightNumeric
            // 
            this.heightNumeric.Location = new System.Drawing.Point(68, 75);
            this.heightNumeric.Name = "heightNumeric";
            this.heightNumeric.Size = new System.Drawing.Size(68, 20);
            this.heightNumeric.TabIndex = 3;
            this.toolTip.SetToolTip(this.heightNumeric, "Height of saved image in pixels");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Height";
            // 
            // saveButton
            // 
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(397, 109);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.OnSaveButtonClicked);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(307, 109);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.OnCancelButtonClicked);
            // 
            // saveInfoCheckbox
            // 
            this.saveInfoCheckbox.AutoSize = true;
            this.saveInfoCheckbox.Location = new System.Drawing.Point(68, 123);
            this.saveInfoCheckbox.Name = "saveInfoCheckbox";
            this.saveInfoCheckbox.Size = new System.Drawing.Size(117, 17);
            this.saveInfoCheckbox.TabIndex = 10;
            this.saveInfoCheckbox.Text = "Save info on image";
            this.saveInfoCheckbox.UseVisualStyleBackColor = true;
            this.saveInfoCheckbox.Visible = false;
            // 
            // filePathCombo
            // 
            this.filePathCombo.FormattingEnabled = true;
            this.filePathCombo.Location = new System.Drawing.Point(15, 12);
            this.filePathCombo.Name = "filePathCombo";
            this.filePathCombo.Size = new System.Drawing.Size(427, 21);
            this.filePathCombo.TabIndex = 11;
            this.toolTip.SetToolTip(this.filePathCombo, "File name to save to");
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(448, 12);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(24, 23);
            this.buttonBrowse.TabIndex = 12;
            this.buttonBrowse.Text = "...";
            this.toolTip.SetToolTip(this.buttonBrowse, "Browse to select the file to save to");
            this.buttonBrowse.Click += new System.EventHandler(this.OnFileSelectionBrowse);
            // 
            // helpIcon
            // 
            this.helpIcon.Enabled = false;
            this.helpIcon.Image = ((System.Drawing.Image)(resources.GetObject("helpIcon.Image")));
            this.helpIcon.InitialImage = ((System.Drawing.Image)(resources.GetObject("helpIcon.InitialImage")));
            this.helpIcon.Location = new System.Drawing.Point(12, 113);
            this.helpIcon.Name = "helpIcon";
            this.helpIcon.Size = new System.Drawing.Size(31, 27);
            this.helpIcon.TabIndex = 13;
            this.helpIcon.TabStop = false;
            this.helpIcon.Visible = false;
            this.helpIcon.MouseLeave += new System.EventHandler(this.OnHelpImageLeave);
            this.helpIcon.MouseHover += new System.EventHandler(this.OnHelpImageHover);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // PercentNumeric
            // 
            this.PercentNumeric.DecimalPlaces = 2;
            this.PercentNumeric.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.PercentNumeric.Location = new System.Drawing.Point(165, 49);
            this.PercentNumeric.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.PercentNumeric.Name = "PercentNumeric";
            this.PercentNumeric.ReadOnly = true;
            this.PercentNumeric.Size = new System.Drawing.Size(68, 20);
            this.PercentNumeric.TabIndex = 16;
            this.toolTip.SetToolTip(this.PercentNumeric, "Scale compared to full resolution");
            // 
            // MicronsPerPixelNumeric
            // 
            this.MicronsPerPixelNumeric.DecimalPlaces = 2;
            this.MicronsPerPixelNumeric.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MicronsPerPixelNumeric.Location = new System.Drawing.Point(292, 51);
            this.MicronsPerPixelNumeric.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.MicronsPerPixelNumeric.Name = "MicronsPerPixelNumeric";
            this.MicronsPerPixelNumeric.ReadOnly = true;
            this.MicronsPerPixelNumeric.Size = new System.Drawing.Size(68, 20);
            this.MicronsPerPixelNumeric.TabIndex = 18;
            this.toolTip.SetToolTip(this.MicronsPerPixelNumeric, "Scale of saved image");
            // 
            // helpLabel
            // 
            this.helpLabel.AutoSize = true;
            this.helpLabel.Location = new System.Drawing.Point(378, 49);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(0, 13);
            this.helpLabel.TabIndex = 14;
            this.helpLabel.Visible = false;
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Location = new System.Drawing.Point(151, 49);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(321, 46);
            this.textBox.TabIndex = 15;
            this.textBox.Text = "The maximum image size is restricted to 20000 pixels in either direction. The asp" +
                "ect ratio will be maintained for any selected region or the entire stitched imag" +
                "e if no particular region is selected.";
            this.textBox.Visible = false;
            // 
            // timer
            // 
            this.timer.Interval = 5000;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Width";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(239, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(366, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "microns per pixel";
            // 
            // SaveDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(494, 138);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MicronsPerPixelNumeric);
            this.Controls.Add(this.PercentNumeric);
            this.Controls.Add(this.helpLabel);
            this.Controls.Add(this.helpIcon);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.filePathCombo);
            this.Controls.Add(this.saveInfoCheckbox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.heightNumeric);
            this.Controls.Add(this.widthNumeric);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 170);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 170);
            this.Name = "SaveDialog";
            this.Text = "Save";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.widthNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PercentNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MicronsPerPixelNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown widthNumeric;
        private System.Windows.Forms.NumericUpDown heightNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox saveInfoCheckbox;
        private System.Windows.Forms.ComboBox filePathCombo;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.PictureBox helpIcon;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label helpLabel;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.NumericUpDown PercentNumeric;
        private System.Windows.Forms.NumericUpDown MicronsPerPixelNumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
    }
}