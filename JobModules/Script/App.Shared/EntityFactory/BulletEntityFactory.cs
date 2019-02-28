using System;
using App.Shared.Components;
using Core.EntityComponent;
using Core.Utils;
using Entitas;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.EntityFactory
{
    public class BulletEntityFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletEntityFactory));
        private BulletContext _bulletContext;
        private IEntityIdGenerator _entityIdGenerator;
        public BulletEntityFactory(BulletContext bulletContext, IEntityIdGenerator entityIdGenerator)
        {
            _bulletContext = bulletContext;
            _entityIdGenerator = entityIdGenerator;
        }

        public Entity CreateBulletEntity(
            int cmdSeq,
            EntityKey entityKey,
            int serverTime,
            Vector3 dir,
            int weaponId,
            EBulletCaliber caliber,
            BulletConfig bulletConfig,
            Vector3 viewPosition,
            Vector3 emitPosition)
        {
            int bulletEntityId = _entityIdGenerator.GetNextEntityId();
        
            Vector3 velocity = dir * bulletConfig.EmitVelocity;
            var bulletEntity = _bulletContext.CreateEntity();
            float maxDistance = bulletConfig.MaxDistance;
            bulletEntity.AddEntityKey(new EntityKey(bulletEntityId, (int)EEntityType.Bullet));

            bulletEntity.AddBulletData(
                velocity, 
                0,
                bulletConfig.Gravity, 
                0, 
                serverTime, 
                maxDistance, 
                bulletConfig.PenetrableLayerCount, 
                bulletConfig.BaseDamage, 
                bulletConfig.PenetrableThickness, 
                bulletConfig,
                bulletConfig.VelocityDecay,
                caliber,
                weaponId);
            bulletEntity.AddPosition(viewPosition);
            bulletEntity.AddOwnerId(entityKey);
            bulletEntity.bulletData.CmdSeq = cmdSeq;
            bulletEntity.bulletData.StartPoint = viewPosition;
            bulletEntity.bulletData.EmitPoint = emitPosition;
            bulletEntity.bulletData.StartDir = dir;
            bulletEntity.isNew = true;
            bulletEntity.AddEmitPosition(emitPosition);
            bulletEntity.isFlagSyncNonSelf = true;
            bulletEntity.AddLifeTime(DateTime.Now, SharedConfig.BulletLifeTime); // in case user logout
            return bulletEntity;
        }
    }
}