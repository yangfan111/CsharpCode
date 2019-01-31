using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;

namespace Core.Configuration.Sound
{
    public interface ISoundConfigManager
    {
        SoundConfigItem GetSoundById(int id);
    }

    public class SoundConfigManager : AbstractConfigManager<SoundConfigManager>, ISoundConfigManager
    {
        public int Hit {
            get
            {
                return 1;
            }
        }
        public int Pickup
        {
            get
            {
                return 6;
            }
        } 
        private Dictionary<int, SoundConfigItem> _weaponSoundDic = new Dictionary<int, SoundConfigItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("sound config xml is empty !");
                return;
            }
            _weaponSoundDic.Clear();
            var cfg = XmlConfigParser<SoundConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("sound config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _weaponSoundDic[item.Id] = item;
            }
        }

        public SoundConfigItem GetSoundById(int id)
        {
            if (!_weaponSoundDic.ContainsKey(id))
            {
                Logger.WarnFormat("Id {0} does not exist in config ", id);
                return null;
            }

            return _weaponSoundDic[id];
        }
    }
}
