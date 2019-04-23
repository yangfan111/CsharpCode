using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class LoadingTipConfig
    {
        public LoadingTip[] Items;
    }

    [XmlType("Item")]
    public class LoadingTip
    {
        public int Id;
        public int Type;
        public string Tip;
        public List<int> GameMode;
    }
}
