namespace GameGUI
{
    partial class InGame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InGame));
            this.ResumeBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResumeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewGameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadFromFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.VideoSettingsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewHelpItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computerConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleKeywordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.HelpBtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ResumeBtn
            // 
            this.ResumeBtn.BackColor = System.Drawing.Color.White;
            this.ResumeBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.ResumeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ResumeBtn.Location = new System.Drawing.Point(29, 158);
            this.ResumeBtn.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.ResumeBtn.Name = "ResumeBtn";
            this.ResumeBtn.Size = new System.Drawing.Size(162, 59);
            this.ResumeBtn.TabIndex = 0;
            this.ResumeBtn.Text = "Resume";
            this.toolTip1.SetToolTip(this.ResumeBtn, "Resumes the game");
            this.ResumeBtn.UseVisualStyleBackColor = false;
            this.ResumeBtn.Click += new System.EventHandler(this.ResumeBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.DarkRed;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(39, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 33);
            this.label1.TabIndex = 1;
            this.label1.Text = "Main Menu";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.SettingsMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ResumeMenuItem,
            this.LoadMenuItem,
            this.SaveMenuItem,
            this.ExitMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileMenuItem.Text = "File";
            // 
            // ResumeMenuItem
            // 
            this.ResumeMenuItem.Name = "ResumeMenuItem";
            this.ResumeMenuItem.Size = new System.Drawing.Size(117, 22);
            this.ResumeMenuItem.Text = "Resume";
            this.ResumeMenuItem.ToolTipText = "Resumes the game";
            this.ResumeMenuItem.Click += new System.EventHandler(this.ResumeMenuItem_Click);
            // 
            // LoadMenuItem
            // 
            this.LoadMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewGameMenuItem,
            this.LoadFromFileMenuItem});
            this.LoadMenuItem.Name = "LoadMenuItem";
            this.LoadMenuItem.Size = new System.Drawing.Size(117, 22);
            this.LoadMenuItem.Text = "Load";
            this.LoadMenuItem.ToolTipText = "Load a previous save";
            // 
            // NewGameMenuItem
            // 
            this.NewGameMenuItem.Name = "NewGameMenuItem";
            this.NewGameMenuItem.Size = new System.Drawing.Size(132, 22);
            this.NewGameMenuItem.Text = "New Game";
            this.NewGameMenuItem.ToolTipText = "Create a new game";
            // 
            // LoadFromFileMenuItem
            // 
            this.LoadFromFileMenuItem.Name = "LoadFromFileMenuItem";
            this.LoadFromFileMenuItem.Size = new System.Drawing.Size(132, 22);
            this.LoadFromFileMenuItem.Text = "From File";
            this.LoadFromFileMenuItem.ToolTipText = "Load a game from file";
            this.LoadFromFileMenuItem.Click += new System.EventHandler(this.LoadFromFileMenuItem_Click);
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.SaveMenuItem.Size = new System.Drawing.Size(117, 22);
            this.SaveMenuItem.Text = "Save";
            this.SaveMenuItem.ToolTipText = "Save current game progress";
            this.SaveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(117, 22);
            this.ExitMenuItem.Text = "Exit";
            this.ExitMenuItem.ToolTipText = "Exit current game";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // SettingsMenuItem
            // 
            this.SettingsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1});
            this.SettingsMenuItem.Name = "SettingsMenuItem";
            this.SettingsMenuItem.Size = new System.Drawing.Size(39, 20);
            this.SettingsMenuItem.Text = "Edit";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VideoSettingsItem});
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem1.Text = "Settings";
            this.settingsToolStripMenuItem1.ToolTipText = "Change game settings";
            // 
            // VideoSettingsItem
            // 
            this.VideoSettingsItem.Name = "VideoSettingsItem";
            this.VideoSettingsItem.Size = new System.Drawing.Size(104, 22);
            this.VideoSettingsItem.Text = "Video";
            this.VideoSettingsItem.Click += new System.EventHandler(this.VideoSettingsItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewHelpItem,
            this.creditsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // ViewHelpItem
            // 
            this.ViewHelpItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalGameToolStripMenuItem,
            this.editModeToolStripMenuItem,
            this.computerConsoleToolStripMenuItem,
            this.consoleKeywordsToolStripMenuItem});
            this.ViewHelpItem.Name = "ViewHelpItem";
            this.ViewHelpItem.Size = new System.Drawing.Size(136, 22);
            this.ViewHelpItem.Text = "Instructions";
            this.ViewHelpItem.ToolTipText = "Click here if you need help";
            this.ViewHelpItem.Click += new System.EventHandler(this.ViewHelpItem_Click);
            // 
            // generalGameToolStripMenuItem
            // 
            this.generalGameToolStripMenuItem.Name = "generalGameToolStripMenuItem";
            this.generalGameToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.generalGameToolStripMenuItem.Text = "General Game";
            this.generalGameToolStripMenuItem.ToolTipText = "General information about the operation of this game";
            this.generalGameToolStripMenuItem.Click += new System.EventHandler(this.generalGameToolStripMenuItem_Click);
            // 
            // editModeToolStripMenuItem
            // 
            this.editModeToolStripMenuItem.Name = "editModeToolStripMenuItem";
            this.editModeToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editModeToolStripMenuItem.Text = "Map Editor (Forge Mode)";
            this.editModeToolStripMenuItem.ToolTipText = "Instructions on how to operate the in-game map editor";
            this.editModeToolStripMenuItem.Click += new System.EventHandler(this.editModeToolStripMenuItem_Click);
            // 
            // computerConsoleToolStripMenuItem
            // 
            this.computerConsoleToolStripMenuItem.Name = "computerConsoleToolStripMenuItem";
            this.computerConsoleToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.computerConsoleToolStripMenuItem.Text = "Computer Console";
            this.computerConsoleToolStripMenuItem.ToolTipText = "Information on how to operate the in-game computer console";
            this.computerConsoleToolStripMenuItem.Click += new System.EventHandler(this.computerConsoleToolStripMenuItem_Click);
            // 
            // consoleKeywordsToolStripMenuItem
            // 
            this.consoleKeywordsToolStripMenuItem.Name = "consoleKeywordsToolStripMenuItem";
            this.consoleKeywordsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.consoleKeywordsToolStripMenuItem.Text = "Console Keywords";
            this.consoleKeywordsToolStripMenuItem.ToolTipText = "List of valid keywords in the in-game console";
            this.consoleKeywordsToolStripMenuItem.Click += new System.EventHandler(this.consoleKeywordsToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.creditsToolStripMenuItem.Text = "Credits";
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // SettingsBtn
            // 
            this.SettingsBtn.BackColor = System.Drawing.Color.White;
            this.SettingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SettingsBtn.Location = new System.Drawing.Point(29, 250);
            this.SettingsBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SettingsBtn.Name = "SettingsBtn";
            this.SettingsBtn.Size = new System.Drawing.Size(162, 59);
            this.SettingsBtn.TabIndex = 3;
            this.SettingsBtn.Text = "Settings";
            this.toolTip1.SetToolTip(this.SettingsBtn, "Change game settings");
            this.SettingsBtn.UseVisualStyleBackColor = false;
            this.SettingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            // 
            // SaveBtn
            // 
            this.SaveBtn.BackColor = System.Drawing.Color.White;
            this.SaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SaveBtn.Location = new System.Drawing.Point(29, 346);
            this.SaveBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(162, 59);
            this.SaveBtn.TabIndex = 4;
            this.SaveBtn.Text = "Save";
            this.toolTip1.SetToolTip(this.SaveBtn, "Save current game progress");
            this.SaveBtn.UseVisualStyleBackColor = false;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.BackColor = System.Drawing.Color.White;
            this.ExitBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ExitBtn.Location = new System.Drawing.Point(29, 513);
            this.ExitBtn.Margin = new System.Windows.Forms.Padding(2);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(162, 59);
            this.ExitBtn.TabIndex = 5;
            this.ExitBtn.Text = "Exit";
            this.toolTip1.SetToolTip(this.ExitBtn, "Exit current game");
            this.ExitBtn.UseVisualStyleBackColor = false;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.Color.DarkRed;
            this.panel1.Controls.Add(this.HelpBtn);
            this.panel1.Controls.Add(this.ResumeBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ExitBtn);
            this.panel1.Controls.Add(this.SettingsBtn);
            this.panel1.Controls.Add(this.SaveBtn);
            this.panel1.Location = new System.Drawing.Point(42, 59);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 635);
            this.panel1.TabIndex = 6;
            // 
            // HelpBtn
            // 
            this.HelpBtn.BackColor = System.Drawing.Color.White;
            this.HelpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.HelpBtn.Location = new System.Drawing.Point(29, 430);
            this.HelpBtn.Margin = new System.Windows.Forms.Padding(2);
            this.HelpBtn.Name = "HelpBtn";
            this.HelpBtn.Size = new System.Drawing.Size(162, 59);
            this.HelpBtn.TabIndex = 8;
            this.HelpBtn.Text = "Help";
            this.toolTip1.SetToolTip(this.HelpBtn, "Click here if you need help");
            this.HelpBtn.UseVisualStyleBackColor = false;
            this.HelpBtn.Click += new System.EventHandler(this.HelpBtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.DarkRed;
            this.pictureBox1.Image = global::GameGUI.Properties.Resources.School_Map001;
            this.pictureBox1.Location = new System.Drawing.Point(436, 86);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(386, 545);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.DarkRed;
            this.panel3.Location = new System.Drawing.Point(413, 59);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(421, 601);
            this.panel3.TabIndex = 9;
            // 
            // InGame
            // 
            this.AcceptButton = this.ResumeBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.DarkBlue;
            this.CancelButton = this.ExitBtn;
            this.ClientSize = new System.Drawing.Size(834, 760);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.MaximizeBox = false;
            this.Name = "InGame";
            this.Opacity = 0.75D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main Menu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.InGame_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ResumeBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button SettingsBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem ResumeMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem VideoSettingsItem;
        private System.Windows.Forms.Button HelpBtn;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewHelpItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewGameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadFromFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computerConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleKeywordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.Panel panel3;

    }
}

