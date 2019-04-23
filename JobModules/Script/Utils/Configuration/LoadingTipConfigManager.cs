using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public class LoadingTipConfigManager : AbstractConfigManager<LoadingTipConfigManager>
    {
        public enum LoadingTipType
        {
            Hall = 1,
            Client = 2,
        }
        private Dictionary<int, LoadingTip> _configs = new Dictionary<int, LoadingTip>();
        private LoadingTipConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<LoadingTipConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach (var item in _config.Items)
            {
                _configs[item.Id] = item;
            }
        }

        public LoadingTip GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null;
            }
            return _configs[id];
        }

        public LoadingTipConfig GetConfig()
        {
            return _config;
        }
    }
}
