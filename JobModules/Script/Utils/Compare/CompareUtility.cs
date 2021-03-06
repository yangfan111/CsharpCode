﻿using System;
using UnityEngine;

namespace Utils.Compare
{

    public class CompareUtility
    {
        public static bool IsApproximatelyEqual(bool left, bool right)
        {
            return left == right;
        }
        
        public static bool IsApproximatelyEqual(Enum left, Enum right)
        {
            return left == right;
        }
        
        public static bool IsBetween<T>( T value, T min, T max) where T : IComparable<T>
        {
            return (min.CompareTo(value) < 0) && (value.CompareTo(max) <= 0);
        }
        public static T LimitBetween<T>( T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(value) > 0)
                return min;
            if (max.CompareTo(value) < 0)
                return max;
            return value;
        }
        public static bool IsApproximatelyEqual(int left, int right)
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