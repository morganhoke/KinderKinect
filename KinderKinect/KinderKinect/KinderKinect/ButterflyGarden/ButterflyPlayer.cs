using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Content;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyPlayer : IKinectListener
    {
        Texture2D kinectVideoTexture = null;
        Texture2D testTex;
        KinectService kinect;
        Game1 myGame;
        Model myModel;
        SpriteBatch batch;
        private Matrix[] transforms;
        private Matrix world;

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


        /// <summary>
        /// My cursors
        /// </summary>
        List<ICursor> hands;
        public List<ICursor> Hands
        {
            get
            {
                return hands;
            }
        }

        private float rotation;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public ButterflyPlayer(Game1 myGame, Vector3 Position, float Rotation, Camera cam)
        {
            hands = new List<ICursor>();
            hands.Add(new KinectRelativeScreenspaceCursor(myGame.Services.GetService(typeof(KinectService)) as KinectService, KinectAbsoluteScreenspaceCursor.Handedness.Left, myGame.GraphicsDevice.Viewport));
            hands.Add(new KinectRelativeScreenspaceCursor(myGame.Services.GetService(typeof(KinectService)) as KinectService, KinectAbsoluteScreenspaceCursor.Handedness.Right, myGame.GraphicsDevice.Viewport));
            kinect = myGame.Services.GetService(typeof(KinectService)) as KinectService;
            batch = myGame.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            position = Position;
            rotation = Rotation;
            world = Matrix.CreateFromAxisAngle(Vector3.Up, rotation) * Matrix.CreateTranslation(Position);
            world = Matrix.CreateScale(0.1f) * world;
            (hands[0] as KinectRelativeScreenspaceCursor).SetWorld(world);
            (hands[1] as KinectRelativeScreenspaceCursor).SetWorld(world);
            (hands[0] as KinectRelativeScreenspaceCursor).SetView(cam.ViewMatrix);
            (hands[1] as KinectRelativeScreenspaceCursor).SetView(cam.ViewMatrix);
            (hands[0] as KinectRelativeScreenspaceCursor).SetView(cam.ProjectionMatrix);
            (hands[1] as KinectRelativeScreenspaceCursor).SetView(cam.ProjectionMatrix);
        }

        public void LoadContent(ContentManager content)
        {
            myModel = content.Load<Model>(@"Models\plane");
            testTex = content.Load<Texture2D>(@"Textures\DebugColor");
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

                for (int depthIndex = 0; depthIndex < depthBits.Length; depthIndex++, outputIndex += kinect.ColorFrame.BytesPerPixel)
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
        public void Update()
        {
            ProcessFrame();
            foreach (ICursor c in hands)
            {
                c.Update();
            }
        }

        public void Draw(Camera cam)
        {
           

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = cam.ViewMatrix;
                    effect.Projection = cam.ProjectionMatrix;
                    effect.Texture = kinectVideoTexture;
                    effect.TextureEnabled = true;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
