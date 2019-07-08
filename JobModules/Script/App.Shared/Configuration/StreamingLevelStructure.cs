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
            StreamingScene scene = XmlConfigParser<StreamingScene>.Load(xml);

            if(Data == null)
            {
                Data = new StreamingData();
            }

            Data.AddScene(scene);
           
        }

        public void Clear()
        {
            if(Data != null && Data.Scenes != null)
            {
                Data.Scenes.Clear();
            }
        }
    }
}