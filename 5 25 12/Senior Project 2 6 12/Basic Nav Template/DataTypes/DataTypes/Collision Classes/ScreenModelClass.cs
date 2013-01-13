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
using System.Collections;

namespace DataTypes
{
    public class ScreenModel : IComparable
    {
        public String Name = "";
        public String ModelName = "";
        public Vector3 Position;
        public String ActionString = "";
        //bounce is a percentage of how much force will be applied when an object is hit. (1 is super bouncy, 0 is not bouncy at all, 1.1 will make it get even bouncier as it hits more stuff)
        public float Bounce = 0f; 
        
        //private bool isColliding=false;
        /*Scuyler's variables*/
        //protected Game1 GameRef;//GAME REF IS BAD!!!
        //uses a device manager instead
        public GraphicsDevice device;
        //enumerated type that represents the collision possibilities 
        public enum ObjectTypes { Sphere = 1, Box, Frustum, Ray, Plane, Room };
        public ObjectTypes CollisionType;
        //the fbx file being displayed by the model
        //An easy class reference to the content processor
        //publicOrientedBoundingBoxLocalBox;
        //initial position of the object
        //public Vector3 LocalPos;
        //-------------------------------------------------------------------
        //ABOUT TO DO SOMETHING CRAZY!!!!!!
        //*In an attempt to reduce future processing woes*
        //The "moveable" boolean will determine if a model can be moved
        //IF A MODEL IS NOT MOVEABLE, it should NOT allocate space in memory for the additional vectors/Bounding box
        public bool Moveable { get; set; }
        //whethor or not the object can be picked up
        //*******PICKUPABLE OBJECTS CAN BE MOVED BUT NOT ALL MOVEABLE OBJECTS CAN BE PICKED UP******
        public bool PickUpable { get; set; }
        private Vector3 LastRotation;
        private Vector3 LastPosition;
        public BoundingSphere Sphere = new BoundingSphere();
        //min and max vectors 
        //protected Vector3 MinVect;
        //protected Vector3 MaxVect;
        //box that will be used when a model moves...preserves the original state just in case it is needed....
        //public BoundingBox MovingBox;

        //each model has an array of of bounding boxes; one per each mesh
        public List<OrientedBoundingBox> MovingBoxes;

        //a vector to store velocity...may end up using data from an out-side physics class...at least it can be saved easily!
        public Vector3 Velocity;

        //to be used when a room model is loaded to store the locations/planes associated with a given room model
        //***ONLY USED/ALLOCATED WITH ROOMS
        public List<Plane> ListOfWalls;
        //bool that holds the value of whether or not the object is currently falling; SHOULD TURN TRUE BY AN OUTSIDE "EVENT"; such as moving the object
        //we can't afford having every object checking for every other all the time; though there should be a way around this problem somewhere
        public bool IsCurrentlyFalling = false;
        //allerts us when the initial fall cylce has started
        public int FallCntr = 0;
        //similar bool to tell the object that it is currently being held
        public bool IsBeingHeld = false;
        //lots to process; should not be created every time an object is drawn
        BoundingBoxBuffers[] buffers;
        /*End Schuyler's variables*/
        public bool freeFall = false;
        public Model MyModel;

        public Texture2D texture;
        //aspect ratio
        public float aspectRatio;
        public bool istextured = false;
        //scaling of the object
        public float scale = 45.0f;
        public Boolean IsLevitating = false;
        //How the object is rotated
        public Vector3 modelRotation = Vector3.Zero;
        public Effect effect;

        //initially declaring the model in multiple formats
        public ScreenModel()
        {
            this.Position = Vector3.Zero;
        }

        public ScreenModel(Vector3 ModelPosition)
        {
            this.Position = ModelPosition;
        }

        public ScreenModel(Model SentModel)
        {
            this.Position = Vector3.Zero;
            this.MyModel = SentModel;
            BoundingSetup();
        }
        public ScreenModel(Model SentModel, Vector3 ModelPosition, GraphicsDevice device)
        {
            this.Position = ModelPosition;
            this.MyModel = SentModel;
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            BoundingSetup();
        }
        public ScreenModel(String Name, Model SentModel, Vector3 ModelPosition, GraphicsDevice device)
        {
            this.Name = Name;
            this.Position = ModelPosition;
            this.MyModel = SentModel;
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            BoundingSetup();
        }
        public ScreenModel(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, GraphicsDevice device)
        {
            this.Name = Name;
            this.Position = ModelPosition;
            this.ModelName = ModelName;
            this.MyModel = SentModel; //Content.Load<Model>(@"Models/"ModelName);
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            BoundingSetup();
        }
        public ScreenModel(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device)
        {
            this.Name = Name;
            this.Position = ModelPosition;
            this.ModelName = ModelName;
            this.MyModel = SentModel; //Content.Load<Model>(@"Models/"ModelName);
            this.modelRotation = ModelRotation;
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            BoundingSetup();
        }
        public ScreenModel(Model SentModel, Vector3 ModelPosition, float ModelScale, Vector3 Rotation, GraphicsDevice device)
        {
            this.Position = ModelPosition;
            this.MyModel = SentModel;
            this.scale = ModelScale;
            this.modelRotation = Rotation;
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            BoundingSetup();
        }

        public ScreenModel(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup)
        {
            this.Name = Name;
            this.Position = ModelPosition;
            this.ModelName = ModelName;
            this.MyModel = SentModel;
            this.modelRotation = ModelRotation;
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            this.Moveable = moveable;
            this.PickUpable = pickup;
            BoundingSetup();
        }

