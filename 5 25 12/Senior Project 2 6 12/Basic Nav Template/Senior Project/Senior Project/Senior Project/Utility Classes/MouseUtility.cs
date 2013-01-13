using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//For use of changing icon:
using System.ComponentModel; 
using System.Runtime.InteropServices; 
using System.Reflection; 

namespace Senior_Project
{
    //**********************************************************************
    //A "utility" class that contains a few key mouse/pointer related methods
    //that maybe expanded later to a more sophisticated class
    //
    //**********************************************************************
    class MouseUtility
    {
        //in "tradition" of C#, we will keep variables public in order to access or mutate values

        //True if mouse is a pointer (for menus); 
        public bool PointerState;

        public MouseUtility()
        {
            PointerState = true;
        }
        //changes mouse to reticule state; different icon, stuck in the center screen
        public void ChangeReticule(Microsoft.Xna.Framework.Game GameScreen)
        {
            GameScreen.IsMouseVisible = true;
            //creates a cursor object with the file path of the Reticule Image
            //file must be a .cur (cursor file) to be usable

            //stupid file paths....being stupid...fix this resource problem later

            //System.Windows.Forms.Cursor CustomReticule = new System.Windows.Forms.Cursor(@"K:\12th Grade\Computer Science\Experimentation\Senior Project\ReticuleTest.cur");
            //System.Windows.Forms.Cursor.Current = CustomReticule;

            //
            /* For use of changing icon:
              using System.ComponentModel; using System.Runtime.InteropServices; using System.Reflection; 
             */

            //sets mouse into the center screen 
            //Mouse.SetPosition();
        }
        //changes mouse to regular pointer state
        public void ChangePointer(Microsoft.Xna.Framework.Game GameScreen)
        {
            GameScreen.IsMouseVisible = true;
            //back to a normal cursor
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
    }
}
