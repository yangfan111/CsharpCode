using System;
using System.Text;
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
        private readonly StringBuilder sb = new StringBuilder();
        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var          controller   = playerEntity.WeaponController();
            var          attackProxy  = playerEntity.WeaponController().AttackProxy;
            var          dataList     = controller.BulletList;
            BulletConfig bulletConfig = attackProxy.WeaponConfigAssy.S_BulletCfg;
            if (bulletConfig == null)
                return;
            int weaponConfigId = attackProxy.WeaponConfigAssy.S_Id;
            var caliber        = (EBulletCaliber) attackProxy.WeaponConfigAssy.NewWeaponCfg.Caliber;

            var          damageBuff = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.BaseDamage);
            var          speedBuff  = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.EmitVelocity);
            var          decayBuff  = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.DistanceDecay);
            BulletEntity bulletEntity;
            foreach (var bulletData in dataList)
            {
                bulletEntity = BulletEntityFactory.CreateBulletEntity(bulletData, playerEntity.entityKey.Value);
                float distanceDecay = bulletConfig.DistanceDecayFactor * (1 + decayBuff / 100);
                float velocityDecay = bulletConfig.VelocityDecay * (1 + decayBuff / 100);
                float maxDistance   = bulletConfig.MaxDistance * (1 + decayBuff / 100);


                var entityBulletData = bulletEntity.bulletData;
                entityBulletData.Velocity      = bulletConfig.EmitVelocity * (1 + speedBuff / 100) * bulletData.Dir;
                entityBulletData.DistanceDecay = Math.Min(0.99f, distanceDecay.FloatPrecision(2));
                entityBulletData.WeaponId      = weaponConfigId;
                entityBulletData.Caliber       = caliber;
                entityBulletData.ServerTime    = cmd.RenderTime;
                entityBulletData.Gravity       = bulletConfig.Gravity;

                entityBulletData.VelocityDecay                    = velocityDecay.FloatPrecision(2);
                entityBulletData.DefaultBulletConfig              = bulletConfig;
                entityBulletData.IsAimShoot                       = attackProxy.IsAiming;
                entityBulletData.PenetrableThickness              = bulletConfig.PenetrableThickness;
                entityBulletData.BaseDamage                       = bulletConfig.BaseDamage + damageBuff;
                entityBulletData.PenetrableLayerCount             = bulletConfig.PenetrableLayerCount;
                entityBulletData.MaxDistance                      = maxDistance.FloatPrecision(2);
                entityBulletData.CmdSeq                           = cmd.Seq;
                entityBulletData.StatisticsInfo.cmdSeq            = cmd.Seq;
                // entityBulletData.StatisticsInfo.originStr         = bulletData.ToStringExt(sb);
                // entityBulletData.StatisticsInfo.bulletBaseStr     = entityBulletData.ToBaseString(sb);
                // entityBulletData.StatisticsInfo.bulletRunStartStr = entityBulletData.ToDynamicString(sb);


                //DebugUtil.AppendShootText(cmd.Seq, "[Bullet Gen]BulletEnity :{0}", entityBulletData);
            }

            dataList.Clear();
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.playerWeaponAuxiliary.BulletList != null && entity.playerWeaponAuxiliary.BulletList.Count > 0;
        }
    }
}