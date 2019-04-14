using App.Shared.Components.Player;
using Core;
using Core.EntityComponent;
using System;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerComponentsReference" />
    /// </summary>
    public partial class WeaponPlayerComponentsAgent
    {
        internal void RemoveBagWeapon(EWeaponSlotType slot)
        {
            var slotData = BagSet[0][slot];
            slotData.Remove(Customize.EmptyConstWeaponkey);//player slot 数据移除
        }

        internal void ClearBagPointer()
        {
            BagSet.ClearPointer();
        }

        internal void SyncBagWeapon(EWeaponSlotType slot, EntityKey key)
        {
            BagSet[0][slot].Sync(key);
        }

        internal void SetBagHeldSlotType(EWeaponSlotType nowSlot)
        {
            var slot = (int)nowSlot;
            var bag = BagSet[0];
            if (bag.HeldSlotPointer == slot)
                return;
            if (!WeaponUtil.VertifyEweaponSlotIndex(slot, true))
                return;
            bag.ChangeSlotPointer(slot);
        }

        internal Func<EntityKey> GenerateBagWeaponKeyExtractor(EWeaponSlotType slotType)
        {
            return () => { return BagSet[0][slotType].WeaponKey; };
        }

        internal Func<EntityKey> GenerateBagEmptyKeyExtractor()
        {
            return () => { return Customize.EmptyConstWeaponkey; };
        }
        internal void AddAuxBullet(PlayerBulletData bulletData)
        {
            if (WeaponAux.BulletList == null) return;
            WeaponAux.BulletList.Add(bulletData);
        }
        internal void AddAuxEffect()
        {
            WeaponAux.ClientInitialize = true;
            WeaponAux.EffectList = new List<EClientEffectType>();
        }
        internal void AddAuxBullet()
        {
            WeaponAux.BulletList = new List<PlayerBulletData>();
        }

        internal void AddAuxEffect(EClientEffectType effectType)
        {
            if (WeaponAux.EffectList != null)
                WeaponAux.EffectList.Add(effectType);
        }
   
        internal int? AutoFire
        {
            get
            {

                if (WeaponAux.HasAutoAction)
                    return WeaponAux.AutoFire;
                return null;
            }
            set { WeaponAux.AutoFire = value.Value; }
        }

        public bool? AutoThrowing
        {
            get
            {
                if (WeaponAux.HasAutoAction)
                    return WeaponAux.AutoThrowing;
                return null;
            }
            set { WeaponAux.AutoThrowing = value.Value; }
        }

    }
}