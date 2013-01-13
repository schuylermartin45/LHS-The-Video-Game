using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DataTypes;

namespace Senior_Project.School_Builder
{
    //holds shared variables for all of the "container" classes; primarly location variables and lists
    public class GenericUnit
    {
        //Model that represents this particular object
        public ScreenModel UnitModel;
        //holds a collection of screen models that are apart of this measurement unit; class rooms will have objects with positions relative to the room, 
        //Phases will have a set of positions open to each phase, etc
        public List<ScreenModel> ModelList;
        //list of "doorways" that determines the end of an area...
        public List<Portal> PortalList;
        //center position of the object in space
        public Vector3 CenterPoint;
        //Number of lower level objects in a room / unit of space; # objects, rooms, etc
        public int NumObjects;
        //object length, width, and height;
        //should correspond with dimensions of the vectors
        public float SizeX;
        public float SizeY;
        public float SizeZ;
        //constructs a generic unit
        public GenericUnit()
        {
            //work on this later
            //UnitModel = new ScreenModel();
            ModelList = new List<ScreenModel>();
            PortalList = new List<Portal>();
            CenterPoint=new Vector3();
            //calculates the unit model's size
            CalculateSize();
        }
        //method that calculates the 3-dimensions of length a model has...may be difficult to do
        private void CalculateSize()
        {
            
        }
        //gets area of the map, for math
        public float Area()
        {
            return(SizeX*SizeY);
        }
        //gets perimeter
        public float Perimeter()
        {
            return((SizeX*2)+(SizeY*2));
        }
        //gets volume
        public float Volume()
        {
            return(Area()*SizeZ);
        }
    }
}
