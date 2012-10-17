namespace ImageStitching
{
    partial class RGBBalanceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RGBBalanceForm));
            this.minNumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.maxNumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.channelTitle1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.minNumericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.maxNumericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.minNumericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.maxNumericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.channelTitle3 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.channelTitle2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown1Y = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1X = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDown2Y = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2X = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.zedGraphControl1 = new ImageStitching.ZedGraphWithCursorsControl();
            this.zedGraphControl2 = new ImageStitching.ZedGraphWithCursorsControl();
            this.zedGraphControl3 = new ImageStitching.ZedGraphWithCursorsControl();
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2X)).BeginInit();
            this.SuspendLayout();
            // 
            // minNumericUpDown1
            // 
            resources.ApplyResources(this.minNumericUpDown1, "minNumericUpDown1");
            this.minNumericUpDown1.Name = "minNumericUpDown1";
            this.minNumericUpDown1.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // maxNumericUpDown1
            // 
            resources.ApplyResources(this.maxNumericUpDown1, "maxNumericUpDown1");
            this.maxNumericUpDown1.Name = "maxNumericUpDown1";
            this.maxNumericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxNumericUpDown1.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // channelTitle1
            // 
            resources.ApplyResources(this.channelTitle1, "channelTitle1");
            this.channelTitle1.Name = "channelTitle1";
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2")});
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.OnRGBComboIndexChanged);
            // 
            // minNumericUpDown2
            // 
            resources.ApplyResources(this.minNumericUpDown2, "minNumericUpDown2");
            this.minNumericUpDown2.Name = "minNumericUpDown2";
            this.minNumericUpDown2.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // maxNumericUpDown2
            // 
            resources.ApplyResources(this.maxNumericUpDown2, "maxNumericUpDown2");
            this.maxNumericUpDown2.Name = "maxNumericUpDown2";
            this.maxNumericUpDown2.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxNumericUpDown2.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // comboBox2
            // 
            this.comboBox2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            resources.GetString("comboBox2.Items"),
            resources.GetString("comboBox2.Items1"),
            resources.GetString("comboBox2.Items2")});
            resources.ApplyResources(this.comboBox2, "comboBox2");
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.OnRGBComboIndexChanged);
            // 
            // minNumericUpDown3
            // 
            resources.ApplyResources(this.minNumericUpDown3, "minNumericUpDown3");
            this.minNumericUpDown3.Name = "minNumericUpDown3";
            this.minNumericUpDown3.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // maxNumericUpDown3
            // 
            resources.ApplyResources(this.maxNumericUpDown3, "maxNumericUpDown3");
            this.maxNumericUpDown3.Name = "maxNumericUpDown3";
            this.maxNumericUpDown3.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxNumericUpDown3.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // channelTitle3
            // 
            resources.ApplyResources(this.channelTitle3, "channelTitle3");
            this.channelTitle3.Name = "channelTitle3";
            // 
            // comboBox3
            // 
            this.comboBox3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            resources.GetString("comboBox3.Items"),
            resources.GetString("comboBox3.Items1"),
            resources.GetString("comboBox3.Items2")});
            resources.ApplyResources(this.comboBox3, "comboBox3");
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.OnRGBComboIndexChanged);
            // 
            // channelTitle2
            // 
            resources.ApplyResources(this.channelTitle2, "channelTitle2");
            this.channelTitle2.Name = "channelTitle2";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // numericUpDown1Y
            // 
            resources.ApplyResources(this.numericUpDown1Y, "numericUpDown1Y");
            this.numericUpDown1Y.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown1Y.Name = "numericUpDown1Y";
            this.numericUpDown1Y.ValueChanged += new System.EventHandler(this.OnChannelShiftChanged);
            // 
            // numericUpDown1X
            // 
            resources.ApplyResources(this.numericUpDown1X, "numericUpDown1X");
            this.numericUpDown1X.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown1X.Name = "numericUpDown1X";
            this.numericUpDown1X.ValueChanged += new System.EventHandler(this.OnChannelShiftChanged);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // numericUpDown2Y
            // 
            resources.ApplyResources(this.numericUpDown2Y, "numericUpDown2Y");
            this.numericUpDown2Y.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown2Y.Name = "numericUpDown2Y";
            this.numericUpDown2Y.ValueChanged += new System.EventHandler(this.OnChannelShiftChanged);
            // 
            // numericUpDown2X
            // 
            resources.ApplyResources(this.numericUpDown2X, "numericUpDown2X");
            this.numericUpDown2X.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown2X.Name = "numericUpDown2X";
            this.numericUpDown2X.ValueChanged += new System.EventHandler(this.OnChannelShiftChanged);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // zedGraphControl1
            // 
            resources.ApplyResources(this.zedGraphControl1, "zedGraphControl1");
            this.zedGraphControl1.IsAntiAlias = true;
            this.zedGraphControl1.IsEnableHEdit = true;
            this.zedGraphControl1.IsEnableHPan = false;
            this.zedGraphControl1.IsEnableHZoom = false;
            this.zedGraphControl1.IsEnableSelection = true;
            this.zedGraphControl1.IsEnableVPan = false;
            this.zedGraphControl1.IsEnableVZoom = false;
            this.zedGraphControl1.IsEnableWheelZoom = false;
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            // 
            // zedGraphControl2
            // 
            resources.ApplyResources(this.zedGraphControl2, "zedGraphControl2");
            this.zedGraphControl2.IsAntiAlias = true;
            this.zedGraphControl2.IsEnableHEdit = true;
            this.zedGraphControl2.IsEnableHPan = false;
            this.zedGraphControl2.IsEnableHZoom = false;
            this.zedGraphControl2.IsEnableSelection = true;
            this.zedGraphControl2.IsEnableVPan = false;
            this.zedGraphControl2.IsEnableVZoom = false;
            this.zedGraphControl2.IsEnableWheelZoom = false;
            this.zedGraphControl2.Name = "zedGraphControl2";
            this.zedGraphControl2.ScrollGrace = 0D;
            this.zedGraphControl2.ScrollMaxX = 0D;
            this.zedGraphControl2.ScrollMaxY = 0D;
            this.zedGraphControl2.ScrollMaxY2 = 0D;
            this.zedGraphControl2.ScrollMinX = 0D;
            this.zedGraphControl2.ScrollMinY = 0D;
            this.zedGraphControl2.ScrollMinY2 = 0D;
            // 
            // zedGraphControl3
            // 
            resources.ApplyResources(this.zedGraphControl3, "zedGraphControl3");
            this.zedGraphControl3.IsAntiAlias = true;
            this.zedGraphControl3.IsEnableHEdit = true;
            this.zedGraphControl3.IsEnableHPan = false;
            this.zedGraphControl3.IsEnableHZoom = false;
            this.zedGraphControl3.IsEnableSelection = true;
            this.zedGraphControl3.IsEnableVPan = false;
            this.zedGraphControl3.IsEnableVZoom = false;
            this.zedGraphControl3.IsEnableWheelZoom = false;
            this.zedGraphControl3.Name = "zedGraphControl3";
            this.zedGraphControl3.ScrollGrace = 0D;
            this.zedGraphControl3.ScrollMaxX = 0D;
            this.zedGraphControl3.ScrollMaxY = 0D;
            this.zedGraphControl3.ScrollMaxY2 = 0D;
            this.zedGraphControl3.ScrollMinX = 0D;
            this.zedGraphControl3.ScrollMinY = 0D;
            this.zedGraphControl3.ScrollMinY2 = 0D;
            // 
            // RGBBalanceForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.numericUpDown2Y);
            this.Controls.Add(this.numericUpDown2X);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.numericUpDown1Y);
            this.Controls.Add(this.numericUpDown1X);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.channelTitle3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.channelTitle2);
            this.Controls.Add(this.channelTitle1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maxNumericUpDown3);
            this.Controls.Add(this.maxNumericUpDown2);
            this.Controls.Add(this.maxNumericUpDown1);
            this.Controls.Add(this.minNumericUpDown3);
            this.Controls.Add(this.minNumericUpDown2);
            this.Controls.Add(this.minNumericUpDown1);
            this.Controls.Add(this.zedGraphControl3);
            this.Controls.Add(this.zedGraphControl2);
            this.Controls.Add(this.zedGraphControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "RGBBalanceForm";
            this.Load += new System.EventHandler(this.RGBBalance_Load);
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2X)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraphWithCursorsControl zedGraphControl1;
        private System.Windows.Forms.NumericUpDown minNumericUpDown1;
        private System.Windows.Forms.NumericUpDown maxNumericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label channelTitle1;
        private System.Windows.Forms.ComboBox comboBox1;
        private ZedGraphWithCursorsControl zedGraphControl2;
        private System.Windows.Forms.NumericUpDown minNumericUpDown2;
        private System.Windows.Forms.NumericUpDown maxNumericUpDown2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox2;
        private ZedGraphWithCursorsControl zedGraphControl3;
        private System.Windows.Forms.NumericUpDown minNumericUpDown3;
        private System.Windows.Forms.NumericUpDown maxNumericUpDown3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label channelTitle3;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label channelTitle2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDown1Y;
        private System.Windows.Forms.NumericUpDown numericUpDown1X;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDown2Y;
        private System.Windows.Forms.NumericUpDown numericUpDown2X;
        private System.Windows.Forms.Label label13;
    }
}