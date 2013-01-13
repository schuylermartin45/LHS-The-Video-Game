using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using Microsoft.dire

namespace DataTypes.ConsoleComputer
{
    public partial class Ipod : Form
    {
        public Ipod()
        {
            InitializeComponent();
        }

        private void Ipod_Load(object sender, EventArgs e)
        {
            
        }

        private void GetSongBtn_Click(object sender, EventArgs e)
        {
            DialogResult Result = this.openFileDialog1.ShowDialog();

        }


        //methods to play different song types
        public static void PlayWAV(String Location, Boolean Repeat)
        {
            //Declare player as a new SoundPlayer with SoundLocation as the sound location
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Location);
            //If the user has Repeat equal to true
            if (Repeat == true)
            {
                //Play the sound continuously
                player.PlayLooping();
            }
            else
            {
                //Play the sound once
                player.Play();
                System.Media.SystemSound sound = System.Media.SystemSounds.Beep;
                sound.Play();
            }
        }
        public static void PlayMP3(String Location)
        {
            //music = new Microsoft.DirectX.AudioVideoPlayback.Audio(Location);
            //music.Play();
        }

    }
}
