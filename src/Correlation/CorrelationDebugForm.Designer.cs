namespace ImageStitching
{
    partial class CorrelationDebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorrelationDebugForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.imageView = new ImageStitching.ImageView();
            this.imageView2 = new ImageStitching.ImageView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.imageView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.imageView2);
            this.splitContainer1.Size = new System.Drawing.Size(889, 569);
            this.splitContainer1.SplitterDistance = 296;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.splitContainer1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // imageView
            // 
            this.imageView.AllowMouseWheelZoom = true;
            this.imageView.AutoScroll = true;
            this.imageView.AutoScrollMinSize = new System.Drawing.Size(296, 569);
            this.imageView.BackColor = System.Drawing.SystemColors.ControlText;
            this.imageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageView.Image = null;
            this.imageView.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.imageView.Location = new System.Drawing.Point(0, 0);
            this.imageView.Name = "imageView";
            this.imageView.Size = new System.Drawing.Size(296, 569);
            this.imageView.TabIndex = 30;
            this.imageView.Text = "imageView1";
            this.imageView.Zoom = 1F;
            this.imageView.ZoomToFit = false;
            // 
            // imageView2
            // 
            this.imageView2.AllowMouseWheelZoom = true;
            this.imageView2.AutoScroll = true;
            this.imageView2.AutoScrollMinSize = new System.Drawing.Size(589, 569);
            this.imageView2.BackColor = System.Drawing.SystemColors.ControlText;
            this.imageView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageView2.Image = null;
            this.imageView2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.imageView2.Location = new System.Drawing.Point(0, 0);
            this.imageView2.Name = "imageView2";
            this.imageView2.Size = new System.Drawing.Size(589, 569);
            this.imageView2.TabIndex = 31;
            this.imageView2.Text = "imageView1";
            this.imageView2.Zoom = 1F;
            this.imageView2.ZoomToFit = false;
            // 
            // CorrelationDebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 569);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CorrelationDebugForm";
            this.Text = "Debug";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ImageView imageView;
        private ImageView imageView2;




    }
}