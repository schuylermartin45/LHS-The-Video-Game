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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using DataTypes.Special_Objects;

namespace DataTypes
{
    //This is a list of all default values that will be used throughout the entire program
    public abstract class DataValues
    {
        //**********************************************************************
        //All constant values
        
        //default screen height and width
        public static float ScreenHeight = 300;
        public static float ScreenWidth = 300;
        //for vision
        //public static float aspectratio = graphics.GraphicsDevice.Viewport.AspectRatio;
        public static float NearPlane = 0.25f;
        public static float FarPlane = 1000f;
        public static Vector3 UserPosition = Vector3.Zero;

        //Physics values such as gravity
        //------------------------------------------------------------------------------------------------------
        //the maximum angle of a ramp that an object may climb
        public static float MaxAngle = MathHelper.PiOver2*0.75f;
        //represents how often the program will check for collisions 
        public static int MovePrecision = 50;
        //represents the movement precision of all objects around the player
        public static int SurroundingsPrecision = 10;
        //for controlling movement speed
        public static float PitchSensitivity = 4f;
        //move sensativity is divided by MovePrecision, as movement will loop 100 times(for the sake of precision)
        public static float MoveSensitivity = 0.2f / MovePrecision;
        //the rate at which the user is pulled down(currently the same rate as movement)
        public static float gravity = -0.2f;//make .5? 
        public static float PGravity = gravity / MovePrecision;
        public static float SGravity = gravity / SurroundingsPrecision;
        //permanently stores gravity...DO NOT ALTER
        public static float DefaultGravity = -0.2f / MovePrecision;
        //acceleration due to gravity
        public static float GravityAcceleration = 1.15f;
        public static float DefaultGravityAcceleration = 1.1f;
        //Terminal Velocity due to air resistance
        public static float TerminalVelocity = -5f / MovePrecision;
        public static float ThrowForce = .3f;

        //Represents the rate at which objects will slow down when in a free fall. Should be a number VERY close to 1
        public static float AirResistance = .91f;
        //jump vars
        //how much the jump should move up per cycle
        public static float JumpIncrement = .03f;
        public static KeyboardState LastState;
        //constant used when checking the displacement of an object; checking every clock cycle is to imprecise with a float
        public static int CheckDisplacement = 50;
        public static Random Rnd = new Random();
        public static SpriteFont Font;
        public static List<String> HudItems = new List<String>(0);
        public static ScreenMenu Hud;
        //------------------------------------------------------------------------------------------------------

        //values that are/were originally for the camera collision class (may have to be used elsewhere later; that's why they are here)
        //------------------------------------------------------------------------------------------------------
        //maximum distance one can pick up an object
        public const float MaxObjectDistance = 7.0f;

        //distance to draw the high res versions of a model
        public const float HighResDistance = 10.0f;

        //move speed reduction for being crouched
        public static float CrouchedReduction = 1f;//0.3f;

        //scalar to increase speed while sprinting
        public static float SprintFactor = 3.75f;
        //time in which one can sprint (in miliseconds)
        public static float SprintTime = 300f;
        
        //Value that slows down the pushing of objects due to friction
        public static float CoefficientFriction = 0.3f;
            
        //how far away the camera is from the center of the model...to correct problems later on; DO NOT CHANGE VALUE IN CODE
        public static Vector3 CameraModelOffSet = new Vector3(0f, -2f, 0f);//-0.5 for Y when height problem is fixed
        //corrects distance from the ground due to the gravity; the head of our person is Y units above the ground
        public static Vector3 CameraGroundOffSet = new Vector3(0f, 0f, 0f);
        public static float CrouchFactor = 2f;
        //To CLARIFY: the user is 1f tall (distance from camera to the ground); 
        //that makes the center of the character set at .5f in the Y direction from the ground or the camera
        //when the character is crouched; these values are multiplied by a half of what they usually are
        //------------------------------------------------------------------------------------------------------

        //**********************************************************************
        //All enumerated types

        //used to determine what kind of reticle to be displayed
        public enum ReticleType { Normal = 1, ObjectPickup, HoldingObj, PortalBlue, PortalOrange, PortalBoth, PortalEmpty, ActionBtn };


        //**********************************************************************
        //All Methods that will be used in multiple places but do not require an object...I.E. Static methods

        public static void LoadNewModel(String Filename, ContentManager Content, List<ScreenModel> models, GraphicsDevice graphics)
        {
            //loads a new file from the files directory      
            if (!File.Exists(Filename + ".xml"))
                Filename = "GenericTemplate";
            Stream stream = File.OpenRead(Filename + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ModelReader));
            ModelReader ModelSpecs = (ModelReader)serializer.Deserialize(stream); //Content.Load<ModelReader>(FolderNames.Files + Filename);
            models.Clear();

