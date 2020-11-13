namespace GameSaveBackup
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.outBox = new System.Windows.Forms.RichTextBox();
            this.localGameList = new System.Windows.Forms.ListView();
            this.uploadBtn = new System.Windows.Forms.Button();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.cloudFiles = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitNewagesoldiercomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshGames_btn = new System.Windows.Forms.Button();
            this.GameDetection_timer = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.processIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadReload = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.noficationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showHideApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supportedSteamGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleDriveBackupCopiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localGamesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchGameexeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGameDirectoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetupCloudDrives_bgWorker = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerList = new System.ComponentModel.BackgroundWorker();
            this.newGameDetection = new System.Windows.Forms.Timer(this.components);
            this.minimizeButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.aboutButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.settings_button = new System.Windows.Forms.Button();
            this.cloudsources_button = new System.Windows.Forms.Button();
            this.supportedgames_button = new System.Windows.Forms.Button();
            this.xmlDownloader = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.noficationMenu.SuspendLayout();
            this.localGamesContextMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // outBox
            // 
            this.outBox.BackColor = System.Drawing.Color.LightSlateGray;
            this.outBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outBox.ForeColor = System.Drawing.Color.White;
            this.outBox.Location = new System.Drawing.Point(13, 565);
            this.outBox.Name = "outBox";
            this.outBox.ReadOnly = true;
            this.outBox.Size = new System.Drawing.Size(990, 166);
            this.outBox.TabIndex = 0;
            this.outBox.Text = "";
            // 
            // localGameList
            // 
            this.localGameList.AllowColumnReorder = true;
            this.localGameList.BackColor = System.Drawing.Color.LightSlateGray;
            this.localGameList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.localGameList.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.localGameList.ForeColor = System.Drawing.Color.White;
            this.localGameList.FullRowSelect = true;
            this.localGameList.GridLines = true;
            this.localGameList.Location = new System.Drawing.Point(6, 19);
            this.localGameList.Name = "localGameList";
            this.localGameList.Size = new System.Drawing.Size(713, 413);
            this.localGameList.TabIndex = 1;
            this.localGameList.TileSize = new System.Drawing.Size(236, 121);
            this.localGameList.UseCompatibleStateImageBehavior = false;
            this.localGameList.View = System.Windows.Forms.View.Tile;
            this.localGameList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.localGameList_ItemSelectionChanged);
            this.localGameList.SelectedIndexChanged += new System.EventHandler(this.localGameList_SelectedIndexChanged);
            this.localGameList.DoubleClick += new System.EventHandler(this.localGame_DoubleClick);
            this.localGameList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.localGame_MouseClick);
            // 
            // uploadBtn
            // 
            this.uploadBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(109)))));
            this.uploadBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uploadBtn.Enabled = false;
            this.uploadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uploadBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uploadBtn.ForeColor = System.Drawing.Color.Turquoise;
            this.uploadBtn.Location = new System.Drawing.Point(43, 438);
            this.uploadBtn.Name = "uploadBtn";
            this.uploadBtn.Size = new System.Drawing.Size(676, 27);
            this.uploadBtn.TabIndex = 2;
            this.uploadBtn.Text = "UPLOAD / BACKUP";
            this.uploadBtn.UseVisualStyleBackColor = false;
            this.uploadBtn.Click += new System.EventHandler(this.uploadBtn_Click);
            // 
            // downloadBtn
            // 
            this.downloadBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(109)))));
            this.downloadBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.downloadBtn.Enabled = false;
            this.downloadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadBtn.ForeColor = System.Drawing.Color.LimeGreen;
            this.downloadBtn.Location = new System.Drawing.Point(44, 438);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(213, 27);
            this.downloadBtn.TabIndex = 3;
            this.downloadBtn.Text = "DOWNLOAD / RESTORE";
            this.downloadBtn.UseVisualStyleBackColor = false;
            this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // cloudFiles
            // 
            this.cloudFiles.BackColor = System.Drawing.Color.LightSlateGray;
            this.cloudFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cloudFiles.Cursor = System.Windows.Forms.Cursors.Default;
            this.cloudFiles.ForeColor = System.Drawing.Color.White;
            this.cloudFiles.FullRowSelect = true;
            this.cloudFiles.GridLines = true;
            this.cloudFiles.Location = new System.Drawing.Point(6, 19);
            this.cloudFiles.Name = "cloudFiles";
            this.cloudFiles.Size = new System.Drawing.Size(251, 413);
            this.cloudFiles.TabIndex = 4;
            this.cloudFiles.UseCompatibleStateImageBehavior = false;
            this.cloudFiles.View = System.Windows.Forms.View.List;
            this.cloudFiles.SelectedIndexChanged += new System.EventHandler(this.cloudFiles_SelectedIndexChanged);
            this.cloudFiles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1017, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationInformationToolStripMenuItem,
            this.visitNewagesoldiercomToolStripMenuItem});
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(135, 20);
            this.aboutToolStripMenuItem.Text = "Game Save Backup";
            // 
            // applicationInformationToolStripMenuItem
            // 
            this.applicationInformationToolStripMenuItem.Name = "applicationInformationToolStripMenuItem";
            this.applicationInformationToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.applicationInformationToolStripMenuItem.Text = "Application Information";
            this.applicationInformationToolStripMenuItem.Click += new System.EventHandler(this.applicationInformationToolStripMenuItem_Click);
            // 
            // visitNewagesoldiercomToolStripMenuItem
            // 
            this.visitNewagesoldiercomToolStripMenuItem.Name = "visitNewagesoldiercomToolStripMenuItem";
            this.visitNewagesoldiercomToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.visitNewagesoldiercomToolStripMenuItem.Text = "Visit Website";
            this.visitNewagesoldiercomToolStripMenuItem.Click += new System.EventHandler(this.visitNewagesoldiercomToolStripMenuItem_Click);
            // 
            // refreshGames_btn
            // 
            this.refreshGames_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(109)))));
            this.refreshGames_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refreshGames_btn.Enabled = false;
            this.refreshGames_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshGames_btn.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.refreshGames_btn.ForeColor = System.Drawing.Color.Aquamarine;
            this.refreshGames_btn.Location = new System.Drawing.Point(6, 438);
            this.refreshGames_btn.Name = "refreshGames_btn";
            this.refreshGames_btn.Size = new System.Drawing.Size(31, 27);
            this.refreshGames_btn.TabIndex = 7;
            this.refreshGames_btn.Text = "q";
            this.refreshGames_btn.UseVisualStyleBackColor = false;
            this.refreshGames_btn.Click += new System.EventHandler(this.button1_Click);
            // 
            // GameDetection_timer
            // 
            this.GameDetection_timer.Tick += new System.EventHandler(this.GameDetection_timer_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processIDToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(164, 26);
            // 
            // processIDToolStripMenuItem
            // 
            this.processIDToolStripMenuItem.Name = "processIDToolStripMenuItem";
            this.processIDToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.processIDToolStripMenuItem.Text = "Delete Cloud File";
            this.processIDToolStripMenuItem.Click += new System.EventHandler(this.processIDToolStripMenuItem_Click);
            // 
            // downloadReload
            // 
            this.downloadReload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(109)))));
            this.downloadReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.downloadReload.Enabled = false;
            this.downloadReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadReload.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.downloadReload.ForeColor = System.Drawing.Color.LimeGreen;
            this.downloadReload.Location = new System.Drawing.Point(6, 438);
            this.downloadReload.Name = "downloadReload";
            this.downloadReload.Size = new System.Drawing.Size(31, 27);
            this.downloadReload.TabIndex = 8;
            this.downloadReload.Text = "q";
            this.downloadReload.UseVisualStyleBackColor = false;
            this.downloadReload.Click += new System.EventHandler(this.downloadReload_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Steam Save Backup in System Tray";
            this.notifyIcon1.ContextMenuStrip = this.noficationMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Steam Save Backup";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notify_DoubleClick);
            // 
            // noficationMenu
            // 
            this.noficationMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHideApplicationToolStripMenuItem,
            this.supportedSteamGamesToolStripMenuItem,
            this.googleDriveBackupCopiesToolStripMenuItem});
            this.noficationMenu.Name = "noficationMenu";
            this.noficationMenu.Size = new System.Drawing.Size(198, 70);
            // 
            // showHideApplicationToolStripMenuItem
            // 
            this.showHideApplicationToolStripMenuItem.Name = "showHideApplicationToolStripMenuItem";
            this.showHideApplicationToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.showHideApplicationToolStripMenuItem.Text = "Show/Hide Application";
            this.showHideApplicationToolStripMenuItem.Click += new System.EventHandler(this.showHideApplicationToolStripMenuItem_Click);
            // 
            // supportedSteamGamesToolStripMenuItem
            // 
            this.supportedSteamGamesToolStripMenuItem.Name = "supportedSteamGamesToolStripMenuItem";
            this.supportedSteamGamesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.supportedSteamGamesToolStripMenuItem.Text = "Supported Games";
            this.supportedSteamGamesToolStripMenuItem.Click += new System.EventHandler(this.supportedSteamGamesToolStripMenuItem_Click);
            // 
            // googleDriveBackupCopiesToolStripMenuItem
            // 
            this.googleDriveBackupCopiesToolStripMenuItem.Name = "googleDriveBackupCopiesToolStripMenuItem";
            this.googleDriveBackupCopiesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.googleDriveBackupCopiesToolStripMenuItem.Text = "Close Application";
            this.googleDriveBackupCopiesToolStripMenuItem.Click += new System.EventHandler(this.googleDriveBackupCopiesToolStripMenuItem_Click);
            // 
            // localGamesContextMenuStrip
            // 
            this.localGamesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playGameToolStripMenuItem,
            this.launchGameexeToolStripMenuItem,
            this.openGameDirectoryMenuItem});
            this.localGamesContextMenuStrip.Name = "localGamesContextMenuStrip";
            this.localGamesContextMenuStrip.Size = new System.Drawing.Size(189, 70);
            // 
            // playGameToolStripMenuItem
            // 
            this.playGameToolStripMenuItem.Name = "playGameToolStripMenuItem";
            this.playGameToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.playGameToolStripMenuItem.Text = "Launch From Steam";
            this.playGameToolStripMenuItem.Click += new System.EventHandler(this.playGameToolStripMenuItem_Click);
            // 
            // launchGameexeToolStripMenuItem
            // 
            this.launchGameexeToolStripMenuItem.Name = "launchGameexeToolStripMenuItem";
            this.launchGameexeToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.launchGameexeToolStripMenuItem.Text = "Launch Game (exe)";
            this.launchGameexeToolStripMenuItem.Click += new System.EventHandler(this.launchGameexeToolStripMenuItem_Click);
            // 
            // openGameDirectoryMenuItem
            // 
            this.openGameDirectoryMenuItem.Name = "openGameDirectoryMenuItem";
            this.openGameDirectoryMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openGameDirectoryMenuItem.Text = "Open Game Directory";
            this.openGameDirectoryMenuItem.Click += new System.EventHandler(this.openGameDirectoryMenuItem_Click);
            // 
            // SetupCloudDrives_bgWorker
            // 
            this.SetupCloudDrives_bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SetupCloudDrives_bgWorker_DoWork);
            this.SetupCloudDrives_bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SetupCloudDrives_bgWorker_Completed);
            // 
            // backgroundWorkerList
            // 
            this.backgroundWorkerList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerList_DoWork);
            // 
            // newGameDetection
            // 
            this.newGameDetection.Enabled = true;
            this.newGameDetection.Interval = 1000;
            this.newGameDetection.Tick += new System.EventHandler(this.newGameDetection_Tick_1);
            // 
            // minimizeButton
            // 
            this.minimizeButton.AccessibleDescription = "Minimize Trainer Manager";
            this.minimizeButton.AccessibleName = "Minimize Button";
            this.minimizeButton.BackColor = System.Drawing.Color.LightSlateGray;
            this.minimizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeButton.ForeColor = System.Drawing.Color.Lavender;
            this.minimizeButton.Location = new System.Drawing.Point(976, 0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(21, 24);
            this.minimizeButton.TabIndex = 15;
            this.minimizeButton.Tag = "";
            this.minimizeButton.Text = "_";
            this.minimizeButton.UseVisualStyleBackColor = false;
            this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.AccessibleDescription = "Close Trainer Manager";
            this.closeButton.AccessibleName = "Close Button";
            this.closeButton.BackColor = System.Drawing.Color.RosyBrown;
            this.closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ForeColor = System.Drawing.Color.Transparent;
            this.closeButton.Location = new System.Drawing.Point(996, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(21, 24);
            this.closeButton.TabIndex = 14;
            this.closeButton.Text = "X";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.AccessibleDescription = "About Trainer Manager";
            this.aboutButton.AccessibleName = "About Button";
            this.aboutButton.BackColor = System.Drawing.Color.DarkKhaki;
            this.aboutButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.aboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutButton.ForeColor = System.Drawing.Color.Lavender;
            this.aboutButton.Location = new System.Drawing.Point(956, 0);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(21, 24);
            this.aboutButton.TabIndex = 13;
            this.aboutButton.Text = "?";
            this.aboutButton.UseVisualStyleBackColor = false;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel2.Location = new System.Drawing.Point(142, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(812, 23);
            this.panel2.TabIndex = 17;
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_MouseMove);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.localGameList);
            this.groupBox1.Controls.Add(this.uploadBtn);
            this.groupBox1.Controls.Add(this.refreshGames_btn);
            this.groupBox1.Location = new System.Drawing.Point(12, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(725, 471);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Games Installed On This PC";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cloudFiles);
            this.groupBox2.Controls.Add(this.downloadBtn);
            this.groupBox2.Controls.Add(this.downloadReload);
            this.groupBox2.Location = new System.Drawing.Point(743, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(262, 471);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cloud Service Backed Up Copies";
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox5.BackgroundImage")));
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox5.InitialImage")));
            this.pictureBox5.Location = new System.Drawing.Point(737, 31);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(268, 52);
            this.pictureBox5.TabIndex = 24;
            this.pictureBox5.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(231, 119);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(160, 87);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(676, 627);
            this.webBrowser1.TabIndex = 26;
            this.webBrowser1.Visible = false;
            this.webBrowser1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.completed);
            // 
            // settings_button
            // 
            this.settings_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.settings_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings_button.Font = new System.Drawing.Font("Webdings", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.settings_button.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.settings_button.Location = new System.Drawing.Point(13, 33);
            this.settings_button.Name = "settings_button";
            this.settings_button.Size = new System.Drawing.Size(54, 48);
            this.settings_button.TabIndex = 27;
            this.settings_button.Text = "@";
            this.settings_button.UseVisualStyleBackColor = true;
            this.settings_button.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // cloudsources_button
            // 
            this.cloudsources_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cloudsources_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cloudsources_button.Font = new System.Drawing.Font("Webdings", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.cloudsources_button.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.cloudsources_button.Location = new System.Drawing.Point(133, 33);
            this.cloudsources_button.Name = "cloudsources_button";
            this.cloudsources_button.Size = new System.Drawing.Size(54, 48);
            this.cloudsources_button.TabIndex = 28;
            this.cloudsources_button.Text = "e";
            this.cloudsources_button.UseVisualStyleBackColor = true;
            this.cloudsources_button.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // supportedgames_button
            // 
            this.supportedgames_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.supportedgames_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.supportedgames_button.Font = new System.Drawing.Font("Webdings", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.supportedgames_button.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.supportedgames_button.Location = new System.Drawing.Point(73, 33);
            this.supportedgames_button.Name = "supportedgames_button";
            this.supportedgames_button.Size = new System.Drawing.Size(54, 48);
            this.supportedgames_button.TabIndex = 29;
            this.supportedgames_button.Text = "i";
            this.supportedgames_button.UseVisualStyleBackColor = true;
            this.supportedgames_button.Click += new System.EventHandler(this.button2_Click);
            // 
            // xmlDownloader
            // 
            this.xmlDownloader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.xmlDownloader_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(109)))));
            this.ClientSize = new System.Drawing.Size(1017, 747);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.supportedgames_button);
            this.Controls.Add(this.cloudsources_button);
            this.Controls.Add(this.settings_button);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.minimizeButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.outBox);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game Save Backup";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.frmMain_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.noficationMenu.ResumeLayout(false);
            this.localGamesContextMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView localGameList;
        private System.Windows.Forms.Button uploadBtn;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.ListView cloudFiles;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button refreshGames_btn;
        private System.Windows.Forms.Timer GameDetection_timer;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem processIDToolStripMenuItem;
        private System.Windows.Forms.Button downloadReload;
        private System.Windows.Forms.ToolStripMenuItem applicationInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visitNewagesoldiercomToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip localGamesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playGameToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker SetupCloudDrives_bgWorker;
        private System.ComponentModel.BackgroundWorker backgroundWorkerList;
        private System.Windows.Forms.Timer newGameDetection;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.ContextMenuStrip noficationMenu;
        private System.Windows.Forms.ToolStripMenuItem showHideApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supportedSteamGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem googleDriveBackupCopiesToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.RichTextBox outBox;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button settings_button;
        private System.Windows.Forms.Button cloudsources_button;
        private System.Windows.Forms.Button supportedgames_button;
        private System.ComponentModel.BackgroundWorker xmlDownloader;
        private System.Windows.Forms.ToolStripMenuItem openGameDirectoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchGameexeToolStripMenuItem;
    }
}

