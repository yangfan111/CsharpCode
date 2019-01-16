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
using Assets.Utils.Configuration;

namespace App.Shared.WeaponLogic
{
    public partial class PlayerWeaponComponentAgent 
    {
        

        public void SetHeldSlot__Type(EWeaponSlotType slot)
        {
            var stateCmp = stateExtractor(false);
            CommonUtil.WeakAssert(stateCmp != null);
            stateCmp.CurrentWeaponSlot = (int)slot;
        }
        public void SetLastWeaponSlot(int slot)
        {
            var comp = stateExtractor(false);
            CommonUtil.WeakAssert(comp != null);
            comp.LastWeapon = slot;
        }
        public void SetHeldSlot__TypeDetail(EWeaponSlotType slot)
        {

            SetHeldSlot__Type(slot);
            //=>TODO:
            // RefreshCurrentWeaponLogic();
            // 需要执行刷新配件逻辑，因为配件会影响相机动作等属性
            //=>TODO:
            //RefreshCurrentWeaponAttachmentLogic();

            if (slot != EWeaponSlotType.None)
            {
                SetLastWeaponSlot((int)slot);
            }
        }




        /// <summary>
        /// 自动查找当前可用手雷,no vertify
        /// </summary>
        /// <param name="grenadeComp"></param>
        private void TryStuffEmptyGrenadeSlot(WeaponComponent grenadeComp)
        {
            //=>TODO:
            ////var grenadeBagAgent = _playerEntity.grenadeInventoryHolder.Inventory;
            ////int nextId = grenadeBagAgent.PickNextGrenadeInstance();
            ////if (nextId < 1) return;
            ////WeaponInfo wpInfo;
            ////Err_WeaponLogicErrCode errCode = AddWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo() { Id = nextId }, out wpInfo);
            ////if (errCode != Err_WeaponLogicErrCode.Sucess)
            ////{
            ////    throw new System.Exception("Stuff empty grenade slot failed");
            ////}
        }



        public void TryStuffGrenadeToSlot()
        {
            var cmp = slotExtractor(EWeaponSlotType.GrenadeWeapon);
            CommonUtil.WeakAssert(cmp != null);
            if (!WeaponUtil.VertifyWeaponComponentStuffed(cmp))
            {
                TryStuffEmptyGrenadeSlot(cmp);
            }

        }
        /// <summary>
        /// 手雷物品自动填充
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="autoStuffSlot"></param>
        /// <returns></returns>
        //private WeaponComponent slotExtractor(EWeaponSlotType slot, bool autoStuffSlot = false)
        //{
        //    var comp = slotExtractor(slot);
        //    if (autoStuffSlot && comp != null && slot == EWeaponSlotType.GrenadeWeapon && comp.Id < 1)
        //        TryStuffEmptyGrenadeSlot(comp);
        //    return comp;
        //}

        internal Err_WeaponLogicErrCode AddWeaponToSlot(EWeaponSlotType slot, WeaponInfo weapon)
        {
            if (slot == EWeaponSlotType.None)
                return Err_WeaponLogicErrCode.Err_SlotNone;
            WeaponComponent weaponComp = slotExtractor(slot);
            if (null == weaponComp)
                return Err_WeaponLogicErrCode.Err_SlotNone;
            var attachments = weapon.GetParts();
            weapon.CopyToWeaponComponentWithDefaultParts(weaponComp);
            weaponComp.FireMode = 0;
            var avatarId = weapon.AvatarId;
            if (avatarId < 1)
                avatarId = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponComp.Id).AvatorId;
            ProcessMountWeaponInPackage(slot.ToWeaponInPackage(), weapon.Id, avatarId);
            RefreshWeaponPartsModel(weapon.Id, slot, new WeaponPartsStruct(), attachments);
            if (slot == HeldSlotType)
            {
                RefreshHeldWeapon();
            }
            return Err_WeaponLogicErrCode.Sucess;
        }

    }
}