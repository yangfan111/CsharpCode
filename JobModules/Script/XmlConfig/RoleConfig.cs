using System.Xml.Serialization;
using System.Collections.Generic;

namespace XmlConfig
{
    [XmlType("child")]
    public class RoleItem : ItemBaseConfig
    {
        public int Subtype;
        public int Sex;

        public bool HasFirstPerson;
        public bool Unique;
        public string ThirdModelAssetBundle;
        public string ThirdModelAssetName;
        public string FirstModelAssetBundle;
        public string FirstModelAssetName;
        
        public List<int> Res;
        public int Camp;      //1T 2CT
        public int CharacterType;
    }

    [XmlRoot("root")]
    public class RoleConfig
    {
        public RoleItem[] Items;

    }
}
