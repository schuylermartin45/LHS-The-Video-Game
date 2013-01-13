using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using CustomContentPipeline;
//Authors: Schuyler and Nick
namespace DoNotUse
{
    public class ScreenModel //can't extend the model class because it is a "sealed" type...weird
    {
        //reference to the game object; much easier to pass over the game object once rather than to constantly having to call various draw methods
        //THIS IS A REFERENCE NOT A VALUE; it is of no use to us to declare a new game object everytime an object is created;
        //CHANGES MADE TO VALUES WILL CHANGE THEM IN THE GAME!!!!!!!!!!!
        //ALSO: because this is a reference and not a copy of the original game values, the values returned from this object should correspond to the in-game data
        protected Game1 GameRef;
        //enumerated type that represents the collision possibilities 
        public enum ObjectTypes { Sphere = 1, Box, Frustum, Ray, Plane, Room };
        public ObjectTypes CollisionType;
        public Vector3 Position;
        public Model MyModel;
        //public float aspectRatio;
        private float scale = 45.0f;
        private float modelRotation = 0.0f;
        //An easy class reference to the content processor
        public BoundingBox LocalBox;
        //-------------------------------------------------------------------
        //ABOUT TO DO SOMETHING CRAZY!!!!!!
        //*In an attempt to reduce future processing woes*
        //The "moveable" boolean will determine if a model can be moved
        //IF A MODEL IS NOT MOVEABLE, it should NOT allocate space in memory for the additional vectors/Bounding box
        public bool Moveable { get; set; }
        //whethor or not the object can be picked up
        //*******PICKUPABLE OBJECTS CAN BE MOVED BUT NOT ALL MOVEABLE OBJECTS CAN BE PICKED UP******
        public bool PickUpable { get; set; }
        //min and max vectors 
        protected Vector3 MinVect;
        protected Vector3 MaxVect;
        //box that will be used when a model moves...preserves the original state just in case it is needed....
        public BoundingBox MovingBox;
        //a vector to store velocity...may end up using data from an out-side physics class...at least it can be saved easily!
        public Vector3 Velocity;
        //to be used when a room model is loaded to store the locations/planes associated with a given room model
        //***ONLY USED/ALLOCATED WITH ROOMS
        public List<Plane> ListOfWalls;
        //lots to process; should not be created every time an object is drawn
        BoundingBoxBuffers buffers;
        //default constructor...do work here
        public ScreenModel()
        {
            //do something....here
        }
        //defaulted moveable object, no velocity
        //overload that instantiates the BoundingBox; MUST USE COLLISION PROCESSOR
        public ScreenModel(Game1 GameRefArg, Model SentModel, Vector3 ModelPosition)
        {
            GameRef = GameRefArg;
            Position = ModelPosition;
            MyModel = SentModel;
            //Gets the object type attribute from the 
            List<object> DataFromProcessor=(List<object>)MyModel.Tag;
            CollisionType = (ObjectTypes)DataFromProcessor[0];
            //only allocates space when the bounding box must be checked;
            //otherwise, then use the sphere to check
            if (CollisionType == ObjectTypes.Box)
            {
                //gets box data from the content 
                //Does not check for exception in this case, because this method should be used on models that go through the processor
                BoundingBox Temp = (BoundingBox)DataFromProcessor[1];
                //adds the Position Vector to the Vectors that control the shape of the bounding box
                //-----> because Vectors aren't as "free floating" in programing as they are in math
                LocalBox = new BoundingBox(Temp.Min + Position, Temp.Max + Position);
                //allocates current data from the default spot...but these values will change based on a direction/velocity vector
                MinVect = LocalBox.Min;
                MaxVect = LocalBox.Max;
                MovingBox = new BoundingBox(MinVect, MaxVect);
            }
            //case for if this model is a room
            if (CollisionType == ObjectTypes.Room)
            {
                ListOfWalls = (List<Plane>)DataFromProcessor[1];
            }
            //allocates the other related objects if the bool is set to true
            Moveable = true;
            //Can be picked up by the user
            PickUpable = true;
            //default velocity as a unit vector...Remember: an object stays at rest until it is acted upon by another force!
            Velocity = Vector3.Zero;
            //used so any object can have it's bounding box drawn...should be removed later if memory needs to be saved
            buffers = new BoundingBoxBuffers(this.MovingBox, GameRef);
        }

