using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration
{
    public class VehicleAssetConfigManager : AbstractConfigManager<VehicleAssetConfigManager>
    {
        private Dictionary<int, VehicleAssetConfigItem> _items = new Dictionary<int, VehicleAssetConfigItem>();

        public VehicleAssetConfigItem GetConfigItem(int id)
        {
            return _items[id];
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.ErrorFormat("vehicle asset config is Empty");
                return;
            }

            var cfg = XmlConfigParser<VehicleAssetConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("vehicle asset config is illegal content:{0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                if (_items.ContainsKey(item.Id))
                {
                    Logger.ErrorFormat("the vehicle asset id has been assigned {0}", item.Id);
                    continue;
                }

                _items[item.Id] = item;
            }

        }
    }
}
