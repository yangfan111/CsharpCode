using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class LadderTimeConfig
    {
        public LadderTime[] Items;
    }

    [XmlType("item")]
    public class LadderTime
    {
        public int Id;
        public string Name;
        public string Start;
        public string End;

        public int MedalPrice;   //英雄勋章开通价格
        public int LevelPrice;   //每级勋章等级
        public int OriginalPrice;//勋章开通原价
        public string Description;

        public int HMedalPrice; //高级开通价格（资格加等级包）
        public int HLevel;      //附赠等级
        public int HOriginalPrice; //高级原价
        public string HDescription; //高级礼包描述
        public string GiftName; //礼包名字
    }
}
