using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class Jock : DataTypes.AI.Clicks
    {
        public Jock()
        {
        }
        public override AI.RoomTypes.ClassType RoomDestination()
        {
            //jocks visit crappy classes...that's what you get for beating up the coders on the playground so many years ago
            Random RanNum = new Random();
            int Probs = RanNum.Next(1, 4);
            switch (Probs)
            {
                case 1:
                    return (AI.RoomTypes.ClassType.Metals);
                case 2:
                    return (AI.RoomTypes.ClassType.English);
                case 3:
                    return (AI.RoomTypes.ClassType.Art);
                default:
                    return (AI.RoomTypes.ClassType.History);
            }
        }
    }
}
