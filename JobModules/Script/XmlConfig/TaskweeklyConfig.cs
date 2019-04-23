using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class TaskweeklyConfig
    {
        public TaskweeklyItem[] Items;
    }

    [XmlType("item")]
    public class TaskweeklyItem
    {
        public int Id;
        public int Season;
        public int Week;
        public int Model;
        public int TaskType; //普通任务：1，英雄任务：2，周任务：3
        public int[] Rule;
        public int Group;
        public int Difficulty;
        public string Description;
        public int[] RewardTypeList;
        public int[] RewardIdList;
        public int[] RewardCntList;

        public int[] Goal;   //完成值
    }
}
