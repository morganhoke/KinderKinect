using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyActivity : IActivity
    {
        ContentManager content;

        ButterflyActivity(Game1 myGame)
        {
            content = new ContentManager(myGame.Services, "Content");
        }

        public void Initalize()
        {
            
        }

        public void LoadContent()
        {
            
        }

        public void Update()
        {
            
        }

        public void Draw(GameTime gameTime)
        {
            
        }

        public void Unload()
        {
            
        }
    }
}
