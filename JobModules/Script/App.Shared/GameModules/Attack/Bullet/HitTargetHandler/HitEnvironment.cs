using System;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core;
using Core.Attack;
using Core.Compensation;
using Core.HitBox;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Attack
{
    public class HitEnvironmentHandler:AbstractHitHandler
    {
        public HitEnvironmentHandler(Contexts contexts) : base(contexts)
        {
            profilerInfo  = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BulletHitHandler_OnHitEnvironment");
        }
        protected override bool Filter(HitBoxOwnerComponent boxOwnerComponent, int cmdSeq)
        {
            return true;
        }

        protected override void Handle(CompensationWorld compensationWorld, IBulletEntityAgent bulletEntityAgent, RaycastHit hit,
                                       PlayerEntity srcPlayer)
        {
            DoHitEnvironment(srcPlayer, bulletEntityAgent, hit);
             bulletEntityAgent.HitType = EHitType.Environment;

            
        }
        private void DoHitEnvironment(PlayerEntity srcPlayer, IBulletEntityAgent bulletEntityAgent, RaycastHit hit)
        {
            if (srcPlayer.gamePlay.IsDead())
            {
                BulletHitHandler._logger.InfoFormat("hit environment dead");
                return;
            }

            BulletHitHandler._logger.InfoFormat("[Hit{0}] OwnerEntityKey {1}, point {2},collider:{3}", cmdSeq,
                bulletEntityAgent.OwnerEntityKey, hit.point, hit.collider.name);
            ThicknessInfo thicknessInfo;
            EnvironmentInfo info =
                            BulletEnvironmentUtil.GetEnvironmentInfoByHitBoxName(hit, bulletEntityAgent.Velocity,
                            out thicknessInfo);
            float damageDecayFactor = SingletonManager.Get<EnvironmentTypeConfigManager>().GetDamageDecayFactorByEnvironmentType(info.Type);
            float energyDecayFactor = SingletonManager.Get<EnvironmentTypeConfigManager>().GetEnergyDecayFactorByEnvironmentType(info.Type);
            float oldThickNess      = bulletEntityAgent.PenetrableThickness;
            float oldDamage         = bulletEntityAgent.BaseDamage;
            bulletEntityAgent.BaseDamage *= damageDecayFactor;
            bulletEntityAgent.PenetrableThickness =
                            bulletEntityAgent.PenetrableThickness * energyDecayFactor - info.Thickness;
            bulletEntityAgent.PenetrableLayerCount -= info.LayerCount;

            if (bulletEntityAgent.PenetrableLayerCount <= 0 || bulletEntityAgent.PenetrableThickness <= 0)
            {
                bulletEntityAgent.IsValid = false;
            }
            else
            {
                bulletEntityAgent.AddPenetrateInfo(info.Type);
            }

            var collider          = hit.collider;
            var fracturedHittable = collider.GetComponent<FracturedHittable>();
            if (fracturedHittable != null)
            {

                var fracturedChunk = HitFracturedHandler.HitFracturedObj(srcPlayer, hit, fracturedHittable);
                
                if (fracturedHittable.HasBulletHole && fracturedChunk != null)
                {
                    // ClientEffectFactory.CreateHitFracturedChunkEffect( hit.point, srcPlayer.entityKey.Value, fracturedHittable.transform, fracturedChunk.ChunkId,hit.point - fracturedChunk.transform.position, hit.normal,info.Type);
                    if (fracturedChunk.IsBroken())
                    {
                        ChunkEffectBehavior.CleanupChunkEffectBehaviors(fracturedChunk.ChunkId);
                    }
                    else
                    {
                        ClientEffectFactory.CreateHitEnvironmentEffect(hit.point, hit.normal, info.Type,
                            (int) EAudioUniqueId.BulletHit,true,fracturedChunk.ChunkId,fracturedChunk.transform);
                    }
                }
                    

                srcPlayer.statisticsData.Statistics.ShootingSuccCount++;

                if (fracturedHittable.HasBulletHole && fracturedChunk != null && bulletEntityAgent.IsValid &&
                thicknessInfo.Thickness > 0)
                {
                    ClientEffectFactory.CreateHitEnvironmentEffect(hit.point, hit.normal, info.Type,
                        (int) EAudioUniqueId.BulletHit,true,fracturedChunk.ChunkId,fracturedChunk.transform);
                    // ClientEffectFactory.CreateHitFracturedChunkEffect(_contexts.clientEffect, _entityIdGenerator,
                    // thicknessInfo.OutPoint, srcPlayer.entityKey.Value, hittedObjectKey, fracturedChunk.ChunkId,
                    // thicknessInfo.OutPoint - fracturedChunk.transform.position, thicknessInfo.Normal);
                }
            }
            else
            {
                ClientEffectFactory.AdHitEnvironmentEffectEvent(srcPlayer, hit.point, hit.normal, info.Type,
                (int) EAudioUniqueId.BulletHit);

                if (bulletEntityAgent.IsValid && thicknessInfo.Thickness > 0)
                {
                    ClientEffectFactory.AdHitEnvironmentEffectEvent(srcPlayer, thicknessInfo.OutPoint,
                    thicknessInfo.Normal, info.Type, (int) EAudioUniqueId.BulletHit);
                }
            }

            BulletHitHandler._logger.InfoFormat(
            "bullet from {0} hit environment {1}, collier {2}, base damage {3}->{4}, penetrable thick {5}->{6}, env ({7}), remain layer {8}",
            bulletEntityAgent.OwnerEntityKey, hit.point, hit.collider.name, oldDamage, bulletEntityAgent.BaseDamage,
            oldThickNess, bulletEntityAgent.PenetrableThickness, info, bulletEntityAgent.PenetrableLayerCount);
        }
      
    }
}