using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class MoneyConfig
    {
        public Money[] Items;
    }

    [XmlType("child")]
    public class Money : ItemBaseConfig
    {

    }
}
