using App.Shared.Components;
using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using App.Shared.WeaponLogic;
using Assets.Utils.Configuration;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using System;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.GameModules.Player
{
    public class PlayerBulletGenerateSystem : AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerBulletGenerateSystem));
        private BulletContext _bulletContext;
        private IEntityIdGenerator _entityIdGenerator;
        private IWeaponConfigManager _weaponConfigManager;
        private Contexts _contexts;
 
        public PlayerBulletGenerateSystem(Contexts contexts, IWeaponConfigManager weaponConfigManager)
        {
            _contexts = contexts;
            _bulletContext = contexts.bullet;
            _entityIdGenerator = contexts.session.commonSession.EntityIdGenerator;
            _weaponConfigManager = weaponConfigManager;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var dataList = playerEntity.playerBulletData.DataList;
            if(dataList.Count > 0)
            {
                var weaponDataConfig = playerEntity.GetWeaponConfig(_contexts);
                if(null == weaponDataConfig)
                {
                    Logger.Error("weapon config is null ");
                    return;
                }
                var bulletConfig = weaponDataConfig.BulletCfg;
                var weaponId = playerEntity.GetController<PlayerWeaponController>().CurrSlotWeaponId(_contexts);
                if(!weaponId.HasValue)
                {
                    return;
                }
                var config = _weaponConfigManager.GetConfigById(weaponId.Value);
                if(null == config)
                {
                    return;
                }
                var caliber = (EBulletCaliber)config.Caliber;

                foreach (var bulletData in dataList)
                {
                    int bulletEntityId = _entityIdGenerator.GetNextEntityId();
        
                    Vector3 velocity = bulletData.Dir * bulletConfig.EmitVelocity;
                    var bulletEntity = _bulletContext.CreateEntity();
                    float maxDistance = bulletConfig.MaxDistance;
                    bulletEntity.AddEntityKey(new EntityKey(bulletEntityId, (int)EEntityType.Bullet));

                    bulletEntity.AddBulletData(
                        velocity,
                        0,
                        bulletConfig.Gravity,
                        0,
                        cmd.RenderTime,
                        maxDistance,
                        bulletConfig.PenetrableLayerCount,
                        bulletConfig.BaseDamage,
                        bulletConfig.PenetrableThickness,
                        bulletConfig,
                        bulletConfig.VelocityDecay,
                        caliber,
                        weaponId.Value);
                    bulletEntity.AddPosition(bulletData.ViewPosition);
                    bulletEntity.AddOwnerId(playerEntity.entityKey.Value);
                    bulletEntity.bulletData.CmdSeq = cmd.Seq;
                    bulletEntity.bulletData.StartPoint = bulletData.ViewPosition;
                    bulletEntity.bulletData.EmitPoint = bulletData.EmitPosition;
                    bulletEntity.bulletData.StartDir = bulletData.Dir;
                    bulletEntity.isNew = true;
                    bulletEntity.AddEmitPosition(bulletData.EmitPosition);
                    bulletEntity.isFlagSyncNonSelf = true;
                    bulletEntity.AddLifeTime(DateTime.Now, SharedConfig.BulletLifeTime); // in case user logout
                    bulletData.ReleaseReference();
                }
                dataList.Clear();
            }
        }

        protected override bool filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasPlayerBulletData;
        }
    }
}
