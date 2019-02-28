using Core;
using Core.EntityComponent;
using System;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="ISharedPlayerWeaponGetter" />
    /// </summary>
    public interface IPlayerWeaponImplentment : ISharedPlayerWeaponGetter
    {
        void DrawWeapon(EWeaponSlotType slot, bool includeAction = true);

        void TryArmWeapon(EWeaponSlotType slot);

        void UnArmHeldWeapon(Action onfinish);

        void ForceUnArmHeldWeapon();

        EntityKey? DropWeapon(EWeaponSlotType slot,bool destroyImmediately);
        EntityKey? DropWeapon(bool destroyImmediately);

        void DestroyWeapon(EWeaponSlotType slotType, int bagIndex);

        bool AutoPickUpWeapon(WeaponScanStruct orient);

        EntityKey? PickUpWeapon(WeaponScanStruct orient, bool arm = true)

        void SwitchIn(EWeaponSlotType in_slot);

        void PureSwitchIn(EWeaponSlotType in_slot);

        void ExpendAfterAttack(EWeaponSlotType slot);

        bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient);

        bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient, out EntityKey lastKey);

        void Interrupt();

        void SetReservedBullet(int count);

        void SetReservedBullet(EWeaponSlotType slot, int count);

        int SetReservedBullet(EBulletCaliber caliber, int count);

        bool SetWeaponPart(EWeaponSlotType slot, int id);

        void DeleteWeaponPart(EWeaponSlotType slot, EWeaponPartType part);
    }
}
