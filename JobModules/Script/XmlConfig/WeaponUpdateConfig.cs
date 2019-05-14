using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class WeaponUpdateConfig
    {
        public WeaponUpdate[] Items;
    }

    [XmlType("child")]
    public class WeaponUpdate
    {
        public int Id;
        public int Tid;
        public int MaxLv;
        public int IsUnlock;                     //0可解锁 1 不可解锁
        public List<int> UpdatePropsList;        //升级可用道具
        public List<int> DismantlingPropsList;   //分解可得道具
        public List<int> SlotTypeList;           //可解锁部位
    }
}

