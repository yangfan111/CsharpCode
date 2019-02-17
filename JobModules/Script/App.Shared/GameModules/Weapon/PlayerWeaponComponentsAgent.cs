using App.Shared.Components.Player;
using App.Shared.Components.Weapon;
using Core;
using Core.EntityComponent;
using System;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponComponentsAgent" />
    /// </summary>
    public class PlayerWeaponComponentsAgent : ISharedPlayerWeaponComponentGetter
    {
        /// <summary>
        /// WeaponStateComponent
        /// </summary>
        private readonly Func<PlayerWeaponBagSetComponent> playerWeaponBagExtractor;

        private readonly Func<OverrideBagComponent> playerWeaponBagOverrideExtractor;

        private readonly Func<PlayerWeaponAuxiliaryComponent> playerWeaponAuxiliaryExtractor;

        /// <summary>
        /// weaponController，正常状态下不应对Controlller持有
        /// </summary>
        private PlayerWeaponController controller;

        /// <summary>
        /// template cache，reduce gc
        /// </summary>
        private PlayerWeaponBagSetComponent bagSetCache;

        private OverrideBagComponent overrideCache;

        private PlayerWeaponAuxiliaryComponent auxCache;

        private static readonly int DefaultBagIndexParam = -1;

        public PlayerWeaponComponentsAgent(
                    Func<PlayerWeaponBagSetComponent> in_bagExtractor, Func<OverrideBagComponent> in_playerWeaponBagOverrideExtractor, Func<PlayerWeaponAuxiliaryComponent> in_playerWeaponAuxiliaryExtractor)
        {
            playerWeaponBagExtractor = in_bagExtractor;
            playerWeaponBagOverrideExtractor = in_playerWeaponBagOverrideExtractor;
            playerWeaponAuxiliaryExtractor = in_playerWeaponAuxiliaryExtractor;

        }

        public void SetController(PlayerWeaponController in_controller)
        {
            controller = in_controller;
        }

        internal void TryStuffGrenadeToSlot()
        {
        }

        internal void RemoveSlotWeapon(EWeaponSlotType slot)
        {
            var slotData = BagSetCache.GetSlotData(slot);
            slotData.Remove();//player slot 数据移除
        }

        internal void AddSlotWeapon(EWeaponSlotType slot, EntityKey key)
        {
            BagSetCache.SetSlotWeaponData(DefaultBagIndexParam, slot, key);
        }

        internal void SetHeldSlotType(EWeaponSlotType slot)
        {
            BagSetCache.SetHeldSlotIndex(DefaultBagIndexParam, (int)slot);
        }

        internal void AddSlotWeaponListener(EWeaponSlotType slotType, int bagIndex, WeaponSlotUpdateEvent listener)
        {
            BagSetCache[bagIndex][slotType].OnWeaponUpdate += listener;
        }

        internal void AddHeldWeaponListener(WeaponHeldUpdateEvent listener)
        {
            BagSetCache[DefaultBagIndexParam].OnHeldWeaponUpdate += listener;
        }

     
     
        /// <summary>
        /// 手雷物品自动填充
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="autoStuffSlot"></param>
        /// <returns></returns>
        internal WeaponBasicDataComponent TryStuffEmptyGrenadeSlot(EWeaponSlotType slot, bool autoStuffSlot = false)
        {
            return null;
        }

        /// <summary>
        /// 自动查找当前可用手雷,no vertify
        /// </summary>
        /// <param name="grenadeComp"></param>
        internal void TryStuffEmptyGrenadeSlot()
        {
        }

        private PlayerWeaponBagSetComponent BagSetCache
        {
            get
            {
                bagSetCache = playerWeaponBagExtractor();
                return bagSetCache;
            }
        }

        internal OverrideBagComponent OverrideCache
        {
            get
            {
                overrideCache = playerWeaponBagOverrideExtractor();
                return overrideCache;
            }
        }

        internal PlayerWeaponAuxiliaryComponent AuxCache
        {
            get
            {
                auxCache = playerWeaponAuxiliaryExtractor();
                return auxCache;
            }
        }

        public EWeaponSlotType HeldSlotType
        {
            get
            {
                return (EWeaponSlotType)BagSetCache.HeldSlotIndex;
            }
        }

        public EWeaponSlotType LastSlotType
        {
            get
            {
                return (EWeaponSlotType)BagSetCache.LastSlotIndex;
            }
        }

        public int HeldBagPointer
        {
            get
            {
                return BagSetCache.HeldBagPointer;
            }
        }

        public bool IsHeldSlotType(EWeaponSlotType slot)
        {
            return HeldSlotType == slot;
        }

    
      

    public bool IsHeldSlotEmpty
    {
        get
        {
            return BagSetCache.IsHeldSlotEmpty();
        }
    }

    public bool IsWeaponSlotEmpty(EWeaponSlotType slot)
    {
        return BagSetCache.GetSlotData(slot).IsEmpty;
    }

    public EWeaponSlotType PollGetLastSlotType()
    {
        EWeaponSlotType last = LastSlotType;
        if (last != EWeaponSlotType.None && !IsWeaponSlotEmpty(last))
        {
            return last;
        }
        else
        {
            for (EWeaponSlotType s = EWeaponSlotType.None + 1; s < EWeaponSlotType.Length; s++)
            {
                if (!IsWeaponSlotEmpty(s))
                    return s;
            }
            return EWeaponSlotType.None;
        }
    }

    public int GetReservedBullet(EWeaponSlotType slot)
    {
        if (slot.IsSlotWithBullet())
        {
            return controller.GetReservedBullet(slot);
        }
        return 0;
    }

    public int GetReservedBullet(EBulletCaliber caliber)
    {
        return controller.GetReservedBullet(caliber);
    }

    public int GetReservedBullet()
    {
        return controller.GetReservedBullet();
    }
}
}
