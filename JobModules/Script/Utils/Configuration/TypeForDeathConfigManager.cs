using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.AssetManager;
using XmlConfig;

namespace Utils.Configuration
{
 
    public class TypeForDeathConfigManager : AbstractConfigManager<TypeForDeathConfigManager>
    {

        private Dictionary<int, TypeForDeath> _configs = new Dictionary<int, TypeForDeath>();
        private TypeForDeathConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<TypeForDeathConfig>.Load(xml);
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

        public TypeForDeath GetConfigById(int id)
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
                AssetName = cfg.CardName,
                BundleName = cfg.BundleName,
            };
        }

        public AssetInfo GetKillIconAssetInfoById(int id)
        {
            var cfg = GetConfigById(id);
            if (null == cfg)
            {
                return new AssetInfo();
            }
            return new AssetInfo
            {
                AssetName = cfg.KillIcon,
                BundleName = cfg.BundleName,
            };
        }


        public TypeForDeathConfig GetConfig()
        {
            return _config;
        }
    }
}
