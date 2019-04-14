using System;
using Core.Components;
using Core.SpatialPartition;
using UnityEngine;

namespace Core.Ui.Map
{
    public struct MapFixedVector3
    {
        public FixedVector3 Vector;
        
        public MapFixedVector3(float x, float y, float z)
        {
            Vector = new FixedVector3(x, y, z);
        }
        public MapFixedVector3(FixedVector3 fixedVector3)
        {
            Vector = fixedVector3;
        }
        public MapFixedVector3(Vector3 vector3)
        {
            Vector = new FixedVector3(vector3);
        }
        
        public void Set(float x, float y, float z)
        {
            Vector.x = x;
            Vector.y = y;
            Vector.z = z;
        }
        public void Set(FixedVector3 fixedVector3)
        {
            Vector = fixedVector3;
        }
        public void Set(Vector3 vector3)
        {
            Vector.x = vector3.x;
            Vector.y = vector3.y;
            Vector.z = vector3.z;
        }
        
        
        
        public override bool Equals(object other)
        {
            return Vector.Equals(other);
        }
        
        public Vector3 WorldVector3()
        {
            return Vector.WorldVector3();
        }

        public Vector3 ShiftedVector3()
        {
            return Vector.ShiftedVector3();
        }
        /**
         *世界坐标的偏移和UI显示坐标便宜
         */
        public Vector3 ShiftedUIVector3()
        {
            var origin = MapOrigin.Origin;
            var v = Vector.ShiftedVector3();
            return v - origin;
        }
        
        public override string ToString()
        {
            var sv = ShiftedVector3();
            return String.Format("world:({0:F1}, {1:F1}, {2 :F1}) shifted:({3:F1}, {4:F1}, {5:F1})", Vector.x, Vector.y, Vector.z, sv.x,
                sv.y, sv.z);
        }
    }
    public struct MapFixedVector2
    {
        public FixedVector2 Vector;
        
        public MapFixedVector2(float x, float y)
        {
            Vector = new FixedVector2(x, y);
        }
        public MapFixedVector2(Vector2 vector2)
        {
            Vector = new FixedVector2(vector2);
        }
        public void Set(FixedVector2 fixedVector2)
        {
            Vector = fixedVector2;
        }
        public void Set(float x, float y)
        {
            Vector.x = x;
            Vector.y = y;
        }
        public void Set(Vector2 vector2)
        {
            Vector.x = vector2.x;
            Vector.y = vector2.y;
        }
        public MapFixedVector2(FixedVector2 fixedVector2)
        {
            Vector = fixedVector2;
        }
        
        public override bool Equals(object other)
        {
            return Vector.Equals(other);
        }
        
        public Vector2 WorldVector2()
        {
            return Vector.WorldVector2();
        }

        /**
         *世界坐标的偏移
         */
        public Vector2 ShiftedVector2()
        {
            return Vector.ShiftedVector2();
        }
        
        /**
         *世界坐标的偏移和UI显示坐标便宜
         */
        public Vector2 ShiftedUIVector2()
        {
            var origin = MapOrigin.Origin.To2D();
            return new Vector2(Vector.x - origin.x, Vector.y - origin.y);
        }
        
        public override string ToString()
        {
            var sv = ShiftedUIVector2();
            return String.Format("world:({0:F1}, {1:F1}) shifted:({2:F1}, {3:F1})", Vector.x, Vector.y, sv.x,
                sv.y);
        }
    }
}