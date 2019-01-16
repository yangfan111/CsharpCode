using Core.Utils;
using Core.Bag;
using XmlConfig;
using WeaponConfigNs;
using Core.Configuration;
using Core.Enums;
using Utils.Configuration;
using Assets.Utils.Configuration;
using App.Shared.Util;
using Utils.Utils;
using App.Shared.Components.Bag;
using Core.WeaponLogic.Attachment;
using App.Shared.EntityFactory;
using Assets.XmlConfig;
using Utils.Appearance;
using Utils.Singleton;

namespace App.Shared.WeaponLogic
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
        /// weapon Controller
        /// </summary>
        private readonly PlayerWeaponController controller;
        public PlayerWeaponComponentAgent(
            WeaponSlotComponenExtractor in_slotExtractor, WeaponStateComponentExtractor in_stateExtractor)
        {
            
            slotExtractor = in_slotExtractor;
            stateExtractor = in_stateExtractor;
            controller = new PlayerWeaponController(this);
        }

        internal void SetHeldWeaponBullet(int count)
        {

            SetSlotWeaponBullet(HeldSlotType, count);
        }
        internal bool RemoveSlotWeapon(EWeaponSlotType slot)
        {
            if (slot == HeldSlotType)
            {
                SetHeldSlot__TypeDetail(EWeaponSlotType.None);
            }
            var comp = slotExtractor(slot);
            CommonUtil.WeakAssert(comp != null);
            comp.Clear();
            var pos = slot.ToWeaponInPackage();
            //=>TODO: _playerEntity.appearanceInterface.Appearance.UnmountWeaponInPackage(pos);
            return true;
        }

        private void RefreshHeldWeaponAttachment()
        {
            var heldSlot = HeldSlotType;
            if (!heldSlot.MayHasPart())
                return;

            WeaponInfo weapon = GetSlot__WeaponInfo(heldSlot);
            if (WeaponUtil.VertifyWeaponId(weapon.Id))
            {
                var attachments = weapon.GetParts();
                //直接影响数值，或者通过数值来体现的表现
                //=>TODO:    
                //SetCurrentWeaponParts(attachments);
                //影响表现
                //=>TODO:    
                //ApplyAttachentsEffect();
                //=>TODO: 添加到背包的时候会执行刷新模型逻辑
                //_playerEntity.RefreshWeaponModel(weapon.WeaponId, GetHoldSlot(), attachments);
            }
        }
        private void RefreshHeldWeapon()
        {
            EWeaponSlotType slot = HeldSlotType;
            //=>TODO: 清理之前的枪械状态信息
            //ClearWeaponState();
            var weapon = GetSlot__WeaponInfo(slot);
            if (!WeaponUtil.VertifyWeaponId(weapon.Id))
            {
                //TODO: _playerEntity.RefreshPlayerWeaponLogic(UniversalConsts.InvalidIntId);
                return;
            }
            //TODO:_playerEntity.RefreshPlayerWeaponLogic(config.Id);
            var weaponComp = slotExtractor(slot);
            // 重置开火模式
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
            {
                if (weaponComp.FireMode == 0)
                {
                    if (SingletonManager.Get<WeaponDataConfigManager>().HasAutoFireMode(weaponComp.Id))
                    {
                        SetFireMode(slot, EFireMode.Auto);
                    }
                    else
                    {
                        SetFireMode(slot, SingletonManager.Get<WeaponDataConfigManager>().GetFirstAvaliableFireMode(weaponComp.Id));
                    }
                }
                else
                {
                    Logger.ErrorFormat("weapon with slot {0} is null !!", slot);
                }
            }
        }
        private void ProcessMountWeaponInPackage(WeaponInPackage pos, int weaponId, int avatarId)
        {
            //=>TODO
            //if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            //{
            //    _playerEntity.AddC4ToBag(weaponId);
            //}
            //else
            //{
            //    _playerEntity.appearanceInterface.Appearance.MountWeaponInPackage(pos, avatarId);
            //}
        }


        public int GetReservedBullet(EWeaponSlotType slot)
        {
            //=>TODO:交互逻辑
            //if (slot.IsSlotWithBullet())
            //{
            //    return _playerEntity.modeLogic.ModeLogic.GetReservedBullet(_playerEntity, slot);
            //}
            //else
            //{
            //    if (Logger.IsDebugEnabled)
            //    {
            //        Logger.DebugFormat("get reserved bullet from slot {0}", slot);
            //    }
            //}
            return 0;
        }

        public void SetReservedBullet(EWeaponSlotType slot, int count)
        {
            //=>TODO:交互逻辑
            //if (slot.IsSlotWithBullet())
            //{
            //    _playerEntity.modeLogic.ModeLogic.SetReservedBullet(_playerEntity, slot, count);
            //}
            //else
            //{
            //    if (Logger.IsDebugEnabled)
            //    {
            //        Logger.DebugFormat("set reserved bullet to slot {0}", slot);
            //    }
            //}
        }

        public void SetReservedBullet(int count)
        {

            //    _playerEntity.modeLogic.ModeLogic.SetReservedBullet(_playerEntity, GetHoldSlot(), count);
        }

        public int SetReservedBullet(EBulletCaliber caliber, int count)
        {
            return 0;

            //  return _playerEntity.modeLogic.ModeLogic.SetReservedBullet(_playerEntity, caliber, count);
        }

        public int GetReservedBullet(EBulletCaliber caliber)
        {
            return 0;

            //return _playerEntity.modeLogic.ModeLogic.GetReservedBullet(_playerEntity, caliber);
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
      

        internal void SetFireMode(EWeaponSlotType slot, EFireMode mode)
        {
            var weapon = slotExtractor(slot);

            if (WeaponUtil.VertifyWeaponComponent(weapon) == EFuncResult.Success)
                weapon.FireMode = (int)mode;
            else
                Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
        }


        internal void ApplyAttachentsEffect()
        {
            RefreshBreath();
        }

        ///// <summary>
        ///// 影响呼吸晃动幅度
        ///// </summary>
        void RefreshBreath()
        {
            //=>TODO:
            //  var breath = _playerEntity.weaponLogic.Weapon.GetBreathFactor();
            //  _playerEntity.appearanceInterface.FirstPersonAppearance.SightShift.SetAttachmentFactor(breath);
        }

    
      
    }
}