        //overload that instantiates the BoundingBox; MUST USE COLLISION PROCESSOR
        public ScreenModel(Game1 GameRefArg, Model SentModel, Vector3 ModelPosition, bool MoveableTrue, bool PickUpTrue)
        {
            GameRef = GameRefArg;
            Position = ModelPosition;
            MyModel = SentModel;
            //aspect ratio comes from the game's aspect ratio value
            //aspectRatio = GameRef.MyPerson.aspectRatio;
            //gets box data from the content 
            //Does not check for exception in this case, because this method should be used on models that go through the processor
            List<object> DataFromProcessor = (List<object>)MyModel.Tag;
            CollisionType = (ObjectTypes)DataFromProcessor[0];
            if (CollisionType == ObjectTypes.Box)
            {
                BoundingBox Temp = (BoundingBox)DataFromProcessor[1];
                //adds the Position Vector to the Vectors that control the shape of the bounding box
                //-----> because Vectors aren't as "free floating" in programing as they are in math
                LocalBox = new BoundingBox(Temp.Min + Position, Temp.Max + Position);
                //allocates current data from the default spot...but these values will change based on a direction/velocity vector
                MinVect = LocalBox.Min;
                MaxVect = LocalBox.Max;
                MovingBox = new BoundingBox(MinVect, MaxVect);
            }
            //for room collisions
            if (CollisionType == ObjectTypes.Room)
            {
                ListOfWalls = (List<Plane>)DataFromProcessor[1];
            }
            //allocates the other related objects if the bool is set to true
            Moveable = MoveableTrue;
            PickUpable = PickUpTrue;
            if (Moveable)
            {
                //default velocity as a unit vector...Remember: an object stays at rest until it is acted upon by another force!
                Velocity = Vector3.Zero;
            }
            //safely defaults the value of pickupable to false if the object is not moveable, regaurdless of the method input
            if(!Moveable)
            {
                PickUpable = false;
            }
            //used so any object can have it's bounding box drawn...should be removed later if memory needs to be saved
            buffers = new BoundingBoxBuffers(this.MovingBox, GameRef);
        }
        //gives us the choice to make a model with a starting velocity
        //Defaulted as a moving object, because it has velocity
        public ScreenModel(Game1 GameRefArg,Model SentModel, Vector3 ModelPosition, Vector3 VelocityArg, bool PickUpTrue)
        {
            GameRef = GameRefArg;
            Position = ModelPosition;
            MyModel = SentModel;
            //Gets the object type attribute from the 
            List<object> DataFromProcessor=(List<object>)MyModel.Tag;
            CollisionType = (ObjectTypes)DataFromProcessor[0];
            //only allocates space when the bounding box must be checked;
            //otherwise, then use the sphere to check
            if (CollisionType == ObjectTypes.Box)
            {
                //Does not check for exception in this case, because this method should be used on models that go through the processor
                BoundingBox Temp = (BoundingBox)DataFromProcessor[1];
                //adds the Position Vector to the Vectors that control the shape of the bounding box
                //-----> because Vectors aren't as "free floating" in programing as they are in math
                LocalBox = new BoundingBox(Temp.Min + Position, Temp.Max + Position);
                //allocates current data from the default spot...but these values will change based on a direction/velocity vector
                MinVect = LocalBox.Min;
                MaxVect = LocalBox.Max;
                MovingBox = new BoundingBox(MinVect, MaxVect);
            }
            //for room collisions
            if (CollisionType == ObjectTypes.Room)
            {
                ListOfWalls = (List<Plane>)DataFromProcessor[1];
            }
            //allocates the other related objects if the bool is set to true
            Moveable = true;
            //object can be picked up
            PickUpable = PickUpTrue;
            //default velocity as a unit vector...Remember: an object stays at rest until it is acted upon by another force!
            Velocity = VelocityArg;
            //used so any object can have it's bounding box drawn...should be removed later if memory needs to be saved
            buffers = new BoundingBoxBuffers(this.MovingBox,GameRef);
        }

