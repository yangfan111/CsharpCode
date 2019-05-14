using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public class WeaponPartSurvivalConfigManager : AbstractConfigManager<WeaponPartSurvivalConfigManager>, IConfigParser
    {
        public Dictionary<int, WeaponPartSurvivalConfigItem> _configDic = new Dictionary<int, WeaponPartSurvivalConfigItem>();

        public override void ParseConfig(string xml)
        {
            var config = XmlConfigParser<WeaponPartSurvivalConfig>.Load(xml);
            if(null == config)
            {
                Logger.Error("weapon part survival config is null !!");
                return;
            }
            foreach(var item in config.Items)
            {
                _configDic[item.Id] = item;
            }
        }

      
        public float GetSizeById(int id)
        {
            var cfg = FindConfigBySetId(id);
            if(null != cfg)
            {
                return cfg.Size;
            }
            return 1f;
        }

        public int GetDefaultPartBySetId(int id)
        {
            var array = FindConfigBySetId(id).PartsList;
            return array.Length == 0 ? 0 : array[0];
        
        }

        public Dictionary<int, WeaponPartSurvivalConfigItem> GetConfigs()
        {
            return _configDic;
        }

        public WeaponPartSurvivalConfigItem FindConfigBySetId(int id)
        {
            WeaponPartSurvivalConfigItem item;
            _configDic.TryGetValue(id, out item);
            return item;
        }
    }
}
