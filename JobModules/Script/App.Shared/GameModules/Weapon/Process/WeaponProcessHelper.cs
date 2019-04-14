using Assets.XmlConfig;
using Core;
using Core.CharacterState;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponProcessHelper" />
    /// </summary>
    internal class WeaponProcessHelper
    {
        public WeaponProcessHelper(IPlayerWeaponSharedGetter getter)
        {
            Getter = getter;
        }

        private IPlayerWeaponSharedGetter Getter;

     

        #region Filters
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

        internal bool FilterSameSpecies(EWeaponSlotType slot)
        {
            return slot != Getter.HeldSlotType;
        }

        internal bool FilterSlotValied(EWeaponSlotType slot)
        {
            return Getter.GetWeaponAgent(slot).IsValid();
        }

        internal bool FilterHoldSecondWeapon()
        {
            return (Getter.HeldSlotType == EWeaponSlotType.SecondaryWeapon);
        }

        internal bool FilterVailed(WeaponScanStruct orient, EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None) return false;
            if (orient.IsUnSafeOrEmpty()) return false;
            return Getter.ModeController.IsSlotValid(slot);
        }

        internal WeaponBaseAgent FilterNoneAgent(EWeaponSlotType slotType)
        {
            if (slotType == EWeaponSlotType.None) return null;
            var agent = Getter.GetWeaponAgent(slotType);
            if (!agent.IsValid())return null;
            return agent;
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

        #endregion

        #region util

        public bool CanCreateDropWeapon(EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.ThrowingWeapon;
        }
        
        public WeaponDrawAppearanceStruct GetDrawAppearanceStruct(EWeaponSlotType slot)
        {
            var weaponDrawStruct = new WeaponDrawAppearanceStruct();
            var secondWeapon     = FilterHoldSecondWeapon();
            weaponDrawStruct.holsterParam = WeaponUtil.GetHolsterParam(secondWeapon);
            weaponDrawStruct.targetSlot = slot;
            weaponDrawStruct.drawParam = secondWeapon
                ? AnimatorParametersHash.Instance.DrawLeftValue
                : AnimatorParametersHash.Instance.DrawRightValue;
            weaponDrawStruct.isSecondWeapon = secondWeapon;
            return weaponDrawStruct;
        }

        public EWeaponSlotType GetRealSlotType(EWeaponSlotType slot)
        {
            return slot == EWeaponSlotType.Pointer ? Getter.HeldSlotType : slot;
        }
        

        internal EWeaponSlotType GetMatchedSlotType(int configId)
        {
            WeaponResConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponConfigId(configId, out itemConfig))
                return EWeaponSlotType.None;
            return GetMatchedSlotType((EWeaponType_Config) itemConfig.Type);
        }

        internal EWeaponSlotType GetMatchedSlotType(EWeaponType_Config weaponType)
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
        #endregion
    }
}