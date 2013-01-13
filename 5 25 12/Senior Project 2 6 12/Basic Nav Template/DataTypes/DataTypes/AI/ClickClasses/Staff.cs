using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    public class Staff : DataTypes.AI.Clicks
    {
        public Staff()
        {
        }
        //can only go into admin rooms
        public override AI.RoomTypes.ClassType RoomDestination()
        {
            //there are 
            return (AI.RoomTypes.ClassType.Admin);
        }
    }
}
