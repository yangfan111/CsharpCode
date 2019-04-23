using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.Utils
{
    public static class MatrixExt
    {
        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward = new Vector3(matrix.m02, matrix.m12, matrix.m22);
            Vector3 upward = new Vector3(matrix.m01, matrix.m11, matrix.m21);

            return Quaternion.LookRotation(forward, upward);
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            return new Vector3(
                new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude,
                new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude,
                new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m31).magnitude);
        }

        public static string ToStringExt(this Matrix4x4 matrix)
        {
            return string.Format("| {0}, {1}, {2}, {3} | {4}, {5}, {6}, {7} | {8}, {9}, {10}, {11} | {12}, {13}, {14}, {15}",
                matrix.m00, matrix.m01, matrix.m02, matrix.m03,
                matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                matrix.m20, matrix.m11, matrix.m22, matrix.m23,
                matrix.m30, matrix.m21, matrix.m22, matrix.m23);
        }
    }
}
