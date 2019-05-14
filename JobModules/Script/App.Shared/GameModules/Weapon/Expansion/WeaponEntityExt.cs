using Core;
using Core.EntityComponent;
using WeaponConfigNs;

namespace App.Shared
{
    /// <summary>
    /// 定义entity原子操作
    /// </summary>
    public static class WeaponEntityExt
    {

        public static void SetRetain(this WeaponEntity weaponEntity,EntityKey owner)
        {
            if (!owner.IsValid())
                return;
            weaponEntity.isFlagSyncSelf = true;
            weaponEntity.isFlagSyncNonSelf = false;
            if (weaponEntity.hasOwnerId)
                weaponEntity.ownerId.Value = owner;
            else
                weaponEntity.AddOwnerId(owner);
        }
    
        public static void SetDestroy(this WeaponEntity weaponEntity)
        {
            if (weaponEntity.hasOwnerId)
                weaponEntity.RemoveOwnerId();
            weaponEntity.isFlagSyncSelf = false;
            weaponEntity.isFlagSyncNonSelf = false;
            weaponEntity.isFlagDestroy = true;
        }

        [System.Obsolete]
        internal static void Activate(this WeaponEntity weaponEntity, bool activate)
        {
            weaponEntity.isActive = activate;
        }

        internal static WeaponScanStruct ToWeaponScan(this WeaponEntity weaponEntity, WeaponPartsAchive partsAchive)
        {
            if (!weaponEntity.hasWeaponScan)
                weaponEntity.AddWeaponScan();
            weaponEntity.weaponScan.CopyFrom(weaponEntity.weaponBasicData);
            weaponEntity.weaponScan.CopyFrom(partsAchive);
            return weaponEntity.weaponScan.Value;
        }
    }
}
