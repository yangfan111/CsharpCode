using System;
using UnityEngine;

namespace Assets.Sources.Free.Utility
{
    public class FreeMathUtility
    {
        public static double RAD = Math.PI / 180;

        public static void AnglesToVector(double yaw, double pitch, ref Vector3 v)
        {
            float xy = (float)Math.Cos(pitch * RAD);

            v.z = xy * (float)Math.Cos(yaw * RAD);
            v.x = -xy * (float)Math.Sin(yaw * RAD);
            v.y = (float)Math.Sin(pitch * RAD);
        }

        public static void VectorToAngles(Vector3 v, ref Vector3 angles)
        {
            float yaw = 0;
            float pitch = 0;

            if (v.x == 0 && v.y == 0)
            {
                yaw = 0;
                if (v.z > 0)
                {
                    pitch = 90;
                }
                else if (v.z < 0)
                {
                    pitch = -90;
                }
            }
            else
            {
                if (v.x != 0)
                {
                    yaw = (float)(Math.Atan2(v.y, v.x) / RAD);
                }
                else
                {
                    if (v.y > 0)
                    {
                        yaw = 90;
                    }
                    else
                    {
                        yaw = -90;
                    }
                }
                var forward = Math.Sqrt(v.x * v.x + v.y * v.y);
                pitch = (float)(Math.Atan2(v.z, forward) / RAD);
            }

            angles.Set(yaw, pitch, 0);
        }


        public static void Vector3DMA(Vector3 v, float s, Vector3 b, ref Vector3 o)
        {
            o.x = v.x + b.x * s;
            o.y = v.y + b.y * s;
            o.z = v.z + b.z * s;
        }
    }
}
