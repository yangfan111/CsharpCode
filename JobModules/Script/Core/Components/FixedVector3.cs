using System;
using Core.SpatialPartition;
using UnityEngine;

namespace Core.Components
{
    public static class WorldOrigin
    {
        public static Vector3 Origin = Vector3.zero;

        public static Vector3 WorldPosition(this Vector3 v)
        {
            return v + Origin;
        }

        public static Vector3 ShiftedPosition(this Vector3 v)
        {
            return v - Origin;
        }

        public static FixedVector3 ShiftedToFixedVector3(this Vector3 v)
        {
            return new FixedVector3(v + Origin);
        }
    }

    public struct FixedVector3
    {
        public const float kEpsilon = 1E-05f;
        public float x;
        public float y;
        public float z;
        public static FixedVector3 zero = new FixedVector3(0, 0, 0);

        public FixedVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public override bool Equals(object other)
        {
            if (!(other is FixedVector3))
                return false;
            FixedVector3 vector3 = (FixedVector3) other;
            return this.x.Equals(vector3.x) && this.y.Equals(vector3.y) && this.z.Equals(vector3.z);
        }

        public FixedVector3(Vector3 worldPosition)
        {
            this.x = worldPosition.x;
            this.y = worldPosition.y;
            this.z = worldPosition.z;
        }

        public Vector3 WorldVector3()
        {
            return new Vector3(x, y, z);
        }

        public Vector3 ShiftedVector3()
        {
            var origin = WorldOrigin.Origin;
            return new Vector3(x - origin.x, y - origin.y, z - origin.z);
        }


        public string ToStringExt()
        {
            var origin = WorldOrigin.Origin;
            return String.Format("world:({0}, {1}, {2}) shifted:({3}, {4},{5})", x, y, z, x - origin.x, y - origin.y,
                z - origin.z);
        }

        public override string ToString()
        {
            var origin = WorldOrigin.Origin;
            return String.Format("world:({0:F1}, {1:F1}, {2:F1}) shifted:({3:F1}, {4:F1},{5:F1})", x, y, z,
                x - origin.x,
                y - origin.y, z - origin.z);
        }
    }

    public struct FixedVector2
    {
        public const float kEpsilon = 1E-05f;
        public float x;

        public float y;
        public static FixedVector2 zero = new FixedVector2(0, 0);

        public FixedVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }


        public override bool Equals(object other)
        {
            if (!(other is FixedVector2))
                return false;
            FixedVector2 vector2 = (FixedVector2) other;
            return this.x.Equals(vector2.x) && this.y.Equals(vector2.y);
        }

        public FixedVector2(Vector2 worldPosition)
        {
            this.x = worldPosition.x;
            this.y = worldPosition.y;
        }

        public Vector2 WorldVector2()
        {
            return new Vector2(x, y);
        }

        public Vector2 ShiftedVector2()
        {
            var origin = WorldOrigin.Origin.To2D();
            return new Vector2(x - origin.x, y - origin.y);
        }


        public string ToStringExt()
        {
            var origin = WorldOrigin.Origin;
            return String.Format("world:({0}, {1}, {2}) shifted:({3}, {4},{5})", x, y, x - origin.x, y - origin.y);
        }

        public override string ToString()
        {
            var origin = WorldOrigin.Origin;
            return String.Format("world:({0:F1}, {1:F1}) shifted:({3:F1}, {4:F1})", x, y, x - origin.x,
                y - origin.y);
        }
    }
}