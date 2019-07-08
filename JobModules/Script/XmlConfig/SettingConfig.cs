using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class SettingConfig
    {
        public SettingConfigItem[] Items;
    }

    [XmlType("item")]
    public class SettingConfigItem
    {
        public int Id;
        public int Type;
        public string TypeName;
        public int ControlType;
        public string Desription;
        public string DefaultValue;
        public List<string> ComboxItemNames;
    }

    [XmlRoot("root")]
    public class SettingConfigVideo
    {
        public SettingConfigVideoItem[] Items;
    }

    [XmlType("item")]
    public class SettingConfigVideoItem
    {
        public int Id;
        public int Type;
        public string TypeName;
        public int ControlType;
        public string Desription;
        public string DefaultValue;
        public List<string> ComboxItemNames;
        public List<int> ValuePerLevel;
    }

}
