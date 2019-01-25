using App.Shared.GameModules.Camera.Utils;
using App.Shared.WeaponLogic.Bullet;
using Core.WeaponLogic.Bullet;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Bullet
{
    public class BulletFireInfoProvider
    {
        private IBulletFireInfo _sightBulletFireInfo;
        private IBulletFireInfo _defaultBulletFireInfo;
        private IBulletFireInfo _currentBulletFireInfo;
        private Contexts _contexts;

        public BulletFireInfoProvider(Contexts contexts)
        {
            _sightBulletFireInfo = new SightBulletFireInfo();
            _defaultBulletFireInfo = new DefaultBulletFireInfo();
            _currentBulletFireInfo = _defaultBulletFireInfo;
            _contexts = contexts;
        }

        public void Prepare(PlayerEntity playerEntity)
        {
            if(playerEntity.IsAiming())
            {
                _currentBulletFireInfo = _sightBulletFireInfo; 
            }
            else
            {
                _currentBulletFireInfo = _defaultBulletFireInfo;
            }
        }

        public Vector3 GetFireDir(int seed, PlayerEntity playerEntity, WeaponEntity weaponEntity)
        {
            return _currentBulletFireInfo.GetFireDir(seed, playerEntity, weaponEntity, _contexts);
        }

        public Vector3 GetFireEmitPosition(PlayerEntity playerEntity)
        {
            return _currentBulletFireInfo.GetFireEmitPosition(playerEntity, _contexts);
        }

        public Vector3 GetFireViewPosition(PlayerEntity playerEntity)
        {
            return _currentBulletFireInfo.GetFireViewPosition(playerEntity, _contexts);
        }
    }
}
