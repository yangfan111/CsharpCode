using App.Shared.Components.Weapon;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.Utils;
using System;
using System.Linq;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponBaseAgent" />
    /// </summary>
    public abstract class WeaponBaseAgent
    {
        //      public static IPlayerWeaponResourceConfigManager ConfigManager { protected get; set; }
        protected WeaponEntity entityCache;

        protected virtual WeaponEntity Entity
        {
            get
            {
                entityCache = WeaponEntityFactory.GetWeaponEntity(WeaponKey);
                return entityCache;
            }
        }

        internal virtual EntityKey WeaponKey
        {
            get { return weaponKeyExtractor(); }
        }

        internal EntityKey EmptyWeaponKey
        {
            get { return emptyKeyExtractor(); }
        }

        protected Func<EntityKey> weaponKeyExtractor;

        protected Func<EntityKey> emptyKeyExtractor;


        protected EWeaponSlotType handledSlot;

        internal WeaponPartsStruct PartsScan
        {
            get
            {
                SyncParts();
                return WeaponPartUtil.CreateParts(partsAchiveCache);
            }
        }

        public abstract int FindNextWeapon(bool autoStuff);
        public abstract bool ExpendWeapon();
        public abstract void ReleaseWeapon();

        public abstract WeaponEntity ReplaceWeapon(EntityKey Owner, WeaponScanStruct orient,
            ref WeaponPartsRefreshStruct refreshParams);

        public WeaponBaseAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor,
            EWeaponSlotType slot, GrenadeCacheHandler grenadeHandler)
        {
            weaponKeyExtractor = in_holdExtractor;
            emptyKeyExtractor = in_emptyExtractor;
            handledSlot = slot;
        }

        public virtual bool IsValid()
        {
            return WeaponKey != EmptyWeaponKey && Entity != null;
        }

        /// <summary>
        /// sync from event of playerEntiy.BagSet.WeaponSlot Component 
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

        public WeaponBasicDataComponent BaseComponent
        {
            get { return Entity != null ? entityCache.weaponBasicData : WeaponBasicDataComponent.Empty; }
        }

        public WeaponRuntimeDataComponent RunTimeComponent
        {
            get { return Entity != null ? entityCache.weaponRuntimeData : WeaponRuntimeDataComponent.Empty; }
        }
        public WeaponClientSyncComponent ClientSyncComponent
        {
            get
            {
                if(Entity == null  )return WeaponClientSyncComponent.Empty;
                if(!entityCache.hasWeaponClientSync)
                    entityCache.AddWeaponClientSync();
                return entityCache.weaponClientSync;
            }
        }
       
        public void ResetDynamic()
        {
            if (IsValid())
            {
                if(!entityCache.hasWeaponClientSync)
                    entityCache.AddWeaponClientSync();
                entityCache.weaponClientSync.IsPullingBolt = false;
//                entityCache.weaponClientSync.IsInterruptSightView = false;
//                entityCache.weaponClientSync.IsRecoverSightView = false;

            }
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

        private WeaponAllConfigs configCache;

        public WeaponAllConfigs WeaponConfigAssy
        {
            get
            {
                if (configCache == null || configCache.S_Id != ConfigId)
                    configCache = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);
                return configCache;
            }
        }

        private readonly WeaponPartsAchive partsAchiveCache = new WeaponPartsAchive();


        public void SyncParts()
        {
            if (IsValid())
            {
                partsAchiveCache.CloneFrom(WeaponConfigAssy.DefaultParts);
                partsAchiveCache.ApplyParts(BaseComponent);
            }
        }

        /// <summary>
        /// call syncParts before usage
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public float GetAttachedAttributeByType(WeaponAttributeType attributeType)
        {
            return SingletonManager.Get<WeaponPartsConfigManager>()
                .GetPartAchiveAttachedAttributeByType(partsAchiveCache, attributeType);
        }

      
        public bool HasSilencerPart
        {
            get
            {
                return GlobalConst.SilencerWeapons.Contains(ConfigId) || SingletonManager.Get<WeaponPartsConfigManager>().GetPartAchiveAttachedAttributeByType
                    (BaseComponent.Muzzle, WeaponAttributeType.FireSound) > 0;
            }
        }

        public bool IsWeaponConfigStuffed(int weaponId)
        {
            if (!IsValid()) return false;
            return entityCache.weaponBasicData.ConfigId == weaponId;
        }


        //public void Reset()
        //{
        //    ResetRuntimeData();
        //    ResetParts();
        //}

        public void ResetRuntimeData()
        {
            if (!IsValid())
                return;
            entityCache.weaponRuntimeData.Accuracy = 0;
            entityCache.weaponRuntimeData.BurstShootCount = 0;
            entityCache.weaponRuntimeData.ContinuesShootCount = 0;
            entityCache.weaponRuntimeData.ContinuesShootReduceTimestamp = 0;
            entityCache.weaponRuntimeData.ContinueAttackEndStamp = 0;
            entityCache.weaponRuntimeData.ContinueAttackStartStamp = 0;
            entityCache.weaponRuntimeData.NextAttackPeriodStamp = 0;
            entityCache.weaponRuntimeData.LastBulletDir = UnityEngine.Vector3.zero;
            entityCache.weaponRuntimeData.LastAttackTimestamp = 0;
            entityCache.weaponRuntimeData.LastSpreadX = 0;
            entityCache.weaponRuntimeData.LastSpreadY = 0;
            ResetDynamic();
        }

        public void ResetParts()
        {
            if (!IsValid())
                return;
            entityCache.weaponBasicData.LowerRail = 0;
            entityCache.weaponBasicData.UpperRail = 0;
            entityCache.weaponBasicData.Stock = 0;
            entityCache.weaponBasicData.Magazine = 0;
            entityCache.weaponBasicData.Muzzle = 0;
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
            get { return WeaponConfigAssy != null ? WeaponConfigAssy.GetReloadSpeed() : 1; }
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
                    ? (EBulletCaliber)WeaponConfigAssy.NewWeaponCfg.Caliber
                    : EBulletCaliber.Length;
            }
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
    }
}