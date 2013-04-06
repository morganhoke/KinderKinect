using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinderKinect.Utils.Gui
{
    /// <summary>
    /// A gui button that a cursor can interact with
    /// </summary>
    class Button
    {
        public enum ButtonState
        {
            Off,
            Highlighted,
            Clicked
        }

        /// <summary>
        /// The hitbox for this button
        /// </summary>
        private Hitbox myHitbox;
        public Hitbox Hit
        {
            get
            {
                return myHitbox;
            }
        }

        /// <summary>
        /// The texture to display when we are not interacting with this particular button
        /// </summary>
        private Texture2D offTex;
        public Texture2D OffTex
        {
            get
            {
                return offTex;
            }
        }

        /// <summary>
        /// The texture to display when we are highlighting the button
        /// </summary>
        private Texture2D highlightTex;
        public Texture2D HighlightTex
        {
            get
            {
                return highlightTex;
            }
        }

        /// <summary>
        /// The texture to display when we click the button
        /// </summary>
        private Texture2D clickTex;
        public Texture2D ClickTex
        {
            get
            {
                return clickTex;
            }
        }

        /// <summary>
        /// The time that we need to hover over the button before the click goes off
        /// </summary>
        private TimeSpan buttonTime;
        public TimeSpan ButtonTime
        {
            get
            {
                return buttonTime;
            }
        }

        private Rectangle area;
        public Rectangle Area
        {
            get
            {
                return area;
            }
        }

        /// <summary>
        /// Keeps a list of all the active cursors in the scene
        /// </summary>
        List<ICursor> cursors;

        ButtonState myState;


        public delegate void ClickedEventHandler(object sender, EventArgs e);
        public event ClickedEventHandler Entered;

        public Button(Texture2D OffTex, Texture2D HighlightTex, Texture2D ClickTex, TimeSpan ButtonTime, List<ICursor> PossibleCursors, Rectangle Area)
        {
            offTex = OffTex;
            highlightTex = HighlightTex;
            clickTex = ClickTex;
            cursors = PossibleCursors;
            myHitbox = new Hitbox(Area);
            buttonTime = ButtonTime;
            area = Area;
            myHitbox.Entered += new Hitbox.EnteredEventHandler(myHitbox_Entered);
            myHitbox.Exited += new Hitbox.ExitedEventHandler(myHitbox_Exited);
            myState = ButtonState.Off;
        }

        void myHitbox_Exited(object sender, EventArgs e)
        {
            (sender as ICursor).BeginHoverState(ButtonTime);
            myState = ButtonState.Off;
        }

        void myHitbox_Entered(object sender, EventArgs e)
        {
            (sender as ICursor).BreakHoverState();
            myState = ButtonState.Highlighted;
        }
    }
}
