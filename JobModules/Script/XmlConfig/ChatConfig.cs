using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class ChatConfig
    {
        public List<Channel> Items;
    }

    [XmlType("item")]
    public class Channel
    {
        public int Id;
        public string ChannelName;
    }
}
