using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlType("child")]
    public class MeleeAttackCDItem
    {
        public int Id;
        public float AttackOneCD;
        public float AttackTwoCD;
    }

    [XmlRoot("root")]
    public class MeleeAttackCDConfig
    {
        public MeleeAttackCDItem[] Items;
    }
}
