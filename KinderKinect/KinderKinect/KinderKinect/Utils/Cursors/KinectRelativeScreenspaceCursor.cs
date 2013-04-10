using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Graphics;

namespace KinderKinect.Utils
{
    /// <summary>
    /// Acts as a cursor when we are intersted in the player's hands at a position in the world
    /// </summary>
    class KinectRelativeScreenspaceCursor : KinectAbsoluteScreenspaceCursor
    {
        Matrix playerWorld;
        Matrix view;
        Matrix projection;
        Viewport port;

        public KinectRelativeScreenspaceCursor(KinectService Kinect, Handedness hand, Viewport Port)
            : base(Kinect, hand)
        {
            playerWorld = new Matrix();
            view = new Matrix();
            projection = new Matrix();
            port = Port;
        }

        public override void Update()
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
                Vector3 positionTemp = new Vector3();
                positionTemp.X = handPoint.X  / 640; // scales up to whatever resolution I like
                positionTemp.Y = handPoint.Y / 480;
                positionTemp.Z = playerWorld.Translation.Z;
                positionTemp = port.Project(positionTemp, projection, view, playerWorld);
                _position.X = positionTemp.X;
                _position.Y = positionTemp.Y;
                _newDataReady = false;
            } 
        }

        public void SetWorld(Matrix World)
        {
            playerWorld = World;
        }

        public void SetView(Matrix View)
        {
            view = View;
        }

        public void SetProjection(Matrix Projection)
        {
            projection = Projection;
        }
    }
}
