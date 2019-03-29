using System.Collections.Generic;
using Shared.Scripts.Terrains;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration.Terrains
{
    public class TerrainTextureConfigManager : AbstractConfigManager<TerrainTextureConfigManager>
    {
        private Dictionary<int, TerrainTextureConfigItem> _dictTextures = new Dictionary<int, TerrainTextureConfigItem>();
        private Dictionary<string, TerrainTextureConfigItem> _dictNameTextures = new Dictionary<string, TerrainTextureConfigItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("terrain texture config xml is empty !");
                return;
            }
            _dictTextures.Clear();
            _dictNameTextures.Clear();
            var cfg = XmlConfigParser<TerrainTextureConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("terrain texture config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _dictTextures[item.Id] = item;
                _dictNameTextures[item.Name] = item;
            }
        }

        public TerrainTextureConfigItem GetTextureById(int id)
        {
            TerrainTextureConfigItem configItem;

            if (_dictTextures.TryGetValue(id, out configItem))
            {
                return configItem;
            }
            return null;
        }

        public TerrainTextureConfigItem GetTextureByName(string name)
        {
            if (!_dictNameTextures.ContainsKey(name))
            {
                return null;
            }
            return _dictNameTextures[name];
        }
    }
}
