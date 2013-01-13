using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTypes
{
    //class that assists in the loading of files located in subfolders
    public class FileLoader
    {
        //array of string subfolders for a file
        private List<String> SubFolders;
        private String MainFolder=FolderNames.Files;
        public String Path;
        public FileLoader(List<String> SubFolderList)
        {
            Path = MainFolder;
            SubFolders = SubFolderList;
            //then builds the path from the list
            this.BuildPath();
        }
        public FileLoader(String TopFolder,List<String> SubFolderList)
        {
            MainFolder = TopFolder;
            Path = MainFolder;
            SubFolders = SubFolderList;
            //then builds the path from the list
            this.BuildPath();
        }
        //build path from the array of subfolders
        public String BuildPath()
        {
            for (int cntr = 0; cntr < SubFolders.Count; cntr++)
            {
                Path += SubFolders[cntr];
            }
            return (Path);
        }
        //method to clear the path to the original 1st folder
        public void ClearPath()
        {
            Path = MainFolder;
        }
        //method that returns a list of all the locations/room files
        public List<String> BuildListOfRooms()
        {
            List<String> ListOfRooms= new List<String>();
            //main entrance/hall

            //add in phase 1 rooms

            //add in phase 2

            //phase 3

            //phase 4

            //phase 5

            //phase 6

            //any other cases we've left out (special cases)
            
            /*
            foreach()
            {
            }*/
            return (ListOfRooms);
        }
        //structure that holds room data...because I didn't want to make yet another class
        public struct RoomData
        {
            //information that can be used to sort the data files later; if you are in phase 1, then only search for classes in that phase
            public String Phase;
            //subdivision for even more sorting help; should be upper, lower, hallway or some other notation
            public String SubDiv;
            //string value of the room
            public String RoomName;
            //integer room number
            public int RoomNum;
            //actual path of the room is stored here
            public String DataFilePath;
        }
    }
}
