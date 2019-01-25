using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.IFactory;
using Core.Sound;
using Core.Utils;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.Sound
{
    public class DummyPlayerSoundManager : IPlayerSoundManager
    {
        public int Play(EPlayerSoundType type)
        {
            return -1;
        }

        public int Play(int id)
        {
            return -1;
        }

        public void Stop(ref int id)
        {
            return;
        }

        public int PlayOnce(EPlayerSoundType soundType)
        {
            return -1;
        }

        public int PlayOnce(int id)
        {
            return -1;
        }
    }
    public class PlayerSoundManager : IPlayerSoundManager
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerSoundManager));
        private ISoundConfigManager _soundConfigManager;
        private ISoundEntityFactory _soundEntityFactory;
        private SoundContext _soundContext;
        private PlayerEntity _playerEntity;
        private SoundConverter _soundConvert;

        public PlayerSoundManager(PlayerEntity playerEntity,
            SoundContext soundContext,
            IPlayerSoundConfigManager playerSoundConfigManager,
            ISoundConfigManager soundConfigManager,
            ISoundEntityFactory soundEntityFactory,
            ITerrainManager terrainManager,
            IMapConfigManager mapConfigManager)
        {
            _soundConfigManager = soundConfigManager;
            _soundEntityFactory = soundEntityFactory;
            _playerEntity = playerEntity;
            _soundContext = soundContext;
            _soundConvert = new SoundConverter(soundConfigManager,
                playerSoundConfigManager,
                terrainManager,
                mapConfigManager);
        }

        public int Play(int soundId)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("play {0}", soundId);
            }
            if(null == _playerEntity)
            {
                return UniversalConsts.InvalidIntId;
            }

            var soundConfigItem = _soundConfigManager.GetSoundById(soundId);
            if(null == soundConfigItem)
            {
                return UniversalConsts.InvalidIntId;
            }
            var entity = soundConfigItem.Sync ?
                _soundEntityFactory.CreateSyncSound(_playerEntity, soundConfigItem, true) :
                _soundEntityFactory.CreateSelfOnlySound(soundConfigItem.Id,
                    _playerEntity.position.Value,
                    true);

            var soundEntity = entity as SoundEntity;
            if(null == soundEntity)
            {
                return UniversalConsts.InvalidIntId; 
            }
            return soundEntity.entityKey.Value.EntityId;
        }

        public int Play(EPlayerSoundType sound)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("play {0}", sound);
            }
            if(null == _playerEntity)
            {
                return UniversalConsts.InvalidIntId;
            }

            var soundConfigItem = _soundConvert.Convert(_playerEntity, sound);
            if(null == soundConfigItem)
            {
                return UniversalConsts.InvalidIntId;
            }
            var entity = soundConfigItem.Sync ?
                _soundEntityFactory.CreateSyncSound(_playerEntity, soundConfigItem, true) :
                _soundEntityFactory.CreateSelfOnlySound(soundConfigItem.Id,
                    _playerEntity.position.Value,
                    true);

            var soundEntity = entity as SoundEntity;
            if(null == soundEntity)
            {
                return UniversalConsts.InvalidIntId; 
            }
            return soundEntity.entityKey.Value.EntityId;
        }

        public int PlayOnce(int soundId)
        {
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("PlayOnce {0}", soundId);
            }
            var soundConfigItem = _soundConfigManager.GetSoundById(soundId);
            return PlayOnce(soundConfigItem);
        }

        public int PlayOnce(EPlayerSoundType sound)
        {
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("PlayOnce {0}", sound);
            }
            if(null == _playerEntity)
            {
                return UniversalConsts.InvalidIntId;
            }

            var soundCfg = _soundConvert.Convert(_playerEntity, sound);
            return PlayOnce(soundCfg);
        }

        private int PlayOnce(SoundConfigItem soundConfig)
        {
            
            if (null == soundConfig)
            {
                Logger.Error("sound config to play is null !");
                return UniversalConsts.InvalidIntId;
            }

            var entity = soundConfig.Sync ?
                _soundEntityFactory.CreateSyncSound(_playerEntity, soundConfig, false) :
                _soundEntityFactory.CreateNonSyncSound(_playerEntity, soundConfig, false);
            var soundEntity = entity as SoundEntity;
            if(null == soundEntity)
            {
                return UniversalConsts.InvalidIntId; 
            }
            soundEntity.isPlayOnce = true;
            return soundEntity.entityKey.Value.EntityId;
        }

        public void Stop(ref int id)
        {
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("stop {0}", id);
            }
            var soundEntity = _soundContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(id, (short)EEntityType.Sound));
            if(null != soundEntity)
            {
                soundEntity.isFlagDestroy = true;
                id = UniversalConsts.InvalidIntId;
            }
        }
    }
}