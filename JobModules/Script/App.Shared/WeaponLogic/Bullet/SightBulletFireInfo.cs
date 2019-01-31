using App.Shared.GameModules.Weapon.Bullet;
using Core.Utils;
using UnityEngine;

namespace App.Shared.WeaponLogic.Bullet
{
    public class SightBulletFireInfo : DefaultBulletFireInfo
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SightBulletFireInfo));

        public override Vector3 GetFireDir(int seed, PlayerEntity playerEntity, WeaponEntity weaponEntity, Contexts contexts)
        {
            var delta = GetFireViewPosition(playerEntity, contexts) - playerEntity.cameraFinalOutputNew.Position;
            var weaponData = weaponEntity.weaponRuntimeInfo;
            return CalculateShotingDir(seed, 
                delta.GetYaw(), 
                delta.GetPitch(), 
                weaponData.LastSpreadX,
                weaponData.LastSpreadY);
        }

        public override Vector3 GetFireViewPosition(PlayerEntity playerEntity, Contexts contexts)
        {
            var gunSightFireViewPosition = playerEntity.firePosition.SightValid ? playerEntity.firePosition.SightPosition : (Vector3?)null;
            if (gunSightFireViewPosition.HasValue)
            {
                return gunSightFireViewPosition.Value;
            }
            else
            {
                return base.GetFireViewPosition(playerEntity, contexts);
            }
        }
    }
}