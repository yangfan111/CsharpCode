using UnityEngine;

namespace Core.Compensation
{
    public struct RaySegment
    {
        public Ray Ray;
        public float Length;
        public override string ToString()
        {
            return string.Format("ray {0},length:{1}", Ray, Length);
        }
    }

    public struct BoxInfo
    {
        public Vector3 Origin;
        public Vector3 HalfExtens;
        public Quaternion Orientation;
        public Vector3 Direction;
        public float Length;
    }
}