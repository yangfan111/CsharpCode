using Core.Utils;
using Core.Bag;

using App.Shared.Components.Bag;
using System.Collections.Generic;
using App.Shared.Util;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;
using Utils.Singleton;
using Core.Configuration;
using Core.Enums;
using Assets.XmlConfig;
using Core;
using XmlConfig;

namespace App.Shared.WeaponLogic
{
    public partial class PlayerWeaponComponentAgent : IPlayerWeaponComponentArchive
    {
        #region//Gettter
        public EWeaponSlotType HeldSlotType
        {
            get
            {
                var stateCmp = stateExtractor(false);
                CommonUtil.WeakAssert(stateCmp != null);
                return WeaponUtil.GetSlotByStateComponent(stateCmp);
            }
        }
        public int HeldSlotWeaponId
        {
            get { return GetSLot__WeaponId(HeldSlotType); }
        }
        public WeaponInfo HeldSlotWeaponInfo
        {
            get { return GetSlot__WeaponInfo(HeldSlotType); }
        }
        public int GetReservedBullet()
        {
            return 0;
            // return _playerEntity.modeLogic.ModeLogic.GetReservedBullet(_playerEntity, GetHoldSlot());
        }
        public int GetLastWeaponSlot()
        {
            var comp = stateExtractor(false);
            CommonUtil.WeakAssert(comp != null);
            return comp.LastWeapon;
        }

        public WeaponInfo GetSlot__WeaponInfo(EWeaponSlotType slot)
        {
            WeaponComponent weapon = slotExtractor(slot);
            return WeaponUtil.ToWeaponInfo(weapon);

        }
        public int GetSLot__WeaponId(EWeaponSlotType slot)
        {
            WeaponComponent weapon = slotExtractor(slot);
            return weapon != null ? weapon.Id : 0;
        }
        public bool IsHeldSlotType(EWeaponSlotType slot)
        {
            return HeldSlotType == slot;
        }
        public bool IsWeaponSlotStuffed(EWeaponSlotType slot)
        {
            WeaponComponent weapon = slotExtractor(slot);
            return WeaponUtil.VertifyWeaponComponentStuffed(weapon);
        }
        public bool IsWeaponStuffedInSlot(int weaponId)
        {
            NewWeaponConfigItem config;
            var ret = WeaponUtil.VertifyWeaponId(weaponId, out config);
            if (!ret) return false;
            var slot = ((EWeaponType)config.Type).ToWeaponSlot();
            var weaponComp = slotExtractor(slot);
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
                return weaponComp.Id == weaponId;
            return false;

        }
        public bool TryGetSlot__WeaponInfo(EWeaponSlotType slot, out WeaponInfo wpInfo)
        {
            wpInfo = GetSlot__WeaponInfo(slot);
            return (wpInfo.Id > 0);

        }
        public EWeaponSlotType PopGetLastWeaponId()
        {
            var last = (EWeaponSlotType)GetLastWeaponSlot();
            if (last != EWeaponSlotType.None && GetSLot__WeaponId(last) > 0)
            {
                return last;
            }
            else
            {
                for (var s = EWeaponSlotType.None + 1; s < EWeaponSlotType.Length; s++)
                {
                    if (GetSLot__WeaponId(s) > 0)
                    {
                        return s;
                    }
                }
                return EWeaponSlotType.None;
            }
        }
        public int HeldFireMode
        {
            get { return (int)GetSlotFireMode(HeldSlotType); }
            set { SetFireMode(HeldSlotType, (EFireMode)value); }
        }

        public bool HeldBolted
        {
            get { return GetSlotWeaponBolted(HeldSlotType); }
            set { SetSlotWeaponBolted(HeldSlotType, value); }
        }
        public int GetSlotFireModeCount(EWeaponSlotType slot)
        {
            var weapon = GetSlot__WeaponInfo(slot);
            if (!WeaponUtil.VertifyWeaponId(weapon.Id))
                return 1;
            return SingletonManager.Get<WeaponDataConfigManager>().GetFireModeCountById(weapon.Id);
        }
        public int HeldWeaponBullet
        {
            get { return GetSlotWeaponBullet(HeldSlotType); }
        }
        public int GetSlotWeaponBullet(EWeaponSlotType slot)
        {
            var weaponComp = slotExtractor(slot);
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
                return weaponComp.Bullet;
            return 0;
        }
        public bool GetSlotWeaponBolted(EWeaponSlotType slot)
        {
            var weapon = slotExtractor(slot);

            if (WeaponUtil.VertifyWeaponComponent(weapon) == EFuncResult.Success)
                return weapon.IsBoltPulled;
            return false;
        }
        public int GetSlotFireMode(EWeaponSlotType slot)
        {
            var weapon = slotExtractor(slot);

            if (WeaponUtil.VertifyWeaponComponent(weapon) == EFuncResult.Success)
                return weapon.FireMode;

            Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
            return (int)EFireMode.Manual;
        }

        public IPlayerWeaponController GetController()
        {
            return controller;
        }

  
        #endregion

    }
}