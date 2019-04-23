using UnityEngine;

namespace App.Client.GPUInstancing.Core.Utils
{
    static class Helper
    {
        public static int RoundToPowerOfTwo(int value)
        {
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return ++value;
        }

        public static float Dot(this Vector3 left, Vector3 right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z;
        }

        public static float FarthestDistance(Vector3 normal, Vector2 basePos, Vector2 rectSize)
        {
            var farthestX = normal.x > 0 ? basePos.x + rectSize.x : basePos.x;
            var farthestZ = normal.z > 0 ? basePos.y + rectSize.y : basePos.y;

            return normal.x * farthestX + normal.z * farthestZ;
        }

        public static bool AlmostEqual(float left, float right, float epsilon = 0.01f)
        {
            return Mathf.Abs(left - right) <= epsilon;
        }

        public static bool AlmostEqual(Vector2 left, Vector2 right, float epsilon = 0.01f)
        {
            return Mathf.Abs(left.x - right.x) <= epsilon &&
                   Mathf.Abs(left.y - right.y) <= epsilon;
        }

        public static bool AlmostEqual(Vector3 left, Vector3 right, float epsilon = 0.01f)
        {
            return Mathf.Abs(left.x - right.x) <= epsilon &&
                   Mathf.Abs(left.y - right.y) <= epsilon &&
                   Mathf.Abs(left.z - right.z) <= epsilon;
        }

        public static void ConvertToFloatArray(this Vector3 v, float[] array)
        {
            array[0] = v.x;
            array[1] = v.y;
            array[2] = v.z;
        }

        public static void ConvertToFloatArray(this Color v, float[] array)
        {
            array[0] = v.r;
            array[1] = v.g;
            array[2] = v.b;
            array[3] = v.a;
        }

        public static void ConvertToFloatArray(this Matrix4x4 m, float[] array)
        {
            array[0] = m[0, 0];
            array[1] = m[1, 0];
            array[2] = m[2, 0];
            array[3] = m[3, 0];
            array[4] = m[0, 1];
            array[5] = m[1, 1];
            array[6] = m[2, 1];
            array[7] = m[3, 1];
            array[8] = m[0, 2];
            array[9] = m[1, 2];
            array[10] = m[2, 2];
            array[11] = m[3, 2];
            array[12] = m[0, 3];
            array[13] = m[1, 3];
            array[14] = m[2, 3];
            array[15] = m[3, 3];
        }
    }
}
