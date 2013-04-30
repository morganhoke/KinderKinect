using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using KinderKinect.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyActivity : IActivity
    {
        ContentManager content;
        Camera myCamera;
        ButterflyLevel currentLevel;
        ButterflyPlayer player;
        Game1 myGame;

        AudioEngine engine;
        WaveBank waveBank;
        SoundBank soundBank;

        Cue BackgroundMusic;
        Cue Birdsong;

        //For debugging and Science!
        SpriteBatch sb;
        Texture2D debugTex;

        Logger errorLogger;

        int counter;

        public ButterflyActivity(Game1 MyGame)
        {
            myGame = MyGame;
            content = new ContentManager(myGame.Services, "Content");
            myCamera = new Camera();
            float aspectRatio = (float)myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)myGame.GraphicsDevice.PresentationParameters.BackBufferHeight;
            myCamera.Perspective(90, myGame.GraphicsDevice.Viewport.AspectRatio, 0.1f, 20f);
           
            myCamera.Position = new Vector3(0f, 1f, -10f);
            myCamera.LookAt(new Vector3(0, 0, 0));
            player = new ButterflyPlayer(myGame, new Vector3(0f, 2f, 0) * 2, 0f, myCamera);
            errorLogger = myGame.Services.GetService(typeof(Logger)) as Logger;
            counter = -1;
            
        }

        public void Initalize()
        {
            currentLevel = new ButterflyLevel(myCamera, player, errorLogger, myGame);
            currentLevel.Completed += new ButterflyLevel.LevelFinishedEventHandler(currentLevel_Completed);
        }

        void currentLevel_Completed(object sender, EventArgs e)
        {
            if (!currentLevel.tryNewTier())
            {
                counter = 30;
            }
        }

        public void LoadContent()
        {
            engine = new AudioEngine("Content\\Audio\\ButterflyAudio.xgs");
            waveBank = new WaveBank(engine, "Content\\Audio\\Waves.xwb");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sounds.xsb");
            BackgroundMusic = soundBank.GetCue("478503_PlayfulStrollJoyfulOboeMorganCut");
            Birdsong = soundBank.GetCue("157536__juskiddink__woodland-birdsong-june");

            BackgroundMusic.Play();
            Birdsong.Play();

            Butterfly.ButterflyTextures = new Texture2D[Enum.GetNames(typeof(Butterfly.ButterflyColors)).Length];
            for (int i = 0; i < Butterfly.ButterflyTextures.Length; i++)
            {
                Butterfly.ButterflyTextures[i] = SingleColorTextureCreator.Create(myGame.GraphicsDevice, 256, 256, (Butterfly.ButterflyColors)(i));
            }
            player.LoadContent(content);
            currentLevel.LoadContent(content, myGame.GraphicsDevice.Viewport);

            //debug code
            sb = myGame.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        }

        public void Update(GameTime gameTime)
        {
            if (counter > -1)
            {
                counter--;
            }
            if (counter == 0)
            {
                counter = -1;
                currentLevel = new ButterflyLevel(myCamera, player, errorLogger, myGame);
                currentLevel.LoadContent(content, myGame.GraphicsDevice.Viewport);
                currentLevel.Completed += new ButterflyLevel.LevelFinishedEventHandler(currentLevel_Completed);
                return;
            }

            currentLevel.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            currentLevel.Draw(myGame.GraphicsDevice, sb);
        }

        public void Unload()
        {
            content.Unload();
        }
    }
}
