using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    [Serializable]
    public class ModelReader
    {
        public struct ModelSpecs
        {
            public String Name;
            public String ModelName;
            public Vector3 Position;
            public Vector3 Rotation;
            public float Bounciness;
            public Boolean Moveable;
            public Boolean PickUpable;
            public Boolean Levitating;

            public ModelSpecs(String Name, String ModelName, Vector3 Position, Vector3 Rotation, Boolean Moveable, Boolean PickUpable, float Bounciness, Boolean Levitating)
            {
                this.Name = Name;
                this.ModelName = ModelName;
                this.Position = Position;
                this.Rotation = Rotation;
                this.Moveable = Moveable;
                this.PickUpable = PickUpable;
                this.Bounciness = Bounciness;
                this.Levitating = Levitating;
            }
        }   
        public struct ActionSpecs
        {
            public ModelSpecs Specs;
            public String Action;

            public ActionSpecs(String Name, String ModelName, Vector3 Position, Vector3 Rotation, Boolean Moveable, Boolean PickUpable, float Bounciness, Boolean Levitating, String ActionInfo)
            {
                Specs = new ModelSpecs(Name, ModelName, Position, Rotation, Moveable, PickUpable, Bounciness, Levitating);
                this.Action = ActionInfo;
            }
        }

        public Vector3 RoomPosition;
        public List<ModelSpecs> Models;
        public List<ActionSpecs> ActionModels;
        public ModelReader()

        {

        }
    }
}
