using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameGUI.Help_Menus
{
    public partial class MenuHelp : Form
    {
        public MenuHelp()
        {
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GeneralInfo Open = new GeneralInfo();
            Open.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConsoleCheats Open = new ConsoleCheats();
            Open.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ForgeInfo Open = new ForgeInfo();
            Open.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Credits Open = new Credits();
            Open.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            About Open = new About();
            Open.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConsoleInfo Open = new ConsoleInfo();
            Open.ShowDialog();
        }

        private void MenuHelp_Load(object sender, EventArgs e)
        {

        }
    }
}
