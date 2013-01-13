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
    //************************
    //very simple windows form that essentially acts as a customized error dialog
    public partial class WarningMessage : Form
    {
        //constants that represent different warning icons
        public const int Question = 1;
        public const int Exclamation = 2;
        public const int RedX = 3;
        public const int Info = 4;
        //constants the represent the warning header
        public const String Header1 = "Warning!";
        public const String Header2 = "Uh-oh!";
        public const String Header3 = "Are you sure?";
        public const String Header4 = "Continue?";
        public const String Header5 = "Confirm Request";
        //string that holds the warning message entered by the user
        public String MessageArg;
        public WarningMessage(String MessageArg,String MessageHeader,int PicType)
        {
            InitializeComponent();
            //sets values on the form accordingly
            this.Icon = ReturnIcon(PicType);
            //also sets the little picture box to the same icon
            this.PicIconBox.Image = ReturnIcon(PicType).ToBitmap();
            //then sets the labels to the error messages
            this.Header.Text = MessageHeader;
            this.MessageText.Text = MessageArg;
        }

        private void WarningMessage_Load(object sender, EventArgs e)
        {

        }
        private Icon ReturnIcon(int PicType)
        {
            Icon IconImg=null;
            switch (PicType)
            {
                case Question: IconImg = Properties.Resources.Question;
                    break;
                case Exclamation: IconImg = Properties.Resources.Exclamation;
                    break;
                case RedX: IconImg = Properties.Resources.RedX;
                    break;
                case Info: IconImg = Properties.Resources.Infomark;
                    break;
            }
            return(IconImg);
        }
    }
}
