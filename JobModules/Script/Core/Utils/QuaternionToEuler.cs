using Core.Compare;
using UnityEngine;

namespace Core.Utils
{
    public class QuaternionToEuler
    {
        public float EulerX { get; private set; }
        public float EulerY { get; private set; }
        public float EulerZ { get; private set; }

        private readonly float _deltaThresholdBetweenFrame;
        private const float Epsilon = 1e-4f;
        
        private static readonly float RadianToDegree = 180 / Mathf.PI;
        private static readonly float DegreeToRadian = Mathf.PI / 180;

        public QuaternionToEuler(float threshold = 90, float initEulerX = 0, float initEulerY = 0, float initEulerZ = 0)
        {
            _deltaThresholdBetweenFrame = threshold;
            EulerX = initEulerX;
            EulerY = initEulerY;
            EulerZ = initEulerZ;
        }
        
        //          | CyCz + SySxSz  CzSySx - CySz  CxSy |
        // RyRxRz = |      CxSz          CxCz        -Sx |
        //          | CySxSz - CzSy  CyCzSx + SySz  CyCx |
        // float[,] matQ =
        // {
        //     { 1 - 2 * rotQ.y * rotQ.y - 2 * rotQ.z * rotQ.z, 2 * rotQ.x * rotQ.y - 2 * rotQ.w * rotQ.z, 2 * rotQ.x * rotQ.z + 2 * rotQ.w * rotQ.y },
        //     { 2 * rotQ.x * rotQ.y + 2 * rotQ.w * rotQ.z, 1 - 2 * rotQ.x * rotQ.x - 2 * rotQ.z * rotQ.z, 2 * rotQ.y * rotQ.z - 2 * rotQ.w * rotQ.x },
        //     { 2 * rotQ.x * rotQ.z - 2 * rotQ.w * rotQ.y, 2 * rotQ.y * rotQ.z + 2 * rotQ.w * rotQ.x, 1 - 2 * rotQ.x * rotQ.x - 2 * rotQ.y * rotQ.y }
        // }
        
        public void Update(Quaternion value)
        {
            var qW = value.w;
            var qX = value.x;
            var qY = value.y;
            var qZ = value.z;

            float m00 = 1 - 2 * (qY * qY + qZ * qZ);
            float m02 = 2 * (qX * qZ + qW * qY);
            float m10 = 2 * (qX * qY + qW * qZ);
            float m11 = 1 - 2 * (qX * qX + qZ * qZ);
            float m12 = 2 * (qY * qZ - qW * qX);
            float m20 = 2 * (qX * qZ - qW * qY);
            float m22 = 1 - 2 * (qX * qX + qY * qY);

            if (CompareUtility.IsApproximatelyEqual(m02, 0, Epsilon) &&
                CompareUtility.IsApproximatelyEqual(m22, 0, Epsilon))
            {
                if (m12 < 0) // Sx equals 1
                {
                    EulerX = Mathf.RoundToInt((EulerX - 90) / 360) * 360 + 90;
                    EulerZ = EulerY + Mathf.Atan2(m20, m00) * RadianToDegree;
                }
                else
                {
                    EulerX = Mathf.RoundToInt((EulerX + 90) / 360) * 360 - 90;
                    EulerZ = Mathf.Atan2(-m20, m00) * RadianToDegree - EulerY;
                }
            }
            else
            {
                var newEulerY = Mathf.Atan2(m02, m22) * RadianToDegree;
                if (Mathf.Abs(newEulerY - EulerY) > _deltaThresholdBetweenFrame)
                    newEulerY += 180 * (newEulerY > EulerY ? -1 : 1);
                EulerY = newEulerY;

                EulerX = RadianToDegree * (CompareUtility.IsApproximatelyEqual(m02, 0, Epsilon)
                             ? Mathf.Atan2(-m12, m22 / Mathf.Cos(EulerY * DegreeToRadian))
                             : Mathf.Atan2(-m12, m02 / Mathf.Sin(EulerY * DegreeToRadian)));
                EulerZ = RadianToDegree * (Mathf.Cos(EulerX * DegreeToRadian) > 0
                             ? Mathf.Atan2(m10, m11)
                             : Mathf.Atan2(-m10, -m11));
            }

            EulerX = (EulerX = EulerX % 360) >= 0 ? EulerX : EulerX + 360;
            EulerY = (EulerY = EulerY % 360) >= 0 ? EulerY : EulerY + 360;
            EulerZ = (EulerZ = EulerZ % 360) >= 0 ? EulerZ : EulerZ + 360;
        }
    }
}