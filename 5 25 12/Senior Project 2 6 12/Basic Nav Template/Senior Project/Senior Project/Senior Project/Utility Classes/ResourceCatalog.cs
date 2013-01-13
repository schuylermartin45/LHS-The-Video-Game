using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
namespace Senior_Project
{
    //**********************************************************************
    //Contains ArrayLists that hold "Manager" Items in various sorted orders
    //includes search, sort, add, subtract methods
    //saves to a file
    //**********************************************************************
    //**EDIT: the resource manager should have it's own seperate program that allows for file changes/associations
    class ResourceCatalog //: Manager //extends Manager for ease of use
    {
        //Resource Catalog index; increments by one when data is entered; effectively acts as a way to sort by date
        //INDEX OF 0 should be the TEXT file that CONTAINS THIS INFORMATION
        public int CurrentIndex;
        //main file path for all resources/"Managers"; locates the "resource" folder
        public String MainPath;
        //file that saves manager data
        StreamWriter FileWriter; //FileWriter = new StreamWriter(@MainPath+""); #remember to close
        StreamReader FileReader; //FileReader = new StreamReader(@MainPath+""); #remember to close
        //Array that holds Manager Objects by index value, (when it is added to the original list...not sure now that helps sorting
        //wise but it makes sense); kind of like a catalog # in a library
        List<Manager> ManagerList=new List<Manager>();
        public ResourceCatalog()
        {
            //probably shouldn't use this folder.....change later? (or just don't use the default)
            this.MainPath = @"C:\Resources\";//MPath;
            //initializes the stream reader and writer to the text file that holds the old data
            FileWriter = new StreamWriter(@MainPath + "Manager0.txt");
            FileReader = new StreamReader(@MainPath + "Manager0.txt");
            //loads the new data in
            ReadData();
        }
        //input a MainPath
        public ResourceCatalog(String MPath)
        {
            this.MainPath = MPath;
            //initializes the stream reader and writer to the text file that holds the old data
            FileWriter = new StreamWriter(@MainPath + "Manager0.txt");
            FileReader = new StreamReader(@MainPath + "Manager0.txt");
            //loads the new data into the List
            ReadData();
        }
        //basic method that sorts based on integer passed to it
        private void GeneralSort()//import a generic type
        {

        }
        
        //sorts based on item index; effectively sorts by date
        public void SortIndex()
        {
            
        }
        //sorts based on item type
        public void SortType()
        {

        }
        //methods that read/write to a text file each manager object
        public void ReadData()
        {
            //index item 0 is the text file that holds the rest of this data...hope this doesnt cause problems later
            //Manager TextData = new Manager();
            //ManagerList.Add(TextData);
            int CountObjects=0;
            //then adds the rest of the file in a loop
            do
            {
                //for once, a switch can use STRINGS!!!!!
                String Data=FileReader.ReadLine();
                //string variables created, to be passed into manager constructor
                String i;
                String Name;
                String Descr;
                String MP;
                String LP;
                String Typ;
                String FExt;
                //these two will need further processing into broken-up componenets
                String Date;
                String Time;
                //another loop to go through the variables of one object
                for (int cntr = 0; cntr < 9; cntr++)
                {

                    //does code according to the requirements of the variable at hand
                    switch (MainPath)
                    {
                        case "Index=":
                            break;
                        case "Name=":
                            break;
                        case "Description=":
                            break;
                        case "MPath=":
                            break;
                        case "LPath=":
                            break;
                        case "Type=":
                            break;
                        case "FileExt=":
                            break;
                        case "Date=":
                            break;
                        case "Time=":
                            break;
                    }
                }
                //creates a new manager object with the data found; adds it to the list
              //  ManagerList.Add(new Manager(CountObjects,
                //increments the counter
                CountObjects++;
            }while(FileReader.EndOfStream==false);
            FileReader.Close();
        }

        //"processor functions split up date/time into int's
        //time should be formatted: 2/34/12
        private int ProcessYear(String Date)
        {
            //AP test taught me to be lazy with i for local ints
            int i=0;
            return(i);
        }
        private int ProcessMonth(String Date)
        {
            int i=0;
            return(i);
        }
        private int ProcessDay(String Date)
        {
            int i = 0;
            return (i);
        }
        
        //time should be formatted: 2:34:12
        //all digits are broken up dependent of the colons; in case of: 05
        private int ProcessHour(String Time)
        {
            String Hour="";
            //most intuitive way: builds up the string until :
            char[] TempArray = Time.ToCharArray();
            int cntr = 0;
            do
            {
                Hour+=TempArray[cntr];
                cntr++;
            }while(TempArray[cntr+1]!=':');
            //converts to int
            return (int.Parse(Hour));
        }
        private int ProcessMin(String Time)
        {
            String Min = "";
            //most intuitive way: builds up the string until :
            char[] TempArray = Time.ToCharArray();
            int cntr = 0;
            //counts # of : seen already
            int ColonCntr = 0;
            do
            {
                //increments ColonCntr

                if (TempArray[cntr] == ':')
                {
                    ColonCntr++;
                    //emergency break; in theory, should just stop processing short, saving time
                    if (ColonCntr > 1)
                        break;
                }
                //adds up string until colon
                if (ColonCntr == 1)
                {
                    Min += TempArray[cntr];
                    cntr++;
                }
            } while (ColonCntr<=1);
            //converts to int
            return (int.Parse(Min));
        }
        private int ProcessSec(String Time)
        {
            String Sec = "";
            //most intuitive way: builds up the string until :
            char[] TempArray = Time.ToCharArray();
            int cntr = 0;
            //counts # of : seen already
            int ColonCntr = 0;
            do
            {
                //increments ColonCntr

                if (TempArray[cntr] == ':')
                {
                    ColonCntr++;
                }
                //adds up string until colon
                if (ColonCntr == 2)
                {
                    Sec += TempArray[cntr];
                    cntr++;
                }
            //terminates at the end of the String
            } while (cntr<TempArray.Length);
            //converts to int
            return (int.Parse(Sec));
        }

        public void WriteData()
        {
            //much easier than reading it out 
            //clears file?
            //loops through all data
            for (int cntr = 0; cntr < ManagerList.Count; cntr++)
            {
                FileWriter.WriteLine(ManagerList[cntr].ReturnText());
            }
            //closes file...important!
            FileWriter.Close();
        }

        //methods that display data while coding latter on-> essentially tools to search through inventory
        //actual search algorithm

        //display data

        //remove a model from the list; resets index of all data back 1 point; after the given index that has been removed

    }
}
