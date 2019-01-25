using UnityEngine;

namespace Core.CameraControl
{
    public static class YawPitchUtility
    {
        public static float Normalize(float yaw)
        {
            yaw = yaw % 360;
            if (yaw < -180f) { yaw = yaw + 360f; }
            else if (yaw > 180f) { yaw = yaw - 360f; }
            return yaw;
        }
        
        public static Vector3 Normalize(Quaternion quat)
        {
            return Normalize(quat.eulerAngles);
        }
        
        public static Vector3 NormalizeExt(this Quaternion quat)
        {
            return Normalize(quat.eulerAngles);
        }
        
        public static Vector3 Normalize(Vector3 eulerAngle)
        {
            return new Vector3(Normalize(eulerAngle.x), Normalize(eulerAngle.y), Normalize(eulerAngle.z));
        }
        
        public static Vector3 NormalizeExt(this Vector3 eulerAngle)
        {
            return new Vector3(Normalize(eulerAngle.x), Normalize(eulerAngle.y), Normalize(eulerAngle.z));
        }
        
        public static float CalcDeltaAngle(float fromAngle, float toAngle)
        {
            fromAngle = Normalize(fromAngle);
            toAngle = Normalize(toAngle);
            if (Mathf.Abs(fromAngle - toAngle) <= 180)
                return toAngle - fromAngle;
            else
            {
                if (fromAngle < 0)
                {
                    return -180 - fromAngle + toAngle - 180;
                }
                else
                {
                    return 180 - fromAngle + toAngle - (-180);
                }
            }
        }
    }
}