using Core.Utils;
using Core.Bag;

using App.Shared.Components.Bag;
using System.Collections.Generic;
using App.Shared.Util;
using App.Shared.Util;
using Core.WeaponLogic.Attachment;

namespace App.Shared.WeaponLogic
{
    public partial class WeaponBagLogic : IWeaponBagLogic
    {
        /// <summary>
        /// 获取武器槽位component
        /// 手雷数据：当前槽位为空的情况下自动查找当前可用手雷
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public WeaponInfo GetWeaponInfo(EWeaponSlotType slot)
        {
            WeaponComponent weapon = GetWeaponComponentBySlot(slot);
            if (weapon != null && weapon.Id > 0)
            {
                return weapon.ToWeaponInfo();
            }
            return WeaponInfo.Empty;
        }
        public bool IsWeaponSlotStuffed(EWeaponSlotType slot)
        {
            WeaponComponent weapon = GetWeaponComponentBySlot(slot);
            return weapon != null && weapon.Id > 0;
        }
        public bool TryGetWeaponSlotInfo(EWeaponSlotType slot, out WeaponInfo wpInfo)
        {
            WeaponComponent weapon = GetWeaponComponentBySlot(slot);
            if (weapon != null && weapon.Id > 0)
            {
                wpInfo = weapon.ToWeaponInfo();
                return true;
            }
            wpInfo = WeaponInfo.Empty;
            return false;
        }
        /// <summary>
        /// 自动查找当前可用手雷,no vertify
        /// </summary>
        /// <param name="grenadeComp"></param>
        private void TryStuffEmptyGrenadeSlot(WeaponComponent grenadeComp)
        {
            var grenadeBagAgent = _playerEntity.grenadeInventoryHolder.Inventory;
            int nextId = grenadeBagAgent.PickNextGrenadeInstance();
            if (nextId < 1) return;
            WeaponInfo wpInfo;
            Err_WeaponLogicErrCode errCode = AddWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo() { Id = nextId }, out wpInfo);
            if (errCode != Err_WeaponLogicErrCode.Sucess)
            {
                throw new System.Exception("Stuff empty grenade slot failed");
            }
        }



        public void TryStuffGrenadeToSlot()
        {
            var cmp = _playerEntity.GetWeaponComponentBySlot(EWeaponSlotType.GrenadeWeapon);
            if (cmp != null && cmp.Id < 1)
                TryStuffEmptyGrenadeSlot(cmp);
        }
        /// <summary>
        /// 手雷物品自动填充
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="autoStuffSlot"></param>
        /// <returns></returns>
        private WeaponComponent GetWeaponComponentBySlot(EWeaponSlotType slot, bool autoStuffSlot = false)
        {
            var comp = _playerEntity.GetWeaponComponentBySlot(slot);
            if (autoStuffSlot && comp != null && slot == EWeaponSlotType.GrenadeWeapon && comp.Id < 1)
                TryStuffEmptyGrenadeSlot(comp);
            return comp;
        }
   
        public Err_WeaponLogicErrCode AddWeaponToSlot(EWeaponSlotType slot, WeaponInfo weapon, out WeaponInfo lastWeapon)
        {

            lastWeapon = WeaponInfo.Empty;
            if (slot == EWeaponSlotType.None)
            {
                Logger.Error("slot should not be none");
                return Err_WeaponLogicErrCode.Err_SlotNone;
            }
            if (!_playerEntity.modeLogic.ModeLogic.IsSlotValid(slot))
            {
                Logger.ErrorFormat("slot {0} is not valid in this mode ", slot);
                return Err_WeaponLogicErrCode.Err_SlotInvailed;
            }

            var weaponCfg = _newWeaponConfigManager.GetConfigById(weapon.Id);
            if (null == weaponCfg)
            {
                return Err_WeaponLogicErrCode.Err_IdNotExisted;
            }
            var weaponComp = GetWeaponComponentBySlot(slot);
            if (null == weaponComp)
            {
                Logger.ErrorFormat("set weapon failed : weaponcompent in slot {0} is null", slot);
                return Err_WeaponLogicErrCode.Err_SlotNone;

            }
            lastWeapon = GetWeaponInfo(slot);
            var attachments = weapon.GetParts();
            weapon.CopyToWeaponComponentWithDefaultParts(weaponComp);
            weaponComp.FireMode = 0;
            var avatarId = weapon.AvatarId;
            if (avatarId < 1)
            {
                avatarId = weaponCfg.AvatorId;
            }
            ProcessMountWeaponInPackage(slot.ToWeaponInPackage(), weapon.Id, avatarId);
            RefreshWeaponPartsModel(weapon.Id, slot, new WeaponPartsStruct(), attachments);
            if (slot == GetCurrentWeaponSlot())
            {
                RefreshCurrentWeaponLogic();
            }
            return Err_WeaponLogicErrCode.Sucess;
        }

    }
}