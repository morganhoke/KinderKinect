using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils.Gui;
using Microsoft.Xna.Framework;

namespace KinderKinect.ButterflyGarden
{
    class Butterfly
    {
        public enum ButterflyColors
        {
            Red,
            Yellow,
            Blue,
            Green,
            Purple,
            Black,
            White,
            Brown,
            Orange,
            Pink
        }

        private Hitbox hitbox;

        public Butterfly(Rectangle area)
        {
            hitbox = new Hitbox(area);
        }

        public void Update()
        {
            
        }

        public void Draw()
        {
        }
    }
}
