using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KinderKinect.Utils
{
    interface ICursor
    {
        /// <summary>
        /// Represents the screenspace coordinates of the cursor.
        /// </summary>
        Vector2 Position
        {
            get;
        }

        /// <summary>
        /// Represents a rectangle region for selection
        /// </summary>
        Rectangle rect
        {
            get;
        }

        bool Valid
        {
            get;
        }

        /// <summary>
        /// Updates the postition of the cursor if it is polling-based
        /// </summary>
        void Update();

        /// <summary>
        /// Used to tell the cursor that it is hovering over a time sensative element
        /// </summary>
        void BeginHoverState(TimeSpan timeToCompletion);

        /// <summary>
        /// Used to tell the cursor that it has stopped hovering over a time sensative element
        /// </summary>
        void BreakHoverState();

    }
}