        public void Update()
        {
            Matrix[] transforms = new Matrix[MyModel.Bones.Count];
            MyModel.CopyAbsoluteBoneTransformsTo(transforms);
            
            foreach (ModelMesh mesh in MyModel.Meshes)
            {
                foreach (BasicEffect effects in mesh.Effects)
                {
                    effects.EnableDefaultLighting();
                    effects.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(Position);
                    effects.View = Matrix.CreateLookAt(GameRef.MyPerson.CameraPosition, new Vector3(GameRef.MyPerson.CameraPosition.X + GameRef.MyPerson.CameraAngle3.X, GameRef.MyPerson.CameraPosition.Y + GameRef.MyPerson.CameraAngle3.Y, GameRef.MyPerson.CameraPosition.Z + GameRef.MyPerson.CameraAngle3.Z), Vector3.Up);
                    effects.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(scale), GameRef.AspectRatio,GameRef.FrontPlane,GameRef.FarPlane);
                }
                mesh.Draw();
            }
        }
        //------------------------------------------------------------------------
        //Methods that provide support for collision checking
        //------------------------------------------------------------------------
        //sets the velocity to one or zero; hopefully makes it easier to start/stop and object
        public void ZeroVelocity()
        {
            Velocity = Vector3.Zero;
        }
        public void UnitVelocity()
        {
            Velocity = Vector3.One;
        }
        //reverse current velocity
        public void ReverseVelocity()
        {
            Velocity *= -1f;
        }
        //returns reverse velocity
        public Vector3 ReturnReverseVelocity()
        {
            return(-1f*Velocity);
        }
        //changes just the position of the box based on velocity
        public void MoveModel()
        {
            Position += Velocity;
        }
        private void MoveModel(Vector3 ChangeVect)
        {
            Position += ChangeVect;
        }
        //changes bounding box and vectors based on the velocity
        public void MoveModelBox()
        {
            //only if object is moveable
            if (Moveable)
            {
                MoveModel();
                if (CollisionType == ObjectTypes.Box)
                {
                    //changes vector values
                    MinVect += Velocity;
                    MaxVect += Velocity;
                    //only moves model box if the object is to be checked with a box
                    MovingBox = new BoundingBox(MinVect, MaxVect);
                }
            }
        }
        //use another vector...in theory should be able to be used to rotate the object + keep the bounding box orriented correctly
        public void MoveModelBox(Vector3 ChangeVect)
        {
            //only if object is moveable
            if (Moveable)
            {
                MoveModel(ChangeVect);
                if (CollisionType == ObjectTypes.Box)
                {
                    MinVect += ChangeVect;
                    MaxVect += ChangeVect;
                    MovingBox = new BoundingBox(MinVect, MaxVect);
                }
            }
        }
        //used with the collision class w/user input (does not move the model with velocity; compounds velocity
        protected void MoveModelBox(Vector3 ChangeVect,bool MoveWithUserInput)
        {
            //only if object is moveable
            if (Moveable)
            {
                //MoveModel();
                //changes vector values
                if (CollisionType == ObjectTypes.Box)
                {
                    MinVect += Velocity;
                    MaxVect += Velocity;
                    MovingBox = new BoundingBox(MinVect, MaxVect);
                }
            }
        }

