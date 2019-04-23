using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class ItemtimelimitConfig
    {
        public Itemtimelimit[] Items;
    }

    [XmlType("item")]
    public class Itemtimelimit
    {
        public int Id;
        public string Name;
        public int Category;    //虚拟大类16
        public int TCategory;    //目标大类
        public int Tid;         //具体物品id
        public int BuyType;     //1，时效 2，数量
        public int Num;         //0为永久
        public int PriceType;   //1充值币（金币）2游戏币（GP） 3礼券
        public int Price;
    }
}
