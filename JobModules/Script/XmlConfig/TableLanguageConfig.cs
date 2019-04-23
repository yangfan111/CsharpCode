using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class TableLanguageConfig
    {
        public TableLanguage[] Items;
    }

    [XmlType("child")]
    public class TableLanguage
    {
        public int Id;
        public string Name;
    }
}
