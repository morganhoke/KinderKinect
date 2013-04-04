using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // TODO IMPLEMENTME
        public Button(Texture2D OffTex, Texture2D HighlightTex, Texture2D ClickTex, TimeSpan ButtonTime, List<ICursor> PossibleCursors)
        {
            offTex = OffTex;
            highlightTex = HighlightTex;
            clickTex = ClickTex;
        }
    }
}
