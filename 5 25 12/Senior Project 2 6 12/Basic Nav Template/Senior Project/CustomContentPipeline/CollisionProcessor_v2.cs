using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;
//using CustomContentPipeline;
namespace CustomContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    //****************************************************************************************
    //This second version will test a new method: creating a series of merged bounding boxes via meshes into one bounding box
    [ContentProcessor(DisplayName = "CustomContentPipeline.CollisionProcessor_v2")]
    public class CollisionProcessor_v2 : ModelProcessor
    {
        //enumerated type that represents the collision possibilities 
        public enum CollisionKind { Sphere = 1, Box, Frustum, Ray, Plane, Room };
        public CollisionKind CollisionArg { get; set; }
        private List<object> ToSendInTag;
        //those vectors then make the bounding box
        public BoundingBox CollisionBox;
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            //allocates list that stores values from the processor to the ScreenModel class
            ToSendInTag = new List<object>();
            CollisionBox = new BoundingBox();
            //code that builds/creates the bounding box for collisions
            //gets the node data from the model-> allows us to go in further through meshes later to find vector sizes
            NodeContentCollection nodeContentCollection = input.Children;
            ModelContent TheContent = base.Process(input, context);
            //CollisionType = new ObjectTypes((int)CollisionArg);
            Enum EnumWrapper = CollisionArg;
            //tags the collision box...allows us to reference them outside...I've used tags before; they are cool and weird
            //As I need to send multiple values through the tag; why not use a list of generic objects?
            //ToSendInTag.Add(EnumWrapper);
            ToSendInTag.Add((int)CollisionArg);
            //only processes the shape of the bounding box when the bounding box is required, saves memory and processing time
            if (CollisionArg == CollisionKind.Box)
            {
                //for objects that are better off using a bounding box
                FindVectorValues(nodeContentCollection);
                ToSendInTag.Add(CollisionBox);
            }
            if (CollisionArg == CollisionKind.Room)
            {
                //calculates bounding collision box
                FindVectorValues(nodeContentCollection);
                //gets the points of the corners; should be 8 of them
                Vector3[] ListOfCorners = CollisionBox.GetCorners();
                //uses maths to get planes from these corners
                // 1  2     5  6
                // 3  4     7  8
                //  up      down
                //needs to get a total of 6 walls
                //create a list of planes to send over that represent the walls of a room
                List<Plane> ListOfWalls = new List<Plane>();
                //6 sides are recieved from using 3 points on each of the planes
                //#1- pts: 1,2,3
                ListOfWalls.Add(new Plane(ListOfCorners[0],ListOfCorners[1],ListOfCorners[2]));
                //#2- pts: 1,5,6
                ListOfWalls.Add(new Plane(ListOfCorners[0], ListOfCorners[4], ListOfCorners[5]));
                //#3- pts: 2,6,8
                ListOfWalls.Add(new Plane(ListOfCorners[1], ListOfCorners[5], ListOfCorners[7]));
                //#4- pts: 4,7,8
                ListOfWalls.Add(new Plane(ListOfCorners[3], ListOfCorners[6], ListOfCorners[7]));
                //#5- pts: 3,5,7
                ListOfWalls.Add(new Plane(ListOfCorners[2], ListOfCorners[4], ListOfCorners[6]));
                //#6- pts: 5,6,8
                ListOfWalls.Add(new Plane(ListOfCorners[4], ListOfCorners[5], ListOfCorners[7]));
                //added to the tag array
                ToSendInTag.Add(ListOfWalls);
            }
            //sends over the list
            TheContent.Tag = ToSendInTag;
            //returns the modified data with the tag (reference) to the bounding box
            return (TheContent);
        }
        //calculates the max and min 3D values of the Model's mesh to make the bounding box
        //I iz good at Maths; actually the math is BASIC
        //NodeCollection is the 
        private void FindVectorValues(NodeContentCollection nodeContentCollection)
        {
            //for each content node in the collection of content
            foreach (NodeContent nodeContent in nodeContentCollection)
            {
                //values local for each mesh made
                //finds the minimum and maximum vectors (min and max pts) in a model
                float MinX = float.MaxValue;
                float MinY = float.MaxValue;
                float MinZ = float.MaxValue;
                float MaxX = float.MinValue;
                float MaxY = float.MinValue;
                float MaxZ = float.MinValue;
                //temp bounding box made for a single mesh
                BoundingBox TempBox;
                //actually finds min and max size if it's a mesh and not some sort of mesh in a mesh (parent and child meshes/nodes)
                if (nodeContent is MeshContent)
                {
                    //collect Content of this mesh layer
                    MeshContent meshContent = (MeshContent)nodeContent;  //ModelMesh Model = (ModelMesh)meshContent;
                    foreach (Vector3 LocalVector in meshContent.Positions)
                    {
                        if (LocalVector.X < MinX)
                            MinX = LocalVector.X;
                        if (LocalVector.Y < MinY)
                            MinY = LocalVector.Y;
                        if (LocalVector.Z < MinZ)
                            MinZ = LocalVector.Z;
                        //now max vars
                        if (LocalVector.X > MaxX)
                            MaxX = LocalVector.X;
                        if (LocalVector.Y > MaxY)
                            MaxY = LocalVector.Y;
                        if (LocalVector.Z > MaxZ)
                            MaxZ = LocalVector.Z;
                    }
                    //found min and max values for one box, now it's time to add that single mesh box into the final box
                    TempBox = new BoundingBox(new Vector3(MinX, MinY, MinZ), new Vector3(MaxX, MaxY, MaxZ));
                    CollisionBox = BoundingBox.CreateMerged(CollisionBox, TempBox);
                }
                    /*
                else
                {
                    //This method is recursive if there are child meshes
                    FindVectorValues(nodeContent.Children);
                }*/
            }
        }
        //foreach (ModelMesh meshPart in meshContent.Children)
        //{
        //foreach mesh (essentially layer of the image) go through the mossitions
        //GeometryContent Geo = new GeometryContent();
        //foreach(GeometryContent Geo2 in Geo.Vertices.Positions)
        private void FindVectorValues(NodeContentCollection nodeContentCollection, bool Alternative)
        {
            //for each content node in the collection of content
            foreach (NodeContent nodeContent in nodeContentCollection)
            {
                //values local for each mesh made
                //finds the minimum and maximum vectors (min and max pts) in a model
                //temp bounding box made for a single mesh
                BoundingBox TempBox;
                if (nodeContent is MeshContent)
                {
                    //collect Content of this mesh layer
                    MeshContent meshContent = (MeshContent)nodeContent;
                    float MinX = float.MaxValue;
                    float MinY = float.MaxValue;
                    float MinZ = float.MaxValue;
                    float MaxX = float.MinValue;
                    float MaxY = float.MinValue;
                    float MaxZ = float.MinValue;
                    //new system for going through meshes
                    foreach (GeometryContent Geo in meshContent.Geometry)
                    {
                        //MinX = float.MaxValue;
                        //MinY = float.MaxValue;
                        //MinZ = float.MaxValue;
                        //MaxX = float.MinValue;
                        //MaxY = float.MinValue;
                        //MaxZ = float.MinValue;
                        for (int cntr = 0; cntr < Geo.Vertices.VertexCount; cntr++)
                        {
                            Vector3 LocalVector = Geo.Vertices.Positions[cntr];
                            if (LocalVector.X < MinX)
                                MinX = LocalVector.X;
                            if (LocalVector.Y < MinY)
                                MinY = LocalVector.Y;
                            if (LocalVector.Z < MinZ)
                                MinZ = LocalVector.Z;
                            //now max vars
                            if (LocalVector.X > MaxX)
                                MaxX = LocalVector.X;
                            if (LocalVector.Y > MaxY)
                                MaxY = LocalVector.Y;
                            if (LocalVector.Z > MaxZ)
                                MaxZ = LocalVector.Z;
                        }
                        //found min and max values for one box, now it's time to add that single mesh box into the final box
                        TempBox = new BoundingBox(new Vector3(MinX, MinY, MinZ), new Vector3(MaxX, MaxY, MaxZ));
                        CollisionBox = BoundingBox.CreateMerged(CollisionBox, TempBox);
                    }
                    //CollisionBox = new BoundingBox(new Vector3(MinX, MinY, MinZ), new Vector3(MaxX, MaxY, MaxZ));
                }
            }
        }
        /*
        public class ObjectTypes
        {
            //series of constants, stored in a object that hold what type of object should be used
            public const int Sphere = 1;
            public const int Box = 2;
            public const int Frustum = 3;
            public const int Ray = 4;
            public const int Play = 5;
            //public enum ObjectTypes { Sphere = 1, Box, Frustum, Ray, Plane, Room };
            public int TypeOfObject;
            public ObjectTypes(int TypeArg)
            {
                TypeOfObject = TypeArg;
            }
        }*/
    }
}
