using System.Collections.Generic;
using Utils.AssetManager;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Configuration
{
    public class IndividuationConfigManager : AbstractConfigManager<IndividuationConfigManager>
    {
        private Dictionary<int, Individuation> _configs = new Dictionary<int, Individuation>();
        private IndividuationConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<IndividuationConfig>.Load(xml);
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

        public Individuation GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null;
            }
            return _configs[id];
        }

        public AssetInfo GetCardAssetInfoById(int id)
        {
            var cfg = GetConfigById(id);
            if (null == cfg)
            {
                return new AssetInfo();
            }
            return new AssetInfo
            {
                AssetName = cfg.Icon,
                BundleName = cfg.IconBundle,
            };
        }

        public IndividuationConfig GetConfig()
        {
            return _config;
        }

        public static IndividuationConfigManager GetInstance()
        {
            return SingletonManager.Get<IndividuationConfigManager>();
        }
    }
}
