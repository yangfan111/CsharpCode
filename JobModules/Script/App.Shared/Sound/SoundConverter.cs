using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Enums;
using Core.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.Sound
{
    public interface ISoundConverter
    {
        SoundConfigItem Convert(PlayerEntity playerEntity, EPlayerSoundType playerSoundType);
    }

    public class SoundConverter : ISoundConverter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SoundConverter));
        private ISoundConfigManager _soundConfigManager;
        private IPlayerSoundConfigManager _playerSoundConfigManager;
        private ITerrainManager _terrainManager;
        private IMapConfigManager _mapConfigManager;

        public SoundConverter(ISoundConfigManager soundConfigManager,
            IPlayerSoundConfigManager playerSoundConfigManager,
            ITerrainManager terrainManager,
            IMapConfigManager mapConfigManager)
        {
            _soundConfigManager = soundConfigManager;
            _playerSoundConfigManager = playerSoundConfigManager;
            _terrainManager = terrainManager;
            _mapConfigManager = mapConfigManager;
        }

        public SoundConfigItem Convert(PlayerEntity playerEntity, EPlayerSoundType playerSoundType)
        {
            switch((EPlayerSoundType)playerSoundType)
            {
                case EPlayerSoundType.Walk:
                    return ConvertTerrainSound(playerEntity, ETerrainSoundType.Walk);
                case EPlayerSoundType.Squat:
                    return ConvertTerrainSound(playerEntity, ETerrainSoundType.Squat);
                case EPlayerSoundType.Crawl:
                    return ConvertTerrainSound(playerEntity, ETerrainSoundType.Crawl);
                case EPlayerSoundType.Land:
                    return ConvertTerrainSound(playerEntity, ETerrainSoundType.Land);
                default:
                    return ConvertPlayerSound(playerEntity, playerSoundType);
            }
        }

        private SoundConfigItem ConvertTerrainSound(PlayerEntity playerEntity, ETerrainSoundType soundType)
        {
            var myTerrain = _terrainManager.GetTerrain(_mapConfigManager.SceneParameters.Id);
            var id = myTerrain.GetSoundId(playerEntity.position.Value, soundType); 
            if (id > 0)
            {
                return _soundConfigManager.GetSoundById(id);
            }
            else
            {
                if (Logger.IsWarnEnabled)
                {
                    Logger.WarnFormat("no {0} sound assigned for terrain {1} in pos {2}", soundType, myTerrain, playerEntity.position.Value);
                }
                return null;
            }
        }

        private SoundConfigItem ConvertPlayerSound(PlayerEntity playerEntity, EPlayerSoundType soundType)
        {
            var id = _playerSoundConfigManager.GetSoundIdByType(soundType);
            return _soundConfigManager.GetSoundById(id);
        }
    }
}
