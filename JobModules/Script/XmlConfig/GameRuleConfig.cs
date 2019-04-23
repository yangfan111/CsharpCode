using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class GameRuleConfig
    {
        public GameRule[] Items;
    }

    [XmlType("item")]
    public class GameRule
    {
        public int Id;
        public int OverTime;
        public int RuleType;
        public int IsGrenade;
        public int IsObserve;
        public int IsThirdPerson;
        public int IsRescue;
        public string IconBundle;
        public string Icon;
        public string Comment;
        public List<int> PlayerNum;
        public int DefaultPlayerNum;
        public List<int> ReliveTimeNum;
        public int DefaultReliveTimeNum;
        public List<int> WaitTimeNum;
        public int DefaultWaitTimeNum;
        public List<int> TeamNum;
        public List<int> ConditionType;
        public int DefaultConditionType;
        public List<string> ConditionDescription;
        public List<int> ConditionCnt1;
        public int DefaultConditionCnt1;
        public List<int> ConditionCnt2;
        public int DefaultConditionCnt2;
        public List<string> ViewShow;
    }
}
