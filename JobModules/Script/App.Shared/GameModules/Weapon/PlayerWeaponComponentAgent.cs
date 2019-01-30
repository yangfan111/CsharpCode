using Core.Utils;
using XmlConfig;
using WeaponConfigNs;
using Core.Configuration;
using Core.Enums;
using App.Shared.Util;
using Utils.Appearance;
using Utils.Singleton;
using System;
using Core;
using Assets.Utils.Configuration;
using App.Shared.Components.Bag;
using Core.WeaponLogic.Attachment;

namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponComponentAgent 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponComponentAgent));
        /// <summary>
        /// WeaponSlotComponent
        /// </summary>
        private readonly WeaponSlotComponenExtractor slotExtractor;
        /// <summary>
        /// WeaponStateComponent
        /// </summary>
        private readonly WeaponStateComponentExtractor stateExtractor;
        /// <summary>
        /// weaponController，正常状态下不应对Controlller持有
        /// </summary>
        private  PlayerWeaponController controller;
        public PlayerWeaponComponentAgent(
            WeaponSlotComponenExtractor in_slotExtractor, WeaponStateComponentExtractor in_stateExtractor)
        {
            
            slotExtractor = in_slotExtractor;
            stateExtractor = in_stateExtractor;
          //  controller = new PlayerWeaponController(this);
        }
    
        internal bool RemoveSlotWeapon(EWeaponSlotType slot,System.Action onSetProcess)
        {
            if (slot == CurrSlotType)
            {
                SetCurrSlotTypeProcess(EWeaponSlotType.None, onSetProcess);
            }
            var comp = slotExtractor(slot);
            CommonUtil.WeakAssert(comp != null);
            comp.Clear();
            WeaponInPackage pos = slot.ToWeaponInPackage();
            return true;
        }

      

     
        internal void SetSlotWeaponBullet(int bullet)
        {
            SetSlotWeaponBullet(CurrSlotType,bullet);
        }
        internal void SetSlotWeaponBullet(EWeaponSlotType slot, int bullet)
        {
            var weaponComp = slotExtractor(slot);
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
                weaponComp.Bullet = bullet;

        }
        /// <summary>
        /// 重置weaponComponent数据
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="weaponId"></param>
        internal void ResetSlotWeaponComponent(EWeaponSlotType slot, int weaponId)
        {
            var weaponComp = slotExtractor(slot);

            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
            {
                weaponComp.Clear();
                weaponComp.Id = weaponId;
            }


        }
        internal void ClearSlotWeaponAttachments(EWeaponSlotType slot)
        {
            var weaponComp = slotExtractor(slot);

            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
                weaponComp.ResetAttachments();

        }

        internal void SetSlotWeaponBolted(EWeaponSlotType slot, bool bolted)
        {
            var weapon = slotExtractor(slot);
            if (WeaponUtil.VertifyWeaponComponent(weapon) == EFuncResult.Success)
                weapon.IsBoltPulled = bolted;

        }
      

        internal void SetSlotFireMode(EWeaponSlotType slot, EFireMode mode)
        {
            var weapon = slotExtractor(slot);

            if (WeaponUtil.VertifyWeaponComponent(weapon) == EFuncResult.Success)
                weapon.FireMode = (int)mode;
            else
                Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
        }
        internal void SetCurrSlotType(EWeaponSlotType slot)
        {
            var stateCmp = stateExtractor(false);
            CommonUtil.WeakAssert(stateCmp != null);
            stateCmp.CurrentWeaponSlot = (int)slot;
            
        }
        internal void SetLastWeaponSlot(int slot)
        {
            var comp = stateExtractor(false);
            CommonUtil.WeakAssert(comp != null);
            comp.LastWeapon = slot;
        }
        internal void SetCurrSlotTypeProcess(EWeaponSlotType slot, System.Action onSetProcess)
        {
            SetCurrSlotType(slot);
            onSetProcess();
          
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
            //var cmp = slotExtractor(EWeaponSlotType.GrenadeWeapon);
            //CommonUtil.WeakAssert(cmp != null);
            //if (!WeaponUtil.VertifyWeaponComponentStuffed(cmp))
            //{
            //    TryStuffEmptyGrenadeSlot(cmp);
            //}

        }
        /// <summary>
        /// 手雷物品自动填充
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="autoStuffSlot"></param>
        /// <returns></returns>
        private WeaponComponent TryStuffEmptyGrenadeSlot(EWeaponSlotType slot, bool autoStuffSlot = false)
        {
            return null; 
            //var comp = slotExtractor(slot);
            //if (autoStuffSlot && comp != null && slot == EWeaponSlotType.GrenadeWeapon && comp.Id < 1)
            //    TryStuffEmptyGrenadeSlot(comp);
            //return comp;
        }

        internal Err_WeaponLogicErrCode AddWeaponToSlot(EWeaponSlotType slot, WeaponInfo weapon, WeaponPartsModelRefresh onModelWeaponPartsRefresh)
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
            onModelWeaponPartsRefresh(weapon, slot, new WeaponPartsStruct(), attachments, true);
            return Err_WeaponLogicErrCode.Sucess;
        }
        /// <summary>
        /// 重置开火模式
        /// </summary>
        /// <param name="slot"></param>
        internal void ResetSlotFireModel(EWeaponSlotType slot)
        {
            var weaponComp = slotExtractor(slot);
            // 重置开火模式
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
            {
                if (weaponComp.FireMode == 0)
                {
                    bool hasAutoFireModel = SingletonManager.Get<WeaponDataConfigManager>().HasAutoFireMode(weaponComp.Id);
                    if (hasAutoFireModel)
                        SetSlotFireMode(slot, EFireMode.Auto);
                    else
                        SetSlotFireMode(slot, SingletonManager.Get<WeaponDataConfigManager>().GetFirstAvaliableFireMode(weaponComp.Id));
                }
            }
        }




    }
}