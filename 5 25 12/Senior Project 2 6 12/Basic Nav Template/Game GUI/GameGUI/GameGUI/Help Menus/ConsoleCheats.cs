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
    public partial class ConsoleCheats : Form
    {
        public ConsoleCheats()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ConsoleCheats_Load(object sender, EventArgs e)
        {
            PopulateGrids();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //populate the rows and columns for the grid system in code
        public void PopulateGrids()
        {
            //.exe programs
            dataGridView1.Rows.Add("Guitar_Guy", "Use a Guitar Hero USB controller to play some real riffs", Properties.Resources.Guitar_App_Screen_Shot_Small);
            dataGridView1.Rows.Add("Science_To_Do", "Crazy Eights made in Windows Forms", Properties.Resources.Portal_Card_Game_Small);
            dataGridView1.Rows.Add("GLaDOS", "What Schuyler does in his spare time", Properties.Resources.GLaDOS_Sings_Small);
            dataGridView1.Rows.Add("Thermodynamics", "Simple temperature converter", Properties.Resources.TempConverter);
            dataGridView1.Rows.Add("Maths", "Basic math trivia game", Properties.Resources.MathGame_Screen_Shots_Small);
            dataGridView1.Rows.Add("The_Gatsby", "English project-program about the novel, The Great Gatsby", Properties.Resources.Gatsby_Small);
            dataGridView1.Rows.Add("Aperture_Science", "Early functional calculator", Properties.Resources.Calculator_Small);

            //.Math programs
            dataGridView2.Rows.Add("binary", "Prints out random binary digits", Properties.Resources.Game_Console_Small);
            dataGridView2.Rows.Add("Ran_Num", "Prints out a string of random digits", Properties.Resources.Game_Console_Small);
            dataGridView2.Rows.Add("probs", "Prints out a string of random probabilities", Properties.Resources.Game_Console_Small);
            dataGridView2.Rows.Add("Fibonacci", "Prints out a specified portion of the Fibonacci sequence", Properties.Resources.Game_Console_Small);

            //.cheat programs
            dataGridView3.Rows.Add("No cheats at this time", "Ran out of development time", Properties.Resources.Game_Console_Small);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
