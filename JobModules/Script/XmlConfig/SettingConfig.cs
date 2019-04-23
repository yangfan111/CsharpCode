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
        public List<int> ComboxItemKeys;
        public List<string> Additional;
        public List<string> SettingId;
    }
}
