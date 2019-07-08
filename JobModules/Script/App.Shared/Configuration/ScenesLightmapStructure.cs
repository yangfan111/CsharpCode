using Shared.Scripts.SceneManagement;
using Utils.Configuration;
using XmlConfig;
using ArtPlugins;
using Shared.Scripts.SceneManagement;
using System.Collections.Generic;
namespace App.Shared.Configuration
{
    public class ScenesLightmapStructure : AbstractConfigManager<ScenesLightmapStructure>
    {
        public MeshRecords Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            if(Data == null)
            {
                Data = new MeshRecords();
            }
            MeshRecordsWrapper warpper = XmlConfigParser<MeshRecordsWrapper>.Load(xml);
            Data.recordsList.AddRange(warpper.meshRecords.recordsList);
           
        }

        public void ListToDict()
        {
            Data.ListToDict();
            Data.recordsList.Clear();
        }

        public void Clear()
        {
            if(Data != null && Data.recordsList != null)
            {
                Data.recordsList.Clear();
            }
        }
    }
}