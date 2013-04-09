using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyPlayer
    {
        List<ICursor> hands;
        ButterflyPlayer(Game1 myGame)
        {
            hands = new List<ICursor>();
            hands.Add(new KinectRelativeScreenspaceCursor(myGame.Services.GetService(typeof(KinectService)) as KinectService, KinectAbsoluteScreenspaceCursor.Handedness.Left));
            hands.Add(new KinectRelativeScreenspaceCursor(myGame.Services.GetService(typeof(KinectService)) as KinectService, KinectAbsoluteScreenspaceCursor.Handedness.Right));
        }

        public void Update()
        {
        }

        public void Draw()
        {
        }
    }
}
