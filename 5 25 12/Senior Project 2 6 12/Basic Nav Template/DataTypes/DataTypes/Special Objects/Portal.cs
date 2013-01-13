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
    public class Portal
    {
        //portal should contain an image file and a center position of the portal along with easy access to the area around the portal gun
        public Portal()
        {
        }
        //tells if the portal is open or not
        public bool IsActive()
        {
            return (false);
        }
    }
}
