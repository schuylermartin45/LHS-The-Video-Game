using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameGUI
{
    public partial class VideoSettings : Form
    {
        //collections for the resolution drop down box
        private ComboBox.ObjectCollection WideCollection;
        private ComboBox.ObjectCollection FullCollection;
        public VideoSettings()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Infomark;
        }

        private void VideoSettings_Load(object sender, EventArgs e)
        {
            //sets the radio button to whatever the current system is set to 

            //same for the listed screen resolutions
            
        }
        //methods that intialize and adds members to the two collections
        private void FillWideCollection()
        {
            WideCollection = new ComboBox.ObjectCollection(this.ResCntrl);
            WideCollection.Add("1024 x 576");
            WideCollection.Add("1366 x 768");
            WideCollection.Add("1600 x 900");
        }
        private void FillFullCollection()
        {
            FullCollection = new ComboBox.ObjectCollection(this.ResCntrl);
            FullCollection.Add("800 x 600");
            FullCollection.Add("1024 x 768");
            FullCollection.Add("1152 x 864");
        }
        //saves data collected again and then continues to then change the screen resolution when changed
        private void OKBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
