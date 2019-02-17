using App.Shared.Components.Weapon;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.WeaponLogic.Attachment;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponComponentsAgent" />
    /// </summary>
    public class WeaponComponentsAgent
    {
        public static WeaponContext WeaponContext { get; set; }

        public static IPlayerWeaponConfigManager ConfigManager { private get; set; }

        private WeaponEntity entity;

        private ExpandWeaponLogicConfig configTempCache;

        private WeaponPartsStruct weaponPartsStructCache;
        private WeaponScanStruct weaponScanStructCache;


        public bool IsVailed()
        {
            return entity != null || entity.entityKey.Value == EntityKey.EmptyWeapon;
        }

        /// <summary>
        /// sync from event of playerEntiy.BagSet.WeaponSlot Component 
        /// </summary>
        /// <param name="entityKey"></param>
        internal void Sync(EntityKey entityKey)
        {
            entity = WeaponEntityFactory.GetWeaponEntity(WeaponContext, entityKey);
        }

        internal WeaponEntity Entity
        {
            get { return entity; }
        }
        internal EntityKey EntityKey { get { return IsVailed() ? entity.entityKey.Value : EntityKey.EmptyWeapon; } }

        internal void SetFlagWaitDestroy()
        {
            if (entity != null)
                entity.SetFlagWaitDestroy();
        }

        internal void SetFlagNoOwner()
        {
            if (entity != null)
                entity.SetFlagNoOwner();
        }

        public WeaponBasicDataComponent BaseComponent
        {
            get { return entity != null ? entity.weaponBasicData : null; }
        }

        public WeaponRuntimeDataComponent RunTimeComponent
        {
            get { return entity != null ? entity.weaponRuntimeData : null; }
        }

        public int GetFireModeCount()
        {
            if (entity != null)
                return SingletonManager.Get<WeaponDataConfigManager>().GetFireModeCountById(entity.weaponBasicData.ConfigId);
            return 1;
        }

        internal WeaponScanStruct? BaseComponentScan
        {
            get
            {
                if (entity != null)
                    return null;
                return entity.ToWeaponScan();
            }
        }

        public int? ConfigId
        {
            get
            {
                if (entity != null)
                    return null;
                return entity.weaponBasicData.ConfigId;
            }
        }

        public bool IsWeaponEmptyReload
        {
            get
            {
                if (entity == null)
                    return false;
                return SingletonManager.Get<WeaponConfigManager>().IsSpecialType(entity.weaponBasicData.ConfigId, ESpecialWeaponType.ReloadEmptyAlways);
            }
        }

        public ExpandWeaponLogicConfig WeaponLogicConfigAssy
        {
            get
            {
                if (entity == null) return null;
                weaponScanStructCache = entity.ToWeaponScan();
                return ConfigManager.GetWeaponLogicConfig(weaponScanStructCache.ConfigId, weaponPartsStructCache.Sync(weaponScanStructCache));

            }
        }

        public bool IsWeaponConfigStuffed(int weaponId)
        {
            if (entity == null) return false;
            return entity.weaponBasicData.ConfigId == weaponId;
        }
        public void ResetFireModel()
        {
            if (entity != null)
            {
                entity.ResetFireModel();
            }
        }

        public void ResetRuntimeData()
        {

            if (null == entity)
                return;
            entity.weaponRuntimeData.Accuracy = 0;
            entity.weaponRuntimeData.BurstShootCount = 0;
            entity.weaponRuntimeData.ContinuesShootCount = 0;
            entity.weaponRuntimeData.ContinuesShootDecreaseNeeded = false;
            entity.weaponRuntimeData.ContinuesShootDecreaseTimer = 0;
            entity.weaponRuntimeData.ContinuousAttackTime = 0;
            entity.weaponRuntimeData.LastBulletDir = UnityEngine.Vector3.zero;
            entity.weaponRuntimeData.LastFireTime = 0;
            entity.weaponRuntimeData.LastSpreadX = 0;
            entity.weaponRuntimeData.LastSpreadY = 0;
        }

        public void ResetParts()
        {
            if (null == entity)
                return;
            entity.weaponBasicData.LowerRail = 0;
            entity.weaponBasicData.UpperRail = 0;
            entity.weaponBasicData.Stock = 0;
            entity.weaponBasicData.Magazine = 0;
            entity.weaponBasicData.Muzzle = 0;
        }

        #region//curr weapon config shortcut
        public CommonFireConfig CommonFireCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.CommonFireCfg : null;

            }
        }

        public TacticWeaponLogicConfig TacticWeaponLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.TacticWeaponLogicCfg : null;

            }
        }

        public DefaultFireLogicConfig DefaultFireLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.DefaultFireLogicCfg : null;

            }
        }

        public DefaultWeaponLogicConfig DefaultWeaponLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.DefaultWeaponLogicCfg : null;

            }
        }

        public PistolAccuracyLogicConfig PistolAccuracyLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.PistolAccuracyLogicCfg : null;

            }
        }

        public BaseAccuracyLogicConfig BaseAccuracyLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.BaseAccuracyLogicCfg : null;

            }
        }

        public FixedSpreadLogicConfig FixedSpreadLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.FixedSpreadLogicCfg : null;

            }
        }

        public PistolSpreadLogicConfig PistolSpreadLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.PistolSpreadLogicCfg : null;

            }
        }

        public ShotgunSpreadLogicConfig ShotgunSpreadLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.ShotgunSpreadLogicCfg : null;

            }
        }

        public RifleSpreadLogicConfig RifleSpreadLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.RifleSpreadLogicCfg : null;

            }
        }

        public SniperSpreadLogicConfig SniperSpreadLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.SniperSpreadLogicCfg : null;

            }
        }

        public RifleKickbackLogicConfig RifleKickbackLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.RifleKickbackLogicCfg : null;

            }
        }

        public FixedKickbackLogicConfig FixedKickbackLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.FixedKickbackLogicCfg : null;

            }
        }

        public DefaultFireModeLogicConfig DefaultFireModeLogicCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.DefaultFireModeLogicCfg : null;

            }
        }

        public RifleFireCounterConfig RifleFireCounterCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.RifleFireCounterCfg : null;

            }
        }

        public BulletConfig BulletCfg
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.BulletCfg : null;

            }
        }

        public int MagazineCapacity
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.CommonFireCfg.MagazineCapacity : 0;
            }
        }

        public float BreathFactor
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.GetBreathFactor() : 1;
            }
        }

        public float ReloadSpeed
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.GetReloadSpeed() : 1;
            }
        }

        public float BaseSpeed
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.GetSpeed() : 1;
            }
        }

        public float BaseFov
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.DefaultFireLogicCfg.Fov : 90;
            }
        }

        public bool CanWeaponSight
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null && configTempCache.DefaultFireLogicCfg != null;

            }
        }

        public float FallbackOffsetFactor
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.FixedKickbackLogicCfg.FallbackOffsetFactor : 0;

            }
        }

        public float FocusSpeed
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                return configTempCache != null ? configTempCache.GetFocusSpeed() : 1;
            }
        }

        public float GetGameFov(bool InShiftState)
        {
            if (entity == null || IsFovModified) return BaseFov;
            if (InShiftState)
            {
                var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(ConfigId.Value);
                if (weaponCfg != null) return weaponCfg.ShiftFov;
            }
            return BaseFov;
        }

        public bool IsFovModified
        {
            get
            {
                configTempCache = WeaponLogicConfigAssy;
                if (configTempCache == null) return false;
                DefaultFireLogicConfig srcConfig = SingletonManager.Get<WeaponDataConfigManager>().GetFireLogicConfig(ConfigId.Value);
                return srcConfig.Fov != configTempCache.GetGunSightFov();
            }
        }


        #endregion
    }
}