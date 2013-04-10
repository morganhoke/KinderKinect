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
        ButterflyPlayer player;
        List<Butterfly> butterflies;
        Camera myCam;
        Butterfly.ButterflyColors solution;
        
        private readonly Vector3[] positions = {   new Vector3(0f, 2f, 0) * 2,
                                                   new Vector3(1f / 2f, (float)Math.Sqrt(3) / 2f + 1f, 0f) * 2,
                                                   new Vector3((float)Math.Sqrt(3) / 2f, 1.5f, 0f) * 2,
                                                   new Vector3(1f, 1f, 0f) * 2,
                                                   new Vector3((float)Math.Sqrt(3) / 2f, .5f, 0f) * 2,
                                                   new Vector3(1f/2f, (-1*(float)Math.Sqrt(3) / 2f) +1, 0f) * 2,
                                                   new Vector3(0f, 0f, 0f) * 2,
                                                   new Vector3(-1f/2f, (-1*(float)Math.Sqrt(3) / 2f) +1, 0f) * 2,
                                                   new Vector3((-1*(float)Math.Sqrt(3) / 2f) , 1f/2f,0f) * 2,
                                                   new Vector3(-1, 1, 0) * 2,
                                                   new Vector3((-1*(float)Math.Sqrt(3) / 2f), 1.5f,0f) * 2,
                                                   new Vector3(-1f/2f,((float)Math.Sqrt(3) / 2f) + 1, 0f) * 2,
                                               };

        public delegate void LevelFinishedEventHandler(object sender, EventArgs e);
        public event LevelFinishedEventHandler Completed;

        public ButterflyLevel(Camera Camera, ButterflyPlayer Player)
        {
            myCam = Camera;
            player = Player;
            butterflies = new List<Butterfly>();
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
            }

            var usedColors = from b in butterflies select b.Color;
            solution = (Butterfly.ButterflyColors)usedColors.ElementAt(random.Next(usedColors.Count()));

        }

        void ButterflySelected(object sender, EventArgs e)
        {
            if ((sender as Butterfly).Color.Equals(solution))
            {
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

        public void Update()
        {
            foreach (Butterfly b in butterflies)
            {
                b.HitBox.update(player.Hands);
            }
        }

        public void Draw(GraphicsDevice device)
        {
            BlendState restore1 = device.BlendState;
            DepthStencilState restore2 = device.DepthStencilState;

            foreach (Butterfly b in butterflies)
            {
                b.Draw(myCam);
            }
            player.Draw(myCam);
            

            device.BlendState = restore1;
            device.DepthStencilState = restore2;
        }
    }
}
