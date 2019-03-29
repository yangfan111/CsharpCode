using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="BulletFireInfoProvider" />
    /// </summary>
    public class BulletFireInfoProvider
    {
        private IBulletFireInfo _sightBulletFireInfo;

        private IBulletFireInfo _defaultBulletFireInfo;

        private IBulletFireInfo _currentBulletFireInfo;

        public BulletFireInfoProvider()
        {
            _sightBulletFireInfo = new SightBulletFireInfo();
            _defaultBulletFireInfo = new DefaultBulletFireInfo();
            _currentBulletFireInfo = _defaultBulletFireInfo;
        }

        public void Prepare(PlayerWeaponController controller)
        {
            if (controller.RelatedCameraSNew.IsAiming())
            {
                _currentBulletFireInfo = _sightBulletFireInfo;
            }
            else
            {
                _currentBulletFireInfo = _defaultBulletFireInfo;
            }
        }

        public Vector3 GetFireDir(int seed, PlayerWeaponController controller)
        {
            return _currentBulletFireInfo.GetFireDir(seed, controller);
        }

        public Vector3 GetFireEmitPosition(PlayerWeaponController controller)
        {
            return _currentBulletFireInfo.GetFireEmitPosition(controller);
        }

        public Vector3 GetFireViewPosition(PlayerWeaponController controller)
        {
            return _currentBulletFireInfo.GetFireViewPosition(controller);
        }
    }
}
