using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class IndividuationConfig
    {
        public Individuation[] Items;
    }

    [XmlType("child")]
    public class Individuation : ItemBaseConfig
    {
        public List<int> ApplyRoleList;
        public int LifeTime;
        public int IntervalCD;
        public int PoseId;
    }
}
