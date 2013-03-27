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
using KinderKinect.Utils;
using Microsoft.Kinect;


namespace KinderKinect
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Logger errorLogger;



        DebugDrawer debugDraw;

        #region KinectSetup

        /// <summary>
        /// This all sets up a kinect, got it from ms sample code
        /// </summary>
        KinectSensor myKinect;

        protected bool setupKinect()
        {
            // Check to see if a Kinect is available
            if (KinectSensor.KinectSensors.Count == 0)
            {
                errorLogger.WriteLine("No Kinects detected");
                return false;
            }

            // Get the first Kinect on the computer
            myKinect = KinectSensor.KinectSensors[0];

            // Start the Kinect running and select all the streams
            try
            {
                myKinect.SkeletonStream.Enable();
                myKinect.ColorStream.Enable();
                myKinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                myKinect.Start();
            }
            catch
            {
                errorLogger.WriteLine("Kinect initialise failed");
                return false;
            }

            // connect a handler to the event that fires when new frames are available

            return true;
        }
        #endregion


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            debugDraw = new DebugDrawer(this);
            errorLogger = new Logger(new Uri(@"..\errorLog.txt", UriKind.Relative));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            setupKinect();

            this.Components.Add(debugDraw);
            this.Services.AddService(typeof(KinectSensor), myKinect);
            this.Services.AddService(typeof(Logger), errorLogger);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
