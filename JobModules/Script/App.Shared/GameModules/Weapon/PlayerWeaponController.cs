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
using App.Shared.Audio;

namespace App.Shared.GameModules.Weapon
{
    public class PlayerWeaponController : ModuleLogicActivator<PlayerWeaponController>, ISharedPlayerWeaponComponentGetter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponController));

        private PlayerEntityWeaponMedium entityMedium;

        private WeaponSlotsAux slotsAux;
        private PlayerWeaponComponentAgent componentAgent;
        
        public PlayerWeaponController()
            //PlayerWeaponComponentAgent weaponAgent)
        {
           // componentAgent = weaponAgent;
            slotsAux = new WeaponSlotsAux();
        }
        public ISharedPlayerWeaponComponentGetter Getter
        {
            get { return componentAgent; }
        }

        #region //initialize

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
        #endregion

        void OnDrawWeaponCallback(Contexts contexts, WeaponInfo weapon, EWeaponSlotType slot)
        {
            OnSlotMountFinish(contexts, weapon, slot);
            entityMedium.Appearance_RemountP3WeaponOnRightHand();
        }
        void OnUnmountWeaponCallback(Contexts contexts, int weaponId, Action onfinish)
        {
            entityMedium.Appearance_UnmountWeaponFromHand();
            componentAgent.SetCurrSlotTypeProcess(contexts, EWeaponSlotType.None,OnSetCurrSlotDetailProcess);
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
        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public EFuncResult SetSlotWeaponPart(Contexts contexts, EWeaponSlotType slot, int id)
        {
            return componentAgent.SetSlotWeaponPart(contexts, slot, id, OnCurrWeaponAttachmentRefresh, OnModelWeaponPartsRefresh);
        }
        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EFuncResult SetSlotWeaponPart(Contexts contexts, int id)
        {
            return componentAgent.SetSlotWeaponPart(contexts, CurrSlotType, id, OnCurrWeaponAttachmentRefresh, OnModelWeaponPartsRefresh);
        }
        /// <summary>
        /// 刷新当前武器parts
        /// </summary>
        private void OnCurrWeaponAttachmentRefresh(Contexts contexts)
        {
            EWeaponSlotType currType = CurrSlotType;
            if (!currType.MayHasPart())
            {
                return;
            }
            WeaponInfo weapon = CurrSlotWeaponInfo(contexts);
            if (weapon.Id < 1) return;
            var attachments = weapon.GetParts();
            //影响表现
            entityMedium.Apperance_RefreshABreath(contexts);
            // 添加到背包的时候会执行刷新模型逻辑
            entityMedium.Model_RefreshWeaponModel(weapon.Id, currType, attachments);
        }
        private void OnModelWeaponPartsRefresh(Contexts contexts, WeaponPartsRefreshData refreshData)
        {
         
            if (refreshData.mountInPackage)
            {
                var avatarId = refreshData.weaponInfo.AvatarId;
                if (avatarId < 1)
                    avatarId = SingletonManager.Get<WeaponConfigManager>().GetConfigById(refreshData.weaponInfo.Id).AvatorId;
                entityMedium.Appearence_ProcessMountWeaponInPackage(refreshData.slot.ToWeaponInPackage(), refreshData.weaponInfo.Id, avatarId);
            }
            entityMedium.Model_RefreshWeaponParts(refreshData.weaponInfo.Id, refreshData.slot, refreshData.oldParts, refreshData.newParts);
            if (refreshData.refreshWeaponLogic)
            {
                if (refreshData.slot == CurrSlotType)
                    RefreshCurrWeapon(contexts);
                var handler = slotsAux.FindHandler(refreshData.slot);
                if (refreshData.lastWeaponId != refreshData.weaponInfo.Id)
                    handler.RecordLastWeaponId(refreshData.lastWeaponId);
            }
        }
        /// <summary>
        /// API:parts
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="part"></param>
        public void DeleteSlotWeaponPart(Contexts contexts, EWeaponSlotType slot, EWeaponPartType part)
        {
            componentAgent.DeleteSlotWeaponPart(contexts, slot, part, OnCurrWeaponAttachmentRefresh, OnModelWeaponPartsRefresh);
        }
        private void OnSetCurrSlotDetailProcess(Contexts contexts)
        {
            RefreshCurrWeapon(contexts);
            // 需要执行刷新配件逻辑，因为配件会影响相机动作等属性
            OnCurrWeaponAttachmentRefresh(contexts);
        }
        #endregion
        /// <summary>
        /// API:相比UseWeapon多了动作,需经由UserCmd触发
        /// </summary>
        /// <param name="slot"></param>
        public void DrawSlotWeapon(Contexts contexts, EWeaponSlotType slot,bool includeAction = true)
        {
            Logger.DebugFormat("DrawWeapon {0}", slot);
            if (componentAgent.IsCurrSlotType(slot))
                return;
            WeaponInfo lastWeapon = componentAgent.CurrSlotWeaponInfo(contexts);
            WeaponInfo destWeapon = componentAgent.GetSlotWeaponInfo(contexts, slot);
            if (destWeapon.Id < 1)
                return;
            AppearanceSpecific();
            //DoDrawInterrupt();
            bool armOnLeft = slot == EWeaponSlotType.SecondaryWeapon;
            float holsterParam = (componentAgent.IsCurrSlotType(EWeaponSlotType.SecondaryWeapon)) ?
                                     AnimatorParametersHash.Instance.HolsterFromLeftValue :
                                     AnimatorParametersHash.Instance.HolsterFromRightValue;
            float drawParam = armOnLeft ?
                                    AnimatorParametersHash.Instance.DrawLeftValue :
                                    AnimatorParametersHash.Instance.DrawRightValue;
            if (includeAction)
            {
                float switchParam = holsterParam * 10 + drawParam;
                if (lastWeapon.Id > 0)
                {
                    entityMedium.CharacterState_SwitchWeapon(() => WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft),
                        () => OnDrawWeaponCallback(contexts, destWeapon, slot), switchParam);
                }
                else
                {
                    WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft);
                    OnSlotMountFinish(contexts, destWeapon, slot);
                    entityMedium.CharacterState_Draw(entityMedium.Appearance_RemountP3WeaponOnRightHand, drawParam);
                }
            }
            else
            {
                //CharacterState控制动作相关
                WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft);
                OnDrawWeaponCallback(contexts, destWeapon, slot);
                OnSlotMountFinish(contexts, destWeapon, slot);
                entityMedium.Appearance_RemountP3WeaponOnRightHand();
            }
          
        }
        private void AppearanceSpecific()
        {
            if (componentAgent.CurrSlotType == EWeaponSlotType.SecondaryWeapon)
                entityMedium.Appearance_MountP3WeaponOnAlternativeLocator();
        }
        private void DoDrawInterrupt()
        {
            entityMedium.CharacterState_DrawInterrupt();
        }
        /// <summary>
        /// API:装备武器
        /// </summary>
        /// <param name="slot"></param>
        public void TryMountSlotWeapon(Contexts contexts, EWeaponSlotType slot)
        {
            WeaponInfo currWeapon = componentAgent.CurrSlotWeaponInfo(contexts);
            WeaponInfo weaponInfo;
            if (componentAgent.TryGetSlotWeaponInfo(contexts, slot, out weaponInfo))
            {
                WeaponToHand(weaponInfo.Id, currWeapon.Id, slot);
                OnSlotMountFinish(contexts, weaponInfo, slot);
            }

        }
        public void UnmountCurrWeapon(Contexts contexts, Action onfinish)
        {
            WeaponInfo weaponInfo = componentAgent.CurrSlotWeaponInfo(contexts);
            AppearanceSpecific();
            float holsterParam = (componentAgent.CurrSlotType == EWeaponSlotType.SecondaryWeapon) ?
                AnimatorParametersHash.Instance.HolsterFromLeftValue :
                AnimatorParametersHash.Instance.HolsterFromRightValue;
            entityMedium.CharacterState_Unmount(() => OnUnmountWeaponCallback(contexts, weaponInfo.Id, onfinish), holsterParam);

        }
        /// <summary>
        /// API:卸载武器
        /// </summary>
        public void UnmountCurrWeapon(Contexts contexts)//float holsterParam)
        {
            UnmountCurrWeapon(contexts, null);
        }
        public IBagDataCacheHelper GetBagCacheHelper(EWeaponSlotType slotType) { return slotsAux.FindHandler(slotType).Helper; }



        /// <summary>
        /// API:卸载武器
        /// </summary>
        public void ForceUnmountCurrWeapon(Contexts contexts)
        {
            entityMedium.Appearance_UnmountWeaponFromHand();
            componentAgent.SetCurrSlotTypeProcess(contexts, EWeaponSlotType.None,OnSetCurrSlotDetailProcess);
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

        private void OnSlotMountFinish(Contexts contexts, WeaponInfo weapon, EWeaponSlotType slot)
        {
            componentAgent.SetCurrSlotTypeProcess(contexts, slot, OnSetCurrSlotDetailProcess);
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
       
        /// <summary>
        /// API: 扔武器
        /// </summary>
        /// <param name="slot"></param>
        public void DropSlotWeapon(Contexts contexts, EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None || slot == EWeaponSlotType.ThrowingWeapon)
            {
                Logger.Error("drop weapon in slot none");
                return;
            }
            var lastWeapon = componentAgent.GetSlotWeaponInfo(contexts, slot);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeapon.Id))
            {
                entityMedium.RemoveC4();
            }
            Logger.DebugFormat("DropWeapon {0}", slot);
            entityMedium.Listener_Drop(slot);
            RemoveSlotWeapon(contexts, slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnDrop();

        }
        /// <summary>
        /// API:interrupt
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="interrupt"></param>
        public void RemoveSlotWeapon(Contexts contexts, EWeaponSlotType slot,bool interrupt = true)
        {
            if (componentAgent.RemoveSlotWeapon(contexts, slot,OnSetCurrSlotDetailProcess))
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
        public int GetSlotWeaponBullet(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponBullet(contexts, slot);
        }

        public void SetSlotWeaponBullet(Contexts contexts, EWeaponSlotType slot, int count)
        {
            componentAgent.SetSlotWeaponBullet(contexts, slot, count);
        }

        public void SetSlotWeaponBullet(Contexts contexts, int count)
        {
            componentAgent.SetSlotWeaponBullet(contexts, count);
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

        /// <summary>
        /// API:自动拾取
        /// </summary>
        /// <param name="weaponInfo"></param>
        /// <returns></returns>
        public bool AutoPickUpWeapon(Contexts contexts, WeaponInfo weaponInfo)
        {
            NewWeaponConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponInfo(weaponInfo, out itemConfig))
                return false;

            var weaponType = SingletonManager.Get<WeaponConfigManager>().GetWeaponType(weaponInfo.Id);
            if (!weaponType.CanAutoPick())
            {
                return false;
            }
            var slotType = GetMatchSlot(contexts, (EWeaponType)itemConfig.Type);
            if (slotType != EWeaponSlotType.None)
            {
                if (componentAgent.IsWeaponSlotStuffed(contexts, slotType))
                {
                    return false;
                }
                var noWeaponInHand = componentAgent.CurrSlotType == EWeaponSlotType.None;
                entityMedium.Listener_Pickup(slotType);
                ReplaceWeaponToSlot(contexts, slotType, weaponInfo, false);
                if (noWeaponInHand)
                {
                    TryMountSlotWeapon(contexts, slotType);
                }
                return true;
            }
            return false;
        }

        private EWeaponSlotType GetMatchSlot(Contexts contexts, EWeaponType weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    var hasPrime = componentAgent.IsWeaponSlotStuffed(contexts, EWeaponSlotType.PrimeWeapon);
                    if (hasPrime && entityMedium.Model_IsSlotAvailable(EWeaponSlotType.SecondaryWeapon))
                        return EWeaponSlotType.SecondaryWeapon;
                    return EWeaponSlotType.PrimeWeapon;
                default:
                    return weaponType.ToWeaponSlot();
            }
        }

        public void Interrupt()
        {
            entityMedium.CharacterState_Interrupt();
        }
        /// <summary>
        /// API:手动拾取武器
        /// </summary>
        /// <param name="weaponInfo"></param>
        /// <returns></returns>
        public WeaponInfo PickUpWeapon(Contexts contexts, WeaponInfo weaponInfo)
        {
            NewWeaponConfigItem weaponCfg;
            if (!WeaponUtil.VertifyWeaponInfo(weaponInfo, out weaponCfg))
                return WeaponInfo.Empty;
            var slotType = GetMatchSlot(contexts, (EWeaponType)weaponCfg.Type);
            if (slotType != EWeaponSlotType.None)
            {
                entityMedium.Listener_Pickup(slotType);
                //除去手雷已经填充到当前槽位的情况
                if (FilterGrenadeStuffedCond(contexts, slotType))
                {
                    WeaponInfo last;
                    var noWeaponInHand = componentAgent.CurrSlotType == EWeaponSlotType.None;
                    if (ReplaceWeaponToSlot(contexts, slotType, weaponInfo, out last))
                    {
                        if(noWeaponInHand)
                        {
                            TryMountSlotWeapon(contexts, slotType);
                        }
                        return last;
                    }
                }
            }
            return WeaponInfo.Empty;
        }
        /// <summary>
        /// API:当前武器被消耗（射击，投弹）
        /// </summary>
        /// <param name="slot"></param>
        public void OnExpend(Contexts contexts, EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None)
                return;

            var weapoonId = CurrSlotWeaponId(contexts);
          
            entityMedium.Listener_OnExpend(slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnExpend(contexts, componentAgent, OnWeaponAutoRestuff);
            if (weapoonId.HasValue)
                GameAudioMedium.PlayWeaponAudio(weapoonId.Value, entityMedium.WeaponHandObject(), (item) => item.Fire);
        }
        /// <summary>
        /// 武器消耗完完自动填充逻辑
        /// </summary>
        /// <param name="slot"></param>
        private void OnWeaponAutoRestuff(Contexts contexts, WeaponSlotExpendData cbData)
        {
            //消耗掉当前武器
            if (cbData.needRemoveCurrent)
                RemoveSlotWeapon(contexts, cbData.slotType, false);
            //自动填充下一项武器
            if (componentAgent.IsWeaponSlotStuffed(contexts, cbData.slotType) || !cbData.needAutoRestuff) return;
            var handler = slotsAux.FindHandler(cbData.slotType);
            int nextId = handler.PickNextId(false);
            if (nextId > 0)
            {
                WeaponInfo last;
                if (ReplaceWeaponToSlot(contexts, cbData.slotType, new WeaponInfo() { Id = nextId }, out last))
                {
                    TryMountSlotWeapon(contexts, cbData.slotType);
                }
            }
        }
        /// 当前槽位同种武器切换逻辑
        private void OnSameSpeciesSwitch(Contexts contexts, EWeaponSlotType slotType, int nextWeaponId)
        {
            if (ReplaceWeaponToSlot(contexts, EWeaponSlotType.ThrowingWeapon, new WeaponInfo
            {
                Id = nextWeaponId,
            }))
            {
                TryMountSlotWeapon(contexts, EWeaponSlotType.ThrowingWeapon);

            }
        }
        /// 不处理手雷已装备情况
        private bool FilterGrenadeStuffedCond(Contexts contexts, EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.ThrowingWeapon||
               !componentAgent.IsWeaponSlotStuffed(contexts, slotType);

        }
        /// <summary>
        /// API:武器槽位切换
        /// </summary>
        /// <param name="in_slot"></param>
        public void SwitchIn(Contexts contexts, EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
                return;
            EWeaponSlotType from_slot = componentAgent.CurrSlotType;
            
            //int from_Id= componentAgent.GetSlotWeaponId(from_slot);
            WeaponInfo wpInfo = componentAgent.GetSlotWeaponInfo(contexts, in_slot);
           
            if (!WeaponUtil.VertifyWeaponInfo(wpInfo))
            {
                entityMedium.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }
            if(from_slot ==in_slot)
            {
                ProcessSameSpeciesSwitchIn(contexts, in_slot);
            }
            else
            {
                DrawSlotWeapon(contexts, in_slot);
                var weaponId = componentAgent.CurrSlotWeaponId(contexts);
                if(weaponId.HasValue)
                {
                    GameAudioMedium.PlayWeaponAudio(weaponId.Value,entityMedium.WeaponHandObject(),(item)=>item.SwitchIn);
                }
                else
                {
                    Logger.Error("weaponId doesn't exist");
                }
            }
         
        }
        public void PureSwitchIn(Contexts contexts, EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
                return;
            EWeaponSlotType from_slot = componentAgent.CurrSlotType;

            //int from_Id= componentAgent.GetSlotWeaponId(from_slot);
            WeaponInfo wpInfo = componentAgent.GetSlotWeaponInfo(contexts, in_slot);

            if (!WeaponUtil.VertifyWeaponInfo(wpInfo))
            {
                entityMedium.ShowTip(Core.Common.ETipType.NoWeaponInSlot);
                return;
            }
            if (from_slot != in_slot)
            {
                DrawSlotWeapon(contexts, in_slot, false);
            }
        }

        ///相同种类武器切换处理逻辑
        private void ProcessSameSpeciesSwitchIn(Contexts contexts, EWeaponSlotType slot )
        {
            //非手雷类型先不做处理
            if (slot != EWeaponSlotType.ThrowingWeapon) return;
            if (entityMedium.CanUseGreande() != Err_WeaponLogicErrCode.Sucess) return;
            int nextId = slotsAux.FindHandler(slot).PickNextId(true);
            if(nextId>0)
            {
                if (ReplaceWeaponToSlot(contexts, EWeaponSlotType.ThrowingWeapon, new WeaponInfo { Id = nextId, }))
                {
                    TryMountSlotWeapon(contexts, EWeaponSlotType.ThrowingWeapon);
                }
            }
        }
       

        public bool ReplaceWeaponToSlot(Contexts contexts, EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify = true)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(contexts, slotType, weaponInfo, vertify, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(Contexts contexts, EWeaponSlotType slotType, WeaponInfo weaponInfo)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(contexts, slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(Contexts contexts, EWeaponSlotType slotType, WeaponInfo weaponInfo, out WeaponInfo lastWp)
        {
            return ReplaceWeaponToSlot(contexts, slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        /// <summary>
        /// API:替换新的武器到槽位上去
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="weaponInfo"></param>
        /// <param name="vertify"></param>
        /// <param name="lastWp"></param>
        /// <returns></returns>
        public bool ReplaceWeaponToSlot(Contexts contexts, EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify, out WeaponInfo lastWp)
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
           
            var errCode = componentAgent.AddWeaponToSlot(contexts, slotType, weaponInfo, OnModelWeaponPartsRefresh, out lastWp);
            return errCode == Err_WeaponLogicErrCode.Sucess;
              
        }
  ///更新当前武器的功能，逻辑
        private void RefreshCurrWeapon(Contexts contexts)
        {
            EWeaponSlotType slot = CurrSlotType;
            // 清理之前的枪械状态信息
           entityMedium.Player_ClearPlayerWeaponState(contexts);
            var weapon = GetSlotWeaponInfo(contexts, slot);
            if (!WeaponUtil.VertifyWeaponId(weapon.Id))
            {
                entityMedium.Player_RefreshPlayerWeaponLogic(contexts, UniversalConsts.InvalidIntId);
                return;
            }
            entityMedium.Player_RefreshPlayerWeaponLogic(contexts, weapon.Id);
            //重置开火模式
            componentAgent.ResetSlotFireModel(contexts, slot);
        }
        #region// IPlayerWeaponComponentcomponentAgentShared Implenentment
        public EWeaponSlotType CurrSlotType { get { return componentAgent.CurrSlotType; } }

        public int? CurrSlotWeaponId(Contexts contexts) { return componentAgent.CurrSlotWeaponId(contexts); }

        public WeaponInfo CurrSlotWeaponInfo(Contexts contexts) { return componentAgent.CurrSlotWeaponInfo(contexts); }

        public int CurrWeaponBullet(Contexts contexts) {  return componentAgent.CurrWeaponBullet(contexts); }
      

        public WeaponInfo GetSlotWeaponInfo(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponInfo(contexts, slot);
        }

        public int? GetSlotWeaponId(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponId(contexts, slot);
        }

        public bool TryGetSlotWeaponInfo(Contexts contexts, EWeaponSlotType slot, out WeaponInfo wpInfo)
        {
            return componentAgent.TryGetSlotWeaponInfo(contexts, slot, out wpInfo);
        }

        public int GetLastWeaponSlot()
        {
            return componentAgent.GetLastWeaponSlot();
        }

        public bool IsWeaponSlotStuffed(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.IsWeaponSlotStuffed(contexts, slot);
        }

        public bool IsWeaponCurrSlotStuffed(Contexts contexts)
        {
            return componentAgent.IsWeaponCurrSlotStuffed(contexts);
        }

        public bool IsWeaponStuffedInSlot(Contexts contexts, int weaponId)
        {
            return componentAgent.IsWeaponStuffedInSlot(contexts, weaponId);
        }

        public EWeaponSlotType PopGetLastWeaponId(Contexts contexts)
        {
            return componentAgent.PopGetLastWeaponId(contexts);
        }

        public int GetSlotFireModeCount(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.GetSlotFireModeCount(contexts, slot);
        }

        public bool GetSlotWeaponBolted(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.GetSlotWeaponBolted(contexts, slot);
        }

        public int GetSlotFireMode(Contexts contexts, EWeaponSlotType slot)
        {
            return componentAgent.GetSlotFireMode(contexts, slot);
        }

        public bool IsCurrSlotType(EWeaponSlotType slot)
        {
            return componentAgent.IsCurrSlotType(slot);
        }

        public override string ToString()
        {
            var slotType = CurrSlotType;
            //var slotCmp = CurrSlotWeaponInfo();
            //TODO log
            //return string.Format("playerControllerInfo==>\n slot:{0},info:{1}", slotType, slotCmp);
            return string.Format("playerControllerInfo==>\n slot:{0}", slotType);
        }
            
        #endregion
    }

}
