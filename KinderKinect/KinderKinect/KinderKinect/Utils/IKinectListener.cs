using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinderKinect.Utils
{
    interface IKinectListener
    {
        /// <summary>
        /// This is how I'll asynchronously get the kinect data out to listening processes
        /// </summary>
        Action NewKinectDataReady
        {
            get;
        }
    }
}
