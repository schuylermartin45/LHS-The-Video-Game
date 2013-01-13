namespace DataTypes.ConsoleComputer
{
    partial class Ipod
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ipod));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.PlaySong = new System.Windows.Forms.Button();
            this.GetSongBtn = new System.Windows.Forms.Button();
            this.FileLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // PlaySong
            // 
            this.PlaySong.Location = new System.Drawing.Point(100, 210);
            this.PlaySong.Name = "PlaySong";
            this.PlaySong.Size = new System.Drawing.Size(75, 23);
            this.PlaySong.TabIndex = 0;
            this.PlaySong.Text = "Play";
            this.PlaySong.UseVisualStyleBackColor = true;
            // 
            // GetSongBtn
            // 
            this.GetSongBtn.Location = new System.Drawing.Point(100, 40);
            this.GetSongBtn.Name = "GetSongBtn";
            this.GetSongBtn.Size = new System.Drawing.Size(75, 23);
            this.GetSongBtn.TabIndex = 1;
            this.GetSongBtn.Text = "Find Song";
            this.GetSongBtn.UseVisualStyleBackColor = true;
            this.GetSongBtn.Click += new System.EventHandler(this.GetSongBtn_Click);
            // 
            // FileLbl
            // 
            this.FileLbl.AutoSize = true;
            this.FileLbl.Location = new System.Drawing.Point(115, 144);
            this.FileLbl.Name = "FileLbl";
            this.FileLbl.Size = new System.Drawing.Size(38, 13);
            this.FileLbl.TabIndex = 2;
            this.FileLbl.Text = "[Song]";
            // 
            // Ipod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.FileLbl);
            this.Controls.Add(this.GetSongBtn);
            this.Controls.Add(this.PlaySong);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Ipod";
            this.Text = "Ipod";
            this.Load += new System.EventHandler(this.Ipod_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button PlaySong;
        private System.Windows.Forms.Button GetSongBtn;
        private System.Windows.Forms.Label FileLbl;
    }
}