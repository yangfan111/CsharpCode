using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.AssetManager;
using XmlConfig;

namespace Utils.Configuration
{
    public enum CardItemType
    {
        None,
        Back,
        Title,
        Badge
    }
    public class CardConfigManager : AbstractConfigManager<CardConfigManager>
    {

        private Dictionary<int, Card> _configs = new Dictionary<int, Card>();
        private CardConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<CardConfig>.Load(xml);
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

        public Card GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null;
            }
            return _configs[id];
        }

        public AssetInfo GetAssetInfoById(int id)
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

        public CardItemType GetCardItemTypeById(int id)
        {
            var cfg = GetConfigById(id);
            if (null == cfg)
            {
                return CardItemType.None;
            }

            return (CardItemType) cfg.Type;
        }


        public CardConfig GetConfig()
        {
            return _config;
        }
    }
}
