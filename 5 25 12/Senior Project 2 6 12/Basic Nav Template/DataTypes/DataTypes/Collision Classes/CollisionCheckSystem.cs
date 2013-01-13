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
    //First (and maybe only?) class that is designed to make collision checking between all visible objects easy to handle in-game
    //Extends the ScreenModel class in order to 
    //Stop explaining Schuyler, it's the player class. This controls your character
    public class CollisionCheckSystem : ScreenModel
    {
        //Two strings that hold the room the person is now and the one they will be in next (when opening a door)
        //these strings should contain the file path from the files folder
        public String CurrentRoom = "Hallway";
        //public String NextRoom = FolderNames.SpecialCases + "GenericRoom";
        //other values to be stored; for loading new rooms
        ContentManager Content;
        List<ScreenModel> models;
        GraphicsDeviceManager graphics;
        public Vector3 NormalVector = Vector3.One*3;
        //will create a bounding box around the room the door is associated with, so that determining which room to destroy when a door is closed is easy.
        OrientedBoundingBox RoomContainer = new OrientedBoundingBox(Vector3.Zero, Vector3.Zero, new Quaternion());
        //two bools that handle jumping/crouching
        private bool StartJump = false;
        //to prevent a double jump
        private bool PreventDoubleJump = false;
        private bool Crouched = false;
        private bool CanMove;
        //timer and boolean that controls sprinting; only runs when button is held down
        private TimerClass SprintTimer = new TimerClass(1f);

        //for use with the xbox controller...may have some special purpose later
        public PlayerIndex PlayerIndexValue = new PlayerIndex();
        //for checking buttons
        KeyboardState LastState = Keyboard.GetState();
        GamePadState GamePadLastState;

        //deals with angle of the camera
        public Vector2 CameraAngle = Vector2.Zero;
        //vector used in drawing other objects in relationship to the camera
        public Vector3 CameraAngle3 = Vector3.Zero;
        //vector that stores the camera's position with a vector!!!!
        public Vector3 CameraPosition;

        //how far away a (picked-up) object is from the center of the model; DO NOT CHANGE VALUE IN CODE
        private Vector3 HoldingObjectOffSet = new Vector3(0f, 0f, 2f);
        //holds the "value" of the object that is currently being held
        public ScreenModel HoldingObject;
        //determines what hitting the pick up button does; if true, you are holding an object
        public bool HoldingObjectTrue = false;
        //used to find the prior center position of the object to move the collision box
        private Vector3 HoldingObjectLastPos = Vector3.Zero;
        //counts clockcycles for above check
        //private int CountCollisionCycles = 0;
        private float storeVY = 0;
        //holds the type of reticle to be displayed
        public DataValues.ReticleType ReticleToDraw = DataValues.ReticleType.Normal;

        //Arraylist that holds references to all objects that are near the user in 
        public List<ScreenModel> VisibleObjects;
        public List<ScreenModel> NearObjects=new List<ScreenModel>(0);
        
        //shield will detect objects in front of the camera to test for better collisions
        public BoundingSphere PersonalShield;
        private Quaternion RotationQuaternion = Quaternion.Identity;
        public Boolean HoldingObjectDistanceSet = true;
        public static Boolean IsForgeModeRunning = false;
        public float Length { get { return _length; } set { _length = value; CalculateBoundingBox(); } }
        public float Height { get { return _height; } set { _height = value; CalculateBoundingBox(); } }
        public float Width { get { return _width; } set { _width = value; CalculateBoundingBox(); } }
        protected float _length=1, _height=1, _width=1;
        public Vector3 Position { get { return base.Position; } set { base.Position = value; CalculateBoundingBox(); } }
        // previous checkarg for chain collisions
        ScreenModel PCheckArg = null;
        MouseState LastMouseState;
        //represents the camera's vector position
        public CollisionCheckSystem(Model SentModel, Vector3 ModelPosition, ContentManager ContentArg, List<ScreenModel> ModelListArg, GraphicsDeviceManager GraphicArg)
            :base(SentModel,ModelPosition,GraphicArg.GraphicsDevice)
        {
            this.MovingBoxes = new List<OrientedBoundingBox>(0);
            MovingBoxes.Add(new OrientedBoundingBox(Vector3.Zero, Vector3.Zero, Quaternion.Identity));
            //camera should always be moveable; but can be changed if need be
            base.Moveable = true;
            this.MyModel = SentModel;
            base.CollisionType = ObjectTypes.Box;
            this.BoundingSetup();
            //correction, person starts at 0 in Y vector
            ModelPosition += DataValues.CameraGroundOffSet;
            //would have made a call to super constructors but I decided to put in specific information; less parameters to pass about
            ModifiedBaseConstuctor(ModelPosition, SentModel);
            VisibleObjects = new List<ScreenModel>();
            //Shield finds nearby objects at a specific distance...items in range will be added to the VisibleObjects list
            PersonalShield = new BoundingSphere(Position,1f);
            //variables used to load up new objects from doors
            Content=ContentArg;
            models = ModelListArg;
            this.graphics = GraphicArg;
            this.device = GraphicArg.GraphicsDevice;
            LastMouseState = Mouse.GetState();

            Height = 2.5f;
            Length = 0.75f;
            Width = 0.75f;
        }
        public new void CalculateBoundingBox()
        {
            //does not calculate the bounding box while in forge's "Fly" mode
            if (DataValues.gravity != 0f)
            {
                this.MovingBoxes[0].Center = Position;
                this.MovingBoxes[0].HalfExtent = new Vector3(Length, Height, Width);
                this.MovingBoxes[0].Orientation = Quaternion.Identity;
            }
        }
        //modified version of the screen model constructor that is specific to the first person model; 
        //modified because the values should be specific to this very special object
        private void ModifiedBaseConstuctor(Vector3 CameraPosArg, Model HumanModel)
        {
            CameraPosition=CameraPosArg;
            //plus ground height, not accounted for
            //CameraPosition += CameraGroundOffSet;
            //should be an off-set version of whereever the camera is
            Position = CameraPosition + DataValues.CameraModelOffSet;
            MyModel = HumanModel;
            if (Moveable)
            {
                Velocity = Vector3.Zero;
            }
        }
        
        public void ChangeStartPos(Vector3 CameraPosArg)
        {
            CameraPosition=CameraPosArg;
            Position = CameraPosition + DataValues.CameraModelOffSet;
        }

        //moves model and person
        public void MovePerson()
        {
            //velocity is used to get the directional change of the model's position to re-create the bounding box
            GetUserInput();
        }
        //Gets data
        private void GetUserInput()
        {
            Velocity = Vector3.Zero;

            //if the game controller is unplugged, the keyboard/mous combination must be used; automatic change-over for the user
            if (!GamePad.GetState(PlayerIndexValue).IsConnected)
            {
                MoveCamera();
                GetVelocity();
                //for jumping
                Jump();
                //for crouching
                if(!IsForgeModeRunning)
                    Crouch();
                Sprint();

                //throws the object
                if ((this.HoldingObjectTrue) && (LastMouseState.LeftButton != Mouse.GetState().LeftButton) && (Mouse.GetState().LeftButton == ButtonState.Pressed))
                {
                    //calculates the direction to throw it the same way the character's direction is determined
                    int SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;
                    int CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;
                    this.HoldingObject.Velocity += new Vector3((float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * DataValues.ThrowForce), 0, (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * DataValues.ThrowForce));

                    //makes object bouncy (Currently for testing purposes)
                    //this.HoldingObject.Bounce = .8f;

                    //puts it into a freefall
                    this.HoldingObject.freeFall = true;
                    this.HoldingObjectTrue = false;

                    //moves the bounding box
                    if ((HoldingObject.CollisionType == ObjectTypes.Box)&&(HoldingObject.GetType()!=typeof(Wall)))
                        HoldingObject.CalculateBoundingBox();
                }
            }
            //only runs this code if the xbox controller is plugged in
            if(GamePad.GetState(PlayerIndexValue).IsConnected)
            {
                //from testing, the mouse works by locating the x and y positions on the screen while the 
                //xbox controller uses a unit circle of values (Radians of +/-1 for X and Y) as input;
                //subtracting x and adding why gives "regular" controll; reversing signs will result in inverse pitch control
                CameraAngle.X -= (GamePad.GetState(PlayerIndexValue).ThumbSticks.Right.X * DataValues.PitchSensitivity); // PitchSensitivity;
                CameraAngle.Y += (GamePad.GetState(PlayerIndexValue).ThumbSticks.Right.Y * DataValues.PitchSensitivity); // PitchSensitivity;

                if ((CameraAngle.Y > 89.9999f) && (CameraAngle.Y < 180)) CameraAngle.Y = 89.9999f;
                if ((CameraAngle.Y < 270.0001f) && (CameraAngle.Y > 180)) CameraAngle.Y = 270.0001f;

                if (CameraAngle.X >= 361) CameraAngle.X -= 360; else if (CameraAngle.X <= -1) CameraAngle.X += 360;
                if (CameraAngle.Y >= 361) CameraAngle.Y -= 360; else if (CameraAngle.Y <= -1) CameraAngle.Y += 360;

                CameraAngle3.Y = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y));
                float ScalerXZ = (float)Math.Sqrt(1 - Math.Pow(CameraAngle3.Y, 2));
                CameraAngle3.X = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * ScalerXZ;
                CameraAngle3.Z = (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * ScalerXZ;

                int SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;
                int CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;

                //Left stick movement; moves on the x/y plane on the ground
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickLeft))
                {
                    Velocity.X += DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)));
                    Velocity.Z += DataValues.MoveSensitivity * (float)( Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)));
                }
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickRight))
                {
                    Velocity.X -= DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)));
                    Velocity.Z -= DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)));
                }

                SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;
                CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;

                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickUp))
                {
                    Velocity.X += DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X)));
                    Velocity.Z += DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X)));
                }
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftThumbstickDown))
                {
                    Velocity.X -= DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X)));
                    Velocity.Z -= DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X)));
                }
                JumpGamePad();
                CrouchGamePad();
                SprintGamePad();
            }
            //***This is how moving up ramps will work***
            //after receiving the velocity, the user will move foreward
            //SlopeMax will find the amount that the position must increase by in order to 
            //have MaxAngle be the maximum angle that can be climbed
            float SlopeMax = (float)Math.Tan(DataValues.MaxAngle) * DataValues.MoveSensitivity;//*100;
            //by repeatedly rising and moving foreward by small amounts, then checking for
            //intersection, you can climb objects with little to no jump. 
            //also implements gravity while it is checking for slopes
            //checkgravity

            float StoreVX, StoreVZ;
            NormalVector = Vector3.One * 3;
            for (int counter = 0; counter < DataValues.MovePrecision; counter++)
            {
                StoreVX = Velocity.X;
                storeVY = Velocity.Y;
                StoreVZ = Velocity.Z;
                Velocity.Y = 0;
                this.CameraPosition.Y += SlopeMax;
                this.Position = CameraPosition + DataValues.CameraModelOffSet;
                Move();
                this.CameraPosition.Y -= SlopeMax;
                this.Position = CameraPosition + DataValues.CameraModelOffSet;
                if (!CheckEitherSurroundings())
                {
                    this.CameraPosition.Y += SlopeMax;
                    PreventDoubleJump = false;
                }
                Velocity.Y = storeVY;
                Velocity.X = 0;
                Velocity.Z = 0;
                Move();
                this.Position = CameraPosition + DataValues.CameraModelOffSet;
                Velocity.X = StoreVX;
                Velocity.Z = StoreVZ;


                storeVY = Velocity.Y;
                
                //check to see if the jump should still be going on
                /*
                if (this.StartJump == false)
                {
                    if (this.CameraPosition.Y > DataValues.CameraGroundOffSet.Y)
                        this.CameraPosition.Y += DataValues.gravity;
                }*/
                this.Position = CameraPosition + DataValues.CameraModelOffSet;

            }
            //only move up in a jump in a single loop cycle
            StartJump = false;
            //resets velocity so that it is in terms of a unit vector
            Velocity *= DataValues.MovePrecision;
            //allows user to pick up objects infront of him/her
            this.DeterminePickUp();
            //can only run special actions in forge world
            if(IsForgeModeRunning == false)
            {
                //action for special objects
                this.DetermineAction();
            }
            //at the end of the loop cycle, update this
            LastState = Keyboard.GetState();
            LastMouseState = Mouse.GetState();
            GamePadLastState = GamePad.GetState(PlayerIndexValue);
        }
        public void MoveCamera()
        {
            //code that makes the reticule move from the mouse; wrong schuyler, this moves the camera angle in relation to the user, but the reticle is always centered
            CameraAngle.X -= (Mouse.GetState().X - ((int)DataValues.ScreenWidth / 2)) / DataValues.PitchSensitivity;
            CameraAngle.Y -= (Mouse.GetState().Y - ((int)DataValues.ScreenHeight / 2)) / DataValues.PitchSensitivity;

            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                CameraAngle.X++;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                CameraAngle.X--;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                CameraAngle.Y++;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                CameraAngle.Y--;*/

            //center screen; where reticule should go at some point; silly schuyler, the reticle should never move. It's always the center of the screen
            Mouse.SetPosition((int)DataValues.ScreenWidth / 2, (int)DataValues.ScreenHeight / 2);

            //makes sure that the user does not look to far up or down. It would be quite annoying to be looking up, then suddenly turn backwards
            if ((CameraAngle.Y > 89) && (CameraAngle.Y < 180)) CameraAngle.Y = 89;
            if ((CameraAngle.Y < 271) && (CameraAngle.Y > 180)) CameraAngle.Y = 271;

            //makes sure that the camera angle never exceeds 360 or is less than 0
            if (CameraAngle.X >= 361) CameraAngle.X -= 360; else if (CameraAngle.X <= -1) CameraAngle.X += 360;
            if (CameraAngle.Y >= 361) CameraAngle.Y -= 360; else if (CameraAngle.Y <= -1) CameraAngle.Y += 360;
        }
        private Boolean Move()
        {
            //PersonalShield.Center = Position + Velocity;
            CameraPosition += Velocity;
            //changes position that is used to create the bounding box
            Position = CameraPosition + DataValues.CameraModelOffSet;
            CanMove = true;
            Vector3 Normal;
            this.MovingBoxes[0].Orientation = Quaternion.Identity;//RotationQuaternion;
            if ((Normal=CheckEitherSurroundingsNormal())!=Vector3.Zero)
            {
                if (!CanMove)
                {
                    PreventDoubleJump = false;
                    NormalVector = (float)Math.Sqrt(Velocity.X * Velocity.X + Velocity.Z * Velocity.Z) * new Vector3(Math.Sign(Velocity.X), 0, Math.Sign(Velocity.Z)) * Normal * 1.01f;
                    CameraPosition -= Velocity;//(float)Math.Sqrt(Velocity.X * Velocity.X + Velocity.Z * Velocity.Z) * new Vector3(Math.Sign(Velocity.X), 0, Math.Sign(Velocity.Z)) * Normal*25;
                    Position = CameraPosition + DataValues.CameraModelOffSet;
                    Velocity.Y = 0f;
                }
                
            }
            else
                CanMove = true;
            this.MovingBoxes[0].Orientation = Quaternion.Identity;
            if (CanMove)
            {
                //in this case, the personal collision shield allows user to pick up objects seen directly in front of him
                if (HoldingObjectTrue)
                {
                    //***********************************************************************
                    //note to self: this system needs a way to correct collisions when done by moving the user's looking direction
                    ScreenModel CheckArg = HoldingObject.CheckCollision(VisibleObjects);
                    if (CheckArg == null)
                    {
                        //code that will move/rotate the object, if picked up
                        this.MoveObject();
                    }
                    else
                    {
                        //if velocity is in the opposite direction of the object that is being hit, then allow movement
                        float InitDist = Vector3.Distance(Position,CheckArg.Position);
                        float FinalDist = Vector3.Distance(Position + Velocity,CheckArg.Position);
                        if (InitDist<FinalDist)
                        {
                            this.MoveObject();
                        }
                        //else, then don't allow movement
                        else
                        {
                            //if a collision occurs with the object being held, reset the player like when the player moves into something
                            PersonalShield.Center = Position - Velocity;
                            CameraPosition -= Velocity;
                            Position = CameraPosition + DataValues.CameraModelOffSet;
                            this.MoveObject();
                            return (false);
                        }
                    }
                }
                return (true);
            }
            //if not, don't move; reset the moving box to the correct position
            else
            {
                //moves the shield back to the previous position
                //PersonalShield.Center = Position - Velocity;
                //CameraPosition -= Velocity;
                Position = CameraPosition + DataValues.CameraModelOffSet;
                Velocity.Y = 0;
                return (false);
            }
        }
        private void GetVelocity()
        {
            Velocity.Y = storeVY;
            //gets the 3d ratios of the camera angles.
            CameraAngle3.Y = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.Y));
            float ScalerXZ = (float)Math.Sqrt(1 - Math.Pow(CameraAngle3.Y, 2));
            CameraAngle3.X = (float)Math.Sin(MathHelper.ToRadians(CameraAngle.X)) * ScalerXZ;
            CameraAngle3.Z = (float)Math.Cos(MathHelper.ToRadians(CameraAngle.X)) * ScalerXZ;

            //because sin and cos are squared, this will make sure that the appropriate directions are applied
            int SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;
            int CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)) < 0) ? -1 : 1;

            //actual floor movement using the WASD keys
            //Technically the ground plane uses the x and z planes,
            //So we use the postulate that sin squared plus cos squared is
            //always equivelant to one to ensure that the user moves at a constant speed
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Velocity.X += DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)));
                Velocity.Z += DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Velocity.X -= DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X + 90)));
                Velocity.Z -= DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X + 90)));
            }

            //rotates the values of front and back for when you are moving in a seperate direction
            SinPos = (Math.Sin(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;
            CosPos = (Math.Cos(MathHelper.ToRadians(CameraAngle.X)) < 0) ? -1 : 1;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Velocity.X += DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X)));
                Velocity.Z += DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X)));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Velocity.X -= DataValues.MoveSensitivity * (float)(Math.Sin(MathHelper.ToRadians(CameraAngle.X)));
                Velocity.Z -= DataValues.MoveSensitivity * (float)(Math.Cos(MathHelper.ToRadians(CameraAngle.X)));
            }
            if (Velocity.Y == 0)
                Velocity.Y = DataValues.PGravity;
            if ((Velocity.Y < .004f) && (Velocity.Y > -.004f))
                Velocity.Y *= -1;
            if (Velocity.Y > 0)
                Velocity.Y /= DataValues.GravityAcceleration;
            else
                Velocity.Y *= DataValues.GravityAcceleration;
            if (Math.Abs(Velocity.Y) > Math.Abs(DataValues.TerminalVelocity))
                Velocity.Y = Math.Abs(DataValues.TerminalVelocity) * Math.Sign(Velocity.Y);
            /*
            if (Position.Y < 0)
                Velocity.Y = 0;
             */
            RotationQuaternion = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(0)*Matrix.CreateRotationY((float)Math.Atan2(Velocity.Z, Velocity.X))*Matrix.CreateRotationZ(0)*Matrix.Identity);
        }

        //methods for jumping/crouching
        //----------------------------------------------------------------------
        //for jumping
        private void Jump()
        {
            //for jumping by aNtHoNy (rewritten by Nick <3)
            //********************************************************
            //jump moves up once per clock cycle; jump "terminates" in Nick's precision checking
            if (((Keyboard.GetState().IsKeyDown(Keys.Space)) && (!LastState.IsKeyDown(Keys.Space))) && (StartJump == false) && (PreventDoubleJump == false))
            {
                Velocity.Y += DataValues.JumpIncrement;// * DataValues.MoveSensitivity;
                StartJump = true;
                //prevents a double jump
                PreventDoubleJump = true;
            } 
        }
        private void JumpGamePad()
        {
            if (((GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.A)) && (!GamePadLastState.IsButtonDown(Buttons.A))) && (StartJump == false) && (PreventDoubleJump == false))
            {
                Velocity.Y += DataValues.JumpIncrement * DataValues.MoveSensitivity;
                StartJump = true;
                //prevents a double jump
                PreventDoubleJump = true;
            } 
        }
        //method for crouching
        private void Crouch()
        {
            // crouch down
            if ((Keyboard.GetState().IsKeyDown(Keys.LeftControl)) && (!LastState.IsKeyDown(Keys.LeftControl)) && (Crouched == false))
            {
                //activates mode
                Crouched = true;
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.LeftControl)) && (!LastState.IsKeyDown(Keys.LeftControl)) && (Crouched == true))
            {
                //deactivates mode
                Crouched = false;
            }
            if (Crouched)
            {
                //velocity reduced while crouched, by a SCALAR! I <3 Vector Calculus!
                Velocity *= DataValues.CrouchedReduction;
            }
        }
        private void CrouchGamePad()
        {
            // crouch down
            if ((GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.RightStick)) && (Crouched == false))
            {
                //activates mode
                Crouched = true;
            }
            else if (GamePad.GetState(PlayerIndexValue).IsButtonUp(Buttons.RightStick) && (Crouched == true))
            {
                //deactivates mode
                Crouched = false;
            }
            if (Crouched)
            {
                //velocity reduced while crouched, by a SCALAR! I <3 Vector Calculus!
                Velocity *= DataValues.CrouchedReduction;
            }
        }
        //works similarily, but for sprinting
        private void Sprint()
        {
            //can't sprint if jumping or crouching
            if (Crouched == false)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    //checks that are kept in place that limit how one can sprint
                    if (SprintTimer.MilliSeconds == 0f)
                    {
                        //starts the timer off
                        SprintTimer.Start();
                    }
                    //can't run once time has been exceeded
                    if (SprintTimer.MilliSeconds < DataValues.SprintTime)
                    {
                        //it is valid to sprint at this time
                        Velocity.X *= DataValues.SprintFactor;
                        Velocity.Z *= DataValues.SprintFactor;  
                    }
                }
                //kills sprinting when key is lifted
                else
                {
                    //resets timer for a new use
                    SprintTimer.Reset();
                }
            }
        }
        private void SprintGamePad()
        {
            //can't sprint if jumping or crouching
            if (Crouched == false)
            {
                if (GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.LeftStick))
                {
                    //checks that are kept in place that limit how one can sprint
                    if (SprintTimer.MilliSeconds == 0f)
                    {
                        //starts the timer off
                        SprintTimer.Start();
                    }
                    //can't run once time has been exceeded
                    if (SprintTimer.MilliSeconds < DataValues.SprintTime)
                    {
                        //it is valid to sprint at this time
                        Velocity.X *= DataValues.SprintFactor;
                        Velocity.Z *= DataValues.SprintFactor;  
                    }
                }
                //kills sprinting when key is lifted
                else
                {
                    //resets timer for a new use
                    SprintTimer.Reset();
                }
            }
        }
        //----------------------------------------------------------------------

        //actual collision checks
        //----------------------------------------------------------------------
        //constantly checks surroundings to add objects in and out of the list; bounding box will be used to check further specifics
        //*********before this is called, the Visible objects list should containt a list of all collision based objects in a load space
        private bool CheckSurroundingsBoundingBox()
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < NearObjects.Count; cntr++)
            {
                if (NearObjects[cntr].CheckCollision(this))
                {
                    return (false);
                }
            }
            return (true);
        }
        private bool CheckSurroundingsBoundingSpheres()
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < NearObjects.Count; cntr++)
            {
                if (NearObjects[cntr].CheckBoundingSpheres(PersonalShield))
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
            for (int cntr = 0; cntr < NearObjects.Count; cntr++)
            {
                //Check this logic out...try using the series of bounding boxes *Nick Start Here*
                if (NearObjects[cntr].CheckCollision(this))
                {
                    //allows us to move moving objects                  
                    CheckMovingOjects(NearObjects[cntr]);
                    return (false);
                }
            }
            return (true);
        }
        public Vector3 CheckEitherSurroundingsNormal()
        {
            Vector3 Normal;
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < NearObjects.Count; cntr++)
            {
                //Check this logic out...try using the series of bounding boxes *Nick Start Here*
                if ((Normal = NearObjects[cntr].CheckCollisionNormal(this)) != Vector3.Zero)
                {
                    //allows us to move moving objects
                    CheckMovingOjects(NearObjects[cntr]);
                    CanMove = ((NearObjects[cntr].Moveable)); //&& (NearObjects[cntr].CheckCollision(VisibleObjects)==null));
                    return (Normal);
                }
            }
            return (Vector3.Zero);
        }
        //same idea, but instead returns the screen model needed, not the
        public ScreenModel CheckEitherSurroundings(bool ReturnObject)
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < NearObjects.Count; cntr++)
            {
                //Check this logic out...try using the series of bounding boxes
                if (NearObjects[cntr].CheckCollision(this))
                {
                    //allows us to move moving objects                  
                    CheckMovingOjects(NearObjects[cntr]);
                    return (NearObjects[cntr]);
                }
            }
            return (null);
        }
        //---------------------------------------protype copy
        public bool CheckEitherSurroundings(ScreenModel CheckArg, ScreenModel PCheckArg)
        {
            //loop that checks the list for chrashing collisions
            for (int cntr = 0; cntr < NearObjects.Count; cntr++)
            {
                //Check this logic out...try using the series of bounding boxes
                if ((NearObjects[cntr].CheckCollision(CheckArg)) && (NearObjects[cntr] != CheckArg) && (NearObjects[cntr] != PCheckArg))
                {
                    //allows us to move moving objects                  
                    CheckMovingOjects(NearObjects[cntr]);
                    return (false);
                }
            }
            return (true);
        }
        //-----------------------------------end prototype
        //awesomeness; if an object is moveable and is moving and pushes the person with force, then move the person!
        //*****TO BE CALLED WHEN A COLLISION HIT HAS BEEN CONFIRMED********
        public void CheckMovingOjects(ScreenModel CheckArg)
        {
            if ((CheckArg.Moveable)&&(CheckArg.Velocity!=Vector3.Zero))
            {
                //moves camera and entities with it
                PersonalShield.Center = Position + CheckArg.Velocity;
                CameraPosition += CheckArg.Velocity;
                Position = CameraPosition + DataValues.CameraModelOffSet;
                //moves the object based on any forced applied by the user, running in the opposite direction
                //for (int counter = 0; counter < DataValues.MovePrecision; counter++)
                    CheckArg.MoveModelBox(this.Velocity);
            }
            //move objects that can be moved when I apply force to them
            if ((CheckArg.Moveable) && (CheckArg.Velocity == Vector3.Zero))
            {
                //for (int counter = 0; counter < DataValues.MovePrecision; counter++)
                    CheckArg.MoveModelBox(this.Velocity);

                CheckEitherSurroundings(CheckArg, PCheckArg);
                PCheckArg = CheckArg;
            }  
        }
        //--------------------------------------------------------------------------------------------------------------

        //----------------------------------------------------------------------      
        //method that checks if the object is visibly in front of the user
        //when the bool argument is true, it will return values/change the reticle based on an object that has a special action,
        //not whether or not it should be picked up
        public ScreenModel ObjectIsInfront()
        {
            //******CONCEPT: *********
            //1) "Highlights" a single possible object that can be picked up
            //this should be the object that best lines up with the center of the reticle
            ScreenModel TestModel = null;
            //Takes the list of a few objects within the threshold and finds the one that is the shortest distance away
            float? ClosestDist = float.MaxValue;
            //list of screen models that 
            ScreenModel TestModelClosest = null;
            //distance between objects, if found
            float? Distance = 0f;
            Ray TestRay = new Ray(CameraPosition, CameraAngle3);
            //Console.WriteLine(TestRay.ToString());
            for (int cntr = 0; cntr < this.NearObjects.Count; cntr++)

            {
                if (((NearObjects[cntr].PickUpable) || (NearObjects[cntr].GetType() == typeof(Door)) || (NearObjects[cntr].GetType() == typeof(ComputerConsole))) || IsForgeModeRunning)
                if (this.NearObjects[cntr].CollisionType == ObjectTypes.Box)
                {
                    //checks bounding boxes, if applicable
                    if (this.NearObjects[cntr].GetType() == typeof(Wall))
                    {
                        foreach (OrientedBoundingBox Box in NearObjects[cntr].MovingBoxes)
                            if ((Distance = Box.Intersects(TestRay)) != null)
                            {
                                TestModel = this.NearObjects[cntr];
                                //3) Checks to see if the object is within a specified distance away using a ray
                                //Uses the position of the model and a direction vector (in front of the camera) to find distance away
                                if ((TestModelClosest != null) && (TestModelClosest.GetType() == typeof(Wall)) && (DataValues.VectorDistance(TestModelClosest.MovingBoxes[0].HalfExtent) > DataValues.VectorDistance(Box.HalfExtent)))
                                {
                                    //stores the closest of the models at the same time as finding models that 
                                    //are within the threshold; more effecient than going through yet another list
                                    TestModelClosest = TestModel;
                                    ClosestDist = Distance - DataValues.VectorDistance(Box.HalfExtent);
                                    break;
                                }
                                if ((Distance - DataValues.VectorDistance(Box.HalfExtent) <= DataValues.MaxObjectDistance) && (Distance - DataValues.VectorDistance(Box.HalfExtent) < ClosestDist))
                                {
                                    //stores the closest of the models at the same time as finding models that 
                                    //are within the threshold; more effecient than going through yet another list
                                    TestModelClosest = TestModel;
                                    ClosestDist = Distance - DataValues.VectorDistance(Box.HalfExtent);
                                    break;
                                }
                            }
                    }
                    else 
                            if (((Distance = this.NearObjects[cntr].GetBoundingBoxes().Intersects(TestRay)) != null))
                            {
                                TestModel = this.NearObjects[cntr];
                                //3) Checks to see if the object is within a specified distance away using a ray
                                //Uses the position of the model and a direction vector (in front of the camera) to find distance away
                                if((TestModelClosest!=null)&&(TestModelClosest.GetType()==typeof(Wall))&&(DataValues.VectorDistance(TestModelClosest.MovingBoxes[0].HalfExtent)>Distance))
                                {
                                    TestModelClosest = TestModel;
                                    ClosestDist = Distance;
                                    break;
                                }
                                if ((Distance <= DataValues.MaxObjectDistance) && (Distance < ClosestDist))
                                {
                                    //stores the closest of the models at the same time as finding models that 
                                    //are within the threshold; more effecient than going through yet another list
                                    TestModelClosest = TestModel;
                                    ClosestDist = Distance;
                                    break;
                                }
                            }
                }
                //idk if we still use this but let's just leave it in case...
                else
                {
                    if (TestRay.Intersects(this.NearObjects[cntr].GetBoundingSpheres()) != null)
                    {
                        TestModel = this.NearObjects[cntr];
                        Distance = TestRay.Intersects(TestModel.GetBoundingSpheres());
                        if ((Distance <= DataValues.MaxObjectDistance) && (Distance < ClosestDist))
                        {
                            //stores the closest of the models at the same time as finding models that 
                            //are within the threshold; more effecient than going through yet another list
                            TestModelClosest = TestModel;
                            ClosestDist = Distance;
                        }
                    }
                }
            }
            //after one has found the closest, re-assing test model
            TestModel = TestModelClosest;
            //2) Checks object to see if it is of pickupable type
            //or if it's a special object with an action
            if (TestModel == null)
            {
                //object cannot be picked up
                this.ReticleToDraw = DataValues.ReticleType.Normal;
                return (null);
            }
            //Console.WriteLine(TestModel.GetType().ToString());
            //checks if it is in that distance...and is a regular screem model object
            if ((ClosestDist <= DataValues.MaxObjectDistance) && ((TestModel.GetType() == typeof(ScreenModel)) || (TestModel.GetType() == typeof(Wall))) && (TestModel.PickUpable == true))
            {
                //object can be picked up, change the reticle
                this.ReticleToDraw = DataValues.ReticleType.ObjectPickup;
                //return the model
                return (TestModel);
            }
            //checks if it is in that distance if it's a special screen model object
            if ((ClosestDist <= DataValues.MaxObjectDistance) && ((TestModel.GetType() != typeof(ScreenModel)) || (TestModel.GetType() != typeof(Wall))))
            {
                //changes color while holding the object
                this.ReticleToDraw = DataValues.ReticleType.ActionBtn;
                //you can only edit walls in forge world...this hides the reticle when the user is not in forge world
                if (TestModel.GetType() == typeof(Wall))
                    this.ReticleToDraw = ((Wall)TestModel).SpecialReticle(IsForgeModeRunning);
                //return the model
                return (TestModel);
            }
            //or return nothing, our friend null, which will cause an exception to be caught, acting as a general check
            //object cannot be picked up
            if (!this.HoldingObjectTrue)
                this.ReticleToDraw = DataValues.ReticleType.Normal;
            return (null);
        }
        //method that provides logic in picking up/dropping of an object, called in main program
        public void DeterminePickUp()
        {
            //uses a local variable so that this.HoldingObject is not changing unless someone presses F
            ScreenModel ThisModel=null;
            //one cannot hold multiple/different objects
            if (!HoldingObjectTrue)
            {
                ThisModel = ObjectIsInfront();
            }
            else if (DataValues.IsPressed(Keys.F))
            {
                DropObject();
                return;
            }

            if (ThisModel != null)
            {
                //as this method will also return objects that are not pickup-able (like special "Action" objects) this check is needed
                if ((ThisModel.PickUpable == true)||(IsForgeModeRunning))
                {
                    if (((!GamePad.GetState(PlayerIndexValue).IsConnected) && (((DataValues.IsPressed(Keys.F)) && (!LastState.IsKeyDown(Keys.F)))) ||
                        ((GamePad.GetState(PlayerIndexValue).IsConnected) && (((GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.X)) && (!GamePadLastState.IsButtonDown(Buttons.X)))))))
                    {
                        this.HoldingObject = ThisModel;
                        //1st hit, pick it up
                        if (!HoldingObjectTrue)
                        {
                            PickUpObject();
                        }
                        //2nd hit, drop the object
                        else
                        {
                            DropObject();
                        }
                    }
                }
            }
        }
        //----------------------------------------------------------------
        //determines the action to be preformed for special cases...that is objects or screen models that 
        private void DetermineAction()
        {
            //this if determines if the object in front of the users is a special case (a subclass of the ScreenModel Class)
            ScreenModel SpecialObject = ObjectIsInfront();
            if (SpecialObject != null)
            {
                if (SpecialObject.GetType() != typeof(ScreenModel))
                {
                    //gets the action key from the user
                    if (((LastMouseState.LeftButton != Mouse.GetState().LeftButton) && (Mouse.GetState().LeftButton == ButtonState.Pressed)) ||
                        ((GamePad.GetState(PlayerIndexValue).IsConnected) && (((GamePad.GetState(PlayerIndexValue).IsButtonDown(Buttons.Y)) && (!GamePadLastState.IsButtonDown(Buttons.Y))))))
                    {   
                        //switch back reticle, unless dictated by this special method
                        this.ReticleToDraw = SpecialObject.SpecialReticle();
                        //special case of a wall; you can change the wall in forge but not in regular mode
                        if (SpecialObject.GetType() == typeof(Wall))
                            this.ReticleToDraw = ((Wall)SpecialObject).SpecialReticle(IsForgeModeRunning);
                        //not sure why nick did this...but w/e...maybe he just doesn't get polymorphism yet
                        else if (SpecialObject.GetType() == typeof(Door))
                            ((Door)SpecialObject).Action(Content, VisibleObjects, graphics,this);
                        else
                            SpecialObject.Action(this.CurrentRoom, Content, models, graphics);
                    }
                }
            }
        }

        //----------------------------------------------------------------
        //method that allows the user to pick up objects 
        public void PickUpObject()
        {
            HoldingObject.Velocity = Vector3.Zero;
            //tells the object that it is being held
            HoldingObject.IsBeingHeld = true;
            HoldingObjectTrue = true;
            float Distance=4f;
            if (this.HoldingObjectDistanceSet)
                if (HoldingObject.CollisionType == ObjectTypes.Box)
                {
                    if (DataValues.VectorDistance(HoldingObject.GetBoundingBoxes().Max - HoldingObject.Position) + 4 > Distance)
                        Distance = 4f + DataValues.VectorDistance(HoldingObject.GetBoundingBoxes().Max - HoldingObject.Position);
                }
                else
                {
                    foreach (ModelMesh Mesh in HoldingObject.MyModel.Meshes)
                        if (Mesh.BoundingSphere.Radius + 4 > Distance)
                            Distance = 4f + Mesh.BoundingSphere.Radius;
                }
            else
                Distance = DataValues.VectorDistance(CameraPosition - HoldingObject.Position - Velocity);
            //Ray TestRay = new Ray(CameraPosition, CameraAngle3);
            HoldingObject.Position = CameraPosition + (Distance * CameraAngle3);

            //HoldingObject.MoveModelBox(Position-HoldingObject.Position);

            //moves model box if this is a box colliding type
            if (HoldingObject.CollisionType == ObjectTypes.Box)
            {
                //first time the last position has to be tracked
                HoldingObjectLastPos = HoldingObject.Position;
            }

            //changes color while holding the object
            this.ReticleToDraw = DataValues.ReticleType.HoldingObj;
        }
        //moves the object with the user when the user moves
        public void MoveObject()
        {
            //the object's position is infront of you by a set distance infront of you
            //calculate the change vector of the current possition to the new position (floating in space)

            //uses the ray to place the object a set distance away from the 
            float Distance;
            if (this.HoldingObjectDistanceSet)
                Distance = 4f;
            else
                Distance = DataValues.VectorDistance(CameraPosition - HoldingObject.Position - Velocity);
            //Ray TestRay = new Ray(CameraPosition, CameraAngle3);
            HoldingObject.Position = CameraPosition + (Distance * CameraAngle3);

            if (this.HoldingObjectDistanceSet)
                HoldingObject.modelRotation.Y = MathHelper.ToRadians(CameraAngle.X);
            
        }


        //method that allows the user to drop objects
        public void DropObject()
        {
            //tells the object that it is not being held
            HoldingObject.IsBeingHeld = false;
            //the object is no longer being held
            HoldingObjectTrue = false;
            //set the object to fall; this will be picked up in the update method for this object and thus independent from the user's control
            HoldingObject.IsCurrentlyFalling = true;

            //recreates a bounding box if needed (but bounding sphere always works)
            if (HoldingObject.CollisionType == ObjectTypes.Box)
            {
                //anthony's much easier version
                HoldingObject.BoundingSetup();
                
                //correction for position
                //---------------------------------------------
                //methods that handle rotation of the bounding box
                //HoldingObject.ConverToOBB();
                //HoldingObject.ConverToABB();
                /*
                //takes the initial position of the bounding box (when loaded)
                Vector3 ChangeVect = HoldingObject.Position - HoldingObject.LocalPos;
                //translates the change for the bounding box
                HoldingObject.MovingBox.Min += ChangeVect;
                HoldingObject.MovingBox.Max += ChangeVect;
                */
                //restores that initial box (current position)
                //HoldingObject.LocalPos = HoldingObject.Position;
                //HoldingObject.LocalBox = HoldingObject.MovingBox;
                //---------------------------------------------
            }
            //changes color back to white...which will get changed next loop cycle anyway....
            this.ReticleToDraw = DataValues.ReticleType.Normal;
        }
        //----------------------------------------------------------------------

        //****************
        //force the person to move outside of this class..like in forge
        public void ForceMovement(Vector3 VelocityArg)
        {
            for (int counter = 0; counter < DataValues.MovePrecision; counter++)
            {
                this.Velocity = VelocityArg;
                Move();
            }
        }
    }
}
