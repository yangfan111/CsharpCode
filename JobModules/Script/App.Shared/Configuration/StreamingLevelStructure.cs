using Shared.Scripts.SceneManagement;
using Utils.Configuration;
using XmlConfig;

namespace App.Shared.Configuration
{
    public class StreamingLevelStructure : AbstractConfigManager<StreamingLevelStructure>
    {
        public StreamingData Data { get; private set; }
        
        public override void ParseConfig(string xml)
        {
            Data = XmlConfigParser<StreamingData>.Load(xml);
        }
    }
}