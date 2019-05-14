using Core.Utils;
using System.Collections.Generic;
using Utils.Configuration;
using XmlConfig;

namespace Assets.Utils.Configuration
{
    /// <summary>
    /// Defines the <see cref="WeaponPropertyConfigManager" />
    /// </summary>
    public class WeaponPropertyConfigManager : AbstractConfigManager<WeaponPropertyConfigManager>, IConfigParser
    {
        private Dictionary<int, WeaponPropertyConfigItem> _weaponPropertyDic = new Dictionary<int, WeaponPropertyConfigItem>();     //武器
        private Dictionary<int, WeaponPropertyConfigItem> _weaponPartPropertyDic = new Dictionary<int, WeaponPropertyConfigItem>(); //配件

        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<WeaponPropertyConfig>.Load(xml);
            foreach (var item in cfg.Items)
            {
                if(item.Category == 2) //武器
                {
                    _weaponPropertyDic[item.WeaponId] = item;
                }

                if(item.Category == 5)
                {
                    _weaponPartPropertyDic[item.WeaponId] = item;
                }
            }
        }

        public Dictionary<int, WeaponPropertyConfigItem> GetConfigs()
        {
            return _weaponPropertyDic;
        }

        public WeaponPropertyConfigItem FindByWeaponId(int id)
        {
            WeaponPropertyConfigItem item = null;
            _weaponPropertyDic.TryGetValue(id, out item);
            AssertUtility.Assert(item != null);
            return item;
        }
    }
}
