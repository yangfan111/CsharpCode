using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class LadderDanConfig
    {
        public LadderDan[] Items;
    }

    [XmlType("item")]
    public class LadderDan
    {
        public int Id;
        public int Model;
        public string Name;
        public int Rule;
        public int Integral;
        public string IconBundle;
        public string Icon;
        public int DanType;
        public List<int> Map;
        public int[] SeasonRewardTypeList;
        public int[] SeasonRewardIdList;
        public int[] SeasonRewardCntList;
        public int[] AdvancedRewardTypeList;
        public int[] AdvancedRewardIdList;
        public int[] AdvancedRewardCntList;
    }
}
