using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    public abstract class FolderNames
    {
        //class that contains string constants that will be used to find folders

        //general folders
        public static String Effects = @"Effects/";
        public static String Fonts = @"Fonts/";

        //sub-groups for models
        public static String Models = @"Models/";
        public static String HighRes = @"HighRes/";
        public static String LowRes = @"LowRes/";
        //these arent technically folders, more like extensions or EXT
        public static String HighResEXT = @"HighRes";
        public static String LowResEXT = @"LowRes";


        public static String Textures = @"Textures/";
        public static String Reticles = @"Reticles/";

        //files that store game room locations
        public static String Files = @"Files/";
        public static String Halls = @"Halls/";
        public static String SpecialCases = @"SpecialCases/";
        public static String TestFiles = @"TestFiles/";
        public static String Wings = @"Wings/";
        //phases
        public static String Phase = @"Phase/";
        public static String Phase1 = @"Phase1/";
        public static String Phase2 = @"Phase2/";
        public static String Phase3 = @"Phase3/";
        public static String Phase4 = @"Phase4/";
        public static String Phase5 = @"Phase5/";
        public static String Phase6 = @"Phase6/";
        //sub groupings of rooms
        public static String Lower = @"Lower/";
        public static String Upper = @"Upper/";
        public static String Room_100_147 = @"Room_100_147/";
        public static String Room_174_157 = @"Room_174_157/";

        //static methods for loading stuff
        
    }
}
