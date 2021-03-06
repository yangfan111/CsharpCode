using System;
using System.Collections.Generic;
using App.Shared.Audio;
using App.Shared.Components;
using App.Shared.Player.Events;
using App.Shared.Util;
using Core;
using Core.Components;
using Core.Configuration;
using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.EntityFactory
{
    public class ClientEffectFactory
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientEffectFactory));

        

        #region new effect 
        public static void AdHitEnvironmentEffectEvent(PlayerEntity srcPlayer, Vector3 hitPoint, Vector3 offset,
                                                       EEnvironmentType environmentType, int hitAuidoId,
                                                       int chunkId = 0, bool needEffectEntity = true)
        {
            HitEnvironmentEvent e =
                            (HitEnvironmentEvent) EventInfos.Instance.Allocate(EEventType.HitEnvironment, false);
            e.EnvironmentType = environmentType;
            e.Offset          = offset;
            e.HitAudioId      = hitAuidoId;
            e.HitPoint        = hitPoint.ShiftedToFixedVector3();
            e.ChunkId         = chunkId;

            e.needEffectEntity = needEffectEntity;

            srcPlayer.localEvents.Events.AddEvent(e);
        }

        public static void CreateHitEnvironmentEffect( HitEnvironmentEvent hitEnvironmentEvent)
        {
            CreateHitEnvironmentEffect(hitEnvironmentEvent.HitPoint.ShiftedVector3(), hitEnvironmentEvent.Offset,
                hitEnvironmentEvent.EnvironmentType, hitEnvironmentEvent.HitAudioId,
                hitEnvironmentEvent.needEffectEntity,hitEnvironmentEvent.ChunkId);
        }
        
        
        
        
        public static void CreateHitEnvironmentEffect(Vector3 hitPoint, Vector3 normal,
                                                      EEnvironmentType environmentType, int hitAudioId,
                                                      bool needEffectEntity,int chunkId,Transform parent = null)
        {
            EffectTypeInfo effectTypeInfo = environmentType.ToClientEfcInfo();
            if (needEffectEntity)
            {
                ClientEffectEmitter clientEffectObj =
                                LocalObjectGenerator.EffectLocal.GetClientEffectEmitter(EEffectObjectClassify.EnvHit);
                IEffectBehavior effectBehavior;
                if (chunkId > 0)
                {
                    var chunkBehavior =  ObjectAllocatorHolder<ChunkEffectBehavior>.Allocate();  
                    //ChunkEffectBehavior  b1 =ObjectAllocatorHolder<ChunkEffectBehavior>.Allocate();
                    chunkBehavior.Initialize(normal, hitPoint, (int) effectTypeInfo.AudioType, hitAudioId,
                        AudioClientEffectType.WeaponEnvHit,chunkId,parent);
                    effectBehavior = chunkBehavior;
                }
                else
                {
                    var noramlEffect =  ObjectAllocatorHolder<NormalEffectBehavior>.Allocate();  

                    //NormalEffectBehavior b2 = ObjectAllocatorHolder<>.Allocate();
                    noramlEffect.Initialize(normal, hitPoint, (int) effectTypeInfo.AudioType, hitAudioId,
                        AudioClientEffectType.WeaponEnvHit);
                    effectBehavior = noramlEffect;
                }
                clientEffectObj.Initialize((int) effectTypeInfo.DefaultId, effectBehavior);
            }
            else //应对于空手情况
            {
                GameAudioMedia.PlayHitEnvironmentAudio(effectTypeInfo.AudioType, hitAudioId, hitPoint);
            }
        }

        public static void CreateBulletFlyEffect(EntityKey owner, Vector3 startPos, Quaternion startRocation,
                                                 Vector3 velocity, int effectId,float delay)
        {
            ClientEffectEmitter clientEffectObj =
                            LocalObjectGenerator.EffectLocal.GetClientEffectEmitter(EEffectObjectClassify.BulletFly);
            var moveEffectBehavior = ObjectAllocatorHolder<MoveEffectBehavior>.Allocate(); 
            moveEffectBehavior.Initialize(startPos, startRocation, velocity, delay);
            clientEffectObj.Initialize(effectId, moveEffectBehavior);
            //DebugUtil.MyLog("emit fire effect:{0},{1}", startPos, effectId);
        }
        public static void CreateBulletDrop(Vector3 position, float yaw, float pitch, int effectId,
                                            int weaponId, AudioGrp_FootMatType dropMatType)
        {
            ClientEffectEmitter clientEffectObj =
                            LocalObjectGenerator.EffectLocal.GetClientEffectEmitter(EEffectObjectClassify.BulletDrop);
            var yawImmobileEffectBehavior = ObjectAllocatorHolder<YawEffectBehavior>.Allocate();
            yawImmobileEffectBehavior.Initialize(pitch, yaw, position, SingletonManager.Get<AudioWeaponManager>().FindById(weaponId).BulletDrop, (int) dropMatType,AudioClientEffectType.BulletDrop);
            clientEffectObj.Initialize(effectId, yawImmobileEffectBehavior);
        }

        public static void CreateMuzzleSparkEffct(Vector3 postion, float pitch, float yaw, int effectId,
                                                  Transform muzzleTrans)
        {
            ClientEffectEmitter clientEffectObj =
                            LocalObjectGenerator.EffectLocal.GetClientEffectEmitter(EEffectObjectClassify.Muzzle);
            var immobileEffectBehavior = ObjectAllocatorHolder<YawEffectBehavior>.Allocate();
            immobileEffectBehavior.Initialize(pitch, yaw, postion, muzzleTrans);
            clientEffectObj.Initialize(effectId, immobileEffectBehavior);
        }
        public static void AddHitPlayerEffectEvent(PlayerEntity srcPlayer, EntityKey target, Vector3 hitPoint,
                                                   int audioId, EBodyPart part)
        {
            HitPlayerEvent e = (HitPlayerEvent) EventInfos.Instance.Allocate(EEventType.HitPlayer, false);
            e.Target      = target;
            e.HitPoint    = hitPoint.ShiftedToFixedVector3();
            e.HitAudioId  = audioId;
            e.HitBodyPart = (byte) part;
            srcPlayer.localEvents.Events.AddEvent(e);
        }

        public static void CreateHitPlayerEffect(Contexts context, EntityKey owner, HitPlayerEvent hitPlayerEvent)
        {
            if (SharedConfig.IsHXMod)
                return;
            var player = context.player.GetEntityWithEntityKey(owner);
            if (null == player || !player.hasBones)
                return;
            var effectId = SingletonManager.Get<ClientEffectCommonConfigManager>()
                            .GetConfigByType(EEffectObjectClassify.PlayerHit).PreLoadCfgId;
            ClientEffectEmitter clientEffectObj =
                            LocalObjectGenerator.EffectLocal.GetClientEffectEmitter(EEffectObjectClassify.PlayerHit);
            var hitPlayerEffectBehavior = ObjectAllocatorHolder<HitPlayerEffectBehavior>.Allocate();
            hitPlayerEffectBehavior.Initialize(hitPlayerEvent.HitPoint.ShiftedVector3(),new AudioEffectData(hitPlayerEvent.HitBodyPart,hitPlayerEvent.HitAudioId,AudioClientEffectType.WeaponPlayerHit), player.bones.Spine);
            clientEffectObj.Initialize(effectId, hitPlayerEffectBehavior);
        }
        #endregion


        public static void AddBeenHitEvent(PlayerEntity srcPlayer, PlayerEntity target, int damageId, int triggerTime)
        {
            if (CanPlayBeenHit(target))
            {
                BeenHitEvent e = (BeenHitEvent) EventInfos.Instance.Allocate(EEventType.BeenHit, false);
                e.Target      = target.entityKey.Value;
                e.UniqueId    = damageId;
                e.TriggerTime = triggerTime;
                srcPlayer.localEvents.Events.AddEvent(e);
            }
        }

        private static bool CanPlayBeenHit(PlayerEntity srcPlayer)
        {
            if (srcPlayer.isFlagSelf)
            {
                return true;
            }

            return srcPlayer.hasThirdPersonAppearance &&
                            ((int) srcPlayer.thirdPersonAppearance.Posture >= (int) ThirdPersonPosture.Stand &&
                            (int) srcPlayer.thirdPersonAppearance.Posture <= (int) ThirdPersonPosture.ProneToCrouch) &&
                            srcPlayer.thirdPersonAppearance.Action == ThirdPersonAction.Null;
        }


        public static void AddHitVehicleEffectEvent(PlayerEntity srcPlayer, EntityKey target, Vector3 hitPoint,
                                                    Vector3 offset, Vector3 normal)
        {
            HitVehicleEvent e = (HitVehicleEvent) EventInfos.Instance.Allocate(EEventType.HitVehicle, false);
            e.Target   = target;
            e.Offset   = offset;
            e.Normal   = normal;
            e.HitPoint = hitPoint;
            srcPlayer.localEvents.Events.AddEvent(e);
        }

        public static void CreateHitVehicleEffect(ClientEffectContext context, IEntityIdGenerator entityIdGenerator,
                                                  Vector3 hitPoint, EntityKey owner, EntityKey target, Vector3 offset,
                                                  Vector3 normal)
        {
            int type         = (int) EClientEffectType.SteelHit;
            var effectEntity = CreateBaseEntity(context, entityIdGenerator, hitPoint, type);
            effectEntity.AddOwnerId(owner);
            effectEntity.AddNormal(normal);
            effectEntity.AddAttachParent(target, offset);
            effectEntity.lifeTime.LifeTime = GlobalConst.CommonEffectLifeMScd;
            effectEntity.AddFlagImmutability(0);
            effectEntity.isFlagSyncNonSelf = false;
        }

     
        public static ClientEffectEntity CreateGrenadeExplosionEffect(ClientEffectContext context,
                                                                      IEntityIdGenerator entityIdGenerator,
                                                                      EntityKey owner, Vector3 position, float yaw,
                                                                      float pitch, int effectId, int effectTime,
                                                                      EClientEffectType effectType)
        {
            var entity = CreateBaseEntity(context, entityIdGenerator, position, (int) effectType);
            entity.AddOwnerId(owner);
            entity.lifeTime.LifeTime = effectTime;
            entity.AddEffectId(effectId);
            entity.AddEffectRotation(yaw, pitch);
            entity.AddFlagImmutability(0);
            return entity;
        }

        /// <summary>
        ///     create spray paint
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityIdGenerator"></param>
        /// <param name="sprayPaintPos">起始位置</param>
        /// <param name="sprayPaintForward">朝向</param>
        /// <param name="sprayPrintMask">掩码</param>
        /// <param name="sprayPrintSize">大小</param>
        /// <param name="sprayPrintType">类型</param>
        /// <param name="sprayPrintSpriteId">贴图</param>
        /// <param name="lifeTime">生命周期</param>
        public static void CreateSprayPaint(ClientEffectContext context, IEntityIdGenerator entityIdGenerator,
                                            Vector3 sprayPaintPos, Vector3 sprayPaintForward, int sprayPrintMask,
                                            Vector3 sprayPrintSize, ESprayPrintType sprayPrintType,
                                            int sprayPrintSpriteId, int lifeTime)
        {
            int type         = (int) EClientEffectType.SprayPrint;
            var effectEntity = context.CreateEntity();
            var nextId       = entityIdGenerator.GetNextEntityId();
            effectEntity.AddEntityKey(new EntityKey(nextId, (int) EEntityType.ClientEffect));
            effectEntity.AddPosition();
            effectEntity.position.Value = sprayPaintPos;
            effectEntity.AddSprayPaint();
            effectEntity.sprayPaint.SprayPaintPos      = sprayPaintPos;
            effectEntity.sprayPaint.SprayPaintForward  = sprayPaintForward;
            effectEntity.sprayPaint.SprayPrintMask     = sprayPrintMask;
            effectEntity.sprayPaint.SprayPrintSize     = sprayPrintSize;
            effectEntity.sprayPaint.SprayPrintType     = (int) sprayPrintType;
            effectEntity.sprayPaint.SprayPrintSpriteId = sprayPrintSpriteId;

            effectEntity.AddEffectType(type);
            effectEntity.AddAssets(false, false);
            effectEntity.AddLifeTime(DateTime.Now, lifeTime);
            effectEntity.isFlagSyncNonSelf = true;
            /*effectEntity.lifeTime.LifeTime = SingletonManager.Get<ClientEffectCommonConfigManager>().DecayLifeTime;*/
            effectEntity.AddFlagImmutability(0);
        }

        public static ClientEffectEntity CreateBaseEntity(ClientEffectContext context,
                                                          IEntityIdGenerator entityIdGenerator, Vector3 pos, int type)
        {
            var effectEntity = context.CreateEntity();
            var nextId       = entityIdGenerator.GetNextEntityId();
            effectEntity.AddEntityKey(new EntityKey(nextId, (int) EEntityType.ClientEffect));
            effectEntity.AddPosition();
            effectEntity.position.Value = pos;
            effectEntity.AddEffectType(type);
            effectEntity.AddAssets(false, false);
            effectEntity.AddLifeTime(DateTime.Now, 6000);
            effectEntity.isFlagSyncNonSelf = true;
            return effectEntity;
        }


    }
}