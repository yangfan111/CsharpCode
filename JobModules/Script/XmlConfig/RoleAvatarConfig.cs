using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{

    [XmlRoot("root")]
    public class RoleAvatarConfig
    {
        public RoleAvatarConfigItem[] Items;
    }

    [XmlType("child")]
    public class RoleAvatarConfigItem : ItemBaseConfig
    {
        public int FRes;
        public int Res;
        public string Bundle;
        public string FPrefab;
        public string MPrefab;
        public float Weight;
        public float Capacity;
        public int PickSound;
        public List<int> ApplyRoleList;
        public List<int> ReplaceList;
        public int Sort;
    }
}
