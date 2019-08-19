using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class WeaponWorkshopConfig
    {
        public WeaponWorkshop[] Items;
    }

    [XmlType("child")]
    public class WeaponWorkshop
    {
        public int Id;
        public string CodeName;
        public string Name;
        public string Description;
        public string IconBundle;
        public string Icon;
        public string BGIcon;
    }
}

