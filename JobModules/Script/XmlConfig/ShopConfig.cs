using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class ShopConfig
    {
        public ShopItem[] Items;
    }

    [XmlType("item")]
    public class ShopItem
    {
        public int Id;
        public string Name;
        public int NameId;
        public List<int> LimtIdLst; //时效ID列表
        public List<int> Recommend; //推荐页签(0,不推荐；1，推荐；2，热卖；3，PVP推荐；4，PVE推荐）
        public List<int> TabShow;   //是否大图展示 列表(0,不推荐；1，推荐；2，热卖；3，PVP推荐；4，PVE推荐）
        public int MaxBuyAmount;
        public int Sort;
        public string IconBundle;  //展示图
        public string Icon;        //展示图
        public int RecommendSort;  //展示排序
    }
}
