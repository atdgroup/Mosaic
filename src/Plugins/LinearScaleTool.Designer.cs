namespace ImageStitching
{
    partial class LinearScaleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearScaleForm));
            this.normaliseButton = new System.Windows.Forms.Button();
            this.minNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.maxNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.zedGraphControl = new ImageStitching.ZedGraphWithCursorsControl();
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // normaliseButton
            // 
            resources.ApplyResources(this.normaliseButton, "normaliseButton");
            this.normaliseButton.Name = "normaliseButton";
            this.normaliseButton.UseVisualStyleBackColor = true;
            this.normaliseButton.Click += new System.EventHandler(this.OnNormaliseClicked);
            // 
            // minNumericUpDown
            // 
            resources.ApplyResources(this.minNumericUpDown, "minNumericUpDown");
            this.minNumericUpDown.Name = "minNumericUpDown";
            this.minNumericUpDown.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
            // 
            // maxNumericUpDown
            // 
            resources.ApplyResources(this.maxNumericUpDown, "maxNumericUpDown");
            this.maxNumericUpDown.Name = "maxNumericUpDown";
            this.maxNumericUpDown.ValueChanged += new System.EventHandler(this.OnMinMaxNumericValueChanged);
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
            // zedGraphControl
            // 
            resources.ApplyResources(this.zedGraphControl, "zedGraphControl");
            this.zedGraphControl.IsAntiAlias = true;
            this.zedGraphControl.IsEnableHEdit = true;
            this.zedGraphControl.IsEnableHPan = false;
            this.zedGraphControl.IsEnableHZoom = false;
            this.zedGraphControl.IsEnableSelection = true;
            this.zedGraphControl.IsEnableVPan = false;
            this.zedGraphControl.IsEnableVZoom = false;
            this.zedGraphControl.IsEnableWheelZoom = false;
            this.zedGraphControl.Name = "zedGraphControl";
            this.zedGraphControl.ScrollGrace = 0D;
            this.zedGraphControl.ScrollMaxX = 0D;
            this.zedGraphControl.ScrollMaxY = 0D;
            this.zedGraphControl.ScrollMaxY2 = 0D;
            this.zedGraphControl.ScrollMinX = 0D;
            this.zedGraphControl.ScrollMinY = 0D;
            this.zedGraphControl.ScrollMinY2 = 0D;
            // 
            // LinearScaleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maxNumericUpDown);
            this.Controls.Add(this.minNumericUpDown);
            this.Controls.Add(this.normaliseButton);
            this.Controls.Add(this.zedGraphControl);
            this.Name = "LinearScaleForm";
            this.Load += new System.EventHandler(this.LinearScale_Load);
            ((System.ComponentModel.ISupportInitialize)(this.minNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraphWithCursorsControl zedGraphControl;
        private System.Windows.Forms.Button normaliseButton;
        private System.Windows.Forms.NumericUpDown minNumericUpDown;
        private System.Windows.Forms.NumericUpDown maxNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}