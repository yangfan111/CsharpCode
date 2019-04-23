using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeaponConfigNs;

namespace XmlConfig
{

    public class WeaponCommonConfig
    {
        public WeaponCommonConfigItem[] Items;
    }

    public class WeaponCommonConfigItem
    {
        public int Id;
        public string Name;
        public AssetInfo Asset;
    }
}
