﻿using App.Shared.Components.Player;
using App.Shared.GameMode;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Common;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponController : IPlayerWeaponProcessor
    {
        public event WeaponProcessEvent onWeaponDropEvt;

        public event WeaponProcessEvent onWeaponPickEvt;

        public event WeaponProcessEvent onWeaponExpendEvt;

        public event WeaponSwitchEvent onWeaponSwitchEvt;

        public event WeaponProcessEvent onWeaponSlotCleanupEvt;

        public void SetProcessListener(IModeProcessListener processListener)
        {
            onWeaponDropEvt   += processListener.OnDrop;
            onWeaponPickEvt   += processListener.OnWeaponPickup;
            onWeaponExpendEvt += processListener.OnExpend;
            onWeaponSwitchEvt += processListener.OnSwitch;
        }

        #region arrangement

        public void TryArmWeaponImmediately(EWeaponSlotType slotType)
        {
            if (!IsHeldSlotEmpty)
                return;
            ArmWeapon(slotType, false);
        }


        public void ArmWeapon(EWeaponSlotType slot, bool includeAction)
        {
            slot = processHelper.GetRealSlotType(slot);
            if (!processHelper.FilterSameSpecies(slot) || !processHelper.FilterSlotValied(slot))
                return;
            var nextAgent = GetWeaponAgent(slot);
            //把当前的一些枪械状态重置掉
            HeldWeaponAgent.StoreDynamic();
            var lastWeapon = HeldWeaponAgent.ComponentScan;
            var destWeapon = GetWeaponAgent(slot).ComponentScan;
            var appearanceStruct = processHelper.GetDrawAppearanceStruct(slot);
            DoSpecialAppearance();
            DoDrawInterrupt();
            if (includeAction)
            {
                if (IsHeldSlotEmpty)
                {
                    DoDrawArmWeaponWithActionFromEmpty(destWeapon, lastWeapon.ConfigId,
                        appearanceStruct);
                }
                else
                {
                    DoDrawArmWeaponWithActionFromOtherWeapon(destWeapon, lastWeapon.ConfigId,
                        appearanceStruct);
                }
            }
            else
            {
                //CharacterState控制动作相关
                DoDrawArmWeaponWithoutAction(destWeapon, lastWeapon.ConfigId,
                    processHelper.GetDrawAppearanceStruct(slot));
            }

            onWeaponSwitchEvt(this, HeldConfigId, EInOrOff.In);
        }

        public EWeaponSlotType UnArmWeapon(bool includeAction)
        {
            return UnArmWeapon(includeAction, null);
        }

        public EWeaponSlotType UnArmWeapon(bool includeAction, Action callback)
        {
            if (IsHeldSlotEmpty)
                return EWeaponSlotType.None;
            HeldWeaponAgent.StoreDynamic();
            var holdSlot = HeldSlotType;
            if (includeAction)
            {
                DoDrawUnarmWeaponWithAction(callback);
            }
            else
            {
                DoDrawUnarmWeaponWithoutAction(null, callback);
            }

            return holdSlot;
        }


        public bool DropWeapon(EWeaponSlotType slotType = EWeaponSlotType.Pointer)
        {
            if (slotType == EWeaponSlotType.Pointer) slotType = HeldSlotType;
            return DropWeapon(slotType, -1);
        }

        public bool DropWeapon(EWeaponSlotType slotType, int bagIndex)
        {
            if (!processHelper.FilterDrop(slotType)) return false;
            var ret = DestroyWeapon(slotType, bagIndex, true);
            onWeaponDropEvt(this, slotType);
            return ret;
        }

        public bool DestroyWeapon(EWeaponSlotType slotType, int bagIndex, bool interrupt = true)
        {
            var weaponAgent = processHelper.FilterNoneAgent(slotType);
            if (weaponAgent == null)
                return false;
            if (WeaponUtil.IsC4p(weaponAgent.ConfigId))
                RelatedAppearence.RemoveC4();
            weaponAgent.ReleaseWeapon();
            //移除武器背包操作
            RemoveBagWeapon(slotType);
            if (IsHeldBagSlotType(slotType))
            {
                SetHeldSlotType(EWeaponSlotType.None);
            }

            DoDrawEmptyWeaponInPackage(slotType, interrupt);
            return true;
        }

        public bool AutoPickUpWeapon(WeaponScanStruct orient)
        {
            var slotType = processHelper.GetMatchedSlotType(orient.ConfigId);
            if (!processHelper.FilterVailed(orient, slotType)) return false;
            if (!processHelper.FilterAutoPickup(slotType)) return false;
            if (ReplaceWeaponToSlot(slotType, orient))
            {
                TryArmWeaponImmediately(slotType);
                onWeaponPickEvt(this, slotType);
                return true;
            }

            return false;
        }

        public bool PickUpWeapon(WeaponScanStruct orient, bool arm = true)
        {
            bool pickupSuccess = false;
            return PickUpWeapon(orient, ref pickupSuccess, arm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="pickupSuccess"></param>
        /// <param name="arm"></param>
        /// <returns>generate scene object</returns>
        public bool PickUpWeapon(WeaponScanStruct orient, ref bool pickupSuccess, bool arm = true)
        {
            var slotType = processHelper.GetMatchedSlotType(orient.ConfigId);
            if (!processHelper.FilterVailed(orient, slotType)) return false;
            if (!processHelper.FilterPickup(slotType)) return false;
            if (ReplaceWeaponToSlot(slotType, orient))
            {
                if (arm)
                    TryArmWeaponImmediately(slotType);
                onWeaponPickEvt(this, slotType);
                pickupSuccess = true;
                return slotType != EWeaponSlotType.ThrowingWeapon;
            }

            return false;
        }

        //有动作版移除
        private void DoDrawUnarmWeaponWithAction(Action callback)
        {
            DoSpecialAppearance();
            int   heldConfigId = HeldConfigId;
            float holsterParam = WeaponUtil.GetUnArmHolsterParam(HeldSlotType);
            RelatedCharState.CharacterUnmount(() => RelatedAppearence.JustUnMountWeaponFromHand(),
                () => DoUnArmFinished(heldConfigId, callback),
                holsterParam);
            onWeaponSwitchEvt(this, heldConfigId, EInOrOff.Off);
        }

        private void DoUnArmFinished(int? weaponId, Action callback)
        {
            RelatedAppearence.JustClearOverrideController();
            SetHeldSlotType(EWeaponSlotType.None);
            if (weaponId.HasValue && WeaponUtil.IsC4p(weaponId.Value))
            {
                UnArmC4();
            }

            ThrowActionExecute();
            if (callback != null)
                callback();
        }

        //直接放回装备
        private void DoDrawUnarmWeaponWithoutAction(int? weaponId, Action callback)
        {
            RelatedAppearence.UnmountWeaponFromHand();
            SetHeldSlotType(EWeaponSlotType.None);
            if (weaponId.HasValue && WeaponUtil.IsC4p(weaponId.Value))
            {
                UnArmC4();
            }

            ThrowActionExecute();
            if (callback != null)
                callback();
        }

        private void DoDrawArmWeaponWithActionFromEmpty(WeaponScanStruct           destWeapon, int lastWeaponId,
                                                        WeaponDrawAppearanceStruct appearanceStruct)
        {
            WeaponToHand(destWeapon.ConfigId, lastWeaponId, appearanceStruct.targetSlot,
                appearanceStruct.armOnLeft);
            DoArmFinished(destWeapon, appearanceStruct.targetSlot);
            RelatedCharState.Select(RelatedAppearence.RemountP3WeaponOnRightHand, appearanceStruct.drawParam);
        }

        /// 有动作，从旧装备上画新装备
        private void DoDrawArmWeaponWithActionFromOtherWeapon(WeaponScanStruct           destWeapon, int lastWeaponId,
                                                              WeaponDrawAppearanceStruct appearanceStruct)
        {
            RelatedCharState.SwitchWeapon(
                () =>
                {
                    WeaponToHand(destWeapon.ConfigId, lastWeaponId, appearanceStruct.targetSlot,
                        appearanceStruct.armOnLeft);
                    onWeaponSwitchEvt(this, destWeapon.ConfigId, EInOrOff.Off);
                },
                () => OnDrawWeaponCallback(destWeapon, appearanceStruct.targetSlot), appearanceStruct.switchParam);
        }

        /// 没动作，直接刷新装备
        private void DoDrawArmWeaponWithoutAction(WeaponScanStruct           destWeapon, int lastWeaponId,
                                                  WeaponDrawAppearanceStruct appearanceStruct)
        {
            WeaponToHand(destWeapon.ConfigId, lastWeaponId, appearanceStruct.targetSlot,
                appearanceStruct.armOnLeft);
            OnDrawWeaponCallback(destWeapon, appearanceStruct.targetSlot);
            DoArmFinished(destWeapon, appearanceStruct.targetSlot);
            RelatedAppearence.RemountP3WeaponOnRightHand();
        }

        private void DoDrawEmptyWeaponInPackage(EWeaponSlotType slotType, bool interrupt)
        {
            WeaponInPackage pos = slotType.ToWeaponInPackage();
            RelatedAppearence.UnmountWeaponInPackage(pos);
            if (interrupt)
                InteractController.InterruptCharactor();
        }

        public void SwitchFireMode()
        {
            if (IsHeldSlotEmpty)
                return;
            var config = HeldWeaponAgent.DefaultFireModeLogicCfg;
            if (config == null || config.AvaliableModes == null)
                return;
            EFireMode mode     = (EFireMode) HeldWeaponAgent.BaseComponent.RealFireModel;
            EFireMode nextMode = config.AvaliableModes[0];
            for (int i = 0; i < config.AvaliableModes.Length; i++)
            {
                if (config.AvaliableModes[i] == mode)
                {
                    nextMode = config.AvaliableModes[(i + 1) % config.AvaliableModes.Length];
                }
            }

            if (nextMode == mode)
            {
                ShowTip(ETipType.FireModeLocked);
            }
            else
            {
                ShowFireModeChangeTip(nextMode);
                if (AudioController != null)
                    AudioController.PlaySimpleAudio(EAudioUniqueId.FireMode);
            }

            HeldWeaponAgent.BaseComponent.FireModel = (int) nextMode;
        }

        private void ShowFireModeChangeTip(EFireMode newFireMode)
        {
            switch (newFireMode)
            {
                case EFireMode.Auto:
                    ShowTip(ETipType.FireModeToAuto);
                    break;
                case EFireMode.Burst:
                    ShowTip(ETipType.FireModeToBurst);
                    break;
                case EFireMode.Manual:
                    ShowTip(ETipType.FireModeToManual);
                    break;
            }
        }

        public void SwitchIn(EWeaponSlotType in_slot)
        {
            if (!processHelper.FilterSwitchIn(in_slot))
            {
                ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }

            if (IsHeldBagSlotType(in_slot))
            {
                SameSpeciesSwitchIn(in_slot);
            }
            else
            {
                ArmWeapon(in_slot, true);
            }
        }

        public void PureSwitchIn(EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
                return;
            EWeaponSlotType from_slot = HeldSlotType;

            //int from_Id= componentAgent.GetSlotWeaponId(from_slot);

            if (IsWeaponSlotEmpty(in_slot))
            {
                ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }

            if (!IsHeldBagSlotType(in_slot))
            {
                ArmWeapon(in_slot, false);
            }
        }

        public void AfterAttack()
        {
            if (HeldSlotType == EWeaponSlotType.None || IsHeldSlotEmpty)
                return;

            //var handler = slotsAux.FindHandler(slot);
            int  reservedBullet     = GetReservedBullet();
            HeldWeaponAgent.ExpendWeapon(reservedBullet);
            onWeaponExpendEvt(this, HeldSlotType);
        }
        private PlayerWeaponBagData FindWeaponBagDataBySlot(int index)
        {
            int length = ModeController.GetUsableWeapnBagLength(RelatedPlayerInfo);
            if (index == HeldBagPointer) return null;
            List<PlayerWeaponBagData> bagDatas = ModeController.FilterSortedWeaponBagDatas(RelatedPlayerInfo);
            if (bagDatas == null || bagDatas.Count <= index) return null;
            PlayerWeaponBagData tarBag = bagDatas.Find(bag => bag.BagIndex == index);
            if (tarBag == null) return null;
            HeldBagPointer = index;
            return tarBag;
        }

        public void SwitchBag(int index)
        {
            if (SwitchBagByReservedType(index))
                return;

            var tarBag = FindWeaponBagDataBySlot(index);
            if (tarBag == null)
                return;
            var removedSlotList = new Byte[(int) EWeaponSlotType.Length];
            DestroyWeapon(EWeaponSlotType.ThrowingWeapon, 0);
            RefreshWeaponAppearance(EWeaponSlotType.ThrowingWeapon);
            GrenadeHandler.ClearCache();
            foreach (var weapon in tarBag.weaponList)
            {
                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
                if (slot != EWeaponSlotType.ThrowingWeapon && slot != EWeaponSlotType.TacticWeapon)
                {
                    var orient = WeaponUtil.CreateScan(weapon);
                    orient.Bullet = weaponAllConfig.PropertyCfg.Bullet;
                    orient.ReservedBullet = weaponAllConfig.PropertyCfg.Bulletmax;
                    if (orient.Magazine > 0)
                    {
                        orient.Bullet += SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(orient.Magazine).Bullet;
                    }
                    ReplaceWeaponToSlot(slot, 0, orient);
                }
                else if (slot == EWeaponSlotType.ThrowingWeapon)
                {
                    GrenadeHandler.AddCache(weapon.WeaponTplId);
                }
                removedSlotList[(int) slot] = 1;
            }

            removedSlotList[(int) EWeaponSlotType.TacticWeapon] = 1;
            removedSlotList[(int) EWeaponSlotType.ThrowingWeapon] = 1;
            for (int i = 1; i < removedSlotList.Length; i++)
            {
                if (removedSlotList[i] == 0)
                    DestroyWeapon((EWeaponSlotType) i, 0);
            }

            EWeaponSlotType newSlot = PollGetLastSlotType();
            TryHoldGrenade(true, false);
            TryArmWeaponImmediately(newSlot);
        }

        public bool SwitchBagByReservedType(int index)
        {
            if (null == RelatedServerUpdate.ReservedWeaponSubType || RelatedServerUpdate.ReservedWeaponSubType.Count == 0)
                return false;

            var tarBag = FindWeaponBagDataBySlot(index);
            if (tarBag == null) return true;
            DestroyWeapon(EWeaponSlotType.ThrowingWeapon, 0);
            RefreshWeaponAppearance(EWeaponSlotType.ThrowingWeapon);
            GrenadeHandler.ClearCache();
            var removedSlotList = new Byte[(int) EWeaponSlotType.Length];
            foreach (var weapon in tarBag.weaponList)
            {
                bool needReserved = false;
                var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
                if (weaponAllConfig.NewWeaponCfg.Type == (int) EWeaponType_Config.ThrowWeapon)
                    needReserved = RelatedServerUpdate.ReservedWeaponSubType.Contains((int) EWeaponSubType.Throw);
                else
                    needReserved = RelatedServerUpdate.ReservedWeaponSubType.Contains(weaponAllConfig.NewWeaponCfg.SubType);

                if (!needReserved)
                    continue;

                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                if (slot != EWeaponSlotType.ThrowingWeapon && slot != EWeaponSlotType.TacticWeapon)
                {
                    var orient = WeaponUtil.CreateScan(weapon);
                    orient.Bullet = weaponAllConfig.PropertyCfg.Bullet;
                    orient.ReservedBullet = weaponAllConfig.PropertyCfg.Bulletmax;
                    if (orient.Magazine > 0)
                    {
                        orient.Bullet += SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(orient.Magazine).Bullet;
                    }

                    ReplaceWeaponToSlot(slot, 0, orient);
                }
                else if (slot == EWeaponSlotType.ThrowingWeapon)
                {
                    GrenadeHandler.AddCache(weapon.WeaponTplId);
                }
                removedSlotList[(int) slot] = 1;
            }

            for (int i = 1; i < removedSlotList.Length; i++)
            {
                if (removedSlotList[i] == 0)
                    DestroyWeapon((EWeaponSlotType) i, 0);
            }
            EWeaponSlotType newSlot = PollGetLastSlotType();
            TryHoldGrenade(true, false);
            TryArmWeaponImmediately(newSlot);
            return true;
        }

        [System.Obsolete]
        public void SwitchBag()
        {
            if (CanSwitchWeaponBag)
            {
                int length = ModeController.GetUsableWeapnBagLength(RelatedPlayerInfo);
                SwitchBag((HeldBagPointer + 1) % length);
            }
        }

        public void InitBag(int pointer)
        {
            ClearBagPointer();
            HeldBagPointer = pointer;
        }

        #endregion

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient)
        {
            if (TryHoldGrenade(orient.ConfigId))
                return true;
            return ReplaceCommonWeapon(slotType, orient, 0);
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, int bagIndex, WeaponScanStruct orient)
        {
            if (TryHoldGrenade(orient.ConfigId))
                return true;
            return ReplaceCommonWeapon(slotType, orient, bagIndex);
        }

        private bool ReplaceCommonWeapon(EWeaponSlotType slotType, WeaponScanStruct orient, int bagIndex)
        {
            //  if (vertify)
            if (!processHelper.FilterVailed(orient, slotType)) return false;
            bool refreshAppearance = true;
            logger.InfoFormat("replace weapon:{0}: ", orient);
            //特殊全局性武器只取武器背包第0个索引值
            var                      weaponAgent   = GetWeaponAgent(slotType);
            WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
            WeaponEntity             newEntity     = weaponAgent.ReplaceWeapon(Owner, orient, ref refreshParams);
            if (newEntity == null) return false;
            SyncBagWeapon(slotType, newEntity.entityKey.Value);
            if (refreshAppearance)
                RefreshModelWeaponParts(refreshParams);
            return true;
        }

        private void RefreshWeaponAppearance(EWeaponSlotType slot = EWeaponSlotType.Pointer)
        {
            var weaponAgent = GetWeaponAgent(slot);
            if (weaponAgent.IsValid())
            {
                WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
                refreshParams.slot       = slot;
                refreshParams.weaponInfo = weaponAgent.ComponentScan;
                // refreshParams.oldParts = new WeaponPartsStruct();
                refreshParams.newParts     = weaponAgent.PartsScan;
                refreshParams.armInPackage = true;
                RefreshModelWeaponParts(refreshParams);
            }
            else
            {
                //不移除C4 RelatedAppearence.RemoveC4();
                WeaponInPackage pos = slot.ToWeaponInPackage();
                RelatedAppearence.UnmountWeaponInPackage(pos);
                InteractController.InterruptCharactor();
            }
        }

        public bool TryHoldGrenade(int greandeId)
        {
            if (_grenadeHandler.AddCache(greandeId))
            {
                TryHoldGrenade();
                return true;
            }

            return false;
        }

        public void TryHoldGrenade(bool autoStuff = true, bool tryArm = true)
        {
            var weaponAgent = this[EWeaponSlotType.ThrowingWeapon];
            if (autoStuff && weaponAgent.IsValid()) return;
            int nextId = _grenadeHandler.FindUsable(autoStuff);
            if (nextId < 0 || weaponAgent.ConfigId == nextId) return;
            WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
            WeaponEntity newEntity =
                weaponAgent.ReplaceWeapon(Owner, WeaponUtil.CreateScan(nextId), ref refreshParams);
            SyncBagWeapon(EWeaponSlotType.ThrowingWeapon, newEntity.entityKey.Value);
            RefreshModelWeaponParts(refreshParams);
            if (tryArm)
                TryArmWeaponImmediately(EWeaponSlotType.ThrowingWeapon);
        }

        public void RemoveGreande(int id)
        {
            int leftCount = _grenadeHandler.RemoveCache(id);
            //其他行为通过回调来做
            if (leftCount < 0) return;
            var weaponAgent = this[EWeaponSlotType.ThrowingWeapon];
            if (weaponAgent.ConfigId == id)
            {
                DestroyWeapon(EWeaponSlotType.ThrowingWeapon, 0);
                TryHoldGrenade();
            }

            RefreshWeaponAppearance();
        }

        public void Interrupt()
        {
        }

        public void SetReservedBullet(int count)
        {
            var currSlot = HeldSlotType;
            if (currSlot.IsSlotWithBullet())
                ModeController.SetReservedBullet(this, HeldSlotType, count);
        }

        public void SetReservedBullet(EWeaponSlotType slot, int count)
        {
            if (slot.IsSlotWithBullet())
                ModeController.SetReservedBullet(this, slot, count);
        }

        public int SetReservedBullet(EBulletCaliber caliber, int count)
        {
            return ModeController.SetReservedBullet(this, caliber, count);
        }

        private void SameSpeciesSwitchIn(EWeaponSlotType slot)
        {
            if (!processHelper.FilterSameSepeciesSwitchIn(slot)) return;
            int nextId = GetWeaponAgent(slot).FindNextWeapon(false);
            if (nextId > 0 && slot == EWeaponSlotType.ThrowingWeapon)
            {
                TryHoldGrenade(false);
            }
        }


        private void SetHeldSlotType(EWeaponSlotType slotType)
        {
            SetBagHeldSlotType(slotType);
            RefreshHeldWeaponDetail();
        }

        private void OnDrawWeaponCallback(WeaponScanStruct weapon, EWeaponSlotType slot)
        {
            DoArmFinished(weapon, slot);
            RelatedAppearence.RemountP3WeaponOnRightHand();
        }

        private void OnUnArmWeaponCallback(int weaponId, Action onfinish)
        {
            RelatedAppearence.UnmountWeaponFromHand();
            SetHeldSlotType(EWeaponSlotType.None);
            if (WeaponUtil.IsC4p(weaponId))
            {
                UnArmC4();
            }

            if (onfinish != null)
                onfinish();
        }

        private void RefreshHeldWeaponAttachment()
        {
            if (!processHelper.FilterRefreshWeapon()) return;
            var baseComp    = HeldWeaponAgent.ComponentScan;
            var attachments = WeaponPartUtil.CreateParts(baseComp);
            ApperanceRefreshABreath(HeldWeaponAgent.BreathFactor);
            // 添加到背包的时候会执行刷新模型逻辑
            ModelRefreshWeaponModel(baseComp.ConfigId, HeldSlotType, attachments);
        }

        private void RefreshModelWeaponParts(WeaponPartsRefreshStruct refreshData)
        {
            if (refreshData.armInPackage)
            {
                var avatarId = refreshData.weaponInfo.AvatarId;
                if (avatarId < 1)
                    avatarId = SingletonManager.Get<WeaponResourceConfigManager>()
                                               .GetConfigById(refreshData.weaponInfo.ConfigId).AvatorId;
                if (WeaponUtil.IsC4p(refreshData.weaponInfo.ConfigId))
                {
                    OverrideBagTactic = refreshData.weaponInfo.ConfigId;
                    UnArmC4();
                }
                else
                {
                    RelatedAppearence.MountWeaponInPackage(refreshData.slot.ToWeaponInPackage(), avatarId);
                }
            }

            WeaponPartUtil.RefreshWeaponPartModels(RelatedAppearence, refreshData.weaponInfo.ConfigId,
                refreshData.oldParts, refreshData.newParts, refreshData.slot);

            if (refreshData.lastWeaponKey.IsValid())
            {
                if (refreshData.slot == HeldSlotType)
                    RefreshHeldWeapon();
                ////var handler = slotsAux.FindHandler(refreshData.slot);

                //if (refreshData.lastWeaponId != refreshData.weaponInfo.ConfigId)
                //    handler.RecordLastWeaponId(refreshData.lastWeaponId);
            }
        }

        private void RefreshHeldWeaponDetail()
        {
            RefreshHeldWeapon();
            // 需要执行刷新配件逻辑，因为配件会影响相机动作等属性
            RefreshHeldWeaponAttachment();
        }

        private void DoSpecialAppearance()
        {
            if (HeldSlotType == EWeaponSlotType.SecondaryWeapon)
                RelatedAppearence.MountP3WeaponOnAlternativeLocator();
        }

        private void DoDrawInterrupt()
        {
            CharacterDrawInterrupt();
        }

        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType targetSlot, bool armOnLeft = false)
        {
            if (WeaponUtil.IsC4p(lastWeaponId))
            {
                UnArmC4();
            }

            if (WeaponUtil.IsC4p(weaponId))
            {
                RelatedAppearence.MountC4(weaponId);
            }

            WeaponInPackage pos = targetSlot.ToWeaponInPackage();
            RelatedAppearence.MountWeaponToHand(pos);
            if (armOnLeft)
                RelatedAppearence.MountP3WeaponOnAlternativeLocator();
        }

        private void DoArmFinished(WeaponScanStruct weapon, EWeaponSlotType slot)
        {
            //TODO:
            SetHeldSlotType(slot);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == EGameMode.Normal)
                {
                    //TODO 判断弹药数量是否足够，如果弹药不足，弹提示框
                    RelatedCharState.ReloadEmpty(() => { });
                }
            }
            else
            {
                //if (!bag.CurBolted)
                //{
                //    //TODO 拉栓动作
                //}
            }
        }

        public void AutoStuffGrenade()
        {
           
            TryHoldGrenade();
            RefreshWeaponAppearance();
        }

        private void RefreshHeldWeapon()
        {
            RelatedOrientation.Reset();

            if (IsHeldSlotEmpty)
                return;
        }

        private LoggerAdapter logger = new LoggerAdapter(typeof(PlayerWeaponController));


        internal void RecoverLastHeldWeapon()
        {
            EWeaponSlotType lastSlot = PollGetLastSlotType(false);
            if (lastSlot != EWeaponSlotType.None)
                ArmWeapon(lastSlot, true);
        }

        #region Update

        public void InternalUpdate(PlayerEntity player)
        {
            if (!HeldWeaponAgent.IsValid())
            {
                if (HeldSlotType != EWeaponSlotType.None)
                    SetHeldSlotType(EWeaponSlotType.None);
            }

            switch (RelatedServerUpdate.EUpdateCmdType)
            {
                case EWeaponUpdateCmdType.UpdateHoldAppearance:
                    logger.Info("WeaponUpdate.UpdateHeldAppearance Come in ");
                    RelatedServerUpdate.UpdateCmdType = 0;
                    //率先刷新手雷物品
                    TryHoldGrenade(true, false);
                    RefreshWeaponAppearance(EWeaponSlotType.ThrowingWeapon);
                    EWeaponSlotType newSlot = PollGetLastSlotType();
                    if (newSlot == HeldSlotType)
                        RefreshWeaponAppearance();
                    else
                    {
                        if (player.playerInfo.JobAttribute == (int) EJobAttribute.EJob_EveryMan)
                        {
                            TryArmWeaponImmediately(newSlot);
                        }
                    }
                    break;
                case EWeaponUpdateCmdType.ExchangePrimaryAppearance:
                    RelatedServerUpdate.UpdateCmdType = 0;
                    RefreshWeaponAppearance(EWeaponSlotType.PrimeWeapon);
                    RefreshWeaponAppearance(EWeaponSlotType.SecondaryWeapon);
                    break;
                default:
                    break;
            }
            
        }

        #endregion
    }
}