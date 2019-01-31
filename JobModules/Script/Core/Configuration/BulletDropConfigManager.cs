using System;
using System.Collections.Generic;
using Core.Utils;
using Utils.Configuration;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace Core.Configuration
{
    public class BulletDropConfigManager : AbstractConfigManager<BulletDropConfigManager>
    {
        private Dictionary<EBulletCaliber, BulletDropConfigItem> _itemDic = new Dictionary<EBulletCaliber, BulletDropConfigItem>(CommonIntEnumEqualityComparer<EBulletCaliber>.Instance);

        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<BulletDropConfig>.Load(xml);
            foreach (var bulletDropConfigItem in cfg.Items)
            {
                _itemDic[bulletDropConfigItem.Type] = bulletDropConfigItem;
            }
        }

        public AssetInfo GetDropEffectAsset(EBulletCaliber caliber)
        {
            var item = GetConfigByCaliber(caliber);
            if (null != item)
            {
                return item.DropAsset;
            }
            Logger.ErrorFormat("Config of caliber {0} does not exist ", caliber);
            return null;
        }

        public AssetInfo GetDropModelAsset(EBulletCaliber caliber)
        {
            var item = GetConfigByCaliber(caliber);
            if (null != item)
            {
                return item.ModelAsset;
            }

            return null;
        }

        public BulletDropConfigItem GetConfigByCaliber(EBulletCaliber caliber)
        {
            if (_itemDic.ContainsKey((caliber)))
            {
                return _itemDic[caliber];
            }

            return null;
        }

    }
}
