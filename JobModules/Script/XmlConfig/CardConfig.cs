using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class CardConfig
    {
        public Card[] Items;
    }

    [XmlType("child")]
    public class Card : ItemBaseConfig
    {

    }
}
