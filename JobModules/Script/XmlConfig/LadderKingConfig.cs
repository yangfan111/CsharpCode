using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class LadderKingConfig
    {
        public LadderKing[] Items;
    }

    [XmlType("item")]
    public class LadderKing
    {
        public int Id;
        public int Model;
        public string Name;
        public int Star;
        public int End;
        public string IconBundle;
        public string Icon;
        public int[] RewardTypeList;
        public int[] RewardIdList;
        public int[] RewardCntList;
    }
}
