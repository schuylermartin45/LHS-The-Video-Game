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
using DataTypes;
//using CustomContentPipeline;
namespace Senior_Project
{
    //First (and maybe only?) class that is designed to make collision checking between all visible objects easy to handle in-game
    //Extends the ScreenModel class in order to 
    public class CollisionCheckSystem : ScreenModel
    {
        //for controlling movement speed
        public float MoveSensitivity = 0.2f;
        public float PitchSensitivity = 4f;
        //for use with the xbox controller...may have some special purpose later
        public PlayerIndex PlayerIndexValue = new PlayerIndex();
        //deals with angle of the camera
        public Vector2 CameraAngle = new Vector2(90, 0);
        //vector used in drawing other objects in relationship to the camera
        public Vector3 CameraAngle3 = Vector3.Zero;
        //vector that stores the camera's position with a vector!!!!
        public Vector3 CameraPosition;
        //how far away the camera is from the center of the model...to correct problems later on; DO NOT CHANGE VALUE IN CODE
        private Vector3 CameraModelOffSet = new Vector3(0f,0f,0f);
        //how far away a (picked-up) object is from the center of the model; DO NOT CHANGE VALUE IN CODE
        private Vector3 HoldingObjectOffSet = new Vector3(0f, 0f, 5f);
        //holds the "value" of the object that is currently being held
        private ScreenModel HoldingObject;
        //determines what hitting the pick up button does; if true, you are holding an object
        private bool HoldingObjectTrue = false;
        //Arraylist that holds references to all objects that are near the user in 
        public List<ScreenModel> VisibleObjects;
        //shield will detect objects in front of the camera to test for better collisions
        public BoundingSphere PersonalShield;
        //represents the camera's vector position
        public CollisionCheckSystem(Game1 TheGame, Model SentModel, Vector3 ModelPosition)
        {
            GameRef = TheGame;
            //camera should always be moveable; but can be changed if need be
            base.Moveable = true;
            //would have made a call to super constructors but I decided to put in specific information; less parameters to pass about
            ModifiedBaseConstuctor(ModelPosition);
            VisibleObjects = new List<ScreenModel>();
            //Shield finds nearby objects at a specific distance...items in range will be added to the VisibleObjects list
            PersonalShield = new BoundingSphere(Position,1f);
        }
        //modified version of the screen model constructor that is specific to the first person model; 
        //modified because the values should be specific to this very special object
        private void ModifiedBaseConstuctor(Vector3 CameraPosArg)
        {
            CameraPosition=CameraPosArg;
            //should be an off-set version of whereever the camera is
            Position = CameraPosition + CameraModelOffSet;
            MyModel = GameRef.Content.Load<Model>("HumanRectangle");
            if (Moveable)
            {
                Velocity = Vector3.Zero;
            }
        }
        
