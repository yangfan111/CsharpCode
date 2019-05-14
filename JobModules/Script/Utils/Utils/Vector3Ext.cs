using System;
using UnityEngine;

namespace Core.Utils
{
    public static class Vector3Ext
    {
        public static Vector4 ToVector4(this Vector3 v3)
        {
            return new Vector4(v3.x, v3.y, v3.z, 1);
        }
        
        public static Vector3 ToVector3(this Vector4 v3)
        {
            return new Vector3(v3.x / v3.w, v3.y / v3.w, v3.z / v3.w);
        }

        public static string ToStringExt(this Vector2 v2)
        {
             return String.Format("({0}, {1})", v2.x, v2.y);
        }
       
        public static string ToStringExt(this Vector3 v3)
        {
             return String.Format("({0}, {1}, {2})", v3.x, v3.y, v3.z);
        }
        
        public static string ToStringExt(this Vector4 v4)
        {
            return String.Format("({0}, {1}, {2}, {3})", v4.x, v4.y, v4.z, v4.w);
        }

        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            Vector3 c;
            c.x = a.x + b.x;
            c.y = a.y + b.y;
            c.z = a.z + b.z;
            return c;
        }

        public static Vector3 Scale(Vector3 a, float scale)
        {
            Vector3 b;
            b.x = a.x * scale;
            b.y = a.y * scale;
            b.z = a.z * scale;
            return b;
        }

        public static void ScaleBy(this Vector3 a, float scale)
        {
            a.x *= scale;
            a.y *= scale;
            a.z *= scale;
        }

        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            Vector3 fromNorm = from.normalized, toNorm = to.normalized;
            float unsignedAngle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(fromNorm, toNorm), -1F, 1F)) * Mathf.Rad2Deg;
            float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(fromNorm, toNorm)));
            return unsignedAngle * sign;
        }

        public static float GetPitch(this Vector3 from)
        {
            return Mathf.Asin(-from.normalized.y) * Mathf.Rad2Deg;
        }

        public static float GetYaw(this Vector3 from)
        {
            return SignedAngle(Vector3.forward, new Vector3(from.x, 0, from.z), Vector3.up);
        }
    }

 
}