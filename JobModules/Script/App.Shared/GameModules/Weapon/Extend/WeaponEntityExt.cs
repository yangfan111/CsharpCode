using Core;
using Core.Configuration;
using Core.EntityComponent;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared
{
    /// <summary>
    /// 定义entity原子操作
    /// </summary>
    public static class WeaponEntityExt
    {
        internal static void SetFlagHasOwnwer(this WeaponEntity weaponEntity, EntityKey entityKey)
        {
            if (!weaponEntity.hasOwnerId)
            {
                weaponEntity.AddOwnerId(entityKey);
            }
            else
            {
                weaponEntity.ownerId.Value = entityKey;
            }
            weaponEntity.isFlagSyncSelf = true;
            weaponEntity.isFlagDestroy = false;
        }

        internal static void SetFlagNoOwner(this WeaponEntity weaponEntity)
        {

            weaponEntity.isFlagSyncSelf = false;
            weaponEntity.isFlagDestroy = true;
            weaponEntity.RemoveOwnerId();

        }

        internal static void SetFlagWaitDestroy(this WeaponEntity weaponEntity)
        {
            if(!weaponEntity.isFlagSyncSelf)
                weaponEntity.isFlagDestroy = true;
        }

        internal static void Activate(this WeaponEntity weaponEntity, bool activate)
        {
            weaponEntity.isActive = activate;
        }

        internal static WeaponScanStruct ToWeaponScan(this WeaponEntity weaponEntity)
        {
            weaponEntity.weaponScan.Value.SyncSelf(weaponEntity);
            return weaponEntity.weaponScan.Value;
        }

        internal static void SetFireMode(this WeaponEntity weapon, EFireMode model)
        {
            weapon.weaponBasicData.FireMode = (int)model;
        }

        internal static void ResetFireModel(this WeaponEntity weapoon)
        {
            if (weapoon.weaponBasicData.FireMode == 0)
            {
                bool hasAutoFireModel = SingletonManager.Get<WeaponDataConfigManager>().HasAutoFireMode(weapoon.weaponBasicData.ConfigId);
                if (hasAutoFireModel)
                    SetFireMode(weapoon, EFireMode.Auto);
                else
                    SetFireMode(weapoon, SingletonManager.Get<WeaponDataConfigManager>().GetFirstAvaliableFireMode(weapoon.weaponBasicData.ConfigId));
            }
        }
    }
}
