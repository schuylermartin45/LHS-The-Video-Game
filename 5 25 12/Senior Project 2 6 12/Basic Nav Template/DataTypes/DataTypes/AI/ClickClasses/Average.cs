using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class Average : DataTypes.AI.Clicks
    {
        public Average()
        {
        }
        //average students are equally likely to go to any type of room
        public override AI.RoomTypes.ClassType RoomDestination()
        {
            //Average students are just a likely to go into any room, except for admin rooms
            int NumRoomTypes = 10; //inlcuding admin
            Random RanNum = new Random();
            int Probs = RanNum.Next(1, NumRoomTypes-1);
            switch(Probs)
            {
                case 1 :
                    return (AI.RoomTypes.ClassType.Art);
                case 2 :
                    return (AI.RoomTypes.ClassType.Computer);
                case 3 :
                    return (AI.RoomTypes.ClassType.Engineering);
                case 4 :
                    return (AI.RoomTypes.ClassType.English);
                case 5 :
                    return (AI.RoomTypes.ClassType.History);
                case 6 :
                    return (AI.RoomTypes.ClassType.Language);
                case 7 :
                    return (AI.RoomTypes.ClassType.Math);
                case 8 :
                    return (AI.RoomTypes.ClassType.Metals);
                default :
                    return (AI.RoomTypes.ClassType.Science);
            }
        }
    }
}
