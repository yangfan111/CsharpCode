using System.Collections.Generic;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration.Terrains
{
    public class TerrainMaterialConfigManager : AbstractConfigManager<TerrainMaterialConfigManager>
    {
        private Dictionary<int, TerrainMaterialConfigItem> _dictMaterials = new Dictionary<int, TerrainMaterialConfigItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("terrain material config xml is empty !");
                return;
            }
            _dictMaterials.Clear();
            var cfg = XmlConfigParser<TerrainMaterialConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("terrain material config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _dictMaterials[item.Id] = item;
            }
        }

        public TerrainMaterialConfigItem GetMaterialById(int id)
        {
            if (!_dictMaterials.ContainsKey(id))
            {
                return null;
            }
            return _dictMaterials[id];
        }
    }
}
