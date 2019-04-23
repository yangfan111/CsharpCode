using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class TypeForDeathConfig
    {
        public List<TypeForDeath> Items;
    }

    [XmlType("child")]
    public class TypeForDeath
    {
        public int Id;
        public string CardName;
        public string KillIcon;
        public string BundleName;
        public string Name;
    }
}

