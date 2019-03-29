using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Audio;
using App.Shared.Components.Player;
using App.Shared.Util;
using Core;
using Core.Appearance;
using Core.CharacterBone;
using Core.CharacterState;
using Core.Common;
using Core.EntityComponent;
using Core.Event;
using Core.Statistics;
using Core.Utils;
using Core.WeaponLogic.Throwing;
using System;
using System.Collections.Generic;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

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
    public partial class PlayerWeaponController : ModuleLogicActivator<PlayerWeaponController>, IPlayerWeaponSharedGetter
    {

        public GameModeControllerBase ModeController
        {
            get { return GameModuleManagement.Get<GameModeControllerBase>(Owner.EntityId); }
        }
        public PlayerAudioController AudioController
        {
            get { return GameModuleManagement.Get<PlayerAudioController>(Owner.EntityId); }
        }
        //  private readonly WeaponSlotsAux slotsAux;
        private WeaponPlayerComponentsAgent weaponPlayerAgent;

        private readonly WeaponProcessUtil weaponProcessor;

        private GrenadeCacheHelper grenadeHelper;

        private WeaponBaseAgent[] slotWeaponAgents;

        public bool CanUseGrenade { get { return weaponPlayerAgent.CanUseGreande; } }

        public IGrenadeCacheHelper GrenadeHelper { get { return grenadeHelper; } }

       //     private readonly Dictionary<EWeaponSlotType, System.Type> weaponAgentAssTypeDict = new Dictionary<EWeaponSlotType, Type>();

        /// <summary>
        /// 槽位武器监听事件
        /// </summary>
        private WeaponBaseAgent CreateWeaponAgent(EWeaponSlotType slotType, System.Type t)
        {
            if (slotWeaponAgents[(int)slotType] == null)
            {
                var func1 = weaponPlayerAgent.GenerateBagWeaponKeyExtractor(slotType);
                var func2 = weaponPlayerAgent.GenerateBagEmptyKeyExtractor();
                var newAgent = (WeaponBaseAgent)Activator.CreateInstance(t,
                    func1, func2, slotType, grenadeHelper);
                slotWeaponAgents[(int)slotType] = newAgent;
            }
            return slotWeaponAgents[(int)slotType];
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
        public PlayerWeaponController()
        {
            weaponProcessor = new WeaponProcessUtil(this);
            slotWeaponAgents = new WeaponBaseAgent[GlobalConst.WeaponSlotMaxLength];
        
        }

        private void OnDerivedTypeInstanceProcess(Type t)
        {
            var attributes = Attribute.GetCustomAttributes(t, false);
            WeaponSpeciesAttribute speciesAttr;
            foreach (Attribute attr in attributes)
            {
                speciesAttr = attr as WeaponSpeciesAttribute;
               CreateWeaponAgent(speciesAttr.slotType, t);
              //  weaponAgentAssTypeDict.Add(speciesAttr.slotType, t);
            }
        }

        public void Initialize(EntityKey owner, WeaponPlayerComponentsAgent agent, GrenadeCacheHelper helper)
        {
            Owner = owner;
            grenadeHelper = helper;
            weaponPlayerAgent = agent;
            CommonUtil.ProcessDerivedTypes(typeof(WeaponBaseAgent), true, (Type t) => OnDerivedTypeInstanceProcess(t));
            //  int modeBagLength = ModeController.GetUsableWeapnBagLength(RelatedPlayerInfo);
            ////多个背包共享一份投掷武器代理
            //if (modeBagLength > 1)
            //{
            //    for (int i = 1; i < modeBagLength; i++)
            //        slotWeaponAgents[i, (int)EWeaponSlotType.ThrowingWeapon] = throwWeaponAgent;
            //}

        }

        public void ResetAllComponents()
        {
            if (RelatedOrient != null)
                RelatedOrient.Reset();
        }

        public void ResetBagLockState()
        {
            BagLockState = false;
            BagOpenLimitTIme = RelatedTime + SingletonManager.Get<GameModeConfigManager>().GetBagLimitTime(ModeController.ModeId);
        }


        public EntityKey Owner { get; private set; }

        public EntityKey EmptyWeaponKey
        {
            get { return weaponPlayerAgent.Customize.EmptyConstWeaponkey; }
        }

        public WeaponBaseAgent HeldWeaponAgent
        {
            get { return this[HeldSlotType]; }
        }
        public WeaponBaseAgent this[EWeaponSlotType slot]
        {
            get { return slotWeaponAgents[(int)slot]; }
        }

        public int HeldConfigId
        {
            get { return HeldWeaponAgent.ConfigId; }
        }

        public WeaponBaseAgent GetWeaponAgent(EWeaponSlotType slotType = EWeaponSlotType.Pointer)
        {
            if (slotType == EWeaponSlotType.Pointer) slotType = HeldSlotType;
            else if (slotType == EWeaponSlotType.LastPointer) slotType = LastSlotType;
            return this[slotType];
        }
        public WeaponBaseAgent GetWeaponAgent(int configId)
        {
            EWeaponSlotType slotType = weaponProcessor.GetMatchedSlot(configId);
            return this[slotType];

        }


        public EWeaponSlotType HeldSlotType
        {
            get { return (EWeaponSlotType)weaponPlayerAgent.BagSet.HeldSlotIndex; }
        }

        public bool IsHeldSlotEmpty
        {
            get { return !HeldWeaponAgent.IsValid(); }
        }

        public EWeaponSlotType LastSlotType
        {
            get
            {
                return (EWeaponSlotType)weaponPlayerAgent.BagSet.LastSlotIndex;
            }
        }

        public int HeldBagPointer
        {
            get { return 0; }
        }

        public int HeldBagPointer2
        {
            get { return weaponPlayerAgent.BagSet.HeldBagPointer2; }
            set { weaponPlayerAgent.BagSet.HeldBagPointer2 = value; }
        }


        public bool IsWeaponSlotEmpty(EWeaponSlotType slot)
        {
            return !GetWeaponAgent(slot).IsValid();
        }

        public bool IsHeldBagSlotType(EWeaponSlotType slot)
        {
            return slot == HeldSlotType;
        }

        public EWeaponSlotType PollGetLastSlotType(bool excludeLast = true)
        {
            if (!excludeLast)
            {
                EWeaponSlotType last = LastSlotType;
                if (last != EWeaponSlotType.None && !IsWeaponSlotEmpty(last))
                {
                    return last;
                }
            }
            for (EWeaponSlotType s = EWeaponSlotType.None + 1; s < EWeaponSlotType.Length; s++)
            {
                if (!IsWeaponSlotEmpty(s))
                    return s;
            }
            return EWeaponSlotType.None;

        }
      

        public int BagOpenLimitTIme
        {
            get { return weaponPlayerAgent.WeaponAux.BagOpenLimitTime; }
            set { weaponPlayerAgent.WeaponAux.BagOpenLimitTime = value; }
        }

        ///overridebag components
        public int OverrideBagTactic
        {
            get { return weaponPlayerAgent.WeaponUpdate.TacticWeapon; }
            set { weaponPlayerAgent.WeaponUpdate.TacticWeapon = value; }
        }

        public bool BagLockState
        {
            get { return weaponPlayerAgent.WeaponAux.BagLockState; }
            set { weaponPlayerAgent.WeaponAux.BagLockState = value; }
        }

        public bool CanSwitchWeaponBag
        {
            get { return ModeController.CanModeSwitchBag && !BagLockState && (BagOpenLimitTIme > RelatedTime); }
        }

//        public void PlayFireAudio()
//        {
//            if (!IsHeldSlotEmpty)
//                GameAudioMedia.PlayWeaponAudio(HeldConfigId, RelatedAppearence.WeaponHandObject(), (config) => config.Fire);
//        }
//        public void PlayPullBoltAudio()
//        {
//            if (!IsHeldSlotEmpty)
//                GameAudioMedia.PlayWeaponAudio(HeldConfigId, RelatedAppearence.WeaponHandObject(), (config) => config.PullBolt);
//        }
//        public void PlayReloadAudio()
//        {
//            if (!IsHeldSlotEmpty)
//                GameAudioMedia.PlayWeaponAudio(HeldConfigId, RelatedAppearence.WeaponHandObject(), (config) => config.ReloadStart);
//        }
        public OrientationComponent RelatedOrient
        {
            get { return weaponPlayerAgent.RelatedOrient; }
        }

        public FirePosition RelatedFirePos
        {
            get { return weaponPlayerAgent.RelatedFirePos; }
        }

        public int RelatedTime
        {
            get { return weaponPlayerAgent.RelatedTime; }
        }

        public CameraFinalOutputNewComponent RelatedCameraFinal
        {
            get { return weaponPlayerAgent.RelatedCameraFinal; }
        }

        public ThrowingActionInfo RelatedThrowActionInfo
        {
            get { return weaponPlayerAgent.RelatedThrowAction; }
        }

        public ICharacterState RelatedStateInterface
        {
            get { return weaponPlayerAgent.RelatedCharState; }
        }

        public ThrowingUpdateComponent RelatedThrowUpdate
        {

            get { return weaponPlayerAgent.RelatedThrowUpdate; }
        }

        public StatisticsData RelatedStatics
        {
            get { return weaponPlayerAgent.RelatedStatistics.Statistics; }
        }

        public CameraStateNewComponent RelatedCameraSNew
        {
            get { return weaponPlayerAgent.RelatedCameraSNew; }
        }

        public ICharacterAppearance RelatedAppearence
        {
            get { return weaponPlayerAgent.RelatedAappearence; }
        }

        public ICharacterBone RelatedBones
        {
            get { return weaponPlayerAgent.RelatedBones; }
        }

        public PlayerInfoComponent RelatedPlayerInfo
        {
            get { return weaponPlayerAgent.RelatedPlayerInfo; }
        }

        public PlayerMoveComponent RelatedPlayerMove
        {
            get { return weaponPlayerAgent.RelatedPlayerMove; }
        }

        public FreeData RelatedFreeData
        {
            get { return (FreeData)weaponPlayerAgent.RelatedFreeData; }
        }

        public PlayerEvents RelatedLocalEvents
        {
            get { return weaponPlayerAgent.RelatedLocalEvents; }
        }

        public PlayerWeaponAmmunitionComponent RelatedAmmunition
        {
            get { return weaponPlayerAgent.RelatedAmmunition; }
        }

        public void AddAuxEffect()
        {
            weaponPlayerAgent.AddAuxEffect();
        }
        public void AddAuxEffect(EClientEffectType effectType)
        {
            weaponPlayerAgent.AddAuxEffect(effectType);
        }
        public void AddAuxBullet()
        {
            weaponPlayerAgent.AddAuxBullet();
        }
        public void  AddAuxBullet(PlayerBulletData bulletData)
        {
            weaponPlayerAgent.AddAuxBullet(bulletData);
        }
        public int ForceInterruptGunSight { get { return weaponPlayerAgent.ForceInterruptGunSight; } set { weaponPlayerAgent.ForceInterruptGunSight = value; } }

        public bool? AutoThrowing { get { return weaponPlayerAgent.AutoThrowing; }set { weaponPlayerAgent.AutoThrowing = value.Value; } }

        public int? AutoFire
        { get { return weaponPlayerAgent.AutoFire; } set { weaponPlayerAgent.AutoFire = value; } }

        public List<PlayerBulletData> BulletList { get { return weaponPlayerAgent.WeaponAux.BulletList; } }

        public List<EClientEffectType> EffectList { get { return weaponPlayerAgent.WeaponAux.EffectList; } }

        public int GetReservedBullet(EBulletCaliber caliber)
        {
            return ModeController.GetReservedBullet(this, caliber);
        }

        public int GetReservedBullet()
        {
            return ModeController.GetReservedBullet(this, HeldSlotType);
        }

        public int GetReservedBullet(EWeaponSlotType slot)
        {
            if (slot.IsSlotWithBullet())
                return ModeController.GetReservedBullet(this, slot);
            return 0;
        }

        public bool IsWeaponInSlot(int configId)
        {
            EWeaponSlotType slotType = WeaponUtil.GetEWeaponSlotTypeById(configId);
            if (slotType == EWeaponSlotType.None) return false;
            return this[slotType].ConfigId == configId;
        }

    }
}
