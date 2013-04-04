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
    public class BackgroundSubtractedPlayer : Microsoft.Xna.Framework.DrawableGameComponent, IKinectListener
    {
        Texture2D kinectVideoTexture = null;
        KinectService kinect;
        Game1 myGame;
        Model myModel;
        
        /// <summary>
        /// The player's position in the scene
        /// </summary>
        Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }


        bool _newDataReady = false;

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
            kinect = myGame.Services.GetService(typeof(KinectService)) as KinectService;
            kinectVideoTexture = new Texture2D(myGame.GraphicsDevice, 640, 480);
            kinect.RegisterKinectListener(this);
            myModel = myGame.Content.Load<Model>("Models/simplePlane");
            base.LoadContent();
        }

        void ProcessFrame()
        {
            

            // Found this online at http://www.imaginativeuniversal.com/blog/post/2012/03/15/The-right-way-to-do-Background-Subtraction-with-the-Kinect-SDK-v1.aspx
                if (kinect.DepthFrame != null && kinect.ColorFrame != null && kinect.ActiveSkeletonNumber != 0 && _newDataReady)
                {
                    var depthBits = new DepthImagePixel[kinect.DepthFrame.PixelDataLength];
                    kinect.DepthFrame.CopyDepthImagePixelDataTo(depthBits);

                    var colorBits = new byte[kinect.ColorFrame.PixelDataLength];
                    kinect.ColorFrame.CopyPixelDataTo(colorBits);
                    int colorStride = kinect.ColorFrame.BytesPerPixel * kinect.ColorFrame.Width;

                    byte[] output = new byte[kinect.DepthFrame.Width * kinect.DepthFrame.Height * kinect.ColorFrame.BytesPerPixel];

                    int outputIndex = 0;

                    var colorCoordinates = new ColorImagePoint[kinect.DepthFrame.PixelDataLength];
                    kinect.Kinect.CoordinateMapper.MapDepthFrameToColorFrame(kinect.DepthFrame.Format, depthBits, kinect.ColorFrame.Format, colorCoordinates);

                    for (int depthIndex = 0;  depthIndex < depthBits.Length; depthIndex++, outputIndex += kinect.ColorFrame.BytesPerPixel)
                    {
                        var playerIndex = depthBits[depthIndex].PlayerIndex;

                        var colorPoint = colorCoordinates[depthIndex];

                        var colorPixelIndex = (colorPoint.X * kinect.ColorFrame.BytesPerPixel) + (colorPoint.Y * colorStride);

                        output[outputIndex + 2] = colorBits[colorPixelIndex];
                        output[outputIndex + 1] = colorBits[colorPixelIndex + 1];
                        output[outputIndex + 0] = colorBits[colorPixelIndex + 2];
                        output[outputIndex + 3] = playerIndex == kinect.ActiveSkeletonNumber ? (byte)255 : (byte)0;

                    }

                        kinectVideoTexture = new Texture2D(myGame.GraphicsDevice, kinect.DepthFrame.Width, kinect.DepthFrame.Height, false, SurfaceFormat.Color);
                        kinectVideoTexture.SetData(output);
                        _newDataReady = false;

                }

            
 

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            ProcessFrame();
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

        public bool NewKinectDataReady
        {
            get
            {
                return _newDataReady;
            }
            set
            {
                _newDataReady = value;
            }
        }
    }
}
