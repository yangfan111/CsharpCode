using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.SettingManager;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Configuration
{

    public class VideoSettingConfigManager : AbstractConfigManager<VideoSettingConfigManager>
    {
        private Dictionary<int, VideoSetting> _configs = new Dictionary<int, VideoSetting>();
        private VideoSettingConfig _config;
        Dictionary<EVideoSettingType, HashSet<VideoSetting>> typeDict = new Dictionary<EVideoSettingType, HashSet<VideoSetting>>();

        public Dictionary<int, VideoSetting> Configs
        {
            get
            {
                return _configs;
            }

            set
            {
                _configs = value;
            }
        }

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<VideoSettingConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach (var item in _config.Items)
            {
                _configs[item.Id] = item;
                var type = (EVideoSettingType)item.Type;
                if (!typeDict.ContainsKey(type))
                {
                    typeDict.Add(type, new HashSet<VideoSetting>());
                }
                typeDict[type].Add(item);
            }
        }

        public VideoSetting GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null;
            }
            return _configs[id];
        }

        public VideoSettingConfig GetConfig()
        {
            return _config;
        }

        public Dictionary<int, float> GetDefaultValueDict()
        {
            Dictionary<int, float> dict = new Dictionary<int, float>();
            foreach (var item in _configs.Values)
            {
                dict.Add(item.Id, item.DefaultValue);
            }
            return dict;
        }

        public Dictionary<EVideoSettingType, HashSet<VideoSetting>> GetTypeDict()
        {
            return typeDict;
        }

        public static VideoSettingConfigManager GetInstance()
        {
            return SingletonManager.Get<VideoSettingConfigManager>();

        }

        public VideoSetting GetItemById(int id)
        {
            VideoSetting res;
            _configs.TryGetValue(id, out res);
            return res;
        }
    }
}
