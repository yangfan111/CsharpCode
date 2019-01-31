using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;

namespace Core.Configuration
{
    public class ClientEffectConfigManager :AbstractConfigManager<ClientEffectConfigManager>
    {
        //方便拿一些对应类型只有一个对象的配置
        private Dictionary<EClientEffectType, ClientEffectConfigItem> _typeDic = new Dictionary<EClientEffectType, ClientEffectConfigItem>(CommonIntEnumEqualityComparer<EClientEffectType>.Instance);
        private Dictionary<int, ClientEffectConfigItem> _configs = new Dictionary<int, ClientEffectConfigItem>();

        public override void ParseConfig(string xml)
        {
            _typeDic.Clear();
            _configs.Clear();
            var cfg = XmlConfigParser<ClientEffectConfig>.Load(xml);
            for (var i = 0; i < cfg.Items.Length; i++)
            {
                _typeDic[cfg.Items[i].Type] = cfg.Items[i];
                if(_configs.ContainsKey(cfg.Items[i].Id))
                {
                    Logger.ErrorFormat("id {0} is repeated ", cfg.Items[i].Id);
                }
                _configs[cfg.Items[i].Id] = cfg.Items[i];
            }
        }

        public ClientEffectConfigItem GetConfigItemById(int id)
        {
            if(!_configs.ContainsKey(id))
            {
                Logger.ErrorFormat("config with id {0} doesn't exist ! ", id);
                return null;
            }
            return _configs[id];
        }

        public ClientEffectConfigItem GetConfigItemByType(EClientEffectType type)
        {
            if (!_typeDic.ContainsKey(type))
            {
                return null;
            }

            if (null == _typeDic[type])
            {
                Logger.ErrorFormat("config with type {0} is null ! ", type);
            }
            return _typeDic[type];
        }
    }
}