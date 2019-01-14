using System.Collections.Generic;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration.Terrains
{
    public class TerrainSoundConfigManager : AbstractConfigManager<TerrainSoundConfigManager>
    {
        private Dictionary<int, TerrainSoundConfigItem> _dictSounds = new Dictionary<int, TerrainSoundConfigItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("terrain sound config xml is empty !");
                return;
            }
            _dictSounds.Clear();
            var cfg = XmlConfigParser<TerrainSoundConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("terrain sound config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _dictSounds[item.Id] = item;
            }
        }

        public TerrainSoundConfigItem GetSoundById(int id)
        {
            if (!_dictSounds.ContainsKey(id))
            {
                return null;
            }
            return _dictSounds[id];
        }
    }
}
