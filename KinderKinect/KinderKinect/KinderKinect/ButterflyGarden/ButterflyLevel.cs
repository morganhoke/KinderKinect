using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using DPSF;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyLevel
    {

        double translationMs = 1000;
        double elapsedtime = 0;
        ButterflyPlayer player;
        List<Butterfly> butterflies;
        Camera myCam;
        Butterfly.ButterflyColors solutionColor;
        int solutionCount;

        SpriteFont font;
        SpriteFont font2;

        AudioEngine engine;
        WaveBank waveBank;
        SoundBank soundBank;
       
        Logger errorLogger;

        bool LevelStarting;

        int tier = 0;

        int mistakeCount = 0;

        private readonly Vector3[] positions = {  // new Vector3(0f, 1f, 0) * 3,
                                                   new Vector3(.75f, (float)Math.Sqrt(3) / 2f , 0f) * 4,
                                                   new Vector3((float)Math.Sqrt(3) / 2f + .25f, .5f, 0f) * 4,
                                                   new Vector3(1.25f, 0f, 0f) * 4,
                                                   new Vector3((float)Math.Sqrt(3) / 2f + .25f, -.5f, 0f) * 4,
                                                   new Vector3(.75f, (-1*(float)Math.Sqrt(3) / 2f) , 0f) * 4,
                                                   //new Vector3(0f, 0f, 0f) * 2,
                                                   new Vector3(-.75f, (-1*(float)Math.Sqrt(3) / 2f) , 0f) * 4,
                                                   new Vector3((-1*(float)Math.Sqrt(3) / 2f) -.25f , -1f/2f,0f) * 4,
                                                   new Vector3(-1.25f, 0, 0) * 4,
                                                   new Vector3((-1*(float)Math.Sqrt(3) / 2f) -.25f, .5f,0f) * 4,
                                                   new Vector3(-.75f,((float)Math.Sqrt(3) / 2f) , 0f) * 4,
                                               };

        public delegate void LevelFinishedEventHandler(object sender, EventArgs e);
        public event LevelFinishedEventHandler Completed;

        List<Tuple<Vector3, int>> spriteQueue;
        List<int> queueTimers;
        List<string> scoreStrings;

        PlayerProfile playerProfile;

        Texture2D Feedback;
        Texture2D background;

        Game1 myGame;

        public ButterflyLevel(Camera Camera, ButterflyPlayer Player, Logger ErrorLogger, Game1 MyGame)
        {
            myCam = Camera;
            player = Player;
            butterflies = new List<Butterfly>();
            LevelStarting = true;
            errorLogger = ErrorLogger;
            myGame = MyGame;
            spriteQueue = new List<Tuple<Vector3, int>>();
            queueTimers = new List<int>();
            scoreStrings = new List<string>();
            playerProfile = myGame.Services.GetService(typeof(PlayerProfile)) as PlayerProfile;
            
        }

        public void LoadContent(ContentManager content, Viewport view)
        {
            Array values = Enum.GetValues(typeof(Butterfly.ButterflyColors));
            Random random = new Random();
            Feedback = content.Load<Texture2D>("Textures\\Feedback");
            
            for (int i = 0; i < positions.Length; i ++)
            {
                butterflies.Add(new Butterfly(positions[i], 0,view, myCam.ViewMatrix, myCam.ProjectionMatrix,  (Butterfly.ButterflyColors)values.GetValue(random.Next(values.Length))));
                butterflies[i].Selected += new Butterfly.ButterflySelectedEventHandler(ButterflySelected);
                butterflies[i].LoadContent(content);
                butterflies[i].Selectable = true;
                butterflies[i].setPosition(butterflies[i].getPosition() * 20);
            }
            Random rand = new Random();
            var usedColors = from b in butterflies where b.GetHidden() == false select b.Color;
            solutionColor = (Butterfly.ButterflyColors)usedColors.ElementAt(random.Next(usedColors.Count()));
            int numSolution = butterflies.Count(c => c.Color == solutionColor && c.GetHidden() == false );
            solutionCount = rand.Next(1, numSolution);
            font = content.Load<SpriteFont>("SpriteFont1");
            font2 = content.Load<SpriteFont>("SpriteFont2");
            engine = new AudioEngine("Content\\Audio\\ButterflyAudio.xgs");
            waveBank = new WaveBank(engine, "Content\\Audio\\Waves.xwb");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sounds.xsb");
            background = content.Load<Texture2D>("Textures\\ButterflyGarden\\forestCrap");
            
            
        }

        public bool tryNewTier()
        {
            var visibleButterflies = from b in butterflies where b.GetHidden() == false select b;
            var avaliableColors = from b in visibleButterflies select b.Color;
            if (mistakeCount >= visibleButterflies.Count()/4)
            {
                return false;
            }
            else if (avaliableColors.Count() <= 3 || tier >= 5)
            {
                return false;
            }
            else
            {
                mistakeCount = 0;
                Random rand = new Random();
                var usedColors = from b in butterflies where b.GetHidden() == false select b.Color;
                solutionColor = (Butterfly.ButterflyColors)usedColors.ElementAt(rand.Next(usedColors.Count()));
                int numSolution = butterflies.Count(c => c.Color == solutionColor && c.GetHidden() == false);
                solutionCount = rand.Next(1, numSolution);
                tier++;
                return true;
            }

        }

        void ButterflySelected(object sender, EventArgs e)
        {
            if ((sender as Butterfly).Color.Equals(solutionColor))
            {
                solutionCount--;
                RightChoice(sender as Butterfly);
                if(Completed != null && solutionCount == 0)
                Completed(this, new EventArgs());
            }
            else
            {
                WrongChoice(sender as Butterfly);
            }
        }

        void WrongChoice(Butterfly b)
        {
            soundBank.PlayCue("73581__benboncan__sad-trombone");
            spriteQueue.Add(new Tuple<Vector3,int>(b.getPosition(), 1));
            queueTimers.Add(30);
            scoreStrings.Add("x 0");
            b.Hide();
            mistakeCount++;
        }

        void RightChoice(Butterfly b)
        {
            spriteQueue.Add(new Tuple<Vector3, int>(b.getPosition(), 0));
            queueTimers.Add(30);
            soundBank.PlayCue("145459__soughtaftersounds__menu-click-sparkle");
            int score = playerProfile.Score;
            score += Math.Max(1, tier);
            playerProfile.Score = score;
            scoreStrings.Add(String.Format("x {0}", Math.Max(1, tier)));
            b.Hide();
        }

        public void Update(GameTime gameTime)
        {

            player.Update();
            if (!LevelStarting)
            {
                foreach (Butterfly b in butterflies)
                {
                    if (!b.GetHidden())
                    {
                        b.setPosition(ButterflyAI.AIGetNewPosition(b, tier, new Vector3(b.TetherPoint, 0)));
                        b.Update(gameTime);
                        if (b.Selectable)
                            b.HitBox.update(player.Hands);
                    }
                }
            }
            else
            {
                elapsedtime += gameTime.ElapsedGameTime.TotalMilliseconds;
                for (int i = 0; i < positions.Length; i++)
                {
                    butterflies[i].setPosition(Vector3.Lerp(positions[i] * 5, positions[i], (float)Math.Min(1, elapsedtime/translationMs)));
                    butterflies[i].Update(gameTime);
                }
                if (butterflies[0].getPosition().X == positions[0].X)
                {
                    LevelStarting = false;
                }
            }
        }

        public void Draw(GraphicsDevice device, SpriteBatch sb)
        {
            BlendState restore1 = device.BlendState;
            DepthStencilState restore2 = device.DepthStencilState;

            sb.Begin();
            sb.Draw(background, new Rectangle(0, 0, myGame.GraphicsDevice.Viewport.Width, myGame.GraphicsDevice.Viewport.Height), Microsoft.Xna.Framework.Color.White) ;
            sb.End();

            device.BlendState = restore1;
            device.DepthStencilState = restore2;

            device.BlendState = BlendState.Opaque;
            player.Draw(myCam, sb);
            
            foreach (Butterfly b in butterflies)
            {
                device.BlendState = BlendState.Opaque;
                
                b.Draw(myCam, sb);
            }

            if (!LevelStarting)
            {
               
                string output1 = string.Format("Catch {0} ", solutionCount);
                string Color = string.Format("{0} ", solutionColor);
                Color = Color.ToUpper();
                string output2;
                if (solutionCount == 1)
                    output2 = "butterfly.";
                else
                    output2 = "butterflies.";

                Vector2 totalStringSize = font.MeasureString(output1 + Color + output2);
                int widthOffset = device.Viewport.Width / 2 - (int)(totalStringSize.X) / 2;
                int colorOffset = widthOffset + (int)(font.MeasureString(output1).X);
                int finalOffset = colorOffset + (int)(font.MeasureString(Color).X);
                sb.Begin();

                for (int i = 0; i < spriteQueue.Count; i++)
                {
                    sb.Draw(Feedback, new Vector2(myGame.GraphicsDevice.Viewport.Project(spriteQueue[i].Item1, myCam.ProjectionMatrix, myCam.ViewMatrix, Matrix.Identity).X,myGame.GraphicsDevice.Viewport.Project(spriteQueue[i].Item1, myCam.ProjectionMatrix, myCam.ViewMatrix, Matrix.Identity).Y) ,new Rectangle( spriteQueue[i].Item2 * 48, 0, 48, 48), Microsoft.Xna.Framework.Color.White);
                    queueTimers[i]--;
                    sb.DrawString(font2, scoreStrings[i], new Vector2(myGame.GraphicsDevice.Viewport.Project(spriteQueue[i].Item1, myCam.ProjectionMatrix, myCam.ViewMatrix, Matrix.Identity).X + 48, myGame.GraphicsDevice.Viewport.Project(spriteQueue[i].Item1, myCam.ProjectionMatrix, myCam.ViewMatrix, Matrix.Identity).Y), Microsoft.Xna.Framework.Color.White);
                                      
                }

                for (int i = 0; i < queueTimers.Count; i++)
                {
                    if (queueTimers[i] <= 0)
                    {
                        spriteQueue.RemoveAt(i);
                        queueTimers.RemoveAt(i);
                        scoreStrings.RemoveAt(i);
                        break;
                    }
                }


                sb.DrawString(font, output1, new Vector2(widthOffset - 2, (device.Viewport.Height / 32) * 27 ), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Yellow);
                switch (solutionColor)
                {
                    case Butterfly.ButterflyColors.Black:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        break;
                    case Butterfly.ButterflyColors.Blue:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Blue);
                        break;
                    case Butterfly.ButterflyColors.Brown:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.SaddleBrown);
                        break;
                    case Butterfly.ButterflyColors.Green:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.LawnGreen);
                        break;
                    case Butterfly.ButterflyColors.Orange:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.DarkOrange);
                        break;
                    case Butterfly.ButterflyColors.Pink:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Orchid);
                        break;
                    case Butterfly.ButterflyColors.Purple:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Purple);
                        break;
                    case Butterfly.ButterflyColors.Red:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Red);
                        break;
                    case Butterfly.ButterflyColors.White:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.GhostWhite);
                        break;
                    case Butterfly.ButterflyColors.Yellow:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Yellow);
                        break;
                }

                sb.DrawString(font, output2, new Vector2(finalOffset - 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset + 2, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset, (device.Viewport.Height / 32) * 27 - 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset, (device.Viewport.Height / 32) * 27 + 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset, (device.Viewport.Height / 32) * 27), Microsoft.Xna.Framework.Color.Yellow);

                sb.Draw(Feedback, new Vector2(device.Viewport.Width/6f * 5f, device.Viewport.Height / 32 * 27), new Rectangle(0, 0, 48, 48), Microsoft.Xna.Framework.Color.White);
                sb.DrawString(font2, String.Format(" = {0}", playerProfile.Score), new Vector2(device.Viewport.Width / 6f * 5f + 48, device.Viewport.Height / 32 * 27), Microsoft.Xna.Framework.Color.White);

                //For debugging 
                //sb.Draw(Butterfly.ButterflyTextures[0], new Rectangle((int)(player.Hands[0].Position.X - 24), (int)(player.Hands[0].Position.Y - 24), 48, 48), Microsoft.Xna.Framework.Color.White);
                //sb.Draw(Butterfly.ButterflyTextures[0], new Rectangle((int)(player.Hands[01].Position.X - 24), (int)(player.Hands[1].Position.Y - 24), 48, 48), Microsoft.Xna.Framework.Color.White);
                sb.End();
            }
            device.BlendState = restore1;
            device.DepthStencilState = restore2;
            
        }
    }
}
