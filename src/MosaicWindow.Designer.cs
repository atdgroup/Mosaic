namespace ImageStitching
{
    partial class MosaicWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MosaicWindow));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.screenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cacheMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOverviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFilenamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showJoinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.processingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performCorrelationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performBlendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.fovStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.correlationStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.blendedStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.viewPanel = new System.Windows.Forms.Panel();
            this.tileViewEditToolBox = new ImageStitching.TileViewEditToolBox();
            this.tileOverView = new ImageStitching.TileOverView();
            this.statusbarStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.feedbackStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolbarStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.mosaicToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.exportToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripZoomComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.profileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.roiToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.linearScaleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.viewPanel.SuspendLayout();
            this.statusbarStripContainer.ContentPanel.SuspendLayout();
            this.statusbarStripContainer.SuspendLayout();
            this.feedbackStatusStrip.SuspendLayout();
            this.toolbarStripContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.processingToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1413, 28);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openToolStripMenuItem2,
            this.OpenToolStripMenuItem3,
            this.toolStripSeparator,
            this.screenshotToolStripMenuItem,
            this.exportMenuItem,
            this.cacheMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OnOpenMenuClicked);
            // 
            // openToolStripMenuItem2
            // 
            this.openToolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem2.Image")));
            this.openToolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem2.Name = "openToolStripMenuItem2";
            this.openToolStripMenuItem2.ShowShortcutKeys = false;
            this.openToolStripMenuItem2.Size = new System.Drawing.Size(205, 24);
            this.openToolStripMenuItem2.Text = "Import Seq File(s)";
            this.openToolStripMenuItem2.Click += new System.EventHandler(this.OnOpenMultiSeqClicked);
            // 
            // OpenToolStripMenuItem3
            // 
            this.OpenToolStripMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("OpenToolStripMenuItem3.Image")));
            this.OpenToolStripMenuItem3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenToolStripMenuItem3.Name = "OpenToolStripMenuItem3";
            this.OpenToolStripMenuItem3.ShowShortcutKeys = false;
            this.OpenToolStripMenuItem3.Size = new System.Drawing.Size(205, 24);
            this.OpenToolStripMenuItem3.Text = "Import Images";
            this.OpenToolStripMenuItem3.Click += new System.EventHandler(this.OnOpenImagesClicked);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(202, 6);
            // 
            // screenshotToolStripMenuItem
            // 
            this.screenshotToolStripMenuItem.Name = "screenshotToolStripMenuItem";
            this.screenshotToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            this.screenshotToolStripMenuItem.Text = "ScreenShot";
            this.screenshotToolStripMenuItem.Click += new System.EventHandler(this.OnScreenShotMenuClicked);
            // 
            // exportMenuItem
            // 
            this.exportMenuItem.Name = "exportMenuItem";
            this.exportMenuItem.Size = new System.Drawing.Size(205, 24);
            this.exportMenuItem.Text = "Export";
            this.exportMenuItem.Click += new System.EventHandler(this.OnExportClicked);
            // 
            // cacheMenuItem
            // 
            this.cacheMenuItem.Name = "cacheMenuItem";
            this.cacheMenuItem.Size = new System.Drawing.Size(205, 24);
            this.cacheMenuItem.Text = "Reload Thumbnails";
            this.cacheMenuItem.Click += new System.EventHandler(this.OnReloadThumbnailsClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExitMenuItemClicked);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.histogramToolStripMenuItem,
            this.imageInformationToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // histogramToolStripMenuItem
            // 
            this.histogramToolStripMenuItem.Name = "histogramToolStripMenuItem";
            this.histogramToolStripMenuItem.Size = new System.Drawing.Size(202, 24);
            this.histogramToolStripMenuItem.Text = "Histogram";
            this.histogramToolStripMenuItem.Visible = false;
            this.histogramToolStripMenuItem.Click += new System.EventHandler(this.OnHistogramShowClicked);
            // 
            // imageInformationToolStripMenuItem
            // 
            this.imageInformationToolStripMenuItem.Name = "imageInformationToolStripMenuItem";
            this.imageInformationToolStripMenuItem.Size = new System.Drawing.Size(202, 24);
            this.imageInformationToolStripMenuItem.Text = "Image Information";
            this.imageInformationToolStripMenuItem.Click += new System.EventHandler(this.OnImageInformationClicked);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showOverviewToolStripMenuItem,
            this.showFilenamesToolStripMenuItem,
            this.showJoinsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.toolsToolStripMenuItem.Text = "Options";
            // 
            // showOverviewToolStripMenuItem
            // 
            this.showOverviewToolStripMenuItem.Checked = true;
            this.showOverviewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showOverviewToolStripMenuItem.Name = "showOverviewToolStripMenuItem";
            this.showOverviewToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.showOverviewToolStripMenuItem.Text = "Show Overview";
            this.showOverviewToolStripMenuItem.Click += new System.EventHandler(this.OnShowOverviewClicked);
            // 
            // showFilenamesToolStripMenuItem
            // 
            this.showFilenamesToolStripMenuItem.Name = "showFilenamesToolStripMenuItem";
            this.showFilenamesToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.showFilenamesToolStripMenuItem.Text = "Show Filenames";
            this.showFilenamesToolStripMenuItem.Visible = false;
            this.showFilenamesToolStripMenuItem.Click += new System.EventHandler(this.showFilenamesToolStripMenuItem_Click);
            // 
            // showJoinsToolStripMenuItem
            // 
            this.showJoinsToolStripMenuItem.Name = "showJoinsToolStripMenuItem";
            this.showJoinsToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.showJoinsToolStripMenuItem.Text = "Show Joins";
            this.showJoinsToolStripMenuItem.Click += new System.EventHandler(this.OnShowJoinsMenuItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(70, 24);
            this.toolStripMenuItem1.Text = "Display";
            // 
            // processingToolStripMenuItem
            // 
            this.processingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.performCorrelationToolStripMenuItem,
            this.performBlendingToolStripMenuItem});
            this.processingToolStripMenuItem.Name = "processingToolStripMenuItem";
            this.processingToolStripMenuItem.Size = new System.Drawing.Size(91, 24);
            this.processingToolStripMenuItem.Text = "Processing";
            // 
            // performCorrelationToolStripMenuItem
            // 
            this.performCorrelationToolStripMenuItem.Name = "performCorrelationToolStripMenuItem";
            this.performCorrelationToolStripMenuItem.Size = new System.Drawing.Size(209, 24);
            this.performCorrelationToolStripMenuItem.Text = "Perform Correlation";
            this.performCorrelationToolStripMenuItem.Click += new System.EventHandler(this.OnShowCorrelationWindow);
            // 
            // performBlendingToolStripMenuItem
            // 
            this.performBlendingToolStripMenuItem.Checked = true;
            this.performBlendingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.performBlendingToolStripMenuItem.Name = "performBlendingToolStripMenuItem";
            this.performBlendingToolStripMenuItem.Size = new System.Drawing.Size(209, 24);
            this.performBlendingToolStripMenuItem.Text = "Perform Blending";
            this.performBlendingToolStripMenuItem.Click += new System.EventHandler(this.OnBlendingClicked);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(128, 24);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.OnHelpClicked);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnAboutMenuItemClicked);
            // 
            // statusStrip
            // 
            this.statusStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fovStatusLabel,
            this.correlationStatusLabel,
            this.blendedStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 3);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1413, 29);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 13;
            this.statusStrip.Text = "statusStrip1";
            // 
            // fovStatusLabel
            // 
            this.fovStatusLabel.AutoSize = false;
            this.fovStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.fovStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fovStatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fovStatusLabel.Name = "fovStatusLabel";
            this.fovStatusLabel.Size = new System.Drawing.Size(200, 24);
            this.fovStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // correlationStatusLabel
            // 
            this.correlationStatusLabel.Name = "correlationStatusLabel";
            this.correlationStatusLabel.Size = new System.Drawing.Size(79, 24);
            this.correlationStatusLabel.Text = "Correlated";
            this.correlationStatusLabel.Click += new System.EventHandler(this.OnCorrelatedLabelClicked);
            // 
            // blendedStatusLabel
            // 
            this.blendedStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.blendedStatusLabel.Name = "blendedStatusLabel";
            this.blendedStatusLabel.Size = new System.Drawing.Size(68, 24);
            this.blendedStatusLabel.Text = "Blended";
            this.blendedStatusLabel.Click += new System.EventHandler(this.OnBlendedLabelClick);
            // 
            // viewPanel
            // 
            this.viewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.viewPanel.Controls.Add(this.tileViewEditToolBox);
            this.viewPanel.Controls.Add(this.tileOverView);
            this.viewPanel.Location = new System.Drawing.Point(0, 64);
            this.viewPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.viewPanel.Name = "viewPanel";
            this.viewPanel.Size = new System.Drawing.Size(1413, 730);
            this.viewPanel.TabIndex = 14;
            // 
            // tileViewEditToolBox
            // 
            this.tileViewEditToolBox.BackColor = System.Drawing.SystemColors.Control;
            this.tileViewEditToolBox.Location = new System.Drawing.Point(16, 15);
            this.tileViewEditToolBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.tileViewEditToolBox.Name = "tileViewEditToolBox";
            this.tileViewEditToolBox.Size = new System.Drawing.Size(135, 74);
            this.tileViewEditToolBox.TabIndex = 1;
            // 
            // tileOverView
            // 
            this.tileOverView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tileOverView.Location = new System.Drawing.Point(13, 359);
            this.tileOverView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tileOverView.Name = "tileOverView";
            this.tileOverView.Size = new System.Drawing.Size(343, 366);
            this.tileOverView.TabIndex = 0;
            this.tileOverView.Visible = false;
            // 
            // statusbarStripContainer
            // 
            // 
            // statusbarStripContainer.ContentPanel
            // 
            this.statusbarStripContainer.ContentPanel.Controls.Add(this.feedbackStatusStrip);
            this.statusbarStripContainer.ContentPanel.Controls.Add(this.statusStrip);
            this.statusbarStripContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.statusbarStripContainer.ContentPanel.Size = new System.Drawing.Size(1413, 32);
            this.statusbarStripContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusbarStripContainer.LeftToolStripPanelVisible = false;
            this.statusbarStripContainer.Location = new System.Drawing.Point(0, 794);
            this.statusbarStripContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.statusbarStripContainer.Name = "statusbarStripContainer";
            this.statusbarStripContainer.RightToolStripPanelVisible = false;
            this.statusbarStripContainer.Size = new System.Drawing.Size(1413, 32);
            this.statusbarStripContainer.TabIndex = 15;
            this.statusbarStripContainer.Text = "toolStripContainer1";
            this.statusbarStripContainer.TopToolStripPanelVisible = false;
            // 
            // feedbackStatusStrip
            // 
            this.feedbackStatusStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.feedbackStatusStrip.AutoSize = false;
            this.feedbackStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.feedbackStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1});
            this.feedbackStatusStrip.Location = new System.Drawing.Point(563, 2);
            this.feedbackStatusStrip.Name = "feedbackStatusStrip";
            this.feedbackStatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.feedbackStatusStrip.Size = new System.Drawing.Size(851, 30);
            this.feedbackStatusStrip.TabIndex = 14;
            this.feedbackStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(460, 25);
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 24);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(957, 578);
            // 
            // toolbarStripContainer
            // 
            this.toolbarStripContainer.AllowDrop = true;
            this.toolbarStripContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // toolbarStripContainer.ContentPanel
            // 
            this.toolbarStripContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolbarStripContainer.ContentPanel.Size = new System.Drawing.Size(1413, 733);
            this.toolbarStripContainer.Location = new System.Drawing.Point(0, 30);
            this.toolbarStripContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolbarStripContainer.Name = "toolbarStripContainer";
            this.toolbarStripContainer.Size = new System.Drawing.Size(1413, 764);
            this.toolbarStripContainer.TabIndex = 16;
            this.toolbarStripContainer.Text = "toolStripContainer2";
            // 
            // mosaicToolStrip
            // 
            this.mosaicToolStrip.CanOverflow = false;
            this.mosaicToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mosaicToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mosaicToolStrip.Location = new System.Drawing.Point(176, 30);
            this.mosaicToolStrip.Name = "mosaicToolStrip";
            this.mosaicToolStrip.Size = new System.Drawing.Size(102, 25);
            this.mosaicToolStrip.TabIndex = 16;
            this.mosaicToolStrip.Text = "toolStrip1";
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.exportToolStripButton,
            this.toolStripSeparator2,
            this.toolStripZoomComboBox});
            this.toolStrip.Location = new System.Drawing.Point(0, 30);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(156, 28);
            this.toolStrip.TabIndex = 14;
            this.toolStrip.Text = "toolStrip";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 25);
            this.openToolStripButton.Text = "toolStripButton1";
            this.openToolStripButton.ToolTipText = "Open mosaic";
            this.openToolStripButton.Click += new System.EventHandler(this.OnToolStripOpenButtonClicked);
            // 
            // exportToolStripButton
            // 
            this.exportToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("exportToolStripButton.Image")));
            this.exportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportToolStripButton.Name = "exportToolStripButton";
            this.exportToolStripButton.Size = new System.Drawing.Size(23, 25);
            this.exportToolStripButton.Text = "toolStripButton1";
            this.exportToolStripButton.ToolTipText = "Export mosaic or selection";
            this.exportToolStripButton.Click += new System.EventHandler(this.OnExportToolButton);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripZoomComboBox
            // 
            this.toolStripZoomComboBox.DropDownWidth = 30;
            this.toolStripZoomComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripZoomComboBox.Items.AddRange(new object[] {
            "500%",
            "300%",
            "250%",
            "100%",
            "75%",
            "50%",
            "25%",
            "Fit"});
            this.toolStripZoomComboBox.Name = "toolStripZoomComboBox";
            this.toolStripZoomComboBox.Size = new System.Drawing.Size(99, 28);
            this.toolStripZoomComboBox.ToolTipText = "Zoom in or out of the display image";
            this.toolStripZoomComboBox.SelectedIndexChanged += new System.EventHandler(this.OnZoomComboBoxSelectedChanged);
            this.toolStripZoomComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnZoomComboBoxKeyDown);
            // 
            // profileToolStripButton
            // 
            this.profileToolStripButton.CheckOnClick = true;
            this.profileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.profileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("profileToolStripButton.Image")));
            this.profileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.profileToolStripButton.Name = "profileToolStripButton";
            this.profileToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.profileToolStripButton.Text = "Profile";
            // 
            // roiToolStripButton
            // 
            this.roiToolStripButton.CheckOnClick = true;
            this.roiToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.roiToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("roiToolStripButton.Image")));
            this.roiToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.roiToolStripButton.Name = "roiToolStripButton";
            this.roiToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.roiToolStripButton.Text = "roiButton1";
            // 
            // linearScaleToolStripButton
            // 
            this.linearScaleToolStripButton.CheckOnClick = true;
            this.linearScaleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.linearScaleToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("linearScaleToolStripButton.Image")));
            this.linearScaleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.linearScaleToolStripButton.Name = "linearScaleToolStripButton";
            this.linearScaleToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.linearScaleToolStripButton.Text = "Linear Scale";
            // 
            // MosaicWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 826);
            this.Controls.Add(this.mosaicToolStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.viewPanel);
            this.Controls.Add(this.statusbarStripContainer);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.toolbarStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(766, 356);
            this.Name = "MosaicWindow";
            this.Text = "Mosaic";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnWindowClosing);
            this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.viewPanel.ResumeLayout(false);
            this.statusbarStripContainer.ContentPanel.ResumeLayout(false);
            this.statusbarStripContainer.ContentPanel.PerformLayout();
            this.statusbarStripContainer.ResumeLayout(false);
            this.statusbarStripContainer.PerformLayout();
            this.feedbackStatusStrip.ResumeLayout(false);
            this.feedbackStatusStrip.PerformLayout();
            this.toolbarStripContainer.ResumeLayout(false);
            this.toolbarStripContainer.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOverviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel fovStatusLabel;
        private System.Windows.Forms.Panel viewPanel;
        private System.Windows.Forms.ToolStripMenuItem screenshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportMenuItem;
        private System.Windows.Forms.ToolStripContainer statusbarStripContainer;
        private System.Windows.Forms.StatusStrip feedbackStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripMenuItem showFilenamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem histogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolbarStripContainer;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton exportToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton profileToolStripButton;
        private System.Windows.Forms.ToolStripButton roiToolStripButton;
        private System.Windows.Forms.ToolStripButton linearScaleToolStripButton;
        private System.Windows.Forms.ToolStrip mosaicToolStrip;
        private System.Windows.Forms.ToolStripMenuItem imageInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showJoinsToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripZoomComboBox;
        private System.Windows.Forms.ToolStripMenuItem processingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performCorrelationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performBlendingToolStripMenuItem;
        private TileOverView tileOverView;
        private TileViewEditToolBox tileViewEditToolBox;
        private System.Windows.Forms.ToolStripStatusLabel correlationStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel blendedStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cacheMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem3;
    }
}