using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;


namespace KinderKinect.Utils
{
    class KinectAbsoluteScreenspaceCursor : ICursor, IKinectListener
    {
        bool _newDataReady = false;
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
        private KinectService kinect;
        private Game1 myGame;


        public KinectAbsoluteScreenspaceCursor(Game1 MyGame, Handedness hand)
        {
            myGame = MyGame;
            kinect = (MyGame).Services.GetService(typeof(KinectService)) as KinectService;
            handSelect = hand;
            kinect.RegisterKinectListener(this);
        }

        public void Update()
        {
            if (kinect.ActiveSkeletonNumber != 0 && _newDataReady)
            {
                Joint hand = new Joint();
                if (handSelect == Handedness.Left)
                {
                    hand = kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.HandLeft];
                }
                else
                {
                    hand = kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.HandRight];
                }

                ColorImagePoint handPoint = kinect.Kinect.CoordinateMapper.MapSkeletonPointToColorPoint(hand.Position, ColorImageFormat.RgbResolution640x480Fps30);
                _position.X = handPoint.X * myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / 640; // scales up to whatever resolution I like
                _position.Y = handPoint.Y * myGame.GraphicsDevice.PresentationParameters.BackBufferHeight / 480;
                _newDataReady = false;
            }
        }

        public bool NewKinectDataReady
        {
            get
            {
                return _newDataReady;
            }
            set
            {
                _newDataReady = value;
            }
        }


        public void BeginHoverState(TimeSpan timeToCompletion)
        {
            //throw new NotImplementedException();
        }

        public void BreakHoverState()
        {
            //throw new NotImplementedException();
        }
    }
}
