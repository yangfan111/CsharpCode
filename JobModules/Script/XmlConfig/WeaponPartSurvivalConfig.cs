using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlType("child")]
    public class WeaponPartSurvivalConfigItem
    {
        public int Id;
        public string Name;
        public int[] PartsList;
        public float Size;
    }


    [XmlRoot("root")]
    public class WeaponPartSurvivalConfig
    {
        public WeaponPartSurvivalConfigItem[] Items;
    }
}
