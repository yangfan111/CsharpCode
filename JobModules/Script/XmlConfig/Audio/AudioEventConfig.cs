using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class AudioEventConfig
    {
        public AudioEventItem[] Items;
    }
    [XmlType("item")]
    public class AudioEventItem
    {
        public int Id;
        public string Event;
        public uint ConvertedId;
        public List<int> SwitchGroup;
        public string BankRef;

    }
}