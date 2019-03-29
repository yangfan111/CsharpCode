using Assets.XmlConfig;
using Core;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponProcessUtil" />
    /// </summary>
    internal class WeaponProcessUtil
    {
        public WeaponProcessUtil(IPlayerWeaponSharedGetter getter)
        {
            Getter = getter;
        }

        private IPlayerWeaponSharedGetter Getter;

        internal bool FilterPickup(EWeaponSlotType slotType)
        {
            return true;
            //return slotType != EWeaponSlotType.ThrowingWeapon ||
            //    Getter.IsWeaponSlotEmpty(slotType);
        }

        internal bool FilterAutoPickup(EWeaponSlotType slotType)
        {
            return slotType.CanAutoPick() && Getter.IsWeaponSlotEmpty(slotType);
        }

        internal EWeaponSlotType GetMatchedSlot(int configId)
        {
            WeaponResConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponConfigId(configId, out itemConfig))
                return EWeaponSlotType.None;
            return GetMatchedSlot((EWeaponType_Config)itemConfig.Type);
        }

        internal EWeaponSlotType GetMatchedSlot(EWeaponType_Config weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType_Config.PrimeWeapon:
                    var hasPrime = !Getter.IsWeaponSlotEmpty(EWeaponSlotType.PrimeWeapon);
                    if (hasPrime && Getter.ModeController.IsSlotValid(EWeaponSlotType.SecondaryWeapon))
                        return EWeaponSlotType.SecondaryWeapon;
                    return EWeaponSlotType.PrimeWeapon;
                default:
                    return weaponType.ToWeaponSlot();
            }
        }

        internal bool FilterVailed(WeaponScanStruct orient, EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None) return false;
            if (orient.IsUnSafeOrEmpty()) return false;
            return Getter.ModeController.IsSlotValid(slot) ;
        }

        internal bool FilterSwitchIn(EWeaponSlotType tarSlot)
        {
            return tarSlot != EWeaponSlotType.None && !Getter.IsWeaponSlotEmpty(tarSlot);
        }

        internal bool FilterDrop(EWeaponSlotType slot)
        {
            return slot != EWeaponSlotType.None && slot != EWeaponSlotType.ThrowingWeapon;
        }

        internal bool FilterSameSepeciesSwitchIn(EWeaponSlotType slot)
        {
            if (slot != EWeaponSlotType.ThrowingWeapon) return false;
            return Getter.CanUseGrenade;
        }

        internal bool FilterRefreshWeapon()
        {
            if (!Getter.HeldSlotType.MayHasPart())
            {
                return false;
            }
            return !Getter.IsHeldSlotEmpty;
        }
    }
}
