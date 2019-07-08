using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class HallSystemEnableConfig
    {
        public HallSystemEnable[] Items;
    }

    [XmlType("Item")]
    public class HallSystemEnable
    {
        public int Id;
        public string SystemName;
        public int Type;
    }
}
