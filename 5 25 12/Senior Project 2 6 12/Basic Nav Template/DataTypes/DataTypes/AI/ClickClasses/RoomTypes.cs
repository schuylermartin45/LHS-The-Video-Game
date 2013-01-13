using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTypes.AI;

namespace DataTypes.AI
{
    public class RoomTypes
    {
        public Room RoomInfo;
        //holds all of the rooms in the school and classifies them as a type
        public static List<Room> RoomList;
        //AI constants
        public enum ClassType { Math = 1, Science, History, English, Computer, Metals, Engineering, Art, Language, Admin };
        //Constructor
        public static void RoomFiller()
        {
            RoomList = new List<Room>();
            FillRooms();
        }
        //constructor builds stucture...i dont remember why i wanted to use a structure
        public RoomTypes()
        {
            RoomInfo = new Room();
            RoomInfo.Num = 0;
            RoomInfo.Name = "";
            RoomInfo.RType = ClassType.Math; 
            //if of type "admin", then fill out this special case (library, nurse, etc)
            RoomInfo.SpecialName = "";
            //phase
            RoomInfo.PhaseNum = 0;
            RoomInfo.Phase = "";
            //subgroupings of a phase
            RoomInfo.SubPhase = "";
            //add in doors?
            RoomInfo.DoorsInRoom = new List<Door>();
        }
        //structure for each room in the building
        public struct Room
        {
            public int Num;// = 0;
            public String Name;// = "";
            public ClassType RType;// = ClassType.Math; 
            //if of type "admin", then fill out this special case (library, nurse, etc)
            public String SpecialName;// = "";
            //phase
            public int PhaseNum;// = 0;
            public String Phase;// = "";
            //subgroupings of a phase
            public String SubPhase;// = "";
            //add in doors?
            public List<Door> DoorsInRoom;
        }
        //all the rooms in the building, destinations for the 
        public static void FillRooms()
        {
            //loops through each phase, filling in the structure as needed
            //phase 1
            int[] RoomsDNE = {106,107,108,113,114,115,116,117,118,119,120,127,128,129,130,131,135,136,137,138,139,140,141,142,143,144,148,149,150,151,152,153,154,155,156,170,171};
            PhaseRoom(RoomsDNE, 101, 174);
            //phase 2
            RoomsDNE = new int[]{211,212,213,214,215,216,217};
            PhaseRoom(RoomsDNE, 201, 221);
            //phase 3
            RoomsDNE = new int[]{1,2};
            PhaseRoom(RoomsDNE, 100, 174);
            //phase 4
            RoomsDNE = new int[]{1,2};
            PhaseRoom(RoomsDNE, 100, 174);
            //phase 5
            RoomsDNE = new int[]{1,2};
            PhaseRoom(RoomsDNE, 100, 174);
            //phase 6
            RoomsDNE = new int[]{ 1, 2 };
            PhaseRoom(RoomsDNE, 610, 627);
        }
        //fills in a single phase
        private static void PhaseRoom(int[] RoomsDNE,int StartNum,int EndNum)
        {
            //increments through rooms that do not exist
            int DNECntr = 0;
            for (int cntr = StartNum; cntr <= EndNum; cntr++)
            {
                //for rooms that don't actually exist...
                if(RoomsDNE[DNECntr] == cntr)
                {
                    //increment the cntr, do not add this room
                    DNECntr++;
                }
                else
                {
                    //fill in the structure
                    Room Temp = new Room();
                    Temp.Num = cntr;
                    Temp.Name = "Room_"+cntr;
                    //extract the first digit of the start num, which will determine the phase
                    Temp.PhaseNum = StartNum % 100;
                    Temp.Phase = DeterminePhase(Temp.PhaseNum);
                    //assign the room type...figure out some clever way to do that...
                    Temp.RType = DetermineType(Temp.Num);
                    //come up with some way of dealing this out...
                    Temp.SubPhase = DetermineSubPhase(Temp.Num);
                    RoomList.Add(Temp);
                }
            }
        }
        public static String DeterminePhase(int PhaseNum)
        {
            String PhaseName = "";
            switch (PhaseNum)
            {
                case 1: PhaseName = "Phase1";
                    break;
                case 2: PhaseName = "Phase2";
                    break;
                case 3: PhaseName = "Phase3";
                    break;
                case 4: PhaseName = "Phase4";
                    break;
                case 5: PhaseName = "Phase5";
                    break;
                case 6: PhaseName = "Phase6";
                    break;
            }
            return (PhaseName);
        }
        public static String DetermineSubPhase(int RoomNum)
        {
            String SubPhase = "";
            int PhaseNum = Convert.ToString(RoomNum)[0];;
            int SubPhaseNum = Convert.ToString(RoomNum)[1];
            if (PhaseNum == 1)
            {
                int LastTwo = (SubPhaseNum*10)+Convert.ToString(RoomNum)[2];
                if (LastTwo < 48)
                {
                    SubPhase = "Side_101";
                }
                else
                {
                    SubPhase = "Side_174";
                }
            }
            else if (PhaseNum == 5)
            {
                if (SubPhaseNum == 3)
                {
                    SubPhase = "Upper";
                }
                else
                {
                    SubPhase = "Lower";
                }
            }
            else if (PhaseNum == 6)
            {
                if (SubPhaseNum == 1)
                {
                    SubPhase = "Lower";
                }
                else
                {
                    SubPhase = "Upper";
                }
            }
            return(SubPhase);
        }
        public static ClassType DetermineType(int RoomNum)
        {
            ClassType TempType = ClassType.Admin;
            int PhaseNum = Convert.ToString(RoomNum)[0]; ;
            int SubPhaseNum = Convert.ToString(RoomNum)[1];
            if (PhaseNum == 1)
            {
                int LastTwo = (SubPhaseNum * 10) + Convert.ToString(RoomNum)[2];
                if (LastTwo < 48)
                {
                    
                }
                else
                {
                    
                }
            }
            else if (PhaseNum == 5)
            {
                if (SubPhaseNum == 3)
                {
                    
                }
                else
                {
                   
                }
            }
            else if (PhaseNum == 6)
            {
                if (SubPhaseNum == 1)
                {
                    
                }
                else
                {
                    
                }
            }
            return (TempType);
        }
        //method that takes a list of all the rooms and spits out a random room of a given type
        public static Room GetRoom(ClassType TypeKey)
        {
            //uses the built in search methods to find all the rooms of a given type
            List<Room> ListOfType = RoomList.FindAll(
                delegate(Room RT)
                {
                    return (RT.RType == TypeKey);
                });
            Random RanIndex = new Random();
            //randomly find a room from the list
            return(ListOfType[RanIndex.Next(0,ListOfType.Count-1)]);
        }
    }
}
