using App.Shared.GameMode;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Attack;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Shared.Components.Player;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

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
            onWeaponPickEvt   += processListener.OnPickup;
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
            //把当前的一些枪械状态重置掉
            HeldWeaponAgent.ResetDynamic();
            var lastWeapon       = HeldWeaponAgent.ComponentScan;
            var destWeapon       = GetWeaponAgent(slot).ComponentScan;
            var appearanceStruct = processHelper.GetDrawAppearanceStruct(slot);

            DoSpecialAppearance();
            DoDrawInterrupt();
            if (includeAction)
            {
                if (IsHeldSlotEmpty)
                {
                    DoDrawArmWeaponWithActionFromEmpty(destWeapon, lastWeapon.ConfigId, appearanceStruct);
                }
                else
                {
                    DoDrawArmWeaponWithActionFromOtherWeapon(destWeapon, lastWeapon.ConfigId, appearanceStruct);
                }
            }
            else
            {
                //CharacterState控制动作相关
                DoDrawArmWeaponWithoutAction(destWeapon, lastWeapon.ConfigId, appearanceStruct);
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
            HeldWeaponAgent.ResetDynamic();
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
            float holsterParam = WeaponUtil.GetHolsterParam(HeldSlotType);
            RelatedCharState.CharacterUnmount(() => DoDrawUnarmWeaponWithoutAction(heldConfigId, callback),
                holsterParam);
            onWeaponSwitchEvt(this, heldConfigId, EInOrOff.Off);
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
                appearanceStruct.isSecondWeapon);
            OnSlotArmFinish(destWeapon, appearanceStruct.targetSlot);
            RelatedCharState.Draw(RelatedAppearence.RemountP3WeaponOnRightHand, appearanceStruct.drawParam);
        }

        /// 有动作，从旧装备上画新装备
        private void DoDrawArmWeaponWithActionFromOtherWeapon(WeaponScanStruct           destWeapon, int lastWeaponId,
                                                              WeaponDrawAppearanceStruct appearanceStruct)
        {
            RelatedCharState.SwitchWeapon(
                () =>
                {
                    WeaponToHand(destWeapon.ConfigId, lastWeaponId, appearanceStruct.targetSlot,
                        appearanceStruct.isSecondWeapon);
                    onWeaponSwitchEvt(this, destWeapon.ConfigId, EInOrOff.Off);
                },
                () => OnDrawWeaponCallback(destWeapon, appearanceStruct.targetSlot), appearanceStruct.switchParam);
        }

        /// 没动作，直接刷新装备
        private void DoDrawArmWeaponWithoutAction(WeaponScanStruct           destWeapon, int lastWeaponId,
                                                  WeaponDrawAppearanceStruct appearanceStruct)
        {
            WeaponToHand(destWeapon.ConfigId, lastWeaponId, appearanceStruct.targetSlot,
                appearanceStruct.isSecondWeapon);
            OnDrawWeaponCallback(destWeapon, appearanceStruct.targetSlot);
            OnSlotArmFinish(destWeapon, appearanceStruct.targetSlot);
            RelatedAppearence.RemountP3WeaponOnRightHand();
        }

        private void DoDrawEmptyWeaponInPackage(EWeaponSlotType slotType, bool interrupt)
        {
            WeaponInPackage pos = slotType.ToWeaponInPackage();
            RelatedAppearence.UnmountWeaponInPackage(pos);
            if (interrupt)
                InteractController.InterruptCharactor();
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
                int lastWeapon = HeldConfigId;
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
            bool destroyAndStuffNew = HeldWeaponAgent.ExpendWeapon();
            if (destroyAndStuffNew)
                AutoStuffHeldWeapon();
            onWeaponExpendEvt(this, HeldSlotType);
        }

        #endregion


        public void SwitchBag(int index)
        {
            int length = ModeController.GetUsableWeapnBagLength(RelatedPlayerInfo);
            if (index == HeldBagPointer2) return;
            HeldBagPointer2 = index;
            List<PlayerWeaponBagData> bagDatas = ModeController.FilterSortedWeaponBagDatas(RelatedPlayerInfo);
            if (bagDatas == null || bagDatas.Count <= index) return;
            PlayerWeaponBagData tarBag = null;
            foreach (var bagData in bagDatas)
            {
                if (bagData.BagIndex == index)
                {
                    tarBag = bagData;
                    break;
                }
            }

            if (tarBag == null) return;
            var removedList = new List<EWeaponSlotType>();
            for (EWeaponSlotType j = EWeaponSlotType.None + 1; j < EWeaponSlotType.Length; j++)
            {
                if (j != EWeaponSlotType.ThrowingWeapon && j != EWeaponSlotType.TacticWeapon)
                    removedList.Add(j);
            }

            DestroyWeapon(EWeaponSlotType.ThrowingWeapon, 0);
            RefreshWeaponAppearance(EWeaponSlotType.ThrowingWeapon);
            GrenadeHandler.ClearCache();
            foreach (var weapon in tarBag.weaponList)
            {
                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);

                var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
                var weaponType      = (EWeaponType_Config) weaponAllConfig.NewWeaponCfg.Type;
                if (slot != EWeaponSlotType.ThrowingWeapon && slot != EWeaponSlotType.TacticWeapon)
                {
                    var orient = WeaponUtil.CreateScan(weapon);
                    orient.Bullet         = weaponAllConfig.PropertyCfg.Bullet;
                    orient.ReservedBullet = weaponAllConfig.PropertyCfg.Bulletmax;
                    if (orient.Magazine > 0)
                    {
                        orient.Bullet += SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(orient.Magazine)
                                                         .Bullet;
                    }

                    ReplaceWeaponToSlot(slot, 0, orient);
                }
                else if (slot == EWeaponSlotType.ThrowingWeapon)
                {
                    //     controller.GrenadeHelper.ClearEntityCache();
                    GrenadeHandler.AddCache(weapon.WeaponTplId);
                }

                removedList.Remove(slot);
            }

            removedList.Remove(EWeaponSlotType.TacticWeapon);
            removedList.Remove(EWeaponSlotType.ThrowingWeapon);

            foreach (var i in removedList)
            {
                DestroyWeapon(i, 0);
            }

            EWeaponSlotType newSlot = PollGetLastSlotType();
            TryHoldGrenade(true, false);
            TryArmWeaponImmediately(newSlot);
        }

        [System.Obsolete]
        public void SwitchBag()
        {
            if (CanSwitchWeaponBag)
            {
                int length = ModeController.GetUsableWeapnBagLength(RelatedPlayerInfo);
                SwitchBag((HeldBagPointer2 + 1) % length);
            }
        }

        public void InitBag(int pointer)
        {
            ClearBagPointer();
            HeldBagPointer2 = pointer;
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient)
        {
            if (TryHoldGrenade(orient.ConfigId))
                return true;
            return ReplaceCommonWeapon(slotType, orient, HeldBagPointer);
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
            bagIndex = 0;
            if (!processHelper.FilterVailed(orient, slotType)) return false;
            bool refreshAppearance = (bagIndex == HeldBagPointer || bagIndex < 0);
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
            OnSlotArmFinish(weapon, slot);
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

        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (WeaponUtil.IsC4p(lastWeaponId))
            {
                UnArmC4();
            }

            if (WeaponUtil.IsC4p(weaponId))
            {
                RelatedAppearence.MountC4(weaponId);
            }

            WeaponInPackage pos = slot.ToWeaponInPackage();
            RelatedAppearence.MountWeaponToHand(pos);
            if (armOnLeft)
                RelatedAppearence.MountP3WeaponOnAlternativeLocator();
        }

        private void OnSlotArmFinish(WeaponScanStruct weapon, EWeaponSlotType slot)
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

        private void AutoStuffHeldWeapon()
        {
            var lastSlotType = HeldSlotType;
            var nextId       = HeldWeaponAgent.FindNextWeapon(true);
            //消耗掉当前武器
            DestroyWeapon(HeldSlotType, -1, false);
            //自动填充下一项武器
            if (lastSlotType == EWeaponSlotType.ThrowingWeapon)
                TryHoldGrenade();
            RefreshWeaponAppearance();
        }

        private void RefreshHeldWeapon()
        {
            RelatedOrient.Reset();

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
          
//          if (player.characterBone.IsWeaponRotState && RelatedCameraSNew.IsAiming())
//          {
//              AddRotStateInterrupt();
//          }
//
//          if (RelatedCharState.IsProneMovement() && RelatedCameraSNew.IsAiming())
//          {
//              AddProneMoveStateInterrupt();
//          }
          //DebugUtil.MyLog("PullBoltEnd:"+HeldWeaponAgent.ClientSyncComponent.PullBoltEnd+"IsPullingBolt:"+HeldWeaponAgent.ClientSyncComponent.IsPullingBolt);
            if (!HeldWeaponAgent.IsValid() && HeldSlotType != EWeaponSlotType.None)
                SetHeldSlotType(EWeaponSlotType.None);
            if (WeaponServerUpdate.UpdateHeldAppearance)
            {
                logger.Info("WeaponUpdate.UpdateHeldAppearance Come in ");
                WeaponServerUpdate.UpdateHeldAppearance = false;
                //率先刷新手雷物品
                TryHoldGrenade(true, false);
                RefreshWeaponAppearance(EWeaponSlotType.ThrowingWeapon);
                EWeaponSlotType newSlot = PollGetLastSlotType();
                if (newSlot == HeldSlotType)
                    RefreshWeaponAppearance();
                else
                    TryArmWeaponImmediately(newSlot);
            }
        }

        #endregion

       
    }
}