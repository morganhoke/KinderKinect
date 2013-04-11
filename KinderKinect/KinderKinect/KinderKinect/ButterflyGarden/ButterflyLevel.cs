using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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

        bool LevelStarting; 


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

        public ButterflyLevel(Camera Camera, ButterflyPlayer Player)
        {
            myCam = Camera;
            player = Player;
            butterflies = new List<Butterfly>();
            LevelStarting = true;
        }

        public void LoadContent(ContentManager content, Viewport view)
        {
            Array values = Enum.GetValues(typeof(Butterfly.ButterflyColors));
            Random random = new Random();
            for (int i = 0; i < positions.Length; i ++)
            {
                butterflies.Add(new Butterfly(positions[i], 0,view, myCam.ViewMatrix, myCam.ProjectionMatrix,  (Butterfly.ButterflyColors)values.GetValue(random.Next(values.Length))));
                butterflies[i].Selected += new Butterfly.ButterflySelectedEventHandler(ButterflySelected);
                butterflies[i].LoadContent(content);
                butterflies[i].Selectable = true;
                butterflies[i].setPosition(butterflies[i].getPosition() * 20);
            }
            Random rand = new Random();
            var usedColors = from b in butterflies select b.Color;
            solutionColor = (Butterfly.ButterflyColors)usedColors.ElementAt(random.Next(usedColors.Count()));
            int numSolution = usedColors.Count(c => c == solutionColor );
            solutionCount = rand.Next(1, numSolution);
            font = content.Load<SpriteFont>("SpriteFont1");
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
            b.Hide();
        }

        void RightChoice(Butterfly b)
        {
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

                sb.DrawString(font, output1, new Vector2(widthOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output1, new Vector2(widthOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Yellow);
                switch (solutionColor)
                {
                    case Butterfly.ButterflyColors.Black:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.White);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        break;
                    case Butterfly.ButterflyColors.Blue:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Blue);
                        break;
                    case Butterfly.ButterflyColors.Brown:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Brown);
                        break;
                    case Butterfly.ButterflyColors.Green:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Green);
                        break;
                    case Butterfly.ButterflyColors.Orange:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Orange);
                        break;
                    case Butterfly.ButterflyColors.Pink:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.DeepPink);
                        break;
                    case Butterfly.ButterflyColors.Purple:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Purple);
                        break;
                    case Butterfly.ButterflyColors.Red:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Red);
                        break;
                    case Butterfly.ButterflyColors.White:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.White);
                        break;
                    case Butterfly.ButterflyColors.Yellow:
                        sb.DrawString(font, Color, new Vector2(colorOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                        sb.DrawString(font, Color, new Vector2(colorOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Yellow);
                        break;
                }

                sb.DrawString(font, output2, new Vector2(finalOffset - 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset + 2, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset, (device.Viewport.Height / 32) * 30 - 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset, (device.Viewport.Height / 32) * 30 + 2), Microsoft.Xna.Framework.Color.Black);
                sb.DrawString(font, output2, new Vector2(finalOffset, (device.Viewport.Height / 32) * 30), Microsoft.Xna.Framework.Color.Yellow);

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
