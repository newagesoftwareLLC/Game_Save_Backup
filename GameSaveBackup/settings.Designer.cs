namespace GameSaveBackup
{
    partial class settings
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
            this.steamLabel = new System.Windows.Forms.Label();
            this.autoDL = new System.Windows.Forms.CheckBox();
            this.miniStartup = new System.Windows.Forms.CheckBox();
            this.autoRestore = new System.Windows.Forms.CheckBox();
            this.winstartup = new System.Windows.Forms.CheckBox();
            this.langSelect = new System.Windows.Forms.ComboBox();
            this.langLabel = new System.Windows.Forms.Label();
            this.mbLabel = new System.Windows.Forms.Label();
            this.maxBackups = new System.Windows.Forms.TextBox();
            this.debugExport = new System.Windows.Forms.CheckBox();
            this.steamList = new System.Windows.Forms.ListBox();
            this.doubleClick = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.hideNotifications = new System.Windows.Forms.CheckBox();
            this.gogList = new System.Windows.Forms.ListBox();
            this.gogRemove_btn = new System.Windows.Forms.Button();
            this.gogAdd_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // steamLabel
            // 
            this.steamLabel.AutoSize = true;
            this.steamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.steamLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.steamLabel.Location = new System.Drawing.Point(290, 9);
            this.steamLabel.Name = "steamLabel";
            this.steamLabel.Size = new System.Drawing.Size(183, 17);
            this.steamLabel.TabIndex = 0;
            this.steamLabel.Text = "Steam Content Libraries";
            // 
            // autoDL
            // 
            this.autoDL.AutoSize = true;
            this.autoDL.ForeColor = System.Drawing.SystemColors.Window;
            this.autoDL.Location = new System.Drawing.Point(12, 12);
            this.autoDL.Name = "autoDL";
            this.autoDL.Size = new System.Drawing.Size(264, 17);
            this.autoDL.TabIndex = 2;
            this.autoDL.Text = "Auto back up game save after you close the game";
            this.autoDL.UseVisualStyleBackColor = true;
            // 
            // miniStartup
            // 
            this.miniStartup.AutoSize = true;
            this.miniStartup.ForeColor = System.Drawing.SystemColors.Window;
            this.miniStartup.Location = new System.Drawing.Point(12, 58);
            this.miniStartup.Name = "miniStartup";
            this.miniStartup.Size = new System.Drawing.Size(183, 17);
            this.miniStartup.TabIndex = 4;
            this.miniStartup.Text = "Minimize to system tray on startup";
            this.miniStartup.UseVisualStyleBackColor = true;
            // 
            // autoRestore
            // 
            this.autoRestore.AutoSize = true;
            this.autoRestore.ForeColor = System.Drawing.SystemColors.Window;
            this.autoRestore.Location = new System.Drawing.Point(12, 35);
            this.autoRestore.Name = "autoRestore";
            this.autoRestore.Size = new System.Drawing.Size(220, 17);
            this.autoRestore.TabIndex = 5;
            this.autoRestore.Text = "Auto download and restore on game start";
            this.autoRestore.UseVisualStyleBackColor = true;
            // 
            // winstartup
            // 
            this.winstartup.AutoSize = true;
            this.winstartup.ForeColor = System.Drawing.SystemColors.Window;
            this.winstartup.Location = new System.Drawing.Point(12, 81);
            this.winstartup.Name = "winstartup";
            this.winstartup.Size = new System.Drawing.Size(186, 17);
            this.winstartup.TabIndex = 6;
            this.winstartup.Text = "Start program on Windows startup";
            this.winstartup.UseVisualStyleBackColor = true;
            // 
            // langSelect
            // 
            this.langSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.langSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.langSelect.FormattingEnabled = true;
            this.langSelect.Items.AddRange(new object[] {
            "English"});
            this.langSelect.Location = new System.Drawing.Point(85, 180);
            this.langSelect.Name = "langSelect";
            this.langSelect.Size = new System.Drawing.Size(106, 21);
            this.langSelect.TabIndex = 7;
            // 
            // langLabel
            // 
            this.langLabel.AutoSize = true;
            this.langLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.langLabel.Location = new System.Drawing.Point(26, 183);
            this.langLabel.Name = "langLabel";
            this.langLabel.Size = new System.Drawing.Size(55, 13);
            this.langLabel.TabIndex = 8;
            this.langLabel.Text = "Language";
            // 
            // mbLabel
            // 
            this.mbLabel.AutoSize = true;
            this.mbLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.mbLabel.Location = new System.Drawing.Point(9, 208);
            this.mbLabel.Name = "mbLabel";
            this.mbLabel.Size = new System.Drawing.Size(72, 13);
            this.mbLabel.TabIndex = 9;
            this.mbLabel.Text = "Max Backups";
            // 
            // maxBackups
            // 
            this.maxBackups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxBackups.Location = new System.Drawing.Point(84, 205);
            this.maxBackups.Name = "maxBackups";
            this.maxBackups.Size = new System.Drawing.Size(42, 20);
            this.maxBackups.TabIndex = 10;
            this.maxBackups.TextChanged += new System.EventHandler(this.maxBackups_TextChanged);
            this.maxBackups.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // debugExport
            // 
            this.debugExport.AutoSize = true;
            this.debugExport.ForeColor = System.Drawing.SystemColors.Window;
            this.debugExport.Location = new System.Drawing.Point(12, 104);
            this.debugExport.Name = "debugExport";
            this.debugExport.Size = new System.Drawing.Size(181, 17);
            this.debugExport.TabIndex = 11;
            this.debugExport.Text = "Export debug console data to file";
            this.debugExport.UseVisualStyleBackColor = true;
            // 
            // steamList
            // 
            this.steamList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.steamList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(106)))));
            this.steamList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.steamList.ForeColor = System.Drawing.Color.White;
            this.steamList.FormattingEnabled = true;
            this.steamList.Location = new System.Drawing.Point(291, 28);
            this.steamList.Name = "steamList";
            this.steamList.Size = new System.Drawing.Size(300, 197);
            this.steamList.TabIndex = 12;
            // 
            // doubleClick
            // 
            this.doubleClick.AutoSize = true;
            this.doubleClick.ForeColor = System.Drawing.SystemColors.Window;
            this.doubleClick.Location = new System.Drawing.Point(12, 127);
            this.doubleClick.Name = "doubleClick";
            this.doubleClick.Size = new System.Drawing.Size(163, 17);
            this.doubleClick.TabIndex = 13;
            this.doubleClick.Text = "Double click on game to play";
            this.doubleClick.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label1.Location = new System.Drawing.Point(596, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "GOG Content Library";
            // 
            // hideNotifications
            // 
            this.hideNotifications.AutoSize = true;
            this.hideNotifications.ForeColor = System.Drawing.SystemColors.Window;
            this.hideNotifications.Location = new System.Drawing.Point(12, 150);
            this.hideNotifications.Name = "hideNotifications";
            this.hideNotifications.Size = new System.Drawing.Size(109, 17);
            this.hideNotifications.TabIndex = 16;
            this.hideNotifications.Text = "Hide Notifications";
            this.hideNotifications.UseVisualStyleBackColor = true;
            // 
            // gogList
            // 
            this.gogList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gogList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(106)))));
            this.gogList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gogList.ForeColor = System.Drawing.Color.White;
            this.gogList.FormattingEnabled = true;
            this.gogList.Location = new System.Drawing.Point(597, 28);
            this.gogList.Name = "gogList";
            this.gogList.Size = new System.Drawing.Size(300, 158);
            this.gogList.TabIndex = 17;
            // 
            // gogRemove_btn
            // 
            this.gogRemove_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gogRemove_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(115)))));
            this.gogRemove_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gogRemove_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gogRemove_btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.gogRemove_btn.Location = new System.Drawing.Point(598, 194);
            this.gogRemove_btn.Name = "gogRemove_btn";
            this.gogRemove_btn.Size = new System.Drawing.Size(148, 33);
            this.gogRemove_btn.TabIndex = 18;
            this.gogRemove_btn.Text = "Remove";
            this.gogRemove_btn.UseVisualStyleBackColor = false;
            this.gogRemove_btn.Click += new System.EventHandler(this.gogRemove_btn_Click);
            // 
            // gogAdd_btn
            // 
            this.gogAdd_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gogAdd_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(115)))));
            this.gogAdd_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gogAdd_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gogAdd_btn.ForeColor = System.Drawing.Color.LimeGreen;
            this.gogAdd_btn.Location = new System.Drawing.Point(752, 194);
            this.gogAdd_btn.Name = "gogAdd_btn";
            this.gogAdd_btn.Size = new System.Drawing.Size(146, 33);
            this.gogAdd_btn.TabIndex = 19;
            this.gogAdd_btn.Text = "Add";
            this.gogAdd_btn.UseVisualStyleBackColor = false;
            this.gogAdd_btn.Click += new System.EventHandler(this.gogAdd_btn_Click);
            // 
            // settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(83)))), ((int)(((byte)(109)))));
            this.ClientSize = new System.Drawing.Size(905, 236);
            this.Controls.Add(this.gogAdd_btn);
            this.Controls.Add(this.gogRemove_btn);
            this.Controls.Add(this.gogList);
            this.Controls.Add(this.hideNotifications);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.doubleClick);
            this.Controls.Add(this.steamList);
            this.Controls.Add(this.debugExport);
            this.Controls.Add(this.maxBackups);
            this.Controls.Add(this.mbLabel);
            this.Controls.Add(this.langLabel);
            this.Controls.Add(this.langSelect);
            this.Controls.Add(this.winstartup);
            this.Controls.Add(this.autoRestore);
            this.Controls.Add(this.miniStartup);
            this.Controls.Add(this.autoDL);
            this.Controls.Add(this.steamLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "settings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Application Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formClosing);
            this.Load += new System.EventHandler(this.settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label steamLabel;
        private System.Windows.Forms.CheckBox autoDL;
        private System.Windows.Forms.CheckBox miniStartup;
        private System.Windows.Forms.CheckBox autoRestore;
        private System.Windows.Forms.CheckBox winstartup;
        private System.Windows.Forms.ComboBox langSelect;
        private System.Windows.Forms.Label langLabel;
        private System.Windows.Forms.Label mbLabel;
        private System.Windows.Forms.TextBox maxBackups;
        private System.Windows.Forms.CheckBox debugExport;
        private System.Windows.Forms.ListBox steamList;
        private System.Windows.Forms.CheckBox doubleClick;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox hideNotifications;
        private System.Windows.Forms.ListBox gogList;
        private System.Windows.Forms.Button gogRemove_btn;
        private System.Windows.Forms.Button gogAdd_btn;
    }
}