        //experimental check system that uses a series of bounding boxes of 
        public bool CheckBoundingSpheres(BoundingSphere CheckArg)
        {
            for (int cntr = 0; cntr < MyModel.Meshes.Count; cntr++)
            {
                //creates a sphere for one mesh
                BoundingSphere Temp = this.MyModel.Meshes[cntr].BoundingSphere;
                //adjusts to center of the model
                Temp.Center += this.Position;
                if(CheckArg.Intersects(Temp))
                {
                    //intersection confimerd, stop checks and prevent model from moving in outer code
                    return(true);
                }
            }
            //else, return false, no hit detected
            return(false);
        }
        //check with a bounding box
        public bool CheckBoundingSpheres(BoundingBox CheckArg)
        {
            for (int cntr = 0; cntr < MyModel.Meshes.Count; cntr++)
            {
                //creates a sphere for one mesh
                BoundingSphere Temp = this.MyModel.Meshes[cntr].BoundingSphere;
                //adjusts to center of the model
                Temp.Center += this.Position;
                if (CheckArg.Intersects(Temp))
                {
                    //intersection confimerd, stop checks and prevent model from moving in outer code
                    return (true);
                }
            }
            //else, return false, no hit detected
            return (false);
        }
        //for plane collisions, i.e. walls
        public bool CheckBoundingSpheres(Plane CheckArg)
        {
            for (int cntr = 0; cntr < MyModel.Meshes.Count; cntr++)
            {
                //creates a sphere for one mesh
                BoundingSphere Temp = this.MyModel.Meshes[cntr].BoundingSphere;
                //adjusts to center of the model
                Temp.Center += this.Position;
                if (CheckArg.Intersects(Temp) == PlaneIntersectionType.Intersecting)
                {
                    //intersection confimerd, stop checks and prevent model from moving in outer code
                    return (true);
                }
            }
            //else, return false, no hit detected
            return (false);
        }
        //merges these bounding spheres together to send over
        protected BoundingSphere GetBoundingSpheres()
        {
            BoundingSphere MergedSphere = new BoundingSphere();
            for (int cntr = 0; cntr < MyModel.Meshes.Count; cntr++)
            {
                //creates a sphere for one mesh
                BoundingSphere Temp = this.MyModel.Meshes[cntr].BoundingSphere;
                //adjusts to center of the model
                //Temp.Center += this.Position;
                //merges all spheres together
                MergedSphere = BoundingSphere.CreateMerged(MergedSphere, Temp);
            }
            //centers the sphere at the end
            MergedSphere.Center += this.Position;
            //return the collection of spheres
            return (MergedSphere);
        }
        //uses a series of checks to determine the best method to call for checking collisions, based on the collision type selected
        public bool CheckCollision(ScreenModel CheckArg)
        {
            bool Test = false;
            if (CheckArg.CollisionType == ObjectTypes.Box)
            {
                if (this.CollisionType == ObjectTypes.Box)
                {
                    Test = this.MovingBox.Intersects(CheckArg.MovingBox);
                }
                if (this.CollisionType == ObjectTypes.Sphere)
                {
                    Test = this.CheckBoundingSpheres(CheckArg.MovingBox);
                }
            }
            else if(CheckArg.CollisionType == ObjectTypes.Sphere)
            {
                if (this.CollisionType == ObjectTypes.Box)
                {
                    Test = this.MovingBox.Intersects(CheckArg.GetBoundingSpheres());
                }
                if (this.CollisionType == ObjectTypes.Sphere)
                {
                    Test = this.CheckBoundingSpheres(CheckArg.GetBoundingSpheres());
                }
            }
            else if (CheckArg.CollisionType == ObjectTypes.Room)
            {
                Test = this.CheckRoom(CheckArg);
            }
            return (Test);
        }
        //to check collisions of contained by wall
        private bool CheckRoom(ScreenModel CheckArg)
        {
            bool Test = false;
            for(int cntr=0; cntr<CheckArg.ListOfWalls.Count; cntr++)
            {
                if (this.CollisionType == ObjectTypes.Box)
                {
                    if (this.MovingBox.Intersects(CheckArg.ListOfWalls[cntr]) == PlaneIntersectionType.Intersecting)
                        Test = true;
                    //if result is true, break loop early, saves time
                    if (Test)
                        break;
                }
                if (this.CollisionType == ObjectTypes.Sphere)
                {
                    Test = this.CheckBoundingSpheres(CheckArg.ListOfWalls[cntr]);
                    if (Test)
                        break;
                }
                //oddly enough, two planes can't intersect each other...no intersect() overload
                /*
                if (this.CollisionType == ObjectTypes.Plane)
                {
                    for (int cntr2 = 0; cntr2 < CheckArg.ListOfWalls.Count; cntr2++)
                    {
                        if (this.ListOfWalls[cntr].Intersects(CheckArg.ListOfWalls[cntr2]) == PlaneIntersectionType.Intersecting)
                        {
                            this.ListOfWalls[cntr].Intersects(
                            Test = true;
                        }
                        if (Test)
                            break;
                    }
                    if (Test)
                        break;
                }*/
            }
            return(Test);
        }
        //-------------------------------------------------------------------------
        //Baby class that will help draw the bounding boxes, much better explanation than microsoft could give
        //source: http://www.roastedamoeba.com/blog/archive/2010/12/10/drawing-an-xna-model-bounding-box
        public class BoundingBoxBuffers
        {    
            public VertexBuffer Vertices;     
            public int VertexCount;     
            public IndexBuffer Indices;    
            public int PrimitiveCount;

