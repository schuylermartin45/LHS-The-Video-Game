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
    public class Light : ScreenModel
    {
        // size of an individual light
        public float radius;

        public Light(String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float Bounciness, Boolean Levitating)
            : base("Light", ModelName, SentModel, ModelPosition, ModelRotation, device, false, pickup, Bounciness, true)
        {
            this.ActionString = "Light";
            radius = 5f;
        }
        public Light(String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float Bounciness, Boolean Levitating, float radiusArg)
            : base("Light", ModelName, SentModel, ModelPosition, ModelRotation, device, false, pickup, Bounciness, true)
        {
            this.ActionString = "Light";
            radius = radiusArg;
        }
        public Light(String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float Bounciness, Boolean Levitating, String ActionString)
            : base("Light", ModelName, SentModel, ModelPosition, ModelRotation, device, false, pickup, Bounciness, true)
        {
            try
            {
                this.radius = float.Parse(ActionString.Substring(ActionString.IndexOf("Radius:") + 7, ActionString.IndexOf(";", ActionString.IndexOf("Radius:"))-ActionString.IndexOf("Radius:")-7));
            }
            catch
            {
                this.radius=5f;
            }
            this.ActionString = "Light";
        }
        public void UpdateActionString()
        {
            this.ActionString = "Light{Radius:" + radius + ";}";
        }
    }
}
