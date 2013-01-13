using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using GameGUI.Help_Menus;

namespace GameGUI
{
    public partial class InGame : Form
    {
        //reference to a game that can be used to exit the game properly
        public Game GameReference;
        public InGame()
        {
            InitializeComponent();
            //creates a new instance of game...this constructor will probably only be used in testing
            GameReference = new Game();
            //this.TopMost = true;
            //this.TopLevel = true;
        }
        public InGame(Game GameArg)
        {
            InitializeComponent();
            GameReference=GameArg;
            //this.TopMost = true;
        }
        private void InGame_Load(object sender, EventArgs e)
        {

        }

        //button events
        private void ResumeBtn_Click(object sender, EventArgs e)
        {
            //should do the same that the escape key does; destroys this form and continues the game
            this.Dispose();
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            SettingsMenu Open = new SettingsMenu();
            Open.ShowDialog();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            EndGame();
        }
        //ending the game may need to be called elsewhere as well
        public void EndGame()
        {
            //should check if user should save
            WarningMessage Check = new WarningMessage("All unsaved information will be lost.", "Are you sure you want to exit?", WarningMessage.Question);
            //shows as dialog; user has no choice but to enter input
            //destroys the game running after check
            if (Check.ShowDialog() == DialogResult.OK)
            {
                //exits the entire game without saving
                //destroys game
                this.GameReference.Exit();
                //destorys form last
                this.Dispose();
            }
        }
        //just saves the file that is already open (not the first save)
        public void QuickSave()
        {
        }
        //general save method
        public void SaveGame()
        {
            //should be able to tell if the game has ever been saved or not
            //(difference between opening the dialog and not)
            /*
            if()
            {
                QuickSave();
            }
            else
            {
            }*/
            SaveLoadForm Open = new SaveLoadForm();
            Open.ShowDialog();
        }
        //equivalent events for the menu events; may just be able to add the click events above (see initialize)
        private void ResumeMenuItem_Click(object sender, EventArgs e)
        {
            //should do the same that the escape key does; destroys this form and continues the game
            this.Dispose();
        }
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            EndGame();
        }
        //Other seperate setting menus get short-circuted through the upper menus
        private void VideoSettingsItem_Click(object sender, EventArgs e)
        {
            //new instance
            VideoSettings Open = new VideoSettings();
            Open.ShowDialog();
        }

        private void ViewHelpItem_Click(object sender, EventArgs e)
        {

        }
        //about button
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About Open = new About();
            Open.ShowDialog();
        }
        //new save vs quick saving the game; saving to the current file vs. a new game altogether
        private void newSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }
        //saves to current file
        private void quickSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickSave();
        }

        private void LoadFromFileMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }

        //actions for the help menu 
        //-------------------------------------------------------------------------
        //general game information
        private void generalGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralInfo Open = new GeneralInfo();
            Open.ShowDialog();
        }
        //Forge World
        private void editModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForgeInfo Open = new ForgeInfo();
            Open.ShowDialog();
        }
        //about the computer console system
        private void computerConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsoleInfo Open = new ConsoleInfo();
            Open.ShowDialog();
        }
        //holds all the "cheat" or keywords valid in the computer console
        private void consoleKeywordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsoleCheats Open = new ConsoleCheats();
            Open.ShowDialog();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Credits Open = new Credits();
            Open.ShowDialog();
        }

        private void HelpBtn_Click(object sender, EventArgs e)
        {
            MenuHelp Open = new MenuHelp();
            Open.ShowDialog();
        }

    }
}
