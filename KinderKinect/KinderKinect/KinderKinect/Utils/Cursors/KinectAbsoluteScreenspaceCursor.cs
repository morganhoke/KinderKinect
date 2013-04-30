using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace KinderKinect.Utils
{
    class KinectAbsoluteScreenspaceCursor : ICursor, IKinectListener
    {
        protected bool _newDataReady = false;
        public enum Handedness
        {
            Left,
            Right
        };


        Rectangle bodySpace;

        protected Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
        }

        protected Rectangle _rect;
        public Rectangle Rectangle
        {
            get
            {
                return _rect;
            }
        }

        protected bool _valid;
        public bool Valid
        {
            get
            {
                return _valid;
            }
        }

        protected Handedness handSelect;
        protected KinectService kinect;
        protected Game1 myGame;


        public KinectAbsoluteScreenspaceCursor(KinectService Kinect, Handedness hand, Game1 MyGame)
        {
            myGame = MyGame;
            kinect = Kinect;
            handSelect = hand;
            kinect.RegisterKinectListener(this);
            _rect = new Rectangle();
            _rect.Width = 48;
            _rect.Height = 48;
            _position = new Vector2();
            bodySpace = new Rectangle();
        }

        public virtual void Update()
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

                
                DepthImagePoint handPoint = kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position, kinect.Kinect.DepthStream.Format);
                _position.X = handPoint.X * myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / kinect.Kinect.DepthStream.FrameWidth; // scales up to whatever resolution I like
                _position.Y = handPoint.Y * myGame.GraphicsDevice.PresentationParameters.BackBufferHeight / kinect.Kinect.DepthStream.FrameHeight;
                _rect.X = (int)(_position.X - 24);
                _rect.Y = (int)(_position.Y - 24);
                _newDataReady = false;
            
                //Creates a deadzone centered at the user's chest, needs more investigation
                bodySpace.X =  kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.ShoulderLeft].Position, kinect.Kinect.DepthStream.Format).X * myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / kinect.Kinect.DepthStream.FrameWidth - 10;
                bodySpace.Y = kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.ShoulderLeft].Position, kinect.Kinect.DepthStream.Format).Y * myGame.GraphicsDevice.PresentationParameters.BackBufferHeight / kinect.Kinect.DepthStream.FrameHeight - 10;
                bodySpace.Width = kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.ShoulderRight].Position, kinect.Kinect.DepthStream.Format).X * myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / kinect.Kinect.DepthStream.FrameWidth - kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.ShoulderLeft].Position, kinect.Kinect.DepthStream.Format).X * myGame.GraphicsDevice.PresentationParameters.BackBufferWidth / kinect.Kinect.DepthStream.FrameWidth +10;
                bodySpace.Height = kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.HipLeft].Position, kinect.Kinect.DepthStream.Format).Y * myGame.GraphicsDevice.PresentationParameters.BackBufferHeight / kinect.Kinect.DepthStream.FrameHeight - kinect.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(kinect.Skeletons[kinect.ActiveSkeletonNumber - 1].Joints[JointType.ShoulderLeft].Position, kinect.Kinect.DepthStream.Format).Y * myGame.GraphicsDevice.PresentationParameters.BackBufferHeight / kinect.Kinect.DepthStream.FrameHeight +10;


                if (hand.TrackingState == JointTrackingState.Tracked && ! bodySpace.Contains((int)_position.X, (int)_position.Y))
                {
                    _valid = true;
                    
                }
                else
                {
                    _valid = false;
                }
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


        public Rectangle rect
        {
            get { return _rect; }
        }
    }
}