            //creates multiple screen models based on the information in the xml file
            foreach (ModelReader.ModelSpecs Spec in ModelSpecs.Models)
            {
                try
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable));
                }
                catch (ContentLoadException)
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable));
                }
                //counted as data loaded
            }
            foreach (ModelReader.ActionSpecs ActionSpec in ModelSpecs.ActionModels)
            {
                ModelReader.ModelSpecs Spec = ActionSpec.Specs;
                try
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable));
                }
                catch (ContentLoadException)
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable));
                }
                //counted as data loaded
            }
            //creates multiple screen models based on the information in the xml file
            foreach (ModelReader.ModelSpecs Spec in ModelSpecs.Models)
            {
                try
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable));
                }
                catch (ContentLoadException)
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable));
                }
                //calls animation for opening the door; this will be opening the door
                //this.Animation();
            }
        }
        public static List<ScreenModel> LoadListOfModel(String Filename, ContentManager Content, GraphicsDeviceManager graphics)
        {
            //loads a new file from the files directory
            if (Filename.Contains('#'))
                Filename = Filename.Substring(0,Filename.IndexOf("#"));
            
            //creates a serializable version of the file, and stores it in modelspecs
            if (!File.Exists("Files\\" + Filename + ".xml"))
                Filename = "GenericTemplate";
            Stream stream = File.OpenRead("Files\\"+Filename + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ModelReader));
            ModelReader ModelSpecs = (ModelReader)serializer.Deserialize(stream); //Content.Load<ModelReader>(FolderNames.Files + Filename);
            List<ScreenModel> models = new List<ScreenModel>(0);

            //creates multiple screen models based on the information in the xml file
            foreach (ModelReader.ModelSpecs Spec in ModelSpecs.Models)
            {
                try
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics.GraphicsDevice, Spec.Moveable, Spec.PickUpable));
                }
                catch (ContentLoadException)
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics.GraphicsDevice, Spec.Moveable, Spec.PickUpable));
                }
                //counted as data loaded
            }
            foreach (ModelReader.ActionSpecs ActionSpec in ModelSpecs.ActionModels)
            {
                ModelReader.ModelSpecs Spec = ActionSpec.Specs;
                try
                {
                    models.Add(DataValues.GetActionModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics.GraphicsDevice, Spec.Moveable, Spec.PickUpable, Spec.Bounciness, Spec.Levitating, ActionSpec.Action));
                }
                catch (ContentLoadException)
                {
                    //models.Add(DataValues.GetActionModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics, Spec.Moveable, Spec.PickUpable, Spec.Bounciness, Spec.Levitating, ActionSpec.Action));
                }
                //counted as data loaded
            }
            return (models);
        }
        //Clear method, clears a list of models
        public static void ClearModelList(List<ScreenModel> models)
        {
            models.Clear();
        }
        //clears all models except for the door object being closed
        public static void ClearModelList(List<ScreenModel> models, Door DoorObject)
        {
            Door TempDoor = (Door)models.Find(
                delegate(ScreenModel DelegateObj)
                {
                    return (DelegateObj == DoorObject);
                }
                );
            models.Clear();
            //add that door back to the model list
            models.Add(TempDoor);
        }
        public static String DisplayTime()
        {
            //simple way to display the time
            String Header = "Current Time: ";
            String AMPM = "AM";
            //corrects for 12 hour time
            int TempHour = DateTime.Now.TimeOfDay.Hours;
            if (DateTime.Now.TimeOfDay.TotalHours > 12)
                AMPM = "PM";
            if (DateTime.Now.TimeOfDay.Hours > 12)
                TempHour -= 12;
            //corrects for lack of 0 in time
            String TempMin = DateTime.Now.TimeOfDay.Minutes.ToString();
            if (DateTime.Now.TimeOfDay.Minutes < 10)
            {
                TempMin = "0" + DateTime.Now.TimeOfDay.Minutes.ToString();
            }
            String TempSec = DateTime.Now.TimeOfDay.Seconds.ToString();
            if (DateTime.Now.TimeOfDay.Seconds < 10)
            {
                TempSec = "0" + DateTime.Now.TimeOfDay.Seconds.ToString();
            }
            //finally returns time
            String Time = "" + TempHour.ToString() + ":" + TempMin + ":" + TempSec + " " + AMPM;
            return (Header + Time);
        }
        //determines the distance of a 3 dimensional point to the point (0,0,0)
        public static float VectorDistance(Vector3 V)
        {
            return ((float)Math.Sqrt(V.X*V.X + V.Y*V.Y + V.Z*V.Z));
        }
        public static void UpdateActions(ScreenModel ThisModel)
        {
            if (ThisModel.GetType() == typeof(Wall))
                ((Wall)ThisModel).UpdateActionString();
            if (ThisModel.GetType() == typeof(Door))
                ((Door)ThisModel).UpdateActionString();
            if (ThisModel.GetType() == typeof(Light))
                ((Light)ThisModel).UpdateActionString();
        }

        //methods for determining special object sub classes...that don't require extra information
        public static ScreenModel GetActionModel(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float Bounciness, Boolean Levitating, String ActionInfo)
        {
            if (ActionInfo.Contains("Wall{"))
                return (new Wall(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, moveable, pickup, Bounciness, Levitating, ActionInfo));
            else if ((Name.Equals("Computer")) || (ActionInfo.Equals("Computer")) || (Name.Equals("Monitor")) || (ActionInfo.Equals("Monitor")))
            {
                return (new ComputerConsole(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, moveable, pickup));
            }
            else if ((Name.Equals("PortalGun")) || (ActionInfo.Equals("PortalGun")))
            {
                return (new PortalGun(Name, ModelName, SentModel, ModelPosition, ModelRotation, device));
            }
            else if ((Name.Equals("Door")) || (ActionInfo.Contains("Door{")))
            {
                return(new Door(Name,ModelName,SentModel,ModelPosition,ModelRotation,device,ActionInfo));
            }
            else if ((Name.Equals("Light")) || (ActionInfo.Contains("Light")))
            {
                return (new Light(ModelName, SentModel, ModelPosition, ModelRotation, device, moveable, pickup, Bounciness, Levitating,ActionInfo));
            }
            else
                return (new ScreenModel(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, moveable, pickup, Bounciness, Levitating));
        }

        public static Boolean IsPressed(Keys TestKey)
        {
            return ((Keyboard.GetState().IsKeyDown(TestKey)) && (!LastState.IsKeyDown(TestKey)));
        }
    }
}
