using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.FreeFramework.framework.ai.move
{
    public class AutoMoveUtil
    {

        private static double angle = Math.Tan(Math.PI / 4);
        private static double Highest = 1.8;

        private static double H_Stand = 1.7;
        private static double H_Crouch = 1.2;
        private static double WalkHeight = 0.3;

        public enum MoveType
        {
            Walk, Jump, Crouch, Creep
        }

        public static bool CanDirectMoveTo(Vector3 from, Vector3 to)
        {
            RaycastHit[] line = GetLine(from, to);
            for (int i = 0; i < line.Length - 1; i++)
            {
                if (!CanGo(line[i], line[i + 1]))
                {
                    return false;
                }
            }

            return true;
        }

        public static Vector3 GetDistancePoint(Vector3 from, Vector3 to, float dis)
        {
            float d = Vector3.Distance(from, to);
            if (dis > d)
            {
                return to;
            }

            return GetGround(new Vector3(from.x + (to.x - from.x) * dis / d,
                from.y + (to.y - from.y) * dis / d, from.z + (to.z - from.z) * dis / d)).point;
        }

        public static MoveType GetMoveType(PlayerEntity player, Vector3 target)
        {
            Vector3 from = player.position.Value;
            Vector3 to = GetDistancePoint(from, target, 0.5f);

            double low = LowHight(from, to);
            double through = ThroughHeight(from, to);

            Debug.LogFormat("low={0}, through={1}, from:{2}, to{3}", low, through, from, to);

            if (low < WalkHeight - 0.001)
            {
                return MoveType.Walk;
            }
            else
            {
                return MoveType.Jump;
            }
        }

        public static float GetYaw(Vector3 from, Vector3 to)
        {
            return (float)(Math.Atan2(to.x - from.x, to.z - from.z) * 180 / Math.PI);
        }

        public static Vector3 GetRotationPos(Vector3 from, Vector3 to, float deltaAngle, int dis)
        {
            double yaw = (GetYaw(from, to) + deltaAngle) * Math.PI / 180;

            return new Vector3(from.x + dis * (float)Math.Sin(yaw), from.y, from.z + dis * (float)Math.Cos(yaw));
        }

        private static bool CanGo(RaycastHit from, RaycastHit to)
        {
            Vector3 delta = from.normal;

            bool canGo = false;

            bool h = UpCanGo(from.point, to.point);
            double len = 0;

            len = Math.Sqrt(delta.x * delta.x + delta.z * delta.z);
            if (len == 0)
            {
                canGo = h;
            }
            else
            {
                if (delta.y / len > angle + 0.001)
                {
                    canGo = h;
                }
                else
                {
                    canGo = false;
                }

            }

            if (len == 0)
            {
                len = 0.0001;
            }

            Debug.LogFormat("{0} from:{1}, to:{2},{3}/{4}={8}<{5},  a:{6}, h:{7}", canGo, from.point, to.point, delta.y, len, angle, delta.y / len <= angle, h, delta.y / len);

            return canGo;
        }

        public static double LowHight(Vector3 from, Vector3 to)
        {
            double h = 0;

            for (int i = 1; i <= (Highest + H_Stand) * 10; i++)
            {
                float currentHight = (float)i * 0.1f;
                if (IsBlocked(new Vector3(from.x, from.y + currentHight, from.z),
                    new Vector3(to.x, from.y + currentHight, to.z)))
                {
                    h = currentHight;
                }
                else
                {
                    if (currentHight - h > Highest)
                    {
                        break;
                    }
                }
            }

            return h;
        }

        public static double ThroughHeight(Vector3 from, Vector3 to)
        {
            double h = 0;
            double maxH = 0;
            for (int i = 0; i < 20; i++)
            {
                float currentHight = 0.2f * (float)i;
                if (IsBlocked(new Vector3(from.x, from.y + currentHight, from.z),
                    new Vector3(to.x, from.y + currentHight, to.z)))
                {
                    h = currentHight;
                }
                else
                {
                    double c = currentHight;
                    if (h <= Highest + 0.01)
                    {
                        maxH = c - h;
                        if (c - h >= 2)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return maxH;
        }

        public static bool UpCanGo(Vector3 from, Vector3 to)
        {
            return ThroughHeight(from, to) >= H_Crouch;
        }

        private static bool IsBlocked(Vector3 from, Vector3 to)
        {
            Ray r = new Ray(from, to - from);

            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo);

            if (hited)
            {
                return hitInfo.point.x >= from.x && hitInfo.point.x <= to.x ||
                    hitInfo.point.x <= from.x && hitInfo.point.x >= to.x;
            }
            else
            {
                return false;
            }
        }

        private static RaycastHit[] GetLine(Vector3 from, Vector3 to)
        {
            float dis = Vector3.Distance(from, to);
            RaycastHit[] vs = new RaycastHit[(int)dis + 2];
            vs[0] = new RaycastHit();
            vs[0].point = from;
            vs[0].normal = new Vector3(0, 1, 0);
            for (int i = 1; i < vs.Length - 1; i++)
            {
                vs[i] = GetGround(new Vector3(from.x + i * (to.x - from.x) / dis, from.y, from.z + i * (to.z - from.z) / dis));
            }
            vs[vs.Length - 1] = new RaycastHit();
            vs[vs.Length - 1].point = to;
            vs[vs.Length - 1].normal = new Vector3(0, 1, 0);

            return vs;
        }


        public static RaycastHit GetGround(Vector3 v)
        {
            Vector3 high = new Vector3(v.x, v.y + 1000, v.z);
            Ray r = new Ray(high, v - high);

            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo);

            if (hited)
            {
                return hitInfo;
            }

            return default(RaycastHit);
        }

    }
}
