using App.Shared.Components;
using App.Shared.Util;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

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
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var controller = playerEntity.WeaponController();

            var dataList = controller.BulletList;
            if (dataList == null || dataList.Count == 0)
                return;
            BulletConfig bulletConfig = controller.HeldWeaponAgent.BulletCfg;
            if (null == bulletConfig)
                return;
            int weaponConfigId = controller.HeldWeaponAgent.ConfigId;
            var caliber = (EBulletCaliber) UserWeaponConfigManagement.FindConfigById(weaponConfigId).NewWeaponCfg.Caliber;

            var heldAgent = controller.HeldWeaponAgent;
            var damageBuff = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.BaseDamage);
            var speedBuff = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.EmitVelocity);
            var decayBuff = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.DistanceDecay);

            foreach (var bulletData in dataList)
            {
                int bulletEntityId = _entityIdGenerator.GetNextEntityId();
                Vector3 velocity = bulletData.Dir * bulletConfig.EmitVelocity * (1 + speedBuff / 100);
                var bulletEntity = _bulletContext.CreateEntity();
                float distanceDecay = bulletConfig.DistanceDecayFactor * (1 + decayBuff / 100);

                bulletEntity.AddEntityKey(new EntityKey(bulletEntityId, (int) EEntityType.Bullet));
                bulletEntity.AddBulletData(velocity, 0, bulletConfig.Gravity, 0, cmd.RenderTime, bulletConfig.MaxDistance * (1 + decayBuff / 100),
                    bulletConfig.PenetrableLayerCount, bulletConfig.BaseDamage + damageBuff, bulletConfig.PenetrableThickness,
                    bulletConfig, bulletConfig.VelocityDecay * (1 + decayBuff / 100), caliber, weaponConfigId, distanceDecay > 0.99f ? 0.99f : distanceDecay);
                bulletEntity.AddPosition();
                bulletEntity.position.Value = bulletData.ViewPosition;
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

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.WeaponController().BulletList != null;
        }
    }
}