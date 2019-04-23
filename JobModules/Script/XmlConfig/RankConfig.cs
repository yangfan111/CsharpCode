using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class RankConfig
    {
        public Rank[] Items;
    }
    [XmlType("child")]
    public class Rank
    {
        public int Id;
        public int Type;
        public int SubType;
        public string TypeName;
        public int TypeNameId;
        public string Name;
        public int NameId;
        public int Limit;
        public List<int> TitleName;
        public int Ladder;
    }
}
