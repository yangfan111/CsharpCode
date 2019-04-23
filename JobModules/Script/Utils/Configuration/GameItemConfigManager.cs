using Utils.AssetManager;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace Utils.Configuration
{
    public class GameItemConfigManager : AbstractConfigManager<GameItemConfigManager>
    {

        private Dictionary<int, GameItemConfigItem> _configs = new Dictionary<int, GameItemConfigItem>();
        private GameItemConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<GameItemConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach(var item in _config.Items)
            {
                _configs[item.Id] = item;
            }
        }

        public GameItemConfigItem GetConfigById(int id)
        {
            if(!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null; 
            }
            return _configs[id];
        }

        public AssetInfo GetAssetById(int id)
        {
            var cfg = GetConfigById(id);
            if(null == cfg)
            {
                return new AssetInfo();
            }
            return new AssetInfo
            {
                AssetName = cfg.Prefab,
                BundleName = cfg.Bundle,
            };
        }

        public float GetSizeById(int id)
        {
            var cfg = GetConfigById(id);
            if(null == cfg)
            {
                return 1;
            }
            return cfg.Size;
        }

        public GameItemConfig GetConfig()
        {
            return _config;
        }


        public string GetTypeStrById(int id)
        {
            string result = "";
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                switch ((GameItemType)cfg.Type)
                {
                    case GameItemType.Bullet:   //子弹
                        {
                            result = "子弹";
                        }
                        break;
                    case GameItemType.Supplies:   //补给品
                        {
                            result = "补给品";
                        }
                        break;
                }
            }

            return result;
        }
    }
}
