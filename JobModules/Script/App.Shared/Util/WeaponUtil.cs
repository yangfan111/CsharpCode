using App.Shared.Components.Bag;
using App.Shared.Components.Weapon;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Enums;
using System;
using System.Collections.Generic;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.Util
{
    public static class WeaponUtil
    {
        #region//vertify
        /// <summary>
        /// 验证WeaponId合法
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns></returns>
        public static bool IsWeaponIdVailed(int weaponId)
        {
            NewWeaponConfigItem config;
            return IsWeaponIdVailed(weaponId, out config);
        }
        public static bool IsWeaponIdVailed(int weaponId, out NewWeaponConfigItem config)
        {
            config = null;
            if (weaponId == 0)
                return false;
            config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            return config != null;
        }
   
        public static bool IsVailed(this WeaponInfo wpInfo)
        {
            NewWeaponConfigItem itemCfg;
            return IsVailed(wpInfo, out itemCfg);

        }
        public static bool IsVailed(this WeaponInfo wpInfo, out NewWeaponConfigItem wpConfig)
        {
            wpConfig = null;
            if (wpInfo.Id < 1)
                return false;
            wpConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(wpInfo.Id);
            if (wpConfig == null)
                return false;
            return true;
        }
      


        #endregion
        #region// Component ext
        /// <summary>
        /// WeaponBag -> weapnKey
        /// </summary>
        /// <param name="bagComponent"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static int ToWeaponKey(this WeaponBagComponent bagComponent, EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                    return bagComponent.PrimeWeapon;
                case EWeaponSlotType.SecondaryWeapon:
                    return bagComponent.SecondaryWeapon;
                case EWeaponSlotType.PistolWeapon:
                    return bagComponent.PistolWeapon;
                case EWeaponSlotType.MeleeWeapon:
                    return bagComponent.MeleeWeapon;
                case EWeaponSlotType.ThrowingWeapon:
                    return bagComponent.ThrowingWeapon;
                case EWeaponSlotType.TacticWeapon:
                    return bagComponent.TacticWeapon;

            }
            return -1;
        }
        /// <summary>
        /// weaponKey => WeaponBag
        /// </summary>
        /// <param name="bagComponent"></param>
        /// <param name="slot"></param>
        /// <param name="weaponKey"></param>
        public static void SetSlotWeaponKey(this WeaponBagComponent bagComponent, EWeaponSlotType slot, int weaponKey)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                    bagComponent.PrimeWeapon = weaponKey;
                    break;
                case EWeaponSlotType.SecondaryWeapon:
                    bagComponent.SecondaryWeapon = weaponKey;
                    break;
                case EWeaponSlotType.PistolWeapon:
                    bagComponent.PistolWeapon = weaponKey;
                    break;
                case EWeaponSlotType.MeleeWeapon:
                    bagComponent.MeleeWeapon = weaponKey;
                    break;
                case EWeaponSlotType.ThrowingWeapon:
                    bagComponent.ThrowingWeapon = weaponKey;
                    break;
                case EWeaponSlotType.TacticWeapon:
                    bagComponent.TacticWeapon = weaponKey;
                    break;
            }


        }
        public static bool IsVailed(this WeaponEntity entity, Contexts context)
        {
            return entity != context.weapon.flagEmptyHandEntity;
        }

        #endregion
        public static WeaponEntity WeaponKeyToEntity(Contexts contexts, int weaponKey)
        {
            return contexts.weapon.GetEntityWithEntityKey(new EntityKey(weaponKey, (short)EEntityType.Weapon));
        }





    }
}

