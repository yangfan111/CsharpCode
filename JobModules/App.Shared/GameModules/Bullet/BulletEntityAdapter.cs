
using System.Collections.Generic;
using App.Shared.GameModules.Player;
using Core.BulletSimulation;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.GameModules.Bullet
{
    public class BulletEntityAdapter : IBulletEntity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletEntityAdapter));
        private BulletEntity _bulletEntity;
        private PlayerContext _playerContext;

        public BulletEntityAdapter(BulletEntity bulletEntity, PlayerContext playerContext)
        {
            _bulletEntity = bulletEntity;
            _playerContext = playerContext;
        }

        public EntityKey OwnerEntityKey { get { return _bulletEntity.ownerId.Value; } }
        public Vector3 Origin { get { return _bulletEntity.position.Value; } set { _bulletEntity.position.Value = value; } }
        public Vector3 Velocity { get { return _bulletEntity.bulletData.Velocity; } set { _bulletEntity.bulletData.Velocity = value; } }
        public float Gravity { get { return _bulletEntity.bulletData.Gravity; } set { _bulletEntity.bulletData.Gravity = value; } }
        public float VelocityDecay { get { return _bulletEntity.bulletData.VelocityDecay; } set { _bulletEntity.bulletData.VelocityDecay = value; } }
        public int NextFrameTime { get { return _bulletEntity.bulletData.RemainFrameTime; } set { _bulletEntity.bulletData.RemainFrameTime = value; } }
        public int ServerTime { get { return _bulletEntity.bulletData.ServerTime; } set { _bulletEntity.bulletData.ServerTime = value; } }
        public bool IsValid { get { return !_bulletEntity.isFlagDestroy; } set { _bulletEntity.isFlagDestroy = !value; } }
        public List<int> ExcludePlayerList
        {
            get
            {
                var ownerEntity = _playerContext.GetEntityWithEntityKey(_bulletEntity.ownerId.Value);
                return ownerEntity.playerHitMaskController.HitMaskController.BulletExcludeTargetList;
            }
        }

        public float Distance
        {
            get
            {
                return _bulletEntity.bulletData.Distance;
            }
            set
            {
                _bulletEntity.bulletData.Distance = value;

                if (_bulletEntity.bulletData.Distance > _bulletEntity.bulletData.MaxDistance)
                {
                    _bulletEntity.isFlagDestroy = true;
                }
            }
        }


        public float DistanceDecayFactor { get { return _bulletEntity.bulletData.DefaultBulletConfig.DistanceDecayFactor; } }
        public float BaseDamage { get { return _bulletEntity.bulletData.BaseDamage; } set { _bulletEntity.bulletData.BaseDamage = value; } }
        public float PenetrableThickness { get { return _bulletEntity.bulletData.PenetrableThickness; } set { _bulletEntity.bulletData.PenetrableThickness = value; } }

        public float GetDamageFactor(EBodyPart part)
        {
            var configs = _bulletEntity.bulletData.DefaultBulletConfig.BodyDamages;
            for (int i = 0; i < configs.Length; i++)
            {
                if (configs[i].BodyPart == part)
                    return configs[i].Factor;
            }

            return 1;
        }

        public void AddPenetrateInfo(EEnvironmentType type)
        {
            if(!_bulletEntity.hasPenetrateInfo)
            {
                _bulletEntity.AddPenetrateInfo(new System.Collections.Generic.List<EEnvironmentType>());
            }
            _bulletEntity.penetrateInfo.Layers.Add(type);
        }

        public int PenetrableLayerCount { get { return _bulletEntity.bulletData.PenetrableLayerCount; } set { _bulletEntity.bulletData.PenetrableLayerCount = value; } }
        public EBulletCaliber Caliber { get { return _bulletEntity.bulletData.Caliber; } }
        public int WeaponId { get { return _bulletEntity.bulletData.WeaponId; } }

        public Vector3 GunEmitPosition
        {
            get
            {
                return _bulletEntity.emitPosition.Value;
            }
        }

        public bool IsNew
        {
            get
            {
                return _bulletEntity.isNew;
            }
            set
            {
                _bulletEntity.isNew = value;
            }
        }

        public bool IsOverWall
        {
            get
            {
                if (!_bulletEntity.hasPenetrateInfo)
                {
                    return false;
                }
                for(int i = 0; i < _bulletEntity.penetrateInfo.Layers.Count; i++)
                {
                    if(_bulletEntity.penetrateInfo.Layers[i] == EEnvironmentType.Concrete)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Vector3 HitPoint
        {
            get
            {
                return _bulletEntity.bulletData.HitPoint;
            }
            set
            {
                _bulletEntity.bulletData.HitPoint = value;
            }
        }

        public EHitType HitType
        {
            get
            {
                return _bulletEntity.bulletData.HitType;
            }
            set
            {
                _bulletEntity.bulletData.HitType = value;
            }
        } 
    }
}