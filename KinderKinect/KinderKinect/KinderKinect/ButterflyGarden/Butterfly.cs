using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using KinderKinect.Utils;

namespace KinderKinect.ButterflyGarden
{
    class Butterfly
    {
        private const int padX = 24, padY = 24, selectionMilis = 500;
        private Stopwatch timeSelected;
        public delegate void ButterflySelectedEventHandler(object sender, EventArgs e);
        public event ButterflySelectedEventHandler Selected;
        private bool hidden;

        /// <summary>
        /// I do this horrible aweful hack to simplify my content loading/unloading job
        /// </summary>
        public static Texture2D[] ButterflyTextures;

        private Matrix[] transforms;
        private Matrix World;
        private Model myModel;

        private ButterflyColors myColor;
        public ButterflyColors Color
        {
            get
            {
                return myColor;
            }
            set
            {
                myColor = value;
            }
        }

        public enum ButterflyColors
        {
            Red,
            Yellow,
            Blue,
            Green,
            Purple,
            Black,
            White,
            Brown,
            Orange,
            Pink
        }

        private Hitbox hitbox;
        public Hitbox HitBox
        {
            get
            {
                return hitbox;
            }
        }



        public Butterfly(Vector3 Position, float rotation, Viewport viewPort, Matrix View, Matrix Projection, ButterflyColors color)
        {

            World = Matrix.CreateFromAxisAngle(Vector3.Up, rotation) * Matrix.CreateTranslation(Position);
            Vector3 ScreenProjection = viewPort.Project(Position, Projection, View, Matrix.CreateScale(0.1f) * World);
            hitbox = new Hitbox(new Rectangle((int)(ScreenProjection.X - padX), (int)(ScreenProjection.Y - padY), 2 * padX, 2 * padY));
            hitbox.Entered += new Hitbox.EnteredEventHandler(hitbox_Entered);
            hitbox.Exited += new Hitbox.ExitedEventHandler(hitbox_Exited);
            timeSelected = new Stopwatch();
            myColor = color;
            hidden = false;

        }

        public void Hide()
        {
            hidden = true;
        }

        public void Show()
        {
            hidden = false;
        }

        public void LoadContent(ContentManager content)
        {
            myModel = content.Load<Model>(@"Models\butterfly");
        }

        void hitbox_Exited(object sender, EventArgs e)
        {
            timeSelected.Stop();
            timeSelected.Reset();
        }

        void hitbox_Entered(object sender, EventArgs e)
        {
            timeSelected.Reset();
            timeSelected.Start();
        }

        public void Update()
        {
            if (timeSelected.ElapsedMilliseconds >= selectionMilis)
            {
                Selected(this, new EventArgs());
            }
        }

        public void Draw(Camera myCam)
        {
            if (!hidden)
            {
              //  myModel.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in myModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = Matrix.CreateScale(0.1f) * World;
                        effect.View = myCam.ViewMatrix;
                        effect.Projection = myCam.ProjectionMatrix;
                        effect.TextureEnabled = true;
                        effect.Texture = ButterflyTextures[(int)(myColor)];

                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }
    }
}
