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
using Microsoft.Kinect;

namespace KinderKinect.Utils
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BackgroundSubtractedPlayer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D kinectVideoTexture = null;
        KinectSensor kinect;
        Game1 myGame;
        byte[] colorData = null;
        DepthImagePixel[] depthData = null;

        Skeleton[] skeletons = null;
        Skeleton activeSkeleton = null;
        Color[] bitmap;


        int activeSkeletonNumber;

        public BackgroundSubtractedPlayer(Game game)
            : base(game)
        {
            myGame = game as Game1;
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

        protected override void LoadContent()
        {
            kinect = myGame.Services.GetService(typeof(KinectSensor)) as KinectSensor;
            kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
            kinectVideoTexture = new Texture2D(myGame.GraphicsDevice, 640, 480);
            base.LoadContent();
        }

        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;

                if (colorData == null)
                    colorData = new byte[colorFrame.Width * colorFrame.Height * 4];

                colorFrame.CopyPixelDataTo(colorData);

                kinectVideoTexture = new Texture2D(GraphicsDevice, colorFrame.Width, colorFrame.Height);

                bitmap = new Color[colorFrame.Width * colorFrame.Height];

                int sourceOffset = 0;

                for (int i = 0; i < bitmap.Length; i++)
                {
                    bitmap[i] = new Color(colorData[sourceOffset + 2],
                        colorData[sourceOffset + 1], colorData[sourceOffset], 255);
                    sourceOffset += 4;
                }

                kinectVideoTexture.SetData(bitmap);
            }

            // Finds the currently active skeleton

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);
            }

            activeSkeletonNumber = 0;

            for (int i = 0; i < skeletons.Length; i++)
            {
                if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                {
                    activeSkeletonNumber = i + 1;
                    activeSkeleton = skeletons[i];
                    break;
                }
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                // Get the depth data

                if (depthFrame == null) return;

                if (depthData == null)
                    depthData = new DepthImagePixel[depthFrame.Width * depthFrame.Height];

                depthFrame.CopyDepthImagePixelDataTo(depthData);

                // Create the mask from the background image

                if (activeSkeletonNumber != 0)
                {
                    ColorImagePoint[] Points = new ColorImagePoint[depthFrame.Width * depthFrame.Height];
                    kinect.CoordinateMapper.MapDepthFrameToColorFrame(DepthImageFormat.Resolution320x240Fps30, depthData, ColorImageFormat.RgbResolution640x480Fps30,Points);               
                }

            }

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch batch = myGame.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            batch.Draw(kinectVideoTexture, new Rectangle(0, 0, myGame.GraphicsDevice.PresentationParameters.BackBufferWidth, myGame.GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            batch.End();
            
            base.Draw(gameTime);
        }
    }
}
