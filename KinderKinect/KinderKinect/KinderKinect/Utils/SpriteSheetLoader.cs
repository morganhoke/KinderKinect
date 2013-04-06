using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;


namespace KinderKinect.Utils
{
    /// <summary>
    /// Parse the output from Sprite Sheet Packer http://spritesheetpacker.codeplex.com/
    /// </summary>
    static class SpriteSheetLoader
    {
        static Dictionary<String, Rectangle> LoadSpriteSheetFile(string File)
        {
            Dictionary<String, Rectangle> lookup = new Dictionary<string, Rectangle>();
            // open a StreamReader to read the index
            string path = @"..\" + File;
            using (StreamReader reader = new StreamReader(path))
            {
                // while we're not done reading...
                while (!reader.EndOfStream)
                {
                    // get a line
                    string line = reader.ReadLine();

                    // split at the equals sign
                    string[] sides = line.Split('=');

                    // trim the right side and split based on spaces
                    string[] rectParts = sides[1].Trim().Split(' ');

                    // create a rectangle from those parts
                    Rectangle r = new Rectangle(
                       int.Parse(rectParts[0]),
                       int.Parse(rectParts[1]),
                       int.Parse(rectParts[2]),
                       int.Parse(rectParts[3]));

                    // add the name and rectangle to the dictionary
                    lookup.Add(sides[0].Trim(), r);
                }
            }


            return lookup;
        }
    }
}
