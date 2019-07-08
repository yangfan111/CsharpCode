using System;
using App.Shared.EntityFactory;
using App.Shared.Util;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerBulletGenerateSystem : AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerBulletGenerateSystem));

        public PlayerBulletGenerateSystem(Contexts contexts)
        {
            BulletEntityFactory.bulletContext     = contexts.bullet;
            BulletEntityFactory.entityIdGenerator = contexts.session.commonSession.EntityIdGenerator;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var          controller   = playerEntity.WeaponController();
            var          dataList     = controller.BulletList;
            var          heldAgent    = controller.HeldWeaponAgent;
            BulletConfig bulletConfig = heldAgent.WeaponConfigAssy.S_BulletCfg;
            if (bulletConfig == null)
                return;
            int weaponConfigId = heldAgent.ConfigId;
            var caliber        = (EBulletCaliber) heldAgent.WeaponConfigAssy.NewWeaponCfg.Caliber;

            var          damageBuff = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.BaseDamage);
            var          speedBuff  = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.EmitVelocity);
            var          decayBuff  = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.DistanceDecay);
            BulletEntity bulletEntity;
            foreach (var bulletData in dataList)
            {
                bulletEntity = BulletEntityFactory.CreateBulletEntity(bulletData, playerEntity.entityKey.Value);
                float distanceDecay = bulletConfig.DistanceDecayFactor * (1 + decayBuff / 100);
                float velocityDecay = bulletConfig.VelocityDecay * (1 + decayBuff / 100);
                float maxDistance   = bulletConfig.MaxDistance * (1 + decayBuff / 100);


                var entityBulletData = bulletEntity.bulletData;
                entityBulletData.Velocity = bulletConfig.EmitVelocity * (1 + speedBuff / 100) * bulletData.Dir;
                entityBulletData.DistanceDecay = Math.Min(0.99f, distanceDecay.FloatPrecision(2));
                entityBulletData.WeaponId      = weaponConfigId;
                entityBulletData.Caliber       = caliber;
                entityBulletData.ServerTime    = cmd.RenderTime;
                entityBulletData.Gravity = bulletConfig.Gravity;

                entityBulletData.VelocityDecay       = velocityDecay.FloatPrecision(2);
                entityBulletData.DefaultBulletConfig = bulletConfig;
                entityBulletData.IsAimShoot = controller.RelatedCameraSNew.IsAiming();
                entityBulletData.PenetrableThickness              = bulletConfig.PenetrableThickness;
                entityBulletData.BaseDamage                       = bulletConfig.BaseDamage + damageBuff;
                entityBulletData.PenetrableLayerCount             = bulletConfig.PenetrableLayerCount;
                entityBulletData.MaxDistance                      = maxDistance.FloatPrecision(2);
                entityBulletData.CmdSeq                           = cmd.Seq;
                entityBulletData.StatisticsInfo.cmdSeq            = cmd.Seq;
                entityBulletData.StatisticsInfo.originStr         = bulletData.ToStringExt();
                entityBulletData.StatisticsInfo.bulletBaseStr     = entityBulletData.ToBaseString();
                entityBulletData.StatisticsInfo.bulletRunStartStr = entityBulletData.ToDynamicString();


                DebugUtil.AppendShootText(cmd.Seq, "[Bullet Gen]BulletEnity :{0}", entityBulletData);
            }

            dataList.Clear();
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.playerWeaponAuxiliary.BulletList != null && entity.playerWeaponAuxiliary.BulletList.Count > 0;
        }
    }
}