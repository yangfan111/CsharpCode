using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class InputBlockConfig
    {
        public InputBlockItem[] Items;
    }

    [XmlType("Item")]
    public class InputBlockItem
    {
        public PostureInConfig PostureType;
        public InputBlockType BlockType;
    }

    public enum InputBlockType
    {
        BlockMovement,
        End
    }
}