using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinderKinect.Utils;
using KinderKinect.ButterflyGarden;
using Microsoft.Xna.Framework;

namespace KinderKinect
{
    class ActivityManager
    {
        IActivity currentActivity;

        public ActivityManager(IActivity StartupActivity)
        {
            currentActivity = StartupActivity;
            currentActivity.Initalize();
        }

        public void LoadContent()
        {
            currentActivity.LoadContent();
        }

        public void SwitchActivities(IActivity newActivity)
        {
            currentActivity.Unload();
            currentActivity = newActivity;
            currentActivity.Initalize();
            currentActivity.LoadContent();
        }

        public void Update()
        {
            currentActivity.Update();
        }

        public void Draw(GameTime gameTime)
        {
            currentActivity.Draw(gameTime);
        }

    }
}
