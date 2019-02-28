using System;
using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.Player.Events;
using Core.Configuration;
using Core.EntityComponent;
using Core.Event;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.EntityFactory
{
    public class ClientEffectFactory
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientEffectFactory));

        // 越后越新
        private static LinkedList<ClientEffectEntity> _bulletDropEntities = new LinkedList<ClientEffectEntity>();

        public static void OnDestroy(ClientEffectEntity entity)
        {
            if (entity.hasEffectType && entity.effectType.Value == (int) EClientEffectType.BulletDrop)
            {
                _bulletDropEntities.Remove(entity);
            }
        }

        public static ClientEffectEntity CreateBaseEntity(ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator, Vector3 pos, int type)
        {
            var effectEntity = context.CreateEntity();
            var nextId = entityIdGenerator.GetNextEntityId();
            effectEntity.AddEntityKey(new EntityKey(nextId, (int) EEntityType.ClientEffect));
            effectEntity.AddPosition(pos);
            effectEntity.AddEffectType(type);
            effectEntity.AddLifeTime(DateTime.Now, 6000);
            effectEntity.isFlagSyncNonSelf = true;
            return effectEntity;
        }

        public static void CreateBulletDrop(ClientEffectContext context, IEntityIdGenerator idGenerator,
            EntityKey owner, Vector3 position, float Yaw, float pitch, int effectId)
        {
            while (_bulletDropEntities.Count >= SingletonManager.Get<ClientEffectCommonConfigManager>().GetBulletDropMaxCount(SharedConfig.IsServer))
            {
                var val = _bulletDropEntities.First.Value;
                if (val.isEnabled)
                {
                    val.isFlagDestroy = true;
                }

                _bulletDropEntities.RemoveFirst();
            }

            var type = (int) EClientEffectType.BulletDrop;
            var entity = CreateBaseEntity(context, idGenerator, position, type);
            entity.AddEffectId(effectId);
            entity.AddOwnerId(owner);
            entity.lifeTime.LifeTime = SingletonManager.Get<ClientEffectCommonConfigManager>().BulletDropLifeTime;
            entity.AddEffectRotation(Yaw, pitch);
            entity.AddFlagImmutability(0);
            _bulletDropEntities.AddLast(entity);
        }

       

        public static void AdHitEnvironmentEffectEvent(PlayerEntity srcPlayer, Vector3 hitPoint, Vector3 offset,  EEnvironmentType environmentType)
        {
            HitEnvironmentEvent e = (HitEnvironmentEvent)EventInfos.Instance.Allocate(EEventType.HitEnvironment, false);
            e.EnvironmentType = environmentType;
            e.Offset = offset;
          
            e.HitPoint = hitPoint;
            srcPlayer.localEvents.Events.AddEvent(e);
        }
        public static void CreateHitEnvironmentEffect(
            ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            Vector3 hitPoint,
            Vector3 normal,
            EntityKey owner,
            EEnvironmentType environmentType)
        {
            Logger.DebugFormat("CreateBulletHitEnvironmentEffet ", environmentType);

            CreateHitEmitEffect(context, entityIdGenerator, hitPoint, normal, owner, environmentType);

            Logger.DebugFormat("EnvType {0} ", environmentType);
            
        }

        public static void CreateMuzzleSparkEffct(
            ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            EntityKey owner,
            Transform parent,
            float pitch,
            float yaw,
            int effectId)
        {
            var entity = CreateBaseEntity(context, entityIdGenerator, parent.position,
                (int) EClientEffectType.MuzzleSpark);
            entity.AddEffectId(effectId);
            entity.AddOwnerId(owner);
            entity.lifeTime.LifeTime = 500;
            entity.AddAttachParent(owner, Vector3.zero);
            entity.AddEffectRotation(yaw, pitch);
            entity.isFlagSyncNonSelf = false;
        }

        private static void CreateHitEmitEffect(
            ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            Vector3 hitPoint,
            Vector3 normal,
            EntityKey owner,
            EEnvironmentType environmentType)
        {
            int type = (int) EClientEffectType.End;
            switch (environmentType)
            {
                case EEnvironmentType.Wood:
                    type = (int) EClientEffectType.WoodHit;
                    break;
                case EEnvironmentType.Steel:
                    type = (int) EClientEffectType.SteelHit;
                    break;
                case EEnvironmentType.Soil:
                    type = (int) EClientEffectType.SoilHit;
                    break;
                case EEnvironmentType.Stone:
                    type = (int) EClientEffectType.StoneHit;
                    break;
                case EEnvironmentType.Glass:
                    type = (int) EClientEffectType.GlassHit;
                    break;
                case EEnvironmentType.Water:
                    type = (int) EClientEffectType.WaterHit;
                    break;
                default:
                    type = (int) EClientEffectType.DefaultHit;
                    break;
            }

            var effectEntity = CreateBaseEntity(context, entityIdGenerator, hitPoint, type);
            effectEntity.AddOwnerId(owner);
            effectEntity.AddNormal(normal);
            effectEntity.lifeTime.LifeTime = SingletonManager.Get<ClientEffectCommonConfigManager>().DecayLifeTime;
            effectEntity.AddFlagImmutability(0);
            effectEntity.isFlagSyncNonSelf = false;
        }

        public static void AddBeenHitEvent(PlayerEntity srcPlayer, EntityKey target, int damageId, int triggerTime)
        {
            BeenHitEvent e = (BeenHitEvent) EventInfos.Instance.Allocate(EEventType.BeenHit, false);
            e.Target = target;
            e.UniqueId = damageId;
            e.TriggerTime = triggerTime;
            srcPlayer.localEvents.Events.AddEvent(e);
        }

        public static void AddHitPlayerEffectEvent(PlayerEntity srcPlayer, EntityKey target, Vector3 hitPoint, Vector3 offset)
        {
            HitPlayerEvent e = (HitPlayerEvent)EventInfos.Instance.Allocate(EEventType.HitPlayer, false);
            e.Target = target;
            e.Offset = offset;
          
            e.HitPoint = hitPoint;
            srcPlayer.localEvents.Events.AddEvent(e);
        }
        
        public static void CreateHitPlayerEffect(
            ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            Vector3 hitPoint,
            EntityKey owner,
            EntityKey target,
            Vector3 offset)
        {
            int type = (int) EClientEffectType.HumanHitEffect;
            var effectEntity = CreateBaseEntity(context, entityIdGenerator, hitPoint, type);
            effectEntity.AddOwnerId(owner);
            effectEntity.AddAttachParent(target, offset);
            effectEntity.isFlagSyncNonSelf = false;
            Logger.DebugFormat("CreateHitPlayerEffect {0} {1}", effectEntity.entityKey.Value,effectEntity.isFlagSyncNonSelf );
        }

       
        

        public static void AddHitVehicleEffectEvent(PlayerEntity srcPlayer, EntityKey target, Vector3 hitPoint, Vector3 offset,
            Vector3 normal)
        {
            HitVehicleEvent e = (HitVehicleEvent)EventInfos.Instance.Allocate(EEventType.HitVehicle, false);
            e.Target = target;
            e.Offset = offset;
            e.Normal = normal;
            e.HitPoint = hitPoint;
            srcPlayer.localEvents.Events.AddEvent(e);
        }
        public static void CreateHitVehicleEffect(
            ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            Vector3 hitPoint,
            EntityKey owner,
            EntityKey target,
            Vector3 offset,
            Vector3 normal)
        {
            int type = (int) EClientEffectType.SteelHit;
            var effectEntity = CreateBaseEntity(context, entityIdGenerator, hitPoint, type);
            effectEntity.AddOwnerId(owner);
            effectEntity.AddNormal(normal);
            effectEntity.AddAttachParent(target, offset);
            effectEntity.lifeTime.LifeTime = SingletonManager.Get<ClientEffectCommonConfigManager>().DecayLifeTime;
            effectEntity.AddFlagImmutability(0);
            effectEntity.isFlagSyncNonSelf = false;
        }

        public static void CreateHitFracturedChunkEffect(
            ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            Vector3 hitPoint,
            EntityKey owner,
            EntityKey target,
            int fragmentId,
            Vector3 offset,
            Vector3 normal)
        {
            int type = (int) EClientEffectType.SteelHit;
            var effectEntity = CreateBaseEntity(context, entityIdGenerator, hitPoint, type);
            effectEntity.AddOwnerId(owner);
            effectEntity.AddNormal(normal);
            effectEntity.AddAttachParent(target, offset);
            effectEntity.attachParent.FragmentId = fragmentId;
            effectEntity.lifeTime.LifeTime = SingletonManager.Get<ClientEffectCommonConfigManager>().DecayLifeTime;
            effectEntity.AddFlagImmutability(0);
        }

        public static void CreateGrenadeExplosionEffect(ClientEffectContext context,
            IEntityIdGenerator entityIdGenerator,
            EntityKey owner, Vector3 position, float yaw, float pitch, int effectId, int effectTime,
            EClientEffectType effectType)
        {
            var entity = CreateBaseEntity(context, entityIdGenerator, position, (int) effectType);
            entity.AddOwnerId(owner);
            entity.lifeTime.LifeTime = effectTime;
            entity.AddEffectId(effectId);
            entity.AddEffectRotation(yaw, pitch);
            entity.AddFlagImmutability(0);
        }
    }
}