using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class Nerd : DataTypes.AI.Clicks
    {
        public Nerd()
        {
        }
        public override AI.RoomTypes.ClassType RoomDestination()
        {
            //nerds are only visit the best classes
            Random RanNum = new Random();
            int Probs = RanNum.Next(1,4);
            switch (Probs)
            {
                case 1:
                    return (AI.RoomTypes.ClassType.Computer);
                case 2:
                    return (AI.RoomTypes.ClassType.Engineering);
                case 3:
                    return (AI.RoomTypes.ClassType.Math);
                default:
                    return (AI.RoomTypes.ClassType.Science);
            }
        }
    }
}
