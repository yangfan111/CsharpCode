using Core.Utils;
using Core.WeaponLogic;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Bullet
{
    public class SightBulletFireInfoProvider : DefaultBulletFireInfoProvider
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SightBulletFireInfoProvider));
        public SightBulletFireInfoProvider(IPlayerWeaponState playerWeaponState):base(playerWeaponState)
        {
            
        }

        public override Vector3 GetFireDir(int seed)
        {
            var pos = GetFireViewPosition();
            var delta = (pos - _playerWeaponState.CameraPosition);
            var yaw = delta.GetYaw();
            var pitch = delta.GetPitch();
            return CalculateShotingDir(seed, yaw, pitch, _playerWeaponState.LastSpreadX,
                _playerWeaponState.LastSpreadY);
        }

        public override Vector3 GetFireViewPosition()
        {
            var sightLocationP1 = _playerWeaponState.GetSightFireViewPosition;
            if (sightLocationP1.HasValue)
            {
                //Logger.InfoFormat("get fire view position:{0}", sightLocationP1.Value.ToStringExt());
                return sightLocationP1.Value;
            }
            else
            {
                return base.GetFireViewPosition();
            }
        }
    }
}
