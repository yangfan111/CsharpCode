using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class LadderInfoConfig
    {
        public LadderInfo[] Items;
    }

    [XmlType("item")]
    public class LadderInfo
    {
        public int Id;
        public int Model;
        public string InfoName;
        public int InfoNameId;
        public int SubIds;
        public int Kingshow;
    }
}
