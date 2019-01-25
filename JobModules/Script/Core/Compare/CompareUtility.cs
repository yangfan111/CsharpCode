using System;
using Core.EntityComponent;
using UnityEngine;

namespace Core.Compare
{

    public class CompareUtility
    {
        public static bool IsApproximatelyEqual(bool left, bool right)
        {
            return left == right;
        }

        public static bool IsApproximatelyEqual(int left, int right)
        {
            return left == right;
        }

        public static bool IsApproximatelyEqual(EntityKey left, EntityKey right)
        {
            return left == right;
        }

        public static bool IsApproximatelyEqual(float left, float right, float maxError = 0.01f)
        {
            return System.Math.Abs(left - right) < maxError;
        }
        public static bool IsApproximatelyEqual(double left, double right, double maxError = 0.01)
        {
            return System.Math.Abs(left - right) < maxError;
        }
        public static bool IsApproximatelyEqual(Vector3 left, Vector3 right, float maxError)
        {
            Vector3 l = left;
            Vector3 r = right;
            return IsApproximatelyEqual(l.x, r.x, maxError) && 
                IsApproximatelyEqual(l.y, r.y, maxError) &&
                   IsApproximatelyEqual(l.z, r.z, maxError);
        }

        public static bool IsApproximatelyEqual(Vector3 left, Vector3 right)
        {
            Vector3 l = left;
            Vector3 r = right;
            return IsApproximatelyEqual(l.x, r.x) && IsApproximatelyEqual(l.y, r.y) &&
                   IsApproximatelyEqual(l.z, r.z);
        }

        public static bool IsApproximatelyEqual(Quaternion left, Quaternion right, float maxError = 0.01f)
        {
            Quaternion l = left;
            Quaternion r = right;
            return IsApproximatelyEqual(l.x, r.x, maxError) && 
                IsApproximatelyEqual(l.y, r.y, maxError) &&
                IsApproximatelyEqual(l.z, r.z, maxError) &&
                IsApproximatelyEqual(l.w, r.w, maxError);
        }
    }
}