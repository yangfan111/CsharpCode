using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using XmlConfig;

namespace Utils.Configuration
{

    public class GlobalDataConfigManager : AbstractConfigManager<GlobalDataConfigManager>
    {
        private Dictionary<int, GlobalDataItem> _configs = new Dictionary<int, GlobalDataItem>();

        public GlobalDataConfigManager()
        {
        }
        private GlobalDataConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<GlobalDataConfig>.Load(xml);
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
        public int GetIntValue(int id)
        {
            int value = 0;
            GlobalDataItem item = GetItemById(id);
            if (item != null)
            {
                int.TryParse(item.Value, out value);
            }
            return value;
        }

        private GlobalDataItem GetItemById(int id)
        {
            GlobalDataItem newItem;
            _configs.TryGetValue(id, out newItem);
            return newItem;
        }

        public long GetLongValue(int id)
        {
            long value = 0;
            GlobalDataItem item = GetItemById(id);
            if (item != null)
            {
                long.TryParse(item.Value, out value);
            }
            return value;
        }

        public float GetFloatValue(int id)
        {
            float value = 0.0f;
            GlobalDataItem item = GetItemById(id);
            if (item != null)
            {
                float.TryParse(item.Value, out value);
            }
            return value;
        }

        public string GetStrValue(int id)
        {
            string value = string.Empty;
            GlobalDataItem item = GetItemById(id);
            if (item != null)
            {
                value = item.Value;
            }
            return value;
        }

        public List<int> GetIntListValue(int id)
        {
            string value = string.Empty;
            GlobalDataItem item = GetItemById(id);
            if (item != null)
            {
                return item.Value.ToList('|', (str) => int.Parse(str));
            }
            return null;
        }


    }
}
