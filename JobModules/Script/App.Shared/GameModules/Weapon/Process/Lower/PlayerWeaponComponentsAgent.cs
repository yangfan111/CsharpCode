using App.Shared.Components.Player;
using App.Shared.Components.Weapon;
using Core;
using Core.EntityComponent;
using Core.Utils;
using System;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponComponentsAgent" />
    /// </summary>
    public class PlayerWeaponComponentsAgent
    {
        /// <summary>
        /// WeaponStateComponent
        /// </summary>
        private readonly Func<PlayerWeaponBagSetComponent>    playerWeaponBagExtractor;

        private readonly Func<OverrideBagComponent>           playerWeaponBagOverrideExtractor;

        private readonly Func<PlayerWeaponAuxiliaryComponent> playerWeaponAuxiliaryExtractor;

        private readonly Func<PlayerWeaponCustomizeComponent> playerCustomizeComponent;


        /// <summary>
        /// template cache，reduce gc
        /// </summary>



        public PlayerWeaponComponentsAgent(
                    Func<PlayerWeaponBagSetComponent> in_bagExtractor, Func<OverrideBagComponent> in_playerWeaponBagOverrideExtractor, Func<PlayerWeaponAuxiliaryComponent> in_playerWeaponAuxiliaryExtractor, Func<PlayerWeaponCustomizeComponent> in_playerCustomizeComponent)
        {
            playerWeaponBagExtractor         = in_bagExtractor;
            playerWeaponBagOverrideExtractor = in_playerWeaponBagOverrideExtractor;
            playerWeaponAuxiliaryExtractor   = in_playerWeaponAuxiliaryExtractor;
            playerCustomizeComponent         = in_playerCustomizeComponent;

        }

        internal void TryStuffGrenadeToSlot()
        {
        }

        internal void RemoveBagWeapon(EWeaponSlotType slot,int bagIndex)
        {
            var slotData = BagSetCache[bagIndex][slot];
            slotData.Remove(EmptyWeaponKey);//player slot 数据移除
        }
        internal void ClearBagPointer()
        {
            BagSetCache.ClearPointer();
        }
        internal void AddBagWeapon(EWeaponSlotType slot, EntityKey key,int bagIndex)
        {
            BagSetCache.SetSlotWeaponData(bagIndex, slot, key);
        }

        internal void SetHeldSlotType(EWeaponSlotType slot)
        {
            BagSetCache.SetHeldSlotIndex(-1, (int)slot);
        }

       
        internal Func<EntityKey> MakeBagWeaponKeyExtractor(EWeaponSlotType slotType, int bagIndex)
        {
            return () => { return BagSetCache[bagIndex][slotType].WeaponKey; };
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
            get{   return playerWeaponBagExtractor();}
        }
        

        internal OverrideBagComponent OverrideCache
        {
            get{return playerWeaponBagOverrideExtractor();}
        }

        internal PlayerWeaponAuxiliaryComponent AuxCache
        {
            get { return playerWeaponAuxiliaryExtractor(); }
        }
        public EntityKey EmptyWeaponKey { get { return playerCustomizeComponent().EmptyConstWeaponkey; } }
        internal EntityKey GrenadeWeaponKey { get { return playerCustomizeComponent().GrenadeConstWeaponKey; } }

        public EWeaponSlotType HeldSlotType { get { return (EWeaponSlotType)BagSetCache.HeldSlotIndex; } }
     
        public EWeaponSlotType LastSlotType { get { return   (EWeaponSlotType)BagSetCache.LastSlotIndex; } }

        public int HeldBagPointer
        {
            get { return BagSetCache.HeldBagPointer; }
            set { BagSetCache.HeldBagPointer = value; }
        }


        public bool IsHeldSlotType(EWeaponSlotType slot) { return HeldSlotType == slot; }

        internal bool IsHeldSlotType(EWeaponSlotType slot,int bagIndex)
        {
            if(bagIndex<0)
                return HeldSlotType == slot;
            return bagIndex == BagSetCache.HeldBagPointer && HeldSlotType == slot;
        }
        public bool IsHeldSlotEmpty { get { return BagSetCache.GetHeldSlotEntityKey() == EmptyWeaponKey ; } }
       

        public bool IsWeaponSlotEmpty(EWeaponSlotType slot)
        {
            return BagSetCache.GetSlotDataKey(slot) == EmptyWeaponKey ;
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
    }
}
