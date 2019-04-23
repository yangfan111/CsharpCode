using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class PlayerStatisticShowConfig
    {
        public PlayerStatisticShowData[] Items;
    }

    [XmlType("item")]
    public class PlayerStatisticShowData
    {
        public int Id;
        public string Name;
        public int NameId;
        public int Valid;
        public int ParentId;
        public string Mode1;
        public List<string> Mode2;
        public List<string> Mode3;
    }
}
