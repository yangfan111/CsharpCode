using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class TaskdailyConfig
    {
        public TaskdailyItem[] Items;
    }

    [XmlType("item")]
    public class TaskdailyItem
    {
        public int Id;
        public string Description;
        public int[] Rule;
        public int Difficulty;
        public int[] RewardTypeList;
        public int[] RewardIdList;
        public int[] RewardCntList;

        public int[] Goal; //完成值
    }
}
