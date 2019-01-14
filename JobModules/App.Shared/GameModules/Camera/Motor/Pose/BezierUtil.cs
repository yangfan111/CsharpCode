using System.ComponentModel;
using Core.Interpolate;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Camera.Motor.Pose
{
    public class BezierUtil
    {
        private int _segmentCount = 10;
        public Vector3[] Result;
        public float TotalX;
   

        public void CreateRandomPoints(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, int segmentCount)
        {
            Vector3[] Points;
            Points = new Vector3[segmentCount * 3 + 1];
            int count = Points.Length;
            Points[0] = Vector3.zero;
            TotalX = 0;
            float f = 1;
            for (int i = 1; i < count - 1; i++)
            {
                float rx = Random.Range(minX, maxX);
               
                TotalX += rx;
                if (i % 3 == 0)
                {
                    Points[i] = new Vector3(TotalX,0,0);
                    f *= -1;
                }
                else
                {
                    float ry = f * Random.Range(maxY / 2, maxY);
                    float rz = f * Random.Range(maxZ / 2, maxZ);
                    Points[i] = new Vector3(TotalX, ry, rz);
                }
            }

            Points[count - 1] = new Vector3(TotalX + Random.Range(minX, maxX), 0,0);
            
            int curveCount = Points.Length ;
            Result = new Vector3[_segmentCount];
            int idx = 0;
            for (int j = 0; j < curveCount-3; j+=3)
            {
                for (int i = 1; i <= _segmentCount; i++)
                {
                    float t = i / (float) _segmentCount;
                    int nodeIndex = j;
                    Vector3 pixel = CalculateCubicBezierPoint(t, Points[nodeIndex], Points[nodeIndex + 1],
                        Points[nodeIndex + 2], Points[nodeIndex + 3]);
                    ArrayUtility.SafeSet(ref Result, idx, pixel, Vector3.zero);
                    idx++;

                }
            }
        }

        public Vector3 GetResult(float x)
        {
            var t = (x % TotalX) / TotalX;
            int lenght = Result.Length;
            var lidx = (int) (t * lenght) % lenght;
            var ridx = ((int) (t * lenght) + 1) % lenght;
            var l = Result[lidx];
            var r = Result[ridx];
            float ratio = t * lenght % 1;
            return InterpolateUtility.Interpolate(l, r, ratio);
        }

        Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            Vector3 p = new Vector3();
            p.x = uuu * p0.x;
            p.y = uuu * p0.y;
            p.z = uuu * p0.z;
            p.x += 3 * uu * t * p1.x;
            p.y += 3 * uu * t * p1.y;
            p.z += 3 * uu * t * p1.z;
            p.x += 3 * u * tt * p2.x;
            p.y += 3 * u * tt * p2.y;
            p.z += 3 * u * tt * p2.z;
            p.x += ttt * p3.x;
            p.y += ttt * p3.y;
            p.z += ttt * p3.z;

            return p;
        }
    }
}