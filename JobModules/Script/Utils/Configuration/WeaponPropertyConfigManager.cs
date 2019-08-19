using Assets.XmlConfig;
using Core.Utils;
using System;
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
            if (item == null)
            {
                Logger.Error("Not found id" + id);
            }
            return item;
        }

        public WeaponProperty GetWeaponProperty(ECategory category, int templateId)
        {
            WeaponPropertyConfigItem cfg = null;
            if (category == ECategory.Weapon)
            {
                cfg = GetWeaponPropertyCfg(templateId);
            }
            else if (category == ECategory.WeaponPart)
            {
                cfg = GetWeaponPartPropertyCfg(templateId);
            }
            if (cfg != null)
            {
                return cfg.GetWeaponProperty();
            }
            return default(WeaponProperty);
        }

        private WeaponPropertyConfigItem GetWeaponPartPropertyCfg(int templateId)
        {
            WeaponPropertyConfigItem item = null;
            _weaponPartPropertyDic.TryGetValue(templateId, out item);
            if (item == null)
            {
                Logger.Error("Not found id" + templateId);
            }
            return item;
        }

        private WeaponPropertyConfigItem GetWeaponPropertyCfg(int templateId)
        {
            return FindByWeaponId(templateId);
        }
    }
}
