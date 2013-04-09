using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyLevel
    {
        ButterflyPlayer player;
        List<Butterfly> butterflies;
        Camera myCam;

        public ButterflyLevel()
        {
        }

        public void Update()
        {
            foreach (Butterfly b in butterflies)
            {
                b.HitBox.update(player.Hands);
            }
        }

        public void Draw()
        {
        }
    }
}
