using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    public class ComputerConsole : ScreenModel
    {

        public ComputerConsole(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, bool Moveable, bool PickUpable)
            : base(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, Moveable, PickUpable, 0f , false)
        {
            ActionString = "Computer";
            //the constructor isn't that special
        }
        public override void SpecialObjForgeAction()
        {
        }
        public override void Animation()
        {
        }
        public override void Action(String Filename, ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics)
        {
            //displays a console using the console system class
            //Console.WriteLine("Here!");
            ConsoleForm MainMenu = new ConsoleForm(ConsoleForm.ConsoleApp.MainMenu);
            MainMenu.ShowDialog(graphics);
        }

        //method that handles what reticles to be drawn, based on the placed portals
        public override DataValues.ReticleType SpecialReticle()
        {
            return(DataValues.ReticleType.ActionBtn);
        }
    }
}
