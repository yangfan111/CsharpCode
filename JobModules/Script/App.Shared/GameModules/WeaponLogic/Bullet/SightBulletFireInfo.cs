using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SightBulletFireInfo" />
    /// </summary>
    public class SightBulletFireInfo : DefaultBulletFireInfo
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SightBulletFireInfo));

        public override Vector3 GetFireDir(int seed, PlayerWeaponController controller)
        {
            var delta = GetFireViewPosition(controller) - controller.RelatedCameraFinal.Position;
            var weaponData = controller.HeldWeaponAgent.RunTimeComponent;
            return CalculateShotingDir(seed,
                delta.GetYaw(),
                delta.GetPitch(),
                weaponData.LastSpreadX,
                weaponData.LastSpreadY);
        }

        public override Vector3 GetFireViewPosition(PlayerWeaponController controller)
        {
            var gunSightFireViewPosition = controller.RelatedFirePos.SightValid ? controller.RelatedFirePos.SightPosition : (Vector3?)null;
            if (gunSightFireViewPosition.HasValue)
            {
                return gunSightFireViewPosition.Value;
            }
            else
            {
                return base.GetFireViewPosition(controller);
            }
        }
    }
}
