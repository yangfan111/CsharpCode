using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Audio;
using App.Shared.Components.Player;
using Core;
using Core.Appearance;
using Core.CharacterBone;
using Core.CharacterState;
using Core.Common;
using Core.EntityComponent;
using Core.Statistics;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Throwing;
using System;
using System.Collections.Generic;
using WeaponConfigNs;
using XmlConfig;
/// <summary>
///     #region//service api
//partial void DrawWeapon(EWeaponSlotType slot, bool includeAction = true);
//public partial void TryArmWeapon(EWeaponSlotType slot);
//public partial void UnArmHeldWeapon(Action onfinish);
//public partial void ForceUnArmHeldWeapon();
//public partial void DropWeapon(EWeaponSlotType slot);
//public partial void RemoveWeapon(EWeaponSlotType slot, bool interrupt = true);
//public partial bool AutoPickUpWeapon(WeaponScanStruct orient);
//public partial EntityKey PickUpWeapon(WeaponScanStruct orient);
//public partial void SwitchIn(EWeaponSlotType in_slot);
//public partial void PureSwitchIn(EWeaponSlotType in_slot);
//public partial void ExpendAfterAttack(EWeaponSlotType slot);
//public partial bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient);
//public partial bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient, bool vertify, out EntityKey lastKey);
//public partial void Interrupt();
//public partial void SetReservedBullet(int count);
//public partial void SetReservedBullet(EWeaponSlotType slot, int count);
//public partial int SetReservedBullet(EBulletCaliber caliber, int count);
//public partial bool SetWeaponPart(EWeaponSlotType slot, int id);
//public partial void DeleteWeaponPart(EWeaponSlotType slot, EWeaponPartType part);

