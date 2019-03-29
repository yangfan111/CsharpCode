using App.Shared.Components;
using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

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
        private Contexts _contexts;
 
        public PlayerBulletGenerateSystem(Contexts contexts)
        {
            _contexts = contexts;
            _bulletContext = contexts.bullet;
            _entityIdGenerator = contexts.session.commonSession.EntityIdGenerator;
         //    SingletonManager.Get<WeaponResourceConfigManager>() = weaponConfigManager;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var controller = playerEntity.WeaponController();
            var dataList = controller.BulletList;
            if(dataList != null && dataList.Count > 0)
            {
                BulletConfig bulletConfig = controller.HeldWeaponAgent.BulletCfg;
                if (null == bulletConfig)
                {
                    return;
                }
                int weaponConfigId = controller.HeldWeaponAgent.ConfigId;
                var caliber = (EBulletCaliber)UserWeaponConfigManagement.FindConfigById(weaponConfigId).NewWeaponCfg.Caliber;

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
                        weaponConfigId);
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

        protected override bool filter(PlayerEntity entity)
        {
            return entity.WeaponController().BulletList != null;
        }
    }
}
