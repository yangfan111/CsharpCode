using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.player;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.item;
using commons.data;
using commons.data.mysql;
using Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Scripts;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.item.config
{
    public class FreeItemConfig
    {
        private static FreeItemInfo[] _itemInfos;

        private static Dictionary<int, Dictionary<int, string>> keyDic;
        private static MyDictionary<string, FreeItemInfo> keyItemDic = new MyDictionary<string, FreeItemInfo>();
        private static Dictionary<int, Dictionary<int, FreeItemInfo>> infoDic;
        public static Dictionary<int, int> partMap;
        public static Dictionary<int, string> avatarMap;

        static FreeItemConfig()
        {
            partMap = new Dictionary<int, int>();
            partMap.Add((int)EWeaponPartType.UpperRail, 1);
            partMap.Add((int)EWeaponPartType.Muzzle, 2);
            partMap.Add((int)EWeaponPartType.LowerRail, 3);
            partMap.Add((int)EWeaponPartType.Magazine, 4);
            partMap.Add((int)EWeaponPartType.Stock, 5);

            avatarMap = new Dictionary<int, string>();
            avatarMap.Add((int)Wardrobe.Armor, "armor");
            avatarMap.Add((int)Wardrobe.Cap, "hel");
            avatarMap.Add((int)Wardrobe.Bag, "bag");
            avatarMap.Add((int)Wardrobe.Waist, "belt");
            avatarMap.Add((int)Wardrobe.PendantFace, "mask");
            avatarMap.Add((int)Wardrobe.Inner, "vest");
            avatarMap.Add((int)Wardrobe.Outer, "coat");
            avatarMap.Add((int)Wardrobe.Glove, "glov");
            avatarMap.Add((int)Wardrobe.Trouser, "pant");
            avatarMap.Add((int)Wardrobe.Foot, "shoe");
        }

        public static FreeItemInfo GetItemInfo(string key)
        {
            IniItems();

            return keyItemDic[key];
        }

        public static int GetSound(string key)
        {
            FreeItemInfo info = GetItemInfo(key);
            if (info.cat == (int)ECategory.GameItem)
            {
                switch (info.id)
                {
                    case 100:
                        return 221;
                    case 101:
                        return 222;
                    case 102:
                        return 223;
                    case 103:
                        return 219;
                    case 104:
                        return 223;
                    case 105:
                        return 220;
                    case 106:
                        return 224;
                    default:
                        break;
                }
            }

            return -1;
        }

        public static void UseAnimation(Contexts contexts, FreeData fd, string key)
        {
            FreeItemInfo info = GetItemInfo(key);
            if (info.cat == 13 && (info.id == 103))
            {
                PlayerAnimationAction.DoAnimation(contexts, 0, fd.Player);
            }
            if (info.cat == 13 && (info.id == 105))
            {
                PlayerAnimationAction.DoAnimation(contexts, 4, fd.Player);
            }
            if (info.cat == 13 && (info.id == 102 || info.id == 104))
            {
                PlayerAnimationAction.DoAnimation(contexts, 1, fd.Player);
            }
            if (info.cat == 13 && (info.id == 101))
            {
                PlayerAnimationAction.DoAnimation(contexts, 2, fd.Player);
            }
            if (info.cat == 13 && (info.id == 100))
            {
                PlayerAnimationAction.DoAnimation(contexts, 3, fd.Player);
            }
        }

        public static int GetSing(FreeItem item)
        {
            FreeItemInfo info = GetItemInfo(item.GetKey());

            return info.sing;
        }

        public static string GetItemKey(int cat, int id)
        {
            IniItems();

            if (keyDic == null)
            {
                keyDic = new Dictionary<int, Dictionary<int, string>>();
            }

            if (!keyDic.ContainsKey(cat))
            {
                keyDic.Add(cat, new Dictionary<int, string>());
            }

            if (!keyDic[cat].ContainsKey(id))
            {
                keyDic[cat].Add(id, (cat * 10000 + id).ToString());
            }

            return keyDic[cat][id];
        }

        public static bool Contains(int cat, int id)
        {
            IniItems();

            return infoDic.ContainsKey(cat) && infoDic[cat].ContainsKey(id);
        }

        public static FreeItemInfo GetItemInfo(int cat, int id)
        {
            IniItems();

            if (infoDic.ContainsKey(cat) && infoDic[cat].ContainsKey(id))
            {
                return infoDic[cat][id];
            }

            return null;
        }

        private static void AddToInfoDic(FreeItemInfo info)
        {
            if (!infoDic.ContainsKey(info.cat))
            {
                infoDic.Add(info.cat, new Dictionary<int, FreeItemInfo>());
            }

            if (!infoDic[info.cat].ContainsKey(info.id))
            {
                infoDic[info.cat].Add(info.id, info);
            }

            keyItemDic[(info.cat * 10000 + info.id).ToString()] = info;
        }

        public static FreeItemInfo[] ItemInfos
        {
            get
            {
                IniItems();

                return _itemInfos;
            }
        }

        private static void IniItems()
        {
            if (_itemInfos == null)
            {
                infoDic = new Dictionary<int, Dictionary<int, FreeItemInfo>>();

                List<FreeItemInfo> list = new List<FreeItemInfo>();
                foreach (var item in SingletonManager.Get<RoleAvatarConfigManager>().GetConfig().Items)
                {
                    if (item != null)
                    {
                        FreeItemInfo itemInfo = new FreeItemInfo((int)ECategory.Avatar, item.Id, ((int)ECategory.Avatar * 10000 + item.Id).ToString(),
                            item.Name, "avatar", (avatarMap.ContainsKey(item.Type) ? avatarMap[item.Type] : item.Type.ToString()), "", "icon/roleavatar/RA_4_12_Icon", "icon/roleavatar/RA_4_12_Icon", 0);

                        if (!string.IsNullOrEmpty(item.Icon))
                        {
                            itemInfo.fIcon = item.IconBundle + "/" + item.Icon;
                            itemInfo.mIcon = item.IconBundle + "/" + item.Icon;
                            itemInfo.capacity = (int)item.Capacity;

                            list.Add(itemInfo);
                            AddToInfoDic(itemInfo);
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("item res {0} is not defined.", item.Id);
                    }
                }

                foreach (WeaponResConfigItem item in SingletonManager.Get<WeaponResourceConfigManager>().GetConfigs().Values)
                {
                    Utils.AssetManager.AssetInfo asset = SingletonManager.Get<WeaponAvatarConfigManager>().GetIcon(item.AvatorId);
                    FreeItemInfo itemInfo = new FreeItemInfo((int)ECategory.Weapon, item.Id, ((int)ECategory.Weapon * 10000 + item.Id).ToString(),
                        item.Name, "weapon", "w" + item.Type.ToString(), "", asset.BundleName + "/" + asset.AssetName, asset.BundleName + "/" + asset.AssetName, 0);
                    list.Add(itemInfo);
                    AddToInfoDic(itemInfo);
                }

                foreach (WeaponPartSurvivalConfigItem part in SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetConfigs().Values)
                {
                    if (part.PartsList != null)
                    {
                        WeaponPartsConfigItem item = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(part.PartsList[0]);
                        FreeItemInfo itemInfo = new FreeItemInfo((int)ECategory.WeaponPart, part.Id, ((int)ECategory.WeaponPart * 10000 + part.Id).ToString(),
                            part.Name, "part", "p" + partMap[item.Type].ToString(), "", "icon/weaponpart/" + item.Icon, "icon/weaponpart/" + item.Icon, 0);
                        itemInfo.weight = item.Weight;
                        list.Add(itemInfo);
                        AddToInfoDic(itemInfo);
                    }
                    else
                    {
                        Debug.LogWarningFormat("weapon part {0},{1},{2} is not valid.", part.Id, part.Name, part.PartsList);
                    }
                }

                foreach (GameItemConfigItem item in SingletonManager.Get<GameItemConfigManager>().GetConfig().Items)
                {
                    FreeItemInfo itemInfo = new FreeItemInfo((int)ECategory.GameItem, item.Id, ((int)ECategory.GameItem * 10000 + item.Id).ToString(),
                        item.Name, "use", "use", "", item.IconBundle + "/" + item.Icon, item.IconBundle + "/" + item.Icon, item.Sing);
                    itemInfo.weight = item.Weight;
                    itemInfo.stack = item.Stack;
                    list.Add(itemInfo);
                    AddToInfoDic(itemInfo);
                }

                _itemInfos = list.ToArray();
            }
        }
    }

    public class FreeItemInfo
    {
        public int cat;
        public int id;
        public string key;
        public string desc;
        public string name;
        public string type;
        public string subType;
        public string fIcon;
        public string mIcon;
        public float weight;
        public int sing;
        public int capacity;
        public int reduce;
        public int stack;

        public FreeItemInfo(int cat, int id, string key, string name, string type, string subType, string desc, string fIcon, string mIcon, int sing)
        {
            this.cat = cat;
            this.id = id;
            this.key = key;
            this.name = name;
            this.desc = desc;
            this.type = type;
            this.subType = subType;
            this.fIcon = fIcon;
            this.mIcon = mIcon;
            this.weight = 0f;
            this.sing = sing;
            this.capacity = 0;
            this.reduce = 0;
            this.stack = 1;
        }
    }

}
