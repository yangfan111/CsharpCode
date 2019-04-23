using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class GlobalDataConfig
    {
        public GlobalDataItem[] Items;
    }

    [XmlType("child")]
    public class GlobalDataItem
    {
        public int Id;
        public string Value;
    }
}
