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

namespace DataTypes.AI
{
    //class that handles the decision making for one person; visual/drawing/animation of a person is handled by the overridden methods of the screen model
    public class AIModel : ScreenModel
    {
        //Click association
        public Clicks TypeOfClick;
        //start position for an AI
        public AI.RoomTypes.Room StartRoom;
        //Room that the AI is heading for
        public AI.RoomTypes.Room TargetRoom;
        //large bounding sphere that detects AI and nearby rooms/doors
        public BoundingSphere AIDetection;

        //AI should bounding spheres
        public AIModel(Model SentModel, GraphicsDevice device) 
            : base(MakeName(),"Human AI",SentModel,Vector3.Zero,Vector3.Zero,device,false,false)
        //ScreenModel(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup)
        {
            // assign what kind of person this is
            TypeOfClick = AllocateClick();
            //allocate target 
            StartRoom = RoomTypes.GetRoom(TypeOfClick.RoomDestination());
            //starting room and model position based on 
            this.Position = StartRoom.DoorsInRoom[0].Position;
            //find another related room
            do{
                TargetRoom = RoomTypes.GetRoom(TypeOfClick.RoomDestination());
            }while(StartRoom.Num == TargetRoom.Num);
            AIDetection = new BoundingSphere(this.Position , 7.0f);
        }

        //method to move the AI
        public void MoveAI(List<Door> InRange)
        {
            //call method to move as needed...based on where the AI is
            Door CheckDoor = FindClosestDoor(InRange);
            if (CheckDoor.Position.Equals(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue)) == false)
            {
                this.MoveBasedOnDoorNumber(InRange);
            }
            else
            {
                this.MoveWithoutDoor();
            }
            //when moved, move the bounding sphere
            AIDetection.Center = this.Position;
        }

        //update method that overrides the

        //------------------------------------------------------------------------
        //Search/move algorithms
        //takes in a list of objects and creates a list of the doors withing range of the user
        private List<Door> ListOfDoorsInRange(List<ScreenModel> VisibleObjects)
        {
            List<Door> InRange = new List<Door>();
            for(int cntr=0; cntr<VisibleObjects.Count; cntr++)
            {
                if(VisibleObjects[cntr].GetType() == typeof(Door))
                {
                    if(VisibleObjects[cntr].CheckCollision(this.AIDetection))
                    {
                        InRange.Add((Door)VisibleObjects[cntr]);
                    }
                }
            }
            return(InRange);
        }
        //takes in a list of doors and spits out the one that is closest to the user
        //typical search method
        private Door FindClosestDoor(List<Door> InRange)
        {
            Door Closest = new Door(new Vector3(float.MaxValue,float.MaxValue,float.MaxValue));
            float SmallestDistance = float.MaxValue;
            for (int cntr = 0; cntr < InRange.Count; cntr++)
            {
                //only checks to the right side of the hallway
                //**************SHOULD CHECK THIS LATER
                if ((Closest.Position.X - this.Position.X > 0) && (Closest.Position.Z - this.Position.Z > 0))
                {
                    //finds based on smallest distance
                    float TempDist = Vector3.Distance(this.Position,Closest.Position);
                    if (TempDist < SmallestDistance)
                    {
                        Closest = InRange[cntr];
                        SmallestDistance = TempDist;
                    }
                }
            }
            return (Closest);
        }
        //method responsible for AI navigation
        public void MoveBasedOnDoorNumber(List<Door> InRange)
        {
            //finds the current door number
            Door CheckDoor = FindClosestDoor(InRange);
            int RoomNum = Door.ReturnRoomNum(CheckDoor.InsideDoor);

            //math!!!!
            int SinPos = (Math.Sin(MathHelper.ToRadians(Position.X + 90)) < 0) ? -1 : 1;
            int CosPos = (Math.Cos(MathHelper.ToRadians(Position.X + 90)) < 0) ? -1 : 1;

            //now use that room number, compare it with the destination and move accordingly
            if (RoomNum < TargetRoom.Num)
            {
                //go forward
                Velocity.X += DataValues.MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(Position.X)) * (float)Math.Sin(MathHelper.ToRadians(Position.X));
                Velocity.Z += DataValues.MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(Position.X)) * (float)Math.Cos(MathHelper.ToRadians(Position.X));
            }
            if (RoomNum > TargetRoom.Num)
            {
                //go backward
                Velocity.X -= DataValues.MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(Position.X)) * (float)Math.Sin(MathHelper.ToRadians(Position.X));
                Velocity.Z -= DataValues.MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(Position.X)) * (float)Math.Cos(MathHelper.ToRadians(Position.X));
                this.Turn(180f);
            }
            //if these too rooms are equal, the AI still has to navigate to the 
            if(RoomNum == TargetRoom.Num)
            {
                //move in a velocity towards the position of the door
                Vector3 Difference = new Vector3(Math.Abs(CheckDoor.Position.X - this.Position.X), Math.Abs(CheckDoor.Position.Y - this.Position.Y), Math.Abs(CheckDoor.Position.Z - this.Position.Z));
                //divide this difference by let's say a large number and apply this change over "time" during the clock cycle
                this.Velocity = (1/50) * Difference;
            }
        }
        public void MoveWithoutDoor()
        {

        }
        //in degrees
        public void Turn(float Theta)
        {
            //check this with Nick....
            this.modelRotation.X = MathHelper.ToRadians(Theta);
            //this.modelRotation.Z = Theta;
            //rebuild the bounding box
            this.BoundingSetup();
        }
        //------------------------------------------------------------------------
        //functions that randomly allocate names,  clicks, ect for each AI model
        //random allocates a fake name to each AI
        private static String LastNames()
        {
            String[] PossibleNames = {"Hilton","Martin","DiMarco","Mayer","Willis","Saur","Kempf","Johnson", "Jobs","Gates","Sampson","Cooper","Wooding","Ewen"};
            Random RanNum = new Random();
            return (PossibleNames[RanNum.Next(0, PossibleNames.Length - 1)]);
        }
        private static String FirstName()
        {
            //male names
            String[] PossibleMale = { "Schuyler", "Anthony", "Nick", "Steve", "Matt", "Jack", "Jim", "Bob", "Bill", "Joel","Cave","Chuck","Jon","Nikoli","Sheldon"};
            //girl names
            String[] PossibleFemale = { "Heather", "Hannah", "GLaDOS", "Jill", "Sarah", "Caroline", "Lauren", "Amanda","Kailey","Sally","Sierra","Cortana","Michelle","Sue"};
            Random RanNum = new Random();
            //randomly make male or female
            if(RanNum.Next(0,1) == 0)
                return (PossibleFemale[RanNum.Next(0, PossibleFemale.Length - 1)]);
            else
                return (PossibleMale[RanNum.Next(0, PossibleMale.Length - 1)]);
        }
        public static String MakeName()
        {
            return (FirstName() + " " + LastNames());
        }
        //probability model to generate the click populations
        public static Clicks AllocateClick()
        {
            Random RanNum = new Random();
            double Probability=RanNum.NextDouble();
            if(Probability < 0.10)
                return(new Staff());
            else if ((Probability >= .10) && (Probability < 0.60))
                return (new Average());
            else if ((Probability >= .60) && (Probability < 0.80))
                return (new Nerd());
            else
                return (new Jock());
        }
    }
}
