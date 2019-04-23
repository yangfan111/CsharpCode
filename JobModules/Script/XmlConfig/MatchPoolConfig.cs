using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class MatchPoolConfig
    {
        public MatchPool[] Items;
    }

    [XmlType("item")]
    public class MatchPool
    {
        public int Id;
        public int MatchGroupId;
        public string ActualMatchPool;
        public int ModeId;
        public int Type;
        public int Default;
        public string IconBundle;
        public string Icon;
        public string Icon_s;
    }
}
