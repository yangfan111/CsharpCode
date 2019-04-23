using UnityEngine;

namespace Core.Utils
{
    static class EulerUtility
    {
        public static float RAD = Mathf.PI / 180;
        public static void AnglesToVectors2(float cameraYaw, float cameraPitch, out Vector3 forward, out Vector3 right, out Vector3 up)
        {
            float sr;
            float sp;
            float sy;
            float cr;
            float cp;
            float cy;
            float yawAngle;

            yawAngle = cameraYaw * RAD;
            sy = Mathf.Sin(yawAngle);
            cy = Mathf.Cos(yawAngle);

            float pitchAngle = cameraPitch * RAD;
            sp = Mathf.Sin(pitchAngle);
            cp = Mathf.Cos(pitchAngle);
            
            sr = 0;
            cr = 1;

            forward.x = cp * cy;
            forward.y = cp * sy;
            forward.z = sp;
            
            right.x = (-1 * sr * sp * cy + -1 * cr * -sy);
            right.y = (-1 * sr * sp * sy + -1 * cr * cy);
            right.z = 1 * sr * cp;
           
            up.x = (cr * sp * cy + -sr * -sy);
            up.y = (cr * sp * sy + -sr * cy);
            up.z = -cr * cp;
        }
    }
}