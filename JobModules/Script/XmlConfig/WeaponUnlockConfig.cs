using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class WeaponUnlockConfig
    {
        public WeaponUnlock[] Items;
    }

    [XmlType("Weapon")]
    public class WeaponUnlock
    {
        public int Id;
        public int Category;
        public int Tid;
        public string Name;
        public int NameId;
        public int UnlockLv;
        public int GP;
        public int Gold;

    }
}