/// </summary>
namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponController" />
    /// </summary>
    public partial class PlayerWeaponController : ModuleLogicActivator<PlayerWeaponController>, ISharedPlayerWeaponGetter
    {
        protected PlayerEntityWeaponInteract weaponInteract;

        protected readonly WeaponSlotsAux slotsAux;

        protected PlayerWeaponComponentsAgent playerWeaponAgent;


        protected readonly WeaponComponentsAgent[,] slotWeaponAgents;

        /// <summary>
        /// 槽位武器监听事件
        /// </summary>

        protected WeaponComponentsAgent CreateGetWeaponAgent(int bagIndex, EWeaponSlotType slotType)
        {
            if (slotWeaponAgents[bagIndex, (int)slotType] == null)
            {
                var func = playerWeaponAgent.MakeBagWeaponKeyExtractor(slotType, bagIndex);
                var newAgent = new WeaponComponentsAgent(func, () => EmptyWeaponKey);
                slotWeaponAgents[bagIndex, (int)slotType] = newAgent;

            }
            return slotWeaponAgents[bagIndex, (int)slotType];
        }
        public IGrenadeCacheHelper GetBagCacheHelper(EWeaponSlotType slotType)
        {
            return slotsAux.FindHandler(slotType).GrenadeHelper;
        }
        //public override string ToString()
        //{
        //    //string s = "";
        //    //foreach(WeaponBagContainer val in playerWeaponAgent.BagSetCache.WeaponBags)
        //    //{
        //    //    s += val.ToString();
        //    //}
        //    ////return s;
        //}
        #region//initialization


        public PlayerWeaponController()
        {

            slotsAux = new WeaponSlotsAux();
            slotWeaponAgents = new WeaponComponentsAgent[GameGlobalConst.WeaponBagMaxCount, GameGlobalConst.WeaponSlotMaxLength];
        }


        public void SetOwner(EntityKey owner)
        {
            Owner = owner;
        }

        public void SetPlayerWeaponAgent(PlayerWeaponComponentsAgent agent)
        {
            playerWeaponAgent = agent;
        }

        public void SetInteract(PlayerEntityWeaponInteract interact)
        {
            weaponInteract = interact;
        }





        //public void SetConfigManager(IPlayerWeaponResourceConfigManager configManager)
        //{
        //    WeaponComponentsAgent.ConfigManager = configManager;
        //}

        public void SetSlotHelper(EWeaponSlotType slotType, ISlotHelper helper)
        {
            CommonSlotHandler handler = slotsAux.FindHandler(slotType);
            handler.SetHelper(helper);

        }
        public void ResetAllComponents()
        {
            if (RelatedOrient != null)
                RelatedOrient.Reset();
        }
        public void ResetBagLockState(int interval)
        {
            BagLockState = false ;
            BagOpenLimitTIme = RelatedTime.ClientTime + interval;
        }
        #endregion

        #region//Player weapon components that can be modified directly

        public void AddAuxBullet(PlayerBulletData bulletData)
        {
            if (playerWeaponAgent.AuxCache.BulletList != null)
                playerWeaponAgent.AuxCache.BulletList.Add(bulletData);
        }
        public void AddAuxEffect()
        {
            playerWeaponAgent.AuxCache.ClientInitialize = true;
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
     

        #endregion

        #region//ISharedPlayerWeaponGetter implement

        public EntityKey Owner { get; protected set; }
        public EntityKey EmptyWeaponKey { get { return playerWeaponAgent.EmptyWeaponKey; } }
        public WeaponComponentsAgent HeldWeaponAgent
        {
            get { return CreateGetWeaponAgent(HeldBagPointer, HeldSlotType); }
        }
        public int HeldConfigId { get { return HeldWeaponAgent.ConfigId; } }
        public WeaponComponentsAgent GetWeaponAgent(EWeaponSlotType slotType = EWeaponSlotType.Pointer, int bagIndex = -1)
        {
            if (bagIndex < 0) bagIndex = HeldBagPointer;

            if (slotType == EWeaponSlotType.Pointer) slotType = HeldSlotType;
            else if (slotType == EWeaponSlotType.LastPointer) slotType = LastSlotType;
            return CreateGetWeaponAgent(bagIndex, slotType);
        }
        public WeaponComponentsAgent GetWeaponAgent(int configId, int bagIndex = -1)
        {
            EWeaponSlotType slotType = WeaponUtil.GetEWeaponSlotTypeById(configId);
            if (slotType == EWeaponSlotType.None) return null;
            if (bagIndex < 0) bagIndex = HeldBagPointer;
            return CreateGetWeaponAgent(bagIndex, slotType);
        }
        public bool IsWeaponInSlot(int configId)
        {
            return GetWeaponAgent(configId) != null;
        }
        public EWeaponSlotType HeldSlotType
        {
            get { return playerWeaponAgent.HeldSlotType; }
        }

        public bool IsHeldSlotEmpty
        {
            get { return playerWeaponAgent.IsHeldSlotEmpty; }
        }

        public EWeaponSlotType LastSlotType
        {
            get
            {
                return playerWeaponAgent.LastSlotType;
            }
        }

        public int HeldBagPointer
        {
            get{return playerWeaponAgent.HeldBagPointer;}
            
        }

        public int GetReservedBullet(EBulletCaliber caliber)
        {
            return RelatedModelLogic.GetReservedBullet(this, caliber);
        }

        public int GetReservedBullet()
        {
            return RelatedModelLogic.GetReservedBullet(this, HeldSlotType);
        }

        public int GetReservedBullet(EWeaponSlotType slot)
        {
            if (slot.IsSlotWithBullet())
                return RelatedModelLogic.GetReservedBullet(this, slot);
            return 0;
        }

        public bool IsWeaponSlotEmpty(EWeaponSlotType slot)
        {
            return playerWeaponAgent.IsWeaponSlotEmpty(slot);
        }

        public bool IsHeldSlotType(EWeaponSlotType slot, int bagIndex = -1)
        {
            return playerWeaponAgent.IsHeldSlotType(slot, bagIndex);
        }

        public EWeaponSlotType PollGetLastSlotType()
        {
            return playerWeaponAgent.PollGetLastSlotType();
        }
        public List<PlayerBulletData> BulletList { get { return playerWeaponAgent.AuxCache.BulletList; } }
        public List<EClientEffectType> EffectList { get { return playerWeaponAgent.AuxCache.EffectList; } }
        public int ForceInterruptGunSight
        {
            get
            {
                if (playerWeaponAgent.AuxCache.ClientInitialize)
                    return playerWeaponAgent.AuxCache.ForceInterruptGunSight;
                return -1;
            }
            set { playerWeaponAgent.AuxCache.ForceInterruptGunSight = value; }
        }
        public int? AutoFire
        {
            get
            {

                if (playerWeaponAgent.AuxCache.HasAutoAction)
                    return playerWeaponAgent.AuxCache.AutoFire;
                return null;
            }
            set { playerWeaponAgent.AuxCache.AutoFire = value.Value; }
        }

        public int BagOpenLimitTIme
        {
            get { return playerWeaponAgent.AuxCache.BagOpenLimitTime; }
            set { playerWeaponAgent.AuxCache.BagOpenLimitTime = value; }
        }
        public bool? AutoThrowing
        {
            get
            {

                if (playerWeaponAgent.AuxCache.HasAutoAction)
                    return playerWeaponAgent.AuxCache.AutoThrowing;
                return null;
            }
            set { playerWeaponAgent.AuxCache.AutoThrowing = value.Value; }
        }

        ///overridebag components
        public int OverrideBagTactic
        {
            get { return playerWeaponAgent.OverrideCache != null ? playerWeaponAgent.OverrideCache.TacticWeapon : 0; }
            set { if (playerWeaponAgent.OverrideCache != null) playerWeaponAgent.OverrideCache.TacticWeapon = value; }

        }
        public bool BagLockState
        {
            get { return playerWeaponAgent.AuxCache.BagLockState; }
            set { playerWeaponAgent.AuxCache.BagLockState = value; }
        }

        public bool CanSwitchWeaponBag
        {
            get { return RelatedModelLogic.CanModeSwitchBag && !BagLockState && BagOpenLimitTIme > RelatedTime.ClientTime; }
        }
        public void PlayFireAudio()
        {
            if (!IsHeldSlotEmpty)
                GameAudioMedium.PlayWeaponAudio(HeldConfigId, RelatedAppearence.WeaponHandObject(), (config) => config.Fire);
        }
        #endregion

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
        public IWeaponMode RelatedModelLogic
        {
            get { return weaponInteract.RelatedModelLogic.ModeLogic; }
        }
        public PlayerWeaponAmmunitionComponent RelatedAmmunition
        {
            get { return weaponInteract.RelatedAmmunition; }
        }

     

        public void ShowTip(ETipType tip)
        {
            weaponInteract.ShowTip(tip);
        }
        public void CreateSetMeleeAttackInfoSync(int atk)
        {
            weaponInteract.CreateSetMeleeAttackInfoSync(atk);
        }

       
        #endregion




    }
}
