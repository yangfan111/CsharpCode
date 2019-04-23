using System.Xml.Serialization;
using WeaponConfigNs;
using System.Collections.Generic;

namespace XmlConfig
{
    [XmlType("child")]
    public class KillFeedBack
    {
        public int Id;      
        public string Icon;
        public string Effect;
    }

    [XmlRoot("root")]
    public class KillFeedBackConfig
    {
        public KillFeedBack[] Items;
    }
}
