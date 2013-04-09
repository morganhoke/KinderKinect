using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace KinderKinect.Utils
{
    /// <summary>
    /// This is a quick and dirty way for me to be able to draw whatever I want whenever I want in order to extract debugging information
    /// </summary>
    public class DebugDrawer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D debugTexture;
        Game1 myGame;
        SpriteBatch spriteBatch;
        ICursor cursorL, cursorR, cursorM;
        BackgroundSubtractedPlayer player;
        public DebugDrawer(Game game)
            : base(game)
        {
            myGame = game as Game1;

            // TODO: Construct any child components here
        }

        protected override void LoadContent()
        {
            debugTexture = myGame.Content.Load<Texture2D>("Textures\\DebugColor");
            cursorR = new KinectAbsoluteScreenspaceCursor(myGame.Services.GetService(typeof(KinectService)) as KinectService, KinectAbsoluteScreenspaceCursor.Handedness.Left);
            cursorL = new KinectAbsoluteScreenspaceCursor(myGame.Services.GetService(typeof(KinectService)) as KinectService, KinectAbsoluteScreenspaceCursor.Handedness.Right);
            cursorM = new MouseCursor();
            player = new BackgroundSubtractedPlayer(myGame);
            myGame.Components.Add(player);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            cursorR.Update();
            cursorL.Update();
            cursorM.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch = myGame.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(debugTexture, new Rectangle((int)cursorR.Position.X, (int)cursorR.Position.Y, 48, 48),null, Color.White, 0f, new Vector2(24, 24), SpriteEffects.None, 0f);
            spriteBatch.Draw(debugTexture, new Rectangle((int)cursorL.Position.X, (int)cursorL.Position.Y, 48, 48), null, Color.White, 0f, new Vector2(24, 24), SpriteEffects.None, 0f);
            spriteBatch.Draw(debugTexture, new Rectangle((int)cursorM.Position.X, (int)cursorM.Position.Y, 48, 48), null, Color.White, 0f, new Vector2(24, 24), SpriteEffects.None, 0f);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }
    }
}
