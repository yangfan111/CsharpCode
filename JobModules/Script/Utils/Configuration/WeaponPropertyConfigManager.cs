using Core.Utils;
using System.Collections.Generic;
using Utils.Configuration;
using WeaponConfigNs;
using XmlConfig;

namespace Assets.Utils.Configuration
{
    /// <summary>
    /// Defines the <see cref="WeaponPropertyConfigManager" />
    /// </summary>
    public class WeaponPropertyConfigManager : AbstractConfigManager<WeaponPropertyConfigManager>, IConfigParser
    {
        private Dictionary<int, WeaponPropertyConfigItem> _weaponPropertyDic = new Dictionary<int, WeaponPropertyConfigItem>();

        private readonly int[] _emptyIntArray = new int[0];

        public override void ParseConfig(string xml)
        {
            var cfg = XmlConfigParser<WeaponPropertyConfig>.Load(xml);
            foreach (var item in cfg.Items)
            {
                _weaponPropertyDic[item.WeaponId] = item;
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
