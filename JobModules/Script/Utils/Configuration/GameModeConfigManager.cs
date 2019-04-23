using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public interface IGameModeConfigManager
    {
        int GetBagLimitTime(int modeId);
    }
    public class GameModeConfigManager : AbstractConfigManager<GameModeConfigManager>, IGameModeConfigManager
    {
        private GameModeConfig _config;
        private Dictionary<int, GameModeConfigItem> _configCache = new Dictionary<int, GameModeConfigItem>();

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<GameModeConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach(var item in _config.Items)
            {
                _configCache[item.Id] = item;
            }
        }

        public EBagType GetBagTypeById(int modeId)
        {
            var config = GetConfigById(modeId); 
            if(null == config)
            {
                return EBagType.Group;
            }
            return (EBagType)config.BagType;
        }

        public int GetBagLimitTime(int modeId)
        {
            var config = GetConfigById(modeId);
            if(null == config)
            {
                return 0; 
            }
            return config.ChangeBag * 1000;
        }

        public int GetWepaonStayTime(int modeId)
        {
            var config = GetConfigById(modeId);
            if(null == config)
            {
                return 0;
            }
            return config.WeaponStayTime * 1000;
        }

        public GameModeConfigItem GetConfigById(int modeId)
        {
            if(!_configCache.ContainsKey(modeId))
            {
                Logger.ErrorFormat("mode id {0} doesn't exist in config ", modeId);
                return null;
            }
            return _configCache[modeId];
        }
    }
}
