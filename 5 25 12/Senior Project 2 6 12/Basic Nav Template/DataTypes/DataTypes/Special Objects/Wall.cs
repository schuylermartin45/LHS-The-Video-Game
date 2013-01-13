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
    public class Wall : ScreenModel
    {
        //a master list of all wall vertices so that they all may be drawn at once (easier for graphics card to handle)
        private static List<List<VertexPositionNormalColor>> VerticesMasterList = new List<List<VertexPositionNormalColor>>(0);
        private static int VerticesListCounter = 0;
        private float Length = 1;
        private float Width = 1;
        private float Height = 1;
        private Color wallColor = Color.Tan;
        private List<VertexPositionNormalColor> vertices;
        private List<VertexPositionNormalColor> verticesLowRes;
        Vector3 LastPosition;
        Vector3 LastRotation;
        VertexBuffer LineVertices;
        IndexBuffer Indices;
        private float span = 1f;

        public Wall(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float bounciness, Boolean levitating, String ActionInfo)
            : base(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, moveable, pickup, bounciness, levitating)
        {
            String Dimensions = ActionInfo.Substring(ActionInfo.IndexOf("Wall{") + 5, ActionInfo.IndexOf("}") - ActionInfo.IndexOf("Wall{") - 5);
            this.Length = float.Parse(Dimensions.Substring(Dimensions.IndexOf("Length:") + 7, Dimensions.IndexOf(';', Dimensions.IndexOf("Length:")) - Dimensions.IndexOf("Length:") - 7));
            this.Width = float.Parse(Dimensions.Substring(Dimensions.IndexOf("Width:") + 6, Dimensions.IndexOf(';', Dimensions.IndexOf("Width:")) - Dimensions.IndexOf("Width:") - 6));
            this.Height = float.Parse(Dimensions.Substring(Dimensions.IndexOf("Height:") + 7, Dimensions.IndexOf(';', Dimensions.IndexOf("Height:")) - Dimensions.IndexOf("Height:") - 7));
            int storeSpot = 0;
            try
            {
                wallColor = new Color(
                int.Parse(Dimensions.Substring(storeSpot = (Dimensions.IndexOf("R:", Dimensions.IndexOf("Color(")) + 2), Dimensions.IndexOf(",", Dimensions.IndexOf("R:", Dimensions.IndexOf("Color("))) - storeSpot)),
                int.Parse(Dimensions.Substring(storeSpot = (Dimensions.IndexOf("G:", Dimensions.IndexOf("Color(")) + 2), Dimensions.IndexOf(",", Dimensions.IndexOf("G:", Dimensions.IndexOf("Color("))) - storeSpot)),
                int.Parse(Dimensions.Substring(storeSpot = (Dimensions.IndexOf("B:", Dimensions.IndexOf("Color(")) + 2), Dimensions.IndexOf(",", Dimensions.IndexOf("B:", Dimensions.IndexOf("Color("))) - storeSpot)),
                int.Parse(Dimensions.Substring(storeSpot = (Dimensions.IndexOf("A:", Dimensions.IndexOf("Color(")) + 2), Dimensions.IndexOf(")", Dimensions.IndexOf("A:", Dimensions.IndexOf("Color("))) - storeSpot)));
            }
            catch (Exception e) { }

            //VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));

            this.LastPosition = this.Position;
            this.LastRotation = this.modelRotation;
            this.CalculateBoundingBox();
            this.CalculateBox();
            
            //Walls should not be moveable or pickupable by default - LIES!!!!
            /*
            this.Moveable = false;
            this.PickUpable = false;
            this.IsLevitating = true;
             */
        }
        public Wall(String Name, String ModelName, Model SentModel, Vector3 ModelPosition, Vector3 ModelRotation, GraphicsDevice device, Boolean moveable, Boolean pickup, float bounciness, Boolean levitating, float Length, float Width, float Height)
            : base(Name, ModelName, SentModel, ModelPosition, ModelRotation, device, moveable, pickup, bounciness, levitating)
        {
            this.Length = Length;
            this.Width = Width;
            this.Height = Height;
            //VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));
            this.LastPosition = this.Position;
            this.LastRotation = this.modelRotation;
            this.CalculateBoundingBox();
            this.CalculateBox();

            //Walls should not be moveable or pickupable by default - LIES!!!!
            /*
            this.Moveable = false;
            this.PickUpable = false;
            this.IsLevitating = true;
             */
        }
        public new void CalculateBoundingBox()
        {

            Matrix Transforms = Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position);
            Vector3 Extents = new Vector3(Length / 2, Height / 2, Width / 2);
            try
            {
                MovingBoxes[0].Center = Position;
                MovingBoxes[0].HalfExtent = Extents;
                MovingBoxes[0].Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position));//Quaternion.CreateFromYawPitchRoll(modelRotation.Y, modelRotation.X, modelRotation.Z);
            }
            catch (Exception)
            {
                Quaternion Rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position));//Quaternion.CreateFromYawPitchRoll(modelRotation.Y, modelRotation.X, modelRotation.Z);
                this.MovingBoxes = new List<OrientedBoundingBox>(0);
                MovingBoxes.Add(new OrientedBoundingBox(Position, Extents, Rotation));
            }
        }
        public void CalculateBox()
        {
            Matrix Transforms = Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position);


            /*if (Length + Width + Height > 1000)
                span = Length + Width + Height/5;*/
            if (Length + Width + Height > 500)
                span = 100;
            else if (Length + Width + Height > 300)
                span = 50;
            else if (Length + Width + Height > 100)
                span = 15;
            else
                span = 3;

            //span = (Length + Width + Height) / 30;
            float endX = (this.Length > span) ? this.Length / 2 - (this.Length % span) : -this.Length / 2;
            float endY = (this.Height > span) ? this.Height / 2 - (this.Height % span) : -this.Height / 2;
            float endZ = (this.Width > span) ? this.Width / 2 - (this.Width % span) : -this.Width / 2;
            List<VertexPositionNormalColor> Positions = new List<VertexPositionNormalColor>(0);

            // Front Surface
            for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
                for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
                {
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                }
            for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            }
            for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            }
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));

            // Back Surface
            for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
                for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
                {
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, y, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, y + span, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, y, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, y, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, y + span, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, y + span, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                }
            for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, endY, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, endY, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, endY, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            }
            for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, y, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, y, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, y + span, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            }
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, endY, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, endY, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, -this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));

            // Left Surface
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
                for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
                {
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y, z), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y + span, z), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y + span, z), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y + span, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                }
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, endY, z), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, endY, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, endY, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            }
            for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y + span, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y + span, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            }
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, endY, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));

            // Right Surface
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
                for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
                {
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, z), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, z), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, z), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                }
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, z), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, z), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            }
            for (float y = -this.Height / 2; y + span <= this.Height / 2; y += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, y + span, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            }
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, endY, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));

            // Top Surface
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
                for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
                {
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                }
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            }
            for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            }
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));

            // Bottom Surface
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
                for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
                {
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, -this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, -this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, -this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, -this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, -this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                    Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, -this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                }
            for (float z = -this.Width / 2; z + span <= this.Width / 2; z += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, -this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, -this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -this.Height / 2, z), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, -this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -this.Height / 2, z + span), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));  
            }
            for (float x = -this.Length / 2; x + span <= this.Length / 2; x += span)
            {
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, -this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, -this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, -this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, -this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x, -this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
                Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(x + span, -this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));  
            }
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, -this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, -this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -this.Height / 2, endZ), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(endX, -this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));  

            // Front Surface    
            /*
            new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, 1), Transforms)),
            new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, 1), Transforms)), 
            new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, 1), Transforms)), 
            new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, 1), Transforms)), 
            new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, 1), Transforms)), 
            new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, 1), Transforms)),  
            
            // Back Surface
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, -1), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 0, -1), Transforms)));

            // Left Surface
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(-1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(-1, 0, 0), Transforms)));

            // Right Surface
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(1, 0, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(1, 0, 0), Transforms)));

            // Top Surface
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, 1, 0), Transforms)));

            // Bottom Surface
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, -1*this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(-1*this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, -1, 0), Transforms)));
            Positions.Add(new VertexPositionNormalColor(Vector3.Transform( new Vector3(this.Length/2, -1*this.Height/2, this.Width/2), Transforms),wallColor, Vector3.Transform( new Vector3(0, -1, 0), Transforms)));
            */
            this.vertices = Positions;
            ScreenModel.BoundingBoxBuffers[] buffers = new ScreenModel.BoundingBoxBuffers[MovingBoxes.Count];


            List<VertexPositionNormalColor> PositionsLowRes = new List<VertexPositionNormalColor>(0);
            // Front Surface
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, 1), Transforms)));  
            
            // Back Surface
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 0, -1), Transforms)));

            // Left Surface
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(-1, 0, 0), Transforms)));

            // Right Surface
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(1, 0, 0), Transforms)));

            // Top Surface
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, 1, 0), Transforms)));

            // Bottom Surface
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, -1 * this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(-1 * this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            PositionsLowRes.Add(new VertexPositionNormalColor(Vector3.Transform(new Vector3(this.Length / 2, -1 * this.Height / 2, this.Width / 2), Transforms), wallColor, Vector3.Transform(new Vector3(0, -1, 0), Transforms)));
            verticesLowRes = PositionsLowRes;
        }
        public void Update(BasicEffect effects)
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

            for (int x = 0; x < MovingBoxes.Count; x++)
                MovingBoxes[x].Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z) * Matrix.CreateTranslation(Position));//Quaternion.CreateFromYawPitchRoll(modelRotation.Y, modelRotation.X, modelRotation.Z);// Matrix.CreateRotationX(modelRotation.X) * Matrix.CreateRotationY(modelRotation.Y) * Matrix.CreateRotationZ(modelRotation.Z));
            if ((this.Position != this.LastPosition) || (this.modelRotation != this.LastRotation))
            {
                this.CalculateBox();
                this.LastPosition = this.Position;
                this.LastRotation = this.modelRotation;
                this.CalculateBoundingBox();
            }
            // Set the transform for the triangle, then draw it, using the created effect
            effects.VertexColorEnabled = true;

            if(VerticesMasterList.Count==0)
                VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));
            if (DataValues.VectorDistance(this.Position - DataValues.UserPosition) < 150 + Math.Sqrt(Length * Length / 4 + Width * Width / 4 + Height * Height / 4))
            {
                if (VerticesMasterList[VerticesListCounter].Count + vertices.Count > 65535)
                {
                    VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));
                    VerticesListCounter++;
                }
                VerticesMasterList[VerticesListCounter].AddRange(vertices);
            }
            else
            {
                if (VerticesMasterList[VerticesListCounter].Count + verticesLowRes.Count > 65535)
                {
                    VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));
                    VerticesListCounter++;
                }
                VerticesMasterList[VerticesListCounter].AddRange(verticesLowRes);
            }
            /*for (int x = 0; x < vertices.Length; x++)
            {
                if ((VerticesMasterList[VerticesListCounter].Count > 54217720) && (VerticesMasterList[VerticesListCounter].Count % 3 == 0))
                {
                    VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));
                    VerticesListCounter++;
                }
                VerticesMasterList[VerticesListCounter].Add(vertices[x]);
            }*/


        }
        public static void DrawAllWalls(Effect effects, GraphicsDevice device)
        {
            //try
            //{
            /*
            List<List<VertexPositionNormalColor>> VerticesList = new List<List<VertexPositionNormalColor>>(0);
            VerticesList.Add(new List<VertexPositionNormalColor>(0));
            int count = 0;
            for (int counter = 0; counter < VerticesMasterList.Count; counter++)
                count+=VerticesMasterList[counter].Length;
            VertexPositionNormalColor[] vertices = new VertexPositionNormalColor[count];
            count=0;
            for (int counter = 0; counter < VerticesMasterList.Count; counter++)
            {
                System.Buffer.BlockCopy(VerticesMasterList[counter], 0, vertices, count, VerticesMasterList[counter].Length * 8);
                count+=VerticesMasterList[counter].Length;
                
                if (VerticesList[count].Count + VerticesMasterList[counter].Length > 100000)
                {   
                    count++;
                    VerticesList.Add(new List<VertexPositionNormalColor>(0));
                }
                VerticesList[count].AddRange(VerticesMasterList[counter]);
            }
            */
            for(int counter=0; counter<VerticesMasterList.Count; counter++)
            {
                VertexPositionNormalColor[] vertices = VerticesMasterList[counter].ToArray();
                if (vertices.Length == 0)
                    continue;
                //running try-catches in loops that are repeating themselves a lot is BAD!
                //Try-catches are memory intensive
                //placing the try catch outside of these two loops helps a little, but solving the issue with
                //device.DrawPrimitives will be even better
                /*
                 * arr1 = {1,2,3,4}
                 * arr2 = {5,6,7,8}
                 * memmove(*arr2+arr1.length)
                 * arr3 = int[arr1.length+arr2.length]
                 * *arr3 = &arr1
                 */
                //try
                //{
                foreach (EffectPass pass in effects.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Draw the six different surfaces of the cube
                    device.DrawUserPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
                }
                //}
                //catch { }
            }
            //catch { }
            VerticesMasterList.Clear();
            VerticesListCounter = 0;
            VerticesMasterList.Add(new List<VertexPositionNormalColor>(0));
        }
        public float GetHeight()
        {
            return this.Height;
        }
        public float GetWidth()
        {
            return this.Width;
        }
        public float GetLength()
        {
            return this.Length;
        }
        public Color GetColor()
        {
            return this.wallColor;
        }
        public void SetHeight(float height)
        {
            this.Height = height;
            this.CalculateBox();
            this.CalculateBoundingBox();
        }
        public void SetWidth(float width)
        {
            this.Width = width;
            this.CalculateBox();
            this.CalculateBoundingBox();
        }
        public void SetLength(float length)
        {
            this.Length = length;
            this.CalculateBox();
            this.CalculateBoundingBox();
        }
        public void SetColor(Color WallColor)
        {
            this.wallColor = WallColor;
            this.CalculateBox();
        }
        public void UpdateActionString()
        {
            this.ActionString = "Wall{Length:" + this.Length + ";Height:" + this.Height + ";Width:" + this.Width + ";Color(R:" + wallColor.R + ",G:" + wallColor.G + ",B:" + wallColor.B + ",A:" + wallColor.A + ");}";
        }

        //reticle for walls when in forge
        public override DataValues.ReticleType SpecialReticle(bool ForgeWorldActive)
        {
            if (ForgeWorldActive == false)
                return (DataValues.ReticleType.Normal);
            else
                return (DataValues.ReticleType.ActionBtn);
        }
    }
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        public VertexPositionNormalColor(Vector3 Position, Color Color, Vector3 Normal)
        {
            this.Position = Position;
            this.Color = Color;
            this.Normal = Normal;
        }
    }
}