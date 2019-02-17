using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="BulletDirUtility" />
    /// </summary>
    public class BulletDirUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletDirUtility));

        public static Vector3 GetThrowingDir(PlayerWeaponController controller)
        {
            var yaw = controller.RelatedOrient.Yaw - controller.RelatedOrient.NegPunchYaw * 2;
            var pitch = controller.RelatedOrient.Pitch - controller.RelatedOrient.NegPunchPitch * 2;

            Quaternion q = Quaternion.Euler(pitch, yaw, 0);
            Vector3 forward = q.Forward();

            forward.y += 0.2f;
            return forward.normalized;
        }

        public static Vector3 CalculateShotingDir(int seed, float yaw, float pitch, float spreadX, float spreadY)
        {
            Quaternion q = Quaternion.Euler(pitch, yaw, 0);
            Vector3 forward = q.Forward();
            Vector3 right = q.Right();
            Vector3 up = q.Up();

            float x;
            float y;
            //,z;
            x = (float)UniformRandom.RandomFloat(seed + 0, -0.5, 0.5) +
                (float)UniformRandom.RandomFloat(seed + 1, -0.5, 0.5);
            y = (float)UniformRandom.RandomFloat(seed + 2, -0.5, 0.5) +
                (float)UniformRandom.RandomFloat(seed + 3, -0.5, 0.5);
            //z = x * x + y * y;
            float res1 = spreadX * x;
            float res2 = spreadY * y;
            right = Vector3Ext.Scale(right, res1);
            up = Vector3Ext.Scale(up, res2);
            var newForward = Vector3Ext.Add(forward, right);
            newForward = Vector3Ext.Add(newForward, up);
            //_logger.InfoFormat("spreadX {0}, spreadY {1}, x {2} y {3}, right{4}, up{5}", spreadX, spreadY, x, y, right, up);
            return newForward;
        }
    }
}
