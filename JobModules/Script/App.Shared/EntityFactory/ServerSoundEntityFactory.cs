using App.Shared.Components;
using App.Shared.Sound;
using Core.Configuration.Sound;
using Core.EntityComponent;
using Core.GameTime;
using Core.Utils;
using Entitas;
using UnityEngine;
using XmlConfig;

namespace App.Shared.EntityFactory
{
    public class ServerSoundEntityFactory : ClientSoundEntityFactory
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerSoundEntityFactory));
        private PlayerContext _playerContext;
        public ServerSoundEntityFactory(SoundContext context,
            PlayerContext playerContext,
            IEntityIdGenerator idGenerator, 
            ICurrentTime currentTime,
            ISoundConfigManager soundConfigManager) : base(context, idGenerator, currentTime, soundConfigManager)
        {
            _playerContext = playerContext;
        }

        public override Entity CreateSelfOnlyMoveSound(Vector3 startPosition, EntityKey target, int id, bool loop)
        {
            return null;
        }

        public override Entity CreateSelfOnlySound(int id, bool loop)
        {
            return null;
        }

        public override Entity CreateSelfOnlySound(int id, Vector3 position, bool loop)
        {
            return null;
        }

        public override Entity CreateNonSyncSound(Entity owner, SoundConfigItem soundConfig, bool loop)
        {
            return null;
        }

        public override Entity CreateSyncSound(Entity entity, SoundConfigItem soundConfig, bool loop)
        {
            var playerEntity = entity as PlayerEntity;
            if(null == playerEntity)
            {
                Logger.ErrorFormat("entity {0} is not player or null", entity);
            }

            if(loop)
            {
                SoundSyncUtil.LoopSoundToComponent((short)soundConfig.Id, playerEntity.sound);
            }
            else
            {
                SoundSyncUtil.NonLoopSoundToComponent((short)soundConfig.Id, playerEntity.sound);
            }
            return null;
        }
    }
}
