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

namespace DataTypes.Special_Objects
{
    public class PortalGun : ScreenModel
    {
        //the portal gun has two portal objects, blue and orange
        private Portal Blue;
        private Portal Orange;
        public PortalGun(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device)
            : base(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, true, true, 0f, false)
        {
            ActionString = "PortalGun";
            //constructs the portal gun
            Blue = new Portal();
            Orange = new Portal();
        }
        public override void SpecialObjForgeAction()
        {
        }
        public override void Animation()
        {
        }
        public override void Action(String Filename, ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics)
        {
            //change to the default portal reticle
            //this.ReticleToDraw = DataValues.ReticleType.PortalEmpty;
        }
        //method that handles what reticles to be drawn, based on the placed portals
        public override DataValues.ReticleType SpecialReticle()
        {
            if ((Blue.IsActive()) && (!Orange.IsActive()))
                return(DataValues.ReticleType.PortalBlue);
            if ((!Blue.IsActive()) && (Orange.IsActive()))
                return(DataValues.ReticleType.PortalOrange);
            if ((Blue.IsActive()) && (Orange.IsActive()))
                return(DataValues.ReticleType.PortalBoth);
            //else, return a blank portal icon
            return (DataValues.ReticleType.PortalEmpty);
        }
    }
}
