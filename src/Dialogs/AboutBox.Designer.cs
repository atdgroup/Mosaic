namespace GrayLabs
{
    namespace Windows
    {
        namespace Forms
        {
            partial class AboutBox
            {
                /// <summary>
                /// Required designer variable.
                /// </summary>
                private System.ComponentModel.IContainer components = null;

                /// <summary>
                /// Clean up any resources being used.
                /// </summary>
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
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
                    this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
                    this.labelSvnVersion = new System.Windows.Forms.Label();
                    this.logoPictureBox = new System.Windows.Forms.PictureBox();
                    this.labelProductName = new System.Windows.Forms.Label();
                    this.labelVersion = new System.Windows.Forms.Label();
                    this.labelCompanyName = new System.Windows.Forms.Label();
                    this.okButton = new System.Windows.Forms.Button();
                    this.labelCopyright = new System.Windows.Forms.Label();
                    this.textBoxAuthorList = new System.Windows.Forms.TextBox();
                    this.authorLabel = new System.Windows.Forms.Label();
                    this.tableLayoutPanel.SuspendLayout();
                    ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
                    this.SuspendLayout();
                    // 
                    // tableLayoutPanel
                    // 
                    this.tableLayoutPanel.ColumnCount = 2;
                    this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.74101F));
                    this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.259F));
                    this.tableLayoutPanel.Controls.Add(this.labelSvnVersion, 1, 2);
                    this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
                    this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
                    this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
                    this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
                    this.tableLayoutPanel.Controls.Add(this.okButton, 1, 7);
                    this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 4);
                    this.tableLayoutPanel.Controls.Add(this.textBoxAuthorList, 1, 6);
                    this.tableLayoutPanel.Controls.Add(this.authorLabel, 1, 5);
                    this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
                    this.tableLayoutPanel.Name = "tableLayoutPanel";
                    this.tableLayoutPanel.RowCount = 8;
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                    this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
                    this.tableLayoutPanel.Size = new System.Drawing.Size(417, 265);
                    this.tableLayoutPanel.TabIndex = 0;
                    // 
                    // labelSvnVersion
                    // 
                    this.labelSvnVersion.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.labelSvnVersion.Location = new System.Drawing.Point(104, 42);
                    this.labelSvnVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
                    this.labelSvnVersion.MaximumSize = new System.Drawing.Size(0, 17);
                    this.labelSvnVersion.Name = "labelSvnVersion";
                    this.labelSvnVersion.Size = new System.Drawing.Size(310, 17);
                    this.labelSvnVersion.TabIndex = 26;
                    this.labelSvnVersion.Text = "Repository Version";
                    this.labelSvnVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    // 
                    // logoPictureBox
                    // 
                    this.logoPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
                    this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
                    this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
                    this.logoPictureBox.Name = "logoPictureBox";
                    this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 7);
                    this.logoPictureBox.Size = new System.Drawing.Size(92, 230);
                    this.logoPictureBox.TabIndex = 12;
                    this.logoPictureBox.TabStop = false;
                    // 
                    // labelProductName
                    // 
                    this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.labelProductName.Location = new System.Drawing.Point(104, 0);
                    this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
                    this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
                    this.labelProductName.Name = "labelProductName";
                    this.labelProductName.Size = new System.Drawing.Size(310, 17);
                    this.labelProductName.TabIndex = 19;
                    this.labelProductName.Text = "Product Name";
                    this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    // 
                    // labelVersion
                    // 
                    this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.labelVersion.Location = new System.Drawing.Point(104, 21);
                    this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
                    this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
                    this.labelVersion.Name = "labelVersion";
                    this.labelVersion.Size = new System.Drawing.Size(310, 17);
                    this.labelVersion.TabIndex = 0;
                    this.labelVersion.Text = "Version";
                    this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    // 
                    // labelCompanyName
                    // 
                    this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.labelCompanyName.Location = new System.Drawing.Point(104, 63);
                    this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
                    this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
                    this.labelCompanyName.Name = "labelCompanyName";
                    this.labelCompanyName.Size = new System.Drawing.Size(310, 17);
                    this.labelCompanyName.TabIndex = 22;
                    this.labelCompanyName.Text = "Company Name";
                    this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    // 
                    // okButton
                    // 
                    this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                    this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.okButton.Location = new System.Drawing.Point(339, 239);
                    this.okButton.Name = "okButton";
                    this.okButton.Size = new System.Drawing.Size(75, 23);
                    this.okButton.TabIndex = 24;
                    this.okButton.Text = "&OK";
                    // 
                    // labelCopyright
                    // 
                    this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.labelCopyright.Location = new System.Drawing.Point(104, 84);
                    this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
                    this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
                    this.labelCopyright.Name = "labelCopyright";
                    this.labelCopyright.Size = new System.Drawing.Size(310, 17);
                    this.labelCopyright.TabIndex = 25;
                    this.labelCopyright.Text = "Copyright";
                    this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    // 
                    // textBoxAuthorList
                    // 
                    this.textBoxAuthorList.AcceptsReturn = true;
                    this.textBoxAuthorList.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.textBoxAuthorList.Location = new System.Drawing.Point(104, 131);
                    this.textBoxAuthorList.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
                    this.textBoxAuthorList.Multiline = true;
                    this.textBoxAuthorList.Name = "textBoxAuthorList";
                    this.textBoxAuthorList.ReadOnly = true;
                    this.textBoxAuthorList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
                    this.textBoxAuthorList.Size = new System.Drawing.Size(310, 102);
                    this.textBoxAuthorList.TabIndex = 27;
                    this.textBoxAuthorList.TabStop = false;
                    this.textBoxAuthorList.Text = "Paul Barber\r\nRos Locke\r\nGlenn Pierce\r\nRichard Edens\r\nVladan Rankov";
                    // 
                    // authorLabel
                    // 
                    this.authorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.authorLabel.Location = new System.Drawing.Point(104, 108);
                    this.authorLabel.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
                    this.authorLabel.MaximumSize = new System.Drawing.Size(0, 17);
                    this.authorLabel.Name = "authorLabel";
                    this.authorLabel.Size = new System.Drawing.Size(310, 17);
                    this.authorLabel.TabIndex = 28;
                    this.authorLabel.Text = "Authors";
                    this.authorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    // 
                    // AboutBox
                    // 
                    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                    this.ClientSize = new System.Drawing.Size(435, 283);
                    this.Controls.Add(this.tableLayoutPanel);
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                    this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                    this.MaximizeBox = false;
                    this.MinimizeBox = false;
                    this.Name = "AboutBox";
                    this.Padding = new System.Windows.Forms.Padding(9);
                    this.ShowIcon = false;
                    this.ShowInTaskbar = false;
                    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                    this.Text = "AboutBox";
                    this.tableLayoutPanel.ResumeLayout(false);
                    this.tableLayoutPanel.PerformLayout();
                    ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
                    this.ResumeLayout(false);

                }

                #endregion

                private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
                private System.Windows.Forms.PictureBox logoPictureBox;
                private System.Windows.Forms.Label labelProductName;
                private System.Windows.Forms.Label labelVersion;
                private System.Windows.Forms.Label labelCompanyName;
                private System.Windows.Forms.Button okButton;
                private System.Windows.Forms.Label labelSvnVersion;
                private System.Windows.Forms.Label labelCopyright;
                private System.Windows.Forms.Label authorLabel;
                private System.Windows.Forms.TextBox textBoxAuthorList;
            }
        }
    }
}
