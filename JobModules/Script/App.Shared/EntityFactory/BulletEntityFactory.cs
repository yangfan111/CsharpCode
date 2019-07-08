using App.Shared.Components;
using Core.EntityComponent;
using Core.Utils;
using Entitas;
using System;
using App.Shared.Components.Player;
using App.Shared.GameModules.Attack;
using Core.Attack;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.EntityFactory
{
    public class BulletEntityFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletEntityFactory));
        public static BulletContext bulletContext {private get; set;  }
        public static IEntityIdGenerator entityIdGenerator { private get; set; }

        public static BulletEntity CreateBulletEntity(PlayerBulletData bulletData, EntityKey owner)
        {
            int     bulletEntityId = entityIdGenerator.GetNextEntityId();
            var     bulletEntity   = bulletContext.CreateEntity();

            bulletEntity.AddEntityKey(new EntityKey(bulletEntityId, (int) EEntityType.Bullet));
            bulletEntity.AddPosition();
            bulletEntity.position.Value = bulletData.ViewPosition;
            bulletEntity.AddOwnerId(owner);
            bulletEntity.AddBulletData();
            bulletEntity.bulletData.StartPoint = bulletData.ViewPosition;
            bulletEntity.bulletData.EmitPoint  = bulletData.EmitPosition;
            bulletEntity.bulletData.StartDir   = bulletData.Dir;
            bulletEntity.isFlagSyncNonSelf     = true;
            bulletEntity.AddLifeTime(DateTime.Now, SharedConfig.BulletLifeTime); // in case user logout
            IBulletEntityAgent entityAgent = new BulletEntityAgent(bulletEntity);
            bulletEntity.AddBulletRuntime(entityAgent,true);
            bulletData.ReleaseReference();
            return bulletEntity;
        }
    }
}