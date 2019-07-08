using Core.Utils;
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
            foreach (var item in cfg.Items)
            {
                _typeDic[item.Type] = item;
                _configs[item.Id] = item;
            }
        }

        public ClientEffectConfigItem GetConfigItemById(int id)
        {
            if (id == 0)
                return null;
            ClientEffectConfigItem efcItem;
            _configs.TryGetValue(id, out efcItem);
            AssertUtility.Assert(efcItem != null,string.Format("config with id {0} doesn't exist ! ",id));
            return efcItem;
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