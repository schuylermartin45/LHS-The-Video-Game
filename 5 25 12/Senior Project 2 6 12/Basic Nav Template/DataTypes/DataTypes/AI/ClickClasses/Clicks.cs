using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes.AI
{
    //clicks or groups of AI that tend to interact with one and another
    public abstract class Clicks
    {
        int TimeTogether = 0;
        int TimeToRoom = 0;

        //returns a destination for the object; the likelyhood of going to some classes is dependent on what click you are in
        public virtual String RoomDestinationToString()
        {
            return("");
        }
        //enum version
        public virtual AI.RoomTypes.ClassType RoomDestination()
        {
            return (AI.RoomTypes.ClassType.Admin);
        }
        //common behavoir for a click type; how AI interact with each other
        public virtual void Behavior()
        {

        }
        
        //how an AI model finds a room
        public virtual void FindRoom()
        {

        }
    }
}
