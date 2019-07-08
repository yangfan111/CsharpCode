using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;

namespace Core.Configuration
{
    public class ClientEffectCommonConfigManager : AbstractConfigManager<ClientEffectCommonConfigManager>
    {
        private Dictionary<EEffectObjectClassify, ClientEffectCommonConfigItem> _configs = new Dictionary<EEffectObjectClassify, ClientEffectCommonConfigItem>(CommonIntEnumEqualityComparer<EEffectObjectClassify>.Instance);
        


        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<ClientEffectCommonConfig>.Load(xml);
            foreach (var item in cfg.Items)
                _configs[item.Type] = item;
        }

        public ClientEffectCommonConfigItem GetConfigByType(EEffectObjectClassify type)
        {
            ClientEffectCommonConfigItem item;
            _configs.TryGetValue(type, out item);
            return item;
        }

        // public int GetBulletDropMaxCount(bool isServer)
        // {
        //     return isServer ? BulletDropServerMaxCount : BulletDropClientMaxCount;
        // }
    }
}