namespace PhotoSift
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.timerAutoAdvance = new System.Windows.Forms.Timer(this.components);
			this.timerHideStatusLabel = new System.Windows.Forms.Timer(this.components);
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblInfoLabel = new System.Windows.Forms.Label();
			this.lblHeader = new System.Windows.Forms.Label();
			this.picCurrent = new System.Windows.Forms.PictureBox();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSetTargetFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuRenameFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuOpenSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAutoAdvanceEnabled = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuResetViewMode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuTransform = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFlipX = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFlipY = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuRotateLeft = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuRotateRight = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuZoom = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuZoomIn = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuZoomOut = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuZoomToWidth = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuZoomToHeight = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuResetZoom = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuHideMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuFullscreen = new System.Windows.Forms.ToolStripMenuItem();
			this.imagePoolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigate = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigateNext = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigatePrev = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuNavigateFirst = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigateLast = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuNavigateForwardMedium = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigateBackMedium = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigateForwardLarge = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuNavigateBackLarge = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuAddImages = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAddFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuClearImages = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuAddInRandomOrder = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuRandimizeOrder = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHomepage = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.picLogo = new System.Windows.Forms.PictureBox();
			this.panelMain = new System.Windows.Forms.Panel();
			this.timerMouseHider = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.picCurrent)).BeginInit();
			this.menuStripMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
			this.panelMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerAutoAdvance
			// 
			this.timerAutoAdvance.Interval = 1000;
			this.timerAutoAdvance.Tick += new System.EventHandler(this.timerAutoAdvance_Tick);
			// 
			// timerHideStatusLabel
			// 
			this.timerHideStatusLabel.Interval = 2000;
			this.timerHideStatusLabel.Tick += new System.EventHandler(this.timerHideStatusLabel_Tick);
			// 
			// lblStatus
			// 
			this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblStatus.AutoSize = true;
			this.lblStatus.BackColor = System.Drawing.Color.Black;
			this.lblStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStatus.ForeColor = System.Drawing.Color.Gray;
			this.lblStatus.Location = new System.Drawing.Point(2, 274);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(53, 16);
			this.lblStatus.TabIndex = 8;
			this.lblStatus.Text = "lblMode";
			this.lblStatus.Visible = false;
			// 
			// lblInfoLabel
			// 
			this.lblInfoLabel.AutoSize = true;
			this.lblInfoLabel.BackColor = System.Drawing.Color.Black;
			this.lblInfoLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfoLabel.ForeColor = System.Drawing.Color.Gray;
			this.lblInfoLabel.Location = new System.Drawing.Point(2, 2);
			this.lblInfoLabel.Name = "lblInfoLabel";
			this.lblInfoLabel.Size = new System.Drawing.Size(222, 16);
			this.lblInfoLabel.TabIndex = 7;
			this.lblInfoLabel.Text = "lblInfoLabel (only shows in fullscreen)";
			// 
			// lblHeader
			// 
			this.lblHeader.AutoSize = true;
			this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHeader.ForeColor = System.Drawing.SystemColors.GrayText;
			this.lblHeader.Location = new System.Drawing.Point(186, 220);
			this.lblHeader.Name = "lblHeader";
			this.lblHeader.Size = new System.Drawing.Size(102, 23);
			this.lblHeader.TabIndex = 5;
			this.lblHeader.Tag = "";
			this.lblHeader.Text = "lblHeader";
			this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// picCurrent
			// 
			this.picCurrent.BackColor = System.Drawing.Color.Black;
			this.picCurrent.Location = new System.Drawing.Point(19, 34);
			this.picCurrent.Margin = new System.Windows.Forms.Padding(0);
			this.picCurrent.Name = "picCurrent";
			this.picCurrent.Size = new System.Drawing.Size(98, 66);
			this.picCurrent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picCurrent.TabIndex = 6;
			this.picCurrent.TabStop = false;
			this.picCurrent.Visible = false;
			this.picCurrent.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picCurrent_MouseMove);
			this.picCurrent.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picCurrent_MouseUp);
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.imagePoolToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStripMain.Location = new System.Drawing.Point(0, 0);
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.menuStripMain.Size = new System.Drawing.Size(472, 24);
			this.menuStripMain.TabIndex = 9;
			this.menuStripMain.Text = "menuStrip1";
			this.menuStripMain.MenuActivate += new System.EventHandler(this.menuStripMain_MenuActivate);
			this.menuStripMain.MenuDeactivate += new System.EventHandler(this.menuStripMain_MenuDeactivate);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSetTargetFolder,
            this.toolStripMenuItem7,
            this.mnuRenameFile,
            this.toolStripMenuItem4,
            this.mnuExit});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// mnuSetTargetFolder
			// 
			this.mnuSetTargetFolder.Name = "mnuSetTargetFolder";
			this.mnuSetTargetFolder.ShortcutKeys = System.Windows.Forms.Keys.F9;
			this.mnuSetTargetFolder.Size = new System.Drawing.Size(203, 22);
			this.mnuSetTargetFolder.Text = "Set Target Base Folder";
			this.mnuSetTargetFolder.Click += new System.EventHandler(this.mnuSetTargetFolder_Click);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(200, 6);
			// 
			// mnuRenameFile
			// 
			this.mnuRenameFile.Name = "mnuRenameFile";
			this.mnuRenameFile.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.mnuRenameFile.Size = new System.Drawing.Size(203, 22);
			this.mnuRenameFile.Text = "Rename File";
			this.mnuRenameFile.Click += new System.EventHandler(this.mnuRenameFile_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(200, 6);
			// 
			// mnuExit
			// 
			this.mnuExit.Name = "mnuExit";
			this.mnuExit.ShortcutKeyDisplayString = "Alt + F4";
			this.mnuExit.Size = new System.Drawing.Size(203, 22);
			this.mnuExit.Text = "Exit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUndo,
            this.toolStripMenuItem3,
            this.mnuCopyToClipboard,
            this.toolStripMenuItem5,
            this.mnuOpenSettings});
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.settingsToolStripMenuItem.Text = "&Edit";
			// 
			// mnuUndo
			// 
			this.mnuUndo.Name = "mnuUndo";
			this.mnuUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.mnuUndo.Size = new System.Drawing.Size(199, 22);
			this.mnuUndo.Text = "Undo";
			this.mnuUndo.Click += new System.EventHandler(this.mnuUndo_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(196, 6);
			// 
			// mnuCopyToClipboard
			// 
			this.mnuCopyToClipboard.Name = "mnuCopyToClipboard";
			this.mnuCopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mnuCopyToClipboard.Size = new System.Drawing.Size(199, 22);
			this.mnuCopyToClipboard.Text = "Copy to Clipboard";
			this.mnuCopyToClipboard.Click += new System.EventHandler(this.mnuCopyToClipboard_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(196, 6);
			// 
			// mnuOpenSettings
			// 
			this.mnuOpenSettings.Name = "mnuOpenSettings";
			this.mnuOpenSettings.ShortcutKeyDisplayString = "";
			this.mnuOpenSettings.ShortcutKeys = System.Windows.Forms.Keys.F12;
			this.mnuOpenSettings.Size = new System.Drawing.Size(199, 22);
			this.mnuOpenSettings.Text = "Settings";
			this.mnuOpenSettings.Click += new System.EventHandler(this.mnuOpenSettings_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAutoAdvanceEnabled,
            this.mnuResetViewMode,
            this.toolStripMenuItem6,
            this.mnuTransform,
            this.mnuZoom,
            this.toolStripMenuItem9,
            this.mnuHideMenu,
            this.mnuFullscreen});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// mnuAutoAdvanceEnabled
			// 
			this.mnuAutoAdvanceEnabled.Name = "mnuAutoAdvanceEnabled";
			this.mnuAutoAdvanceEnabled.ShortcutKeyDisplayString = "Pause";
			this.mnuAutoAdvanceEnabled.Size = new System.Drawing.Size(220, 22);
			this.mnuAutoAdvanceEnabled.Text = "Auto Advance";
			this.mnuAutoAdvanceEnabled.Click += new System.EventHandler(this.mnuAutoAdvanceEnabled_Click);
			// 
			// mnuResetViewMode
			// 
			this.mnuResetViewMode.Name = "mnuResetViewMode";
			this.mnuResetViewMode.ShortcutKeys = System.Windows.Forms.Keys.F8;
			this.mnuResetViewMode.Size = new System.Drawing.Size(220, 22);
			this.mnuResetViewMode.Text = "Reset Scale Mode Each Pic";
			this.mnuResetViewMode.Click += new System.EventHandler(this.mnuResetViewMode_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(217, 6);
			// 
			// mnuTransform
			// 
			this.mnuTransform.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFlipX,
            this.mnuFlipY,
            this.mnuRotateLeft,
            this.mnuRotateRight});
			this.mnuTransform.Name = "mnuTransform";
			this.mnuTransform.Size = new System.Drawing.Size(220, 22);
			this.mnuTransform.Text = "Transform";
			// 
			// mnuFlipX
			// 
			this.mnuFlipX.Name = "mnuFlipX";
			this.mnuFlipX.ShortcutKeyDisplayString = "";
			this.mnuFlipX.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Down)));
			this.mnuFlipX.Size = new System.Drawing.Size(222, 22);
			this.mnuFlipX.Text = "Flip X";
			this.mnuFlipX.Click += new System.EventHandler(this.mnuFlipImage_Click);
			// 
			// mnuFlipY
			// 
			this.mnuFlipY.Name = "mnuFlipY";
			this.mnuFlipY.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Up)));
			this.mnuFlipY.Size = new System.Drawing.Size(222, 22);
			this.mnuFlipY.Text = "Flip Y";
			this.mnuFlipY.Click += new System.EventHandler(this.mnuFlipY_Click);
			// 
			// mnuRotateLeft
			// 
			this.mnuRotateLeft.Name = "mnuRotateLeft";
			this.mnuRotateLeft.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Left)));
			this.mnuRotateLeft.Size = new System.Drawing.Size(222, 22);
			this.mnuRotateLeft.Text = "Rotate Left";
			this.mnuRotateLeft.Click += new System.EventHandler(this.mnuRotateLeft_Click);
			// 
			// mnuRotateRight
			// 
			this.mnuRotateRight.Name = "mnuRotateRight";
			this.mnuRotateRight.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Right)));
			this.mnuRotateRight.Size = new System.Drawing.Size(222, 22);
			this.mnuRotateRight.Text = "Rotate Right";
			this.mnuRotateRight.Click += new System.EventHandler(this.mnuRotateRight_Click);
			// 
			// mnuZoom
			// 
			this.mnuZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuZoomIn,
            this.mnuZoomOut,
            this.mnuZoomToWidth,
            this.mnuZoomToHeight,
            this.toolStripMenuItem13,
            this.mnuResetZoom});
			this.mnuZoom.Name = "mnuZoom";
			this.mnuZoom.Size = new System.Drawing.Size(220, 22);
			this.mnuZoom.Text = "Zoom";
			// 
			// mnuZoomIn
			// 
			this.mnuZoomIn.Name = "mnuZoomIn";
			this.mnuZoomIn.ShortcutKeyDisplayString = "+";
			this.mnuZoomIn.Size = new System.Drawing.Size(186, 22);
			this.mnuZoomIn.Text = "Zoom In";
			this.mnuZoomIn.Click += new System.EventHandler(this.mnuZoomIn_Click);
			// 
			// mnuZoomOut
			// 
			this.mnuZoomOut.Name = "mnuZoomOut";
			this.mnuZoomOut.ShortcutKeyDisplayString = "−";
			this.mnuZoomOut.Size = new System.Drawing.Size(186, 22);
			this.mnuZoomOut.Text = "Zoom Out";
			this.mnuZoomOut.Click += new System.EventHandler(this.mnuZoomOut_Click);
			// 
			// mnuZoomToWidth
			// 
			this.mnuZoomToWidth.Name = "mnuZoomToWidth";
			this.mnuZoomToWidth.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
			this.mnuZoomToWidth.Size = new System.Drawing.Size(186, 22);
			this.mnuZoomToWidth.Text = "Zoom to Width";
			this.mnuZoomToWidth.Click += new System.EventHandler(this.mnuZoomToWidth_Click);
			// 
			// mnuZoomToHeight
			// 
			this.mnuZoomToHeight.Name = "mnuZoomToHeight";
			this.mnuZoomToHeight.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.mnuZoomToHeight.Size = new System.Drawing.Size(186, 22);
			this.mnuZoomToHeight.Text = "Zoom to Height";
			this.mnuZoomToHeight.Click += new System.EventHandler(this.mnuZoomToHeight_Click);
			// 
			// toolStripMenuItem13
			// 
			this.toolStripMenuItem13.Name = "toolStripMenuItem13";
			this.toolStripMenuItem13.Size = new System.Drawing.Size(183, 6);
			// 
			// mnuResetZoom
			// 
			this.mnuResetZoom.Name = "mnuResetZoom";
			this.mnuResetZoom.ShortcutKeyDisplayString = "Backspace";
			this.mnuResetZoom.Size = new System.Drawing.Size(186, 22);
			this.mnuResetZoom.Text = "Reset";
			this.mnuResetZoom.Click += new System.EventHandler(this.mnuResetZoom_Click);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(217, 6);
			// 
			// mnuHideMenu
			// 
			this.mnuHideMenu.Name = "mnuHideMenu";
			this.mnuHideMenu.ShortcutKeyDisplayString = "Tab";
			this.mnuHideMenu.Size = new System.Drawing.Size(220, 22);
			this.mnuHideMenu.Text = "Hide Menubar";
			this.mnuHideMenu.Click += new System.EventHandler(this.mnuHideMenu_Click);
			// 
			// mnuFullscreen
			// 
			this.mnuFullscreen.Name = "mnuFullscreen";
			this.mnuFullscreen.ShortcutKeyDisplayString = "";
			this.mnuFullscreen.ShortcutKeys = System.Windows.Forms.Keys.F11;
			this.mnuFullscreen.Size = new System.Drawing.Size(220, 22);
			this.mnuFullscreen.Text = "Fullscreen";
			this.mnuFullscreen.Click += new System.EventHandler(this.mnuFullscreen_Click);
			// 
			// imagePoolToolStripMenuItem
			// 
			this.imagePoolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNavigate,
            this.toolStripMenuItem8,
            this.mnuAddImages,
            this.mnuAddFolder,
            this.mnuClearImages,
            this.toolStripMenuItem1,
            this.mnuAddInRandomOrder,
            this.mnuRandimizeOrder});
			this.imagePoolToolStripMenuItem.Name = "imagePoolToolStripMenuItem";
			this.imagePoolToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
			this.imagePoolToolStripMenuItem.Text = "&Image Pool";
			// 
			// mnuNavigate
			// 
			this.mnuNavigate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNavigateNext,
            this.mnuNavigatePrev,
            this.toolStripMenuItem10,
            this.mnuNavigateFirst,
            this.mnuNavigateLast,
            this.toolStripMenuItem11,
            this.mnuNavigateForwardMedium,
            this.mnuNavigateBackMedium,
            this.mnuNavigateForwardLarge,
            this.mnuNavigateBackLarge});
			this.mnuNavigate.Name = "mnuNavigate";
			this.mnuNavigate.Size = new System.Drawing.Size(220, 22);
			this.mnuNavigate.Text = "Navigate";
			// 
			// mnuNavigateNext
			// 
			this.mnuNavigateNext.Name = "mnuNavigateNext";
			this.mnuNavigateNext.ShortcutKeyDisplayString = "Right";
			this.mnuNavigateNext.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateNext.Text = "Next Image";
			this.mnuNavigateNext.Click += new System.EventHandler(this.mnuNavigateNext_Click);
			// 
			// mnuNavigatePrev
			// 
			this.mnuNavigatePrev.Name = "mnuNavigatePrev";
			this.mnuNavigatePrev.ShortcutKeyDisplayString = "Left";
			this.mnuNavigatePrev.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigatePrev.Text = "Previous Image";
			this.mnuNavigatePrev.Click += new System.EventHandler(this.mnuNavigatePrev_Click);
			// 
			// toolStripMenuItem10
			// 
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Size = new System.Drawing.Size(243, 6);
			// 
			// mnuNavigateFirst
			// 
			this.mnuNavigateFirst.Name = "mnuNavigateFirst";
			this.mnuNavigateFirst.ShortcutKeyDisplayString = "Home";
			this.mnuNavigateFirst.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateFirst.Text = "Go to First";
			this.mnuNavigateFirst.Click += new System.EventHandler(this.mnuNavigateFirst_Click);
			// 
			// mnuNavigateLast
			// 
			this.mnuNavigateLast.Name = "mnuNavigateLast";
			this.mnuNavigateLast.ShortcutKeyDisplayString = "End";
			this.mnuNavigateLast.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateLast.Text = "Go to Last";
			this.mnuNavigateLast.Click += new System.EventHandler(this.mnuNavigateLast_Click);
			// 
			// toolStripMenuItem11
			// 
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.Size = new System.Drawing.Size(243, 6);
			// 
			// mnuNavigateForwardMedium
			// 
			this.mnuNavigateForwardMedium.Name = "mnuNavigateForwardMedium";
			this.mnuNavigateForwardMedium.ShortcutKeyDisplayString = "Ctrl+Right";
			this.mnuNavigateForwardMedium.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateForwardMedium.Text = "Forward Jump (Medium)";
			this.mnuNavigateForwardMedium.Click += new System.EventHandler(this.mnuNavigateForwardMedium_Click);
			// 
			// mnuNavigateBackMedium
			// 
			this.mnuNavigateBackMedium.Name = "mnuNavigateBackMedium";
			this.mnuNavigateBackMedium.ShortcutKeyDisplayString = "Ctrl+Left";
			this.mnuNavigateBackMedium.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateBackMedium.Text = "Backward Jump (Medium)";
			this.mnuNavigateBackMedium.Click += new System.EventHandler(this.mnuNavigateBackMedium_Click);
			// 
			// mnuNavigateForwardLarge
			// 
			this.mnuNavigateForwardLarge.Name = "mnuNavigateForwardLarge";
			this.mnuNavigateForwardLarge.ShortcutKeyDisplayString = "Shift+Right";
			this.mnuNavigateForwardLarge.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateForwardLarge.Text = "Forward Jump (Large)";
			this.mnuNavigateForwardLarge.Click += new System.EventHandler(this.mnuNavigateForwardLarge_Click);
			// 
			// mnuNavigateBackLarge
			// 
			this.mnuNavigateBackLarge.Name = "mnuNavigateBackLarge";
			this.mnuNavigateBackLarge.ShortcutKeyDisplayString = "Shift+Left";
			this.mnuNavigateBackLarge.Size = new System.Drawing.Size(246, 22);
			this.mnuNavigateBackLarge.Text = "Backward Jump (Large)";
			this.mnuNavigateBackLarge.Click += new System.EventHandler(this.mnuNavigateBackLarge_Click);
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(217, 6);
			// 
			// mnuAddImages
			// 
			this.mnuAddImages.Name = "mnuAddImages";
			this.mnuAddImages.ShortcutKeys = System.Windows.Forms.Keys.Insert;
			this.mnuAddImages.Size = new System.Drawing.Size(220, 22);
			this.mnuAddImages.Text = "Add Images";
			this.mnuAddImages.Click += new System.EventHandler(this.mnuAddImages_Click);
			// 
			// mnuAddFolder
			// 
			this.mnuAddFolder.Name = "mnuAddFolder";
			this.mnuAddFolder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Insert)));
			this.mnuAddFolder.Size = new System.Drawing.Size(220, 22);
			this.mnuAddFolder.Text = "Add from Folder";
			this.mnuAddFolder.Click += new System.EventHandler(this.mnuAddFolder_Click);
			// 
			// mnuClearImages
			// 
			this.mnuClearImages.Name = "mnuClearImages";
            this.mnuClearImages.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
			this.mnuClearImages.Text = "Clear";
			this.mnuClearImages.Click += new System.EventHandler(this.mnuClearImages_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(217, 6);
			// 
			// mnuAddInRandomOrder
			// 
			this.mnuAddInRandomOrder.Name = "mnuAddInRandomOrder";
			this.mnuAddInRandomOrder.Size = new System.Drawing.Size(220, 22);
			this.mnuAddInRandomOrder.Text = "Add in Random Order";
			this.mnuAddInRandomOrder.Click += new System.EventHandler(this.mnuAddInRandomOrder_Click);
			// 
			// mnuRandimizeOrder
			// 
			this.mnuRandimizeOrder.Name = "mnuRandimizeOrder";
			this.mnuRandimizeOrder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.mnuRandimizeOrder.Size = new System.Drawing.Size(220, 22);
			this.mnuRandimizeOrder.Text = "Randomize Order Now";
			this.mnuRandimizeOrder.Click += new System.EventHandler(this.mnuRandimizeOrder_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowHelp,
            this.mnuHomepage,
            this.toolStripMenuItem2,
            this.mnuAbout});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// mnuShowHelp
			// 
			this.mnuShowHelp.Name = "mnuShowHelp";
			this.mnuShowHelp.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.mnuShowHelp.Size = new System.Drawing.Size(172, 22);
			this.mnuShowHelp.Text = "View Help";
			this.mnuShowHelp.Click += new System.EventHandler(this.mnuShowHelp_Click);
			// 
			// mnuHomepage
			// 
			this.mnuHomepage.Name = "mnuHomepage";
			this.mnuHomepage.Size = new System.Drawing.Size(172, 22);
			this.mnuHomepage.Text = "PhotoSift Homepage";
			this.mnuHomepage.Click += new System.EventHandler(this.mnuHomepage_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(169, 6);
			// 
			// mnuAbout
			// 
			this.mnuAbout.Name = "mnuAbout";
			this.mnuAbout.Size = new System.Drawing.Size(172, 22);
			this.mnuAbout.Text = "About PhotoSift";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// picLogo
			// 
			this.picLogo.BackColor = System.Drawing.Color.Transparent;
			this.picLogo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
			this.picLogo.Location = new System.Drawing.Point(0, 0);
			this.picLogo.Name = "picLogo";
			this.picLogo.Size = new System.Drawing.Size(472, 292);
			this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.picLogo.TabIndex = 10;
			this.picLogo.TabStop = false;
			this.picLogo.Tag = "";
			// 
			// panelMain
			// 
			this.panelMain.Controls.Add(this.lblHeader);
			this.panelMain.Controls.Add(this.lblStatus);
			this.panelMain.Controls.Add(this.lblInfoLabel);
			this.panelMain.Controls.Add(this.picCurrent);
			this.panelMain.Controls.Add(this.picLogo);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 24);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(472, 292);
			this.panelMain.TabIndex = 11;
			this.panelMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseUp);
			// 
			// timerMouseHider
			// 
			this.timerMouseHider.Interval = 1000;
			this.timerMouseHider.Tick += new System.EventHandler(this.timerMouseHider_Tick);
			// 
			// frmMain
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(472, 316);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.menuStripMain);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStripMain;
			this.MinimumSize = new System.Drawing.Size(160, 200);
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PhotoSift";
			this.Deactivate += new System.EventHandler(this.frmMain_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.Shown += new System.EventHandler(this.frmMain_Shown);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			((System.ComponentModel.ISupportInitialize)(this.picCurrent)).EndInit();
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
			this.panelMain.ResumeLayout(false);
			this.panelMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timerAutoAdvance;
		private System.Windows.Forms.Timer timerHideStatusLabel;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label lblInfoLabel;
		private System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.PictureBox picCurrent;
		private System.Windows.Forms.MenuStrip menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuExit;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuShowHelp;
		private System.Windows.Forms.ToolStripMenuItem mnuAbout;
		private System.Windows.Forms.ToolStripMenuItem mnuHomepage;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem mnuOpenSettings;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem imagePoolToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuAddImages;
		private System.Windows.Forms.ToolStripMenuItem mnuAddFolder;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem mnuClearImages;
		private System.Windows.Forms.ToolStripMenuItem mnuAddInRandomOrder;
		private System.Windows.Forms.ToolStripMenuItem mnuFlipX;
		private System.Windows.Forms.PictureBox picLogo;
		private System.Windows.Forms.ToolStripMenuItem mnuFullscreen;
		private System.Windows.Forms.ToolStripMenuItem mnuSetTargetFolder;
		private System.Windows.Forms.ToolStripMenuItem mnuCopyToClipboard;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.ToolStripMenuItem mnuHideMenu;
		private System.Windows.Forms.ToolStripMenuItem mnuAutoAdvanceEnabled;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem mnuRenameFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem mnuFlipY;
		private System.Windows.Forms.ToolStripMenuItem mnuRotateRight;
		private System.Windows.Forms.ToolStripMenuItem mnuRotateLeft;
		private System.Windows.Forms.Timer timerMouseHider;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigate;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateNext;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigatePrev;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateFirst;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateLast;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateForwardMedium;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateBackMedium;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateForwardLarge;
		private System.Windows.Forms.ToolStripMenuItem mnuNavigateBackLarge;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
		private System.Windows.Forms.ToolStripMenuItem mnuUndo;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem mnuZoomToWidth;
		private System.Windows.Forms.ToolStripMenuItem mnuZoomToHeight;
		private System.Windows.Forms.ToolStripMenuItem mnuTransform;
		private System.Windows.Forms.ToolStripMenuItem mnuZoomIn;
		private System.Windows.Forms.ToolStripMenuItem mnuZoomOut;
		private System.Windows.Forms.ToolStripMenuItem mnuZoom;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
		private System.Windows.Forms.ToolStripMenuItem mnuResetZoom;
		private System.Windows.Forms.ToolStripMenuItem mnuResetViewMode;
		private System.Windows.Forms.ToolStripMenuItem mnuRandimizeOrder;
	}
}

