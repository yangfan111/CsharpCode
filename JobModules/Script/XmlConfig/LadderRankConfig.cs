using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlType("item")]
    public class LadderRankConfigItem
    {
        public int Id;
        public int Rank;
        public int CoefficientK;
        public int MaxAi;
    }

    [XmlRoot("root")]
    public class LadderRankConfig
    {
        public LadderRankConfigItem[] Items;
    }
}
