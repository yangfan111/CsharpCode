using Utils.Configuration;
using XmlConfig;

namespace App.Shared.Configuration
{
    public class MapsDescription : AbstractConfigManager<MapsDescription>
    {
        public MapConfig Data { get; private set; }

        public override void ParseConfig(string xml)
        {
            Data = XmlConfigParser<MapConfig>.Load(xml);
        }
    }
}