        //moves model and person
        public void MovePerson()
        {
            //velocity is used to get the directional change of the model's position to re-create the bounding box
            if(!Keyboard.GetState().IsKeyDown(Keys.LeftShift)) //allows us to move out of the game frame while testing
                GetUserInput();
        }
        //Gets data 
        private void GetUserInput()
        {
            Velocity = Vector3.Zero;
            //if the game controller is unplugged, the keyboard/mous combination must be used; automatic change-over for the user
            if (!GamePad.GetState(PlayerIndexValue).IsConnected)
            {
                //opens up the pause menu when escape key is pressed
                GameRef.MyScreen.CreateEscKey(GameRef,GameRef.graphics);
                
                //code that makes the reticule move from the mouse
                CameraAngle.X -= (Mouse.GetState().X - (GameRef.graphics.PreferredBackBufferWidth/2)) / PitchSensitivity;
                CameraAngle.Y -= (Mouse.GetState().Y - (GameRef.graphics.PreferredBackBufferHeight/2)) / PitchSensitivity;
                
                //center screen; where reticule should go at some point
                Mouse.SetPosition(GameRef.graphics.PreferredBackBufferWidth / 2,GameRef.graphics.PreferredBackBufferHeight / 2);

                if ((CameraAngle.Y > 90) && (CameraAngle.Y < 180)) CameraAngle.Y = 90;
                if ((CameraAngle.Y < 270) && (CameraAngle.Y > 180)) CameraAngle.Y = 270;

                if (CameraAngle.X >= 361) CameraAngle.X -= 360; else if (CameraAngle.X <= -1) CameraAngle.X += 360;
                if (CameraAngle.Y >= 361) CameraAngle.Y -= 360; else if (CameraAngle.Y <= -1) CameraAngle.Y += 360;

                CameraAngle3.X = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X));
                CameraAngle3.Z = (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X));
                CameraAngle3.Y = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y));

                int SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;
                int CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;

                //actual floor movement
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    Velocity.X += MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90));
                    Velocity.Z += MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    Velocity.X -= MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90));
                    Velocity.Z -= MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90));
                }

                SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;
                CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    Velocity.X += MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X));
                    Velocity.Z += MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    Velocity.X -= MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X));
                    Velocity.Z -= MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X));
                }
                //for jumping
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Jump();
                }
            }
            //only runs this code if the xbox controller is plugged in
            if(GamePad.GetState(PlayerIndexValue).IsConnected)
            {
                //opens up the pause menu when Start Button is Pressed (xbox controller
                GameRef.MyScreen.CreateStartBtn(GameRef,GameRef.graphics);
                
                //from testing, the mouse works by locating the x and y positions on the screen while the 
                //xbox controller uses a unit circle of values (Radians of +/-1 for X and Y) as input;
                //subtracting x and adding why gives "regular" controll; reversing signs will result in inverse pitch control
                CameraAngle.X -= (GamePad.GetState(PlayerIndexValue).ThumbSticks.Right.X * PitchSensitivity); // PitchSensitivity;
                CameraAngle.Y += (GamePad.GetState(PlayerIndexValue).ThumbSticks.Right.Y * PitchSensitivity); // PitchSensitivity;

                if ((CameraAngle.Y > 90) && (CameraAngle.Y < 180)) CameraAngle.Y = 90;
                if ((CameraAngle.Y < 270) && (CameraAngle.Y > 180)) CameraAngle.Y = 270;

                if (CameraAngle.X >= 361) CameraAngle.X -= 360; else if (CameraAngle.X <= -1) CameraAngle.X += 360;
                if (CameraAngle.Y >= 361) CameraAngle.Y -= 360; else if (CameraAngle.Y <= -1) CameraAngle.Y += 360;

                CameraAngle3.X = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X));
                CameraAngle3.Z = (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X));
                CameraAngle3.Y = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y));

                int SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;
                int CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;

                //Left stick movement; moves on the x/y plane on the ground
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickLeft))
                {
                    Velocity.X += MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90));
                    Velocity.Z += MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90));
                }
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickRight))
                {
                    Velocity.X -= MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90));
                    Velocity.Z -= MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90));
                }

                SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;
                CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;

                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickUp))
                {
                    Velocity.X += MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X));
                    Velocity.Z += MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X));
                }
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickDown))
                {
                    Velocity.X -= MoveSensitivity * SinPos * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X));
                    Velocity.Z -= MoveSensitivity * CosPos * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X));
                }
                //for jumping
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.A))
                {
                    Jump();
                }
            }
            //velocity now records the change made by the user's entry
            //if the new position checks out, MoveModel moves the model sphere for the check
            PersonalShield.Center = Position + Velocity;
            if (CheckEitherSurroundings())
            {
                //Console.WriteLine("True");
                CameraPosition += Velocity;
                //changes position that is used to create the bounding box
                Position = CameraPosition + CameraModelOffSet;
                //in this case, the personal collision shield allows user to pick up objects seen directly in front of him

                if (HoldingObjectTrue)
                {
                    //the object's position is infront of you by a set distance infront of you
                    HoldingObject.Position = this.Position + HoldingObjectOffSet;
                }
            }
            //if not, don't move; reset the moving box to the correct position
            else
            {
                //moves the shield back to the previous position
                PersonalShield.Center = Position - Velocity;
            }
        }
        //function for jumping...button specified above
        private void Jump()
        {
        }

        //constantly checks surroundings to add objects in and out of the list; bounding box will be used to check further specifics
        //*********before this is called, the Visible objects list should containt a list of all collision based objects in a load space
        private bool CheckSurroundingsBoundingBox()
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < VisibleObjects.Count; cntr++)
            {
                if (PersonalShield.Intersects(VisibleObjects[cntr].MovingBox))
                {
                    return (false);
                }
            }
            return (true);
        }
        private bool CheckSurroundingsBoundingSpheres()
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < VisibleObjects.Count; cntr++)
            {
                if (VisibleObjects[cntr].CheckBoundingSpheres(PersonalShield))
                {
                    return (false);
                }
            }
            return (true);
        }
        //Check the model based on it's collision type
        //some objects are better off using a series of mesh-based bounding boxes than spheres
        public bool CheckEitherSurroundings()
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < VisibleObjects.Count; cntr++)
            {
                if (VisibleObjects[cntr].CollisionType == ObjectTypes.Box)
                {
                    //Check this logic out...try using the series of bounding boxes
                    if (VisibleObjects[cntr].MovingBox.Intersects(this.PersonalShield))
                    {
                        //allows us to move moving objects                  
                        CheckMovingOjects(VisibleObjects[cntr]);
                        //allows us to pick up pickupable objects
                        DeterminePickUp(VisibleObjects[cntr]);
                        return (false);
                    }
                }
                else if ((VisibleObjects[cntr].CollisionType == ObjectTypes.Sphere))
                {
                    if (VisibleObjects[cntr].CheckBoundingSpheres(PersonalShield))
                    {
                        CheckMovingOjects(VisibleObjects[cntr]);
                        DeterminePickUp(VisibleObjects[cntr]);
                        return (false);
                    }
                }
                //case to handle a room 
                else if (VisibleObjects[cntr].CollisionType == ObjectTypes.Room)
                {
                    //gets an array of planes contained in a room
                    for (int cntr2 = 0; cntr2 < VisibleObjects[cntr].ListOfWalls.Count; cntr2++)
                    {
                        if (PersonalShield.Intersects(VisibleObjects[cntr].ListOfWalls[cntr]) == PlaneIntersectionType.Intersecting)
                        {
                            CheckMovingOjects(VisibleObjects[cntr]);
                            DeterminePickUp(VisibleObjects[cntr]);
                            return (false);
                        }
                    }
                }
            }
            return (true);
        }
        //awesomeness; if an object is moveable and is moving and pushes the person with force, then move the person!
        //*****TO BE CALLED WHEN A COLLISION HIT HAS BEEN CONFIRMED********
        private void CheckMovingOjects(ScreenModel CheckArg)
        {
            if ((CheckArg.Moveable)&&(CheckArg.Velocity!=Vector3.Zero))
            {
             
                //moves camera and entities with it
                PersonalShield.Center = Position + CheckArg.Velocity;
                CameraPosition += CheckArg.Velocity;
                Position = CameraPosition + CameraModelOffSet;
                //moves the object based on any forced applied by the user, running in the opposite direction
                CheckArg.MoveModelBox(this.Velocity);
                //updating the drawing is not actually needed, as it is handled by the game's drawing code
                //CheckArg.Update();
            }
            //move objects that can be moved when I apply force to them
            if ((CheckArg.Moveable) && (CheckArg.Velocity == Vector3.Zero))
            {
                CheckArg.MoveModelBox(this.Velocity);
                //CheckArg.Update();
            }  
        }
        //--------------------------------------------------------------------------------------------------------------
        //method that checks if the object is visibly in front of the user
        private bool ObjectIsInfront(ScreenModel ToTestPickUp)
        {
            //use reticule
            /*
            if (Math.Abs(ToTestPickUp.Position.X) >= Math.Abs(this.Position.X))
            {
                if (Math.Abs(ToTestPickUp.Position.Y) >= Math.Abs(this.Position.Y))
                {
                    if (Math.Abs(ToTestPickUp.Position.Z) >= Math.Abs(this.Position.Z))
                    {
                        return (true);
                    }
                }
            }*/
            return (true);
        }
        //method that provides logic in picking up/dropping of an object
        private void DeterminePickUp(ScreenModel ToTestPickUp)
        {
            //Console.WriteLine("Key Hit: "+GameRef.MyScreen.KeyHasBeenPressed(Keys.E));
            //first tests if the object can be picked up and if the object is infront of the user
            if ((ToTestPickUp.PickUpable)&&(ObjectIsInfront(ToTestPickUp)))
            {
                //for picking up/putting down objects ||
                //for picking up/putting down objects, with the E button
                /*
                (((!GamePad.GetState(PlayerIndexValue).IsConnected) && (GameRef.MyScreen.KeyHasBeenPressed(Keys.E)))
                    ||((GamePad.GetState(PlayerIndexValue).IsConnected) && (GameRef.MyScreen.ButtonHasBeenPressed(Buttons.X))))
                */
                if ((!GamePad.GetState(PlayerIndexValue).IsConnected) && (Mouse.GetState().LeftButton==ButtonState.Pressed))
                    //||((GamePad.GetState(PlayerIndexValue).IsConnected) && (GamePad.GetState(PlayerIndexValue).Triggers.Right==))
                {
                    Console.WriteLine("Getting Past IF");
                    //1st hit, pick it up
                    if (!HoldingObjectTrue)
                    {
                        //changes the sign of the bool
                        HoldingObjectTrue = true;
                        //object is now held under a new name
                        this.HoldingObject = ToTestPickUp;
                        //the object's position is infront of you by a set distance infront of you
                        HoldingObject.Position = this.Position + HoldingObjectOffSet;
                        

                        //MOVE BOUNDING BOX WITH OBJECT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    }
                    //2nd hit, drop the object
                    else
                    {
                        HoldingObjectTrue = false;
                        
                    }
                }
            }
        }
        //method that allows the user to pick up objects 
        private void PickUpObject()
        {
        }
        //method that allows the user to drop objects
        private void DropObject()
        {
        }
    }
}
