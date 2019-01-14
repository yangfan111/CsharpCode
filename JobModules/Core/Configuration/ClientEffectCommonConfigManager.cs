using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;

namespace Core.Configuration
{
    public class ClientEffectCommonConfigManager : AbstractConfigManager<ClientEffectCommonConfigManager>
    {
        private Dictionary<EClientEffectType, ClientEffectCommonConfigItem> _configs = new Dictionary<EClientEffectType, ClientEffectCommonConfigItem>(CommonEnumEqualityComparer<EClientEffectType>.Instance);
        public override bool IsServer { get; set; }
        public int BulletDropLifeTime { get; private set; }
        public int DecayLifeTime { get; private set; }

        public int BulletDropMaxCount
        {
            get { return IsServer ? _bulletDropServerMaxCount : _bulletDropClientMaxCount; }
        }

        private int _bulletDropClientMaxCount;
        private int _bulletDropServerMaxCount;

        public ClientEffectCommonConfigManager()
        {
            // 默认值，测试或者配置出错时使用
            _bulletDropClientMaxCount = 30;
            _bulletDropServerMaxCount = 100;
            BulletDropLifeTime = 30000;
        }


        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<ClientEffectCommonConfig>.Load(xml);
            foreach (var item in cfg.Items)
            {
                if (_configs.ContainsKey(item.Type))
                {
                    Logger.ErrorFormat("Duplicate define for type {0}", item.Type);
                }
                _configs[item.Type] = item;

                switch (item.Type)
                {
                    case EClientEffectType.BulletDrop:
                        _bulletDropServerMaxCount = item.ServerLimit;
                        _bulletDropClientMaxCount = item.ClientLimit;
                        BulletDropLifeTime = item.LifeTime;
                        break;
                   case EClientEffectType.DefaultHit:
                        DecayLifeTime = item.LifeTime;
                        break;
                }
            }
        }

        public ClientEffectCommonConfigItem GetConfigByType(EClientEffectType type)
        {
            if (_configs.ContainsKey(type))
            {
                return _configs[type];
            }
            else
            {
                Logger.ErrorFormat("{0} does not exist in config" , type);
                return null;
            }
        }
    }
}