using Core.Utils;
using Core;
using App.Shared.Components.Bag;
using System.Collections.Generic;
using App.Shared.Util;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;
using Utils.Singleton;
using Core.Configuration;
using Core.Enums;
using Assets.XmlConfig;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponComponentAgent : ISharedPlayerWeaponComponentGetter
    {
        #region//Gettter
        public EWeaponSlotType CurrSlotType
        {
            get
            {
                var stateCmp = stateExtractor(false);
                CommonUtil.WeakAssert(stateCmp != null);
                return WeaponUtil.GetSlotByStateComponent(stateCmp);
            }
        }
        public int CurrSlotWeaponId
        {
            get { return GetSlotWeaponId(CurrSlotType); }
        }
        public WeaponInfo CurrSlotWeaponInfo
        {
            get { return GetSlotWeaponInfo(CurrSlotType); }
        }
        public int GetReservedBullet()
        {
            return controller.GetReservedBullet();
        }
        public int GetLastWeaponSlot()
        {
            var comp = stateExtractor(false);
            CommonUtil.WeakAssert(comp != null);
            return comp.LastWeapon;
        }

        public WeaponInfo GetSlotWeaponInfo(EWeaponSlotType slot)
        {
            WeaponComponent weapon = slotExtractor(slot);
            return WeaponUtil.ToWeaponInfo(weapon);

        }
        public int GetSlotWeaponId(EWeaponSlotType slot)
        {
            WeaponComponent weapon = slotExtractor(slot);
            return weapon != null ? weapon.Id : 0;
        }
        public bool IsCurrSlotType(EWeaponSlotType slot)
        {
            return CurrSlotType == slot;
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
        public bool TryGetSlotWeaponInfo(EWeaponSlotType slot, out WeaponInfo wpInfo)
        {
            wpInfo = GetSlotWeaponInfo(slot);
            return (wpInfo.Id > 0);

        }
        public EWeaponSlotType PopGetLastWeaponId()
        {
            var last = (EWeaponSlotType)GetLastWeaponSlot();
            if (last != EWeaponSlotType.None && GetSlotWeaponId(last) > 0)
            {
                return last;
            }
            else
            {
                for (var s = EWeaponSlotType.None + 1; s < EWeaponSlotType.Length; s++)
                {
                    if (GetSlotWeaponId(s) > 0)
                    {
                        return s;
                    }
                }
                return EWeaponSlotType.None;
            }
        }
        public int CurrFireMode
        {
            get { return (int)GetSlotFireMode(CurrSlotType); }
            set { SetSlotFireMode(CurrSlotType, (EFireMode)value); }
        }

        public bool CurrBolted
        {
            get { return GetSlotWeaponBolted(CurrSlotType); }
            set { SetSlotWeaponBolted(CurrSlotType, value); }
        }
        public int GetSlotFireModeCount(EWeaponSlotType slot)
        {
            var weapon = GetSlotWeaponInfo(slot);
            if (!WeaponUtil.VertifyWeaponId(weapon.Id))
                return 1;
            return SingletonManager.Get<WeaponDataConfigManager>().GetFireModeCountById(weapon.Id);
        }
        public int CurrWeaponBullet
        {
            get { return GetSlotWeaponBullet(CurrSlotType); }
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


        /// <summary>
        /// 属于特殊处理，正常情况下不需要持有controller
        /// </summary>
        /// <param name="in_controller"></param>
        public void SetController(PlayerWeaponController in_controller)
        {
            controller = in_controller;
        }

        public bool IsWeaponCurrSlotStuffed()
        {
            return IsWeaponSlotStuffed(CurrSlotType);
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
        #endregion

    }
}