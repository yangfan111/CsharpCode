using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlType("child")]
    public class PropConfigItem : ItemBaseConfig
    {
        public float Weight;
        public int shopID;
        public string Numtype;
        public int Script;
        public List<int> TidList;
        public List<int> PacksTypeList;
        public int EffectVal;   //作用值
    }

    [XmlRoot("root")]
    public class PropConfig
    {
        public PropConfigItem[] Items;
    }   
}
