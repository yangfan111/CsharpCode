using System.Collections.Generic;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration.Terrains
{
    public class TerrainEffectConfigManager : AbstractConfigManager<TerrainEffectConfigManager>
    {
        private Dictionary<int, TerrainEffectConfigItem> _dictEffects = new Dictionary<int, TerrainEffectConfigItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("terrain effect config xml is empty !");
                return;
            }
            _dictEffects.Clear();
            var cfg = XmlConfigParser<TerrainEffectConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("terrain effect config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _dictEffects[item.Id] = item;
            }
        }

        public TerrainEffectConfigItem GetEffectById(int id)
        {
            if (!_dictEffects.ContainsKey(id))
            {
                return null;
            }
            return _dictEffects[id];
        }
    }
}
