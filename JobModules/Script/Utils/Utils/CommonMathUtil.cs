using System;
using UnityEngine;

namespace Utils.Utils
{
    public class CommonMathUtil
    {
        public static float GetAngle(Vector2 pos1, Vector2 pos2)
        {
            if (pos1.x > pos2.x)
            {
                return (float)(Math.Acos((pos1.y - pos2.y) / Vector2.Distance(pos1, pos2)) * 180 / Math.PI);
            }
            else
            {
                return 360f - (float)(Math.Acos((pos1.y - pos2.y) / Vector2.Distance(pos1, pos2)) * 180 / Math.PI);
            }
        }

        public static float TransComAngle(float angle)
        {
            angle = angle % 360;
            return angle < 0 ? 360 + angle : angle;
        }

        public static float GetDiffAngle(float angle1, float angle2)
        {
            float diffAngle = Mathf.Abs(angle1 - angle2);
            return Mathf.Min(360 - diffAngle, diffAngle);
        }

        public static bool Raycast(Vector3 fromV, Vector3 toV, float maxDis, int layerMask, out Vector3 hitPoint)
        {
            Ray ray = new Ray();
            ray.origin = fromV;
            ray.direction = new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z);

            RaycastHit hitInfo;
            bool hited = Physics.Raycast(ray, out hitInfo, maxDis, layerMask);

            hitPoint = hitInfo.point;
            return hited;
        }

        public static bool Raycast(Ray ray, float maxDis, int layerMask, out RaycastHit hitInfo)
        {
            bool hited = Physics.Raycast(ray, out hitInfo, maxDis, layerMask);
            return hited;
        }

        public static Vector3 GetSpacePos(Vector3 pos, float maxDis, int layerMask)
        {
            Vector3 retPos = Vector3.zero;

            Ray ray = new Ray();
            ray.origin = pos;
            RaycastHit hitInfo;

            ray.direction = Vector3.left;
            if (Raycast(ray, maxDis, layerMask, out hitInfo))
                retPos += hitInfo.normal;

            ray.direction = Vector3.right;
            if (Raycast(ray, maxDis, layerMask, out hitInfo))
                retPos += hitInfo.normal;

            ray.direction = Vector3.forward;
            if (Raycast(ray, maxDis, layerMask, out hitInfo))
                retPos += hitInfo.normal;

            ray.direction = Vector3.back;
            if (Raycast(ray, maxDis, layerMask, out hitInfo))
                retPos += hitInfo.normal;

            if (retPos.magnitude > 0)
                retPos = retPos.normalized * maxDis;

            pos += retPos;

            ray.origin = pos + Vector3.up * maxDis;
            ray.direction = Vector3.down;
            if (Raycast(ray, maxDis * 2, layerMask, out hitInfo))
                pos.y = hitInfo.point.y + 0.1f;

            return pos;
        }
    }
}
