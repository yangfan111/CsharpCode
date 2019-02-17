using App.Shared.Components.Player;
using App.Shared.Components.SceneObject;
using App.Shared.Components.Weapon;
using App.Shared.GameModules.Weapon;
using Core;
using Core.EntityComponent;
using WeaponConfigNs;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="WeaponCompositionExt" />
    /// </summary>
    public static class WeaponCompositionExt
    {

        public static void SyncSelf(this WeaponObjectComponent weaponSceneObject, WeaponScanStruct weaponInfo)
        {
            weaponSceneObject.WeaponKey = WeaponUtil.IsWeaponKeyVaild(weaponInfo.WeaponKey )? weaponInfo.WeaponKey:EntityKey.Default;
            weaponSceneObject.ConfigId= weaponInfo.ConfigId;
        }

        //用于weaponEntity创建
        public static void SyncSelf(this WeaponBasicDataComponent weaponComp, WeaponScanStruct weaponInfo)
        {
            weaponComp.ConfigId = weaponInfo.ConfigId;
            weaponComp.WeaponAvatarId = weaponInfo.AvatarId;
            weaponComp.Bullet = weaponInfo.Bullet;
            weaponComp.ReservedBullet = weaponInfo.ReservedBullet;
            weaponComp.UpperRail = weaponInfo.UpperRail;
            weaponComp.LowerRail = weaponInfo.LowerRail;
            weaponComp.Magazine = weaponInfo.Magazine;
            weaponComp.Muzzle = weaponInfo.Muzzle;
            weaponComp.Stock = weaponInfo.Stock;
        }

     
        public static void SyncSelf(this WeaponScanStruct weaponScan, WeaponEntity weaponEntity)
        {
            var weaponBasicData = weaponEntity.weaponBasicData;
            weaponScan.Assign(weaponEntity.entityKey.Value, weaponBasicData.ConfigId);
            weaponScan.AvatarId = weaponBasicData.WeaponAvatarId;
            weaponScan.UpperRail = weaponBasicData.UpperRail;
            weaponScan.LowerRail = weaponBasicData.LowerRail;
            weaponScan.Magazine = weaponBasicData.Magazine;
            weaponScan.Muzzle = weaponBasicData.Muzzle;
            weaponScan.Bullet = weaponBasicData.Bullet;
            weaponScan.Stock = weaponBasicData.Stock;
            weaponScan.ReservedBullet = weaponBasicData.ReservedBullet;
            weaponScan.FireMode = (EFireMode)weaponBasicData.FireMode;
            weaponScan.PullBolt = weaponBasicData.PullBolt;
        }

        public static WeaponBagSlotData GetSlotData(this PlayerWeaponBagSetComponent compoennt)
        {
            return compoennt.HeldBagContainer.HeldSlotData;
        }

        public static WeaponBagSlotData GetSlotData(this PlayerWeaponBagSetComponent component, EWeaponSlotType slot)
        {
            return component.HeldBagContainer[slot];
        }

        public static EntityKey GetSlotDataKey(this PlayerWeaponBagSetComponent component, EWeaponSlotType slot)
        {
            return component.HeldBagContainer[slot].WeaponKey;
        }

        public static EntityKey GetHeldSlotEntityKey(this PlayerWeaponBagSetComponent component)
        {
            return component.HeldBagContainer.HeldSlotData.WeaponKey;
        }

        public static bool IsHeldSlotEmpty(this PlayerWeaponBagSetComponent component)
        {
            return component.HeldBagContainer.HeldSlotData.IsEmpty;
        }

        public static bool IsSlotEmpty(this PlayerWeaponBagSetComponent component, EWeaponSlotType slot)
        {
            return component.HeldBagContainer[slot].IsEmpty;
        }

        public static void SetHeldSlotIndex(this PlayerWeaponBagSetComponent component, int bagIndex, int nowSlot)
        {
            var bag = component[bagIndex];
            if (bag.HeldSlotPointer == nowSlot)
                return;
            if (!WeaponUtil.VertifyEweaponSlotIndex(nowSlot, true))
                return;
            bag.ChangeSlotPointer(nowSlot);
        }

        public static void SetSlotWeaponData(this PlayerWeaponBagSetComponent component, int bagIndex, EWeaponSlotType nowSlot, EntityKey weaponKey)
        {
            if (bagIndex < 0)
                bagIndex = component.HeldBagPointer;
            component[bagIndex][nowSlot].Sync(weaponKey);
        }
    }
}
