using Shared.Scripts.SceneManagement;
using Utils.Configuration;
using XmlConfig;
using ArtPlugins;

namespace App.Shared.Configuration
{
    public class ScenesLightmapStructure : AbstractConfigManager<ScenesLightmapStructure>
    {
        public ScenesLightmapData.MeshRecords Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            Data = XmlConfigParser<ScenesLightmapData.MeshRecords>.Load(xml);

            Data.ListToDict();
            Data.recordsList.Clear();
        }
    }
}