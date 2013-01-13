using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Graphics.GraphicsDevice;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.Xml.Serialization;
using DataTypes;

namespace Senior_Project
{
    public class ForgeClass
    {
        //console we use to edit objects in forge world
        public ConsoleForm MyConsole;
        //only constructs this object...really bad code but I'm overriding Nick's code
        // forms are not working well
        public ForgeClass()
        {
            MyConsole = new ConsoleForm();
        }
        //This is the list of models that appear on the menu to be used
        public  List<String> ModelList = new List<String>();

        public  Boolean IsRunning;

        //Determines if the menu is currently being shown
        public  Boolean ShowMenu = true;//false;

        //Determines if the list of directions should be shown
        public  Boolean ShowDirections = false;

        //This string represents the long set of directions
        private  String Directions;

        //This string represents a shorter set of directions to be shown when the directions set is closed
        public  String ClosedDirections;

        //The menu of objects
        public  ScreenMenu Menu;

        //the menu controlling each individual object
        public  ScreenMenu ModelMenu;
        public  SpriteBatch spriteBatch;

        //represents the last keyboard state
        private  KeyboardState LastState;
        //to read the mouse input
        private  MouseState LastMouseState;

        public  Boolean PropertyMenu=false;

        //determines if the forge is currently in rotation edit/location edit mode
        public  Boolean RotationEditMode = false;
        public  Boolean LocationEditMode = false;
        public  Boolean WallEditMode = false;
        public  Boolean StampModel = false;
        public Boolean ColorEditMode = false;
        public  String Title = "";
        private  FileInfo[] files;

        public float TemporaryGravity = 0f;
        public float TemporaryGravityAcceleration = 1f;

        //initializes forge mode with some basic settings
        public  void Initialize(SpriteBatch SentBatch, Texture2D TransparentPng, ContentManager Content, Color FontColor, Color SelectColor, Color BackColor)
        {
            //our in-game console system
            //declared out in main program
            //MyConsole = new ConsoleForm();

            //Sets the title
            Title = "Forge Mode:";

            //initializes the directions(AKA the obnoxious string of directions)
            Directions = "Controls:\nOpen Menu: Ctrl+M\nNavigate Menu: Ctrl+Up/Ctrl+Down\nCreate Item: Enter\nDrop Item: E\nDelete Item: Select, then press Delete/Backspace\nHide/Show Help Bar: Ctrl+H";

            //shows the user how to access the large list of directions
            ClosedDirections = "Press Ctrl+H to show the help menu";

            //Determines if the long list of directions is being shown
            ShowDirections = false;

            //gets the sprite batch
            spriteBatch = SentBatch;

            //clears the menu
            ModelList.Clear();

            //creates the menu controlling the model's properties
            ModelMenu = new ScreenMenu(ModelList, new Vector2(0, DataValues.ScreenHeight - 70), DataValues.Font, 3, 0, FontColor, SelectColor, BackColor, 12, TransparentPng);
            
            //fills the list of models with default options
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\Models\\HighRes");
            files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                String Filename = Path.GetFileNameWithoutExtension(file.Name);
                for (int x = 1; x < Filename.Length; x++)
                    if ((Filename[x] == Filename.ToUpper()[x]) && (Filename[x] != ' '))
                        Filename.Insert(x, " ");
                ModelList.Add(Filename);
            }

            //gets the current keyboard state
            LastState = Keyboard.GetState();

            //initializes the menu
            Menu = new ScreenMenu(ModelList, new Vector2(0, DataValues.ScreenHeight - 70), DataValues.Font, 3, 0, FontColor, SelectColor, BackColor, 12, TransparentPng);
        }

        //gets the model filename
        public  String GetModelFileName(String ModelName)
        {
            //returns the file name without the spaces
            for (int x = 0; x < ModelName.Length; x++)
                if (ModelName[x] == ' ')
                    ModelName.Remove(x, 1);

            return ModelName;
        }

        //a simple function getting the key's last state
        public  Boolean IsPressed(Keys TestKey)
        {
            return ((Keyboard.GetState().IsKeyDown(TestKey)) && (!LastState.IsKeyDown(TestKey)));
        }

        //a simple function to determine if either of the control buttons are being pressed
        public  Boolean IsControlPressed()
        {
            return (Keyboard.GetState().IsKeyDown(Keys.LeftControl)) || (Keyboard.GetState().IsKeyDown(Keys.RightControl));
        }
        public  void LoadNewModel(String Filename, ContentManager Content, List<ScreenModel> models,GraphicsDeviceManager graphics)
        {
            //loads a new file from the files directory            
            Stream stream = File.OpenRead(Filename + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ModelReader));
            ModelReader ModelSpecs = (ModelReader)serializer.Deserialize(stream); //Content.Load<ModelReader>(FolderNames.Files + Filename);
            models.Clear();

