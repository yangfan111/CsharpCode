using Assets.Utils.Configuration;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using System.Collections.Generic;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared
{
    /// <summary>
    /// 定义entity原子操作
    /// </summary>
    public static class WeaponEntityExt
    {

        internal static void SetFlagOwner(this WeaponEntity weaponEntity,EntityKey owner)
        {
            if (weaponEntity.entityKey.Value == EntityKey.Default)
                return;
            if(owner == EntityKey.Default)
            {
                if (weaponEntity.hasOwnerId)
                    weaponEntity.RemoveOwnerId();
                weaponEntity.isFlagSyncSelf = false;
                weaponEntity.isFlagSyncNonSelf = true;
                //weaponEntity.isFlagSyncSelf = false;
            }
            else
            {
                if(!weaponEntity.hasOwnerId)
                {
                    weaponEntity.AddOwnerId(owner);
                }
                else
                {
                    weaponEntity.ownerId.Value = owner;
                }
                weaponEntity.isFlagSyncSelf = true;
                weaponEntity.isFlagSyncNonSelf = false;
            }
           
        }
        internal static void SetFlagNoOwner(this WeaponEntity weaponEnity)
        {
            SetFlagOwner(weaponEnity,EntityKey.Default);
        }
      
    
        internal static void SetFlagWaitDestroy(this WeaponEntity weaponEntity)
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

        internal static WeaponScanStruct ToWeaponScan(this WeaponEntity weaponEntity)
        {
            if (!weaponEntity.hasWeaponScan)
                weaponEntity.AddWeaponScan();
            weaponEntity.weaponScan.SyncSelf(EntityKey.Default,weaponEntity.weaponBasicData);
            return weaponEntity.weaponScan.Value;
        }

    
      
        internal static void Recycle(this WeaponEntity weapon,bool hard = false)
        {
            weapon.weaponBasicData.Reset();
            weapon.weaponRuntimeData.Reset();
            if (hard)
                weapon.entityKey.Value = EntityKey.Default;


        }
        //public static void FillPartList(this WeaponEntity entity, List<int> partList)
        //{
        //    if (null == partList)
        //    {
        //        return;
        //    }
        //    partList.Clear();
        //    if (entity.weaponBasicData.UpperRail > 0)
        //    {
        //        partList.Add(entity.weaponBasicData.UpperRail);
        //    }
        //    if (entity.weaponBasicData.LowerRail > 0)
        //    {
        //        partList.Add(entity.weaponBasicData.LowerRail);
        //    }
        //    if (entity.weaponBasicData.Magazine > 0)
        //    {
        //        partList.Add(entity.weaponBasicData.Magazine);
        //    }
        //    if (entity.weaponBasicData.Stock > 0)
        //    {
        //        partList.Add(entity.weaponBasicData.Stock);
        //    }
        //    if (entity.weaponBasicData.Muzzle > 0)
        //    {
        //        partList.Add(entity.weaponBasicData.Muzzle);
        //    }
        //}
    }
}
