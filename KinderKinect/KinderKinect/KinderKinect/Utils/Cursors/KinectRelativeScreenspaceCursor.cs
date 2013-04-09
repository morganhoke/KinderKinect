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

        public KinectRelativeScreenspaceCursor(KinectService Kinect, Handedness hand)
            : base(Kinect, hand)
        {
            playerWorld = new Matrix();
            view = new Matrix();
            projection = new Matrix();
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

                Microsoft.Xna.Framework.Vector3 handVector = new Microsoft.Xna.Framework.Vector3(hand.Position.X, hand.Position.Y, hand.Position.Z);
                handVector = myGame.GraphicsDevice.Viewport.Project(handVector, projection, view, playerWorld);

                _position = new Vector2(handVector.X, handVector.Y);
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
