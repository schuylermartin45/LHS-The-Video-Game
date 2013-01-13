using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GameGUI
{
    public partial class SaveLoadForm : Form
    {
        private Thread NewThread;
        public String Path="";
        public SaveLoadForm()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Infomark;
            NewThread = new Thread(new ThreadStart(CallFolder));
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            //this thread state must be set to prevent a thread exception!!!!!!!!!!!!!
            this.Hide();
            NewThread.SetApartmentState(ApartmentState.STA);
            NewThread.Start();
        }
        private void CallFolder()
        {
            //opens the folder browser-> then sets the string in the path text box
            DialogResult Result = this.folderBrowserDialog1.ShowDialog();
            if (Result == DialogResult.OK)
            {
                //gets data from the box; string path
                Path=this.folderBrowserDialog1.SelectedPath;
                //sets the path name to the text box control
                this.FilePathTxtBx.Text = Path;
            }
            //kills thread
            NewThread.Suspend();
            NewThread = null;
            this.ShowDialog();
        }
        //saves the game
        private void SaveBtn_Click(object sender, EventArgs e)
        {

        }
        //loads a new game
        private void LoadBtn_Click(object sender, EventArgs e)
        {

        }

        private void SaveLoadForm_Load(object sender, EventArgs e)
        {

        }

    }
}
