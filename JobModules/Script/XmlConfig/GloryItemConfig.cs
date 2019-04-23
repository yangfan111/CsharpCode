using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class GloryItemConfig
    {
        public GloryItem[] Items;
    }

    [XmlType("child")]
    public class GloryItem
    {
        public int Id;
        public string Name;
        public int NameId;
        public string Icon;
    }
}
