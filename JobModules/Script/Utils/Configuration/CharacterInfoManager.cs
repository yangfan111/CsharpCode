using System.Collections.Generic;
using Core.Utils;
using XmlConfig;

namespace Utils.Configuration
{
    public class CharacterInfoManager: AbstractConfigManager<CharacterInfoManager>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterInfoManager));
        private Dictionary<int, CharacterInfoItem> _characterInfoConfigs = new Dictionary<int, CharacterInfoItem>();
        
        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("characterInfo config xml is empty !");
                return;
            }
            _characterInfoConfigs.Clear();
            var cfg = XmlConfigParser<CharacterInfoConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("characterInfo config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                _characterInfoConfigs.Add(item.Type, item);
            }
        }
        
        public CharacterInfoItem GetCharacterInfoByType(int type)
        {
            CharacterInfoItem ret;
            if (!_characterInfoConfigs.TryGetValue(type, out ret))
            {
                Logger.WarnFormat("Not exist characterInfo item type: {0}", type);
            }
            
            return ret;
        }
        
        public CharacterInfoItem GetDefaultInfo()
        {            
            return GetCharacterInfoByType(0);
        }
    }
}