using System;
using App.Shared.Components;
using App.Shared.GameModules.Player;
using Assets.XmlConfig;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.EntityFactory
{
    public class ThrowingEntityFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingEntityFactory));
        public static ThrowingEntity CreateThrowingEntity(
            ThrowingContext throwingContext,
            IEntityIdGenerator entityIdGenerator,
            PlayerEntity playerEntity,
            int serverTime, Vector3 dir, float initVel,
            NewWeaponConfigItem newWeaponConfig,
            ThrowingConfig throwingConfig)
        {
            int throwingEntityId = entityIdGenerator.GetNextEntityId();

            var emitPost = PlayerEntityUtility.GetThrowingEmitPosition(playerEntity);
            Vector3 velocity = dir * initVel;
            var throwingEntity = throwingContext.CreateEntity();

            throwingEntity.AddEntityKey(new EntityKey(throwingEntityId, (int)EEntityType.Throwing));

            throwingEntity.AddThrowingData(
                velocity,
                false,
                false,
                0,
                serverTime,
                false,
                initVel,
                throwingConfig,
                newWeaponConfig.SubType
            );

            throwingEntity.AddPosition(emitPost);
            throwingEntity.AddOwnerId(playerEntity.entityKey.Value);
            throwingEntity.isFlagSyncNonSelf = true;
            throwingEntity.AddLifeTime(DateTime.Now, throwingConfig.CountdownTime + 2000);
            return throwingEntity;
        }
    }
}