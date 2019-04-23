using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class GameModeConfig
    {
        public GameModeConfigItem[] Items;
    }

    [XmlType("item")]
    public class GameModeConfigItem
    {
        public int Id;
        public string Name;
        public int NameId;
        public int ParentId;
        public int SortIndex;
        public List<int> RuleId;
        public string RoomName;
        public int RoomNameId;
        public string Description;
        public int NewMode;
        public int Default;
        public List<int> Map;
        public string IconBundle;
        public string Icon;
        public int Valid;
        public int Type; // GameNewModeConst,Custom = 0,//自定义 Abyss = 2,//深渊模式Adventure = 3,//冒险模式Ladder = 4,//天梯模式

        public int StructureType;  //1队伍 2 阵营 3 无阵营
        public List<int> Content;
        public int SortId;
        public int OrderBy;  //0升序 1 降序
        public int BagType;
        public List<int> Channel;
        public int DefaultChannel;
        public int WeaponStayTime;
        public int ChangeBag;
    }

    public enum EBagType
    {
        None,
        Chicken,
        Group,
        Bounty,
    }
}