            public BoundingBoxBuffers(BoundingBox boundingBox,Game1 GameRef)
            {
                //BoundingBoxBuffers boundingBoxBuffers = new BoundingBoxBuffers();
                this.PrimitiveCount = 24;
                this.VertexCount = 48;
                VertexBuffer vertexBuffer = new VertexBuffer(GameRef.GraphicsDevice,
                    typeof(VertexPositionColor), this.VertexCount,
                    BufferUsage.WriteOnly);
                List<VertexPositionColor> vertices = new List<VertexPositionColor>();
                const float ratio = 5.0f;
                Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0);
                Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0);
                Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio);
                Vector3[] corners = boundingBox.GetCorners();
                // Corner 1.     
                AddVertex(vertices, corners[0]);
                AddVertex(vertices, corners[0] + xOffset);
                AddVertex(vertices, corners[0]);
                AddVertex(vertices, corners[0] - yOffset);
                AddVertex(vertices, corners[0]);
                AddVertex(vertices, corners[0] - zOffset);
                // Corner 2.     
                AddVertex(vertices, corners[1]);
                AddVertex(vertices, corners[1] - xOffset);
                AddVertex(vertices, corners[1]);
                AddVertex(vertices, corners[1] - yOffset);
                AddVertex(vertices, corners[1]);
                AddVertex(vertices, corners[1] - zOffset);
                // Corner 3.  
                AddVertex(vertices, corners[2]);
                AddVertex(vertices, corners[2] - xOffset);
                AddVertex(vertices, corners[2]);
                AddVertex(vertices, corners[2] + yOffset);
                AddVertex(vertices, corners[2]);
                AddVertex(vertices, corners[2] - zOffset);
                // Corner 4.   
                AddVertex(vertices, corners[3]);
                AddVertex(vertices, corners[3] + xOffset);
                AddVertex(vertices, corners[3]);
                AddVertex(vertices, corners[3] + yOffset);
                AddVertex(vertices, corners[3]);
                AddVertex(vertices, corners[3] - zOffset);
                // Corner 5.    
                AddVertex(vertices, corners[4]);
                AddVertex(vertices, corners[4] + xOffset);
                AddVertex(vertices, corners[4]);
                AddVertex(vertices, corners[4] - yOffset);
                AddVertex(vertices, corners[4]);
                AddVertex(vertices, corners[4] + zOffset);
                // Corner 6.     
                AddVertex(vertices, corners[5]);
                AddVertex(vertices, corners[5] - xOffset);
                AddVertex(vertices, corners[5]);
                AddVertex(vertices, corners[5] - yOffset);
                AddVertex(vertices, corners[5]);
                AddVertex(vertices, corners[5] + zOffset);
                // Corner 7.   
                AddVertex(vertices, corners[6]);
                AddVertex(vertices, corners[6] - xOffset);
                AddVertex(vertices, corners[6]);
                AddVertex(vertices, corners[6] + yOffset);
                AddVertex(vertices, corners[6]);
                AddVertex(vertices, corners[6] + zOffset);
                // Corner 8.  
                AddVertex(vertices, corners[7]);
                AddVertex(vertices, corners[7] + xOffset);
                AddVertex(vertices, corners[7]);
                AddVertex(vertices, corners[7] + yOffset);
                AddVertex(vertices, corners[7]);
                AddVertex(vertices, corners[7] + zOffset);
                vertexBuffer.SetData(vertices.ToArray());
                this.Vertices = vertexBuffer;
                IndexBuffer indexBuffer = new IndexBuffer(GameRef.GraphicsDevice, IndexElementSize.SixteenBits, this.VertexCount,
                    BufferUsage.WriteOnly);
                indexBuffer.SetData(Enumerable.Range(0, this.VertexCount).Select(i => (short)i).ToArray());
                this.Indices = indexBuffer;
            }
            private void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
            {
                vertices.Add(new VertexPositionColor(position, Color.White));
            } 
        } 
        

        //methods to use for human testing
        public void DrawBoundingBox()
        {

            BasicEffect effects = new BasicEffect(GameRef.GraphicsDevice);

            BasicEffect lineEffect = new BasicEffect(GameRef.GraphicsDevice);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false; 
            lineEffect.VertexColorEnabled = true;


            GameRef.GraphicsDevice.SetVertexBuffer(buffers.Vertices);
            GameRef.GraphicsDevice.Indices = buffers.Indices;
            effects.World = Matrix.Identity;
            effects.View = Matrix.CreateLookAt(GameRef.MyPerson.CameraPosition, new Vector3(GameRef.MyPerson.CameraPosition.X + GameRef.MyPerson.CameraAngle3.X, GameRef.MyPerson.CameraPosition.Y + GameRef.MyPerson.CameraAngle3.Y, GameRef.MyPerson.CameraPosition.Z + GameRef.MyPerson.CameraAngle3.Z), Vector3.Up);
            effects.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(scale), GameRef.AspectRatio,GameRef.FrontPlane,GameRef.FarPlane);
            foreach (EffectPass pass in effects.CurrentTechnique.Passes)
            { 
                pass.Apply(); 

                //graphicsDevice.DrawIndexedPrimitives(,,,,,

                GameRef.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 
                    buffers.VertexCount, 0, buffers.PrimitiveCount);
            }          
        }
    }
}
