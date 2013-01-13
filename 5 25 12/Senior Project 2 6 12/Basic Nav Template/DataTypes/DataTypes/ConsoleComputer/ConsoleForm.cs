using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DataTypes
{
    public partial class ConsoleForm : Form
    {
        //enum representing what kind of form is to be created
        //last mode, called ForgeMode, turns the console into a system that can be used to edit the in-game objects
        public enum ConsoleApp { MainMenu = 0, RanNum, RanProb, RanBinary, Fibinacci, ForgeMode };
        public ConsoleApp ConsoleType;
        public String ContentDirectory;

        //return type to the 
        public object ForgeModeOut = null;

        //the console is currently up...only applicable to forge world
        public bool ConsoleUp = false;

        //max output of "items" for the mini math programs
        public int MAXOUT = 1500;
        public ConsoleForm(ConsoleApp TypeArg)
        {
            InitializeComponent();
            //beeps when created
            Console.Beep();
            ConsoleType = TypeArg;
            //works backwords from the executable to find the directory of the exectuables (found with the other content)
            ContentDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString()).ToString();
            UpdatePrompts();
        }

        //other constructor, used exclusively for editing in forge mode

        //sets the console up for forge world use
        public ConsoleForm()
        {
            InitializeComponent();
            this.ConsoleType = ConsoleApp.ForgeMode;
            this.Opacity = 0.93;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            UpdatePrompts();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ConsoleUp = false;
            this.Hide();//Dispose();
        }

        //default method/menu to run all other apps
        public void MainMenuSys()
        {
            //clear last output after the next bit was entered
            this.textBox2.Clear();
            String ReadLine = this.textBox1.Text;
            //waits for user input
            //this.ProgramPrompt("Console_Input: ");
            //ReadLine = this.ReadLine();
            //checks the command, then does appropriate code
            if (ReadLine == ProjectKeywords.GuitarApp + ProjectKeywords.Executable)
            {
                this.RunExecutable(@"GuitarAppDestribution\" + ProjectKeywords.GuitarExe + ProjectKeywords.Executable);
                //reset console when this process is dead
                EndApp();
            }
            if (ReadLine == ProjectKeywords.GLaDOSSings + ProjectKeywords.Executable)
            {
                this.RunExecutable(ProjectKeywords.GLaDOSExe + ProjectKeywords.Executable);
                //reset console when this process is dead
                EndApp();
            }
            if (ReadLine == ProjectKeywords.TemperatureConverter + ProjectKeywords.Executable)
            {
                this.RunExecutable(ProjectKeywords.TemperatureConverterExe + ProjectKeywords.Executable);
                EndApp();
            }
            if (ReadLine == ProjectKeywords.PortalCardGame + ProjectKeywords.Executable)
            {
                this.RunExecutable(@"MyCardGame\GameFiles\" + ProjectKeywords.PortalCardGameExe + ProjectKeywords.Executable);
                EndApp();
            }
            if (ReadLine == ProjectKeywords.MathTrivia + ProjectKeywords.Executable)
            {
                this.RunExecutable(ProjectKeywords.MathTriviaExe + ProjectKeywords.Executable);
                EndApp();
            }
            if (ReadLine == ProjectKeywords.Calculator + ProjectKeywords.Executable)
            {
                this.RunExecutable(ProjectKeywords.CalculatorExe + ProjectKeywords.Executable);
                EndApp();
            }
            if (ReadLine == ProjectKeywords.GreatGatsby + ProjectKeywords.Executable)
            {
                this.RunExecutable(ProjectKeywords.GreatGatsbyExe + ProjectKeywords.Executable);
                EndApp();
            }

            //simple math functions..homage to our first/earliest programs
            if (ReadLine == ProjectKeywords.RandomNums + ProjectKeywords.MathExe)
            {
                this.ConsoleType = ConsoleApp.RanNum;
                this.UpdatePrompts();
            }
            if (ReadLine == ProjectKeywords.RanProbability + ProjectKeywords.MathExe)
            {
                //creates a new Console window over the main menu
                this.ConsoleType = ConsoleApp.RanProb;
                this.UpdatePrompts();
            }
            if (ReadLine == ProjectKeywords.Binary + ProjectKeywords.MathExe)
            {
                this.ConsoleType = ConsoleApp.RanBinary;
                this.UpdatePrompts();
            }
            if (ReadLine == ProjectKeywords.FibonacciSeq + ProjectKeywords.MathExe)
            {
                this.ConsoleType = ConsoleApp.Fibinacci;
                this.UpdatePrompts();
            }
            //help menu/exit stuff
            if(ReadLine == "Help")
            {
                this.ProgramOutput("Type 'Exit' to leave any console application.");
                this.ProgramOutput(Environment.NewLine+"Otherwise: Please contact our benefactor, the network administrator.");
            }
            if(ReadLine == "Exit")
            {
                Console.Beep();
                this.ProgramOutput(Environment.NewLine+"Good-Bye!");
                Thread.Sleep(500);
                this.Dispose();
                //Environment.Exit(0);
            }
            //clear for the next entry
            this.textBox1.Clear();
        }

        //*************************************************************************
        //similar to the main menu loop, except used only for the forgemode console
        public void ForgeMenuSys()
        {
            //clear last output after the next bit was entered
            this.textBox2.Clear();
            String ReadLine = this.textBox1.Text;
            //available return types..returns to the main game
            //easier to convert to an integer than it is the other way around...
            float FloatOut = 0f;
            //help menu/exit stuff
            if (ReadLine == "Help")
            {
                this.ProgramOutput("Type 'Exit' to leave any console application.");
                this.ProgramOutput(Environment.NewLine + "Otherwise: Please contact our benefactor, the network administrator.");
            }
            else if (ReadLine == "Exit")
            {
                Console.Beep();
                this.ProgramOutput(Environment.NewLine + "Good-Bye!");
                Thread.Sleep(500);
                this.Dispose();
                //Environment.Exit(0);
            }
            //then check for return types
            //sets the generic object to the value that was 
            else if (Single.TryParse(ReadLine, out FloatOut))
            {
                ForgeModeOut = FloatOut;
            }
            else
            {
                ForgeModeOut = ReadLine;
            }
            //clear for the next entry
            //this.textBox1.Clear();
            //clear the generic type
            //ForgeModeOut=null;

            //once data is collected, hide this form...take information needed from it
            //...create/destroy this form each time forge mode is enabled
            this.ConsoleUp = false;
            //this.Close();
            //false will allow the method to retain important variables
            this.Hide();
        }
        //out version of getting console IO
        public void ConsoleOut(object OutputToConsole)
        {
            this.textBox2.Text += OutputToConsole.ToString();
        }
        public void ConsoleOutLine(object OutputToConsole)
        {
            this.textBox2.Text += (Environment.NewLine + OutputToConsole.ToString());
        }
        //so you don't have to convert to a string each time
        public void ConsoleOutLine(String OutputToConsole)
        {
            this.textBox2.Text += (Environment.NewLine + OutputToConsole.ToString());
        }
        //same thing, but display to screen asap
        public void ConsoleOutLineShow(String OutputToConsole,GraphicsDeviceManager graphics)
        {
            //set the graphics.fullscreen to false to allow the console to appear on top
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            ConsoleUp = true;
            this.textBox1.Text = "[No Input]";
            this.textBox2.Text += (Environment.NewLine + OutputToConsole.ToString());
            this.UpdatePrompts();
            //shows the dialog
            this.ShowDialog();
            //reset when dialog is done
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
        //posts console prompt
        public void ConsoleInputPrompt(String InputRequest,GraphicsDeviceManager graphics)
        {
            //set the graphics.fullscreen to false to allow the console to appear on top
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            ConsoleUp = true;
            this.label1.Text = "<c/>:" + InputRequest;
            this.UpdatePrompts();
            //shows the dialog
            this.ShowDialog();
            //reset when dialog is done
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            //this.label1.Refresh();
        }
        //to get the values 

        //*************************************************************************
        //methods from the console system, now incorportated in the console form

        public void RunExecutable(String FileName)
        {
            try
            {
                Process Executable = Process.Start(ProjectKeywords.ProgramDirectory + FileName);//ContentDirectory + @"\Senior ProjectContent\OldProjects\" + FileName);
                //SetParent(Executable.MainWindowHandle,Console.Handle or something like that...look into this
                //presumably, you can't return to the console or game until this app has been excited
                Executable.WaitForExit();
            }
            catch (Exception)
            {
                Console.Beep();
                textBox2.Text = "Error 117- File Run Error";
                System.Windows.Forms.MessageBox.Show(ProjectKeywords.ProgramDirectory + FileName,"File should be: ");
                Thread.Sleep(500);
                EndApp();
            }
        }
        public void SetUpConsole()
        {
            //ShowConsole();
            Console.Beep();
        }
        //methods for each console program
        public void ProgramHeader(String Name)
        {
            this.label2.Text = Name;
        }
        public void ProgramPrompt(String Prompt)
        {
            this.label1.Text = "<c/>:" + Prompt;
        }
        public void ProgramOutput(String Output)
        {
            this.textBox2.Text += Output;
        }
        public void ClearOutput()
        {
            this.textBox2.Text = "";
        }
        public String ReadLine()
        {
            return (this.textBox1.Text);
        }

        //***********************************************************************
        //very simple methods for  generating large collections of binary/random numbers
        //ONLY TO BE CALLED IN THE BUTTON EVENT

        private void RandomNumberGenerator()
        {
            //clear last output after the next bit was entered
            this.textBox2.Clear();
            this.ProgramHeader("Welcome to the Random Number Generator!");
            String TestNum = this.textBox1.Text;
            int Amount = 0;
            this.ProgramPrompt("How many numbers: ");
            //TestNum = ReadLine();
            if (TestNum != "Exit")
            {
                //object to get random data
                Random Randomizer = new Random();
                try
                {
                    Amount = Int32.Parse(TestNum);
                }
                catch
                {
                    this.ProgramOutput(Environment.NewLine + "Invalid Input");
                }
                if (Amount < MAXOUT)
                {
                    for (int cntr = 0; cntr < Amount; cntr++)
                    {
                        this.ProgramOutput(Randomizer.Next(0, 10).ToString());
                    }
                }
                else
                {
                    this.ProgramOutput("Error- Stack Overflow");
                }
            }
            else
                EndApp();
            //clear the last entry (what was entered)
            this.textBox1.Clear();
        }
        private void RandomProbabilityGenerator()
        {
            //clear last output after the next bit was entered
            this.textBox2.Clear();
            this.ProgramHeader("Welcome to the Random Probability Generator!");
            String TestNum = this.textBox1.Text;
            int Amount = 0;
            this.ProgramPrompt("How many probabilities: ");
            //TestNum = ReadLine();
            if (TestNum != "Exit")
            {
                //object to get random data
                Random Randomizer = new Random();
                try
                {
                    Amount = Int32.Parse(TestNum);
                }
                catch
                {
                    this.ProgramOutput(Environment.NewLine + "Invalid Input");
                }
                if (Amount < MAXOUT)
                {
                    for (int cntr = 0; cntr < Amount; cntr++)
                    {
                        this.ProgramOutput(Math.Round(Randomizer.NextDouble(), 4) + " ");
                    }
                }
                else
                {
                    this.ProgramOutput("Error- Stack Overflow");
                }
            }
            else
                EndApp();
            //clear the last entry
            this.textBox1.Clear();
        }
        private void RandomBinaryGenerator()
        {
            //clear last output after the next bit was entered
            this.textBox2.Clear();
            this.ProgramHeader("Welcome to the Random Binary Generator!");
            String TestNum = this.textBox1.Text;
            int Amount = 0;
            this.ProgramPrompt("How many bits: ");
            //TestNum = ReadLine();
            if (TestNum != "Exit")
            {
                //object to get random data
                Random Randomizer = new Random();
                //checks for invalid input exceptions
                try
                {
                    Amount = Int32.Parse(TestNum);
                }
                catch
                {
                    this.ProgramOutput(Environment.NewLine + "Invalid Input");
                }
                if (Amount < MAXOUT)
                {
                    for (int cntr = 0; cntr < Amount; cntr++)
                    {
                        this.ProgramOutput(Randomizer.Next(0, 2).ToString());
                    }
                }
                else
                {
                    this.ProgramOutput("Error- Stack Overflow");
                }
            }
            else
                EndApp();
            //clear the last entry
            this.textBox1.Clear();
        }
        private void FibinacciGenerator()
        {
            //clear last output after the next bit was entered
            this.textBox2.Clear();
            this.ProgramHeader("Welcome to the Fibonacci Generator!");
            String TestNum = this.textBox1.Text;
            int Amount = 0;
            this.ProgramPrompt("How many numbers: ");
            //TestNum = ReadLine();
            if (TestNum != "Exit")
            {
                int FirstNum = 1;
                int SecondNum = 1;
                this.ProgramOutput(Environment.NewLine);
                try
                {
                    Amount = Int32.Parse(TestNum);
                    //cheating! Quick and dirty way to correct for the printing of the first two digits
                    if(Amount==1)
                        this.ProgramOutput(FirstNum + " ");
                    else if(Amount>1)
                        this.ProgramOutput(FirstNum + " " + SecondNum + " ");
                    //then we correct accordingly
                    Amount -= 2;
                }
                catch
                {
                    this.ProgramOutput(Environment.NewLine + "Invalid Input");
                }
                if (Amount < MAXOUT+2)
                {
                    for (int cntr = 0; cntr < Amount; cntr++)
                    {
                        int Temp = FirstNum + SecondNum;
                        SecondNum = FirstNum;
                        FirstNum = Temp;
                        //auto-stop
                        if ((Temp >= int.MaxValue) || (Temp < 0))
                            this.ProgramOutput(Environment.NewLine + "Error- Max Integer Value reached");
                        else
                            this.ProgramOutput(Temp + " ");
                    }
                }
                else
                {
                    this.ProgramOutput("Error- Stack Overflow");
                }
            }
            else
                EndApp();
            //clear the last entry
            this.textBox1.Clear();
        }
        //method for all the end code for any console app
        private void EndApp()
        {
            Console.Beep();
            this.ProgramOutput(Environment.NewLine + "Good-Bye!");
            this.ConsoleType = ConsoleApp.MainMenu;
            Thread.Sleep(500);
            UpdatePrompts();
        }
        private void ConsoleForm_Load(object sender, EventArgs e)
        {
        }

        //this event fires upon entering the keyboard
        private void button2_Click(object sender, EventArgs e)
        {
            //determine run-time type based on the type of form that was created
            if (this.ConsoleType == ConsoleApp.MainMenu)
            {
                this.MainMenuSys();
            }
            else if (this.ConsoleType == ConsoleApp.RanNum)
            {
                this.RandomNumberGenerator();
            }
            else if (this.ConsoleType == ConsoleApp.RanProb)
            {
                this.RandomProbabilityGenerator();
            }
            else if (this.ConsoleType == ConsoleApp.RanBinary)
            {
                this.RandomBinaryGenerator();
            }
            else if (this.ConsoleType == ConsoleApp.Fibinacci)
            {
                this.FibinacciGenerator();
            }
            else if (this.ConsoleType == ConsoleApp.ForgeMode)
            {
                this.ForgeMenuSys();
            }
        }
        //updates dialoges based on type
        private void UpdatePrompts()
        {
            //clear the output regardless
            if (this.ConsoleType != ConsoleApp.ForgeMode)
                this.textBox2.Clear();
            this.textBox1.Clear();
            if (this.ConsoleType == ConsoleApp.MainMenu)
            {
                this.ProgramHeader("Welcome to the LHS Network");
                this.ProgramPrompt("");
                //resizing of the textbox based on the size of the prompt
                this.textBox1.Location = new System.Drawing.Point(this.label1.Width + 10, this.textBox1.Location.Y);
                this.textBox1.Width = (this.panel1.Location.X + this.panel1.Width) - 10;
            }
            else if (this.ConsoleType == ConsoleApp.RanNum)
            {
                this.ProgramHeader("Welcome to the Random Number Generator!");
                this.ProgramPrompt("How many numbers: ");
                //resizing of the textbox based on the size of the prompt
                this.textBox1.Location = new System.Drawing.Point(this.label1.Width + 10, this.textBox1.Location.Y);
                this.textBox1.Width = (this.panel1.Location.X + this.panel1.Width) - 10;
            }
            else if (this.ConsoleType == ConsoleApp.RanProb)
            {
                this.ProgramHeader("Welcome to the Random Probability Generator!");
                this.ProgramPrompt("How many probabilities: ");
                //resizing of the textbox based on the size of the prompt
                this.textBox1.Location = new System.Drawing.Point(this.label1.Width + 10, this.textBox1.Location.Y);
                this.textBox1.Width = (this.panel1.Location.X + this.panel1.Width) - 10;
            }
            else if (this.ConsoleType == ConsoleApp.RanBinary)
            {
                this.ProgramHeader("Welcome to the Random Binary Generator!");
                this.ProgramPrompt("How many bits: ");
                //resizing of the textbox based on the size of the prompt
                this.textBox1.Location = new System.Drawing.Point(this.label1.Width + 10, this.textBox1.Location.Y);
                this.textBox1.Width = (this.panel1.Location.X + this.panel1.Width) - 10;
            }
            else if (this.ConsoleType == ConsoleApp.Fibinacci)
            {
                this.ProgramHeader("Welcome to the Fibinacci Generator!");
                this.ProgramPrompt("How many numbers: ");
                //resizing of the textbox based on the size of the prompt
                this.textBox1.Location = new System.Drawing.Point(this.label1.Width + 10, this.textBox1.Location.Y);
                this.textBox1.Width = (this.panel1.Location.X + this.panel1.Width) - 10;
            }
            else if (this.ConsoleType == ConsoleApp.ForgeMode)
            {
                this.ProgramHeader("Forge Editor");
                //this.ProgramPrompt("Field: ");
                //resizing of the textbox based on the size of the prompt
                this.textBox1.Location = new System.Drawing.Point(this.label1.Width + 10, this.textBox1.Location.Y);
                this.textBox1.Width = (this.panel1.Location.X + this.panel1.Width) - 10;
            }
            this.Refresh();
        }

        //overload the show dialog method for drawing the form
        //call and get colors from a dialog menu
        public System.Windows.Forms.DialogResult ShowDialog(GraphicsDeviceManager graphics)
        {
            //set the graphics.fullscreen to false to allow the console to appear on top
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            ConsoleUp = true;
            //calls the original show dialog
            DialogResult Result = base.ShowDialog();
            //reset when dialog is done
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            return (Result);
        }

        //call and get colors from a dialog menu
        public System.Drawing.Color ShowColorDialog(Microsoft.Xna.Framework.Color InitialColor,GraphicsDeviceManager graphics)
        {
            //set the graphics.fullscreen to false to allow the console to appear on top
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            ConsoleUp = true;
            //takes the current color and converts it to the starting color on the prompt
            System.Drawing.Color ConvertColor = System.Drawing.Color.FromArgb(InitialColor.A, InitialColor.R, InitialColor.G, InitialColor.B);
            colorDialog1.Color=ConvertColor;
            colorDialog1.FullOpen = true;
            colorDialog1.ShowDialog(this);
            //reset when dialog is done
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            return(colorDialog1.Color);
        }
        public void CloseColorDialog()
        {
            ConsoleUp = false;
            colorDialog1.Dispose();
        }
    }
}
