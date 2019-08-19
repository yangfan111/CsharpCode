using App.Server.GameModules.GamePlay;
using App.Shared.Configuration;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Assets.XmlConfig;
using Core;
using Core.Attack;
using Core.Compensation;
using Core.EntityComponent;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using System;
using System.Collections.Generic;
using UltimateFracturing;
using UnityEngine;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Throwing
{
    public class ThrowingSimulationSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingSimulationSystem));

        private Contexts _contexts;
        private int _layerMask;
        private readonly ThrowingMoveSimulator _moveSimulator;
        private ICompensationWorldFactory _compensationWorldFactory;
        private ThrowingHitHandler _throwingHitHandler;

        private IGroup<ThrowingEntity> _throwings;
        private IGroup<VehicleEntity> _vehicles;
        private IGroup<PlayerEntity> _players;

        //爆炸特效时间
        private static int _bombEffectTime = 3000;
        //烟雾弹持续时间
        private static int _fogBombEffectTime = 35000;

        private static bool _newRaycast = true;
        
        public ThrowingSimulationSystem(
            Contexts contexts,
            ICompensationWorldFactory compensationWorldFactory,
            ThrowingHitHandler hitHandler)
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
                if (throwing.isFlagDestroy)
                    continue;

                if (throwing.ownerId.Value != ownerKey)
                {
                    CheckVisible(throwing);
                    continue;
                }

                PlayerEntity player = _contexts.player.GetEntityWithEntityKey(throwing.ownerId.Value);

                if (throwing.CanExplosion())
                {
                    //爆炸
                    ExplosionEffect(throwing);
                    //伤害
                    if (SharedConfig.IsOffline || SharedConfig.IsServer)
                    {
                        BombingHandler(throwing);
                    }
                    
                    if(!throwing.throwingData.IsFly)
                    {
                        if (SharedConfig.IsServer)
                        {
                            FreeRuleEventArgs args = _contexts.session.commonSession.FreeArgs as FreeRuleEventArgs;
                            (args.Rule as IGameRule).HandleWeaponFire(_contexts, player, throwing.throwingData.WeaonConfig);
                            (args.Rule as IGameRule).HandleWeaponState(_contexts, player, throwing.throwingData.WeaonConfig.Id);
                        }
                        player.WeaponController().RelatedStatisticsData.UseThrowingCount++;
                        player.stateInterface.State.ForceFinishGrenadeThrow();
                        AfterAttack(_contexts, player);
                        player.WeaponController().AutoStuffGrenade();
                    }
                    throwing.isFlagDestroy = true;
                    continue;
                }
                if(player != null && player.throwingUpdate.ReadyFly && throwing.CanStartFlySimulation())
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
                        PlayOneEffect(throwing, throwing.throwingData.ThrowConfig.EnterWaterEffectId, throwing.position.Value, false);
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

        private void AfterAttack(Contexts contexts, PlayerEntity playerEntity)
        {
            playerEntity.throwingAction.ActionData.InternalCleanUp();
            playerEntity.WeaponController().AfterAttack();
        }

        private void OldRaycast()
        {
            CompensationWorld lastCompensationWorld = null;
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
                    if (lastCompensationWorld.Raycast(segment.RaySegment, out hit, _layerMask,0))
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
            var throwAmm = SingletonManager.Get<ThrowAmmunitionCalculator>();
            var dir = throwAmm.GetFireDir(0,playerEntity.WeaponController(),0);
            Vector3 pos = throwAmm.GetFireEmitPosition(playerEntity.WeaponController());
            Vector3 vel = dir * throwingEntity.throwingData.InitVelocity;
            throwingEntity.position.Value = pos;
            throwingEntity.throwingData.Velocity = vel;
            throwingEntity.throwingData.IsFly = true;
      //      DebugUtil.MyLog("StartFly Pos:{0} dir:{1}",pos,dir);

            /*if (SharedConfig.IsServer)
            {
                IEventArgs args = (IEventArgs)_contexts.session.commonSession.FreeArgs;

                if (!args.Triggers.IsEmpty(FreeTriggerConstant.WEAPON_STATE))
                {
                    SimpleParaList dama = new SimpleParaList();
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

                    args.Trigger(FreeTriggerConstant.WEAPON_STATE, new TempUnit("state", sp), new TempUnit("current", (FreeData)(playerEntity).freeData.FreeData) );
                }
            }*/

            //清理状态
            AfterAttack(_contexts, playerEntity);
        }

        private void ExplosionEffect(ThrowingEntity throwing)
        {
            //特效
            int effectId;
            Vector3 effectPos = throwing.position.Value;
            if (SingletonManager.Get<MapConfigManager>().InWater(throwing.position.Value))
            {
                effectId = throwing.throwingData.ThrowConfig.WaterBombEffectId;
                float wy = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(effectPos);
                if (!float.IsNaN(wy) && throwing.throwingData.WeaponSubType == (int)EWeaponSubType.Grenade)
                {//破片手雷水里特效拉到水面
                    effectPos.y = wy;
                }
            }
            else
            {
                effectId = throwing.throwingData.ThrowConfig.BombEffectId;
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
            var bombAudioId = Core.EAudioUniqueId.GrenadeExplosion;
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
                        bombAudioId = Core.EAudioUniqueId.FlashBombExplosion;
                        break;
                    case EWeaponSubType.FogBomb:
                        effectType = EClientEffectType.FogBomb;
                        bombAudioId = Core.EAudioUniqueId.FoggyBombExplosion;
                        effectTime = _fogBombEffectTime;
                        break;
                    case EWeaponSubType.BurnBomb:
                        effectType = EClientEffectType.BurnBomb;
                        break;
                }
            }
            else
            {
                bombAudioId = EAudioUniqueId.None;
            }
            if (effectId > 0)
            {
                float effectYaw = throwing.throwingData.IsFly ? 0 : 1;
                var effectEntity = ClientEffectFactory.CreateGrenadeExplosionEffect(_contexts.clientEffect, entityIdGenerator,
                                throwing.ownerId.Value, effectPos, effectYaw, 0, effectId, effectTime, effectType);
                if (effectEntity.hasAudio)
                {
                    effectEntity.RemoveAudio();
                }
                effectEntity.AddAudio((int)AudioClientEffectType.ThrowExplosion);
                effectEntity.audio.AudioClientEffectArg1 = (int)bombAudioId;
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
                throwing.throwingData.Velocity *= (1 - throwing.throwingData.ThrowConfig.CollisionVelocityDecay);
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
                default:
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
                if (player.hasPlayerMask && player.playerMask.SelfMask == (int) EPlayerMask.Invincible)
                    continue;

                float dis = Vector3.Distance(throwing.position.Value, player.position.Value);

                if (dis < throwing.throwingData.ThrowConfig.DamageRadius
                    && ((!throwing.throwingData.IsFly && throwing.ownerId.Value == player.entityKey.Value)
                    || !CommonMathUtil.Raycast(throwing.position.Value, player.position.Value, dis, _layerMask, out hitPoint)
                    || !CommonMathUtil.Raycast(throwing.position.Value, player.bones.Head.position, dis, _layerMask, out hitPoint)) )
                {
                    float damage = (1 - dis/throwing.throwingData.ThrowConfig.DamageRadius) * throwing.throwingData.ThrowConfig.BaseDamage;
                    _throwingHitHandler.OnPlayerDamage(_contexts, sourcePlayer, player, new PlayerDamageInfo(damage, (int)EUIDeadType.Weapon, (int)EBodyPart.None,
                        GetWeaponIdBySubType((EWeaponSubType)throwing.throwingData.WeaponSubType), false, false, false, player.position.Value, player.position.Value - throwing.position.Value));
                }
            }
            foreach (VehicleEntity vehicle in _vehicles)
            {
                float dis = Vector3.Distance(throwing.position.Value, vehicle.position.Value);
                if (dis < throwing.throwingData.ThrowConfig.DamageRadius
                    && !CommonMathUtil.Raycast(throwing.position.Value, vehicle.position.Value, dis, _layerMask, out hitPoint))
                {
                    float damage = (1 - dis / throwing.throwingData.ThrowConfig.DamageRadius) * throwing.throwingData.ThrowConfig.BaseDamage;
                    _throwingHitHandler.OnVehicleDamage(vehicle, damage);
                }
            }
            var colliders = Physics.OverlapSphere(throwing.position.Value, throwing.throwingData.ThrowConfig.DamageRadius, UnityLayerManager.GetLayerMask(EUnityLayerName.UserInputRaycast) | UnityLayerManager.GetLayerMask(EUnityLayerName.Glass));
            foreach (var collider in colliders)
            {
                CreateMapObjWhenBomb(collider, sourcePlayer);

                var distance = Vector3.Distance(collider.transform.position, throwing.position.Value);
                float trueDamage = distance > throwing.throwingData.ThrowConfig.DamageRadius ? 0f : Mathf.Max(0f, throwing.throwingData.ThrowConfig.BaseDamage * (1 - distance / throwing.throwingData.ThrowConfig.DamageRadius));

                if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast))
                {
                    var parent = collider.transform;
                    while (null != parent)
                    {
                        var fractured = parent.GetComponent<FracturedObject>();
                        if (null != fractured)
                        {
                            if (!HasObstacle(collider.transform.position, throwing.position.Value, (obstacleTrans) =>
                            {
                                var obstacleParent = obstacleTrans.parent;
                                while (null != obstacleParent)
                                {
                                    if (obstacleParent == fractured.transform)
                                    {
                                        return true;
                                    }
                                    obstacleParent = obstacleParent.parent;
                                }
                                return false;
                            }))
                            fractured.Explode(Vector3.zero, trueDamage);
                            break;
                        }
                        parent = parent.parent;
                    }
                }
                if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Glass))
                {
                    var parent = collider.transform;
                    while (null != parent)
                    {
                        var fractured = parent.GetComponent<FracturedGlassyChunk>();
                        if (null != fractured)
                        {
                            fractured.MakeBroken();
                        }
                        parent = parent.parent;
                    }
                }
            }
        }

        private void CreateMapObjWhenBomb(Collider collider, PlayerEntity sourcePlayer)
        {
            _logger.DebugFormat("bomb effect: {0}", collider);
            var fracturedHittable = collider.GetComponent<FracturedHittable>();
            if (fracturedHittable != null)
            {
                MapObjectUtility.StoreCreateMapObjMsg(MapObjectUtility.GetGameObjType(fracturedHittable.Owner),
                    MapObjectUtility.GetGameObjId(fracturedHittable.Owner), sourcePlayer);
                _logger.DebugFormat("Create fractured mapObj when bomb, {0}", fracturedHittable.Owner);
            }
            else
            {
                var fracObj = collider.GetComponent<FractureObjRecorder>();
                if (fracObj != null)
                {
                    var gameObj = fracObj.owner;
                    MapObjectUtility.StoreCreateMapObjMsg(MapObjectUtility.GetGameObjType(gameObj),
                        MapObjectUtility.GetGameObjId(gameObj), sourcePlayer);
                    _logger.DebugFormat("Create door mapObj when bomb, {0}", gameObj);
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

        private bool HasObstacle(Vector3 colPosition, Vector3 bombPosition, Func<Transform, bool> exclude = null)
        {
            RaycastHit hitInfo;
            if (null == exclude)
            {
                Debug.DrawLine(bombPosition, colPosition, Color.red, 10f);
                if (Physics.Linecast(bombPosition, colPosition, out hitInfo, UnityLayerManager.GetLayerMask(EUnityLayerName.Default)))
                {
                    return true;
                }
                return false;
            }
            var dir = colPosition - bombPosition;
            var obstacles = Physics.RaycastAll(bombPosition, dir, dir.magnitude, UnityLayerManager.GetLayerMask(EUnityLayerName.Default));
            foreach (var obstacle in obstacles)
            {
                if (!exclude(obstacle.transform))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
