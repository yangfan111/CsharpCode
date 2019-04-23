using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class ServerTipCodeConfig
    {
        public ServerTipCodeConfigItem[] Items;
    }

    [XmlType("child")]
    public class ServerTipCodeConfigItem
    {
        public int Id;
        public string Name;
    }
}
