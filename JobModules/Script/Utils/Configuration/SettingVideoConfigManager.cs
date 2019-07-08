using Core.Utils;
using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public class SettingVideoConfigManager : AbstractConfigManager<SettingVideoConfigManager>
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SettingVideoConfigManager));
        private Dictionary<int, SettingConfigVideoItem> _configDic = new Dictionary<int, SettingConfigVideoItem>();
        public Dictionary<int, SettingConfigVideoItem> ConfigDic { get { return _configDic; } set { _configDic = value; } }
        public SettingVideoConfigManager()
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
            var cfg = XmlConfigParser<SettingConfigVideo>.Load(xml);
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

        public SettingConfigVideoItem GetItemById(int id)
        {
            SettingConfigVideoItem ret;
            if (!_configDic.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist role item id: {0}", id);
            }
            return ret;
        }
    }
}
