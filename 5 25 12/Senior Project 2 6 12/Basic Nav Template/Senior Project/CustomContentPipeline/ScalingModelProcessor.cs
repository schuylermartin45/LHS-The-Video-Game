using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace CustomContentPipeline
{
    [ContentProcessor(DisplayName = "CustomContentPipeline.TestScaleModelProcessor")]
    public class ScalingModelProcessor : ModelProcessor
    {
        
        //to make a variable a "property" (seen in the right-hand menu) you do this:
        private float myscale = 1.0f;  // Backing store
        public float MyScale
        {
            get
            {
                return myscale;
            }
            set
            {
                myscale = value;
            }
        }
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            MeshHelper.TransformScene(input, Matrix.CreateScale(MyScale));
            //base is C#'s version of "super"; as in the "base" class
            return base.Process(input, context);
        }
    }
}
