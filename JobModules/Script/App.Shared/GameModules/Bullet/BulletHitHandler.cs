using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.EntityFactory;
using App.Shared.GameModules.SceneObject;
using App.Shared.GameModules.Vehicle;
using App.Shared.Util;
using Core;
using Core.BulletSimulation;
using Core.Compensation;
using Core.EntityComponent;
using Core.Enums;
using Core.HitBox;
using Core.IFactory;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Bullet
{
    public class BulletHitHandler : IBulletHitHandler
    {
        private IEntityIdGenerator _entityIdGenerator;
        private IPlayerDamager _damager;
        private IDamageInfoCollector _damageInfoCollector;
        private IEnvironmentTypeConfigManager _environmentTypeConfigManager;

        private CustomProfileInfo _OnHitPlayer = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BulletHitHandler_OnHitPlayer");
        private CustomProfileInfo _OnHitVehicle = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BulletHitHandler_OnHitVehicle");
        private CustomProfileInfo _OnHitEnvironment = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BulletHitHandler_OnHitEnvironment");
        public BulletHitHandler(
            Contexts contexts, 
            IEntityIdGenerator entityIdGenerator, 
            IPlayerDamager damager, 
            IDamageInfoCollector damageInfoCollector,
            ISoundEntityFactory soundEntityFactory,
            IEnvironmentTypeConfigManager environmentTypeConfigManager)
        {
            _contexts = contexts;
            this._entityIdGenerator = entityIdGenerator;
            _damager = damager;
            _damageInfoCollector = damageInfoCollector;
            _soundEntityFactory = soundEntityFactory;
            _environmentTypeConfigManager = environmentTypeConfigManager;
           _sceneObjectEntityFactory = _contexts.session.entityFactoryObject.SceneObjectEntityFactory;
        }

        private Contexts _contexts;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletHitHandler));
        private ISoundEntityFactory _soundEntityFactory;
        private int _hitLayerMask;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;

        public void OnHitVehicle(
            PlayerEntity srcPlayer, 
            VehicleEntity targetVehicle, 
            IBulletEntity bulletEntity,
            RaycastHit hit)
        {
            if(srcPlayer.gamePlay.IsDead())
            {
                return;
            }
            Collider collider = hit.collider;
            VehiclePartIndex partIndex;
            var hitBoxFactor = VehicleEntityUtility.GetHitFactor(targetVehicle, collider, out partIndex);

            var totalDamage = GetBulletDamage(bulletEntity, hitBoxFactor, Vector3.Distance(hit.point, bulletEntity.GunEmitPosition));
            var gameData = targetVehicle.GetGameData();
            gameData.DecreaseHp(partIndex, totalDamage, srcPlayer.entityKey.Value);
            srcPlayer.statisticsData.Statistics.TotalDamage += totalDamage;

            bulletEntity.IsValid = false;
            
            srcPlayer.statisticsData.Statistics.ShootingSuccCount++;
            _logger.DebugFormat(
                "bullet from {0} hit vehicle {1}, part {2}",
                bulletEntity.OwnerEntityKey,
                targetVehicle.entityKey.Value,
                collider.name);
            

            ClientEffectFactory.AddHitVehicleEffectEvent(
                srcPlayer,
                targetVehicle.entityKey.Value,
                hit.point,
                hit.point - targetVehicle.position.Value,
                hit.normal);
        }

        public void OnHitPlayer(Contexts contexts, PlayerEntity srcPlayer, PlayerEntity targetPlayer, IBulletEntity bulletEntity,
            RaycastHit hit, Vector3 targetPlayerPostion, int cmdSeq)
        {
            if(srcPlayer.gamePlay.IsDead())
            {
                return;
            }
            Collider collider = hit.collider;

            EBodyPart part = BulletPlayerUtility.GetBodyPartByHitBoxName(collider);

            
            _logger.DebugFormat("OnHitPlayer in {0}", part);
            
            float hitboxFactor = bulletEntity.GetDamageFactor(part);
            float totalDamage = GetBulletDamage(bulletEntity, hitboxFactor, Vector3.Distance(hit.point, bulletEntity.GunEmitPosition));

            bulletEntity.IsValid = false;

            //由于动画放在客户端做了,服务器调用的命令会被忽视,需要发送事件到客户端
//            if (targetPlayer.hasStateInterface && targetPlayer.stateInterface.State.CanBeenHit())
//            {
//                targetPlayer.stateInterface.State.BeenHit();
//            }
            
            ClientEffectFactory.AddBeenHitEvent(srcPlayer, targetPlayer.entityKey.Value, GeneraterUniqueHitId(srcPlayer, cmdSeq), contexts.session.currentTimeObject.CurrentTime);
            ClientEffectFactory.AddHitPlayerEffectEvent(srcPlayer, targetPlayer.entityKey.Value, hit.point, hit.point - targetPlayer.position.Value);
            
            _logger.InfoFormat(
                "bullet from {0} hit player {1}, part {2}, hitbox factor {3}, result damage {4}",
                bulletEntity.OwnerEntityKey,
                targetPlayer.entityKey.Value,
                collider,
                hitboxFactor,
                totalDamage);
            

            if(!targetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
            {   
                //有效命中
                if (targetPlayer.gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
                {
                    srcPlayer.statisticsData.Statistics.ShootingPlayerCount++;
                }
                srcPlayer.statisticsData.Statistics.ShootingSuccCount++;
            }

            BulletPlayerUtility.ProcessPlayerHealthDamage(
                contexts,
                _damager,
                srcPlayer, 
                targetPlayer, 
                new PlayerDamageInfo(totalDamage, (int)EUIDeadType.Weapon, (int)part, bulletEntity.WeaponId, bulletEntity.IsOverWall),
                _damageInfoCollector);
        }

        public static int GeneraterUniqueHitId(PlayerEntity srcPlayer, int cmdSeq)
        {
            return (srcPlayer.entityKey.Value.EntityId << 16) + cmdSeq;   
        }

        private float GetBulletDamage(IBulletEntity bulletEntity, float hitboxFactor, float distance)
        {
            float baseHarm = bulletEntity.BaseDamage;
            float distanceDecay = bulletEntity.DistanceDecayFactor;
            // 武器基础伤害 * (距离系数 ^ (实际命中距离 / 1270)) * hitbox系数 * 防弹装备系数 * 穿透系数
            float totalDamage = baseHarm * Mathf.Pow(distanceDecay, distance / 12.7f) * hitboxFactor;

            _logger.InfoFormat(
                "bullet damage baseHarm {0}, distance decay {1}, distance {2}, hitbox factor {3}, result damage {4}",
                baseHarm,
                distanceDecay,
                distance,
                hitboxFactor,
                totalDamage);

            return totalDamage;
        }

        public IBulletHitHandler SetHitLayerMask(int hitLayerMask)
        {
            _hitLayerMask = hitLayerMask;
            return this;
        } 

        public virtual void OnHit(int cmdSeq, IBulletEntity bulletEntity, RaycastHit hit, ICompensationWorld compensationWorld)
        {
            
            bulletEntity.HitPoint = hit.point;
            Collider collider = hit.collider;
            if (collider == null)
            {
                _logger.ErrorFormat("bullet hit unknown collier {0}", bulletEntity.OwnerEntityKey);
                return;
            }
            PlayerEntity srcPlayer = _contexts.player.GetEntityWithEntityKey(bulletEntity.OwnerEntityKey);
            if (srcPlayer == null)
            {
                _logger.WarnFormat("bullet from unkown {0} hit environment {1}, collier {2}",
                    bulletEntity.OwnerEntityKey,
                    hit.point, collider.name);
                return;
            }
            PlayerEntity targetPlayer = null;
            VehicleEntity targetVehicle = null;
            var comp = hit.collider.transform.gameObject.GetComponent<HitBoxOwnerComponent>();
            if (comp != null)
            {
                targetPlayer = _contexts.player.GetEntityWithEntityKey(comp.OwnerEntityKey);
                targetVehicle = _contexts.vehicle.GetEntityWithEntityKey(comp.OwnerEntityKey);
            }

            if (targetPlayer != null)
            {
                try
                {
                    _OnHitPlayer.BeginProfileOnlyEnableProfile();
                    Vector3 pos;
                    if (compensationWorld.TryGetEntityPosition(targetPlayer.entityKey.Value, out pos))
                    {
                        OnHitPlayer(_contexts, srcPlayer, targetPlayer, bulletEntity, hit, pos, cmdSeq);
                    }
                    else
                    {
                        _logger.ErrorFormat("cant get player compensation position with key {0}", targetPlayer.entityKey.Value);
                        OnHitPlayer(_contexts, srcPlayer, targetPlayer, bulletEntity, hit, targetPlayer.position.Value, cmdSeq);
                    }
                    bulletEntity.HitType = EHitType.Player;
                }
                finally
                {
                    _OnHitPlayer.EndProfileOnlyEnableProfile();
                }
            }
            else if (targetVehicle != null)
            {
                try
                {
                    _OnHitVehicle.BeginProfileOnlyEnableProfile();
                    OnHitVehicle(srcPlayer, targetVehicle, bulletEntity, hit);
                    bulletEntity.HitType = EHitType.Vehicle;
                }
                finally
                {
                    _OnHitVehicle.EndProfileOnlyEnableProfile();
                }
                
            }
            else
            {
                try
                {
                    _OnHitEnvironment.BeginProfileOnlyEnableProfile();
                    OnHitEnvironment(srcPlayer, bulletEntity, hit);
                    bulletEntity.HitType = EHitType.Environment;
                }
                finally
                {
                    _OnHitEnvironment.EndProfileOnlyEnableProfile();
                }
                
            }
        }

        private void OnHitEnvironment(PlayerEntity srcPlayer, IBulletEntity bulletEntity, RaycastHit hit)
        {
            if(srcPlayer.gamePlay.IsDead())
            {
                return;
            }
            ThicknessInfo thicknessInfo;
            EnvironmentInfo info = BulletEnvironmentUtility.GetEnvironmentInfoByHitBoxName(hit, bulletEntity.Velocity, out thicknessInfo);
            float damageDecayFactor = _environmentTypeConfigManager.GetDamageDecayFactorByEnvironmentType(info.Type);
            float energyDecayFactor = _environmentTypeConfigManager.GetEnergyDecayFactorByEnvironmentType(info.Type);
            float oldThickNess = bulletEntity.PenetrableThickness;
            float oldDamage = bulletEntity.BaseDamage;
            bulletEntity.BaseDamage *= damageDecayFactor;
            bulletEntity.PenetrableThickness = bulletEntity.PenetrableThickness * energyDecayFactor - info.Thickness;
            bulletEntity.PenetrableLayerCount -= info.LayerCount;

            if (bulletEntity.PenetrableLayerCount <= 0 || bulletEntity.PenetrableThickness <= 0)
            {
                bulletEntity.IsValid = false;
            }
            else
            {
                bulletEntity.AddPenetrateInfo(info.Type);
            }

            EBulletCaliber caliber = bulletEntity.Caliber; //根据口径

            var collider = hit.collider;
            var fracturedHittable = collider.GetComponent<FracturedHittable>();
            
            if (fracturedHittable != null)
            {
                EntityKey? hittedObjectKey = null;
                
                var sceneObjectEntity = MapObjectUtility.GetMapObjectOfFracturedHittable(fracturedHittable);
                if (sceneObjectEntity != null)
                    hittedObjectKey = sceneObjectEntity.entityKey.Value;
                else
                {
                    var mapObjectEntity = MapObjectUtility.GetMapObjectOfFracturedHittable(fracturedHittable);
                    if (mapObjectEntity != null)
                        hittedObjectKey = mapObjectEntity.entityKey.Value;
                }

                FracturedAbstractChunk fracturedChunk = null;
                
                if (null != hittedObjectKey)
                {
                    fracturedChunk = fracturedHittable.Hit(hit.point, hit.normal);
                    if(fracturedHittable.HasBulletHole && fracturedChunk != null)
                        ClientEffectFactory.CreateHitFracturedChunkEffect(
                            _contexts.clientEffect,
                            _entityIdGenerator,
                            hit.point,
                            srcPlayer.entityKey.Value,
                            hittedObjectKey.Value,
                            fracturedChunk.ChunkId,
                            hit.point - fracturedChunk.transform.position,
                            hit.normal);

                    srcPlayer.statisticsData.Statistics.ShootingSuccCount++; 
                }
                else
                {
                    _logger.ErrorFormat("no entity reference attached to {0}", fracturedHittable.name);
                }
                if (fracturedHittable.HasBulletHole && fracturedChunk != null 
                    && bulletEntity.IsValid && thicknessInfo.Thickness > 0)
                {
                    ClientEffectFactory.CreateHitFracturedChunkEffect(_contexts.clientEffect,
                        _entityIdGenerator,
                        thicknessInfo.OutPoint,
                        srcPlayer.entityKey.Value,
                        hittedObjectKey.Value,
                        fracturedChunk.ChunkId,
                        thicknessInfo.OutPoint - fracturedChunk.transform.position,
                        thicknessInfo.Normal);
                }
            }
            else
            {
                ClientEffectFactory.AdHitEnvironmentEffectEvent(srcPlayer, hit.point,
                    hit.normal, info.Type);

                if(bulletEntity.IsValid && thicknessInfo.Thickness > 0)
                {
                    ClientEffectFactory.AdHitEnvironmentEffectEvent(srcPlayer,  thicknessInfo.OutPoint, thicknessInfo.Normal,info.Type);
                  
                }
            }

           
            _logger.DebugFormat(
                "bullet from {0} hit environment {1}, collier {2}, base damage {3}->{4}, penetrable thick {5}->{6}, env ({7}), remain layer {8}",
                bulletEntity.OwnerEntityKey,
                hit.point, hit.collider.name,
                oldDamage, bulletEntity.BaseDamage,
                oldThickNess, bulletEntity.PenetrableThickness,
                info,
                bulletEntity.PenetrableLayerCount);
        }
    }
}