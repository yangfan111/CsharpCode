using App.Shared.Components.SceneObject;
using App.Shared.Components.Weapon;
using Assets.XmlConfig;
using Core;
using Core.EntityComponent;
using System.Collections.Generic;
using Utils.Appearance;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="WeaponCompositionExt" />
    /// </summary>
    public static class WeaponCompositionExt
    {
        /// <summary>
        /// sync WeaponObjectComponent from WeaponScanStruct 
        /// </summary>
        /// <param name="weaponSceneObject"></param>
        /// <param name="weaponInfo"></param>
        public static void FillPartList(this WeaponObjectComponent component, List<int> partList)
        {
            if (null == partList)
            {
                return;
            }
            partList.Clear();
            if (component.UpperRail > 0)
            {
                partList.Add(component.UpperRail);
            }
            if (component.LowerRail > 0)
            {
                partList.Add(component.LowerRail);
            }
            if (component.Magazine > 0)
            {
                partList.Add(component.Magazine);
            }
            if (component.Stock > 0)
            {
                partList.Add(component.Stock);
            }
            if (component.Muzzle > 0)
            {
                partList.Add(component.Muzzle);
            }
        }

        /// <summary>
        /// sync WeaponBasicDataComponent from WeaponScanStruct
        /// </summary>
        /// <param name="weaponComp"></param>
        /// <param name="weaponInfo"></param>
        public static void SyncSelf(this WeaponBasicDataComponent weaponComp, WeaponScanStruct weaponInfo)
        {
            weaponComp.ConfigId = weaponInfo.ConfigId;
            weaponComp.WeaponAvatarId = weaponInfo.AvatarId;
            //    DebugUtil.MyLog("Bullet Sync" + weaponInfo.Bullet,DebugUtil.DebugColor.Black);
            weaponComp.Bullet = weaponInfo.Bullet;
            weaponComp.ReservedBullet = weaponInfo.ReservedBullet;
            weaponComp.UpperRail = weaponInfo.UpperRail;
            weaponComp.LowerRail = weaponInfo.LowerRail;
            weaponComp.Magazine = weaponInfo.Magazine;
            weaponComp.Muzzle = weaponInfo.Muzzle;
            weaponComp.Stock = weaponInfo.Stock;
        }

        /// <summary>
        /// 该槽位的武器使用的时候是否引起数据变化 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool IsSlotChangeByCost(this EWeaponSlotType slot)
        {
            switch (slot)
            {
                default:
                case EWeaponSlotType.PrimeWeapon:
                case EWeaponSlotType.SecondaryWeapon:
                case EWeaponSlotType.PistolWeapon:
                case EWeaponSlotType.TacticWeapon:
                case EWeaponSlotType.ThrowingWeapon:
                    return true;
                case EWeaponSlotType.MeleeWeapon:
                    return false;
            }
        }

        public static bool IsSlotWithBullet(this EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                case EWeaponSlotType.SecondaryWeapon:
                case EWeaponSlotType.PistolWeapon:
                    return true;
                default:
                    return false;
            }
        }

        public static WeaponInPackage ToWeaponInPackage(this EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                    return WeaponInPackage.PrimaryWeaponOne;
                case EWeaponSlotType.SecondaryWeapon:
                    return WeaponInPackage.PrimaryWeaponTwo;
                case EWeaponSlotType.PistolWeapon:
                    return WeaponInPackage.SideArm;
                case EWeaponSlotType.MeleeWeapon:
                    return WeaponInPackage.MeleeWeapon;
                case EWeaponSlotType.ThrowingWeapon:
                    return WeaponInPackage.ThrownWeapon;
                case EWeaponSlotType.TacticWeapon:
                    return WeaponInPackage.TacticWeapon;
                default:
                    return WeaponInPackage.ThrownWeapon;
            }
        }

        public static EWeaponSlotType ToWeaponSlot(this EWeaponType_Config weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType_Config.PrimeWeapon:
                    return EWeaponSlotType.PrimeWeapon;
                case EWeaponType_Config.SubWeapon:
                    return EWeaponSlotType.PistolWeapon;
                case EWeaponType_Config.MeleeWeapon:
                    return EWeaponSlotType.MeleeWeapon;
                case EWeaponType_Config.ThrowWeapon:
                    return EWeaponSlotType.ThrowingWeapon;
                case EWeaponType_Config.TacticWeapon:
                    return EWeaponSlotType.TacticWeapon;
                default:
                    return EWeaponSlotType.None;
            }
        }

        public static bool MayHasPart(this EWeaponType_Config weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType_Config.PrimeWeapon:
                case EWeaponType_Config.SubWeapon:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 该槽位的武器是否可能有配件
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool MayHasPart(this EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                case EWeaponSlotType.SecondaryWeapon:
                case EWeaponSlotType.PistolWeapon:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanAutoPick(this EWeaponType_Config weaponType)
        {
            return weaponType != EWeaponType_Config.ThrowWeapon;
        }

        public static bool CanAutoPick(this EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.ThrowingWeapon;
        }

        //    switch (slotType)
        //    {
        //        case EWeaponSlotType.MeleeWeapon:
        //        case EWeaponSlotType.PrimeWeapon:
        //        case EWeaponSlotType.SubWeapon:
        //        case EWeaponSlotType.TacticWeapon:
        //            return true;
        //        default:
        //            return false;
        //    }
        //}
        public static bool IsValid(this EntityKey enittyKey)
        {
            return enittyKey != EntityKey.Default && enittyKey != default(EntityKey);
        }

        public static void BindCustomizeWeaponKey(this WeaponBagContainer bagContainer, EntityKey entityKey)
        {
            for (EWeaponSlotType i = EWeaponSlotType.None; i < EWeaponSlotType.Length; i++)
            {
                bagContainer[i].Remove(entityKey);
            }
        }

        public static bool IsUnSafeOrEmpty(this WeaponScanStruct scan)
        {
            return !scan.IsSafeVailed || scan.ConfigId == WeaponUtil.EmptyHandId;
        }
    }
}
