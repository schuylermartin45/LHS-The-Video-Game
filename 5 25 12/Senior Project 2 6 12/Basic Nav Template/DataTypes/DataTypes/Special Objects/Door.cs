using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DataTypes
{
    //extends the screen model object so that it can be rendered the same way as all other objects
    public class Door : ScreenModel //, IComparable
    {
        //bool that controls the opening and closing of the door
        public bool OpenTrue = false;
        //two strings that contain the connecting rooms-> to open the files needed
        //Door side that is "in" the room; opens into the room; does not stick out into hall
        public String InsideDoor = ""; //this should be the name of the file that this door is in
        //door facing the hall
        public String OutsideDoor = "";
        //two vectors that allow us to figure out which side of the door a person is on
        public Vector3 InnerVect;
        public Vector3 OutsideVect;
        private BoundingBox StoreBoundingBox;
        //how far the vectors are from the center point
        private const float PtDistFromDoor = 0.5f;
        private Boolean OpenOut = false;
        private List<ScreenModel> LocalModels = new List<ScreenModel>(0);
        private List<ScreenModel> ThisRoomModels = new List<ScreenModel>(0);
        //time-based event object that closes doors after a time-out
        private DoorTimerWrapper DoorCloser;
        //door oposite of this one
        public Door InverseDoor;
        //dummy constructor that just takes in a model position...for use in AI Classes
        public Door(Vector3 ModelPosition)
        {
            this.Position = ModelPosition;
        }
        //doors will be placed in 
        public Door(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, String OutDoor, String InDoor)
            : base(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, false, false, 0f, false)
        {
            //gets assigned when file is saved/opened...it is the room that this door is a part of
            //InsideDoor = "";//InDoor;
            //ActionString Used
            OutsideDoor = OutDoor;
            InsideDoor = InDoor;
            //calculates the 2 vectors
            CalculateTwoVectors();
        }
        public Door(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, String Action)
            : base(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, false, false, 0f, false)
        {
            //gets assigned when file is saved/opened...it is the room that this door is a part of
            //InsideDoor = "";//InDoor;
            //ActionString Used
            this.ActionString = Action;
            try
            {
                String Content = ActionString.Substring(ActionString.IndexOf("Door{") + 5, ActionString.IndexOf("}") - ActionString.IndexOf("Door{") - 5);
                InsideDoor = Content.Substring(Content.IndexOf("In:") + 3, Content.IndexOf(";", Content.IndexOf("In:")) - Content.IndexOf("In:") - 3);
                OutsideDoor = Content.Substring(Content.IndexOf("Out:") + 4, Content.IndexOf(";", Content.IndexOf("Out:")) - Content.IndexOf("Out:") - 4);
            }
            catch { }
            //calculates the 2 vectors
            CalculateTwoVectors();
            BoundingSetup();
        }
        //calculate the inner/outter door vectors
        private void CalculateTwoVectors()
        {
            //sorts through a bunch of meshpoints...like when calculating the bounding boxes
            foreach (ModelMesh mesh in this.MyModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // The stride is how big, in bytes, one vertex is in the vertex buffer
                    // We have to use this as we do not know the make up of the vertex
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    byte[] vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1);

                    // Find minimum and maximum xyz values for this mesh part
                    // We know the position will always be the first 3 float values of the vertex data
                    Vector3 vertPosition = new Vector3();
                    for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);
                    }
                    //transforms the vectors once calculated out
                    Matrix[] transforms = new Matrix[MyModel.Bones.Count];
                    MyModel.CopyAbsoluteBoneTransformsTo(transforms);
                    vertPosition = Vector3.Transform(vertPosition, transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position));
                    //logic that checks to see if the point calculated out is 
                    if (vertPosition.Y == this.Position.Y)
                    {
                        Vector3 Displacement = this.Position - vertPosition;
                        //if the distance is equal to one direction vector, 
                        if (Displacement == Vector3.Forward)
                        {
                            InnerVect = vertPosition;
                        }
                        else if (Displacement == Vector3.Backward)
                        {
                            OutsideVect = vertPosition;
                        }
                    }
                }
            }
        }
        private float CalculateDistanceFromUser(Vector3 CheckPt, CollisionCheckSystem User)
        {
            return (Vector3.Distance(User.Position, CheckPt));
        }
        public String GetUserRoomLocation(CollisionCheckSystem User)
        {
            /*
            Console.WriteLine("DoorPos: " + this.Position.ToString());
            Console.WriteLine("OutsideVect " + this.OutsideVect.ToString());
            Console.WriteLine("InnerVect " + this.InnerVect.ToString());
            Console.WriteLine("UserPos: " + User.Position.ToString());*/
            float InnerDist = CalculateDistanceFromUser(InnerVect, User);
            float OutsideDist = CalculateDistanceFromUser(OutsideVect, User);
            //Console.WriteLine("In Dist: " + InnerDist);
            //Console.WriteLine("Out Dist: " + OutsideDist);
            if (InnerDist < OutsideDist)
            {
                //assings value in the user object
                User.CurrentRoom = this.InsideDoor;
                return (this.InsideDoor);
            }
            else if (InnerDist == OutsideDist)
            {
                User.CurrentRoom = "Between " + this.InsideDoor + "," + this.OutsideDoor;
                return ("Between " + this.InsideDoor + "," + this.OutsideDoor);
            }
            else
            {
                User.CurrentRoom = this.OutsideDoor;
                return (this.OutsideDoor);
            }
        }
        //static method that takes a list of objects from a game and determines what room the computer
        //IS CALLED IN THE GAME"S UPDATE METHOD!!!
        /*
        public String WhereAmI(CollisionCheckSystem User)
        {
            List<Door> TempListOfDoors = new List<Door>();
            //goes through a list of doors in a room and determines 
            for (int cntr = 0; cntr < User.VisibleObjects.Count; cntr++)
            {
                //creates a temporary list of closed doors
                if (User.VisibleObjects[cntr].GetType() == typeof(Door))
                {
                    if(((Door)User.VisibleObjects[cntr]).OpenTrue==false)
                        TempListOfDoors.Add((Door)User.VisibleObjects[cntr]);
                }
            }
            //shortens up the inside string of this door 

            //goes through the list of doors and builds a string that is composed of similar characters 
            foreach (Door ADoor in TempListOfDoors)
            {
                String RoomName = "";
                //pulls out just the name of the room
                if (ADoor.InsideDoor.Contains('#'))
                    RoomName = ADoor.InsideDoor.Substring(0, ADoor.InsideDoor.IndexOf("#"));
                
            }
        //SCHUYLER IS SOOOO MUCH COOLER THAN ME
            //return the room you are in
            return (GetUserRoomLocation);
        }*/
        //in forgeworld, this method can be called to require the map editor to change the 
        public override void SpecialObjForgeAction()
        {
            try
            {
                //the map edditor can now change this property whenever this method is called in forge mode
                //Console.Write("Enter the path and name of the room that is outside this room: ");
                this.OutsideDoor = (String)Console.ReadLine();
                //Console.WriteLine("The new outside room to this door is: " + OutsideDoor);
            }
            catch (Exception)
            {
            }
        }
        //static method that returns an integer value of the room
        public static int ReturnRoomNum(String RoomName)
        {
            int RoomNum = 0;
            //searches through the list of characters and creates a list of ints
            for (int cntr = 0; cntr < RoomName.Length; cntr++)
            {
                char TempChar = RoomName[cntr];
                try
                {
                    int ToAdd = int.Parse(TempChar.ToString());
                    //if this isn't caught by the exception, then continue to add
                    RoomNum *= 10;
                    RoomNum += ToAdd;
                }
                catch (FormatException)
                {
                }
            }
            return (RoomNum);
        }
        //animation for the doors to open; also moves bounding box
        public override void Animation()
        {
            //turns the door open...figure out how to animate objects in xna
            /*
            this.modelRotation.X = 0.5f;
            for (int cntr = 0; cntr < 100; cntr++)
            {
            }*/
        }
        //LOLZ I'M JUST TYPING RANDOM THINGS THIS ISN"T A REAL PROGRAM, I DONT KNOW WHAT IM DOING.
        //method to be called if the user has performed an action
        public void Action(ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics,CollisionCheckSystem User)
        {
            if (ThisRoomModels.Count == 0)
                foreach(ScreenModel ThisModel in models)
                    ThisRoomModels.Add(ThisModel);
            //figures out if the door should be opened or closed
            if (OpenTrue)
                if ((GetBoundingBoxSurroundings(InverseDoor).Contains(User.Position)) && (GetBoundingBoxSurroundings(this).Contains(User.Position)))
                    if (DataValues.VectorDistance(GetBoundingBoxSurroundings(InverseDoor).HalfExtent) > DataValues.VectorDistance(GetBoundingBoxSurroundings(this).HalfExtent))
                        CloseDoor(Content, models, graphics, User.Position);
                    else
                        InverseDoor.CloseDoor(Content, models, graphics, User.Position);
                else
                    if (GetBoundingBoxSurroundings(InverseDoor).Contains(User.Position))
                        InverseDoor.CloseDoor(Content, models, graphics, User.Position);
                    else
                        CloseDoor(Content, models, graphics, User.Position);
            else
            {
                OpenDoor(Content, models, graphics, User.Position);
                //starts/assigns timer to close door
                DoorCloser = new DoorTimerWrapper(this, Content, models, graphics, User);
                DoorCloser.Start();
            }
        }
        public void OpenDoor(ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics, Vector3 PersonPosition)
        {
            //stores the location and dimensions of this object, so it can 
            this.StoreBoundingBox = this.GetBoundingBoxes();
            float Distance = DataValues.VectorDistance(PersonPosition - this.Position);
            this.Position -= Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            if (Distance > DataValues.VectorDistance(PersonPosition - this.Position))
                this.modelRotation.Y+=(float)Math.PI;
            this.Position += Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));

            LoadNewRoom(Content, models, graphics);    

            OpenOut = true;

            this.Position -= Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            this.modelRotation.Y += (float)Math.PI / 2;

            //makes sure that the door is correctly oriented.
            InverseDoor.Position = this.Position;
            InverseDoor.modelRotation = this.modelRotation;
            //InverseDoor.Position -= Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.GetBoundingBoxes().Max.X - this.GetBoundingBoxes().Min.X, 2) + Math.Pow(this.GetBoundingBoxes().Max.Z - this.GetBoundingBoxes().Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.GetBoundingBoxes().Max.X - this.GetBoundingBoxes().Min.X, 2) + Math.Pow(this.GetBoundingBoxes().Max.Z - this.GetBoundingBoxes().Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            //InverseDoor.modelRotation.Y += (float)Math.PI / 2;
            InverseDoor.InverseDoor = this;
            InverseDoor.OpenOut = !OpenOut;
            InverseDoor.OutsideDoor = this.InsideDoor;
            InverseDoor.StoreBoundingBox = this.StoreBoundingBox;
            OpenTrue = true;
        }
        public void CloseDoor(ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics,Vector3 PersonPosition)
        {
            if (OpenOut)
            {
                this.modelRotation.Y -= (float)Math.PI / 2;
                this.Position += Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            }
            else
            {
                this.modelRotation.Y += (float)Math.PI / 2;
                this.Position -= Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            }

            OpenTrue = false;

            //removes all the models from the game that should be removed when the door is closed
            DeleteRoom(this, models);       
        }
        
        //pringles
        //overload that handles the case of walking past the door
        /*
        public void CloseDoor(ContentManager Content,GraphicsDeviceManager graphics, Vector3 PersonPosition,String UserRoomLocation)
        {
            if (OpenOut)
            {
                this.modelRotation.Y -= (float)Math.PI / 2;
                this.Position += Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            }
            else
            {
                this.modelRotation.Y += (float)Math.PI / 2;
                this.Position -= Vector3.Transform(new Vector3((float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2)), 0, (float)Math.Sqrt(Math.Pow(this.StoreBoundingBox.Max.X - this.StoreBoundingBox.Min.X, 2) + Math.Pow(this.StoreBoundingBox.Max.Z - this.StoreBoundingBox.Min.Z, 2))) / 2, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            }
            OpenTrue = false;
            //decides which room to delete models from
            if(UserRoomLocation.Contains(RoomNameEdit(this.InsideDoor)))
            {
                //removes all the models from the game that should be removed when the door is closed
                DeleteRoom(this, this.ThisRoomModels);
            }
            else if(UserRoomLocation.Contains(RoomNameEdit(this.OutsideDoor)))
            {
                DeleteRoom(this.InverseDoor,this.InverseDoor.ThisRoomModels);
            }
        }*/

        //kill "extensions" from the door strings
        public String RoomNameEdit(String RoomDoorString)
        {
            if (RoomDoorString.Contains('#'))
                return(RoomDoorString.Substring(0,RoomDoorString.IndexOf("#")));
            else
                return(RoomDoorString);
        }

        public static OrientedBoundingBox GetBoundingBoxSurroundings(Door ThisDoor)
        {
            //makes a bounding box around the room
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (ScreenModel ThisModel in ThisDoor.ThisRoomModels)
                if (ThisModel != ThisDoor)
                    if(ThisModel.GetType() != typeof(Door))
                        foreach (OrientedBoundingBox Box in ThisModel.MovingBoxes)
                        {
                            foreach (Vector3 Corner in Box.GetRotatedCorners())
                            {
                                Max = Vector3.Max(Corner, Max);
                                Min = Vector3.Min(Corner, Min);
                            }
                        }
            return (new OrientedBoundingBox((Max + Min) / 2, (Max - Min) / 2, Quaternion.Identity));
        }
        public static OrientedBoundingBox GetBoundingBoxOther(Door ThisDoor)
        {
            //makes a bounding box around the room
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (ScreenModel ThisModel in ThisDoor.LocalModels)
                if (ThisModel != ThisDoor)
                    foreach (OrientedBoundingBox Box in ThisModel.MovingBoxes)
                    {
                        foreach (Vector3 Corner in Box.GetRotatedCorners())
                        {
                            Max = Vector3.Max(Corner, Max);
                            Min = Vector3.Min(Corner, Min);
                        }
                    }
            return (new OrientedBoundingBox((Max + Min) / 2, (Max - Min) / 2, Quaternion.Identity));
        }
        //find the center point of a room to spawn the character
        public static Vector3 FindCenterRoom(Door ThisDoor)
        {
            OrientedBoundingBox BoundBox = GetBoundingBoxOther(ThisDoor);
            return(BoundBox.Center);
        }

        //M Y PROGRAM SUCKS
        public static void DeleteRoom(Door ThisDoor, List<ScreenModel> models)
        {
            try
            {

                for (int x = 0; x < ThisDoor.LocalModels.Count; x++)
                {
                    if ((ThisDoor.LocalModels[x].GetType() == typeof(Door)) && (((Door)ThisDoor.LocalModels[x]).OpenTrue) && ((Door)ThisDoor.LocalModels[x] != ThisDoor.InverseDoor))
                        DeleteRoom((Door)ThisDoor.LocalModels[x], models);
                    models.Remove(ThisDoor.LocalModels[x]);
                }
            }
            catch
            {
                DeleteRoom(ThisDoor, models);
            }
        }
        public void LoadNewRoom(ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics)
        {
            if (this.ThisRoomModels.Count == 0)
                foreach (ScreenModel ThisModel in models)
                    this.ThisRoomModels.Add(ThisModel);

            //extracts the rooms from the file
            LocalModels = DataValues.LoadListOfModel(this.OutsideDoor, Content, graphics);
            
            //finds the door in the other room that matches this door, so that it can be used to make the other room relative to this one
            InverseDoor=this;
            foreach (ScreenModel model in LocalModels)
                if (model.GetType() == typeof(Door))
                {
                    ((Door)model).ThisRoomModels.Clear();
                    foreach (ScreenModel ThisModel in LocalModels)
                        ((Door)model).ThisRoomModels.Add(ThisModel);
                    if ((((Door)model).OutsideDoor.Equals(this.InsideDoor)) || (((Door)model).InsideDoor.Equals(this.OutsideDoor)) || InverseDoor==this)
                        InverseDoor = (Door)model;
                }

            //finds the relative difference in rotation and position
            Vector3 RoomRotation = this.modelRotation - InverseDoor.modelRotation+new Vector3(0,(float)Math.PI,0);
            Vector3 RoomPosition = this.Position - InverseDoor.Position;

            //adds the rooms to delete when the inverse door is closed(because it will be set to open when the room is created)
            InverseDoor.OpenTrue = true;

            if (InverseDoor != this)
            {
                InverseDoor.LocalModels.Clear();
                foreach (ScreenModel ThisModel in ThisRoomModels)
                    InverseDoor.LocalModels.Add(ThisModel);
            }

            //creates the transformation so that all objects are rotated about the door's position
            Matrix Transforms = Matrix.CreateRotationX(RoomRotation.X) * Matrix.CreateRotationY(RoomRotation.Y) * Matrix.CreateRotationZ(RoomRotation.Z) * Matrix.CreateTranslation(this.Position);

            //physically rotates the objects, and adds them to the models list
            foreach (ScreenModel model in LocalModels)
            {
                model.Position += RoomPosition;
                model.Position = Vector3.Transform(model.Position, Transforms);
                model.modelRotation += RoomRotation;
                models.Add(model);
                model.BoundingSetup();
            }

            //repositions all objects so that they are aligned to the door
            RoomPosition = this.Position - InverseDoor.Position;
            foreach (ScreenModel model in LocalModels)
                model.Position += RoomPosition;
            
        }
        public void UpdateActionString()
        {
            this.ActionString = "Door{In:" + InsideDoor + ";Out:" + OutsideDoor + ";}";
        }
        /*
        //overloads the compare to method
        public int CompareTo(object Object)
        {
            //if these are the same object (same memory reference) then return 0
            if (Object == this)
            {
                return (0);
            }
            //if not, return -1 as I don't care if it's located or not
            return (-1);
        }*/
        //to generically find a door
        public static bool FindTypeOfDoor(ScreenModel Test)
        {
            if (Test.GetType() == typeof(Door))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }


        public static Vector3 FindCenterRoom(ScreenModel screenModel)
        {
            //throw new NotImplementedException();
            if (screenModel != null)
                return (screenModel.Position);
            else
                return (Vector3.Zero);
        }
    }
}
