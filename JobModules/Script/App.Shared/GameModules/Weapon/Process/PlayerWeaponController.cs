using App.Shared.GameMode;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Attack;
using Core.CharacterState;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Shared.Util;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using Random = UnityEngine.Random;

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
            onWeaponDropEvt += processListener.OnDrop;
            onWeaponExpendEvt += processListener.OnExpend;
            onWeaponSwitchEvt += processListener.OnSwitch;
            onWeaponPickEvt += processListener.OnPickup;
        }

        public void DrawWeapon(EWeaponSlotType slot, bool includeAction = true)
        {
            if (HeldSlotType == slot)
                return;
            WeaponScanStruct lastWeapon = HeldWeaponAgent.ComponentScan;
            if (!GetWeaponAgent(slot).IsValid()) return;
            var destWeapon = GetWeaponAgent(slot).ComponentScan;
            AppearanceSpecific();
            DoDrawInterrupt();
            bool armOnLeft = slot == EWeaponSlotType.SecondaryWeapon;
            float holsterParam = WeaponUtil.GetHolsterParam(HeldSlotType == EWeaponSlotType.SecondaryWeapon);
            float drawParam = armOnLeft
                ? AnimatorParametersHash.Instance.DrawLeftValue
                : AnimatorParametersHash.Instance.DrawRightValue;
            if (includeAction)
            {
                float switchParam = holsterParam * 10 + drawParam;
                if (IsHeldSlotEmpty)
                {
                    WeaponToHand(destWeapon.ConfigId, lastWeapon.ConfigId, slot, armOnLeft);
                    OnSlotArmFinish(destWeapon, slot);
                    RelatedStateInterface.Draw(RelatedAppearence.RemountP3WeaponOnRightHand, drawParam);
                  
                }
                else
                {
                    RelatedStateInterface.SwitchWeapon(
                        () =>
                        {
                            WeaponToHand(destWeapon.ConfigId, lastWeapon.ConfigId, slot, armOnLeft);
                            if (onWeaponSwitchEvt != null)
                                onWeaponSwitchEvt(this, destWeapon.ConfigId, InOrOff.Off);
                            },
                        () => OnDrawWeaponCallback(destWeapon, slot), switchParam);
                }
            }
            else
            {
                //CharacterState控制动作相关
                WeaponToHand(destWeapon.ConfigId, lastWeapon.ConfigId, slot, armOnLeft);
                OnDrawWeaponCallback(destWeapon, slot);
                OnSlotArmFinish(destWeapon, slot);
                RelatedAppearence.RemountP3WeaponOnRightHand();
            }
            if (onWeaponSwitchEvt != null)
                onWeaponSwitchEvt(this,HeldConfigId,InOrOff.In);
        }

        public void TryArmWeapon(EWeaponSlotType slot)
        {
            if (HeldSlotType != EWeaponSlotType.None)
                return;
            var agent = GetWeaponAgent(slot);
            if (!agent.IsValid()) return;
            // if (!currWeapon.IsSafeVailed) return;
            WeaponToHand(agent.ConfigId, HeldConfigId, slot);
            OnSlotArmFinish(agent.ComponentScan, slot);
        }

        public void UnArmHeldWeapon(Action onfinish)
        {
            WeaponScanStruct weaponInfo = HeldWeaponAgent.ComponentScan;
            AppearanceSpecific();
            float holsterParam = WeaponUtil.GetHolsterParam(HeldSlotType);
            RelatedStateInterface.CharacterUnmount(() => OnUnArmWeaponCallback(weaponInfo.ConfigId, onfinish),
                holsterParam);
            if (onWeaponSwitchEvt != null)
                onWeaponSwitchEvt(this, weaponInfo.ConfigId, InOrOff.Off);
        }

     

        public void ForceUnArmHeldWeapon()
        {
            RelatedAppearence.UnmountWeaponFromHand();
            SetHeldSlotTypeProcess(EWeaponSlotType.None);
            weaponPlayerAgent.ThrowActionExecute();
        }

        public bool DropWeapon(EWeaponSlotType slotType = EWeaponSlotType.Pointer)
        {
            if (slotType == EWeaponSlotType.Pointer) slotType = HeldSlotType;
            return DropWeapon(slotType, -1);
        }

        public bool DropWeapon(EWeaponSlotType slotType, int bagIndex)
        {
            if (!weaponProcessor.FilterDrop(slotType)) return false;
            var ret = DestroyWeapon(slotType, bagIndex, true);
            if (onWeaponDropEvt != null)
                onWeaponDropEvt(this, slotType);
            return ret;
        }

        public bool DestroyWeapon(EWeaponSlotType slotType, int bagIndex, bool interrupt = true)
        {
            var weaponAgent = GetWeaponAgent(slotType);
            if (!weaponAgent.IsValid()) return false;
            if (WeaponUtil.IsC4p(weaponAgent.ConfigId))
                RelatedAppearence.RemoveC4();
            weaponAgent.ReleaseWeapon();
            //移除武器背包操作
            weaponPlayerAgent.RemoveBagWeapon(slotType);
            if (IsHeldBagSlotType(slotType))
            {
                SetHeldSlotTypeProcess(EWeaponSlotType.None);
            }

            if (bagIndex == -1 || bagIndex == HeldBagPointer)
            {
                WeaponInPackage pos = slotType.ToWeaponInPackage();
                RelatedAppearence.UnmountWeaponInPackage(pos);
                if (interrupt)
                    Interrupt();
            }

            return true;
        }

        public bool AutoPickUpWeapon(WeaponScanStruct orient)
        {
            var slotType = weaponProcessor.GetMatchedSlot(orient.ConfigId);
            if (!weaponProcessor.FilterVailed(orient, slotType)) return false;
            if (!weaponProcessor.FilterAutoPickup(slotType)) return false;
            if (ReplaceWeaponToSlot(slotType, orient))
            {
                TryArmWeapon(slotType);
                if (onWeaponPickEvt != null)
                    onWeaponPickEvt(this, slotType);
                return true;
            }

            return false;
        }

        public bool PickUpWeapon(WeaponScanStruct orient, bool arm = true)
        {
            var slotType = weaponProcessor.GetMatchedSlot(orient.ConfigId);
            if (!weaponProcessor.FilterVailed(orient, slotType)) return false;
            if (!weaponProcessor.FilterPickup(slotType)) return false;
            if (ReplaceWeaponToSlot(slotType, orient))
            {
                if (arm)
                    TryArmWeapon(slotType);
                if (onWeaponPickEvt != null)
                    onWeaponPickEvt(this, slotType);
                return slotType != EWeaponSlotType.ThrowingWeapon;
            }

            return false;
        }

        public bool PickUpWeapon(WeaponScanStruct orient, ref bool pickupSuccess, bool arm = true)
        {
            var slotType = weaponProcessor.GetMatchedSlot(orient.ConfigId);
            if (!weaponProcessor.FilterVailed(orient, slotType)) return false;
            if (!weaponProcessor.FilterPickup(slotType)) return false;
            if (ReplaceWeaponToSlot(slotType, orient))
            {
                if (arm)
                    TryArmWeapon(slotType);
                if (onWeaponPickEvt != null)
                    onWeaponPickEvt(this, slotType);
                pickupSuccess = slotType != EWeaponSlotType.ThrowingWeapon;
                return pickupSuccess;
            }

            return false;
        }

        public void ShowTip(Core.Common.ETipType tip)
        {
            weaponPlayerAgent.ShowTip(tip);
        }

        public void SwitchIn(EWeaponSlotType in_slot)
        {
            if (!weaponProcessor.FilterSwitchIn(in_slot))
            {
                weaponPlayerAgent.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }

            if (IsHeldBagSlotType(in_slot))
            {
                SameSpeciesSwitchIn(in_slot);
            }
            else
            {
                int lastWeapon = HeldConfigId;
                DrawWeapon(in_slot);
              
              
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
                weaponPlayerAgent.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }

            if (!IsHeldBagSlotType(in_slot))
            {
                DrawWeapon(in_slot, false);
            }
        }

        public void ExpendAfterAttack()
        {
            if (HeldSlotType == EWeaponSlotType.None || IsHeldSlotEmpty)
                return;
  
            //var handler = slotsAux.FindHandler(slot);
            bool destroyAndStuffNew = HeldWeaponAgent.ExpendWeapon();
            if (destroyAndStuffNew)
                AutoStuffHeldWeapon();
            if (onWeaponExpendEvt != null) onWeaponExpendEvt(this, HeldSlotType);
        }

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
            GrenadeHelper.ClearCache();
            foreach (var weapon in tarBag.weaponList)
            {
                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);

                var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
                var weaponType = (EWeaponType_Config) weaponAllConfig.NewWeaponCfg.Type;
                if (slot != EWeaponSlotType.ThrowingWeapon && slot != EWeaponSlotType.TacticWeapon)
                {
                    var orient = WeaponUtil.CreateScan(weapon);
                    orient.Bullet = weaponAllConfig.PropertyCfg.Bullet;
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
                    GrenadeHelper.AddCache(weapon.WeaponTplId);
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
            TryArmWeapon(newSlot);
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
            weaponPlayerAgent.ClearBagPointer();
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
            if (!weaponProcessor.FilterVailed(orient, slotType)) return false;
            bool refreshAppearance = (bagIndex == HeldBagPointer || bagIndex < 0);
            //特殊全局性武器只取武器背包第0个索引值
            var weaponAgent = GetWeaponAgent(slotType);
            WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
            WeaponEntity newEntity = weaponAgent.ReplaceWeapon(Owner, orient, ref refreshParams);
            if (newEntity == null) return false;
            weaponPlayerAgent.SyncBagWeapon(slotType, newEntity.entityKey.Value);
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
                refreshParams.slot = slot;
                refreshParams.weaponInfo = weaponAgent.ComponentScan;
                // refreshParams.oldParts = new WeaponPartsStruct();
                refreshParams.newParts = weaponAgent.PartsScan;
                refreshParams.armInPackage = true;
                RefreshModelWeaponParts(refreshParams);
            }
            else
            {
                //不移除C4 RelatedAppearence.RemoveC4();
                WeaponInPackage pos = slot.ToWeaponInPackage();
                RelatedAppearence.UnmountWeaponInPackage(pos);
                Interrupt();
            }
        }

        public bool TryHoldGrenade(int greandeId)
        {
            if (grenadeHelper.AddCache(greandeId))
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
            int nextId = grenadeHelper.FindUsable(autoStuff);
            if (nextId < 0 || weaponAgent.ConfigId == nextId) return;
            WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
            WeaponEntity newEntity = weaponAgent.ReplaceWeapon(Owner, WeaponUtil.CreateScan(nextId), ref refreshParams);
            weaponPlayerAgent.SyncBagWeapon(EWeaponSlotType.ThrowingWeapon, newEntity.entityKey.Value);
            RefreshModelWeaponParts(refreshParams);
            if (tryArm)
                TryArmWeapon(EWeaponSlotType.ThrowingWeapon);
        }

        public void RemoveGreande(int id)
        {
            int leftCount = grenadeHelper.RemoveCache(id);
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
            weaponPlayerAgent.CharacterInterrupt();
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
            if (!weaponProcessor.FilterSameSepeciesSwitchIn(slot)) return;
            int nextId = GetWeaponAgent(slot).FindNextWeapon(false);
            if (nextId > 0 && slot == EWeaponSlotType.ThrowingWeapon)
            {
                TryHoldGrenade(false);
            }
        }

        public void CreateSetMeleeAttackInfoSync(int in_Sync)
        {
            weaponPlayerAgent.CreateSetMeleeAttackInfoSync(in_Sync);
        }

        public void CreateSetMeleeAttackInfo(MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            weaponPlayerAgent.CreateSetMeleeAttackInfo(attackInfo, config);
        }

        private void SetHeldSlotTypeProcess(EWeaponSlotType slotType)
        {
            weaponPlayerAgent.SetBagHeldSlotType(slotType);
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
            SetHeldSlotTypeProcess(EWeaponSlotType.None);
            if (WeaponUtil.IsC4p(weaponId))
            {
                weaponPlayerAgent.UnArmC4();
            }

            if (onfinish != null)
                onfinish();
        }

        private void RefreshHeldWeaponAttachment()
        {
            if (!weaponProcessor.FilterRefreshWeapon()) return;
            var baseComp = HeldWeaponAgent.ComponentScan;
            var attachments = WeaponPartUtil.CreateParts(baseComp);
            weaponPlayerAgent.ApperanceRefreshABreath(HeldWeaponAgent.BreathFactor);
            // 添加到背包的时候会执行刷新模型逻辑
            weaponPlayerAgent.ModelRefreshWeaponModel(baseComp.ConfigId, HeldSlotType, attachments);
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
                    weaponPlayerAgent.UnArmC4();
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

        private void AppearanceSpecific()
        {
            if (HeldSlotType == EWeaponSlotType.SecondaryWeapon)
                RelatedAppearence.MountP3WeaponOnAlternativeLocator();
        }

        private void DoDrawInterrupt()
        {
            weaponPlayerAgent.CharacterDrawInterrupt();
        }

        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (WeaponUtil.IsC4p(lastWeaponId))
            {
                weaponPlayerAgent.UnArmC4();
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
            SetHeldSlotTypeProcess(slot);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == EGameMode.Normal)
                {
                    //TODO 判断弹药数量是否足够，如果弹药不足，弹提示框
                    RelatedStateInterface.ReloadEmpty(() => { });
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
            var nextId = HeldWeaponAgent.FindNextWeapon(true);
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
        public void LateUpdate(PlayerEntity player)
        {
            if (!HeldWeaponAgent.IsValid() && HeldSlotType != EWeaponSlotType.None)
                SetHeldSlotTypeProcess(EWeaponSlotType.None);
            if (weaponPlayerAgent.WeaponUpdate.UpdateHeldAppearance)
            {
                logger.Info("WeaponPlayerAgent.WeaponUpdate.UpdateHeldAppearance Come in ");
                weaponPlayerAgent.WeaponUpdate.UpdateHeldAppearance = false;
                //率先刷新手雷物品
                TryHoldGrenade(true, false);
                RefreshWeaponAppearance(EWeaponSlotType.ThrowingWeapon);
                EWeaponSlotType newSlot = PollGetLastSlotType();
                if (newSlot == HeldSlotType)
                    RefreshWeaponAppearance();
                else
                    TryArmWeapon(newSlot);
            }

            //      if(player.playerWeaponUpdateComponent2.WaitUpdateBagIndex)
            //      {
            ////          DebugUtil.MyLog("update in",DebugUtil.DebugColor.Green);
            //          //  TryHoldGrenade();
            //          player.playerWeaponUpdateComponent2.WaitUpdateBagIndex = false;
            //          for (EWeaponSlotType j = EWeaponSlotType.None + 1; j < EWeaponSlotType.Length; j++)
            //          {
            //              if(j == EWeaponSlotType.ThrowingWeapon || j == EWeaponSlotType.TacticWeapon)
            //                  RefreshWeaponAppearance(j);
            //          }
            //          EWeaponSlotType newSlot = PollGetLastSlotType();
            //          TryArmWeapon(newSlot);
            //      }
        }

        #region Patched

        #endregion
    }
}