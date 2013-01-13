using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Senior_Project
{
    //Because there is no general Bounding class that all other bounding object extend, 
    //I have to make a class to allow casting/easy checks in models 
    public class BoundingGeneral
    {
        public enum Collisions { Sphere = 1, Box, Frustum, Ray, Plane, Room };
        public Collisions CollisionType;

        private BoundingBox BoxObj;
        private BoundingSphere SphereObj;
        private BoundingFrustum FrustumObj;
        private Ray RayObj;
        private Plane PlaneObj;
        
        //constructors
        public BoundingGeneral()
        {

        }



    }
}
