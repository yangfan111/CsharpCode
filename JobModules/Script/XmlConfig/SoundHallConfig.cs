using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class SoundHallConfig
    {
        public SoundHall[] Items;
    }

    [XmlType("child")]
    public class SoundHall
    {
        public int Id;
        public string Bundle;
        public string Res;
        public int Type;
        public float Volume;
    }
}
