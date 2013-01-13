using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DataTypes;

using System.Threading;
using DataTypes.Special_Objects;

namespace Senior_Project
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //The master list of all models currently loaded
        List<ScreenModel> models = new List<ScreenModel>();

        //this is a subset of the models list composed of models that are close to the user. They will have higher quality models.
        List<ScreenModel> modelsHighRes = new List<ScreenModel>();

        //this is a subset of the models list composed of models that are far away from the user. They will have lower quality models.
        List<ScreenModel> modelsLowRes = new List<ScreenModel>();
        List<List<OrientedBoundingBox>> BoundingBoxModels = new List<List<OrientedBoundingBox>>(0);
        //An xml file reader used for unloading the content from each room
        public XmlTextReader RoomContent;

        //Gets the previous state of the keyboard, so a button may be held down without a function repeating
        public KeyboardState LastState;
        //determines if you are in forge mode
        public Boolean RunForge;

        public GraphicsDeviceManager graphics;

        //new code that Anthony and I added in for nightvision and the FPS Counter
        // fps stuff
        public int framecntr = 0;
        public int framerate = 0;
        TimeSpan ElapsedTime = TimeSpan.Zero;
        
        // night vision stuff
        bool NV = false;
        Color FontColor = Color.White;
        Color BackColor = Color.Black;
        Color SelectColor = Color.DarkRed;

        SpriteBatch spriteBatch;
        //for drawing the game reticle to the screen
        Texture2D GameReticle;
        BasicEffect effects;
        Effect LightingEffect;
        //and for the portal reticles...nick don't kill me yet I've had a very long weekend
        Texture2D PBlueReticle;
        Texture2D POrangeReticle;
        Texture2D PBothReticle;
        Texture2D PEmptyReticle;

        //SongCollection SongList = new SongCollection();
        List<Song> SongList = new List<Song>(0);

        int SongCounter = 0;

        PlayerIndex FirstPlayer = new PlayerIndex();
        

        //Thread BoundingBoxesThread;

        //gets the currect state of the mouse so that the change in camera angle may be assessed 
        public MouseState MouseCurrent;
        //public Vector3 modelPosition = new Vector3(0, 1, 0);
        //public Vector3 position = Vector3.Zero;

        public ScreenUtility MyScreen;
        //public BasicPhysicsClass MyPhysics = new BasicPhysicsClass();
        //runs collision tests for the camera
        public CollisionCheckSystem MyPerson;
        //game's aspect ratio; so very redudant
        public float aspectRatio;
        //used to delete the game screen once; during the start of the draw method
        private bool SplashScreenDeleted = false;
        TimerClass gameClock;
        //values that measure the progress of loading content
        //Items loaded thus far
        private int ItemsLoaded = 0;
        //calculated from the list of objects to be loaded (visible objects) and any additional objects
        private int TotalItems = 4;//initial two objects not in the "models" list: the game reticle and the "player" model; +4 portal objects
        //holds distance of the near and far viewing frustum plane
        //public float FrontPlane = 0.25f;
        //public float FarPlane = 1000f;//10000.0f;
        //moving 

        public ForgeClass ForgeObj = new ForgeClass();//;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //set to full screen in game constructor
            MyScreen = new ScreenUtility(FirstPlayer);
            //....uhhh <- Don't set to full screen yet. WAAAAY too many bugs. 
            MyScreen.SetGameFullScreen(graphics);
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize objects here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        //this is no longer a void type due to the need to use the yield "iterator" keyword
        protected override void LoadContent()
        {
            //sets mouse to center screen...so you don't look up
            Mouse.SetPosition(-MyScreen.FindCenterScreen().X, MyScreen.FindCenterScreen().Y);
            gameClock = new TimerClass();
            //displays initial 0%
            MyScreen.RunSplashScreen(0f,graphics);
            //person controls initialized

            MyPerson = new CollisionCheckSystem(Content.Load<Model>("HumanRectangle"), new Vector3(5.0f, 3.0f, 0.0f), Content, models, graphics);
            
            //Total amount of objects to be loaded in the game; populated by the data file(s) used
            TotalItems += CountModels(MyPerson.CurrentRoom);
            TotalItems += CountSongs();
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            //increments loaded items
            LoadProgress();
            MouseCurrent = Mouse.GetState();
            LastState = new KeyboardState();
            //loads models in a file
            LoadNewModel(MyPerson.CurrentRoom);
            //person moves to center of the room
            MyPerson.ChangeStartPos(new Vector3(-400, 99, -250));//Door.FindCenterRoom(models.Find(Door.FindTypeOfDoor)));
            //adds all objects currently loaded to the collision check system (for the person)
            MyPerson.VisibleObjects = models;
            DataTypes.DataValues.ScreenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            DataTypes.DataValues.ScreenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            MyPerson.Position = new Vector3(0, 10, 0);
            //--------------------------------------------------------------
            //For drawing the reticle
            //used in load content method
            spriteBatch = new SpriteBatch(GraphicsDevice);
            DataValues.Font = Content.Load<SpriteFont>(@"Fonts\Font");
            // TODO: use this.Content to load your game content here            
            GameReticle = this.Content.Load<Texture2D>(FolderNames.Textures + FolderNames.Reticles + "ReticleBetter");
            LoadProgress();
            //oh and the 3 portal reticles to....to be coded later...
            PBlueReticle = this.Content.Load<Texture2D>(FolderNames.Textures + FolderNames.Reticles + "PortalBlue");
            LoadProgress();
            POrangeReticle = this.Content.Load<Texture2D>(FolderNames.Textures + FolderNames.Reticles + "PortalOrange");
            LoadProgress();
            PBothReticle = this.Content.Load<Texture2D>(FolderNames.Textures + FolderNames.Reticles + "PortalBoth");
            LoadProgress();
            PEmptyReticle = this.Content.Load<Texture2D>(FolderNames.Textures + FolderNames.Reticles + "PortalEmpty");
            ForgeObj.Initialize(spriteBatch, Content.Load<Texture2D>(@"Textures\Blank"), Content,FontColor,SelectColor,BackColor);
            LoadProgress();
            //models.Add(new Wall("Wall", "wall", Content.Load<Model>(@"Models\HighRes\wall"), new Vector3(5, 5, 5), new Vector3(0f,1.5f,.5f), GraphicsDevice, true, true, .8f, true, "Wall{Length:3;Height:1;Width:5;}"));
            //models.Add(new Wall("Wall", "wall", Content.Load<Model>(@"Models\HighRes\wall"), new Vector3(5, 5, 5), Vector3.Zero, GraphicsDevice, true, true, 3,1,5));
            //--------------------------------------------------------------
            effects = new BasicEffect(GraphicsDevice);
            LoadProgress();
            DataValues.LastState = Keyboard.GetState();
            // loads the lighting effect HLSL file Ambient.fx
            // this is what is used to light wall objects only
            LightingEffect = Content.Load<Effect>("Effects/Ambient");
            GetMusic();
            DataValues.UserPosition = MyPerson.Position;
            DataValues.HudItems = new String[] { "", "", "", "", ""}.ToList<String>();
            DataValues.Hud = new ScreenMenu(DataValues.HudItems, new Vector2(DataTypes.DataValues.ScreenWidth - 300, DataTypes.DataValues.ScreenHeight - 110), DataValues.Font, 5, 0, FontColor, FontColor, BackColor, 12, Content.Load<Texture2D>(@"Textures\Blank"));
        }
        public void GetMusic()
        {
            List<Song> Songs = new List<Song>(0);
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "..\\..\\Files\\Music");
            FileInfo[] files = dir.GetFiles();
            for (int x=0; x<files.Length; x++)
            {
                try
                {
                    String Filename = Path.GetFileName(files[x].Name);
                    if (Filename.Contains(".mp3") || Filename.Contains(".wav"))
                        Songs.Add(Song.FromUri(Path.GetFileNameWithoutExtension(files[x].Name), new Uri("Files/Music/" + Filename, UriKind.Relative)));
                    LoadProgress();
                }
                catch(Exception e)
                {
                }
            }
            int spot;
            while (Songs.Count > 0)
            {
                SongList.Add(Songs.ElementAt(spot = DataValues.Rnd.Next(Songs.Count)));
                Songs.RemoveAt(spot);
            }
        }
        public int CountSongs()
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "..\\..\\Files\\Music");
            FileInfo[] files = dir.GetFiles();
            return(files.Length);
        }
        //to be called by the yield keyword; acts like a thread...
        //measures progress made by the content loaded
        public float LoadProgress()
        {
            ItemsLoaded++;
            //Console.WriteLine("Loaded: " + ItemsLoaded);
            //Console.WriteLine("Total: " + TotalItems);
            float PercentLoaded = ((float)ItemsLoaded / (float)TotalItems);
            //Console.WriteLine(PercentLoaded);
            MyScreen.SplScreen.UpdatePercent(PercentLoaded);
            return(PercentLoaded);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        Song CurrentSong;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //reset control for the frame counter
            ElapsedTime += gameTime.ElapsedGameTime;
            if (ElapsedTime > TimeSpan.FromSeconds(1))
            {
                ElapsedTime -= TimeSpan.FromSeconds(1);
                framerate = framecntr;
                framecntr = 0;
            }
            if (DataValues.IsPressed(Keys.F12))
                MediaPlayer.Stop();
            //schuyler is awesome and added pausing
            if (DataValues.IsPressed(Keys.F11))
            {
                //System.Windows.Forms.MessageBox.Show(MediaPlayer.State.ToString());
                if (MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();//Play(CurrentSong = SongList.ElementAt(SongCounter));
                else if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
            }
            if (DataValues.IsPressed(Keys.F10))
            {
                MediaPlayer.Stop();
                SongCounter -= 2;
            }
            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(CurrentSong = SongList.ElementAt(++SongCounter % SongList.Count));

            // enables and disables night vision mode
            if (DataValues.IsPressed(Keys.R))
            {
                if (MyScreen.GamePaused == false)
                    NV = !NV;
            }
            if (NV)
            {
                FontColor = Color.Black;
                SelectColor = Color.DarkRed;
                BackColor = Color.White;
            }
            else
            {
                FontColor = Color.White;
                SelectColor = Color.DarkRed;
                BackColor = Color.Black;
            }
            
            //check to pause the game once another form is opened (such as the computer console)
            //Console.WriteLine(System.Windows.Forms.Application.OpenForms.Count);
            //apparently the Game counts as one (and the console doesn't)!!!!!!!!!!!!!!!!!!!
            if (System.Windows.Forms.Application.OpenForms.Count > 1)
            {
                MyScreen.GamePaused = true;
            }
            else
                MyScreen.GamePaused = false;
            //allows us to move out of the game frame while testing
            if ((!Keyboard.GetState().IsKeyDown(Keys.LeftAlt)) && (this.IsActive))
            {
                //very simple check to see if the game is paused; if it is, no code is preformed in the update method
                if (MyScreen.GamePaused == false)
                {

                    foreach (ScreenModel ThisModel in models)
                        ThisModel.Sphere = ThisModel.GetBoundingSpheres();

                    foreach (ScreenModel ThisModel in models)
                        if (!(MyPerson.HoldingObjectTrue && ThisModel == MyPerson.HoldingObject) && (!ThisModel.IsLevitating))
                        {
                            //ThisModel.MoveModelBox();
                            ThisModel.CheckSurroundings(models);
                        }

                    if (Keyboard.GetState().IsKeyDown(Keys.B))
                    {
                        BoundingBoxModels.Clear();
                        List<OrientedBoundingBox> BigBoundingBox = new List<OrientedBoundingBox>(0);
                        foreach (ScreenModel ThisModel in models)
                        {
                            BoundingBoxModels.Add(ThisModel.MovingBoxes);
                            if (ThisModel.GetType() == typeof(Door))
                            {
                                List<OrientedBoundingBox> DoorBox = new List<OrientedBoundingBox>(0);
                                DoorBox.Add(Door.GetBoundingBoxOther((Door)ThisModel));
                                BoundingBoxModels.Add(DoorBox);
                            }
                        }
                        BoundingBoxModels.Add(BigBoundingBox);
                        BoundingBoxModels.Add(MyPerson.MovingBoxes);
                    }

                    //opens up the pause menu when escape key is pressed
                    MyScreen.CreateEscKey(this, this.graphics);
                    //opens up the pause menu when Start Button is Pressed (xbox controller)
                    MyScreen.CreateStartBtn(this, this.graphics);
                    //test
                    //models[1].DrawBoundingBox(MyPerson.CameraPosition, MyPerson.CameraAngle3, MyPerson.aspectRatio);
                    if (IsPressed(Keys.F5)) //((Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.alt)) && 
                    {
                        RunForge = !RunForge;
                        MyPerson.HoldingObjectDistanceSet = !MyPerson.HoldingObjectDistanceSet;
                        CollisionCheckSystem.IsForgeModeRunning = !CollisionCheckSystem.IsForgeModeRunning;
                        //construct and destroy the console when forge world is created
                        if (CollisionCheckSystem.IsForgeModeRunning == true)
                            ForgeObj.MyConsole = new ConsoleForm();
                        else
                        {
                            ForgeObj.MyConsole.Dispose();
                            //ForgeObj.MyConsole = null;
                            //reset gravity changes in forge world...have fun kids
                            DataValues.gravity = DataValues.DefaultGravity;
                            DataValues.GravityAcceleration = DataValues.DefaultGravityAcceleration;
                            ForgeObj.TemporaryGravity = 0f;
                        }
                    }


                    //won't move the player if forge is in location edit mode or rotation edit mode
                    if (!ForgeObj.EditModeRunning())
                        MyPerson.MovePerson();
                    else
                        MyPerson.MoveCamera();
                    DataValues.UserPosition = MyPerson.Position;
                    //pringles
                    //update the user's location
                    //Door.WhereAmI(MyPerson);
                    
                    //tells what door is actually calculated
                    //Console.WriteLine(MyPerson.CurrentRoom);

                    //Console.WriteLine("Velocity = " + MyPerson.Velocity);
                    modelsLowRes.Clear();
                    modelsHighRes.Clear();
                    //clears all of the models in the game
                    for (int x = 0; x < models.Count; x++)
                    {
                        ScreenModel model = models[x];
                        if (Vector3Distance(model.Position, MyPerson.CameraPosition) - ((model.CollisionType == CollisionCheckSystem.ObjectTypes.Sphere) ? model.GetBoundingSpheres().Radius : DataValues.VectorDistance(model.GetBoundingBoxes().Max - model.Position)) > DataValues.HighResDistance)
                            modelsLowRes.Add(model);
                        else
                            modelsHighRes.Add(model);
                    }
                    foreach (ScreenModel model in modelsLowRes)
                    {
                        try
                        {
                            model.LoadModel(Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + model.ModelName + FolderNames.LowResEXT));
                        }
                        catch { }
                        //test: falling objects
                        //model.IsCurrentlyFalling = true;
                    }
                    foreach (ScreenModel model in modelsHighRes)
                    {
                        //if there is only one copy, such as the room you are in, just load the default file
                        try
                        {
                            model.LoadModel(Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + model.ModelName));
                        }
                        catch { }
                    }

                    //resets the objects the player sees
                    MyPerson.VisibleObjects = models;
                    MyPerson.NearObjects = modelsHighRes;
                }
            }

            LastState = Keyboard.GetState();
            DataValues.LastState = Keyboard.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Draw(GameTime gameTime)
        {
            //counts frames
            framecntr++;
            effects.World = Matrix.Identity;
            effects.View = Matrix.CreateLookAt(MyPerson.CameraPosition, new Vector3(MyPerson.CameraPosition.X + MyPerson.CameraAngle3.X, MyPerson.CameraPosition.Y + MyPerson.CameraAngle3.Y, MyPerson.CameraPosition.Z + MyPerson.CameraAngle3.Z), Vector3.Up);
            effects.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), this.aspectRatio, DataValues.NearPlane, DataValues.FarPlane);
            foreach (EffectPass pass in effects.CurrentTechnique.Passes)
                pass.Apply();

            //deletes the splash screen once the game draws the first time
            if (SplashScreenDeleted == false)
            {
                //deletes screen once done
                MyScreen.SplScreen.Dispose();
                SplashScreenDeleted = true;
                //slight delay, prevents user from going WTF?
                Thread.Sleep(300);
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // stores lights
            List<Vector3> LightList = new List<Vector3>();
            // stores corresponding list of light radii
            List<float> RadList = new List<float>();

            foreach (ScreenModel UpdateModel in models)
            {
                if (UpdateModel.GetType() == typeof(Light))
                {
                    if (DataValues.VectorDistance(UpdateModel.Position - DataValues.UserPosition) - ((Light)UpdateModel).radius * 5 > 400)
                        continue;
                    // adds all light positions from light models
                    LightList.Add(UpdateModel.Position);
                    // adds all the corresponding light radii
                    RadList.Add(((Light)UpdateModel).radius);
                }
            }
            // creates a light around the person in night vision mode to assist in low level lighting areas
            if (NV)
            {
                LightList.Add(MyPerson.Position);
                RadList.Add(10);
            }
            foreach (ScreenModel UpdateModel in modelsLowRes)
            {
                if (UpdateModel.GetType() == typeof(Light))
                {
                    if (CollisionCheckSystem.IsForgeModeRunning)
                    {
                        UpdateModel.MovingBoxes[0].HalfExtent = new Vector3(.25f, .25f, .25f);
                        UpdateModel.Update(effects, LightList,NV);
                    }
                    else
                        UpdateModel.MovingBoxes[0].HalfExtent = new Vector3(0, 0, 0);
                }
                else
                    UpdateModel.Update(effects, LightList,NV);
            }
            foreach (ScreenModel UpdateModel in modelsHighRes)
            {
                if (UpdateModel.GetType() == typeof(Light))
                {
                    if (CollisionCheckSystem.IsForgeModeRunning)
                    {
                        UpdateModel.MovingBoxes[0].HalfExtent = new Vector3(.25f, .25f, .25f);
                        UpdateModel.Update(effects, LightList,NV);
                    }
                    else
                        UpdateModel.MovingBoxes[0].HalfExtent = new Vector3(0, 0, 0);
                }
                else
                    UpdateModel.Update(effects, LightList,NV);
            }

            // converts lists into vectors to be passed into the shader
            Vector3[] LightPositions = LightList.ToArray();
            float[] RadiusSizes = RadList.ToArray();

            // specifies position of lights will eventually move to forge mode

            // sets world, view and projection matrices
            LightingEffect.Parameters["World"].SetValue(effects.World);
            LightingEffect.Parameters["View"].SetValue(effects.View);
            LightingEffect.Parameters["Projection"].SetValue(effects.Projection);
            // sets inverse transpose matrix to calculate diffusive lighting
            LightingEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(effects.World)));
            // passes over the number of lights and the array of light positions
            LightingEffect.Parameters["numlights"].SetValue(LightList.Count);
            LightingEffect.Parameters["LightPositions"].SetValue(LightPositions);
            LightingEffect.Parameters["RadiusSizes"].SetValue(RadiusSizes);
            // passes whether night vision is enabled or not to the shader to determine whether it should add a green tint
            LightingEffect.Parameters["NV"].SetValue(NV);
            //draws all the walls at once (easier for the gpu to handle)
            Wall.DrawAllWalls(LightingEffect, GraphicsDevice);


            //skip this method if the game is paused
            if (MyScreen.GamePaused == false)
            {
                //draws bounding box
                if (IsPressed(Keys.B))
                    foreach (ScreenModel UpdateModel in models)
                        UpdateModel.BoundingSetup();
                if (Keyboard.GetState().IsKeyDown(Keys.B))
                    this.ShowBB();
            }
            //stupid check that must be made to get our console displayed in forge world
            if (ForgeObj.MyConsole.ConsoleUp == false)
            {   //MyPerson.VisibleObjects[0].DrawBoundingBox();CollisionCheckSystem.IsForgeModeRunning
                //Draws reticle in the center of the screen
                //--------------------------------------------------------------
                //drawing of the reticle
                spriteBatch.Begin();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend); 
                //changes the reticle based on the circumstances
                if (MyPerson.ReticleToDraw == DataValues.ReticleType.ObjectPickup)
                {
                    spriteBatch.Draw(this.GameReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.Orange);
                }
                else if (MyPerson.ReticleToDraw == DataValues.ReticleType.ActionBtn)
                {
                    spriteBatch.Draw(this.GameReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.LawnGreen);//Color.Navy);
                }
                else if (MyPerson.ReticleToDraw == DataValues.ReticleType.HoldingObj)
                {
                    spriteBatch.Draw(this.GameReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.DarkOrange);
                }
                //for an inevitable eventuality
                else if (MyPerson.ReticleToDraw == DataValues.ReticleType.PortalBlue)
                {
                    spriteBatch.Draw(this.PBlueReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.White);
                }
                else if (MyPerson.ReticleToDraw == DataValues.ReticleType.PortalOrange)
                {
                    spriteBatch.Draw(this.POrangeReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.White);
                }
                else if (MyPerson.ReticleToDraw == DataValues.ReticleType.PortalBoth)
                {
                    spriteBatch.Draw(this.PBothReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.White);
                }
                else if (MyPerson.ReticleToDraw == DataValues.ReticleType.PortalEmpty)
                {
                    spriteBatch.Draw(this.PEmptyReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.White);
                }
                else
                {
                    spriteBatch.Draw(this.GameReticle, new Vector2((graphics.PreferredBackBufferWidth / 2) + (GameReticle.Width / 2), (graphics.PreferredBackBufferHeight / 2) + (GameReticle.Height / 2)), Color.White);
                }
                //spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts\\Font"), framerate.ToString(), new Vector2(10, 10), Color.Black);

                String DoorLocation = MyPerson.ObjectIsInfront() != null && MyPerson.ObjectIsInfront().GetType() == typeof(Door) ? "This door is to: " + ((Door)MyPerson.ObjectIsInfront()).OutsideDoor : "";
                if(DoorLocation.Contains('#'))
                    DoorLocation = DoorLocation.Substring(0, DoorLocation.IndexOf('#'));
                int HudSpacer = 1;
                if ((DoorLocation.Length == 0)||(RunForge))
                    HudSpacer = 0;
                else
                    DataValues.HudItems[0] = DoorLocation;
                DataValues.HudItems[3] = "";
                DataValues.HudItems[4] = "";
                DataValues.HudItems[0 + HudSpacer] = DataValues.UserPosition.ToString();
                DataValues.HudItems[1 + HudSpacer] = DataValues.DisplayTime();
                DataValues.HudItems[2 + HudSpacer] = CurrentSong.Name + ((CurrentSong.Artist.ToString().Length > 0) ? " - " + CurrentSong.Artist : "");

                //new code for the fps counter and night vision (displayed in one line)
                if (NV)
                    DataValues.HudItems[3 + HudSpacer] = "Night Vision: Enabled  " + framerate.ToString() + " FPS";
                else
                    DataValues.HudItems[3 + HudSpacer] = "Night Vision: Disabled  " + framerate.ToString() + " FPS";

                DataValues.Hud.SelectedItemColor = FontColor;
                DataValues.Hud.FontColor = FontColor;

                if (RunForge)
                    ForgeObj.RunForgeMode(models, Content, graphics, MyPerson,FontColor);
                /*
                spriteBatch.DrawString(DataValues.Font, DataValues.DisplayTime() + '\n' + CurrentSong.Name + ((CurrentSong.Artist.ToString().Length > 0) ? " - " + CurrentSong.Artist : ""), new Vector2(DataTypes.DataValues.ScreenWidth - 300, DataTypes.DataValues.ScreenHeight - 90), Color.Black);
                spriteBatch.DrawString(DataValues.Font, DoorLocation + '\n' + DataValues.UserPosition.ToString(), new Vector2(DataTypes.DataValues.ScreenWidth - 300, DataTypes.DataValues.ScreenHeight - 120), Color.Black);
                 */
                DataValues.Hud.Draw(spriteBatch);
                spriteBatch.End();
                //MyPerson.NormalVector.ToString()
                // resets depth buffering so that stuff is drawn in 3D
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            //--------------------------------------------------------------
            //Thread.Sleep(40);
            base.Draw(gameTime);
        }

        //-------------------------------Main Game Functions----------------------------------
        public void LoadNewModel(String Filename)
        {
            //loads a new file from the files directory            
            Stream stream = File.OpenRead(@"Files\"+Filename + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ModelReader));
            ModelReader ModelSpecs = (ModelReader)serializer.Deserialize(stream); //Content.Load<ModelReader>(FolderNames.Files + Filename);
            models.Clear();

            //creates multiple screen models based on the information in the xml file
            foreach (ModelReader.ModelSpecs Spec in ModelSpecs.Models)
            {
                try
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, GraphicsDevice, Spec.Moveable, Spec.PickUpable, Spec.Bounciness, Spec.Levitating));
                }
                catch (ContentLoadException)
                {
                    try
                    {
                        models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, GraphicsDevice, Spec.Moveable, Spec.PickUpable));
                    }
                    catch (ContentLoadException) { }
                }
                //counted as data loaded
                LoadProgress();
            }
            foreach (ModelReader.ActionSpecs ActionSpec in ModelSpecs.ActionModels)
            {
                ModelReader.ModelSpecs Spec = ActionSpec.Specs;
                try
                {
                    models.Add(DataValues.GetActionModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, GraphicsDevice, Spec.Moveable, Spec.PickUpable, Spec.Bounciness, Spec.Levitating, ActionSpec.Action));
                }
                catch (ContentLoadException)
                {
                    try
                    {
                        models.Add(DataValues.GetActionModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, GraphicsDevice, Spec.Moveable, Spec.PickUpable, Spec.Bounciness, Spec.Levitating, ActionSpec.Action));
                    }
                    catch (ContentLoadException) { }
                }
                //counted as data loaded
                LoadProgress();
            }
        }
        //counts # models in a file
        public int CountModels(String Filename)
        {
            //loads a new file from the files directory            
            Stream stream = File.OpenRead(@"Files\" + Filename + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ModelReader));
            ModelReader ModelSpecs = (ModelReader)serializer.Deserialize(stream); //Content.Load<ModelReader>(FolderNames.Files + Filename);
            models.Clear();
            int Count = 0;
            //creates multiple screen models based on the information in the xml file
            foreach (ModelReader.ModelSpecs Spec in ModelSpecs.Models)
            {
                Count++;
            }
            foreach (ModelReader.ActionSpecs ActionSpec in ModelSpecs.ActionModels)
            {
                Count++;
            }
            return(Count);
        }
        public double Vector3Distance(Vector3 point1, Vector3 point2)
        {
            return Math.Pow(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.Z - point2.Z, 2), .5);
        }
        public Boolean IsPressed(Keys TestKey)
        {
            return ((Keyboard.GetState().IsKeyDown(TestKey)) && (!LastState.IsKeyDown(TestKey)));
        }
        public void ShowBB()
        {
            //buffers = new BoundingBoxBuffers(MovingBox, device);

            int totalBuffers = 0;
            foreach (List<OrientedBoundingBox> MovingBoxes in BoundingBoxModels)
                totalBuffers += MovingBoxes.Count;

            ScreenModel.BoundingBoxBuffers[] buffers = new ScreenModel.BoundingBoxBuffers[totalBuffers];

            int bufferCntr=0;
            for(int cntr=0; cntr<BoundingBoxModels.Count; cntr++)
                for (int cntr2 = 0; cntr2 < BoundingBoxModels[cntr].Count; cntr2++)
                    buffers[bufferCntr++] = new ScreenModel.BoundingBoxBuffers(BoundingBoxModels[cntr][cntr2], GraphicsDevice);

            List<VertexPositionColor> verticesList = new List<VertexPositionColor>();
            for (int x = 0; x < buffers.Length; x++)
                    for (int y = 0; y < buffers[x].verticesList.Count; y++)
                        verticesList.Add(buffers[x].verticesList[y]);

            VertexBuffer Vertices = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), verticesList.Count, BufferUsage.WriteOnly);
            Vertices.SetData(verticesList.ToArray());

            IndexBuffer Indices = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, verticesList.Count, BufferUsage.WriteOnly);
            Indices.SetData(Enumerable.Range(0, verticesList.Count).Select(i => (short)i).ToArray());
            GraphicsDevice.SetVertexBuffer(Vertices);
            GraphicsDevice.Indices = Indices;

            /*BasicEffect lineEffect = new BasicEffect(GraphicsDevice);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;*/
            effects.World = Matrix.Identity;
            effects.View = Matrix.CreateLookAt(MyPerson.CameraPosition, new Vector3(MyPerson.CameraPosition.X + MyPerson.CameraAngle3.X, MyPerson.CameraPosition.Y + MyPerson.CameraAngle3.Y, MyPerson.CameraPosition.Z + MyPerson.CameraAngle3.Z), Vector3.Up);
            effects.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), this.aspectRatio, DataValues.NearPlane, DataValues.FarPlane);

            foreach (EffectPass pass in effects.CurrentTechnique.Passes)
            {
                pass.Apply();

                /*for (int x = 0; x < buffers.Length; x++)
                {

                    GraphicsDevice.SetVertexBuffer(buffers[x].Vertices);
                    GraphicsDevice.Indices = buffers[x].Indices;*/
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, verticesList.Count, 0, verticesList.Count / 2);
                //}
            }
        }
        //----------------------------End Main Game Functions-----------------------------------
    }
}
