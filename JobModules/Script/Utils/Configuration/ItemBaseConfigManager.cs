using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Configuration
{

    public class ItemBaseConfigManager : Singleton<ItemBaseConfigManager>
    {
        public ItemBaseConfigManager()
        {
            InitDict();
        }

        private void InitDict()
        {
            _dict = new Dictionary<ECategory, Dictionary<int, ItemBaseConfig>>();
            var list = Enum.GetValues(typeof(ECategory));
            foreach (ECategory it in list)
            {
                _dict.Add(it, new Dictionary<int, ItemBaseConfig>());
            }
        }

        Dictionary<ECategory, Dictionary<int, ItemBaseConfig>> _dict;

        public ItemBaseConfig GetConfigById(int cat, int id,bool isSurvivalMode = false)
        {
            ECategory category = (ECategory)cat;
            ItemBaseConfig res = null;
            var dict = _dict[category];
            if (dict.TryGetValue(id, out res)) return res;

            switch (category)
            {
                case ECategory.GameItem:
                    res = SingletonManager.Get<GameItemConfigManager>().GetConfigById(id);break;
                case ECategory.WeaponPart:
                    if (isSurvivalMode)
                    {
                        var config = SingletonManager.Get<WeaponPartSurvivalConfigManager>().FindConfigBySetId(id);
                        var partId = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(id);
                        var realConfig = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
                        if (config != null && realConfig != null)
                            realConfig.Name = config.Name;
                        res = realConfig;
                    }
                    else
                    {
                        res = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(id);
                    }
                    break;
                case ECategory.Prop:
                    res = SingletonManager.Get<PropConfigManager>().GetConfigById(id); break;
                case ECategory.Avatar:
                    res = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(id); break;
                case ECategory.Weapon:
                    var weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(id);
                    if (weaponConfig == null) break;
                    var weaponAvatarConfig = SingletonManager.Get<WeaponAvatarConfigManager>()
                        .GetConfigById(weaponConfig.AvatorId);
                    res = weaponConfig.Copy();
                    if (weaponAvatarConfig != null)
                    {
                        res.IconBundle = weaponAvatarConfig.IconBundle;
                        res.Icon = weaponAvatarConfig.Icon;
                    }
                    break;
            }
            dict[id] = res;
            return res;


        }
    }

}
