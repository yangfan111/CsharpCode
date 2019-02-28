using App.Shared.Audio;
using App.Shared.Components;
using App.Shared.Util;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Attack;
using Core.CharacterState;
using Core.EntityComponent;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using System;
using Utils.Appearance;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{

    /// <summary>
    /// Defines the <see cref="PlayerWeaponController" />
    /// </summary>
    public partial class PlayerWeaponController : IPlayerWeaponImplentment
    {
        public event WeaponDropEvent onWeaponDropEvt;

        public event WeaponProcessEvent onWeaponPickEvt;

        public event WeaponProcessEvent onWeaponExpendEvt;

        public event WeaponProcessEvent onWeaponSlotCleanupEvt;

        public void SetProcessListener(IWeaponProcessListener processListener)
        {
            onWeaponDropEvt += processListener.OnDrop;
            onWeaponExpendEvt += processListener.OnExpend;
            onWeaponPickEvt += processListener.OnPickup;

        }

        public void DrawWeapon(EWeaponSlotType slot, bool includeAction = true)
        {
            if (playerWeaponAgent.IsHeldSlotType(slot))
                return;
            WeaponScanStruct lastWeapon = HeldWeaponAgent.BaseComponentScan;
            WeaponScanStruct destWeapon = GetWeaponAgent(slot).BaseComponentScan;
            if (!destWeapon.IsSafeVailed)
                return;
            AppearanceSpecific();
            //DoDrawInterrupt();
            bool armOnLeft = slot == EWeaponSlotType.SecondaryWeapon;
            float holsterParam = WeaponUtil.GetHolsterParam(playerWeaponAgent.IsHeldSlotType(EWeaponSlotType.SecondaryWeapon));
            float drawParam = armOnLeft ? AnimatorParametersHash.Instance.DrawLeftValue : AnimatorParametersHash.Instance.DrawRightValue;
            if (includeAction)
            {
                float switchParam = holsterParam * 10 + drawParam;
                if (lastWeapon.IsSafeVailed)
                {
                    RelatedStateInterface.SwitchWeapon(() => WeaponToHand(destWeapon.ConfigId, lastWeapon.ConfigId, slot, armOnLeft),
                        () => OnDrawWeaponCallback(destWeapon, slot), switchParam);
                }
                else
                {
                    WeaponToHand(destWeapon.ConfigId, lastWeapon.ConfigId, slot, armOnLeft);
                    OnSlotArmFinish(destWeapon, slot);
                    RelatedStateInterface.Draw(RelatedAppearence.RemountP3WeaponOnRightHand, drawParam);
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
        }

        public void TryArmWeapon(EWeaponSlotType slot)
        {
            if (IsWeaponSlotEmpty(slot)) return;
            var agent = GetWeaponAgent(slot);
            if (!agent.IsVailed()) return;
            // if (!currWeapon.IsSafeVailed) return;
           
            WeaponToHand(agent.ConfigId, HeldConfigId, slot);
            OnSlotArmFinish(agent.BaseComponentScan, slot);
        }

        public void UnArmHeldWeapon(Action onfinish)
        {
            WeaponScanStruct weaponInfo = HeldWeaponAgent.BaseComponentScan;
            AppearanceSpecific();
            float holsterParam = WeaponUtil.GetHolsterParam(HeldSlotType);
            RelatedStateInterface.CharacterUnmount(() => OnUnArmWeaponCallback(weaponInfo.ConfigId, onfinish), holsterParam);
        }

        public void UnArmHeldWeapon()//float holsterParam)
        {
            UnArmHeldWeapon(null);
        }

        public void ForceUnArmHeldWeapon()
        {
            RelatedAppearence.UnmountWeaponFromHand();
            SetHeldSlotTypeProcess(EWeaponSlotType.None);
            weaponInteract.ThrowActionExecute();
        }
       
        public bool DropWeapon(EWeaponSlotType slotType = EWeaponSlotType.Pointer)
        {
            if (slotType == EWeaponSlotType.Pointer) slotType = HeldSlotType;
            return DropWeapon(slotType, -1);
        }
        /// <summary>
        /// 主动扔把枪到地上，手雷不能丢
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="destroyImmediately"></param>
        /// <returns>是否创建场景掉落武器</returns>
        public bool DropWeapon(EWeaponSlotType slotType,int bagIndex)
        {
            if (slotType == EWeaponSlotType.None || slotType == EWeaponSlotType.ThrowingWeapon)
                return false;
            var weaponAgent = GetWeaponAgent(slotType,bagIndex);
            if (!weaponAgent.IsVailed()) return false;
            if (WeaponUtil.IsC4p(weaponAgent.ConfigId))
            {
                RelatedAppearence.RemoveC4();
            }
            weaponAgent.SetFlagWaitDestroy();
            RemoveWeapon(slotType);
            var handler = slotsAux.FindHandler(slotType);
         //   handler.Drop();
            return true;
        }
        /// <summary>
        /// 直接销毁一把武器
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagIndex"></param>
        public void DestroyWeapon(EWeaponSlotType slotType,int bagIndex)
        {
            if (slotType == EWeaponSlotType.None ) return;
            var weaponAgent = GetWeaponAgent(slotType, bagIndex);
            if (!weaponAgent.IsVailed()) return;
            //移除武器实体操作
            var slotHandler = slotsAux.FindHandler(slotType);
            slotHandler.DestroyWeapon(weaponAgent);
            //移除武器背包操作
            playerWeaponAgent.RemoveBagWeapon(slotType, bagIndex);
            if (IsHeldSlotType(slotType, bagIndex))
            {
                SetHeldSlotTypeProcess(EWeaponSlotType.None);
            }
            if (bagIndex == -1 || bagIndex == HeldBagPointer)
            {
                WeaponInPackage pos = slotType.ToWeaponInPackage();
                RelatedAppearence.UnmountWeaponInPackage(pos);
                Interrupt();
            }
        }
  
        private void RemoveWeapon(EWeaponSlotType slot,int bagIndex =-1, bool interrupt = true)
        {
            if (slot == EWeaponSlotType.None ) return;
            WeaponComponentsAgent weaponAgent = GetWeaponAgent(slot, bagIndex);
            if (!weaponAgent.IsVailed()) return;
            //移除武器实体操作
            var slotHandler = slotsAux.FindHandler(slot);
            slotHandler.ReleaseWeapon(weaponAgent);
            //移除武器背包操作
            playerWeaponAgent.RemoveBagWeapon(slot, bagIndex);
            if (IsHeldSlotType(slot, bagIndex))
            {
                SetHeldSlotTypeProcess(EWeaponSlotType.None);
            }
            if(bagIndex ==-1 || bagIndex == HeldBagPointer)
            {
                WeaponInPackage pos = slot.ToWeaponInPackage();
                RelatedAppearence.UnmountWeaponInPackage(pos);
                if (interrupt)
                    Interrupt();
            }
            
        }
        /// <summary>
        /// 自动拾取
        /// </summary>
        /// <param name="orient"></param>
        /// <returns>返回成功与否</returns>
        public bool AutoPickUpWeapon(WeaponScanStruct orient)
        {
            WeaponResConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponConfigId(orient.ConfigId, out itemConfig))
                return false;
            if (!((EWeaponType_Config)itemConfig.Type).CanAutoPick())
            {
                return false;
            }
            var slotType = GetMatchSlot((EWeaponType_Config)itemConfig.Type);
            if (slotType != EWeaponSlotType.None)
            {
                if (!IsWeaponSlotEmpty(slotType))
                {
                    return false;
                }
                var noWeaponInHand = playerWeaponAgent.HeldSlotType == EWeaponSlotType.None;

                ReplaceWeaponToSlot(slotType, orient);
                if (noWeaponInHand)
                {
                    TryArmWeapon(slotType);
                }
                if (onWeaponPickEvt != null)
                    onWeaponPickEvt(this, slotType);
                return true;
            }
            return false;
        }
        public void ArmGreande()
        {
            int nextGreandeId =slotsAux.FindHandler(EWeaponSlotType.ThrowingWeapon).FindNext(true);
            //TODO:使用
        }
        /// <summary>
        /// pickup中的老物体会自动移出玩家身上
        /// </summary>
        /// <param name="orient"></param>
        /// <returns>返回是否执行生存掉落场景物体</returns>
        public bool PickUpWeapon(WeaponScanStruct orient,bool arm = true)
        {
            WeaponResConfigItem weaponCfg;
            if (!WeaponUtil.VertifyWeaponConfigId(orient.ConfigId, out weaponCfg))
                return false ;
            if (orient.ConfigId == WeaponUtil.EmptyHandId) return false;
            var slotType = GetMatchSlot((EWeaponType_Config)weaponCfg.Type);
            //除去手雷已经填充到当前槽位的情况
            if (slotType == EWeaponSlotType.None || !FilterGrenadeStuffedCond(slotType)) return false;
            bool lastEmpty = IsWeaponSlotEmpty(slotType);
            EntityKey last;
            var noWeaponInHand = HeldSlotType == EWeaponSlotType.None;
            if (ReplaceWeaponToSlot(slotType, orient,-1, out last))
            {
                if (arm && noWeaponInHand)
                {
                    TryArmWeapon(slotType);
                }
                if (onWeaponPickEvt != null)
                    onWeaponPickEvt(this, slotType);
                return slotType != EWeaponSlotType.ThrowingWeapon;
            }
            return false;
        }
      
        public void SwitchIn(EWeaponSlotType in_slot)
        {

            if (IsWeaponSlotEmpty(in_slot))
            {
                weaponInteract.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }

            if (IsHeldSlotType(in_slot))
            {
                SameSpeciesSwitchIn(in_slot);
            }
            else
            {
                DrawWeapon(in_slot);
                GameAudioMedium.PlayWeaponAudio(HeldWeaponAgent.ConfigId, RelatedAppearence.WeaponHandObject(), (item) => item.SwitchIn);
            }
        }

        public void PureSwitchIn(EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
                return;
            EWeaponSlotType from_slot = playerWeaponAgent.HeldSlotType;

            //int from_Id= componentAgent.GetSlotWeaponId(from_slot);

            if (IsWeaponSlotEmpty(in_slot))
            {
                weaponInteract.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }
            if (!IsHeldSlotType(in_slot))
            {
                DrawWeapon(in_slot, false);
            }
        }

        public void ExpendAfterAttack()
        {
            ExpendAfterAttack(HeldSlotType);
        }

        public void ExpendAfterAttack(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None)
                return;
            var weaponId = HeldWeaponAgent.ConfigId;
            if (weaponId < 1) return;

            var handler = slotsAux.FindHandler(slot);
            var agent = GetWeaponAgent(slot);
            handler.Expend(agent, OnWeaponAutoRestuff);
            PlayFireAudio();
            if (onWeaponExpendEvt != null)
                onWeaponExpendEvt(this, slot);

        }
        
        public void SwitchBag(int pointer,int nextInterval,bool refreshAppreace)
        {
            playerWeaponAgent.HeldBagPointer = pointer;
            ResetBagLockState(nextInterval);
            //TODO:切换外观
        }
        public void InitBag(int pointer, int nextInterval)
        {
            playerWeaponAgent.ClearBagPointer();
            playerWeaponAgent.HeldBagPointer = pointer;
            ResetBagLockState(nextInterval);


        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient)
        {
            EntityKey lastKey;
            return ReplaceWeaponToSlot(slotType, orient, HeldBagPointer,out lastKey);
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient, out EntityKey lastKey)
        {
            return ReplaceWeaponToSlot(slotType, orient, HeldBagPointer, out lastKey);
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, int bagIndex,WeaponScanStruct orient)
        {
            EntityKey lastKey;
            return ReplaceWeaponToSlot(slotType, orient, bagIndex,out lastKey);
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient, int bagIndex, out EntityKey lastKey)
        {
            lastKey = EmptyWeaponKey;
            bool refreshAppearance = (bagIndex < 0 || bagIndex == HeldBagPointer);
            //  if (vertify)
            if (slotType == EWeaponSlotType.None) return false;
            if (orient.IsUnSafeOrEmpty()) return false;
            if (!weaponInteract.ModelIsSlotAvailable(slotType)) return false;
            WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
            
            var weaponAgent = GetWeaponAgent(slotType, bagIndex);
            lastKey = weaponAgent.WeaponKey;
            var handler = slotsAux.FindHandler(slotType);
            WeaponEntity orientEntity = handler.ReplaceWeapon(weaponAgent, Owner,orient, ref refreshParams);
            if(orientEntity != null)
            {
                playerWeaponAgent.AddBagWeapon(slotType, orientEntity.entityKey.Value, bagIndex);
                if(refreshAppearance)
                    RefreshModelWeaponParts(refreshParams);
                return true;
            }
            return false;
        }

        public void Interrupt()
        {
            weaponInteract.CharacterInterrupt();
        }

        public void SetReservedBullet(int count)
        {
            var currSlot = HeldSlotType;
            if (currSlot.IsSlotWithBullet())
                RelatedModelLogic.SetReservedBullet(this, HeldSlotType, count);
        }

        public void SetReservedBullet(EWeaponSlotType slot, int count)
        {
            if (slot.IsSlotWithBullet())
                RelatedModelLogic.SetReservedBullet(this, slot, count);
        }

        public int SetReservedBullet(EBulletCaliber caliber, int count)
        {
            return RelatedModelLogic.SetReservedBullet(this, caliber, count);
        }

        
        /// 当前槽位已满状态下不能拾取手雷
        private bool FilterGrenadeStuffedCond(EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.ThrowingWeapon ||
                IsWeaponSlotEmpty(slotType);
        }
        private void SameSpeciesSwitchIn(EWeaponSlotType slot)
        {
            //非手雷类型先不做处理
            if (slot != EWeaponSlotType.ThrowingWeapon) return;
            if (!weaponInteract.CanUseGreande()) return;

            int nextId = slotsAux.FindHandler(slot).FindNext(false);
            if (nextId > 0)
            {
                if (ReplaceWeaponToSlot(EWeaponSlotType.ThrowingWeapon, WeaponUtil.CreateScan(nextId)))
                {
                    TryArmWeapon(EWeaponSlotType.ThrowingWeapon);
                }
            }
        }

        public void CreateSetMeleeAttackInfo(MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            weaponInteract.CreateSetMeleeAttackInfo(attackInfo, config);
        }

        private void SetHeldSlotTypeProcess(EWeaponSlotType slotType)
        {
            playerWeaponAgent.SetHeldSlotType(slotType);
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
                weaponInteract.UnmountC4();
            }
            if (null != onfinish)
            {
                onfinish();
            }
        }

        private void RefreshHeldWeaponAttachment()
        {
            EWeaponSlotType currType = HeldSlotType;
            if (!currType.MayHasPart())
            {
                return;
            }
            WeaponScanStruct weapon = HeldWeaponAgent.BaseComponentScan;
            if (!weapon.IsSafeVailed) return;
            var attachments = weapon.GetParts();
            weaponInteract.ApperanceRefreshABreath(HeldWeaponAgent.BreathFactor);
            // 添加到背包的时候会执行刷新模型逻辑
            weaponInteract.ModelRefreshWeaponModel(weapon.ConfigId, currType, attachments);
        }

        private void RefreshModelWeaponParts(WeaponPartsRefreshStruct refreshData)
        {

            if (refreshData.armInPackage)
            {
                var avatarId = refreshData.weaponInfo.AvatarId;
                if (avatarId < 1)
                    avatarId = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(refreshData.weaponInfo.ConfigId).AvatorId;
                if (WeaponUtil.IsC4p(refreshData.weaponInfo.ConfigId))
                {
                    OverrideBagTactic = refreshData.weaponInfo.ConfigId;
                    weaponInteract.UnmountC4();
                }
                else
                {
                    RelatedAppearence.MountWeaponInPackage(refreshData.slot.ToWeaponInPackage(), avatarId);
                }
            }
            weaponInteract.ModelRefreshWeaponParts(refreshData.weaponInfo.ConfigId, refreshData.slot, refreshData.oldParts, refreshData.newParts);
            if (refreshData.lastWeaponKey.IsVailed())
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
            if (playerWeaponAgent.HeldSlotType == EWeaponSlotType.SecondaryWeapon)
                RelatedAppearence.MountP3WeaponOnAlternativeLocator();
        }

        private void DoDrawInterrupt()
        {
            weaponInteract.CharacterDrawInterrupt();
        }

        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (WeaponUtil.IsC4p(lastWeaponId))
            {
                weaponInteract.UnmountC4();
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
            SetHeldSlotTypeProcess(slot);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == GameMode.Normal)
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

        private EWeaponSlotType GetMatchSlot(EWeaponType_Config weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType_Config.PrimeWeapon:
                    var hasPrime = !playerWeaponAgent.IsWeaponSlotEmpty(EWeaponSlotType.PrimeWeapon);
                    if (hasPrime && weaponInteract.ModelIsSlotAvailable(EWeaponSlotType.SecondaryWeapon))
                        return EWeaponSlotType.SecondaryWeapon;
                    return EWeaponSlotType.PrimeWeapon;
                default:
                    return weaponType.ToWeaponSlot();
            }
        }

        private void OnWeaponAutoRestuff(WeaponSlotExpendStruct cbData)
        {
            //消耗掉当前武器
            if (cbData.needRemoveCurrent)
                RemoveWeapon(cbData.slotType,-1, false);
            //自动填充下一项武器
            if (!IsWeaponSlotEmpty(cbData.slotType) || !cbData.needAutoRestuff) return;
            var handler = slotsAux.FindHandler(cbData.slotType);
            int nextId = handler.FindNext(false);
            if (nextId > 0)
            {
                EntityKey last;
                if (ReplaceWeaponToSlot(cbData.slotType, WeaponUtil.CreateScan(nextId), true, out last))
                {
                    TryArmWeapon(cbData.slotType);
                }
            }
        }

        private void OnSameSpeciesSwitch(EWeaponSlotType slotType, int nextWeaponId)
        {
            if (ReplaceWeaponToSlot(EWeaponSlotType.ThrowingWeapon, WeaponUtil.CreateScan(nextWeaponId)))
            {
                TryArmWeapon(EWeaponSlotType.ThrowingWeapon);
            }
        }

        private void RefreshHeldWeapon()
        {
            RelatedOrient.Reset();

            if (IsHeldSlotEmpty)
                return;

            //重置开火模式
          //  HeldWeaponAgent.ResetFireModel();
        }

    }
}
