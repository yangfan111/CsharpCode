using System;
using App.Shared.Components;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.EntityFactory
{
    public class ThrowingEntityFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingEntityFactory));

        public static ThrowingContext    ThrowingContext   { set; private get; }

        public static ThrowingEntity GetEntityWithEntityKey(EntityKey entityKey)
        {
            return ThrowingContext.GetEntityWithEntityKey(entityKey);
        }

        public static IEntityIdGenerator EntityIdGenerator { set; private get; }

        public static ThrowingEntity CreateThrowingEntity(PlayerWeaponController controller, int serverTime,
                                                          Vector3 dir,Vector3 viePos, float initVel,
                                                          WeaponResConfigItem newWeaponConfig,
                                                          ThrowingConfig throwingConfig)
        {
            int throwingEntityId = EntityIdGenerator.GetNextEntityId();

         
            Vector3 velocity       = dir * initVel;
            ThrowingEntity     throwingEntity = ThrowingContext.CreateEntity();

            throwingEntity.AddEntityKey(new EntityKey(throwingEntityId, (int) EEntityType.Throwing));
            // throwingEntity.AddThrowingData(velocity, false, false, 0, serverTime, false, initVel, throwingConfig,
            //     newWeaponConfig.SubType);
            throwingEntity.AddThrowingData(throwingConfig,newWeaponConfig,initVel,0,serverTime,velocity,newWeaponConfig.SubType);
            throwingEntity.AddPosition();
            //TODO:EmitPos,ViewPos做射线检测取合适的点
            throwingEntity.position.Value = viePos;
            throwingEntity.AddOwnerId(controller.Owner);
            throwingEntity.isFlagSyncNonSelf = true;
            throwingEntity.AddLifeTime(DateTime.Now, throwingConfig.CountdownTime + 2000);
            _logger.InfoFormat("CreateThrowing from {0} with velocity {1}, entity key {2}",
                throwingEntity.position.Value, throwingEntity.throwingData.Velocity, throwingEntity.entityKey);
            return throwingEntity;
        }

        public static void StartThrowingEntityFly(EntityKey entityKey, bool isThrow, float initVel)
        {
            ThrowingEntity throwing = ThrowingContext.GetEntityWithEntityKey(entityKey);
            if (null != throwing)
            {
                throwing.throwingData.IsThrow      = isThrow;
                throwing.throwingData.InitVelocity = initVel;
            }
        }
        public static void DestroyThrowing(EntityKey entityKey)
        {
            ThrowingEntity throwing = ThrowingContext.GetEntityWithEntityKey(entityKey);
            if (null != throwing)
            {
                throwing.isFlagDestroy = true;
            }
        }
    }
}