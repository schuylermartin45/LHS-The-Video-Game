using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Senior_Project.School_Builder
{
    //holds data and information about objects/locations/and limits of a classroom
    public class ClassroomClass : GenericUnit
    {
        //4 walls of the room; for collision purposes
        public Plane NorthWall;
        public Plane SouthWall;
        public Plane WestWall;
        public Plane EastWall;
        //positions of the doors; represented by a lower and an upper point


        //
    }
}
