using Shared.Scripts.SceneManagement;
using Utils.Configuration;
using XmlConfig;
using Shared.Scripts.Util;

namespace App.Shared.Configuration
{
    public class StreamingLevelStructure : AbstractConfigManager<StreamingLevelStructure>
    {
        public StreamingData Data { get; private set; }
        
        public override void ParseConfig(string xml)
        {
            StreamingScene scene = StreamingSerialization.DeserializeByString<StreamingScene>(xml);
            //StreamingScene scene = XmlConfigParser<StreamingScene>.Load(xml);

            foreach (var obj in scene.Objects)
            {
                foreach (var compData in obj.CompDataList)
                {
                    compData.monoClassFullName.GetTypeByStr();
                }
            }


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