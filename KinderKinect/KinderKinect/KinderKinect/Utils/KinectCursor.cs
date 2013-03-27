using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;


namespace KinderKinect.Utils
{
    class KinectCursor : ICursor
    {

        public enum Handedness
        {
            Left,
            Right
        };


        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
        }

        private Handedness handSelect;
        private KinectSensor kinect;
        private int activeSkeletonNumber;
        private Skeleton[] skeletons = null;
        private Skeleton activeSkeleton = null;
        private Game1 myGame;


        public KinectCursor(Game1 MyGame, Handedness hand)
        {
            myGame = MyGame;
            kinect = (MyGame).Services.GetService(typeof(KinectSensor)) as KinectSensor;
            handSelect = hand;
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
        }

        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }
            activeSkeletonNumber = 0;

            for (int i = 0; i < skeletons.Length; i++)
            {
                if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                {
                    activeSkeletonNumber = i + 1;
                    activeSkeleton = skeletons[i];
                    break;
                }
            }
        }

        public void Update()
        {
            if (activeSkeletonNumber != 0)
            {
                Joint hand = new Joint();
                if (handSelect == Handedness.Left)
                {
                    hand = activeSkeleton.Joints[JointType.HandLeft];
                }
                else
                {
                    hand = activeSkeleton.Joints[JointType.HandRight];
                }

                ColorImagePoint handPoint = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(hand.Position, ColorImageFormat.RgbResolution640x480Fps30);
                _position.X = handPoint.X * myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / 640; // scales up to whatever resolution I like
                _position.Y = handPoint.Y * myGame.GraphicsDevice.PresentationParameters.BackBufferHeight / 480; 
            }
        }
    }
}
