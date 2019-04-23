using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class MedalConfig
    {
        public Medal[] Items;
    }

    [XmlType("item")]
    public class Medal
    {
        public int Id;
        public int level;

        public int[] RewardTypeList;
        public int[] RewardIdList;
        public int[] RewardCntList;

        //英雄勋章奖励
        public int[] HRewardTypeList;
        public int[] HRewardIdList;
        public int[] HRewardCntList;

        public int BigGrid;  //0为普通格 1为大格子
    }
}
