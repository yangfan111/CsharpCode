using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utils.Configuration;
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
            _dict = new Dictionary<EItemCategory, Dictionary<int, ItemBaseConfig>>();
            var list = Enum.GetValues(typeof(EItemCategory));
            foreach (EItemCategory it in list)
            {
                _dict.Add(it, new Dictionary<int, ItemBaseConfig>());
            }
        }

        Dictionary<EItemCategory, Dictionary<int, ItemBaseConfig>> _dict;

        public ItemBaseConfig GetConfigById(int cat, int id,bool isSurvivalMode = false)
        {
            EItemCategory category = (EItemCategory)cat;
            ItemBaseConfig res = null;
            var dict = _dict[category];
            if (dict.TryGetValue(id, out res)) return res;

            switch (category)
            {
                case EItemCategory.BattleProp:
                    res = SingletonManager.Get<GameItemConfigManager>().GetConfigById(id);break;
                case EItemCategory.WeaponPart:
                    if (isSurvivalMode)
                    {
                        var config = SingletonManager.Get<WeaponPartSurvivalConfigManager>().FindConfigBySetId(id);
                        var partId = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(id);
                        var realConfig = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
                        if (config != null)
                            realConfig.Name = config.Name;
                        res = realConfig;
                    }
                    else
                    {
                        res = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(id);
                    }
                    break;
                case EItemCategory.Prop:
                    res = SingletonManager.Get<PropConfigManager>().GetConfigById(id); break;
                case EItemCategory.RoleAvatar:
                    res = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(id); break;
                case EItemCategory.Weapon:
                    var weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(id);
                    var weaponAvatarConfig = SingletonManager.Get<WeaponAvatarConfigManager>()
                        .GetConfigById(weaponConfig.AvatorId);
                    res = weaponConfig.Copy();
                    //res = weaponConfig;
                    if (weaponAvatarConfig != null)
                    {
                        res.IconBundle = weaponAvatarConfig.IconBundle;
                        res.Icon = weaponAvatarConfig.Icon;
                    }
                    break;
            }
            return res;


        }
    }

    public enum EItemCategory
    {
        None,
        Resource = 1,
        Weapon = 2,
        WeaponAvatar = 3,
        Role = 4,
        WeaponPart = 5,
        Carrier = 6,
        CarrierAvatar = 7,
        DogTag = 8,
        RoleAvatar = 9,
        Prop = 10,
        Pack = 11,
        BattleProp = 13   //局内道具
    }

}
