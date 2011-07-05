using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Spacestro.Entities;

namespace Spacestro.Cloud.AI
{
    class AIUtil
    {
        public static float getAnglePointingAt(Vector2 looker, Vector2 target)
        {
            Vector2 tempVect = new Vector2(target.X - looker.X, target.Y - looker.Y);
            return (float)Math.Atan2(tempVect.Y, tempVect.X);
        }

        public static float getAnglePredictPlayerPos(Enemy e, Player p)
        {
            Vector2 vect1 = p.Position - e.Position;

            float a = p.Velocity.LengthSquared() - (15 * 15);
            float b = 2 * (Vector2.Dot(p.Velocity, vect1));
            float c = vect1.LengthSquared();

            if ((b * b - 4 * a * c) > 0 && a != 0)
            {
                float t1 = (-b + (float)Math.Sqrt((double)(b * b - 4 * a * c))) / (2 * a);
                float t2 = (-b + (float)Math.Sqrt((double)(b * b - 4 * a * c))) / (2 * a);

                float goodtime;
                if (t1 > 0 && t2 > 0)
                {
                    if (t1 > t2)
                        goodtime = t2;
                    else
                        goodtime = t1;
                }
                else if (t1 > 0) { goodtime = t1; }
                else { goodtime = t2; }

                Vector2 predictPos = p.Position + p.Velocity * goodtime * -3;

                return getAnglePointingAt(e.Position, predictPos);
            }
            else
            {
                return -1;
            }
        }
    }
}
