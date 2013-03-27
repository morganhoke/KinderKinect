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
        /// Updates the postition of the cursor if it is polling-based
        /// </summary>
        void Update();



    }
}
