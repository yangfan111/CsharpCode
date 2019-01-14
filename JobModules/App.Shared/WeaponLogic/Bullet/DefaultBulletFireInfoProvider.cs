using Core.Utils;
using Core.WeaponLogic;
using Core.WeaponLogic.Bullet;
using UnityEngine;

namespace App.Shared.WeaponLogic.Bullet
{
    public class DefaultBulletFireInfoProvider : IBulletFireInfoProvider
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultBulletFireInfoProvider));
        protected IPlayerWeaponState _playerWeaponState;
        private static GameObject _go;
        private static GameObject go
        {
            get
            {
                if (_go == null)
                {
                    _go = new GameObject();
                    _go.name = "PlayerEntityUtil";
                }

                return _go;
            }
        }
        public static Vector3 BulletEmittorOffset = new Vector3(0, 0, 0.1f);

        public DefaultBulletFireInfoProvider(IPlayerWeaponState playerWeaponState)
        {
            _playerWeaponState = playerWeaponState;
        }

        public virtual Vector3 GetFireDir(int seed)
        {
            var yaw = _playerWeaponState.ViewYaw - _playerWeaponState.NegPunchYaw * 2;
            var pitch = _playerWeaponState.ViewPitch - _playerWeaponState.NegPunchPitch * 2;
            var dir = CalculateShotingDir(seed, yaw, pitch, _playerWeaponState.LastSpreadX,
                _playerWeaponState.LastSpreadY);

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
            //,z;
            x = (float) UniformRandom.RandomFloat(seed + 0, -0.5, 0.5) +
                (float) UniformRandom.RandomFloat(seed + 1, -0.5, 0.5);
            y = (float) UniformRandom.RandomFloat(seed + 2, -0.5, 0.5) +
                (float) UniformRandom.RandomFloat(seed + 3, -0.5, 0.5);
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

        public virtual Vector3 GetFireEmitPosition()
        {
            if(_playerWeaponState.MuzzleP3LocatorPosition.HasValue)
            {
                return _playerWeaponState.MuzzleP3LocatorPosition.Value;
            }
            Logger.Error("no muzzle locator in weapon and part");
            return GetFireViewPosition();
        }

        public virtual Vector3 GetFireViewPosition()
        {
            go.transform.rotation = Quaternion.Euler(_playerWeaponState.CameraAngle);
            go.transform.position = _playerWeaponState.CameraPosition; 
            return go.transform.TransformPoint(BulletEmittorOffset);
        }
    }
}
