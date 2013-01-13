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
    public partial class SplashScreen : Form
    {
        //----------------------------------------------------------------------------------
        //alternative method of loading with real percentages of loadingness
        public SplashScreen(float PercentLoaded)
        {
            InitializeComponent();
        }
        //update method that changes the load bar based on value determined in the game
        public void UpdatePercent(float PercentLoaded)
        {
            //redudant check that prevents the percent from being >100
            if (((int)(PercentLoaded * 100)) < 100)
            {
                this.progressBar1.Value = (int)(PercentLoaded * 100);
                this.PercentLoadLbl.Text = this.progressBar1.Value.ToString() + "%";
            }
            else
            {
                this.progressBar1.Value = 100;
                this.PercentLoadLbl.Text = this.progressBar1.Value.ToString() + "%";
            }
            this.Refresh();
        }
        //----------------------------------------------------------------------------------
        public SplashScreen(int MilliSeconds)
        {
            InitializeComponent();
            this.timer1.Interval = MilliSeconds;
            //starts timer
            this.timer1.Start();
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
        }
        //basic loading bar; timer fires event every X milliseconds
        private void timer1_Tick(object sender, EventArgs e)
        {
            Random RandomPercent = new Random();
            //increments percent value; random increments makes it appear more exciting-> like it's actually doing something
            int RanTemp = RandomPercent.Next(1, 5);
            if (RanTemp + this.progressBar1.Value < 100)
            {
                this.progressBar1.Value += RanTemp;
            }
            else
            {
                this.progressBar1.Value++;
            }
            //changes percent shown, when load bar changes
            this.PercentLoadLbl.Text = this.progressBar1.Value.ToString() + "%";
            //ends the timer; deletes frame
            if (this.progressBar1.Value >= 100)
            {
                this.timer1.Stop();
                this.Dispose();
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
