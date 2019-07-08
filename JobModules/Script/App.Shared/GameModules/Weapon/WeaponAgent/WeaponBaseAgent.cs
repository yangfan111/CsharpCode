using System;
using App.Shared.Audio;
using App.Shared.Components.Weapon;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    ///     Defines the <see cref="WeaponBaseAgent" />
    /// </summary>
    public abstract class WeaponBaseAgent
    {
        private readonly WeaponPartsAchive partsAchiveCache = new WeaponPartsAchive();

        private WeaponAllConfigs configCache;

        protected Func<EntityKey> emptyKeyExtractor;

        //      public static IPlayerWeaponResourceConfigManager ConfigManager { protected get; set; }
        protected WeaponEntity entityCache;


        protected EWeaponSlotType handledSlot;

        protected Func<EntityKey> weaponKeyExtractor;

        public WeaponBaseAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor,
                               EWeaponSlotType slot, GrenadeCacheHandler grenadeHandler)
        {
            weaponKeyExtractor = in_holdExtractor;
            emptyKeyExtractor  = in_emptyExtractor;
            handledSlot        = slot;
        }

        protected virtual WeaponEntity Entity
        {
            get
            {
                if (entityCache == null || !entityCache.hasOwnerId || entityCache.ownerId.Value != WeaponKey)
                    entityCache = WeaponEntityFactory.GetWeaponEntity(WeaponKey);
                return entityCache;
            }
        }

        public virtual EntityKey WeaponKey
        {
            get { return weaponKeyExtractor(); }
        }

        internal EntityKey EmptyWeaponKey
        {
            get { return emptyKeyExtractor(); }
        }

        internal WeaponPartsStruct PartsScan
        {
            get
            {
                SyncParts();
                return WeaponPartUtil.CreateParts(partsAchiveCache);
            }
        }

        public EntityKey Owner { get; set; }

        internal virtual bool CanApplyPart
        {
            get { return IsValid(); }
        }

        public WeaponBasicDataComponent BaseComponent
        {
            get { return Entity != null ? entityCache.weaponBasicData : WeaponBasicDataComponent.Empty; }
        }

        public WeaponRuntimeDataComponent RunTimeComponent
        {
            get { return Entity != null ? entityCache.weaponRuntimeData : WeaponRuntimeDataComponent.Empty; }
        }


        public int FireModeCount
        {
            get
            {
                if (IsValid())
                    return WeaponConfigAssy.FireModeCount;
                return 1;
            }
        }

        public WeaponScanStruct ComponentScan
        {
            get
            {
                if (Entity != null)
                {
                    SyncParts();
                    return entityCache.ToWeaponScan(partsAchiveCache);
                }

                return WeaponScanStruct.Empty;
            }
        }

        public virtual int ConfigId
        {
            get
            {
                if (IsValid())
                    return entityCache.weaponBasicData.ConfigId;
                return WeaponUtil.EmptyHandId;
            }
        }

        public bool IsWeaponEmptyReload
        {
            get
            {
                if (!IsValid())
                    return false;
                return SingletonManager.Get<WeaponResourceConfigManager>()
                                .IsSpecialType(ConfigId, ESpecialWeaponType.ReloadEmptyAlways);
            }
        }

        public WeaponAllConfigs WeaponConfigAssy
        {
            get
            {
                if (configCache == null || configCache.S_Id != ConfigId)
                    configCache = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);

                return configCache;
            }
        }


        public bool HasSilencerPart
        {
            get
            {
                return WeaponConfigAssy.S_IsSilence || SingletonManager.Get<WeaponPartsConfigManager>()
                                .GetPartAchiveAttachedAttributeByType(BaseComponent.Muzzle,
                                    WeaponAttributeType.FireSound) > 0;
            }
        }


        public CommonFireConfig CommonFireCfg
        {
            get { return WeaponConfigAssy.S_CommonFireCfg; }
        }

        public TacticWeaponBehaviorConfig TacticWeaponLogicCfg
        {
            get { return WeaponConfigAssy.S_TacticBehvior; }
        }

        public DefaultFireLogicConfig DefaultFireLogicCfg
        {
            get { return WeaponConfigAssy.S_DefaultFireLogicCfg; }
        }


        public DefaultWeaponBehaviorConfig DefaultWeaponLogicCfg
        {
            get { return WeaponConfigAssy.S_DefualtBehavior; }
        }

        public PistolAccuracyLogicConfig PistolAccuracyLogicCfg
        {
            get { return WeaponConfigAssy.S_PistolAccuracyLogicCfg; }
        }


        public BaseAccuracyLogicConfig BaseAccuracyLogicCfg
        {
            get { return WeaponConfigAssy.S_BaseAccuracyLogicCfg; }
        }


        public FixedSpreadLogicConfig FixedSpreadLogicCfg
        {
            get { return WeaponConfigAssy.S_FixedSpreadLogicCfg; }
        }


        public PistolSpreadLogicConfig PistolSpreadLogicCfg
        {
            get { return WeaponConfigAssy.S_PistolSpreadLogicCfg; }
        }


        public ShotgunSpreadLogicConfig ShotgunSpreadLogicCfg
        {
            get { return WeaponConfigAssy.S_ShotgunSpreadLogicCfg; }
        }


        public RifleSpreadLogicConfig RifleSpreadLogicCfg
        {
            get { return WeaponConfigAssy.S_RifleSpreadLogicCfg; }
        }

        public SniperSpreadLogicConfig SniperSpreadLogicCfg
        {
            get { return WeaponConfigAssy.S_SniperSpreadLogicCfg; }
        }

        public RifleShakeConfig RifleShakeCfg
        {
            get { return WeaponConfigAssy.SRifleShakeCfg; }
        }


        public FixedShakeConfig FixedShakeCfg
        {
            get { return WeaponConfigAssy.SFixedShakeCfg; }
        }


        public DefaultFireModeLogicConfig DefaultFireModeLogicCfg
        {
            get { return WeaponConfigAssy.S_DefaultFireModeLogicCfg; }
        }


        public WeaponResConfigItem ResConfig
        {
            get { return WeaponConfigAssy.NewWeaponCfg; }
        }

        public RifleFireCounterConfig RifleFireCounterCfg
        {
            get { return WeaponConfigAssy.S_RifleFireCounterCfg; }
        }

        public BulletConfig BulletCfg
        {
            get { return WeaponConfigAssy.S_BulletCfg; }
        }

        public FireRollConfig FireRollCfg
        {
            get { return WeaponConfigAssy.S_FireRollCfg; }
        }

        public int MagazineCapacity
        {
            get { return CommonFireCfg != null ? MagazineCapacity : 0; }
        }

        public float BreathFactor
        {
            get { return WeaponConfigAssy != null ? WeaponConfigAssy.GetBreathFactor() : 1; }
        }

        public float ReloadSpeed
        {
            get
            {
                if (!IsValid())
                    return 1f;
                var baseReloadSpeed = WeaponConfigAssy.GetReloadSpeed();
                SyncParts();
                var partReloadSpeed = SingletonManager.Get<WeaponPartsConfigManager>()
                                .GetPartAchiveAttachedAttributeByType(partsAchiveCache.Magazine,
                                    WeaponAttributeType.ReloadSpeed);
                if (partReloadSpeed <= 0) partReloadSpeed = 1f;
                return partReloadSpeed * baseReloadSpeed;
            }
        }

        public float BaseSpeed
        {
            get { return WeaponConfigAssy != null ? WeaponConfigAssy.S_Speed : DefaultSpeed; }
        }


        public float DefaultSpeed
        {
            get
            {
                var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(WeaponUtil.EmptyHandId);
                return config.S_Speed;
            }
        }

        //   public float BaseFov { get { return DefaultFireLogicCfg != null ? DefaultFireLogicCfg.Fov : GlobalConst.AimDefaultFov; } }


        public bool CanWeaponSight
        {
            get { return DefaultFireLogicCfg != null; }
        }


        public float FallbackOffsetFactor
        {
            get
            {
                if (FixedShakeCfg != null) return FixedShakeCfg.FallbackOffsetFactor;
                return 0f;
            }
        }

        public float CmrFocusSpeed
        {
            get { return WeaponConfigAssy != null ? WeaponConfigAssy.GetFocusSpeed() : 0f; }
        }

        //public bool IsFovModified { get { return DefaultFireLogicCfg != null && DefaultFireLogicCfg.Fov != WeaponConfigAssy.GetGunSightFov(); } }
        public EBulletCaliber Caliber
        {
            get
            {
                return WeaponConfigAssy != null
                                ? (EBulletCaliber) WeaponConfigAssy.NewWeaponCfg.Caliber
                                : EBulletCaliber.Length;
            }
        }

        public abstract int  FindNextWeapon(bool autoStuff);
        public abstract bool ExpendWeapon(int reservedBullet);
        public abstract void ReleaseWeapon();

        public abstract WeaponEntity ReplaceWeapon(EntityKey Owner, WeaponScanStruct orient,
                                                   ref WeaponPartsRefreshStruct refreshParams);

        public virtual bool IsValid()
        {
            return WeaponKey != EmptyWeaponKey && Entity != null;
        }

        /// <summary>
        ///     sync from event of playerEntiy.BagSet.WeaponSlot Component
        /// </summary>
        /// <param name="entityKey"></param>
        //internal void Sync(EntityKey entityKey)
        //{
        //    if (entityKey == EntityKey.Default)
        //        weaponEntity = WeaponUtil.EmptyWeapon;
        //    else
        //        weaponEntity = WeaponEntityFactory.GetWeaponEntity( entityKey);
        //    WeaponConfigAssy = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);
        //}
        internal WeaponEntity GetEntity()
        {
            if (IsValid()) return entityCache;
            return null;
        }

        //        public WeaponClientUpdateComponent ClientUpdateComponent
        //        {
        //            get
        //            {
        //                if (Entity == null) return WeaponClientUpdateComponent.Empty;
        //                if (!entityCache.hasWeaponClientUpdate)
        //                    entityCache.AddWeaponClientUpdate();
        //                return entityCache.weaponClientUpdate;
        //            }
        //        }
        public void InterruptPullBolt()
        {
            if (RunTimeComponent.PullBoltFinish)
            {
                RunTimeComponent.PullBoltInterrupt = false;
                return;

            }
            if (GlobalConst.EnableWeaponLog)
                DebugUtil.MyLog("Do Interrupt :" + ConfigId);
            RunTimeComponent.PullBoltInterrupt = true;
            var audioController = GameModuleManagement.Get<PlayerAudioControllerBase>(Owner.EntityId);
            audioController.StopPullBoltAudio(ConfigId);
        }

        public void SaveDynamic()
        {
            InterruptPullBolt();
        }


        public void SyncParts()
        {
            if (IsValid())
            {
                partsAchiveCache.CloneFrom(WeaponConfigAssy.DefaultParts);
                partsAchiveCache.ApplyParts(BaseComponent);
            }
        }

        /// <summary>
        ///     call syncParts before usage
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public float GetAttachedAttributeByType(WeaponAttributeType attributeType)
        {
            return SingletonManager.Get<WeaponPartsConfigManager>()
                            .GetPartAchiveAttachedAttributeByType(partsAchiveCache, attributeType);
        }

        public bool IsWeaponConfigStuffed(int weaponId)
        {
            if (!IsValid()) return false;
            return entityCache.weaponBasicData.ConfigId == weaponId;
        }

        public void ResetRuntimeData()
        {
            if (!IsValid())
                return;
            entityCache.weaponRuntimeData.Accuracy                      = 0;
            entityCache.weaponRuntimeData.BurstShootCount               = 0;
            entityCache.weaponRuntimeData.ContinuesShootCount           = 0;
            entityCache.weaponRuntimeData.ContinuesShootReduceTimestamp = 0;
            entityCache.weaponRuntimeData.ContinueAttackEndStamp        = 0;
            entityCache.weaponRuntimeData.ContinueAttackStartStamp      = 0;
            entityCache.weaponRuntimeData.NextAttackPeriodStamp         = 0;
            entityCache.weaponRuntimeData.LastBulletDir                 = Vector3.zero;
            entityCache.weaponRuntimeData.LastAttackTimestamp           = 0;
            entityCache.weaponRuntimeData.LastSpreadX                   = 0;
            entityCache.weaponRuntimeData.LastSpreadY                   = 0;
            entityCache.weaponRuntimeData.PullBoltInterrupt             = false;
            entityCache.weaponRuntimeData.NeedAutoReload                = false;
            entityCache.weaponRuntimeData.PullBoltFinish                = true;
        }

        public void ResetParts()
        {
            if (!IsValid())
                return;
            entityCache.weaponBasicData.LowerRail = 0;
            entityCache.weaponBasicData.UpperRail = 0;
            entityCache.weaponBasicData.Stock     = 0;
            entityCache.weaponBasicData.Magazine  = 0;
            entityCache.weaponBasicData.Muzzle    = 0;
            entityCache.weaponBasicData.SideRail  = 0;
            entityCache.weaponBasicData.Bore      = 0;
            entityCache.weaponBasicData.Feed      = 0;
            entityCache.weaponBasicData.Trigger   = 0;
            entityCache.weaponBasicData.Interlock = 0;
            entityCache.weaponBasicData.Brake     = 0;
        }
        //public float GetOpenedFov()
        //{
        //    float attachFov = 0;
        //    if (BaseComponent.UpperRail > 0)
        //        attachFov = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(BaseComponent.UpperRail).Fov;
        //    return attachFov + BaseFov;
        //}

        public float GetGameFov(bool InShiftState)
        {
            if (IsValid() && ResConfig != null)
            {
                float upperFov = 0;
                if (BaseComponent.UpperRail > 0)
                    upperFov = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(BaseComponent.UpperRail)
                                    .Fov;
                if (InShiftState)
                {
                    if (upperFov > 0 && upperFov != ResConfig.AimFov)
                        return upperFov;
                    if (ResConfig != null)
                        return ResConfig.ShiftFov;
                }

                return upperFov > 0 ? upperFov : ResConfig.AimFov;
            }

            return 0;
        }

        public string[] GetDOFParams()
        {
            if (IsValid() && ResConfig != null)
            {
                string[] DOFParameter;
                if (BaseComponent.UpperRail > 0)
                    DOFParameter = SingletonManager.Get<WeaponPartsConfigManager>()
                                    .GetConfigById(BaseComponent.UpperRail).GetDOFParameters;
                else
                    DOFParameter = ResConfig.GetDOFParameters;
                return DOFParameter;
            }

            return null;
            //                if (InShiftState)
            //                {
            //                    if (DOFParameter != null && DOFParameter.Count>0)
            //                        return true;
            //                    DOFParameter = ResConfig.DOFParameter;
            //                    return true;
            //                }
            //                if(DOFParameter == null || DOFParameter.Count ==0)
        }
    }
}