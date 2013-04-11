using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KinderKinect
{
    interface IActivity
    {
        /// <summary>
        /// Init all the non-graphics codez/settings this activity requires
        /// </summary>
        void Initalize();

        /// <summary>
        /// Load all the content this activity requires
        /// </summary>
        void LoadContent();

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        /// <summary>
        /// Free resources at this level
        /// </summary>
        void Unload();
    }
}
