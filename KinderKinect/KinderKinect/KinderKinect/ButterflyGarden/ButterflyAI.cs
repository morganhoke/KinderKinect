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
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), 0.05f);
                case 3:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), 0.075f);
                case 4:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .1f);
                case 5:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .125f);
                case 6:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .15f);
                case 7:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .175f);
                case 8:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .2f);
                case 9:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), .225f);
                default:
                    return Wander(b, new Vector2(tetherPoint.X, tetherPoint.Y), 0f);
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
            float maxDistanceFromTether = Math.Min(tether.Y, tether.X);

            float normalizedDistance = distanceFromTether / maxDistanceFromTether;

            float turnBackToTetherSpeed = normalizedDistance;

            float angle = getRotationAngle(new Vector2(ButterflyPosition.X, ButterflyPosition.Y), tether, wanderDirection, turnBackToTetherSpeed);

            wanderDirection = Vector2.Transform(wanderDirection, Matrix.CreateRotationZ(angle));

            Vector3 displacement = new Vector3(wanderDirection, 0);

            return Vector3.Clamp(ButterflyPosition + (displacement * speed), new Vector3(-6, -6, 0), new Vector3(6,5,0));
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
