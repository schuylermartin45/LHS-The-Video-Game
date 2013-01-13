using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DataTypes.AI
{
    //the AI director is a collective force or "intelligence" behind the scenes; this dictates the grand decisions made in a broader sense
    public class AIDirector
    {
        public List<AIModel> ListOfAI = new List<AIModel>();
        //AI should exist until they reach their room destination, then dissappear for general hallway actions

        //once in a room, randomly allocate AI into a room

        //fills the lsit
        public void FillList(ContentManager Content,GraphicsDevice device)
        {
            for (int cntr = 0; cntr < ListOfAI.Count; cntr++)
            {
                ListOfAI[cntr] = new AIModel(Content.Load<Model>("Human AI"),device);
            }
        }
        //moves through the list of AI and tells them to move
        public void MoveALL(List<Door> InRange)
        {
            for(int cntr = 0; cntr<ListOfAI.Count; cntr++)
            {
                ListOfAI[cntr].MoveAI(InRange);
            }
        }
    }
}
