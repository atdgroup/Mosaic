namespace ImageStitching
{
    partial class TileViewEditToolBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.crossButton = new System.Windows.Forms.Button();
            this.tickButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // crossButton
            // 
            this.crossButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.crossButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.crossButton.Image = global::ImageStitching.Properties.Resources.cross;
            this.crossButton.Location = new System.Drawing.Point(8, 10);
            this.crossButton.Name = "crossButton";
            this.crossButton.Size = new System.Drawing.Size(39, 40);
            this.crossButton.TabIndex = 1;
            this.crossButton.UseVisualStyleBackColor = false;
            this.crossButton.Click += new System.EventHandler(this.OnCrossClicked);
            // 
            // tickButton
            // 
            this.tickButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tickButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tickButton.Image = global::ImageStitching.Properties.Resources.tick;
            this.tickButton.Location = new System.Drawing.Point(53, 10);
            this.tickButton.Name = "tickButton";
            this.tickButton.Size = new System.Drawing.Size(39, 40);
            this.tickButton.TabIndex = 0;
            this.tickButton.UseVisualStyleBackColor = false;
            this.tickButton.Click += new System.EventHandler(this.OnTickPressed);
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // TileViewEditToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.crossButton);
            this.Controls.Add(this.tickButton);
            this.Name = "TileViewEditToolBox";
            this.Size = new System.Drawing.Size(117, 155);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button tickButton;
        private System.Windows.Forms.Button crossButton;
        private System.Windows.Forms.Timer timer;

    }
}
