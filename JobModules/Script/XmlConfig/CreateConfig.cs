using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class CreateConfig
    {
        public Create[] Items;
    }

    [XmlType("item")]
    public class Create
    {
        public int Id;
        public string Sex;
        public int RoleId;
        public int GP;
        public List<int> ChooseHairList;
        public List<int> ChooseInnerList;
        public List<int> ChooseTrouserList;
        public List<int> ChooseFootList;
    }
}
