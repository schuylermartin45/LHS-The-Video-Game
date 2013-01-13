using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senior_Project
{
    public class ObjectTypes
    {
        //series of constants, stored in a object that hold what type of object should be used
        public const int Sphere = 1;
        public const int Box = 2;
        public const int Frustum = 3;
        public const int Ray = 4;
        public const int Play = 5;
        //public enum ObjectTypes { Sphere = 1, Box, Frustum, Ray, Plane, Room };
        public int TypeOfObject;
        public ObjectTypes(int TypeArg)
        {
            TypeOfObject = TypeArg;
        }
    }
}
