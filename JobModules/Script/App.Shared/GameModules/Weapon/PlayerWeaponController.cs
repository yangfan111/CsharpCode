using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Audio;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Util;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Appearance;
using Core.Attack;
using Core.CharacterBone;
using Core.CharacterState;
using Core.Common;
using Core.EntityComponent;
using Core.Free;
using Core.GameModeLogic;
using Core.Statistics;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Throwing;
using System;
using System.Collections.Generic;
using Utils.Appearance;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponController" />
    /// </summary>
    public partial class PlayerWeaponController : ModuleLogicActivator<PlayerWeaponController>, ISharedPlayerWeaponComponentGetter, IPlayerWeaponControllerFrameWork
    {
        private PlayerEntityWeaponInteract weaponInteract;

        private readonly WeaponSlotsAux slotsAux;

        private PlayerWeaponComponentsAgent playerWeaponAgent;

        public EntityKey OwnerKey { get; private set; }

        public ISharedPlayerWeaponComponentGetter Getter
        {
            get { return playerWeaponAgent; }
        }

        #region//initialization
        public void SetOwner(EntityKey owner)
        {
            OwnerKey = owner;
        }

        public void SetPlayerWeaponAgent(PlayerWeaponComponentsAgent agent)
        {
            playerWeaponAgent = agent;
            playerWeaponAgent.AddHeldWeaponListener(UpdateHeldWeaponAgent);
        }

        public void SetInteract(PlayerEntityWeaponInteract interact)
        {
            weaponInteract = interact;
        }

        public void SetProcessListener(IWeaponProcessListener processListener)
        {
            weaponInteract.ProcessListener = processListener;
        }

        public void SetWeaponContext(WeaponContext context)
        {
            WeaponComponentsAgent.WeaponContext = context;
        }

        public void SetConfigManager(IPlayerWeaponConfigManager configManager)
        {
            WeaponComponentsAgent.ConfigManager = configManager;
        }

        public void SetBagCacheHelper(EWeaponSlotType slotType, IBagDataCacheHelper helper)
        {
            WeaponSlotHandlerBase handler = slotsAux.FindHandler(slotType);
            handler.SetHelper(helper);
        }
        public void ResetAllComponents()
        {
            if (RelatedOrient != null)
                RelatedOrient.Reset();
        }
        #endregion

        #region//auxliary  component 
        public bool HasWeaponAutoState
        {
            get { return true; }
        }
        public void AddAuxBullet(PlayerBulletData bulletData)
        {
            if (playerWeaponAgent.AuxCache.BulletList != null)
                playerWeaponAgent.AuxCache.BulletList.Add(bulletData);
        }
        public void AddAuxEffect()
        {
            playerWeaponAgent.AuxCache.EffectList = new List<EClientEffectType>();
        }
        public void AddAuxBullet()
        {
            playerWeaponAgent.AuxCache.BulletList = new List<PlayerBulletData>();
        }
        public void AddAuxEffect(EClientEffectType effectType)
        {
            if (playerWeaponAgent.AuxCache.EffectList != null)
                playerWeaponAgent.AuxCache.EffectList.Add(effectType);
        }
        public List<PlayerBulletData> BulletList { get { return playerWeaponAgent.AuxCache.BulletList; } }
        public List<EClientEffectType> EffectList { get { return playerWeaponAgent.AuxCache.EffectList; } }
        public int ForceInterruptGunSight
        {
            get { return playerWeaponAgent.AuxCache.ForceInterruptGunSight; }
            set { playerWeaponAgent.AuxCache.ForceInterruptGunSight = value; }
        }
        public int AutoFire
        {
            get { return playerWeaponAgent.AuxCache.AutoFire; }
            set { playerWeaponAgent.AuxCache.AutoFire = value; }
        }
        public int BagOpenLimitTIme
        {
            get { return playerWeaponAgent.AuxCache.BagOpenLimitTime; }
            set { playerWeaponAgent.AuxCache.BagOpenLimitTime = value; }
        }
        public bool AutoThrowing
        {
            get { return playerWeaponAgent.AuxCache.AutoThrowing; }
            set { playerWeaponAgent.AuxCache.AutoThrowing = value; }
        }

        ///overridebag components
        public int OverrideBagTactic
        {
            get { return playerWeaponAgent.OverrideCache != null ? playerWeaponAgent.OverrideCache.TacticWeapon : 0; }
            set { if (playerWeaponAgent.OverrideCache != null) playerWeaponAgent.OverrideCache.TacticWeapon = value; }

        }





















        #endregion
        public void DrawSlotWeapon(EWeaponSlotType slot, bool includeAction = true)
        {
            if (playerWeaponAgent.IsHeldSlotType(slot))
                return;
            WeaponScanStruct? lastWeapon = HeldWeaponAgent.BaseComponentScan;
            WeaponScanStruct? destWeapon = GetWeaponAgent(slot).BaseComponentScan;
            if (!destWeapon.HasValue)
                return;
            AppearanceSpecific();
            //DoDrawInterrupt();
            bool armOnLeft = slot == EWeaponSlotType.SecondaryWeapon;
            float holsterParam = (playerWeaponAgent.IsHeldSlotType(EWeaponSlotType.SecondaryWeapon)) ?
                                     AnimatorParametersHash.Instance.HolsterFromLeftValue :
                                     AnimatorParametersHash.Instance.HolsterFromRightValue;
            float drawParam = armOnLeft ?
                                    AnimatorParametersHash.Instance.DrawLeftValue :
                                    AnimatorParametersHash.Instance.DrawRightValue;
            if (includeAction)
            {
                float switchParam = holsterParam * 10 + drawParam;
                if (lastWeapon.HasValue)
                {
                    weaponInteract.CharacterState_SwitchWeapon(() => WeaponToHand(destWeapon.Value.ConfigId, lastWeapon.Value.ConfigId, slot, armOnLeft),
                        () => OnDrawWeaponCallback(destWeapon.Value, slot), switchParam);
                }
                else
                {
                    WeaponToHand(destWeapon.Value.ConfigId, lastWeapon.Value.ConfigId, slot, armOnLeft);
                    OnSlotArmFinish(destWeapon.Value, slot);
                    weaponInteract.CharacterState_Draw(weaponInteract.Appearance_RemountP3WeaponOnRightHand, drawParam);
                }
            }
            else
            {
                //CharacterState控制动作相关
                WeaponToHand(destWeapon.Value.ConfigId, lastWeapon.Value.ConfigId, slot, armOnLeft);
                OnDrawWeaponCallback(destWeapon.Value, slot);
                OnSlotArmFinish(destWeapon.Value, slot);
                weaponInteract.Appearance_RemountP3WeaponOnRightHand();
            }
        }

        public void TryArmSlotWeapon(EWeaponSlotType slot)
        {
            WeaponScanStruct? currWeapon = HeldWeaponAgent.BaseComponentScan;
            if (!currWeapon.HasValue) return;
            WeaponScanStruct? destWeapon = GetWeaponAgent(slot).BaseComponentScan;
            if (!destWeapon.HasValue) return;
            WeaponToHand(destWeapon.Value.ConfigId, currWeapon.Value.ConfigId, slot);
            OnSlotArmFinish(destWeapon.Value, slot);
        }

        public void UnArmHeldWeapon(Action onfinish)
        {
            WeaponScanStruct weaponInfo = HeldWeaponAgent.BaseComponentScan.Value;
            AppearanceSpecific();
            float holsterParam = (playerWeaponAgent.HeldSlotType == EWeaponSlotType.SecondaryWeapon) ?
                AnimatorParametersHash.Instance.HolsterFromLeftValue :
                AnimatorParametersHash.Instance.HolsterFromRightValue;
            weaponInteract.CharacterState_Unmount(() => OnUnArmWeaponCallback(weaponInfo.ConfigId, onfinish), holsterParam);
        }

        public void UnArmHeldWeapon()//float holsterParam)
        {
            UnArmHeldWeapon(null);
        }

        public IBagDataCacheHelper GetBagCacheHelper(EWeaponSlotType slotType)
        {
            return slotsAux.FindHandler(slotType).Helper;
        }

        public void ForceUnarmCurrWeapon()
        {
            weaponInteract.Appearance_UnmountWeaponFromHand();
            SetHeldSlotTypeProcess(EWeaponSlotType.None);
            weaponInteract.ThrowAction_Execute();
        }

        public void DropSlotWeapon(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None || slot == EWeaponSlotType.ThrowingWeapon)
            {
                return;
            }
            WeaponEntity lastWeapon = GetWeaponAgent(slot).Entity;
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeapon.weaponBasicData.ConfigId))
            {
                weaponInteract.RemoveC4();
            }
            weaponInteract.Listener_Drop(slot);
            RemoveSlotWeapon(slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnDrop();
        }

        public void RemoveSlotWeapon(EWeaponSlotType slot, bool interrupt = true)
        {
            if (slot == EWeaponSlotType.None || IsWeaponSlotEmpty(slot)) return;
            WeaponComponentsAgent weaponAgent = GetWeaponAgent(slot);
            if (!weaponAgent.IsVailed()) return;
            weaponAgent.SetFlagNoOwner();
            playerWeaponAgent.RemoveSlotWeapon(slot);
            if (IsHeldSlotType(slot))
            {
                SetHeldSlotTypeProcess(EWeaponSlotType.None);
            }
            WeaponInPackage pos = slot.ToWeaponInPackage();
            weaponInteract.Appearance_UnmountWeaponInPackage(pos);
            if (interrupt)
                Interrupt();
        }

        public bool AutoPickUpWeapon(WeaponScanStruct orient)
        {
            NewWeaponConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponConfigId(orient.ConfigId, out itemConfig))
                return false;

            var weaponType = SingletonManager.Get<WeaponConfigManager>().GetWeaponType(orient.ConfigId);
            if (!weaponType.CanAutoPick())
            {
                return false;
            }
            var slotType = GetMatchSlot((EWeaponType)itemConfig.Type);
            if (slotType != EWeaponSlotType.None)
            {
                if (!IsWeaponSlotEmpty(slotType))
                {
                    return false;
                }
                var noWeaponInHand = playerWeaponAgent.HeldSlotType == EWeaponSlotType.None;
                weaponInteract.Listener_Pickup(slotType);
                ReplaceWeaponToSlot(slotType, orient);
                if (noWeaponInHand)
                {
                    TryArmSlotWeapon(slotType);
                }
                return true;
            }
            return false;
        }

        public EntityKey PickUpWeapon(WeaponScanStruct orient)
        {
            NewWeaponConfigItem weaponCfg;
            if (!WeaponUtil.VertifyWeaponConfigId(orient.ConfigId, out weaponCfg))
                return EntityKey.EmptyWeapon;
            var slotType = GetMatchSlot((EWeaponType)weaponCfg.Type);
            if (slotType != EWeaponSlotType.None)
            {
                weaponInteract.Listener_Pickup(slotType);
                //除去手雷已经填充到当前槽位的情况
                if (FilterGrenadeStuffedCond(slotType))
                {
                    EntityKey last;
                    var noWeaponInHand = HeldSlotType == EWeaponSlotType.None;
                    if (ReplaceWeaponToSlot(slotType, orient, true, out last))
                    {
                        if (noWeaponInHand)
                        {
                            TryArmSlotWeapon(slotType);
                        }
                        return last;
                    }
                }
            }
            return EntityKey.EmptyWeapon;
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
                ProcessSameSpeciesSwitchIn(in_slot);
            }
            else
            {
                DrawSlotWeapon(in_slot);
                GameAudioMedium.PlayWeaponAudio(HeldWeaponAgent.ConfigId.Value, weaponInteract.WeaponHandObject(), (item) => item.SwitchIn);
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
                DrawSlotWeapon(in_slot, false);
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
            var weaponId = heldWeaponAgent.ConfigId;
            if (!weaponId.HasValue) return;
            weaponInteract.Listener_OnExpend(slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnExpend(HeldWeaponAgent, OnWeaponAutoRestuff);
            GameAudioMedium.PlayWeaponAudio(weaponId.Value, weaponInteract.WeaponHandObject(), (item) => item.Fire);
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient)
        {
            EntityKey lastKey;
            return ReplaceWeaponToSlot(slotType, orient, true, out lastKey);
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient, bool vertify, out EntityKey lastKey)
        {
            lastKey = EntityKey.EmptyWeapon;
            //  if (vertify)
            if (slotType == EWeaponSlotType.None) return false;
            var weaonCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(orient.ConfigId);
            if (weaonCfg == null)
                return false;
            if (!weaponInteract.Model_IsSlotAvailable(slotType)) return false;
            WeaponPartsRefreshStruct refreshParams = new WeaponPartsRefreshStruct();
            if (AddWeaponToSlot(slotType, orient, ref lastKey, ref refreshParams))
            {
                RefreshModelWeaponParts(refreshParams);
                return true;
            }
            return false;
        }

        private bool AddWeaponToSlot(EWeaponSlotType slot, WeaponScanStruct orient, ref EntityKey lastWeaponKey, ref WeaponPartsRefreshStruct refreshParams)
        {

            if (slot == EWeaponSlotType.None)
            {
                return false;
            }
            var lastWeaponAgent = GetWeaponAgent(slot);
            lastWeaponAgent.SetFlagNoOwner();
            lastWeaponKey = lastWeaponAgent.EntityKey;
            var orientKey = orient.WeaponKey;
            ///捡起即创建
            WeaponEntity orientEntity = WeaponEntityFactory.GetOrCreateWeaponEntity(WeaponComponentsAgent.WeaponContext, OwnerKey, ref orient);
            bool createNewEntity = orientEntity.entityKey.Value != orientKey;
            playerWeaponAgent.AddSlotWeapon(slot, orientEntity.entityKey.Value);
            if (!createNewEntity)
                orient.CopyToWeaponComponentWithDefaultParts(orientEntity);

            WeaponPartsStruct parts = orient.GetParts();
            var avatarId = orient.AvatarId;
            if (avatarId < 1)
            {
                avatarId = SingletonManager.Get<WeaponConfigManager>().GetConfigById(orient.ConfigId).AvatorId;
            }
            refreshParams.weaponInfo = orient;
            refreshParams.slot = slot;
            refreshParams.oldParts = new WeaponPartsStruct();
            refreshParams.newParts = parts;
            refreshParams.armInPackage = true;
            if (lastWeaponKey != EntityKey.EmptyWeapon)
                refreshParams.SetRefreshLogic(lastWeaponKey);
            return true;
        }

        public void Interrupt()
        {
            weaponInteract.CharacterState_Interrupt();
        }

        public void ProcessMountWeaponInPackage(WeaponInPackage pos, int weaponId, int avatarId)
        {
            weaponInteract.Appearence_ProcessMountWeaponInPackage(pos, weaponId, avatarId);
        }

        public void SetReservedBullet(int count)
        {
            var currSlot = HeldSlotType;
            if (currSlot.IsSlotWithBullet())
                weaponInteract.Model_SetReservedBullet(HeldSlotType, count);
        }

        public void SetReservedBullet(EWeaponSlotType slot, int count)
        {
            if (slot.IsSlotWithBullet())
                weaponInteract.Model_SetReservedBullet(slot, count);
        }

        public int SetReservedBullet(EBulletCaliber caliber, int count)
        {
            return weaponInteract.Model_SetReservedBullet(caliber, count);
        }

        private void SetHeldSlotTypeProcess(EWeaponSlotType slotType)
        {
            playerWeaponAgent.SetHeldSlotType(slotType);
            RefreshHeldWeaponDetail();
        }

        private void OnDrawWeaponCallback(WeaponScanStruct weapon, EWeaponSlotType slot)
        {
            OnSlotArmFinish(weapon, slot);
            weaponInteract.Appearance_RemountP3WeaponOnRightHand();
        }

        private void OnUnArmWeaponCallback(int weaponId, Action onfinish)
        {
            weaponInteract.Appearance_UnmountWeaponFromHand();
            SetHeldSlotTypeProcess(EWeaponSlotType.None);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
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
            WeaponScanStruct? weapon = HeldWeaponAgent.BaseComponentScan;
            if (!weapon.HasValue) return;
            var attachments = weapon.Value.GetParts();
            weaponInteract.Apperance_RefreshABreath(HeldWeaponAgent.BreathFactor);
            // 添加到背包的时候会执行刷新模型逻辑
            weaponInteract.Model_RefreshWeaponModel(weapon.Value.ConfigId, currType, attachments);
        }

        private void RefreshModelWeaponParts(WeaponPartsRefreshStruct refreshData)
        {

            if (refreshData.armInPackage)
            {
                var avatarId = refreshData.weaponInfo.AvatarId;
                if (avatarId < 1)
                    avatarId = SingletonManager.Get<WeaponConfigManager>().GetConfigById(refreshData.weaponInfo.ConfigId).AvatorId;
                weaponInteract.Appearence_ProcessMountWeaponInPackage(refreshData.slot.ToWeaponInPackage(), refreshData.weaponInfo.ConfigId, avatarId);
            }
            weaponInteract.Model_RefreshWeaponParts(refreshData.weaponInfo.ConfigId, refreshData.slot, refreshData.oldParts, refreshData.newParts);
            if (refreshData.needRefreshWeaponLogic)
            {
                if (refreshData.slot == HeldSlotType)
                    RefreshHeldWeapon();
                var handler = slotsAux.FindHandler(refreshData.slot);
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
                weaponInteract.Appearance_MountP3WeaponOnAlternativeLocator();
        }

        private void DoDrawInterrupt()
        {
            weaponInteract.CharacterState_DrawInterrupt();
        }

        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeaponId))
            {
                weaponInteract.UnmountC4();
            }
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                weaponInteract.MountC4(weaponId);
            }
            WeaponInPackage pos = slot.ToWeaponInPackage();
            weaponInteract.Appearance_MountWeaponToHand(pos);
            if (armOnLeft)
                weaponInteract.Appearance_MountP3WeaponOnAlternativeLocator();
        }

        private void OnSlotArmFinish(WeaponScanStruct weapon, EWeaponSlotType slot)
        {
            SetHeldSlotTypeProcess(slot);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == GameMode.Normal)
                {
                    //TODO 判断弹药数量是否足够，如果弹药不足，弹提示框
                    weaponInteract.CharacterState_ReloadEmpty(() => { });
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

        private EWeaponSlotType GetMatchSlot(EWeaponType weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    var hasPrime = !playerWeaponAgent.IsWeaponSlotEmpty(EWeaponSlotType.PrimeWeapon);
                    if (hasPrime && weaponInteract.Model_IsSlotAvailable(EWeaponSlotType.SecondaryWeapon))
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
                RemoveSlotWeapon(cbData.slotType, false);
            //自动填充下一项武器
            if (!IsWeaponSlotEmpty(cbData.slotType) || !cbData.needAutoRestuff) return;
            var handler = slotsAux.FindHandler(cbData.slotType);
            int nextId = handler.PickNextId(false);
            if (nextId > 0)
            {
                EntityKey last;
                if (ReplaceWeaponToSlot(cbData.slotType, WeaponUtil.CreateScan(nextId), true, out last))
                {
                    TryArmSlotWeapon(cbData.slotType);
                }
            }
        }

        private void OnSameSpeciesSwitch(EWeaponSlotType slotType, int nextWeaponId)
        {
            if (ReplaceWeaponToSlot(EWeaponSlotType.ThrowingWeapon, WeaponUtil.CreateScan(nextWeaponId)))
            {
                TryArmSlotWeapon(EWeaponSlotType.ThrowingWeapon);
            }
        }

        private bool FilterGrenadeStuffedCond(EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.ThrowingWeapon ||
                IsWeaponSlotEmpty(slotType);
        }

        private void RefreshHeldWeapon()
        {
            RelatedOrient.Reset();

            if (IsHeldSlotEmpty)
                return;

            //重置开火模式
            HeldWeaponAgent.ResetFireModel();
        }

        private void ProcessSameSpeciesSwitchIn(EWeaponSlotType slot)
        {
            //非手雷类型先不做处理
            if (slot != EWeaponSlotType.ThrowingWeapon) return;
            if (!weaponInteract.CanUseGreande()) return;
            int nextId = slotsAux.FindHandler(slot).PickNextId(true);
            if (nextId > 0)
            {
                if (ReplaceWeaponToSlot(EWeaponSlotType.ThrowingWeapon, WeaponUtil.CreateScan(nextId)))
                {
                    TryArmSlotWeapon(EWeaponSlotType.ThrowingWeapon);
                }
            }
        }

        public EWeaponSlotType HeldSlotType
        {
            get { return Getter.HeldSlotType; }
        }

        public bool IsHeldSlotEmpty
        {
            get { return Getter.IsHeldSlotEmpty; }
        }

        public EWeaponSlotType LastSlotType
        {
            get
            {
                return Getter.LastSlotType;
            }
        }

        public int HeldBagPointer
        {
            get
            {
                return Getter.HeldBagPointer;
            }
        }

        public int GetReservedBullet(EBulletCaliber caliber)
        {
            return weaponInteract.Model_GetReserveBullet(caliber);
        }

        public int GetReservedBullet()
        {
            return weaponInteract.Model_GetReserveBullet(HeldSlotType);
        }

        public int GetReservedBullet(EWeaponSlotType slot)
        {
            return weaponInteract.Model_GetReserveBullet(slot);
        }

        public bool IsWeaponSlotEmpty(EWeaponSlotType slot)
        {
            return Getter.IsWeaponSlotEmpty(slot);
        }

        public bool IsHeldSlotType(EWeaponSlotType slot)
        {
            return Getter.IsHeldSlotType(slot);
        }

        public EWeaponSlotType PollGetLastSlotType()
        {
            return Getter.PollGetLastSlotType();
        }
        #region//related components and operation
        public OrientationComponent RelatedOrient
        {
            get { return weaponInteract.RelatedOrient; }
        }

        public FirePosition RelatedFirePos
        {
            get { return weaponInteract.RelatedFirePos; }
        }

        public TimeComponent RelatedTime
        {
            get { return weaponInteract.RelatedTime; }
        }

        public CameraFinalOutputNewComponent RelatedCameraFinal
        {
            get { return weaponInteract.RelatedCameraFinal; }
        }

        public ThrowingActionInfo RelatedThrowActionInfo
        {
            get { return weaponInteract.RelatedThrowAction.ActionInfo; }
        }

        public ICharacterState RelatedStateInterface
        {
            get { return weaponInteract.RelatedCharState.State; }
        }

        public ThrowingUpdateComponent RelatedThrowUpdate
        {

            get { return weaponInteract.RelatedThrowUpdate; }
        }

        public StatisticsData RelatedStatics
        {
            get { return weaponInteract.RelatedStatistics.Statistics; }
        }

        public CameraStateNewComponent RelatedCameraSNew
        {
            get { return weaponInteract.RelatedCameraSNew; }
        }

        public ICharacterAppearance RelatedAppearence
        {
            get { return weaponInteract.RelatedAppearence.Appearance; }
        }
        public ICharacterBone RelatedBones
        {
            get { return weaponInteract.RelatedBones.CharacterBone; }
        }
        public PlayerInfoComponent RelatedPlayerInfo
        {
            get { return weaponInteract.RelatedPlayerInfo; }
        }

        public PlayerMoveComponent RelatedPlayerMove
        {
            get { return weaponInteract.RelatedPlayerMove; }
        }
        public FreeData RelatedFreeData
        {
            get { return (FreeData)weaponInteract.RelatedFreeData.FreeData; }
        }
        public LocalEventsComponent RelatedLocalEvents
        {
            get { return weaponInteract.RelatedLocalEvents; }
        }
        public IWeaponModeLogic RelatedModelLogic
        {
            get { return weaponInteract.RelatedModelLogic.ModeLogic; }
        }

        public void ShowTip(ETipType tip)
        {
            weaponInteract.ShowTip(tip);
        }

        public void CreateSetMeleeAttackInfo(MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            weaponInteract.CreateSetMeleeAttackInfo(attackInfo, config);
        }

        public void CreateSetMeleeAttackInfoSync(int atk)
        {
            weaponInteract.CreateSetMeleeAttackInfoSync(atk);
        }
        #endregion
    }
}
