using System;
using App.Shared.Components;
using Core.Configuration.Sound;
using Core.EntityComponent;
using Core.GameTime;
using Core.IFactory;
using Core.Utils;
using Entitas;
using UnityEngine;
using XmlConfig;

namespace App.Shared.EntityFactory
{
    public class ClientSoundEntityFactory : ISoundEntityFactory
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientSoundEntityFactory));
        private SoundContext _soundContext;
        private IEntityIdGenerator _idGenerator;
        private ICurrentTime _currentTime;
        private ISoundConfigManager _soundConfigManager;

        public ClientSoundEntityFactory(SoundContext context, 
            IEntityIdGenerator idGenerator, 
            ICurrentTime currentTime, 
            ISoundConfigManager soundConfigManager)
        {
            _soundContext = context;
            _idGenerator = idGenerator;
            _currentTime = currentTime;
            _soundConfigManager = soundConfigManager;
        }

        public virtual Entity CreateSelfOnlyMoveSound(Vector3 startPosition, EntityKey target, int id, bool loop)
        {
            var entity = CreateSelfOnlySound(id, startPosition, loop) as SoundEntity;
            if(null != entity)
            {
                entity.AddParent(target);
            }
            return entity;
        }

        private Entity CreateSelfOnlySoundEntity(int id)
        {
            var config = _soundConfigManager.GetSoundById(id);
            if (config == null)
            {
                Logger.ErrorFormat("config with sound type {0} not exist", id);
                return null;
            }
            var entity = CreateBaseSoundEntity(config);
            return entity;
        }

        public virtual Entity CreateSelfOnlySound(int id, bool loop)
        {
            var entity = CreateSelfOnlySoundEntity(id) as SoundEntity;
            if(entity!= null && loop)
            {
                entity.AddSoundPlayInfo();
                entity.soundPlayInfo.Loop = true;
            }
            return entity;
        }

        public virtual Entity CreateSelfOnlySound(int id, Vector3 position, bool loop)
        {
            var entity = CreateSelfOnlySound(id, loop) as SoundEntity; 
            if(null != entity)
            {
                entity.AddPosition(position);
            }
            return entity;
        }

        public virtual Entity CreateSound(int id, bool loop)
        {
            var config = _soundConfigManager.GetSoundById(id);
            if (config == null)
            {
                Logger.ErrorFormat("config with sound type {0} not exist", id);
                return null;
            }

            return CreateBaseSoundEntity(config);
        }

        private SoundEntity CreateBaseSoundEntity(SoundConfigItem config)
        {
            var entity = _soundContext.CreateEntity(); 
            entity.AddEntityKey(new EntityKey(_idGenerator.GetNextEntityId(), (int)EEntityType.Sound));
            entity.AddTimeInfo(_currentTime.CurrentTime + config.Delay);
            entity.AddAssetInfo(config.Asset.AssetName, config.Asset.BundleName);
            return entity;
        }

        public virtual Entity CreateSyncSound(Entity owner, SoundConfigItem soundConfig, bool loop)
        {
            var player = owner as PlayerEntity;
            if(null == player)
            {
                Logger.ErrorFormat("CreateNonSyncSound failed , owner {0} is not player or null", owner);
                return null;
            }
 
            var entity = CreateBaseSoundEntity(soundConfig);
            if(null != entity)
            {
                entity.AddParent(player.entityKey.Value);
            }
            return entity;
        }

        public virtual Entity CreateNonSyncSound(Entity owner, SoundConfigItem soundConfig, bool loop)
        {
            var player = owner as PlayerEntity;
            if(null == player)
            {
                Logger.ErrorFormat("CreateNonSyncSound failed , owner {0} is not player or null", owner);
                return null;
            }
            var entity = CreateBaseSoundEntity(soundConfig);
            if(null != entity)
            {
                entity.AddParent(player.entityKey.Value);
            }
            return entity;
        }
    }
}