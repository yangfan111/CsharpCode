using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class PlayerStatisticConfig
    {
        public PlayerStatisticData[] Items;
    }

    [XmlType("item")]
    public class PlayerStatisticData
    {
        public int Id;
        public string Name;
        public int NameId;
        public string SubIds;
        public int caculateType;
        public float WidthScale;
    }
}
