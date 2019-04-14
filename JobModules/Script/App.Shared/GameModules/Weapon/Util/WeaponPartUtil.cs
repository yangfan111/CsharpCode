using System.Collections.Generic;
using App.Shared.Components.SceneObject;
using App.Shared.Components.Weapon;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Appearance;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    public static class WeaponPartUtil
    {
        public static WeaponPartsStruct CreateParts(WeaponScanStruct info)
        {
            var result = new WeaponPartsStruct
            {
                LowerRail = info.LowerRail,
                UpperRail = info.UpperRail,
                Magazine = info.Magazine,
                Muzzle = info.Muzzle,
                Stock = info.Stock,
            };
            CombineDefaultParts(ref result, info.ConfigId);
            return result;
        }
        public static WeaponPartsStruct CreateParts(WeaponPartsAchive achive)
        {
            var result = new WeaponPartsStruct
            {
                LowerRail = achive.LowerRail,
                UpperRail = achive.UpperRail,
                Muzzle = achive.Muzzle,
                Stock = achive.Stock,
                Magazine = achive.Magazine,
            };
            return result;
        }
//        public static WeaponPartsStruct CreateParts(WeaponBasicDataComponent comp)
//        {
//            var result = new WeaponPartsStruct
//            {
//                LowerRail = comp.LowerRail,
//                UpperRail = comp.UpperRail,
//                Muzzle = comp.Muzzle,
//                Stock = comp.Stock,
//                Magazine = comp.Magazine,
//            };
//
//            CombineDefaultParts(ref result, comp.ConfigId);
//            return result;
//        }

        /// <summary>
        /// 添加装备默认配件信息
        /// </summary>
        /// <param name="result"></param>
        /// <param name="weaponId"></param>
        public static void CombineDefaultParts(ref WeaponPartsStruct result, int weaponId)
        {
            var defaultParts = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weaponId).DefaultParts;

            result.LowerRail = result.LowerRail > 0 ? result.LowerRail : defaultParts.LowerRail;
            result.UpperRail = result.UpperRail > 0 ? result.UpperRail : defaultParts.UpperRail;
            result.Muzzle = result.Muzzle > 0 ? result.Muzzle : defaultParts.Muzzle;
            result.Magazine = result.Magazine > 0 ? result.Magazine : defaultParts.Magazine;
            result.Stock = result.Stock > 0 ? result.Stock : defaultParts.Stock;
        }

        public static void ApplyParts(this WeaponBasicDataComponent comp, WeaponPartsStruct attach)
        {
            comp.LowerRail = attach.LowerRail;
            comp.UpperRail = attach.UpperRail;
            comp.Muzzle = attach.Muzzle;
            comp.Magazine = attach.Magazine;
            comp.Stock = attach.Stock;
        }
        public static void ApplyParts(this WeaponPartsAchive partsAchive,WeaponBasicDataComponent result)
        {
            partsAchive.LowerRail = result.LowerRail > 0 ? result.LowerRail : partsAchive.LowerRail;
            partsAchive.UpperRail = result.UpperRail > 0 ? result.UpperRail : partsAchive.UpperRail;
            partsAchive.Muzzle = result.Muzzle > 0 ? result.Muzzle : partsAchive.Muzzle;
            partsAchive.Magazine = result.Magazine > 0 ? result.Magazine : partsAchive.Magazine;
            partsAchive.Stock = result.Stock > 0 ? result.Stock : partsAchive.Stock;
        }
        public static void ApplyParts(this WeaponObjectComponent result,WeaponPartsAchive partsAchive)
        {
            result.LowerRail = result.LowerRail > 0 ? result.LowerRail : partsAchive.LowerRail;
            result.UpperRail = result.UpperRail > 0 ? result.UpperRail : partsAchive.UpperRail;
            result.Muzzle    = result.Muzzle > 0 ? result.Muzzle : partsAchive.Muzzle;
            result.Magazine  = result.Magazine > 0 ? result.Magazine : partsAchive.Magazine;
            result.Stock     = result.Stock > 0 ? result.Stock : partsAchive.Stock;
        }

        public static List<int> CollectParts(this WeaponObjectComponent data)
        {
            var list = new List<int>(5);
            if(data.LowerRail > 0 )
                list.Add(data.LowerRail);
            if(data.UpperRail > 0 )
                list.Add(data.UpperRail);
            if(data.Muzzle > 0 )
                list.Add(data.Muzzle);
            if(data.Magazine > 0 )
                list.Add(data.Magazine);
            if(data.Stock > 0 )
                list.Add(data.Stock);
            return list;

        }

        /// <summary>
        /// 根据PartType修改配件信息
        /// </summary>
        /// <param name="attach"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WeaponPartsStruct ModifyPartItem(WeaponPartsStruct attach, EWeaponPartType type, int id)
        {
            switch (type)
            {
                case EWeaponPartType.LowerRail:
                    attach.LowerRail = id;
                    break;
                case EWeaponPartType.UpperRail:
                    attach.UpperRail = id;
                    break;
                case EWeaponPartType.Muzzle:
                    attach.Muzzle = id;
                    break;
                case EWeaponPartType.Magazine:
                    attach.Magazine = id;
                    break;
                case EWeaponPartType.Stock:
                    attach.Stock = id;
                    break;
            }

            return attach;
        }

        /// <summary>
        /// 通过survivalCfgId查找适合weapon的首个配件信息
        /// </summary>
        /// <param name="survivalCfgId"></param>
        /// <param name="weaponId"></param>
        /// <returns></returns>
        public static int GetWeaponFstMatchedPartId(int survivalCfgId, int weaponId)
        {
            var cfg = SingletonManager.Get<WeaponPartSurvivalConfigManager>().FindConfigBySetId(survivalCfgId);
            if (null == cfg)
                return 0;
            for (int i = 0; i < cfg.PartsList.Length; i++)
            {
                if (SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(cfg.PartsList[i], weaponId))
                {
                    return cfg.PartsList[i];
                }
            }

            return 0;
        }

        public static int[] ToAniLocation(this WeaponPartsStruct parts)
        {
            int[] arr = new int[(int) WeaponPartLocation.EndOfTheWorld];
            arr[(int) WeaponPartLocation.LowRail] = parts.LowerRail;
            arr[(int) WeaponPartLocation.Scope] = parts.UpperRail;
            arr[(int) WeaponPartLocation.Buttstock] = parts.Stock;
            arr[(int) WeaponPartLocation.Muzzle] = parts.Muzzle;
            arr[(int) WeaponPartLocation.Magazine] = parts.Magazine;
            return arr;
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponPartUtil));

        public static void RefreshWeaponPartModels(ICharacterAppearance appearance, int weaponId,
            WeaponPartsStruct oldParts, WeaponPartsStruct parts, EWeaponSlotType slot)
        {
            var weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
            if (null == weaponConfig)
                return;
            if (!((EWeaponType_Config) weaponConfig.Type).MayHasPart())
            {
                Logger.WarnFormat("weapon type {0} has no attachment by default ", weaponConfig.Type);
                return;
            }

            var pos = slot.ToWeaponInPackage();
          //  var oldArr = oldParts.ToAniLocation();
            var newArr = parts.ToAniLocation();
            for (WeaponPartLocation tmp = WeaponPartLocation.None + 1; tmp < WeaponPartLocation.EndOfTheWorld; tmp++)
            {
                if (newArr[(int) tmp] > 0)
                {
                    appearance.MountAttachment(pos, tmp, newArr[(int) tmp]);
                }
                else
                {
                     appearance.UnmountAttachment(pos, tmp);
                }
            }
        }
        
  
        
    }
}