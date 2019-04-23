using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class VideoSettingConfig
    {
        public VideoSetting[] Items;
    }

    [XmlType("item")]
    public class VideoSetting
    {
        public int Id;
        public int Type;
        public string Description;
        public float DefaultValue;
        public int ControlType;
        public string[] LevelNames;
        public List<float> LevelDatas;
        public float MinValue;
        public float MaxValue;
    }
}
