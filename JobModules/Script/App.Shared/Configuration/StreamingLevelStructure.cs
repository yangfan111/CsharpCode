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

            var count = Data.Scenes.Count;
            for (int i = 0; i < count; i++)
            {
                var scene = Data.Scenes[i];
                scene.Index = i;

                var goCount = scene.Objects.Count;
                for (int j = 0; j < goCount; j++)
                {
                    scene.Objects[j].ConvertFromSerialization(scene);
                    scene.Objects[j].Index = j;
                }
            }
        }
    }
}