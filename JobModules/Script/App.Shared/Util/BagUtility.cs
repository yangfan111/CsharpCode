using Core.Utils;
using App.Shared.Components.Player;
using Core;
using XmlConfig;
using Utils.Configuration;
using Utils.Appearance;
using Assets.XmlConfig;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Shared
{
    public static class BagUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BagUtility));

        public static int GetRealAttachmentId(int attachId, int weaponId)
        {
            var cfg = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetConfigById(attachId);
            if (null == cfg)
            {
                return 0;
            }
            for (int i = 0; i < cfg.PartsList.Length; i++)
            {
                if (SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(cfg.PartsList[i], weaponId))
                {
                    return cfg.PartsList[i];
                }
            }
            return 0;
        }

        /// <summary>
        /// 该槽位的武器是否可能有配件
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool MayHasPart(this EWeaponSlotType slot)
        {
            switch(slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                case EWeaponSlotType.SecondaryWeapon:
                case EWeaponSlotType.PistolWeapon:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 该槽位的武器使用的时候是否引起数据变化 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool IsSlotChangeByCost(this EWeaponSlotType slot)
        {
            switch(slot)
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
            switch(slot)
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
                    Logger.ErrorFormat("slot {0} is illegal for weapon ", slot);
                    return WeaponInPackage.ThrownWeapon;
            }
        }

        public static EWeaponSlotType ToWeaponSlot(this EWeaponType weaponType)
        {
            switch(weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    return EWeaponSlotType.PrimeWeapon;
                case EWeaponType.SubWeapon:
                    return EWeaponSlotType.PistolWeapon;
                case EWeaponType.MeleeWeapon:
                    return EWeaponSlotType.MeleeWeapon;
                case EWeaponType.ThrowWeapon:
                    return EWeaponSlotType.ThrowingWeapon;
                case EWeaponType.TacticWeapon:
                    return EWeaponSlotType.TacticWeapon;
                default:
                    return EWeaponSlotType.None;
            }
        }

        public static bool MayHasPart(this EWeaponType weaponType)
        {
            switch(weaponType)
            {
                case EWeaponType.PrimeWeapon:
                case EWeaponType.SubWeapon:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanAutoPick(this EWeaponType weaponType)
        {
            switch(weaponType)
            {
                case EWeaponType.MeleeWeapon:
                case EWeaponType.PrimeWeapon:
                case EWeaponType.SubWeapon:
                case EWeaponType.TacticWeapon:
                    return true;
                default:
                    return false;
            }
        }

       
         
 
    }
}
