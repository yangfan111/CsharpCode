using Core.Utils;
using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public class SettingConfigManager : AbstractConfigManager<SettingConfigManager>
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SettingConfigManager));
        private Dictionary<int, SettingConfigItem> _configDic = new Dictionary<int, SettingConfigItem>();

        public Dictionary<int, SettingConfigItem> ConfigDic { get { return _configDic; } set { _configDic = value; } }

        public SettingConfigManager()
        {
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("SettingUI config xml is empty !");
                return;
            }
            _configDic.Clear();
            var cfg = XmlConfigParser<SettingConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("RoleAsset config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                _configDic.Add(item.Id, item);
            }
        }

        public SettingConfigItem GetItemById(int id)
        {
            SettingConfigItem ret;
            if (!_configDic.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist role item id: {0}", id);
            }
            return ret;
        }

        
    }
}