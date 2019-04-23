using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class TableLanguageMapConfig
    {
        public TableLanguageMap[] Items;
    }

    [XmlType("child")]
    public class TableLanguageMap
    {
        public int Id;
        public string Table;
        public List<string> Name;
        public List<string> TranslateName;
    }
}
