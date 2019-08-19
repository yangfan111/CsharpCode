using System.Collections.Generic;
using Utils.AssetManager;
using XmlConfig;

namespace Utils.Configuration
{
    public class WeaponWorkShopConfigManager : AbstractConfigManager<WeaponWorkShopConfigManager>
    {

        private Dictionary<int, WeaponWorkshop> _configs = new Dictionary<int, WeaponWorkshop>();
        private WeaponWorkshopConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<WeaponWorkshopConfig>.Load(xml);
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

        public WeaponWorkshop GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in  config ", id);
                return null;
            }
            return _configs[id];
        }

        public AssetInfo GetWorkShopIcon(int workShop)
        {
            AssetInfo assetIconInfo;
            var cfg = GetConfigById(workShop);
            assetIconInfo = cfg != null ? new AssetInfo(cfg.IconBundle, cfg.Icon) : new AssetInfo(string.Empty, string.Empty);
            return assetIconInfo;
        }
    }
}
