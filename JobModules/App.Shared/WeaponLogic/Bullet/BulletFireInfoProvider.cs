using Core.WeaponLogic;
using Core.WeaponLogic.Bullet;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Bullet
{
    public class BulletFireInfoProviderDispatcher : IBulletFireInfoProviderDispatcher
    {
        private IPlayerWeaponState _playerWeaponState;
        private IBulletFireInfoProvider _sightBulletFireInfoProvider;
        private IBulletFireInfoProvider _defaultBulletFireInfoProvider;
        private IBulletFireInfoProvider _currentBulletFireInfoProvider;
        public BulletFireInfoProviderDispatcher(IPlayerWeaponState playerWeaponState)
        {
            _playerWeaponState = playerWeaponState;
            _sightBulletFireInfoProvider = new SightBulletFireInfoProvider(playerWeaponState);
            _defaultBulletFireInfoProvider = new DefaultBulletFireInfoProvider(playerWeaponState);
            _currentBulletFireInfoProvider = _defaultBulletFireInfoProvider;
        }

        public void Prepare()
        {
            if(_playerWeaponState.IsAiming)
            {
                _currentBulletFireInfoProvider = _sightBulletFireInfoProvider; 
            }
            else
            {
                _currentBulletFireInfoProvider = _defaultBulletFireInfoProvider;
            }
        }

        public Vector3 GetFireDir(int seed)
        {
            return _currentBulletFireInfoProvider.GetFireDir(seed);
        }

        public Vector3 GetFireEmitPosition()
        {
            return _currentBulletFireInfoProvider.GetFireEmitPosition();
        }

        public Vector3 GetFireViewPosition()
        {
            return _currentBulletFireInfoProvider.GetFireViewPosition();
        }
    }
}
