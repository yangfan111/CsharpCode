using Utils.Configuration;
using ArtPlugins;
using XmlConfig;

namespace App.Shared.Configuration
{
    public class ScenesIndoorCullStructure : AbstractConfigManager<ScenesIndoorCullStructure>
    {
        public ScenesIndoorCullData.IndoorCullRecords Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            Data = XmlConfigParser<ScenesIndoorCullData.IndoorCullRecords>.Load(xml);

            Data.ListToDict();
            Data.recordsList.Clear();
        }
    }
}