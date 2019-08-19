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
        public int TimeType;
        public string Bundle;
        public string Res;
        public List<float> ShowRotation;  //展示角度
        public float ShowDistance;        //展示距离

    }

    [XmlRoot("root")]
    public class PropConfig
    {
        public PropConfigItem[] Items;
    }   
}