        /* - This is the one we use - */
        public ScreenModel(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float Bounciness, Boolean Levitating)
        {
            this.Name = Name;
            this.Position = ModelPosition;
            this.ModelName = ModelName;
            this.MyModel = SentModel;
            this.modelRotation = ModelRotation;
            this.device = device;
            this.aspectRatio = device.Viewport.AspectRatio;
            this.Moveable = moveable;
            this.PickUpable = pickup;
            this.Bounce = Bounciness;
            this.IsLevitating = Levitating;
            BoundingSetup();
        }

        public void LoadModel(Model SentModel)
        {
            this.MyModel = SentModel;
        }
        public void LoadGraphicsDevice(GraphicsDevice device)
        {
            this.device=device;
        }
        //sends over the camera, direction vector (for drawing models) 
        //and a list of objects nearby to check collisions (may want some way to create this list automatically)
        public virtual void Update(BasicEffect effects, List<Vector3> Lights, bool NV)
        {
            if ((modelRotation.X < 0) || (modelRotation.X > Math.PI * 2))
                modelRotation.X = modelRotation.X % ((float)Math.PI * 2);
            if (modelRotation.X < 0)
                modelRotation.X += ((float)Math.PI * 2);
            if ((modelRotation.Y < 0) || (modelRotation.Y > Math.PI * 2))
                modelRotation.Y = modelRotation.Y % ((float)Math.PI * 2);
            if (modelRotation.Y < 0)
                modelRotation.Y += ((float)Math.PI * 2);
            if ((modelRotation.Z < 0) || (modelRotation.Z > Math.PI * 2))
                modelRotation.Z = modelRotation.Z % ((float)Math.PI * 2);
            if (modelRotation.Z < 0)
                modelRotation.Z += ((float)Math.PI * 2);

            if ((modelRotation != LastRotation) || (Position != LastPosition))
            {
                for (int x = 0; x < MovingBoxes.Count; x++)
                    this.CalculateBoundingBox();
                // MovingBoxes[x].Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position));
                LastRotation = modelRotation;
                LastPosition = Position;
            }
            if (this.GetType() == typeof(Wall))
            {
                ((Wall)this).Update(effects);
                return;
            }
            //skips all these checks if the object is not moveable...cuts down on checks?

            /*
            if (this.Moveable)
            {
                //runs collision checks when the object has a non-zero velocity; 
                //when an object is pushed by another object it should add the other object's velocity * the coeffecient of friction
                if (Velocity != Vector3.Zero)
                {
                    ScreenModel CheckArg = this.CheckCollision(VisibleObjects);
                    if (CheckArg != null)
                    {
                        //add velocities together
                        if (CheckArg.Moveable == true)
                        {
                            //velocity of object being hit changes
                            CheckArg.Velocity = this.Velocity * DataValues.CoefficientFriction;
                            //move the object
                            CheckArg.MoveModelBox();
                        }
                        //otherwise stop this object
                        else //if ((CheckArg.Moveable == false) || (CheckArg.Velocity == Vector3.Zero))
                        {
                            this.Velocity = Vector3.Zero;
                        }
                    }
                }
            }*/

            //moves object based on velocity
            //this.MoveModelBox();

            float[] ClosestDistance = { float.MaxValue, float.MaxValue, float.MaxValue };
            Vector3[] ClosestLight = {Vector3.Zero,Vector3.Zero,Vector3.Zero};
            foreach (Vector3 ThisLight in Lights)
                if (DataValues.VectorDistance(this.Position - ThisLight) < ClosestDistance[0])
                {
                    ClosestDistance[0] = DataValues.VectorDistance(this.Position - ThisLight);
                    ClosestLight[0] = this.Position-ThisLight;
                }
                else if(DataValues.VectorDistance(this.Position - ThisLight) < ClosestDistance[1])
                {
                    ClosestDistance[1] = DataValues.VectorDistance(this.Position - ThisLight);
                    ClosestLight[1] = this.Position-ThisLight;
                }
                else if (DataValues.VectorDistance(this.Position - ThisLight) < ClosestDistance[2])
                {
                    ClosestDistance[2] = DataValues.VectorDistance(this.Position - ThisLight);
                    ClosestLight[2] = this.Position - ThisLight;
                }

            Matrix[] transforms = new Matrix[MyModel.Bones.Count];
            MyModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in MyModel.Meshes)
            {
                foreach (BasicEffect MeshEffects in mesh.Effects)
                {
                    MeshEffects.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position);
                    MeshEffects.View = effects.View;
                    MeshEffects.Projection = effects.Projection;
                    MeshEffects.AmbientLightColor = new Vector3(.2f, .2f, .2f);
                    //MeshEffects.DiffuseColor = new Vector3(.5f, .5f, .5f);
                    MeshEffects.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
                    MeshEffects.SpecularPower = 1;
                    MeshEffects.Alpha = 1.0f;
                    //float x = DateTime.Now.TimeOfDay.Seconds/60.0f;
                    //float y = DateTime.Now.TimeOfDay.Seconds/60.0f;
                    //float z = DateTime.Now.TimeOfDay.Seconds/60.0f;
                    //MeshEffects.EmissiveColor = new Vector3(x, x, 0);
                    
                    MeshEffects.LightingEnabled = true;
                    if (MeshEffects.LightingEnabled)
                    {
                        MeshEffects.DirectionalLight0.Enabled = true; // enable each light individually
                        if (MeshEffects.DirectionalLight0.Enabled)
                        {
                            // x direction
                            float intensity = ((330 - ClosestDistance[0]) / (300));
                            if (intensity < 0)
                                intensity = 0;
                            MeshEffects.DirectionalLight0.DiffuseColor = new Vector3(.5f, .5f, .5f); // range is 0 to 1
                            if (NV)
                                MeshEffects.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f); // range is 0 to 1
                            MeshEffects.DirectionalLight0.Direction = Vector3.Normalize(ClosestLight[0]) * intensity;
                            // points from the light to the origin of the scene
                            MeshEffects.DirectionalLight0.SpecularColor = Vector3.One;
                        }

                        MeshEffects.DirectionalLight1.Enabled = false;
                        if (MeshEffects.DirectionalLight1.Enabled)
                        {
                            // x direction
                            float intensity = ((330 - ClosestDistance[1]) / (300));
                            if (intensity < 0)
                                intensity = 0;
                            MeshEffects.DirectionalLight1.DiffuseColor = new Vector3(.5f, .5f, .5f); // range is 0 to 1
                            if (NV)
                                MeshEffects.DirectionalLight1.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f); // range is 0 to 1
                            MeshEffects.DirectionalLight1.Direction = Vector3.Normalize(ClosestLight[1]) * intensity;
                            // points from the light to the origin of the scene
                            MeshEffects.DirectionalLight1.SpecularColor = Vector3.One;
                        }

