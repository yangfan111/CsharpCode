using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.AssetManager;

namespace Core.Configuration
{
    public class ConfigSet
    {
        public static AssetInfo GetP3ResourceFromId(int id)
        {
            return new AssetInfo("weapon/ar", "WPN_AKM_P3");
        }

        public static AssetInfo GetP1ResourceFromId(int id)
        {
            return new AssetInfo("weapon/ar", "WPN_AKM_P1");
        }
    }
}
