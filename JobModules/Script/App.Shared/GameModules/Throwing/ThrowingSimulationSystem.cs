using App.Shared.GameModules.Player;
using Core.Compensation;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;
using App.Shared.GameModules.Bullet;
using App.Shared.EntityFactory;
using Core.WeaponLogic;
using Core.EntityComponent;
using App.Shared.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Enums;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;
using Utils.Appearance;
using Core.IFactory;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Player;
using com.wd.free.@event;
using com.wd.free.para;
using App.Shared.FreeFramework.framework.@event;
using App.Shared.FreeFramework.framework.trigger;
using UltimateFracturing;
using Utils.Singleton;
using App.Shared.Util;
using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using App.Shared.GameModules.Weapon.Behavior;

namespace App.Shared.GameModules.Throwing
{
    public class ThrowingSimulationSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingSimulationSystem));

        private Contexts _contexts;
        private int _layerMask;
        private readonly ThrowingMoveSimulator _moveSimulator;
        private ICompensationWorldFactory _compensationWorldFactory;
        private IThrowingHitHandler _throwingHitHandler;

        private IGroup<ThrowingEntity> _throwings;
        private IGroup<VehicleEntity> _vehicles;
        private IGroup<PlayerEntity> _players;

        //爆炸特效时间
        private static int _bombEffectTime = 3000;
        //烟雾弹持续时间
        private static int _fogBombEffectTime = 35000;

        private static bool _newRaycast = true;
        private ISoundEntityFactory _soundEntityFactory;

        public ThrowingSimulationSystem(
            Contexts contexts,
            ICompensationWorldFactory compensationWorldFactory,
            IThrowingHitHandler hitHandler,
            ISoundEntityFactory soundEntityFactory)
        {
            _contexts = contexts;
            _layerMask = UnityLayers.SceneCollidableLayerMask | UnityLayerManager.GetLayerIndex(EUnityLayerName.UI);//BulletLayers.GetBulletLayer();
            _moveSimulator = new ThrowingMoveSimulator(20, contexts.player);
            _compensationWorldFactory = compensationWorldFactory;
            _throwingHitHandler = hitHandler;
            //all throwings
            _throwings = _contexts.throwing.GetGroup(ThrowingMatcher.AllOf(ThrowingMatcher.ThrowingData));
            //all vehicles
            _vehicles = _contexts.vehicle.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.GameObject));
            //all players
            _players = _contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.FirstPersonModel, PlayerMatcher.ThirdPersonModel));
            _soundEntityFactory = soundEntityFactory;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            Update(owner.OwnerEntityKey, cmd.FrameInterval);
        }

        private List<IThrowingSegment> _allThrowingSegments = new List<IThrowingSegment>();
        private ThrowingSegmentComparator _comparator = new ThrowingSegmentComparator();

        public void Update(EntityKey ownerKey, int frameTime)
        {
            _allThrowingSegments.Clear();
            foreach (ThrowingEntity throwing in _throwings.GetEntities())
            {
                if (throwing.ownerId.Value != ownerKey)
                {
                    CheckVisible(throwing);
                    continue;
                }

                if (throwing.isFlagDestroy)
                {
                    continue;
                }

                PlayerEntity player = _contexts.player.GetEntityWithEntityKey(throwing.ownerId.Value);

                //销毁被中断的手雷
                if (null != player && player.throwingAction.ActionInfo.IsInterrupt
                    && player.throwingAction.ActionInfo.ThrowingEntityKey == throwing.entityKey.Value)
                {
                    player.throwingAction.ActionInfo.IsInterrupt = false;
                    throwing.isFlagDestroy = true;
                    continue;
                }

                if (throwing.hasLifeTime && (DateTime.Now - throwing.lifeTime.CreateTime).TotalMilliseconds > throwing.throwingData.Config.CountdownTime)
                {
                    //爆炸
                    ExplosionEffect(throwing);
                    //伤害
                    if (SharedConfig.IsOffline || SharedConfig.IsServer)
                    {
                        BombingHandler(throwing);
                    }
                    throwing.isFlagDestroy = true;
                    if(!throwing.throwingData.IsFly)
                    {
                        player.stateInterface.State.FinishGrenadeThrow();
                        CastGrenade(_contexts, player);
                    }
                    continue;
                }
                
                if (throwing.throwingData.IsThrow
                    && !throwing.throwingData.IsFly 
                    && null != player
                    && player.throwingUpdate.ReadyFly
                    && !throwing.isFlagDestroy)
                {
                    //开始飞出
                    StartFlying(player, throwing);
                }

                if (throwing.throwingData.IsFly)
                {
                    CheckVisible(throwing);
                    bool isInWater = throwing.throwingData.IsInWater;

                    var segments = _moveSimulator.MoveThrowing(throwing, frameTime);
                    if (null != segments)
                        _allThrowingSegments.AddRange(segments);

                    //入水特效
                    throwing.throwingData.IsInWater = SingletonManager.Get<MapConfigManager>().InWater(throwing.position.Value);
                    if (!isInWater && throwing.throwingData.IsInWater)
                    {
                        PlayOneEffect(throwing, throwing.throwingData.Config.EnterWaterEffectId, throwing.position.Value, false);
                    }
                }
                else if (null != player)
                {
                    Vector3 pos = PlayerEntityUtility.GetHandWeaponPosition(player);
                    throwing.position.Value = pos;
                }
            }

            _allThrowingSegments.Sort(_comparator);

            if (_newRaycast)
            {
                NewRaycast();
            }
            else
            {
                OldRaycast();
            }
        }

        private void CastGrenade(Contexts contexts, PlayerEntity playerEntity)
        {
            if(!playerEntity.throwingAction.ActionInfo.IsReady)
            {
                return;
            }
            playerEntity.throwingAction.ActionInfo.ClearState();
            playerEntity.WeaponController().ExpendAfterAttack();
        }

        private void OldRaycast()
        {
            ICompensationWorld lastCompensationWorld = null;
            foreach (var segment in _allThrowingSegments)
            {
                if (!segment.IsValid)
                    continue;
                if (lastCompensationWorld != null)
                {
                    if (lastCompensationWorld.ServerTime != segment.ServerTime)
                    {
                        lastCompensationWorld.Release();
                        lastCompensationWorld = _compensationWorldFactory.CreateCompensationWorld(segment.ServerTime);
                    }
                }
                else
                {
                    lastCompensationWorld = _compensationWorldFactory.CreateCompensationWorld(segment.ServerTime);
                    if (lastCompensationWorld == null)
                        _logger.ErrorFormat("create compensation world at time {0}, FAILED", segment.ServerTime);
                    else
                    {
                        if (_logger.IsDebugEnabled)
                            _logger.DebugFormat("create compensation world at time {0}, SUCC", segment.ServerTime);
                    }
                }

                if (lastCompensationWorld != null)
                {
                    RaycastHit hit;
                    lastCompensationWorld.Self = segment.ThrowingEntity.ownerId.Value;
                    lastCompensationWorld.ExcludePlayerList = segment.ExcludePlayerList; 
                    if (lastCompensationWorld.Raycast(segment.RaySegment, out hit, _layerMask))
                    {
                        CollisionHandler(segment, hit);
                    }
                }
            }
            if (lastCompensationWorld!=null )
            {
                lastCompensationWorld.Release();
            }
        }

        private void NewRaycast()
        {
            RaycastHit hit;
            foreach (var segment in _allThrowingSegments)
            {
                if (!segment.IsValid)
                    continue;

                bool isHit = CommonMathUtil.Raycast(segment.RaySegment.Ray, segment.RaySegment.Length, _layerMask, out hit);
                if (isHit)
                {
                    CollisionHandler(segment, hit);
                    break;
                }
            }
        }

        private void CheckVisible(ThrowingEntity throwing)
        {
            if (throwing.hasThrowingGameObject && throwing.throwingData.IsThrow && throwing.throwingData.IsFly)
            {
                throwing.throwingGameObject.UnityObject.AsGameObject.SetActive(true);
            }
        }

        private void StartFlying(PlayerEntity playerEntity, ThrowingEntity throwingEntity)
        {
            var dir = BulletDirUtility.GetThrowingDir(playerEntity.WeaponController());
            Vector3 vel = dir * throwingEntity.throwingData.InitVelocity;
            Vector3 pos = PlayerEntityUtility.GetThrowingEmitPosition(playerEntity.WeaponController());
            throwingEntity.position.Value = pos;
            throwingEntity.throwingData.Velocity = vel;
            throwingEntity.throwingData.IsFly = true;

            if (SharedConfig.IsServer)
            {
                IEventArgs args = (IEventArgs)_contexts.session.commonSession.FreeArgs;

                if (!args.Triggers.IsEmpty((int)EGameEvent.WeaponState))
                {
                    SimpleParaList dama = new SimpleParaList();
                    //TODO 确认逻辑
                    dama.AddFields(new ObjectFields(playerEntity));
                    var weaponData = playerEntity.WeaponController().HeldWeaponAgent.ComponentScan;
                    if (!weaponData.IsSafeVailed)
                        return;
                    dama.AddPara(new IntPara("CarryClip", playerEntity.WeaponController().GetReservedBullet()));
                    dama.AddPara(new IntPara("Clip", weaponData.Bullet));
                    var config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponData.ConfigId);
                    dama.AddPara(new IntPara("ClipType", null == config ? 0 : config.Caliber));
                    dama.AddPara(new IntPara("id", weaponData.ConfigId));
                    SimpleParable sp = new SimpleParable(dama);

                    args.Trigger((int)EGameEvent.WeaponState, new TempUnit("state", sp), new TempUnit("current", (FreeData)(playerEntity).freeData.FreeData) );
                }
            }

            //清理状态
            CastGrenade(_contexts, playerEntity);
        }

        private void ExplosionEffect(ThrowingEntity throwing)
        {
            //特效
            int effectId;
            Vector3 effectPos = throwing.position.Value;
            if (SingletonManager.Get<MapConfigManager>().InWater(throwing.position.Value))
            {
                effectId = throwing.throwingData.Config.WaterBombEffectId;
                float wy = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(effectPos);
                if (!float.IsNaN(wy) && throwing.throwingData.WeaponSubType == (int)EWeaponSubType.Grenade)
                {//破片手雷水里特效拉到水面
                    effectPos.y = wy;
                }
            }
            else
            {
                effectId = throwing.throwingData.Config.BombEffectId;
            }

            //烟雾弹位置计算
            if (throwing.throwingData.WeaponSubType == (int)EWeaponSubType.FogBomb)
            {
                effectPos = CommonMathUtil.GetSpacePos(effectPos, 0.5f, _layerMask);
            }

            PlayOneEffect(throwing, effectId, effectPos, true);
        }

        private void PlayOneEffect(ThrowingEntity throwing, int effectId, Vector3 effectPos, bool isBomb)
        {
           
           
           var entityIdGenerator = _contexts.session.commonSession.EntityIdGenerator;
       

            EClientEffectType effectType = EClientEffectType.GrenadeExplosion;
            int effectTime = _bombEffectTime;
            if (isBomb)
            {
                //爆炸特效类型
                switch ((EWeaponSubType)throwing.throwingData.WeaponSubType)
                {
                    case EWeaponSubType.Grenade:
                        effectType = EClientEffectType.GrenadeExplosion;
                        break;
                    case EWeaponSubType.FlashBomb:
                        effectType = EClientEffectType.FlashBomb;
                        break;
                    case EWeaponSubType.FogBomb:
                        effectType = EClientEffectType.FogBomb;
                        effectTime = _fogBombEffectTime;
                        break;
                    case EWeaponSubType.BurnBomb:
                        effectType = EClientEffectType.BurnBomb;
                        break;
                }
            }

            if (effectId > 0)
            {
                float effectYaw = throwing.throwingData.IsFly ? 0 : 1;
                ClientEffectFactory.CreateGrenadeExplosionEffect(_contexts.clientEffect, entityIdGenerator,
                                throwing.ownerId.Value, effectPos, effectYaw, 0, effectId, effectTime, effectType);
            }
        }

        private void CollisionHandler(IThrowingSegment segment, RaycastHit hit)
        {
            //            Debug.LogFormat("Throwing collision dir:{0}, pos:{1}, normal:{2}", segment.RaySegment.Ray.direction, segment.RaySegment.Ray.origin, hit.normal);
            //glass broken
            var reflectiveFace = true;
            if (hit.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Glass))
            {
                var glassChunk = hit.collider.GetComponent<FracturedGlassyChunk>();
                if (glassChunk != null)
                {
                    glassChunk.MakeBroken();
                }
                reflectiveFace = false;
            }

            if (DebugConfig.DrawThrowingLine)
            {
                RuntimeDebugDraw.Draw.DrawLine(segment.RaySegment.Ray.origin, (segment.RaySegment.Ray.origin + hit.normal * 5), Color.red, 60f);
            }

            ThrowingEntity throwing = segment.ThrowingEntity;
            //位置
            throwing.position.Value = segment.RaySegment.Ray.origin;
            if (reflectiveFace)
            {
                //反射
                throwing.throwingData.Velocity = Vector3.Reflect(throwing.throwingData.Velocity, hit.normal);
                //衰减
                throwing.throwingData.Velocity *= (1 - throwing.throwingData.Config.CollisionVelocityDecay);
            }
        }

        private void BombingHandler(ThrowingEntity throwing)
        {
            switch ((EWeaponSubType)throwing.throwingData.WeaponSubType)
            {
                case EWeaponSubType.Grenade:
                    GrenadeDamageHandler(throwing);
                    break;
                case EWeaponSubType.FlashBomb:
                    FlashDamageHandler(throwing);
                    break;
                case EWeaponSubType.FogBomb:
                    FogDamageHandler(throwing);
                    break;
                case EWeaponSubType.BurnBomb:
                    BurnDamageHandler(throwing);
                    break;
            }
        }

        private void GrenadeDamageHandler(ThrowingEntity throwing)
        {
            PlayerEntity sourcePlayer = null;
            if (throwing.hasOwnerId)
            {
                sourcePlayer = _contexts.player.GetEntityWithEntityKey(throwing.ownerId.Value);
            }

            Vector3 hitPoint;
            foreach (PlayerEntity player in _players)
            {
                float dis = Vector3.Distance(throwing.position.Value, player.position.Value);
                //头部
                Vector3 headPos = player.position.Value;
                Transform tran;
                if (player.appearanceInterface.Appearance.IsFirstPerson)
                {
                    var root = player.RootGo();
                    tran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName);
                }
                else
                {
                    var root = player.RootGo();
                    tran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName);
                }
                if (null != tran)
                {
                    headPos = tran.position;
                }
                if (dis < throwing.throwingData.Config.DamageRadius
                    && ( (!throwing.throwingData.IsFly && throwing.ownerId.Value == player.entityKey.Value)
                    || !CommonMathUtil.Raycast(throwing.position.Value, player.position.Value, dis, _layerMask, out hitPoint)
                    || !CommonMathUtil.Raycast(throwing.position.Value, headPos, dis, _layerMask, out hitPoint)) )
                {
                    float damage = (1 - dis/throwing.throwingData.Config.DamageRadius) * throwing.throwingData.Config.BaseDamage;
                    _throwingHitHandler.OnPlayerDamage(sourcePlayer, player, new PlayerDamageInfo(damage, (int)EUIDeadType.Weapon, (int)EBodyPart.None, GetWeaponIdBySubType((EWeaponSubType)throwing.throwingData.WeaponSubType)));
                }
            }
            foreach (VehicleEntity vehicle in _vehicles)
            {
                float dis = Vector3.Distance(throwing.position.Value, vehicle.position.Value);
                if (dis < throwing.throwingData.Config.DamageRadius
                    && !CommonMathUtil.Raycast(throwing.position.Value, vehicle.position.Value, dis, _layerMask, out hitPoint))
                {
                    float damage = (1 - dis / throwing.throwingData.Config.DamageRadius) * throwing.throwingData.Config.BaseDamage;
                    _throwingHitHandler.OnVehicleDamage(vehicle, damage);
                }
            }
        }

        private void FlashDamageHandler(ThrowingEntity throwing)
        {
            
        }

        private void FogDamageHandler(ThrowingEntity throwing)
        {
            
        }

        private void BurnDamageHandler(ThrowingEntity throwing)
        {
            
        }

        private int GetWeaponIdBySubType(EWeaponSubType subType)
        {
            int weaponId = 37;
            switch (subType)
            {
                case EWeaponSubType.Grenade:
                    weaponId = 37;
                    break;
                case EWeaponSubType.FlashBomb:
                    weaponId = 38;
                    break;
                case EWeaponSubType.FogBomb:
                    weaponId = 39;
                    break;
                case EWeaponSubType.BurnBomb:
                    weaponId = 40;
                    break;
            }
            return weaponId;
        }
    }
}