using Core;
using Core.Utils;
using App.Shared.Components;
using Utils.Appearance;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.CharacterState;
using System;
using Utils.Singleton;
using App.Shared.Util;
using WeaponConfigNs;
using Core.Enums;
using XmlConfig;
using Core.GameModeLogic;
using Utils.Utils;
using Core.WeaponLogic.Attachment;

namespace App.Shared.GameModules.Weapon
{
    public class PlayerWeaponController : ModuleLogicActivator<PlayerWeaponController>, ISharedPlayerWeaponComponentGetter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponController));

        private PlayerEntityWeaponMedium entityMedium;

        private WeaponBagSlotsAux slotsAux;
        private PlayerWeaponComponentAgent componentAgent;
        
        public PlayerWeaponController()
            //PlayerWeaponComponentAgent weaponAgent)
        {
           // componentAgent = weaponAgent;
            slotsAux = new WeaponBagSlotsAux();
        }
        public ISharedPlayerWeaponComponentGetter Getter
        {
            get { return componentAgent; }
        }



        public void SetAgent(PlayerWeaponComponentAgent agent)
        {
            componentAgent = agent;
        }
        public void SetMedium(PlayerEntityWeaponMedium medium)
        {
            entityMedium = medium;
        }
        public void SetListener(IWeaponProcessListener processListener)
        {
            entityMedium.ProcessListener = processListener;
        }
        public void SetBagCacheHelper(EWeaponSlotType slotType,IBagDataCacheHelper helper)
        {
            WeaponSlotHandlerBase handler = slotsAux.FindHandler(slotType);
            handler.SetHelper(helper);
        }
        

        void OnDrawWeaponCallback(WeaponInfo weapon, EWeaponSlotType slot)
        {
            OnSlotMountFinish(weapon, slot);
            entityMedium.Appearance_RemountP3WeaponOnRightHand();
        }
        void OnUnmountWeaponCallback(int weaponId, Action onfinish)
        {
            entityMedium.Appearance_UnmountWeaponFromHand();
            componentAgent.SetCurrSlotTypeProcess(EWeaponSlotType.None,OnSetCurrSlotDetailProcess);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                entityMedium.UnmountC4();
            }
            if (null != onfinish)
            {
                onfinish();
            }
        }

        #region// parts相关
        public EFuncResult SetSlotWeaponPart(EWeaponSlotType slot, int id)
        {
            return componentAgent.SetSlotWeaponPart(slot, id, OnCurrWeaponAttachmentRefresh, OnModelWeaponPartsRefresh);
        }
        public EFuncResult SetSlotWeaponPart(int id)
        {
            return componentAgent.SetSlotWeaponPart(CurrSlotType, id, OnCurrWeaponAttachmentRefresh, OnModelWeaponPartsRefresh);
        }
        /// <summary>
        /// 刷新当前武器parts
        /// </summary>
        private void OnCurrWeaponAttachmentRefresh()
        {
            EWeaponSlotType currType = CurrSlotType;
            if (!currType.MayHasPart())
            {
                return;
            }
            WeaponInfo weapon = CurrSlotWeaponInfo;
            if (weapon.Id < 1) return;
            var attachments = weapon.GetParts();
           //直接影响数值，或者通过数值来体现的表现
            entityMedium.Weapon_SetAttachment(attachments);
            //影响表现
            entityMedium.Apperance_RefreshABreath();
            // 添加到背包的时候会执行刷新模型逻辑
            entityMedium.Model_RefreshWeaponModel(weapon.Id, currType, attachments);
        }
        private void OnModelWeaponPartsRefresh(WeaponInfo weaponInfo, EWeaponSlotType slot, WeaponPartsStruct oldAttachment, WeaponPartsStruct newAttachments,bool mountInPackages)
        {
            entityMedium.Model_RefreshWeaponParts(weaponInfo.Id, slot, oldAttachment, newAttachments);
            if(mountInPackages)
            {
                var avatarId = weaponInfo.AvatarId;
                if (avatarId < 1)
                    avatarId = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponInfo.Id).AvatorId;
                entityMedium.Appearence_ProcessMountWeaponInPackage(slot.ToWeaponInPackage(), weaponInfo.Id, avatarId);
            }
        }
        public void DeleteSlotWeaponPart(EWeaponSlotType slot, EWeaponPartType part)
        {
            componentAgent.DeleteSlotWeaponPart(slot, part, OnCurrWeaponAttachmentRefresh, OnModelWeaponPartsRefresh);
        }
        private void OnSetCurrSlotDetailProcess()
        {
            RefreshCurrWeapon();
            // 需要执行刷新配件逻辑，因为配件会影响相机动作等属性
            OnCurrWeaponAttachmentRefresh();
        }
        #endregion
        /// <summary>
        /// 相比UseWeapon多了动作,需经由UserCmd触发
        /// </summary>
        /// <param name="slot"></param>
        public void DrawSlotWeapon(EWeaponSlotType slot)
        {
            Logger.DebugFormat("DrawWeapon {0}", slot);
            if (componentAgent.IsCurrSlotType(slot))
                return;
            WeaponInfo lastWeapon = componentAgent.CurrSlotWeaponInfo;
            WeaponInfo destWeapon = componentAgent.GetSlotWeaponInfo(slot);
            if (destWeapon.Id < 1)
                return;

            bool armOnLeft = slot == EWeaponSlotType.PrimeWeapon2;
            float holsterParam = (componentAgent.IsCurrSlotType(EWeaponSlotType.PrimeWeapon2)) ?
                                    AnimatorParametersHash.Instance.HolsterFromLeftValue :
                                    AnimatorParametersHash.Instance.HolsterFromRightValue;
            float drawParam = armOnLeft ?
                                    AnimatorParametersHash.Instance.DrawLeftValue :
                                    AnimatorParametersHash.Instance.DrawRightValue;
            float switchParam = holsterParam * 10 + drawParam;
            AppearanceSpecific();
            DoDrawInterrupt();
            if (lastWeapon.Id > 0)
            {
                entityMedium.CharacterState_SwitchWeapon(() => WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft),
                    () => OnDrawWeaponCallback(destWeapon, slot), switchParam);
            }
            else
            {
                WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft);
                OnSlotMountFinish(destWeapon, slot);
                entityMedium.CharacterState_Draw(entityMedium.Appearance_RemountP3WeaponOnRightHand, drawParam);
            }
        }
        private void AppearanceSpecific()
        {
            if (componentAgent.CurrSlotType == EWeaponSlotType.PrimeWeapon2)
                entityMedium.Appearance_MountP3WeaponOnAlternativeLocator();
        }
        private void DoDrawInterrupt()
        {
            entityMedium.CharacterState_DrawInterrupt();
        }

        public void TryMountSlotWeapon(EWeaponSlotType slot)
        {
            WeaponInfo currWeapon = componentAgent.CurrSlotWeaponInfo;
            WeaponInfo weaponInfo;
            if (componentAgent.TryGetSlotWeaponInfo(slot, out weaponInfo))
            {
                WeaponToHand(weaponInfo.Id, currWeapon.Id, slot);
                OnSlotMountFinish(weaponInfo, slot);
            }

        }
        public void UnmountCurrWeapon(Action onfinish)
        {
            WeaponInfo weaponInfo = componentAgent.CurrSlotWeaponInfo;
            AppearanceSpecific();
            float holsterParam = (componentAgent.CurrSlotType == EWeaponSlotType.PrimeWeapon2) ?
                AnimatorParametersHash.Instance.HolsterFromLeftValue :
                AnimatorParametersHash.Instance.HolsterFromRightValue;
            entityMedium.CharacterState_Unmount(() => OnUnmountWeaponCallback(weaponInfo.Id, onfinish), holsterParam);

        }

        public void UnmountCurrWeapon()//float holsterParam)
        {
            UnmountCurrWeapon(null);
        }
        public IBagDataCacheHelper GetBagCacheHelper(EWeaponSlotType slotType) { return slotsAux.FindHandler(slotType).Helper; }
    
      


        public void ForceUnmountCurrWeapon()
        {
            entityMedium.Appearance_UnmountWeaponFromHand();
            componentAgent.SetCurrSlotTypeProcess(EWeaponSlotType.None,OnSetCurrSlotDetailProcess);
            entityMedium.ThrowAction_Execute();
        }


        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeaponId))
            {
                entityMedium.UnmountC4();
            }
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                entityMedium.MountC4(weaponId);
            }
            WeaponInPackage pos = slot.ToWeaponInPackage();
            entityMedium.Appearance_MountWeaponToHand(pos);
            if (armOnLeft)
                entityMedium.Appearance_MountP3WeaponOnAlternativeLocator();
        }

        private void OnSlotMountFinish(WeaponInfo weapon, EWeaponSlotType slot)
        {
            componentAgent.SetCurrSlotTypeProcess(slot, OnSetCurrSlotDetailProcess);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == GameMode.Normal)
                {
                    //TODO 判断弹药数量是否足够，如果弹药不足，弹提示框
                    entityMedium.CharacterState_ReloadEmpty(() => { });
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
       

        public void DropSlotWeapon(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None || slot == EWeaponSlotType.GrenadeWeapon)
            {
                Logger.Error("drop weapon in slot none");
                return;
            }
            var lastWeapon = componentAgent.GetSlotWeaponInfo(slot);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeapon.Id))
            {
                entityMedium.RemoveC4();
            }
            Logger.DebugFormat("DropWeapon {0}", slot);
            entityMedium.Listener_Drop(slot);
            RemoveSlotWeapon(slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnDrop();

        }
        public void RemoveSlotWeapon(EWeaponSlotType slot,bool interrupt = true)
        {
            if (componentAgent.RemoveSlotWeapon(slot,OnSetCurrSlotDetailProcess))
            {
                WeaponInPackage pos = slot.ToWeaponInPackage();
                entityMedium.Appearance_UnmountWeaponInPackage(pos);
                if(interrupt)
                    Interrupt();
            }
        }
       
        public int GetReservedBullet(EWeaponSlotType slot)
        {
            return entityMedium.Model_GetReserveBullet(slot);
        }
        public int GetSlotWeaponBullet(EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponBullet(slot);
        }

        public void SetSlotWeaponBullet(EWeaponSlotType slot, int count)
        {
            componentAgent.SetSlotWeaponBullet(slot, count);
        }

        public void SetSlotWeaponBullet(int count)
        {
            componentAgent.SetSlotWeaponBullet(count);
        }

        /// <summary>
        /// reserved bullet
        /// </summary>
        /// <param name="count"></param>

        public void SetReservedBullet(int count)
        {
            var currSlot = CurrSlotType;
            if (currSlot.IsSlotWithBullet())
                entityMedium.Model_SetReservedBullet(CurrSlotType, count);
        }
        public void SetReservedBullet(EWeaponSlotType slot, int count)
        {
            if (slot.IsSlotWithBullet())
                entityMedium.Model_SetReservedBullet(slot, count);
        }
        public int SetReservedBullet(EBulletCaliber caliber, int count)
        {
            return entityMedium.Model_SetReservedBullet(caliber, count);
        }

        public int GetReservedBullet(EBulletCaliber caliber)
        {
            return entityMedium.Model_GetReserveBullet(caliber);
        }
        public int GetReservedBullet()
        {
            return entityMedium.Model_GetReserveBullet(CurrSlotType);
        }
        public void ProcessMountWeaponInPackage(WeaponInPackage pos, int weaponId, int avatarId)
        {
            entityMedium.Appearence_ProcessMountWeaponInPackage(pos,weaponId,avatarId);
        }


        public bool AutoPickUpWeapon(WeaponInfo weaponInfo)
        {
            NewWeaponConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponInfo(weaponInfo, out itemConfig))
                return false;

            var weaponType = SingletonManager.Get<WeaponConfigManager>().GetWeaponType(weaponInfo.Id);
            if (!weaponType.CanAutoPick())
            {
                return false;
            }
            var slotType = GetMatchSlot((EWeaponType)itemConfig.Type);
            if (slotType != EWeaponSlotType.None)
            {
                if (componentAgent.IsWeaponSlotStuffed(slotType))
                {
                    return false;
                }
                var noWeaponInHand = componentAgent.CurrSlotType == EWeaponSlotType.None;
                entityMedium.Listener_Pickup(slotType);
                ReplaceWeaponToSlot(slotType, weaponInfo, false);
                if (noWeaponInHand)
                {
                    TryMountSlotWeapon(slotType);
                }
                return true;
            }
            return false;
        }

        private EWeaponSlotType GetMatchSlot(EWeaponType weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    var hasPrime = componentAgent.IsWeaponSlotStuffed(EWeaponSlotType.PrimeWeapon1);
                    if (hasPrime && entityMedium.Model_IsSlotAvailable(EWeaponSlotType.PrimeWeapon2))
                        return EWeaponSlotType.PrimeWeapon2;
                    return EWeaponSlotType.PrimeWeapon1;
                default:
                    return weaponType.ToWeaponSlot();
            }
        }

        public void Interrupt()
        {
            entityMedium.CharacterState_Interrupt();
        }
        public WeaponInfo PickUpWeapon(WeaponInfo weaponInfo)
        {
            NewWeaponConfigItem weaponCfg;
            if (!WeaponUtil.VertifyWeaponInfo(weaponInfo, out weaponCfg))
                return WeaponInfo.Empty;
            var slotType = GetMatchSlot((EWeaponType)weaponCfg.Type);
            if (slotType != EWeaponSlotType.None)
            {
                entityMedium.Listener_Pickup(slotType);
                //除去手雷已经填充到当前槽位的情况
                if (FilterGrenadeStuffedCond(slotType))
                {
                    WeaponInfo last;
                    if (ReplaceWeaponToSlot(slotType, weaponInfo, out last))
                    {
                        TryMountSlotWeapon(slotType);
                        return last;
                    }
                }
            }
            return WeaponInfo.Empty;
        }

        public void OnExpend(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None)
                return;
            entityMedium.Listener_OnExpend(slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnExpend(componentAgent, OnWeaponAutoRestuff);
        }
        /// <summary>
        /// 武器消耗完完自动填充逻辑
        /// </summary>
        /// <param name="slot"></param>
        private void OnWeaponAutoRestuff(WeaponSlotExpendData cbData)
        {
            //消耗掉当前武器
            if (cbData.needRemoveCurrent)
                RemoveSlotWeapon(cbData.slotType,false);
            //自动填充下一项武器
            if (componentAgent.IsWeaponSlotStuffed(cbData.slotType) || !cbData.needAutoRestuff) return;
            var handler = slotsAux.FindHandler(cbData.slotType);
            int nextId = handler.PickNextId(false);
            if (nextId > 0)
            {
                WeaponInfo last;
                if (ReplaceWeaponToSlot(cbData.slotType, new WeaponInfo() { Id = nextId }, out last))
                {
                    TryMountSlotWeapon(cbData.slotType);
                }
            }
        }
        /// <summary>
        /// 当前槽位同种武器切换逻辑
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="nextWeaponId"></param>
        private void OnSameSpeciesSwitch(EWeaponSlotType slotType, int nextWeaponId)
        {
            if (ReplaceWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo
            {
                Id = nextWeaponId,
            }))
            {
                TryMountSlotWeapon(EWeaponSlotType.GrenadeWeapon);

            }
        }
        /// <summary>
        /// 不处理手雷已装备情况
        /// </summary>
        /// <param name="slotType"></param>
        /// <returns></returns>
        private bool FilterGrenadeStuffedCond(EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.GrenadeWeapon ||
               !componentAgent.IsWeaponSlotStuffed(slotType);

        }

        public void SwitchIn(EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
                return;
            var from_slot = componentAgent.CurrSlotType;
            var handler = slotsAux.FindHandler(in_slot);
            WeaponInfo wpInfo = componentAgent.GetSlotWeaponInfo(in_slot);
           
            if (!WeaponUtil.VertifyWeaponInfo(wpInfo))
            {
                entityMedium.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }
            if(from_slot ==in_slot)
            {
                ProcessSameSpeciesSwitchIn(in_slot);
            }
            else
            {
                DrawSlotWeapon(in_slot);
            }


            //   _playerEntity.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
        }
        void ProcessSameSpeciesSwitchIn(EWeaponSlotType slot )
        {
            //非手雷类型先不做处理
            if (slot != EWeaponSlotType.GrenadeWeapon) return;
            if (entityMedium.CanUseGreande() != Err_WeaponLogicErrCode.Sucess) return;
            int nextId = slotsAux.FindHandler(slot).PickNextId(true);
            if(nextId>0)
            {
                if (ReplaceWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo { Id = nextId, }))
                {
                    TryMountSlotWeapon(EWeaponSlotType.GrenadeWeapon);
                }
            }
        }
       

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify = true)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(slotType, weaponInfo, vertify, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, out WeaponInfo lastWp)
        {
            return ReplaceWeaponToSlot(slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify, out WeaponInfo lastWp)
        {
            lastWp = WeaponInfo.Empty;
            if (vertify)
            {
                if (slotType == EWeaponSlotType.None) return false;
                var weaonCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponInfo.Id);
                if (weaonCfg == null)
                    return false;
            }
            if (!entityMedium.Model_IsSlotAvailable(slotType)) return false;
            int lastWeaponId = GetSlotWeaponId(slotType);
            var errCode = componentAgent.AddWeaponToSlot(slotType, weaponInfo, OnModelWeaponPartsRefresh);
            if (errCode == Err_WeaponLogicErrCode.Sucess)
            {
                if (slotType == CurrSlotType)
                    RefreshCurrWeapon();
                var handler = slotsAux.FindHandler(slotType);
                if(lastWeaponId != weaponInfo.Id)
                    handler.RecordLastWeaponId(lastWeaponId);
                return true;
            }
            return false;
        }
  
        private void RefreshCurrWeapon()
        {
            EWeaponSlotType slot = CurrSlotType;
            // 清理之前的枪械状态信息
           entityMedium.Player_ClearPlayerWeaponState();
            var weapon = GetSlotWeaponInfo(slot);
            if (!WeaponUtil.VertifyWeaponId(weapon.Id))
            {
                entityMedium.Player_RefreshPlayerWeaponLogic(UniversalConsts.InvalidIntId);
                return;
            }
            entityMedium.Player_RefreshPlayerWeaponLogic(weapon.Id);
            //重置开火模式
            componentAgent.ResetSlotFireModel(slot);
        }
        #region// IPlayerWeaponComponentcomponentAgentShared Implenentment
        public EWeaponSlotType CurrSlotType { get { return componentAgent.CurrSlotType; } }

        public int CurrSlotWeaponId { get { return componentAgent.CurrSlotWeaponId; } }

        public WeaponInfo CurrSlotWeaponInfo { get { return componentAgent.CurrSlotWeaponInfo; } }

        public int CurrFireMode { get { return componentAgent.CurrFireMode; } set { componentAgent.CurrFireMode = value; } }
        public bool CurrBolted { get { return componentAgent.CurrBolted; } set { componentAgent.CurrBolted = value; } }

        public int CurrWeaponBullet { get { return componentAgent.CurrWeaponBullet; } }
      

        public WeaponInfo GetSlotWeaponInfo(EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponInfo(slot);
        }

        public int GetSlotWeaponId(EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponId(slot);
        }

        public bool TryGetSlotWeaponInfo(EWeaponSlotType slot, out WeaponInfo wpInfo)
        {
            return componentAgent.TryGetSlotWeaponInfo(slot, out wpInfo);
        }

        public int GetLastWeaponSlot()
        {
            return componentAgent.GetLastWeaponSlot();
        }

        public bool IsWeaponSlotStuffed(EWeaponSlotType slot)
        {
            return componentAgent.IsWeaponSlotStuffed(slot);
        }

        public bool IsWeaponCurrSlotStuffed()
        {
            return componentAgent.IsWeaponCurrSlotStuffed();
        }

        public bool IsWeaponStuffedInSlot(int weaponId)
        {
            return componentAgent.IsWeaponStuffedInSlot(weaponId);
        }

        public EWeaponSlotType PopGetLastWeaponId()
        {
            return componentAgent.PopGetLastWeaponId();
        }

        public int GetSlotFireModeCount(EWeaponSlotType slot)
        {
            return componentAgent.GetSlotFireModeCount(slot);
        }

        public bool GetSlotWeaponBolted(EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponBolted(slot);
        }

        public int GetSlotFireMode(EWeaponSlotType slot)
        {
            return componentAgent.GetSlotFireMode(slot);
        }

        public bool IsCurrSlotType(EWeaponSlotType slot)
        {
            return componentAgent.IsCurrSlotType(slot);
        }
        #endregion
    }

}
