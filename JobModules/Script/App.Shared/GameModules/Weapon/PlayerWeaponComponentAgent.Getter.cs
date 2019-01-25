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
using App.Shared.WeaponLogic;

namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponComponentAgent : ISharedPlayerWeaponComponentGetter
    {
        #region//Gettter
        public EWeaponSlotType CurrSlotType
        {
            get
            {
                return (EWeaponSlotType)_playerEntity.bagState.CurSlot;
                //var stateCmp = stateExtractor(false);
                //CommonUtil.WeakAssert(stateCmp != null);
                //return WeaponUtil.GetSlotByStateComponent(stateCmp);
            }
        }
        public int? CurrSlotWeaponId(Contexts contexts)
        {
            return GetSlotWeaponId(contexts, CurrSlotType);
        }
        public WeaponInfo CurrSlotWeaponInfo(Contexts contexts)
        {
            return GetSlotWeaponInfo(contexts, CurrSlotType);
        }
        public int GetReservedBullet()
        {
            return controller.GetReservedBullet();
        }
        public int GetLastWeaponSlot()
        {
            var comp = stateExtractor();
            CommonUtil.WeakAssert(comp != null);
            return comp.LastSlot;
        }

        public WeaponInfo GetSlotWeaponInfo(Contexts contexts, EWeaponSlotType slot)
        {
            WeaponEntity weaponEntity;
            if(_playerEntity.TryGetWeapon(contexts, slot, out weaponEntity))
            {
                return weaponEntity.ToWeaponInfo();
            }
            return new WeaponInfo();
        }

        public int? GetSlotWeaponId(Contexts contexts, EWeaponSlotType slot)
        {
            var weapon = slotExtractor(contexts, slot);
            if(weapon == null)
            {
                return null;
            }
            return weapon.WeaponId;
        }

        public bool IsCurrSlotType(EWeaponSlotType slot)
        {
            return CurrSlotType == slot;
        }

        public bool IsWeaponSlotStuffed(Contexts contexts, EWeaponSlotType slot)
        {
            return _playerEntity.HasWeaponInSlot(contexts, slot);
        }

        public bool IsWeaponStuffedInSlot(Contexts contexts, int weaponId)
        {
            NewWeaponConfigItem config;
            var ret = WeaponUtil.VertifyWeaponId(weaponId, out config);
            if (!ret) return false;
            var slot = ((EWeaponType)config.Type).ToWeaponSlot();
            var weaponComp = slotExtractor(contexts, slot);
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
                return weaponComp.WeaponId == weaponId;
            return false;

        }
        public bool TryGetSlotWeaponInfo(Contexts contexts, EWeaponSlotType slot, out WeaponInfo wpInfo)
        {
            wpInfo = GetSlotWeaponInfo(contexts, slot);
            return (wpInfo.Id > 0);
        }
        public EWeaponSlotType PopGetLastWeaponId(Contexts contexts)
        {
            var last = (EWeaponSlotType)GetLastWeaponSlot();
            if (last != EWeaponSlotType.None && GetSlotWeaponId(contexts, last) > 0)
            {
                return last;
            }
            else
            {
                for (var s = EWeaponSlotType.None + 1; s < EWeaponSlotType.Length; s++)
                {
                    if (GetSlotWeaponId(contexts, s) > 0)
                    {
                        return s;
                    }
                }
                return EWeaponSlotType.None;
            }
        }

        public int GetSlotFireModeCount(Contexts contexts, EWeaponSlotType slot)
        {
            var weapon = GetSlotWeaponInfo(contexts, slot);
            if (!WeaponUtil.VertifyWeaponId(weapon.Id))
                return 1;
            return SingletonManager.Get<WeaponDataConfigManager>().GetFireModeCountById(weapon.Id);
        }

        public int CurrWeaponBullet(Contexts contexts)
        {
            return GetSlotWeaponBullet(contexts, CurrSlotType);
        }
        public int GetSlotWeaponBullet(Contexts contexts, EWeaponSlotType slot)
        {
            var weaponData = _playerEntity.GetWeaponData(contexts, slot);
            return null == weaponData ? 0 : weaponData.Bullet;
        }

        public bool GetSlotWeaponBolted(Contexts contexts, EWeaponSlotType slot)
        {
            var weaponData = _playerEntity.GetWeaponData(contexts, slot);
            return null == weaponData ? false : weaponData.PullBolt;
        }
        public int GetSlotFireMode(Contexts contexts, EWeaponSlotType slot)
        {
            var weaponData = _playerEntity.GetWeaponData(contexts, slot);
            return null == weaponData ? (int)EFireMode.Manual : weaponData.FireMode;
        }

        /// <summary>
        /// 属于特殊处理，正常情况下不需要持有controller
        /// </summary>
        /// <param name="in_controller"></param>
        public void SetController(PlayerWeaponController in_controller)
        {
            controller = in_controller;
        }

        public bool IsWeaponCurrSlotStuffed(Contexts contexts)
        {
            return IsWeaponSlotStuffed(contexts, CurrSlotType);
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