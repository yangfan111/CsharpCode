using System.Collections.Generic;
using Shared.Scripts.Terrains;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration.Terrains
{
    public class TerrainTextureTypeConfigManager : AbstractConfigManager<TerrainTextureTypeConfigManager>
    {
        private Dictionary<int, TerrainTextureTypeConfigItem> _dictTextures = new Dictionary<int, TerrainTextureTypeConfigItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("terrain texture type config xml is empty !");
                return;
            }
            _dictTextures.Clear();
            var cfg = XmlConfigParser<TerrainTextureTypeConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("terrain texture type config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _dictTextures[item.Type] = item;
            }
        }

        public TerrainTextureTypeConfigItem GetTextureTypeById(int id)
        {
            if (!_dictTextures.ContainsKey(id))
            {
                return null;
            }
            return _dictTextures[id];
        }
    }
}
