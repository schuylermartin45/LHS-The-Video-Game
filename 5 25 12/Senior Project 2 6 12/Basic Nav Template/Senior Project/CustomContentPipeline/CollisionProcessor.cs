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
    [ContentProcessor(DisplayName = "CustomContentPipeline.CollisionProcessor")]
    public class CollisionProcessor : ModelProcessor
    {
        //list of "integer" constants; this enum limits the types of data that can be set for this property
        //this is found in the list in the designer
        public enum Collisions { Sphere = 1, Box, Frustum, Ray, Plane, Room };
        //Properties of this content importer
        //short hand accessor/mutator for properties
        public Collisions CollisionType { get; set; }
        //bellow is old syntax
        /*
        private Collisions collisiontype = Collisions.Sphere;        
        public Collisions CollisionType
        {
            //accessor/muttator for the collision type
            get
            {
                return collisiontype;
            }
            set
            {
                if (value == 0)
                    throw new ArgumentNullException();
                collisiontype = value;
            }
        }*/
        /*
        //public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        public CollisionProcessor(NodeContent input, ContentProcessorContext context)
        {
            if (CollisionType == Collisions.Frustum)
                Console.WriteLine("Continue Testing");
            System.Console.WriteLine("Continue");
            base.Process(input, context);
        }
        public CollisionProcessor()
        {
            
        }*/
        //finds the minimum and maximum vectors (min and max pts) in a model
        private float MinX = float.MaxValue;
        private float MinY = float.MaxValue;
        private float MinZ = float.MaxValue;
        private float MaxX = float.MinValue;
        private float MaxY = float.MinValue;
        private float MaxZ = float.MinValue;
        //those associated vectors
        private Vector3 MinVect;
        private Vector3 MaxVect;
        //those vectors then make the bounding box
        public BoundingBox CollisionBox;
        
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            //scale and rotate objects before drawing the bounding box
            //base.Process(input, context);
            //MeshHelper.TransformScene(input, Matrix.CreateScale(this.Scale));

            //gets rest of processing in the base class of the model processor
            //ModelContent TheContent = base.Process(input, context);


            //code that builds/creates the bounding box for collisions
            //gets the node data from the model-> allows us to go in further through meshes later to find vector sizes
            NodeContentCollection nodeContentCollection = input.Children;
            //for objects that are better off using a bounding box
            //if (this.CollisionType == Collisions.Box)
            //{
                FindVectorValues(nodeContentCollection);
            //}
            //for objects that are better off using a bounding sphere
                /*
            else if (this.CollisionType == Collisions.Sphere)
            {
                //calculates 
                BoundingSphere CollisionSphere = new BoundingSphere(TempVect,Radius);
                
            }*/
            //now that these values have been changed/found the min and mav vectors can be instantiated
            MinVect = new Vector3(MinX, MinY, MinZ);
            MaxVect = new Vector3(MaxX, MaxY, MaxZ);
           // MinVect *= this.Scale;
           // MaxVect *= this.Scale;
            //now the actual bounding box can be allocated to memory
            CollisionBox = new BoundingBox(MinVect, MaxVect);
            
            ModelContent TheContent = base.Process(input, context);

            //tags the collision box...allows us to reference them outside...I've used tags before; they are cool and weird
            //if (this.CollisionType == Collisions.Box)
            //{
                TheContent.Tag = CollisionBox;
            //}
            /*
            else if (this.CollisionType == Collisions.Sphere)
            {
                TheContent.Tag = CollisionBox;
            }*/
            //returns the modified data with the tag (reference) to the bounding box
            return (TheContent);
            //returns back to the super class
            //return base.Process(input, context);
        }
        //calculates the max and min 3D values of the Model's mesh to make the bounding box
        //I iz good at Maths; actually the math is BASIC
        //NodeCollection is the 
        private void FindVectorValues(NodeContentCollection nodeContentCollection)
        {
            //for each content node in the collection of content
            foreach (NodeContent nodeContent in nodeContentCollection)
            {
                //actually finds min and max size if it's a mesh and not some sort of mesh in a mesh (parent and child meshes/nodes)
                if (nodeContent is MeshContent)
                {
                    //collect Content of this mesh layer
                    MeshContent meshContent = (MeshContent)nodeContent;
                    //foreach mesh (essentially layer of the image) go through the mossitions
                    foreach (Vector3 LocalVector in meshContent.Positions)
                    {
                        if (LocalVector.X < MinX)
                            MinX = LocalVector.X;
                        if (LocalVector.Y < MinY)
                            MinY = LocalVector.Y;
                        if (LocalVector.Z < MinZ)
                            MinZ = LocalVector.Z;
                        //
                        if (LocalVector.X > MaxX)
                            MaxX = LocalVector.X;
                        if (LocalVector.Y > MaxY)
                            MaxY = LocalVector.Y;
                        if (LocalVector.Z > MaxZ)
                            MaxZ = LocalVector.Z;
                    }
                }
                else
                {
                    //This method is recursive if there are child meshes
                    FindVectorValues(nodeContent.Children);
                }
            }
        }
    }
}