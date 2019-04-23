using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class RoleExpConfig
    {
        public RoleExp[] Items;
    }

    [XmlType("child")]
    public class RoleExp
    {
        public int Id;
        public int Exp;
    }
}
