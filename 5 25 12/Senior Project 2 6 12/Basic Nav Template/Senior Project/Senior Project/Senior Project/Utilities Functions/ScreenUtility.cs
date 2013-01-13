using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
//projects/solutions used (ambigous as to which)
using GameGUI;
using System.Threading;
using DataTypes;
//Author: Schuyler
namespace Senior_Project
{
    //**********************************************************************
    //A "utility" class that contains a few key screen resizing, layout managing(?) methods
    //that maybe expanded later to a more sophisticated class
    //
    //**********************************************************************
    public class ScreenUtility
    {
        //value to tell if the game is in full screen mode or not
        public bool FullScreen;
        //holds if in wide screen or not
        public bool WideScreen;
        //Holds the old state of the keyboard
        public KeyboardState OldState;
        public GamePadState OldStateBtn;
        private PlayerIndex PlayerIndexValue;
        //bool that holds if the game is paused or not
        public bool GamePaused;
        public SplashScreen SplScreen;
        public ScreenUtility(PlayerIndex PlayerIndexValueArg)
        {
            SplScreen = new SplashScreen(0f);
            FullScreen = true;
            //should be loaded in somehow...to be done later
            WideScreen = true;
            //gets initial keyboard state, will change after first loop of Update()
            OldState = Keyboard.GetState();
            PlayerIndexValue = PlayerIndexValueArg;
            OldStateBtn = GamePad.GetState(PlayerIndexValue);
            //games starts unpaused
            GamePaused = false;
        }
        //displays the splash screen
        public void RunSplashScreen(GraphicsDeviceManager graphics)
        {
            //allows the game to be shown in full screen mode
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            SplScreen = new SplashScreen(1000);
            SplScreen.ShowDialog();
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
        public void RunSplashScreen(int MilliSeconds, GraphicsDeviceManager graphics)
        {
            //allows the game to be shown in full screen mode
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            SplScreen = new SplashScreen(MilliSeconds);
            SplScreen.ShowDialog();
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
        //for use by actual percent of the game loaded
        public void RunSplashScreen(float PercentLoaded, GraphicsDeviceManager graphics)
        {
            //allows the splahs screen to be seen
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            //SplScreen = new SplashScreen(0f);
            SplScreen.Show();
            //updated to current reading, of 0f
            SplScreen.UpdatePercent(0f);
        }
        /*
        public void RunSplashScreenThread(int MilliSeconds)
        {
            //done in a seperate thread from the rest of the game, which is allowed to load simultaneously
            Thread StartSplash = new Thread(new ParameterizedThreadStart();
            StartSplash.Start();

            StartSplash.Suspend();
        }*/
        //sets screen to full screen
        public void SetGameFullScreen(GraphicsDeviceManager graphics)
        {
            FullScreen = true;
            /*
            graphics.IsFullScreen = true;
            graphics.GraphicsDevice.PresentationParameters.IsFullScreen = true;
            graphics.GraphicsDevice.PresentationParameters.BackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;//SystemInformation.PrimaryMonitorSize.Width;
            graphics.GraphicsDevice.PresentationParameters.BackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;//SystemInformation.PrimaryMonitorSize.Height;
            DataTypes.DataValues.ScreenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            DataTypes.DataValues.ScreenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            graphics.ApplyChanges();*/
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = SystemInformation.PrimaryMonitorSize.Height;
            graphics.PreferredBackBufferWidth = SystemInformation.PrimaryMonitorSize.Width;
            //method called to update the graphics
            graphics.ApplyChanges();
        }
        //with resolution controls; probably changed in a dialog at some point
        public void SetGameFullScreen(int ResX, int ResY, GraphicsDeviceManager graphics)
        {
            FullScreen = false;
            graphics.IsFullScreen = false;
            graphics.GraphicsDevice.PresentationParameters.BackBufferWidth = ResX;
            graphics.GraphicsDevice.PresentationParameters.BackBufferHeight = ResY;
            graphics.GraphicsDevice.PresentationParameters.IsFullScreen = true;
            DataTypes.DataValues.ScreenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            DataTypes.DataValues.ScreenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            graphics.ApplyChanges();
        }
        //changes screen to another size
        public void SetGameScreenSize(int X, int Y, GraphicsDeviceManager graphics)
        {
            FullScreen = false;
            graphics.IsFullScreen = false;
            graphics.GraphicsDevice.PresentationParameters.BackBufferWidth = X;
            graphics.GraphicsDevice.PresentationParameters.BackBufferHeight = Y;
            graphics.GraphicsDevice.PresentationParameters.IsFullScreen = false;
            DataTypes.DataValues.ScreenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            DataTypes.DataValues.ScreenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            graphics.ApplyChanges();
        }
        //sets screen to full screen; if it's a form...(menus?)
        public void SetFormFullScreen(Form TheForm)
        {
            //set bool
            FullScreen = true;
            //must disable border first before maximizing to hide the taskbar
            TheForm.FormBorderStyle = FormBorderStyle.None;
            TheForm.WindowState = FormWindowState.Maximized;
            TheForm.TopMost = true;
        }
        public void SetFormScreenSize(int X, int Y, Form TheForm)
        {
            TheForm.Width = X;
            TheForm.Height = Y;
        }
        //simple method to allow the Escape key to act as an exit method;
        //The reason for making all of these "utility" methods in classes is to allow for easy code distribution
        //once the game really gets going
        //****Pre: Must be added in Update() method as it is currently written-> make event later
        public void CreateEscKey(Game GameArg)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                GameArg.Exit();
        }
        //overload that opens up the esc form menu
        public void CreateEscKey(Game GameArg, GraphicsDeviceManager graphics)
        {
            //use of the KeyHasBeenPressed() method prevents the form from opening up millions of instances of the form
            //checks the GamePaused value so that the escape key won't be able interfere with the game; that is now handled by the form
            if ((KeyHasBeenPressed(Microsoft.Xna.Framework.Input.Keys.Escape))&&(GamePaused==false))
            {
                //game is considered to be paused
                GamePaused = true;
                //changing the game out of full screen allows the forms menu to be displayed properly on top
                graphics.IsFullScreen = false;
                //to keep the background to the right size (still full screen), get system's resolution
                graphics.PreferredBackBufferHeight = SystemInformation.PrimaryMonitorSize.Height;
                graphics.PreferredBackBufferWidth = SystemInformation.PrimaryMonitorSize.Width;
                //method called to update the graphics
                graphics.ApplyChanges();
                //sends over the game in order to mess with settings later on
                InGame Open = new InGame(GameArg);
                //Open.TopLevel = true;
                Open.ShowDialog();
                //game is unpaused
                GamePaused = false;
                //sets screen back to normal after the form is destroyed
                graphics.IsFullScreen = true;
                try
                {
                    //null reference problem when game actually closes
                    graphics.ApplyChanges();
                }
                catch (NullReferenceException)
                {
                    //do nothing...as of now
                }
            }
        }
        //same idea, but uses the xbox controller's start button
        public void CreateStartBtn(Game GameArg, GraphicsDeviceManager graphics)
        {
            //use of the KeyHasBeenPressed() method prevents the form from opening up millions of instances of the form
            //checks the GamePaused value so that the escape key won't be able interfere with the game; that is now handled by the form
            if ((ButtonHasBeenPressed(Microsoft.Xna.Framework.Input.Buttons.Start) && (GamePaused == false)))
            {
                //game is considered to be paused
                GamePaused = true;
                //changing the game out of full screen allows the forms menu to be displayed properly on top
                graphics.IsFullScreen = false;
                //to keep the background to the right size (still full screen), get system's resolution
                graphics.PreferredBackBufferHeight = SystemInformation.PrimaryMonitorSize.Height;
                graphics.PreferredBackBufferWidth = SystemInformation.PrimaryMonitorSize.Width;
                //method called to update the graphics
                graphics.ApplyChanges();
                //sends over the game in order to mess with settings latter on
                InGame Open = new InGame(GameArg);
                //Open.TopLevel = true;
                Open.ShowDialog();
                //game is unpaused
                GamePaused = false;
                //sets screen back to normal after the form is destroyed
                graphics.IsFullScreen = true;
                try
                {
                    //null reference problem when game actually closes
                    graphics.ApplyChanges();
                }
                catch (NullReferenceException)
                {
                    //do nothing...as of now
                }
            }
        }
        //method that tells if a key has been pressed between updates; to be put into the Update() method
        //because XNA doesn't support events in the traditional sense
        public bool KeyHasBeenPressed(Microsoft.Xna.Framework.Input.Keys KeyArg)
        {
            KeyboardState NewState = Keyboard.GetState();
            bool KeywasPressed = false;
            // Is the SPACE key down?
            if (NewState.IsKeyDown(KeyArg))
            {
                // If not down last update, key has just been pressed.
                if (!OldState.IsKeyDown(KeyArg))
                {
                    KeywasPressed = true;
                }
            }
            else if (OldState.IsKeyDown(KeyArg))
            {
                // Key was down last update, but not down now, so it has just been released
                KeywasPressed = false;
            }
            // Update saved state of the keyboard
            OldState = NewState;
            //returns the bool, to be used in the program in logic
            return (KeywasPressed);
        }
        //same thing, but for xbox input
        public bool ButtonHasBeenPressed(Microsoft.Xna.Framework.Input.Buttons KeyArg)
        {
            GamePadState NewState = GamePad.GetState(PlayerIndexValue);
            bool KeywasPressed = false;
            // Is the SPACE key down?
            if (NewState.IsButtonDown(KeyArg))
            {
                // If not down last update, key has just been pressed.
                if (!OldStateBtn.IsButtonDown(KeyArg))
                {
                    KeywasPressed = true;
                }
            }
            else if (OldStateBtn.IsButtonDown(KeyArg))
            {
                // Key was down last update, but not down now, so it has just been released
                KeywasPressed = false;
            }
            // Update saved state of the keyboard
            OldStateBtn = NewState;
            //returns the bool, to be used in the program in logic
            return (KeywasPressed);
        }
        //returns the center point of the screen
        public Point FindCenterScreen()
        {
            return(new Point(SystemInformation.PrimaryMonitorSize.Width/2,SystemInformation.PrimaryMonitorSize.Height/2));
        }
    }
}
