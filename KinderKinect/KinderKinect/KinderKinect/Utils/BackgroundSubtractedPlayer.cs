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

            // Found this online at http://www.imaginativeuniversal.com/blog/post/2012/03/15/The-right-way-to-do-Background-Subtraction-with-the-Kinect-SDK-v1.aspx
            using (var depthFrame = e.OpenDepthImageFrame())
            using (var colorFrame = e.OpenColorImageFrame())
            {
                if (depthFrame != null && colorFrame != null && activeSkeletonNumber != 0)
                {
                    var depthBits = new DepthImagePixel[depthFrame.PixelDataLength];
                    depthFrame.CopyDepthImagePixelDataTo(depthBits);

                    var colorBits = new byte[colorFrame.PixelDataLength];
                    colorFrame.CopyPixelDataTo(colorBits);
                    int colorStride = colorFrame.BytesPerPixel * colorFrame.Width;

                    byte[] output = new byte[depthFrame.Width * depthFrame.Height * colorFrame.BytesPerPixel];

                    int outputIndex = 0;

                    var colorCoordinates = new ColorImagePoint[depthFrame.PixelDataLength];
                    kinect.CoordinateMapper.MapDepthFrameToColorFrame(depthFrame.Format, depthBits, colorFrame.Format, colorCoordinates);

                    for (int depthIndex = 0;  depthIndex < depthBits.Length; depthIndex++, outputIndex += colorFrame.BytesPerPixel)
                    {
                        var playerIndex = depthBits[depthIndex].PlayerIndex;

                        var colorPoint = colorCoordinates[depthIndex];

                        var colorPixelIndex = (colorPoint.X * colorFrame.BytesPerPixel) + (colorPoint.Y * colorStride);

                        output[outputIndex + 2] = colorBits[colorPixelIndex];
                        output[outputIndex + 1] = colorBits[colorPixelIndex + 1];
                        output[outputIndex + 0] = colorBits[colorPixelIndex + 2];
                        output[outputIndex + 3] = playerIndex == activeSkeletonNumber ? (byte)255 : (byte)0;

                    }
                    kinectVideoTexture = new Texture2D(myGame.GraphicsDevice,  depthFrame.Width, depthFrame.Height, false, SurfaceFormat.Color);
                    kinectVideoTexture.SetData(output);

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
            //This is temp draw code, to be replaced by some nice 3d code
            SpriteBatch batch = myGame.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
            batch.Draw(kinectVideoTexture, new Rectangle(0, 0, myGame.GraphicsDevice.PresentationParameters.BackBufferWidth, myGame.GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            batch.End();
            
            base.Draw(gameTime);
        }
    }
}
