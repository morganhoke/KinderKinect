using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KinderKinect.ButterflyGarden
{
    class ButterflyAI
    {
        static Random rand = new Random();

        public static Vector3 AIGetNewPosition(Butterfly b, int tier, Vector3 tetherPoint)
        {
            switch (tier)
            {
                case 0:
                    goto default;
                case 1:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), 0.025f);
                case 2:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), 0.03f);
                case 3:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), 0.035f);
                case 4:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .040f);
                case 5:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .045f);
                case 6:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .050f);
                case 7:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .55f);
                case 8:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .60f);
                case 9:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .225f);
                default:
                    return Wander(b, new Vector2(b.getPosition().X, b.getPosition().Y), 0f);
            }
        }

        /// <summary>
        /// This code heavily influenced by http://xbox.create.msdn.com/en-US/education/catalog/sample/chase_evade
        /// </summary>
        /// <param name="b"> the butterfly I wish to operate on</param>
        /// <param name="tether">that butterfly's tether point</param>
        /// <param name="speed">the speed at which we can move the butterfly</param>
        /// <returns></returns>
        private static Vector3 Wander(Butterfly b, Vector2 tether, float speed)
        {
            

            float wanderX = b.WanderDirection.X;
            float wanderY = b.WanderDirection.Y;

            Vector3 ButterflyPosition = b.getPosition();

            wanderX += MathHelper.Lerp(-.25f, .25f, (float)rand.NextDouble());
            wanderY += MathHelper.Lerp(-.25f, .25f, (float)rand.NextDouble());

            Vector2 wanderDirection = new Vector2(wanderX, wanderY);

            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }

            b.WanderDirection = wanderDirection;

            float distanceFromTether = Vector2.Distance(new Vector2(b.getPosition().X, b.getPosition().Y), tether);
            float maxDistanceFromTether = 1f;

            float normalizedDistance = distanceFromTether / maxDistanceFromTether;

            float turnBackToTetherSpeed = normalizedDistance;

            float angle = getRotationAngle(new Vector2(ButterflyPosition.X, ButterflyPosition.Y), tether, wanderDirection, turnBackToTetherSpeed * .15f);

            wanderDirection = Vector2.Transform(wanderDirection, Matrix.CreateRotationZ(angle));

            Vector3 displacement = new Vector3(wanderDirection, 0);

            Vector3 updateVect = ButterflyPosition + (displacement * speed);

            Vector3 clampedVect = Vector3.Clamp(updateVect, new Vector3(-6, -6, 0), new Vector3(6,5,0));
            
            if(updateVect.Length() != clampedVect.Length()) // we may be at an edge, lets flip ourselves 90 degrees
            {
                b.WanderDirection = b.WanderDirection * -1;
            }

            return clampedVect;
        }

        /// <summary>
        /// helps with tethering, based upon http://xbox.create.msdn.com/en-US/education/catalog/sample/chase_evade
        /// </summary>
         private static float getRotationAngle(Vector2 position, Vector2 toFace, Vector2 direction, float speed)
        {
            float x = toFace.X - position.X;
            float y = toFace.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float currentAngle = (float)Math.Atan2(direction.Y, direction.X);


            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -speed, speed);

            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
         /// Stolen from http://xbox.create.msdn.com/en-US/education/catalog/sample/chase_evade
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
         private static float WrapAngle(float radians)
         {
             while (radians < -MathHelper.Pi)
             {
                 radians += MathHelper.TwoPi;
             }
             while (radians > MathHelper.Pi)
             {
                 radians -= MathHelper.TwoPi;
             }
             return radians;
         }


    }
}
