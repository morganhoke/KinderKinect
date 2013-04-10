using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using KinderKinect.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyActivity : IActivity
    {
        ContentManager content;
        Camera myCamera;
        ButterflyLevel currentLevel;
        ButterflyPlayer player;
        Game1 myGame;

        public ButterflyActivity(Game1 MyGame)
        {
            myGame = MyGame;
            content = new ContentManager(myGame.Services, "Content");
            myCamera = new Camera();
            float aspectRatio = (float)myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)myGame.GraphicsDevice.PresentationParameters.BackBufferHeight;
            myCamera.Perspective(90, aspectRatio, 0.1f, 1000f);
           
            myCamera.Position = new Vector3(0f, 1f, -10f);
            myCamera.LookAt(new Vector3(0, 0, 0));
            player = new ButterflyPlayer(myGame, new Vector3(0f, 1f, 0f), 0f);
        }

        public void Initalize()
        {
            currentLevel = new ButterflyLevel(myCamera, player);
        }

        public void LoadContent()
        {
            Butterfly.ButterflyTextures = new Texture2D[Enum.GetNames(typeof(Butterfly.ButterflyColors)).Length];
            for (int i = 0; i < Butterfly.ButterflyTextures.Length; i++)
            {
                Butterfly.ButterflyTextures[i] = content.Load<Texture2D>(@"Textures\ButterflyGarden\" + Enum.GetNames(typeof(Butterfly.ButterflyColors)).ElementAt(i));
            }
            player.LoadContent(content);
            currentLevel.LoadContent(content, myGame.GraphicsDevice.Viewport);
        }

        public void Update()
        {
            currentLevel.Update();
        }

        public void Draw(GameTime gameTime)
        {
            currentLevel.Draw(myGame.GraphicsDevice);
        }

        public void Unload()
        {
            content.Unload();
        }
    }
}
