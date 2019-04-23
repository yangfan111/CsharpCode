using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class RoleAvatarResConfig
    {
        public RoleAvatorRes[] Items;
    }

    [XmlType("child")]
    public class RoleAvatorRes
    {
        public int Id;
        public string Name;
        public int NameId;
        public int IsSkinned;
        public int AvatarType;
        public int Default;
        public int HaveSecond;
        public string HaveHideAvatar;
        public int HaveP1Avatar;
        public int Sex;
        public string BundleName;
        public string AssetName;
        public string BundleNameInside;
        public string SecondRes;
        public string SecondType;
        public List<int> HideAvatars;
    }
}
