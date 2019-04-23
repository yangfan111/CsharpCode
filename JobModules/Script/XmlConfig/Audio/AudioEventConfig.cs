using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class AudioEventConfig
    {
        public AudioEventItem[] Items { get; private set; }
    }
    [XmlType("item")]
    public class AudioEventItem
    {
        public int Id;
        public string Event;
        public List<int> SwitchGroup;
        public string BankRef;

    }
}