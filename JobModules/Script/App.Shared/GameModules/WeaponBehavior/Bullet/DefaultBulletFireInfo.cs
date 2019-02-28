using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="DefaultBulletFireInfo" />
    /// </summary>
    public class DefaultBulletFireInfo : IBulletFireInfo
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultBulletFireInfo));

        public static Vector3 BulletEmittorOffset = new Vector3(0, 0, 0.1f);

        public virtual Vector3 GetFireDir(int seed, PlayerWeaponController controller)
        {
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;


            var orientation = controller.RelatedOrient;
            var yaw = orientation.Yaw - orientation.NegPunchYaw * 2;
            var pitch = orientation.Pitch - orientation.NegPunchPitch * 2;
            var dir = CalculateShotingDir(seed, yaw, pitch, weaponState.LastSpreadX,
                weaponState.LastSpreadY);

            return dir.normalized;
        }

        protected Vector3 CalculateShotingDir(int seed, float yaw, float pitch, float spreadX, float spreadY)
        {
            Quaternion q = Quaternion.Euler(pitch, yaw, 0);
            Vector3 forward = q.Forward();
            Vector3 right = q.Right();
            Vector3 up = q.Up();

            float x;
            float y;
            x = (float)UniformRandom.RandomFloat(seed + 0, -0.5, 0.5) +
                (float)UniformRandom.RandomFloat(seed + 1, -0.5, 0.5);
            y = (float)UniformRandom.RandomFloat(seed + 2, -0.5, 0.5) +
                (float)UniformRandom.RandomFloat(seed + 3, -0.5, 0.5);
            float res1 = spreadX * x;
            float res2 = spreadY * y;
            right = Vector3Ext.Scale(right, res1);
            up = Vector3Ext.Scale(up, res2);
            var newForward = Vector3Ext.Add(forward, right);
            newForward = Vector3Ext.Add(newForward, up);
            return newForward;
        }

        public virtual Vector3 GetFireEmitPosition(PlayerWeaponController controller)
        {

            var firePosition = controller.RelatedFirePos;
            if (firePosition == null)
                return Vector3.zero;
            var muzzleP3LocatorPosition = firePosition.MuzzleP3Valid ? firePosition.MuzzleP3Position : (Vector3?)null;
            if (muzzleP3LocatorPosition.HasValue)
            {
                return muzzleP3LocatorPosition.Value;
            }
            return GetFireViewPosition(controller);
        }

        public virtual Vector3 GetFireViewPosition(PlayerWeaponController controller)
        {
            var outputView = controller.RelatedCameraFinal;
            if (outputView == null)
            {
                LogError("controller has no cameraFinalOutputNew");
                return Vector3.zero;
            }
            var cameraAngle = outputView.EulerAngle;
            //var matrix = Matrix4x4.TRS(BulletEmittorOffset, Quaternion.Euler(cameraAngle), Vector3.one);
            //var offset = matrix.ExtractPosition();
            return outputView.Position;
            //return offset + outputView.Position;
        }

        private void LogError(string msg)
        {
            Logger.Error(msg);
            System.Console.WriteLine(msg);
        }
    }
}
