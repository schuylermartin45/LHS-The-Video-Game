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

namespace DataTypes
{
    public class ScreenMenu
    {
        //This is the list of strings that are shown in the menu
        public List<String> MenuItems;

        //The current Item Selected (Uses Get and Set methods to not allow for values out of range)
        private int ItemSelected;

        //Font Size
        private float FontSize;

        //The amount of items Shown at once
        private int ItemsShown;

        //The location of the menu
        public Vector2 Location;

        //The color of the font of most values
        public Color FontColor;

        //The type of font(Should not be changed, because this class is optomized to the font set provided
        public SpriteFont Font;

        //the background rectangle color
        public Color BackgroundColor;

        //the color of the selected item
        public Color SelectedItemColor;

        //Set if you want to show the background
        public Boolean ShowBackground = true;

        //This has to be a transparent or solid white for this class to have optimal colors
        public Texture2D BlankBackground;
        public Vector2 Padding = new Vector2(2, 1);
        //Initializes the menu with default values, but needs to have the background and font sent over
        public ScreenMenu(Texture2D Background, SpriteFont Font)
        {
            this.Font = Font;
            MenuItems = new List<String>();
            ItemSelected = 0;
            Location = new Vector2(0, 0);
            ItemsShown = 3;
            FontColor = Color.Black;
            BackgroundColor = Color.White;
            FontSize = 1;
            SelectedItemColor = Color.Red;
            this.BlankBackground = Background;

        }

        //initializes the menu to be fully customizable
        public ScreenMenu(List<String> MenuItems, Vector2 MenuLocation, SpriteFont Font, int ItemsShownAtOnce, int SelectedItem, Color TextColor, Color SelectedItemColor, Color BackgroundColor, float FontSize, Texture2D Background)
        {
            this.MenuItems = MenuItems;
            this.ItemSelected = SelectedItem;
            this.Location = MenuLocation;
            this.ItemsShown = ItemsShownAtOnce;
            this.FontColor = TextColor;
            this.SelectedItemColor = SelectedItemColor;
            this.BackgroundColor = BackgroundColor;
            this.FontSize = FontSize/12;
            this.BlankBackground = Background;
            this.Font = Font;
        }

        //changes the selected item such that it will not be out of bounds
        public void ChangeSelectedItem(int ItemSpot)
        {
            if ((ItemSpot >= 0) && (ItemSpot < MenuItems.Count))
                this.ItemSelected = ItemSpot;
            else if (ItemSpot < 0)
                this.ItemSelected =MenuItems.Count - 1;
            else
                this.ItemSelected = 0;
        }

        //gets the string selected in the menu
        public String GetSelectedItem()
        {
            return MenuItems[ItemSelected];
        }

        //gets the position selected in the menu
        public int GetSelectedItemSpot()
        {
            return this.ItemSelected;
        }

        //Draws the menu. Should be placed last so that the menu appears on top
        public void Draw(SpriteBatch spriteBatch)
        {
            //finds the widest string in the menu, and sets up the width of the menu to be accomodating
            Vector2 LargestString = Vector2.Zero;
            foreach (String word in MenuItems)
                LargestString.X = Vector2.Max(Font.MeasureString(word), LargestString).X;
            LargestString.Y = Font.MeasureString(" ").Y;
            LargestString += Padding;

            //shows the background rectangle, with cool maths to control the height and width
            if (ShowBackground)
                spriteBatch.Draw(BlankBackground, new Rectangle((int)Location.X, (int)Location.Y, (int)(LargestString.X * FontSize + 30), (int)(3 * 5 * FontSize * ItemsShown + 24)), BackgroundColor);

            //Shows the items in the list, and uses some cool maths to determine the the padding around the outside, and the space between each line
            for (int x = ItemSelected; x < ((ItemsShown + ItemSelected < MenuItems.Count) ? ItemSelected + ItemsShown : MenuItems.Count); x++)
                spriteBatch.DrawString(Font, MenuItems[x], Location + new Vector2(15, 12 + FontSize * LargestString.Y * (x - ItemSelected)), (x == ItemSelected) ? SelectedItemColor : FontColor, 0, new Vector2(0, 0), FontSize, new SpriteEffects(), 0);
        }
    }
}
