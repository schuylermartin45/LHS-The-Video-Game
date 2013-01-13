using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senior_Project
{
    //**********************************************************************
    //Basic class to hold location, naming, and indexing data for our resources
    //the Catologue Class will hold Manager Objects for sorting, indexing, finding, etc.
    //**********************************************************************
    class Manager : ResourceCatalog
    {
        //Integer constants that represent file types
        public const int Audio = 1;
        public const int Texture = 2;
        public const int Model = 3;
        public const int Pic = 4;
        //index external apps; extra .exe's
        public const int ExternalApp = 5;
        public const int Animations = 6;
        public const int TextFiles = 7;
        //-------------------------------
        public int Index;
        public String Name;
        public String Description;
        //holds path until "local path"; held by the resource catalog class
        //public String MainPath;
        //holds local file path
        public String LocalPath;
        //holds the entire path
        public String FullPath;
        //Store file type
        public int Type;
        //stores file extension--> we do what we must, because we can
        public String FileExt;
        //REALLY COOL!--> Time Stamps the computer time that this item was added
        public DateTime DateAdded;
        //default constructor; constructs the index 0 item that hold
        public Manager()
        {
            Index = 0;
            Name = "Manager0";
            Description = "Important: saves manager catalog data";
            //MainPath = MPath; inherited
            //up until the Resources folder is handled by the MainPath
            LocalPath = @"\";
            Type = TextFiles;
            FileExt = ".txt";
            FullPath = MainPath + LocalPath + Name + FileExt;
            //time stamps the object once added to the registry
            DateAdded = DateTime.Now;
        }
        //now with arguments!
        public Manager(int i,String NameArg,String DescripArg,String MPath,String LPath,int TypeArg,String ExtArg)
        {
            Index=i;
            Name=NameArg;
            Description = DescripArg;
            //MainPath = MPath; inherited
            LocalPath = LPath;
            Type = TypeArg;
            FileExt = ExtArg;
            FullPath = MainPath + LocalPath + Name + FileExt;
            //time stamps the object once added to the registry
            DateAdded = DateTime.Now;
        }
        //this constructor should only be used to create new Manager items in the manager program
        public Manager(int i, String NameArg, String DescripArg, String MPath, String LPath, int TypeArg, 
            String ExtArg, int year, int month, int day, int hour, int minute, int second)
        {
            Index = i;
            Name = NameArg;
            Description = DescripArg;
            //MainPath = MPath; inherited
            LocalPath = LPath;
            Type = TypeArg;
            FileExt = ExtArg;
            FullPath = MainPath + LocalPath + Name + FileExt;
            //time stamps the object once added to the registry
            DateAdded = new DateTime(year, month, day, hour, minute, second);
        }
        //now with less parameters!
        public Manager(int i, String NameArg, String MPath, String LPath, int TypeArg, String ExtArg)
        {
            //nname
            Index = i;
            Name = NameArg;
            Description = "Basic Manager Object";
            //paths
            //MainPath = MPath; inherited
            LocalPath = LPath;
            //type and extension arguments
            Type = TypeArg;
            FileExt = ExtArg;
            FullPath = MainPath + LocalPath + Name + FileExt;
            //time stamps the object once added to the registry
            DateAdded = DateTime.Now;
        }
        //returns the manager in a text format
        public String ReturnText()
        {
            String TextOutput =
            "Index= " + "\n" +
            "Name= " + this.Name + "\n" +//should be able to include spaces
            "Description= " + this.Description + "\n" +//should be able to include spaces
            "MPath= " + this.MainPath + "\n" +
            "LPath= " + this.LocalPath + "\n" +
            "Type= " + this.Type + "\n" +
            "FileExt= " + this.FileExt + "\n" +
                //full path can be deduced from 3 other strings; code done as a seperate process
                //DateTime.ToString() is in this format:
            "Date= " + this.DateAdded.ToString("d") + "\n" +
            //specific to the second
            "Time= " +this.DateAdded.ToString("T") + "\n";
            //Console.WriteLine(DateTime.Now.ToString("T"));
            return(TextOutput);
        }
    }
}
