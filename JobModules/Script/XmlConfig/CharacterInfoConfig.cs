using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class CharacterInfoConfig
    {
        public CharacterInfoItem[] Items;
    }

    [XmlType("child")]
    public class CharacterInfoItem
    {
        public int Id;
        public int Type;
        public float JumpSpeed;
        public float BigJumpHeight;
        public float StandHeight;
        public float StandRadius;
        public float CrouchHeight;
        public float CrouchRadius;
        public float ProneHeight;
        public float ProneRadius;
        public float StepOffset;
        public float SlopeLimit;
    }
}