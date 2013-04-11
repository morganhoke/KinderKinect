using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KinderKinect.ButterflyGarden
{
   
        /// <summary>
        /// Creates single color textures (i.e. for drawing rectangles).
    /// Adapted from http://www.xnawiki.com/index.php/Single-Color_Texture
        /// </summary>
        class SingleColorTextureCreator
        {
            

            /// <summary>
            /// Creates a 1x1 pixel texture of the specified color.
            /// </summary>
            /// <param name="graphicsDevice">The graphics device to use.</param>
            /// <param name="color">The color to set the texture to.</param>
            /// <returns>The newly created texture.</returns>
            public static Texture2D Create(GraphicsDevice graphicsDevice, Butterfly.ButterflyColors color)
            {
                return Create(graphicsDevice, 1, 1, color);
            }

            /// <summary>
            /// Creates a texture of the specified color.
            /// </summary>
            /// <param name="graphicsDevice">The graphics device to use.</param>
            /// <param name="width">The width of the texture.</param>
            /// <param name="height">The height of the texture.</param>
            /// <param name="color">The color to set the texture to.</param>
            /// <returns>The newly created texture.</returns>
            public static Texture2D Create(GraphicsDevice graphicsDevice, int width, int height, Butterfly.ButterflyColors color)
            {
                // create the rectangle texture without colors
                Texture2D texture = new Texture2D(
                    graphicsDevice,
                    width,
                    height,
                    false,
                    SurfaceFormat.Color);

                // Create a color array for the pixels
                Color[] colors = new Color[width * height];
                for (int i = 0; i < colors.Length; i++)
                {
                    switch (color)
                    {
                        case Butterfly.ButterflyColors.Black:
                            colors[i] = Color.Black;
                            break;
                        case Butterfly.ButterflyColors.Blue:
                            colors[i] = Color.Blue;
                            break;
                        case Butterfly.ButterflyColors.Brown:
                            colors[i] = Color.SaddleBrown;
                            break;
                        case Butterfly.ButterflyColors.Green:
                            colors[i] = Color.LawnGreen;
                            break;
                        case Butterfly.ButterflyColors.Orange:
                            colors[i] = Color.DarkOrange;
                            break;
                        case Butterfly.ButterflyColors.Pink:
                            colors[i] = Color.Orchid;
                            break;
                        case Butterfly.ButterflyColors.Purple:
                            colors[i] = Color.Purple;
                            break;
                        case Butterfly.ButterflyColors.Red:
                            colors[i] = Color.Red;
                            break;
                        case Butterfly.ButterflyColors.White:
                            colors[i] = Color.GhostWhite;
                            break;
                        case Butterfly.ButterflyColors.Yellow:
                            colors[i] = Color.Yellow;
                            break;
                    }
                }

                // Set the color data for the texture
                texture.SetData(colors);

                return texture;
            }
        }
    }
