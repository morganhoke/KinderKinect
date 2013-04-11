using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KinderKinect.Utils.Gui
{
    /// <summary>
    /// Represents a rectangular area of the screen that a cursor may intteract with, as well as methods to drive that interaction
    /// </summary>
    class Hitbox
    {
        /// <summary>
        /// The area on the screen that the player can interact with 
        /// </summary>
        private Rectangle hitArea;
        public Rectangle HitArea
        {
            get
            {
                return hitArea;
            }
        }

        private List<ICursor> cursorsInside;

        public delegate void EnteredEventHandler(object sender, EventArgs e);
        public event EnteredEventHandler Entered;

        public delegate void ExitedEventHandler(object sender, EventArgs e);
        public event ExitedEventHandler Exited;


        public Hitbox(Rectangle area)
        {
            hitArea = area;
            cursorsInside = new List<ICursor>();
        }



        public void update(List<ICursor> cursors)
        {
            foreach (ICursor c in cursors)
            {
                if (hitArea.Contains((int)(c.Position.X), (int)(c.Position.Y)))
                {
                    if (!cursorsInside.Contains(c))
                    {
                        cursorsInside.Add(c);
                        if (Entered != null)
                        {
                            Entered(c, new EventArgs());
                        }
                    }
                }
                else if (cursorsInside.Contains(c))
                {
                    cursorsInside.Remove(c);
                    if (Exited != null)
                    {
                        Exited(c, new EventArgs());
                    }
                }
            }
        }

    }
}
