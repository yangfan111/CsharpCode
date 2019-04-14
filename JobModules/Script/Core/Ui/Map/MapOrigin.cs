using UnityEngine;

namespace Core.Ui.Map
{
    public static class MapOrigin
    {
        public static Vector3 Origin = Vector3.zero;
        public static Vector2 Size = Vector2.zero;
        
        public static Vector3 WorldPosition(this Vector3 v)
        {
            return v + Origin;
        }

        public static Vector3 ShiftedPosition(this Vector3 v)
        {
            return v - Origin;
        }

    }
}