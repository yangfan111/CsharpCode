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
        public int Type; // GameNewModeConst,Custom = 0,//自定义 Abyss = 2,//深渊模式Adventure = 3,//冒险模式Ladder = 4,//天梯模式
        public int ChickenType;
        public int SortIndex;
        public int Valid;
        public int Default;
        public List<int> RuleId;
        public string RoomName;
        public int RoomNameId;
        public string Description;
        public int DescriptionID;
        public int NewMode;
        public List<int> Map;
        public string IconBundle;
        public string Icon;
        public List<int> Channel;
        public int DefaultChannel;
        public int BagType;
        public int ChangeBag;
        public int WeaponStayTime;
        public int StructureType;  //1队伍 2 阵营 3 无阵营
        public List<int> Content;
        public int SortId;
        public int OrderBy;  //0升序 1 降序
        public int Mode;
        public int Clothes;
        public float ExpCoefficient;
        public int ExpMode;
        public string TeamADescription;
        public string TeamBDescription;
    }

    public enum EBagType
    {
        None,
        Chicken,
        Group,
        Bounty,
    }
}
