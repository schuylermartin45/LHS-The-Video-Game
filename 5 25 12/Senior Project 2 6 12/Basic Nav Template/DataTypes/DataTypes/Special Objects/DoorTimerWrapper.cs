using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    public class DoorTimerWrapper : TimerClass
    {
        //seconds until the door closes
        private const float SecondsTillClose = 5.0f;
        private Door DoorOfInterest;

        //crap variables needed to close the stupid door
        private ContentManager Content;
        private List<ScreenModel> models;
        private GraphicsDeviceManager graphics;
        //private Vector3 PersonPosition;
        private CollisionCheckSystem UserRef;

        public DoorTimerWrapper(Door DoorObject, ContentManager ContentArg, List<ScreenModel> modelsArg, GraphicsDeviceManager graphicsArg,CollisionCheckSystem User) :
            base()
        {
            //object that needs to be referenced in order to be closed
            DoorOfInterest = DoorObject;

            //args...girrrrrr
            Content = ContentArg;
            models = modelsArg;
            graphics = graphicsArg;
            UserRef = User;
        }

        //override that closes the door after a set time-out
        public override void IncrementEvent(object source, ElapsedEventArgs e)
        {
            //just increments milliseconds by the amount when 
            MilliSeconds += IncrementLength;
            //increments seconds; every 1000 milliseconds
            Seconds = MilliSeconds / 1000f;
            //kills the event, closes door
            if (Seconds >= SecondsTillClose)
            {
                if (DoorOfInterest.OpenTrue)
                    DoorOfInterest.Action(Content, models, graphics, UserRef);
                /*
                if(DoorOfInterest.OpenTrue)
                {
                    //kills the door that has been opened
                    if (Door.GetBoundingBoxSurroundings(DoorOfInterest.InverseDoor).Contains(this.UserRef.Position))
                        DoorOfInterest.CloseDoor(Content, models, graphics, this.UserRef.Position);
                    else
                        DoorOfInterest.InverseDoor.CloseDoor(Content, models, graphics, this.UserRef.Position);
                    //DoorOfInterest.CloseDoor(this.Content,this.graphics, this.PersonPosition,this.UserLocation);
                }
                 */
                //kill event
                this.Stop();
            }
        }
    }
}
