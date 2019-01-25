using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using App.Shared.Components.Player;
using Core.Compare;
using Core.EntityComponent;
using Core.Event;
using UnityEngine;

namespace App.Shared.Components.Serializer
{
    public static class FieldEqualUtil
    {
        public static bool Equals(double x, double y)
        {
            return x == y;
        }

        public static bool Equals(int x, int y)
        {
            return x == y;
        }

        public static bool Equals(long x, long y)
        {
            return x == y;
        }

        public static bool Equals(float x, float y)
        {
            return x == y;
        }

        public static bool Equals(bool x, bool y)
        {
            return x == y;
        }

        public static bool Equals(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool Equals(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool Equals(Quaternion a, Quaternion b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }
        public static bool Equals(String a, String b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b, StringComparison.Ordinal);
        }  
        public static bool Equals(EntityKey a, EntityKey b)
        {
            return EntityKeyComparer.Instance.Compare(a, b)==0;
        }
        public static bool Equals(StateInterCommands a, StateInterCommands b)
        {
            return false;
        }
		
        public static bool Equals(PlayerEvents a, PlayerEvents b)
        {
            return false;
        }
    }
}