            //creates multiple screen models based on the information in the xml file
            foreach (ModelReader.ModelSpecs Spec in ModelSpecs.Models)
            {
                try
                {
                    models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Spec.ModelName), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics.GraphicsDevice, Spec.Moveable, Spec.PickUpable));
                }
                catch (ContentLoadException)
                {
                    //worst case scenariio catch
                    try
                    {
                        models.Add(new ScreenModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics.GraphicsDevice, Spec.Moveable, Spec.PickUpable));
                    }
                    catch (ContentLoadException) { }
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
                    try
                    {
                        models.Add(DataValues.GetActionModel(Spec.Name, Spec.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + Spec.ModelName + FolderNames.LowResEXT), Spec.Position + ModelSpecs.RoomPosition, Spec.Rotation, graphics.GraphicsDevice, Spec.Moveable, Spec.PickUpable, Spec.Bounciness, Spec.Levitating, ActionSpec.Action));
                    }
                    catch (ContentLoadException) { }
                }
                //counted as data loaded
            }
        }
        private  void ShowMenus(CollisionCheckSystem Person,Color FontColorArg)
        {
            if ((Person.HoldingObjectTrue)||(PropertyMenu))
            {
                ModelMenu.MenuItems.Clear();
                ModelMenu.MenuItems.Add("Name: " + Person.HoldingObject.Name);
                ModelMenu.MenuItems.Add("Location: (" + Person.HoldingObject.Position.X + ", " + Person.HoldingObject.Position.Y + ", " + Person.HoldingObject.Position.Z + ')');
                ModelMenu.MenuItems.Add("Rotation: (" + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.X) + ", " + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Y) + ", " + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Z) + ')');
                ModelMenu.MenuItems.Add("Moveable: " + Person.HoldingObject.Moveable);
                ModelMenu.MenuItems.Add("Pickupable: " + Person.HoldingObject.PickUpable);
                ModelMenu.MenuItems.Add("Levitating: " + Person.HoldingObject.IsLevitating);
                ModelMenu.MenuItems.Add("Bounciness: " + Person.HoldingObject.Bounce);
                ModelMenu.MenuItems.Add("Stamp");
                if (Person.HoldingObject.GetType() == typeof(Wall))
                {
                    ModelMenu.MenuItems.Add("Dimensions: (" + ((Wall)Person.HoldingObject).GetLength() + ", " + ((Wall)Person.HoldingObject).GetHeight() + ", " + ((Wall)Person.HoldingObject).GetWidth() + ')');
                    ModelMenu.MenuItems.Add("Color: " + ((Wall)Person.HoldingObject).GetColor());
                }
                if (Person.HoldingObject.GetType() == typeof(Door))
                {
                    ModelMenu.MenuItems.Add("Door Sender: " + ((Door)Person.HoldingObject).OutsideDoor);
                    ModelMenu.MenuItems.Add("Door Reciever: " + ((Door)Person.HoldingObject).InsideDoor);
                }
                if (Person.HoldingObject.GetType() == typeof(Light))
                {
                    ModelMenu.MenuItems.Add("Light Radius: " + ((Light)Person.HoldingObject).radius);
                }
                ModelMenu.Draw(spriteBatch);
            }
            else
            {
                //adds all the models in the models directory to the menu list
                Menu.MenuItems.Clear();
                foreach (FileInfo file in files)
                {
                    String Filename = Path.GetFileNameWithoutExtension(file.Name);
                    for (int x = 1; x < Filename.Length; x++)
                        if ((Filename[x] == Filename.ToUpper()[x]) && (Filename[x] != ' '))
                            Filename.Insert(x, " ");
                    Menu.MenuItems.Add(Filename);
                }
                Menu.Draw(spriteBatch);
            }
        }
        private  void GetObjectMenuOption(CollisionCheckSystem Person,GraphicsDeviceManager graphics)
        {
            switch (ModelMenu.GetSelectedItemSpot())
            {
                case 0:
                    RotationEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    MyConsole.ConsoleOutLine("The model's current name: " + Person.HoldingObject.Name);
                    MyConsole.ConsoleInputPrompt("Enter the model's name: ", graphics);
                    //prevents deleting the name
                    if ((String)MyConsole.ForgeModeOut != "")
                        Person.HoldingObject.Name = (String)MyConsole.ForgeModeOut;
                    break;
                case 1:
                    WallEditMode = false;
                    RotationEditMode = false;
                    ColorEditMode = false;
                    LocationEditMode = !LocationEditMode;
                    break;
                case 2:
                    WallEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    RotationEditMode = !RotationEditMode;
                    break;
                case 3:
                    WallEditMode = false;
                    RotationEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    Person.HoldingObject.Moveable = !Person.HoldingObject.Moveable;
                    break;
                case 4:
                    WallEditMode = false;
                    RotationEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    Person.HoldingObject.PickUpable = !Person.HoldingObject.PickUpable;
                    break;
                case 5:
                    WallEditMode = false;
                    RotationEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    Person.HoldingObject.IsLevitating = !Person.HoldingObject.IsLevitating;
                    break;
                case 6:
                    WallEditMode = false;
                    RotationEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    MyConsole.ConsoleOutLine("Current bounciness: "+Person.HoldingObject.Bounce);
                    MyConsole.ConsoleInputPrompt("Enter the model's bounciness(between 0 and 1): ", graphics);
                    Person.HoldingObject.Bounce = (float)MyConsole.ForgeModeOut;
                    break;
                case 7:
                    StampModel = true;
                    break;
                case 8:
                    RotationEditMode = false;
                    LocationEditMode = false;
                    ColorEditMode = false;
                    if (Person.HoldingObject.GetType() == typeof(Light))
                    {
                        MyConsole.ConsoleOutLine("Current light radius: "+((Light)Person.HoldingObject).radius);
                        MyConsole.ConsoleInputPrompt("Enter the light radius: ", graphics);
                        try
                        {
                            ((Light)Person.HoldingObject).radius = (float)MyConsole.ForgeModeOut;
                        }
                        catch { }
                    }
                    if (Person.HoldingObject.GetType() == typeof(Wall))
                        WallEditMode = !WallEditMode;
                    if (Person.HoldingObject.GetType() == typeof(Door))
                    {
                        MyConsole.ConsoleOutLine("Current room this door will load: "+((Door)Person.HoldingObject).OutsideDoor);
                        MyConsole.ConsoleInputPrompt("Enter the name of the room this door will load: ", graphics);
                        try
                        {
                            ((Door)Person.HoldingObject).OutsideDoor = MyConsole.ForgeModeOut.ToString();
                        }
                        catch { }
                    }
                    break;
                case 9:
                    WallEditMode = false;
                    RotationEditMode = false;
                    LocationEditMode = false;
                    if (Person.HoldingObject.GetType() == typeof(Wall))
                        ColorEditMode = !ColorEditMode;
                    if (Person.HoldingObject.GetType() == typeof(Door))
                    {
                        MyConsole.ConsoleOutLine("Current room this door is in: "+((Door)Person.HoldingObject).InsideDoor);
                        MyConsole.ConsoleInputPrompt("Enter the name of the room this door is in: ", graphics);
                        try
                        {
                            ((Door)Person.HoldingObject).InsideDoor = MyConsole.ForgeModeOut.ToString();
                        }
                        catch { }
                    }
                    break;
            }
        }
        public  Boolean EditModeRunning()
        {
            return (LocationEditMode || RotationEditMode || WallEditMode || ColorEditMode);
        }
        public  Boolean RunForgeMode(List<ScreenModel> models, ContentManager Content,GraphicsDeviceManager graphics, CollisionCheckSystem Person,Color FontColorArg)
        {
            Menu.Location.Y = DataValues.ScreenHeight - 70;

            //includes reversing gravity, fly-mode, and more!
            this.PhysicsMods(Person);

            //shows/hides the menu
            if ((IsControlPressed()) && (IsPressed(Keys.M)))
                ShowMenu = !ShowMenu;

            if ((IsControlPressed()) && (IsPressed(Keys.L)))
            {
                MyConsole.ConsoleOutLine("Your Current Coordinates: (" + Person.CameraPosition.X + ' ' + Person.CameraPosition.Y + ' ' + Person.CameraPosition.Z + "). Press enter to keep the original value.");
                MyConsole.ConsoleInputPrompt("Change the X coordinate: ", graphics);
                try
                {
                    Person.CameraPosition.X = (float)MyConsole.ForgeModeOut;
                }
                catch (Exception)
                {
                }
                MyConsole.ConsoleOutLine("Your Current Coordinates: (" + Person.CameraPosition.X + ' ' + Person.CameraPosition.Y + ' ' + Person.CameraPosition.Z + "). Press enter to keep the original value.");
                MyConsole.ConsoleInputPrompt("Change the Y coordinate: ", graphics);
                try
                {
                    Person.CameraPosition.Y = (float)MyConsole.ForgeModeOut;
                }
                catch (Exception)
                {
                }
                MyConsole.ConsoleOutLine("Your Current Coordinates: (" + Person.CameraPosition.X + ' ' + Person.CameraPosition.Y + ' ' + Person.CameraPosition.Z + "). Press enter to keep the original value.");
                MyConsole.ConsoleInputPrompt("Change the Z coordinate: ", graphics);
                try
                {
                    Person.CameraPosition.Z = (float)MyConsole.ForgeModeOut;
                }
                catch (Exception)
                {
                }
            }

            if (IsPressed(Keys.P))
            {
                ScreenModel ThisModel = Person.ObjectIsInfront();
                if (ThisModel != null)
                {
                    Person.HoldingObject = ThisModel;
                    PropertyMenu = true;
                }
                else
                {
                    PropertyMenu = false;
                    //reset all edit modes to false
                    RotationEditMode = false;
                    LocationEditMode = false;
                    WallEditMode = false;
                    StampModel = false;
                    ColorEditMode = false;
                }

            }

            //shows/hides the directions
            if ((IsControlPressed()) && (IsPressed(Keys.H)))
                ShowDirections = !ShowDirections;
            //************************************
            //navigates the menu
            //Mouse scrolling value
            int Difference = MouseScroll();
            if ((Difference > 0) || (IsPressed(Keys.Up)))
                if ((Person.HoldingObjectTrue)||(PropertyMenu))
                    ModelMenu.ChangeSelectedItem(ModelMenu.GetSelectedItemSpot() - 1);
                else
                    Menu.ChangeSelectedItem(Menu.GetSelectedItemSpot() - 1);
            if ((Difference < 0) || (IsPressed(Keys.Down)))
                if ((Person.HoldingObjectTrue)||(PropertyMenu))
                    ModelMenu.ChangeSelectedItem(ModelMenu.GetSelectedItemSpot() + 1);
                else
                    Menu.ChangeSelectedItem(Menu.GetSelectedItemSpot() + 1);

            //draws the menu
            if (ShowMenu)
                ShowMenus(Person,FontColorArg);

            //if the menu is being shown, and the enter button is pressed, a new object will be created
            if ((ShowMenu) && (IsPressed(Keys.Enter)))
            {
                if ((Person.HoldingObjectTrue)||(PropertyMenu))
                    GetObjectMenuOption(Person, graphics);
                else
                {
                    if (GetModelFileName(Menu.GetSelectedItem()).CompareTo("wall") == 0)
                        models.Add(new Wall(Menu.GetSelectedItem(), GetModelFileName(Menu.GetSelectedItem()), Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + GetModelFileName(Menu.GetSelectedItem())), Person.Position + Person.CameraAngle3 * 4f, Vector3.Zero, graphics.GraphicsDevice, false, false, 0, true, 1f, 1f, 1f));
                    else
                    {
                        //finally loads in the right constructor
                        String[] CheckNames = {"Light", "Computer", "Monitor", "PortalGun", "Door" };
                        bool ConstructorFound = false;
                        for (int cntr = 0; cntr < CheckNames.Length; cntr++)
                        {
                            if (GetModelFileName(Menu.GetSelectedItem()).Contains(CheckNames[cntr]))
                            {
                                //in the off chance the file screws up...
                                try
                                {
                                    models.Add(DataValues.GetActionModel(CheckNames[cntr], GetModelFileName(Menu.GetSelectedItem()), Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + GetModelFileName(Menu.GetSelectedItem())), Person.Position + Person.CameraAngle3 * 4f, Vector3.Zero, graphics.GraphicsDevice, true, true, 0f, false, GetModelFileName(Menu.GetSelectedItem())));
                                    ConstructorFound = true;
                                    break;
                                }
                                catch (ContentLoadException)
                                {
                                    try
                                    {
                                        models.Add(DataValues.GetActionModel(CheckNames[cntr], GetModelFileName(Menu.GetSelectedItem()), Content.Load<Model>(FolderNames.Models + FolderNames.LowRes + GetModelFileName(Menu.GetSelectedItem()) + FolderNames.LowResEXT), Person.Position + Person.CameraAngle3 * 4f, Vector3.Zero, graphics.GraphicsDevice, true, true, 0f, false, GetModelFileName(Menu.GetSelectedItem())));
                                        ConstructorFound = true;
                                        break;
                                    }
                                    catch (ContentLoadException) { }
                                }
                            }
                        }
                        //loads a normal screen object
                        if (ConstructorFound == false)
                            models.Add(new ScreenModel(Menu.GetSelectedItem(), GetModelFileName(Menu.GetSelectedItem()), Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + GetModelFileName(Menu.GetSelectedItem())), Person.Position + Person.CameraAngle3 * 4f, Vector3.Zero, graphics.GraphicsDevice, true, true, 0, false));
                    }
                    //has the user holding the object
                    Person.HoldingObject = models[models.Count - 1];
                    Person.PickUpObject();
                }
            }

            if (StampModel)
            {
                ScreenModel NewHoldingObject;
                DataValues.UpdateActions(Person.HoldingObject);
                NewHoldingObject = DataValues.GetActionModel(Person.HoldingObject.Name, Person.HoldingObject.ModelName, Content.Load<Model>(FolderNames.Models + FolderNames.HighRes + Person.HoldingObject.ModelName), Person.HoldingObject.Position, Person.HoldingObject.modelRotation, graphics.GraphicsDevice, Person.HoldingObject.Moveable, Person.HoldingObject.PickUpable, Person.HoldingObject.Bounce, Person.HoldingObject.IsLevitating, Person.HoldingObject.ActionString);
                models.Add(NewHoldingObject);
                Person.HoldingObject = NewHoldingObject;
                StampModel = false;
            }

            //picks up and drops objects
            if (IsControlPressed() && IsPressed(Keys.X))
                if (Person.HoldingObjectTrue)
                    Person.HoldingObjectTrue = !Person.HoldingObjectTrue;
                else
                    Person.DeterminePickUp();

            //deletes a selected object
            if ((Person.HoldingObjectTrue) && (IsPressed(Keys.Delete) || IsPressed(Keys.Back)))
                for (int counter = 0; counter < models.Count; counter++)
                    if (models[counter] == Person.HoldingObject)
                    {
                        Console.Beep();
                        models.RemoveAt(counter);
                        Person.HoldingObjectTrue = !Person.HoldingObjectTrue;
                        //kick out all of applicable edit modes
                        RotationEditMode = false;
                        LocationEditMode = false;
                        WallEditMode = false;
                        StampModel = false;
                    }
            if ((Person.HoldingObjectTrue == false) && ((IsControlPressed()) && IsPressed(Keys.Delete)))
                models.Clear();
            //allows the user to rotate an object
            if (RotationEditMode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Person.HoldingObject.modelRotation.X += 0.01f;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Person.HoldingObject.modelRotation.X -= 0.01f;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Person.HoldingObject.modelRotation.Y += 0.01f;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Person.HoldingObject.modelRotation.Y -= 0.01f;
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    Person.HoldingObject.modelRotation.Z += 0.01f;
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    Person.HoldingObject.modelRotation.Z -= 0.01f;
                if (IsPressed(Keys.T))
                {
                    MyConsole.ConsoleOutLine("Current Rotation: (" + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.X) + ' ' + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Y) + ' ' + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Z) + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the X rotation: ", graphics);
                    try
                    {
                        Person.HoldingObject.modelRotation.X = (float)MathHelper.ToRadians((float)MyConsole.ForgeModeOut);
                    }
                    catch (Exception)
                    {
                    }
                    MyConsole.ConsoleOutLine("Current Rotation: (" + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.X) + ' ' + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Y) + ' ' + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Z) + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the Y rotation: ", graphics);
                    try
                    {
                        Person.HoldingObject.modelRotation.Y = (float)MathHelper.ToRadians((float)MyConsole.ForgeModeOut);
                    }
                    catch (Exception)
                    {
                    }
                    MyConsole.ConsoleOutLine("Current Rotation: (" + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.X) + ' ' + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Y) + ' ' + MathHelper.ToDegrees(Person.HoldingObject.modelRotation.Z) + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the Z rotation: ", graphics);
                    try
                    {
                        Person.HoldingObject.modelRotation.Z = (float)MathHelper.ToRadians((float)MyConsole.ForgeModeOut);
                    }
                    catch (Exception)
                    {
                    }
                }
            }


            //allows the user to edit the location of an object
            if (LocationEditMode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Person.HoldingObject.Position.X += 0.02f;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Person.HoldingObject.Position.X -= 0.02f;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Person.HoldingObject.Position.Z += 0.02f;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Person.HoldingObject.Position.Z -= 0.02f;
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    Person.HoldingObject.Position.Y += 0.02f;
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    Person.HoldingObject.Position.Y -= 0.02f;
                if (IsPressed(Keys.T))
                {
                    MyConsole.ConsoleOutLine("Current Coordinates: (" + Person.HoldingObject.Position.X + ' ' + Person.HoldingObject.Position.Y + ' ' + Person.HoldingObject.Position.Z + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the X coordinate: ", graphics);
                    try
                    {
                        Person.HoldingObject.Position.X = (float)MyConsole.ForgeModeOut;
                    }
                    catch (Exception)
                    {
                    }
                    MyConsole.ConsoleOutLine("Current Coordinates: (" + Person.HoldingObject.Position.X + ' ' + Person.HoldingObject.Position.Y + ' ' + Person.HoldingObject.Position.Z + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the Y coordinate: ", graphics);
                    try
                    {
                        Person.HoldingObject.Position.Y = (float)MyConsole.ForgeModeOut;
                    }
                    catch (Exception)
                    {
                    }
                    MyConsole.ConsoleOutLine("Current Coordinates: (" + Person.HoldingObject.Position.X + ' ' + Person.HoldingObject.Position.Y + ' ' + Person.HoldingObject.Position.Z + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the Z coordinate: ", graphics);
                    try
                    {
                        Person.HoldingObject.Position.Z = (float)MyConsole.ForgeModeOut;
                    }
                    catch (Exception)
                    {
                    }
                    Person.HoldingObjectTrue = false;
                    PropertyMenu = true;
                }
                if(Person.HoldingObjectTrue)
                    Person.PickUpObject();
            }
            if (WallEditMode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    ((Wall)Person.HoldingObject).SetLength(((Wall)Person.HoldingObject).GetLength() + .01f);
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    ((Wall)Person.HoldingObject).SetLength(((Wall)Person.HoldingObject).GetLength() - .01f);
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    ((Wall)Person.HoldingObject).SetWidth(((Wall)Person.HoldingObject).GetWidth() + .01f);
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    ((Wall)Person.HoldingObject).SetWidth(((Wall)Person.HoldingObject).GetWidth() - .01f);
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                    ((Wall)Person.HoldingObject).SetHeight(((Wall)Person.HoldingObject).GetHeight() + .01f);
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    ((Wall)Person.HoldingObject).SetHeight(((Wall)Person.HoldingObject).GetHeight() - .01f);
                if (IsPressed(Keys.T))
                {
                    MyConsole.ConsoleOutLine("Current Dimensions: (" + ((Wall)Person.HoldingObject).GetLength() + ' ' + ((Wall)Person.HoldingObject).GetHeight() + ' ' + ((Wall)Person.HoldingObject).GetWidth() + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the X dimension(Length): ", graphics);
                    try
                    {
                        ((Wall)Person.HoldingObject).SetLength((float)MyConsole.ForgeModeOut);
                    }
                    catch (Exception) { }
                    MyConsole.ConsoleOutLine("Current Dimensions: (" + ((Wall)Person.HoldingObject).GetLength() + ' ' + ((Wall)Person.HoldingObject).GetHeight() + ' ' + ((Wall)Person.HoldingObject).GetWidth() + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the Y dimension(Height): ", graphics);
                    try
                    {
                        ((Wall)Person.HoldingObject).SetHeight((float)MyConsole.ForgeModeOut);
                    }
                    catch (Exception){}
                    MyConsole.ConsoleOutLine("Current Dimensions: (" + ((Wall)Person.HoldingObject).GetLength() + ' ' + ((Wall)Person.HoldingObject).GetHeight() + ' ' + ((Wall)Person.HoldingObject).GetWidth() + "). Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the Z dimension(Width): ", graphics);
                    try
                    {
                        ((Wall)Person.HoldingObject).SetWidth((float)MyConsole.ForgeModeOut);
                    }
                    catch (Exception) {}
                }

            }

            //color editing  mode
            if (ColorEditMode == true)
            {
                /*
                 * Old color edit system
                if (Person.HoldingObject.GetType() == typeof(Wall))
                {
                    String Red = ((Wall)Person.HoldingObject).GetColor().R.ToString();
                    String Green = ((Wall)Person.HoldingObject).GetColor().G.ToString();
                    String Blue = ((Wall)Person.HoldingObject).GetColor().B.ToString();
                    String Alpha = ((Wall)Person.HoldingObject).GetColor().A.ToString();
                    //forced-entry RGB Values
                    MyConsole.ConsoleOutLine("Current Red: " + Red + ". Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the model's red value (0 to 255): ", graphics);
                    try
                    {
                        if (MyConsole.ForgeModeOut.ToString() != "")
                            Red = MyConsole.ForgeModeOut.ToString();
                    }
                    catch (Exception) { }
                    MyConsole.ConsoleOutLine("Current Green: " + Green + ". Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the model's green value (0 to 255): ", graphics);
                    try
                    {
                        if (MyConsole.ForgeModeOut.ToString() != "")
                            Green = MyConsole.ForgeModeOut.ToString();
                    }
                    catch (Exception) { }
                    MyConsole.ConsoleOutLine("Current Blue: " + Blue + ". Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the model's blue value (0 to 255): ", graphics);
                    try
                    {
                        if (MyConsole.ForgeModeOut.ToString() != "")
                            Blue = MyConsole.ForgeModeOut.ToString();
                    }
                    catch (Exception) { }
                    MyConsole.ConsoleOutLine("Current Alpha (Transparency): " + Alpha + ". Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the model's alpha value (0 to 255): ", graphics);
                    try
                    {
                        if (MyConsole.ForgeModeOut.ToString() != "")
                            Alpha = MyConsole.ForgeModeOut.ToString();
                    }
                    catch (Exception) { }
                    //change color here
                    ((Wall)Person.HoldingObject).SetColor(new Color(Int32.Parse(Red), Int32.Parse(Green), Int32.Parse(Blue), Int32.Parse(Alpha)));
                    //((Wall)Person.HoldingObject).SetColor(DataValues.GetNextColor(((Wall)Person.HoldingObject).GetColor()));
                }*/

                //A to change the alpha value...for whatever reason the prompt does not inclued an alpha control
                if(IsPressed(Keys.A))
                {
                    String Red = ((Wall)Person.HoldingObject).GetColor().R.ToString();
                    String Green = ((Wall)Person.HoldingObject).GetColor().G.ToString();
                    String Blue = ((Wall)Person.HoldingObject).GetColor().B.ToString();
                    String Alpha = ((Wall)Person.HoldingObject).GetColor().A.ToString();
                    MyConsole.ConsoleOutLine("Current Alpha (Transparency): " + Alpha + ". Press enter to keep the original value.");
                    MyConsole.ConsoleInputPrompt("Enter the model's alpha value (0 to 255): ", graphics);
                    try
                    {
                        if (MyConsole.ForgeModeOut.ToString() != "")
                            Alpha = MyConsole.ForgeModeOut.ToString();
                    }
                    catch (Exception) { }
                    //change color here
                    ((Wall)Person.HoldingObject).SetColor(new Color(Int32.Parse(Red), Int32.Parse(Green), Int32.Parse(Blue), Int32.Parse(Alpha)));
                }
                //color values...weird conversions from byte have to be done
                if (IsPressed(Keys.T))
                {
                    //simple dialog that allows the user to choose a color from the color dialog
                    System.Drawing.Color FromDialogColor = MyConsole.ShowColorDialog(((Wall)Person.HoldingObject).GetColor(),graphics);
                    Color ConvertedFromDialogColor = new Color(FromDialogColor.R, FromDialogColor.G, FromDialogColor.B, FromDialogColor.A);
                    ((Wall)Person.HoldingObject).SetColor(ConvertedFromDialogColor);
                    MyConsole.CloseColorDialog();
                }
            }

            if (IsControlPressed() && IsPressed(Keys.S))
            {
                MyConsole.ConsoleOutLine("You are about to save the file.");
                MyConsole.ConsoleInputPrompt("Enter the file name: ", graphics);

                String FileName = "Files\\" + MyConsole.ForgeModeOut + ".xml";
                String FileNameBackup = "Files\\" + MyConsole.ForgeModeOut + "-backup.xml";

                if(File.Exists(FileName))
                {
                    if(File.Exists(FileNameBackup))
                        File.Delete(FileNameBackup);
                    File.Move(FileName, FileNameBackup);
                }

                Stream stream = File.Create(FileName);
                XmlSerializer serializer = new XmlSerializer(typeof(ModelReader));
                ModelReader XMLData = new ModelReader();
                XMLData.ActionModels = new List<ModelReader.ActionSpecs>(0);
                XMLData.Models = new List<ModelReader.ModelSpecs>(0);
                foreach (ScreenModel ThisModel in models)
                {
                    if ((ThisModel.GetType() != typeof(ScreenModel) || (ThisModel.ActionString == "Computer") || (ThisModel.ActionString == "PortalGun") || (ThisModel.ActionString == "Door")))
                    {
                        DataValues.UpdateActions(ThisModel);
                        XMLData.ActionModels.Add(new ModelReader.ActionSpecs(ThisModel.Name, ThisModel.ModelName, ThisModel.Position, ThisModel.modelRotation, ThisModel.Moveable, ThisModel.PickUpable, ThisModel.Bounce, ThisModel.IsLevitating, ThisModel.ActionString));
                    }
                    else
                        XMLData.Models.Add(new ModelReader.ModelSpecs(ThisModel.Name, ThisModel.ModelName, ThisModel.Position, ThisModel.modelRotation, ThisModel.Moveable, ThisModel.PickUpable, ThisModel.Bounce, ThisModel.IsLevitating));
                }
                XMLData.RoomPosition = Vector3.Zero;
                serializer.Serialize(stream, XMLData);
                stream.Close();


                /*
                FileStream File = new FileStream(FileName, FileMode.CreateNew);
                StreamWriter Writer = new StreamWriter(File);
                Writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<XnaContent>\n<Asset Type=\"DataTypes.ModelReader\">\n<RoomPosition>0 0 0</RoomPosition><Models>");
                foreach (ScreenModel ThisModel in models)
                    Writer.WriteLine("<Item><Name>" + ThisModel.Name + "</Name><ModelName>" + ThisModel.ModelName + "</ModelName><Position>" + ThisModel.Position.X + ' ' + ThisModel.Position.Y + ' ' + ThisModel.Position.Z + "</Position><Rotation>" + ThisModel.modelRotation.X + ' ' + ThisModel.modelRotation.Y + ' ' + ThisModel.modelRotation.Z + "</Rotation><Moveable>" + ThisModel.Moveable + "</Moveable><PickUpable>" + ThisModel.PickUpable + "</PickUpable></Item>");
                Writer.WriteLine("</Models></Asset></XnaContent>");
                Writer.Close();
                File.Close();*/

            }
            if (IsControlPressed() && IsPressed(Keys.O))
            {
                while (true)
                {
                    MyConsole.ConsoleOutLine("You are about to open a file.");
                    MyConsole.ConsoleInputPrompt("Enter the file name: ", graphics);
                    String FileName = "Files\\" + MyConsole.ForgeModeOut;
                    try
                    {
                        LoadNewModel(FileName, Content, models, graphics);
                        break;
                    }
                    catch (Exception)
                    {
                        MyConsole.ConsoleOutLine("File was not found. Please try again.");
                    }
                }
            }

            if (IsPressed(Keys.M))
            {
                LocationEditMode = false;
                RotationEditMode = false;
                ColorEditMode = false;
            }
            if (IsPressed(Keys.F))
            {
                LocationEditMode = false;
                RotationEditMode = false;
                WallEditMode = false;
                ColorEditMode = false;
            }

            spriteBatch.DrawString(DataValues.Font, ShowDirections ? Directions : ClosedDirections, new Vector2(10, 10), FontColorArg);
            spriteBatch.DrawString(DataValues.Font, Title + ((TemporaryGravity != 0) ? "\n(343 Guilty Spark Mode)" : "") + (LocationEditMode ? "\n(Location Edit Mode)" : "") + (RotationEditMode ? "\n(Rotation Edit Mode)" : "") + (WallEditMode ? "\n(Wall Edit Mode)" : "") + (ColorEditMode ? "\n(Color Edit Mode)" : ""), new Vector2(DataTypes.DataValues.ScreenWidth - 150, 10), FontColorArg);

            //spriteBatch.DrawString(DataValues.Font, DataValues.DisplayTime(), new Vector2(DataTypes.DataValues.ScreenWidth - 150, DataTypes.DataValues.ScreenHeight - 75), Color.Black);

            ScreenModel TestInFront = Person.ObjectIsInfront();
            if (TestInFront != null)
            {
                DataValues.HudItems[4] = DataValues.HudItems[3];
                DataValues.HudItems[3] = "Looking at: " + TestInFront.ModelName;
                //spriteBatch.DrawString(DataValues.Font, "Looking at: " + TestInFront.ModelName + "\nAt Distance: " + Vector3.Distance(Person.CameraPosition,TestInFront.Position), new Vector2(DataTypes.DataValues.ScreenWidth - 200, DataTypes.DataValues.ScreenHeight - 40), Color.Black);
            }
             
            //pringles
            //prevents mouse from moving/jumping about after exiting an edit mode
            //if(EditModeRunning() == false)
            LastState = Keyboard.GetState();
            LastMouseState = Mouse.GetState();
            return (true);
        }
        //************************************
        //reads the change in the mousewheel position from the last frame, much like the button
        public  int MouseScroll()
        {
            return (Mouse.GetState().ScrollWheelValue - LastMouseState.ScrollWheelValue);
        }
        //***************************************
        //physics alterations...use of function keys
        public void PhysicsMods(CollisionCheckSystem Person)
        {
            if (EditModeRunning() == false)
            {
                //like an eagle
                this.Fly(Person);
                this.ReverseGravity();
            }
        }
        //Fly-by mode
        public void Fly(CollisionCheckSystem Person)
        {
            //cntrl F1 to fly
            if(IsPressed(Keys.F1))
            {
                if (this.TemporaryGravity == 0f)
                {
                    this.TemporaryGravity = DataValues.gravity;
                    this.TemporaryGravityAcceleration = DataValues.GravityAcceleration;
                    DataValues.GravityAcceleration = 0f;
                    DataValues.gravity = 0f;
                }
                else
                {
                    DataValues.gravity = this.TemporaryGravity;
                    DataValues.GravityAcceleration = TemporaryGravityAcceleration;
                    this.TemporaryGravityAcceleration = 0f;
                    this.TemporaryGravity = 0f;
                }
            }
            if(DataValues.gravity == 0f)
            {
                //fly by mode enables us to act as the 343 Guilty Spark in Halo
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    Person.ForceMovement(new Vector3(0, (.5f * DataValues.MoveSensitivity), 0));
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        Person.ForceMovement(new Vector3(0, (5f * DataValues.MoveSensitivity), 0));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    Person.ForceMovement(new Vector3(0, (-.5f * DataValues.MoveSensitivity), 0));
                    if(Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        Person.ForceMovement(new Vector3(0, (-5f * DataValues.MoveSensitivity), 0));
                }
                //"Overrides" creation of the bounding box
                /*
                Person.MovingBoxes[0].Center = Vector3.Zero;
                Person.MovingBoxes[0].HalfExtent = Vector3.Zero;
                Person.MovingBoxes[0].Orientation = Quaternion.Identity;
                 */
            }
        }
        //for s's and g's you can tottally mess with physics in these methods
        private void ReverseGravity()
        {
            if (IsPressed(Keys.F2))
            {
                DataValues.gravity *= -1;
                //also changes the Temporary gravity, so changes are observed in fly by mode
                this.TemporaryGravity *= -1;
            }
        }
        //increase/decrease the force of gravity by small increments
        private void IncreaseFg()
        {
            if (IsPressed(Keys.F3))
            {
                DataValues.gravity *= 1.5f;
                //also changes the Temporary gravity, so changes are observed in fly by mode
                this.TemporaryGravity *= 1.5f;
            }
        }
        private void DecreaseFg()
        {
            if (IsPressed(Keys.F4))
            {
                DataValues.gravity *= .5f;
                //also changes the Temporary gravity, so changes are observed in fly by mode
                this.TemporaryGravity *= .5f;
            }
        }
    }
}
