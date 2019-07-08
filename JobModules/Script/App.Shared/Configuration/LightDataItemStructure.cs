using Shared.Scripts.SceneManagement;
using Utils.Configuration;
using XmlConfig;
using ArtPlugins;
using Shared.Scripts.SceneManagement;
namespace App.Shared.Configuration
{
    public class LightDataItemStructure : AbstractConfigManager<LightDataItemStructure>
    {
        public MeshRecordsWrapper Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            Data = XmlConfigParser<MeshRecordsWrapper>.Load(xml);
           
        }
    }
}