                        MeshEffects.DirectionalLight2.Enabled = false;
                        if (MeshEffects.DirectionalLight2.Enabled)
                        {
                            // x direction
                            float intensity = ((330 - ClosestDistance[2]) / (300));
                            if (intensity < 0)
                                intensity = 0;
                            MeshEffects.DirectionalLight2.DiffuseColor = new Vector3(.5f, .5f, .5f); // range is 0 to 1
                            if (NV)
                                MeshEffects.DirectionalLight2.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f); // range is 0 to 1
                            MeshEffects.DirectionalLight2.Direction = Vector3.Normalize(ClosestLight[2]) * intensity;
                            // points from the light to the origin of the scene
                            MeshEffects.DirectionalLight2.SpecularColor = Vector3.One;
                        }
                    }
                }
                mesh.Draw();
            }
            //for testing
            //DrawBoundingBox(camera, Angle, aspectRatio);
        }
        //-------------------------------------------------------------------------
        //Implementing "Gravity Class" *Schuyler, I moved the gravity to the move section
        /*
        public void Fall(List<ScreenModel> VisibleObjects)
        {
            //Console.WriteLine("Falling "+FallCntr);
            //local calculation done each time to keep track of the change that has been done (velocity*acceleration)
            float KeepTrackOfChange = 0f;
            if (FallCntr == 0)
                this.Velocity.Y += DataValues.gravity;
            //acceleration crashes the game....not sure why!!!!!!!!!...not very necessery though
            //increase velocity due to gravitational acceleration until terminal velocity is reached
            if ((this.Velocity.Y * DataValues.GravityAcceleration) > DataValues.TerminalVelocity)
            {
                this.Velocity.Y = (this.Velocity.Y>0)?this.Velocity.Y/DataValues.GravityAcceleration:this.Velocity.Y*DataValues.GravityAcceleration;
                if (this.Velocity.Y < .002f)
                    this.Velocity.Y *= -1;
                //KeepTrackOfChange = this.Velocity.Y;
            }
            //Console.WriteLine("Velocity Y: "+this.Velocity.Y);

            if (Position.Y > 0)//(this.GetBoundingSpheres().Radius))
            {
                Position.Y += KeepTrackOfChange;//this.Velocity.Y;
                FallCntr++;
            }

            /*
            if(this.CollisionType == ObjectTypes.Sphere)
            {
                //gravity moves object down
                if (Position.Y > 0)//(this.GetBoundingSpheres().Radius))
                {
                    Position.Y -= KeepTrackOfChange;//this.Velocity.Y;
                    FallCntr++;
                }
                //object has hit the ground or an object underneath it
                if ((CheckCollision(VisibleObjects) != null) || (Position.Y <= 0))
                {
                    //undoes the last iteration CORRECTLY NOW THAT THE ACCELERATION AFFECT HAS BEEN KEPT TRACK OF
                    Position.Y += KeepTrackOfChange;//this.Velocity.Y;
                    //resets the fall cntr
                    FallCntr = 0;
                    //Velocity is no longer effected by gravity
                    //this.Velocity.Y -= DataValues.gravity;
                    this.Velocity.Y *= Bounce * -1;
                    //object can no longer fall
                    if(this.Velocity.Y==0)
                        IsCurrentlyFalling = false;
                }
            }
            if(this.CollisionType == ObjectTypes.Box)
            {
                //gravity moves object down
                if (Position.Y > 0)//(this.GetBoundingSpheres().Radius))
                {
                    Position.Y -= KeepTrackOfChange;//this.Velocity.Y;
                    //moves bounding box
                    this.MoveModelBox(new Vector3(0f, -KeepTrackOfChange, 0f));
                    FallCntr++;
                }
                //object has hit the ground or an object underneath it
                if ((CheckCollision(VisibleObjects) != null)||(Position.Y<=0))
                {
                    //undoes the last iteration CORRECTLY NOW THAT THE ACCELERATION AFFECT HAS BEEN KEPT TRACK OF
                    Position.Y += KeepTrackOfChange;//this.Velocity.Y;
                    this.MoveModelBox(new Vector3(0f, KeepTrackOfChange, 0f));
                    //resets the fall cntr
                    FallCntr = 0;
                    //Velocity is no longer effected by gravity
                    //this.Velocity.Y -= DataValues.gravity;
                    this.Velocity.Y *= Bounce * -1;
                    //object can no longer fall
                    if (this.Velocity.Y == 0)
                        IsCurrentlyFalling = false;
                }
            }
            
        }*/

        //------------------------------------------------------------------------
        //Methods that provide support for collision checking
        //------------------------------------------------------------------------
        //sets the velocity to one or zero; hopefully makes it easier to start/stop and object
        public void BoundingSetup()
        {
            //Gets the object type attribute from the 
            List<object> DataFromProcessor = (List<object>)MyModel.Tag;
            CollisionType = (ObjectTypes)DataFromProcessor[0];
            //only allocates space when the bounding box must be checked;
            //otherwise, then use the sphere to check
            if (CollisionType == ObjectTypes.Box)
            {
                MovingBoxes = new List<OrientedBoundingBox>(0);
                foreach (ModelMesh mesh in this.MyModel.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                        MovingBoxes.Add(new OrientedBoundingBox(Vector3.Zero, Vector3.Zero, new Quaternion()));
                //allocates the bounding box array based on individual meshes
                this.CalculateBoundingBox();
                //Console.WriteLine(this.ModelName);
                //Console.WriteLine(this.MovingBoxes.Count);

                //default velocity as a unit vector...Remember: an object stays at rest until it is acted upon by another force! - Velocity already has this default. Defining it here is useless and problematic
                //Velocity = Vector3.Zero;
                //used so any object can have it's bounding box drawn...should be removed later if memory needs to be saved
                buffers = new BoundingBoxBuffers[this.MovingBoxes.Count];
                for (int cntr = 0; cntr < this.MovingBoxes.Count; cntr++)
                {
                    buffers[cntr] = new BoundingBoxBuffers(this.MovingBoxes[cntr], device);
                }
            }
        }
        //alternative way to create a bounding box; which is a lot more accurate/adaptabel then the collision processor and maybe faster
        //also adapted from source code online...but made better
        public void CalculateBoundingBox()
        {
            //allocates space for array
            if (this.GetType() == typeof(Wall))
            {
                ((Wall)this).CalculateBoundingBox();
                return;
            }
            if (this.GetType() == typeof(CollisionCheckSystem))
            {
                ((CollisionCheckSystem)this).CalculateBoundingBox();
                return;
            }
            //this.MovingBoxes = new List<OrientedBoundingBox>(0);
            // Create variables to hold min and max xyz values for the model. Initialise them to extremes
            //Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            //Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            int counter = 0;
            Matrix[] transforms = new Matrix[MyModel.Bones.Count];
            MyModel.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in this.MyModel.Meshes)
            {
                //spot
                // There may be multiple parts in a mesh (different materials etc.) so loop through each
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
                    Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                    Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                    // The stride is how big, in bytes, one vertex is in the vertex buffer
                    // We have to use this as we do not know the make up of the vertex
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    byte[] vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1); // fixed 13/4/11

                    // Find minimum and maximum xyz values for this mesh part
                    // We know the position will always be the first 3 float values of the vertex data
                    Vector3 vertPosition = new Vector3();

                    // transform by mesh bone transforms..like in the update method
                    for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);
                        vertPosition = Vector3.Transform(vertPosition, transforms[mesh.ParentBone.Index]);

                        // update our running values from this vertex
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                    //transformation code
                    //---------------------------------------------------------------------

                    // Expand model extents by the ones from this mesh
                    //modelMin = Vector3.Min(modelMin, meshMin);
                    //modelMax = Vector3.Max(modelMax, meshMax);
                    //---------------------------------------------------------------------
                    //creates a bounding box for each mesh
                    try
                    {
                        MovingBoxes[counter].HalfExtent = (meshMax - meshMin) / 2;
                        MovingBoxes[counter].Center = Vector3.Transform(meshMin + MovingBoxes[counter].HalfExtent, Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z)) + Position;
                        MovingBoxes[counter].HalfExtent = new Vector3(Math.Abs(MovingBoxes[counter].HalfExtent.X), Math.Abs(MovingBoxes[counter].HalfExtent.Y), Math.Abs(MovingBoxes[counter].HalfExtent.Z));
                        MovingBoxes[counter].Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
                    }
                    catch (Exception)
                    {
                        BoundingSetup();
                        return;
                    }
                    counter++;
                }
                //transformation code goes here for only one box
            }
        }
        //new methods to handle a bounding box array
        //applies a change to all the bounding boxes in an array
        protected void ApplyChangeToBB(Vector3 Change)
        {
            for (int cntr = 0; cntr < this.MovingBoxes.Count; cntr++)
            {
                //two temp vectors to store change
                this.MovingBoxes[cntr].Center += Change;
            }
        }


        //Velocity Helper Methods-------------------------------
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
            return (-1f * Velocity);
        }
        //End Velocity Helper Methods-------------------------------

        //changes just the position of the box based on velocity
        public void MoveModel()
        {
            Position += Velocity;
        }
        private void MoveModel(Vector3 ChangeVect)
        {
            Position += ChangeVect;
        }
        //changes bounding box and vectors based on the velocity. Gravity is only ever applied here. WARNING - do not use this method more than once per object.
        public Boolean MoveModelBox()
        {
            //only if object is moveable
            if (Moveable)
            {

                //Console.WriteLine("Falling "+FallCntr);
                //local calculation done each time to keep track of the change that has been done (velocity*acceleration)
                float KeepTrackOfChange = 0f;
                if ((FallCntr == 0) || (Velocity.Y == 0f))
                {
                    this.Velocity.Y += DataValues.SGravity;
                    FallCntr = 0;
                }
                //increase velocity due to gravitational acceleration until terminal velocity is reached
                if (this.Velocity.Y > DataValues.TerminalVelocity * DataValues.MovePrecision/DataValues.SurroundingsPrecision)
                {
                    this.Velocity.Y = (float)(((DataValues.SGravity > 0) ^ (this.Velocity.Y > 0)) ? this.Velocity.Y / Math.Pow(DataValues.GravityAcceleration, DataValues.MovePrecision / DataValues.SurroundingsPrecision) : this.Velocity.Y * Math.Pow(DataValues.GravityAcceleration, DataValues.MovePrecision / DataValues.SurroundingsPrecision));
                    if ((Math.Abs(this.Velocity.Y) < .0004f) && (FallCntr > 0))
                        this.Velocity.Y *= -1;

                    //KeepTrackOfChange = this.Velocity.Y; 
                }

                /*
                if (Position.Y > this.GetBoundingSpheres().Radius)
                {
                    Position.Y += KeepTrackOfChange;//this.Velocity.Y;
                    FallCntr++;
                }*/

                //reduces the velocity of an object when in free fall. May remove the if statement if this function is only used for freefalls
                if (freeFall)
                {
                    Velocity.X *= DataValues.AirResistance + (float)(Math.Pow(Math.Pow(Velocity.X, 2) + Math.Pow(Velocity.Z, 2), .5)) / 10 * 3;
                    Velocity.Z *= DataValues.AirResistance + (float)(Math.Pow(Math.Pow(Velocity.X, 2) + Math.Pow(Velocity.Z, 2), .5)) / 10 * 3;
                    if ((Velocity.X == 0f) && (Velocity.Y == 0f))
                        freeFall = false;
                }
                MoveModel();
                if (CollisionType == ObjectTypes.Box)
                    //changes vector values for the min and max positions in the bounding box
                    this.ApplyChangeToBB(this.Velocity);
            }
            if (Velocity == Vector3.Zero)
                return (false);
            return (true);
        }

        public void CheckSurroundings(List<ScreenModel> models)
        {
            MoveModelBox();
            MoveModelBox(-Velocity);
            this.Velocity /= DataValues.SurroundingsPrecision;
            if ((this.Velocity.X == this.Velocity.Y) && (this.Velocity.X == this.Velocity.Z) && (this.Velocity.X == 0))
                return;
            for (int x = 0; x < DataValues.SurroundingsPrecision; x++)
            {
                this.MoveModel();
                this.ApplyChangeToBB(Velocity);
                if (this.CheckCollision(models) != null)
                    break;
            }
            this.Velocity *= DataValues.SurroundingsPrecision;
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
                    //changes vector values for the min and max positions in the bounding box
                    this.ApplyChangeToBB(ChangeVect);
                }
            }
        }
        //creates a change vector from the object based on being moved by the user; new point
        public void PickUpVelocity(Vector3 OldPosition)
        {
            Velocity += (this.Position - OldPosition);
        }


        //used with the collision class w/user input (does not move the model with velocity; compounds velocity
        protected void MoveModelBox(Vector3 ChangeVect, bool MoveWithUserInput)
        {
            //only if object is moveable
            if (Moveable)
            {
                //MoveModel();
                //changes vector values
                if (CollisionType == ObjectTypes.Box)
                {
                    //changes vector values for the min and max positions in the bounding box
                    this.ApplyChangeToBB(ChangeVect);
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
                if (CheckArg.Intersects(Temp))
                {
                    //intersection confimerd, stop checks and prevent model from moving in outer code
                    return (true);
                }
            }
            //else, return false, no hit detected
            return (false);
        }
        //check with a bounding box
        public bool CheckBoundingSpheres(List<OrientedBoundingBox> CheckArg)
        {
            for (int cntr = 0; cntr < MyModel.Meshes.Count; cntr++)
            {
                //creates a sphere for one mesh
                BoundingSphere Temp = this.MyModel.Meshes[cntr].BoundingSphere;
                //adjusts to center of the model
                Temp.Center += this.Position;
                for (int cntr2 = 0; cntr2 < CheckArg.Count; cntr2++)
                {
                    if (CheckArg[cntr2].Intersects(Temp))
                    {
                        //intersection confimerd, stop checks and prevent model from moving in outer code
                        return (true);
                    }
                }
            }
            //else, return false, no hit detected
            return (false);
        }
        //checks a list using the boudning box, regarless whether or not this object usually uses a bounding box
        public ScreenModel CheckBoundingSpheres(List<ScreenModel> VisibleObjects)
        {
            bool Test = false;
            //omg, schuyler used a foreach loop! 1st time ever
            foreach(ScreenModel CheckArg in VisibleObjects)
            {
                //checks to see if this object is checking itself...just in case
                //yes, this should be comparing memory addresses
                if (this != CheckArg)
                {
                    if (CheckArg.CollisionType == ObjectTypes.Box)
                    {
                        Test = this.CheckBoundingSpheres(CheckArg.MovingBoxes);
                    }
                    else if (CheckArg.CollisionType == ObjectTypes.Sphere)
                    {
                        Test = this.CheckBoundingSpheres(CheckArg.GetBoundingSpheres());
                    }
                    //if an object hit is detected, break the check
                    if (Test)
                    {
                        //kills loop when collision is detected
                        return (CheckArg);
                    }
                }
            }
            return (null);
        }
        //merges these bounding spheres together to send over
        public BoundingSphere GetBoundingSpheres()
        {
            if (this.GetType() == typeof(Wall))
                return (new BoundingSphere(this.Position, DataValues.VectorDistance(new Vector3(((Wall)this).GetLength() / 2, ((Wall)this).GetHeight() / 2, ((Wall)this).GetWidth() / 2))));
            if (this.GetType() == typeof(CollisionCheckSystem))
                return (new BoundingSphere(this.Position, DataValues.VectorDistance(new Vector3(((CollisionCheckSystem)this).Length / 2, ((CollisionCheckSystem)this).Height / 2, ((CollisionCheckSystem)this).Width / 2))));

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
        public BoundingBox GetBoundingBoxes()
        {
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for(int x=0; x<this.MovingBoxes.Count; x++)
            {
                for (int y = 0; y < MovingBoxes[x].GetCorners().Length; y++)
                {
                    Vector3 Corner = MovingBoxes[x].GetCorners()[y];
                    if (Corner.X > Max.X)
                        Max.X = Corner.X;
                    else if (Corner.X < Min.X)
                        Min.X = Corner.X;
                    if (Corner.Y > Max.Y)
                        Max.Y = Corner.Y;
                    else if (Corner.Y < Min.Y)
                        Min.Y = Corner.Y;
                    if (Corner.Z > Max.Z)
                        Max.Z = Corner.Z;
                    else if (Corner.Z < Min.Z)
                        Min.Z = Corner.Z;
                }
            }
            //return the collection of boxes
            return (new BoundingBox(Min,Max));
        }
        //uses a series of checks to determine the best method to call for checking collisions, based on the collision type selected
        public bool CheckCollision(ScreenModel CheckArg)
        {
            bool Test = false;
            if (CheckArg.CollisionType == ObjectTypes.Box)
            {
                if (this.CollisionType == ObjectTypes.Box)
                {
                    Test = this.CheckBBArray(CheckArg.MovingBoxes);
                }
                if (this.CollisionType == ObjectTypes.Sphere)
                {
                    Test = this.CheckBoundingSpheres(CheckArg.MovingBoxes);
                }
            }
            else if (CheckArg.CollisionType == ObjectTypes.Sphere)
            {
                if (this.CollisionType == ObjectTypes.Box)
                {
                    Test = this.CheckBBArray(CheckArg.GetBoundingSpheres());
                }
                if (this.CollisionType == ObjectTypes.Sphere)
                {
                    Test = this.CheckBoundingSpheres(CheckArg.GetBoundingSpheres());
                }
            }
            return (Test);
        }
        public Vector3 CheckCollisionNormal(ScreenModel CheckArg)
        {
            Vector3 Test = Vector3.Zero;
            if (CheckArg.CollisionType == ObjectTypes.Box)
                if (this.CollisionType == ObjectTypes.Box)
                    Test = this.CheckBBArray(CheckArg.MovingBoxes, CheckArg.Velocity);

            return (Test);
        }
        //for special case of the "Personal Shield"
        public bool CheckCollision(BoundingSphere CheckArg)
        {
            bool Test = false;
            if (this.CollisionType == ObjectTypes.Box)
            {
                Test = this.CheckBBArray(CheckArg);
            }
            if (this.CollisionType == ObjectTypes.Sphere)
            {
                Test = this.CheckBoundingSpheres(CheckArg);
            }
            return (Test);
        }
        //version of this that handles a list of screen models, not just a single one
        public ScreenModel CheckCollision(List<ScreenModel> VisibleObjects)
        {
            bool Test = false;
            for(int x=0; x< VisibleObjects.Count; x++)
            {
                ScreenModel CheckArg = VisibleObjects[x];
                if (!CheckArg.Sphere.Intersects(Sphere))
                    continue;
                //checks to see if this object is checking itself...just in case
                //yes, this should be comparing memory addresses
                if (this != CheckArg)
                {
                    if (CheckArg.CollisionType == ObjectTypes.Box)
                    {
                        if (this.CollisionType == ObjectTypes.Box)
                        {
                            Test = this.CheckBBArray(CheckArg.MovingBoxes);
                        }
                        if (this.CollisionType == ObjectTypes.Sphere)
                        {
                            Test = this.CheckBoundingSpheres(CheckArg.MovingBoxes);
                        }
                    }
                    else if (CheckArg.CollisionType == ObjectTypes.Sphere)
                    {
                        if (this.CollisionType == ObjectTypes.Box)
                        {
                            Test = this.CheckBBArray(CheckArg.GetBoundingSpheres());
                        }
                        if (this.CollisionType == ObjectTypes.Sphere)
                        {
                            Test = this.CheckBoundingSpheres(CheckArg.GetBoundingSpheres());
                        }
                    }
                    if (Test)
                    {
                        ApplyBounce(CheckArg);
                        return (CheckArg);
                    }
                }
            }
            //will make the object bounce if it hits the Y-axis. (Currently used because we have no working floor)
            /*
            if (Position.Y - ((this.CollisionType == ObjectTypes.Sphere) ? this.GetBoundingSpheres().Radius : DataValues.VectorDistance(this.GetBoundingBoxes().Max - this.Position)) <= 0)
                ApplyBounce(null);
             */
            return (null);
        }
        public void ApplyBounce(ScreenModel CheckArg)
        {
            //stores the velocity
            Vector3 StoreVelocity = this.Velocity;

            //Moves the object backwards
            this.MoveModelBox(this.Velocity * -1);

            /*this will figure out which axises the object has to change direction in*/
            //checks the X-axis
            this.MoveModelBox(new Vector3(Velocity.X, 0, 0));
            bool intersected = (CheckArg == null) ? false : CheckCollision(CheckArg);
            if (intersected)
            {
                this.MoveModelBox(new Vector3(Velocity.X * -1, 0, 0));
                Velocity.X *= -1;
            }

            //checks the Y-axis
            this.MoveModelBox(new Vector3(0, Velocity.Y, 0));
            intersected = (CheckArg == null) ? ((Position.Y - ((this.CollisionType == ObjectTypes.Sphere) ? this.GetBoundingSpheres().Radius : DataValues.VectorDistance(this.GetBoundingBoxes().Max - this.Position)) <= 0) ? true : false) : CheckCollision(CheckArg);
            if (intersected)
            {
                this.MoveModelBox(new Vector3(0, Velocity.Y * -1, 0));
                Velocity.Y *= -1;
                FallCntr = 1;
            }

            //checks the Z-axis
            this.MoveModelBox(new Vector3(0, 0, Velocity.Z));
            intersected = (CheckArg == null) ? false : CheckCollision(CheckArg);
            if (intersected)
            {
                this.MoveModelBox(new Vector3(0, 0, Velocity.Z * -1));
                Velocity.Z *= -1;
            }


            //will apply friction only to axises that changed direction
            if (Velocity.X == StoreVelocity.X * -1)
                this.Velocity.X *= this.Bounce;
            if (Velocity.Y == StoreVelocity.Y * -1)
                this.Velocity.Y *= this.Bounce;
            if (Velocity.Z == StoreVelocity.Z * -1)
                this.Velocity.Z *= this.Bounce;

            //will stop objects if they are moving too slowly
            if (Math.Abs(this.Velocity.X) < .0001f)
                this.Velocity.X = 0;
            if (Math.Abs(this.Velocity.Z) < .0001f)
                this.Velocity.Z = 0;
            //if (this.FallCntr < 5)
                //this.Velocity.Y = 0;

        }
        //two more check collision methods that deal with bounding box arrays
        private bool CheckBBArray(List<OrientedBoundingBox> CheckArg)
        {
            bool Test = false;
            //checks through each bounding box in the array
            for (int cntr = 0; cntr < CheckArg.Count; cntr++)
            {
                if (CheckArg[cntr].Intersects(Sphere))
                    for (int cntr2 = 0; cntr2 < this.MovingBoxes.Count; cntr2++)
                    {
                        Test = this.MovingBoxes[cntr2].Intersects(CheckArg[cntr]);
                        //short-circuit the inner loop to save time, once a collision has been detected
                        if (Test == true)
                        {
                            //Console.WriteLine(this.ModelName);
                            return true;
                        }
                    }
            }
            return (false);
        }
        private Vector3 CheckBBArray(List<OrientedBoundingBox> CheckArg, Vector3 Velocity)
        {
            Vector3 Test = Vector3.Zero;
            //checks through each bounding box in the array
            for (int cntr = 0; cntr < CheckArg.Count; cntr++)
            {
                if (CheckArg[cntr].Intersects(Sphere))
                    for (int cntr2 = 0; cntr2 < this.MovingBoxes.Count; cntr2++)
                    {
                        Test = this.MovingBoxes[cntr2].ContainsNormal(CheckArg[cntr]);
                        //short-circuit the inner loop to save time, once a collision has been detected
                        if (Test!=Vector3.Zero)
                        {
                            //Console.WriteLine(this.ModelName);
                            return Test;
                        }
                    }
            }
            return (Test);
        }
        private bool CheckBBArray(BoundingSphere CheckArg)
        {
            bool Test = false;
            //checks through each bounding box in the array
            for (int cntr = 0; cntr < this.MovingBoxes.Count; cntr++)
            {
                Test = this.MovingBoxes[cntr].Intersects(CheckArg);
                //short-circuit the system to save time, once a collision has been detected
                if (Test == true)
                    break;
            }
            return (Test);
        }

        //----------------------------------------------------------------
        //these methods are "dead"/have no code for regular screen model cases; sub classes will override these for their specific needs
        //animation for the doors to open; also moves bounding 
        public virtual void Animation()
        {
        }
        //method to be called if the user has performed an action
        public virtual void Action(String Filename, ContentManager Content, List<ScreenModel> models, GraphicsDeviceManager graphics)
        {
        }
        //one for special forge world uses
        public virtual void SpecialObjForgeAction()
        {
        }
        //any special reticles needed for the object
        public virtual DataValues.ReticleType SpecialReticle()
        {
            return(DataValues.ReticleType.Normal);
        }
        //overload that takes a bool
        public virtual DataValues.ReticleType SpecialReticle(bool TrueFalse)
        {
            return (DataValues.ReticleType.Normal);
        }
        //overloads the compare to method
        public int CompareTo(object Object)
        {
            //if these are the same object (same memory reference) then return 0
            if ((ScreenModel)Object == this)
            {
                return (0);
            }
            //if not, return -1 as I don't care if it's located or not
            return (-1);
        }
        //----------------------------------------------------------------

        //-------------------------------------------------------------------------
        //Baby class that will help draw the bounding boxes, much better explanation than microsoft could give
        //source: http://www.roastedamoeba.com/blog/archive/2010/12/10/drawing-an-xna-model-bounding-box
        public class BoundingBoxBuffers
        {
            public VertexBuffer Vertices;
            public int VertexCount;
            public IndexBuffer Indices;
            public int PrimitiveCount;
            public GraphicsDevice device;
            public List<VertexPositionColor> verticesList = new List<VertexPositionColor>(0);
            public Color LineColor = Color.White;

            public BoundingBoxBuffers(OrientedBoundingBox boundingBox, GraphicsDevice Graphics, Color LineColor)
            {
                this.LineColor = LineColor;
                SetVertices(boundingBox, Graphics);
            }
            public BoundingBoxBuffers(OrientedBoundingBox boundingBox, GraphicsDevice Graphics)
            {
                SetVertices(boundingBox, Graphics);
            }
            public void SetVertices(OrientedBoundingBox boundingBox, GraphicsDevice Graphics)
            {
                //BoundingBoxBuffers boundingBoxBuffers = new BoundingBoxBuffers();
                this.device = Graphics;
                this.PrimitiveCount = 24;
                this.VertexCount = 48;
                VertexBuffer vertexBuffer = new VertexBuffer(device,
                    typeof(VertexPositionColor), this.VertexCount,
                    BufferUsage.WriteOnly);
                const float ratio = 5.0f;
                Vector3 xOffset = new Vector3((boundingBox.HalfExtent.X) / ratio, 0, 0);
                Vector3 yOffset = new Vector3(0, (boundingBox.HalfExtent.Y) / ratio, 0);
                Vector3 zOffset = new Vector3(0, 0, (boundingBox.HalfExtent.Z) / ratio);
                Vector3[] corners = boundingBox.GetRotatedCorners();

                // Corner 1.     
                AddVertex(verticesList, corners[0]);
                AddVertex(verticesList, corners[1]);
                AddVertex(verticesList, corners[0]);
                AddVertex(verticesList, corners[4]);
                AddVertex(verticesList, corners[0]);
                AddVertex(verticesList, corners[3]);
                // Corner 2.     
                AddVertex(verticesList, corners[1]);
                AddVertex(verticesList, corners[2]);
                AddVertex(verticesList, corners[1]);
                AddVertex(verticesList, corners[5]);
                AddVertex(verticesList, corners[1]);
                AddVertex(verticesList, corners[0]);
                // Corner 3.     
                AddVertex(verticesList, corners[2]);
                AddVertex(verticesList, corners[3]);
                AddVertex(verticesList, corners[2]);
                AddVertex(verticesList, corners[6]);
                AddVertex(verticesList, corners[2]);
                AddVertex(verticesList, corners[1]);
                // Corner 4.     
                AddVertex(verticesList, corners[3]);
                AddVertex(verticesList, corners[0]);
                AddVertex(verticesList, corners[3]);
                AddVertex(verticesList, corners[7]);
                AddVertex(verticesList, corners[3]);
                AddVertex(verticesList, corners[2]);
                // Corner 5.     
                AddVertex(verticesList, corners[4]);
                AddVertex(verticesList, corners[5]);
                AddVertex(verticesList, corners[4]);
                AddVertex(verticesList, corners[0]);
                AddVertex(verticesList, corners[4]);
                AddVertex(verticesList, corners[7]);
                // Corner 6.     
                AddVertex(verticesList, corners[5]);
                AddVertex(verticesList, corners[6]);
                AddVertex(verticesList, corners[5]);
                AddVertex(verticesList, corners[1]);
                AddVertex(verticesList, corners[5]);
                AddVertex(verticesList, corners[4]);
                // Corner 7.     
                AddVertex(verticesList, corners[6]);
                AddVertex(verticesList, corners[7]);
                AddVertex(verticesList, corners[6]);
                AddVertex(verticesList, corners[2]);
                AddVertex(verticesList, corners[6]);
                AddVertex(verticesList, corners[5]);
                // Corner 8.     
                AddVertex(verticesList, corners[7]);
                AddVertex(verticesList, corners[4]);
                AddVertex(verticesList, corners[7]);
                AddVertex(verticesList, corners[3]);
                AddVertex(verticesList, corners[7]);
                AddVertex(verticesList, corners[6]);

                vertexBuffer.SetData(verticesList.ToArray());
                this.Vertices = vertexBuffer;
                IndexBuffer indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, this.VertexCount,
                    BufferUsage.WriteOnly);
                indexBuffer.SetData(Enumerable.Range(0, this.VertexCount).Select(i => (short)i).ToArray());
                this.Indices = indexBuffer;
            }
            private void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
            {
                vertices.Add(new VertexPositionColor(position, LineColor));
            }
        }


        //methods to use for human testing
        public void DrawBoundingBox(Vector3 CameraPosition, Vector3 CameraAngle3)
        {
            //buffers = new BoundingBoxBuffers(MovingBox, device);
            BasicEffect effects = new BasicEffect(device);

            BasicEffect lineEffect = new BasicEffect(device);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;

            for (int cntr = 0; cntr < this.buffers.Length; cntr++)
            {
                device.SetVertexBuffer(buffers[cntr].Vertices);
                device.Indices = buffers[cntr].Indices;
                effects.World = Matrix.Identity;
                effects.View = Matrix.CreateLookAt(CameraPosition, new Vector3(CameraPosition.X + CameraAngle3.X, CameraPosition.Y + CameraAngle3.Y, CameraPosition.Z + CameraAngle3.Z), Vector3.Up);
                effects.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(scale), this.aspectRatio, DataValues.NearPlane, DataValues.FarPlane);
                foreach (EffectPass pass in effects.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    //graphicsDevice.DrawIndexedPrimitives(,,,,,

                    device.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                        buffers[cntr].VertexCount, 0, buffers[cntr].PrimitiveCount);
                }
            }
        }
    }
}
