using XmlConfig;
using System.Collections.Generic;
using Utils.Configuration;

namespace Core.Configuration
{
    public class ClipDropConfigManager : AbstractConfigManager<ClipDropConfigManager>
    {
        private ClipDropConfig _config;
        private Dictionary<int, ClipDropConfigItem> _configDic = new Dictionary<int, ClipDropConfigItem>();

        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<ClipDropConfig>.Load(xml);
            _config = cfg; 
            foreach(var item in cfg.Items)
            {
                _configDic[item.Id] = item;
            }
        }

        public ClipDropConfigItem GetConfigByWeapon(int id)
        {
            if(_configDic.ContainsKey(id))
            {
                return _configDic[id];
            }
            Logger.WarnFormat("weapon {0} does not exist in clip drop config ", id);
            return null;
        }
    }
}
