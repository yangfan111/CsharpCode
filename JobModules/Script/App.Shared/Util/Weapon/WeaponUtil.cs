using App.Shared.Components.Bag;
using App.Shared.Components.Weapon;
using Assets.Utils.Configuration;
using Core;
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
        public static bool VertifyWeaponId(int weaponId)
        {
            NewWeaponConfigItem config;
            return VertifyWeaponId(weaponId, out config);
        }
        public static bool VertifyWeaponId(int weaponId, out NewWeaponConfigItem config)
        {
            config = null;
            if (weaponId == 0)
                return false;
            config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            return config != null;
        }
        /// <summary>
        /// 验证weaponDataComponent合法
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static EFuncResult VertifyWeaponComponent(WeaponBasicInfoComponent comp)
        {
            NewWeaponConfigItem itemCfg;
            return VertifyWeaponComponent(comp, out itemCfg);
        }

        public static bool VertifyWeaponInfo(WeaponInfo wpInfo)
        {
            NewWeaponConfigItem itemCfg;
            return VertifyWeaponInfo(wpInfo, out itemCfg);

        }
        public static bool VertifyWeaponInfo(WeaponInfo wpInfo, out NewWeaponConfigItem wpConfig)
        {
            wpConfig = null;
            if (wpInfo.Id < 1)
                return false;
            wpConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(wpInfo.Id);
            if (wpConfig == null)
                return false;
            return true;
        }
        /// <summary>
        /// 验证weaponDataComponent合法
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static EFuncResult VertifyWeaponComponent(WeaponBasicInfoComponent comp, out NewWeaponConfigItem wpConfig)
        {
            wpConfig = null;
            if (comp == null || comp.WeaponId < 1)
                return EFuncResult.Exception;
            wpConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(comp.WeaponId);
            if (wpConfig == null)
                return EFuncResult.Exception;
            return EFuncResult.Success;
        }
 
        /// <summary>
        /// 判断weaponComponentData是已装填
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool VertifyWeaponComponentStuffed(WeaponBasicInfoComponent comp)
        {
            return comp != null && comp.WeaponId > 0;
        }

        #endregion
    }
}

