using App.Shared.Components.Weapon;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.WeaponLogic.Attachment;
using System;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponComponentsAgent" />
    /// </summary>
    public class WeaponComponentsAgent
    {

  //      public static IPlayerWeaponResourceConfigManager ConfigManager { private get; set; }

        private WeaponEntity Entity
        {
            get { return  WeaponEntityFactory.GetWeaponEntity(WeaponKey); }
        }
        internal EntityKey WeaponKey { get { return weaponKeyExtractor(); } }

        internal EntityKey EmptyWeaponKey { get { return emptyKeyExtractor(); } }

        private Func<EntityKey> weaponKeyExtractor;

        private Func<EntityKey> emptyKeyExtractor;

        public WeaponComponentsAgent(Func<EntityKey> in_extractor,Func<EntityKey> in_emptyExtractor)
        {
            weaponKeyExtractor = in_extractor;
            emptyKeyExtractor = in_emptyExtractor;
        }

        public bool IsVailed()
        {
            return WeaponKey != EmptyWeaponKey && Entity != null ;
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
                if (IsVailed()) return Entity;
                return null;
        }

        internal void SetFlagWaitDestroy()
        {
            if (IsVailed())
                Entity.SetFlagWaitDestroy();
        }

        internal void SetFlagNoOwner()
        {
            if (IsVailed())
                Entity.SetFlagNoOwner();
        }

        public WeaponBasicDataComponent BaseComponent
        {
            get { return IsVailed() ? Entity.weaponBasicData : WeaponUtil.EmptyWeaponBase; }
        }

        public WeaponRuntimeDataComponent RunTimeComponent
        {
            get { return IsVailed() ? Entity.weaponRuntimeData : WeaponUtil.EmptyRun; }
        }

        public int FireModeCount
        {
            get
            {
                if (IsVailed())
                    return WeaponConfigAssy.FireModeCount;
                return 1;
            }
        }

        public WeaponScanStruct BaseComponentScan
        {
            get{return Entity.ToWeaponScan();}
        }

        public int ConfigId
        {
            get { return Entity.weaponBasicData.ConfigId; }
        }

        public bool IsWeaponEmptyReload
        {
            get
            {
                if (!IsVailed())
                    return false;
                return SingletonManager.Get<WeaponResourceConfigManager>().IsSpecialType(ConfigId, ESpecialWeaponType.ReloadEmptyAlways);
            }
        }

        public WeaponAllConfigs WeaponConfigAssy
        {
            get {
                if (IsVailed())
                    return SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);
                return SingletonManager.Get<WeaponConfigManagement>().FindConfigById(WeaponUtil.EmptyHandId);
            }
        }
    

        public bool IsWeaponConfigStuffed(int weaponId)
        {
            if (!IsVailed()) return false;
            return Entity.weaponBasicData.ConfigId == weaponId;
        }

     

        public void Reset()
        {
            ResetRuntimeData();
            ResetParts();
        }

        public void ResetRuntimeData()
        {

            if (!IsVailed())
                return;
            Entity.weaponRuntimeData.Accuracy = 0;
            Entity.weaponRuntimeData.BurstShootCount = 0;
            Entity.weaponRuntimeData.ContinuesShootCount = 0;
            Entity.weaponRuntimeData.ContinuesShootDecreaseNeeded = false;
            Entity.weaponRuntimeData.ContinuesShootDecreaseTimer = 0;
            Entity.weaponRuntimeData.ContinueAttackEndStamp = 0;
            Entity.weaponRuntimeData.ContinueAttackStartStamp = 0;
            Entity.weaponRuntimeData.NextAttackPeriodStamp= 0;
            Entity.weaponRuntimeData.LastBulletDir = UnityEngine.Vector3.zero;
            Entity.weaponRuntimeData.LastFireTime = 0;
            Entity.weaponRuntimeData.LastSpreadX = 0;
            Entity.weaponRuntimeData.LastSpreadY = 0;
        }

        public void ResetParts()
        {
             if (!IsVailed())
                return;
            Entity.weaponBasicData.LowerRail = 0;
            Entity.weaponBasicData.UpperRail = 0;
            Entity.weaponBasicData.Stock = 0;
            Entity.weaponBasicData.Magazine = 0;
            Entity.weaponBasicData.Muzzle = 0;
        }

        public CommonFireConfig CommonFireCfg                     { get { return WeaponConfigAssy.S_CommonFireCfg; } }
        
        public TacticWeaponBehaviorConfig TacticWeaponLogicCfg       { get { return WeaponConfigAssy.S_TacticBehvior; } }

        public DefaultFireLogicConfig DefaultFireLogicCfg         { get { return WeaponConfigAssy.S_DefaultFireLogicCfg; } }
  

        public DefaultWeaponBehaviorConfig DefaultWeaponLogicCfg     { get { return WeaponConfigAssy.S_DefualtBehavior; } }
 
        public PistolAccuracyLogicConfig PistolAccuracyLogicCfg   { get { return WeaponConfigAssy.S_PistolAccuracyLogicCfg; } }


        public BaseAccuracyLogicConfig BaseAccuracyLogicCfg       { get { return WeaponConfigAssy.S_BaseAccuracyLogicCfg; } }
  

        public FixedSpreadLogicConfig FixedSpreadLogicCfg         { get { return WeaponConfigAssy.S_FixedSpreadLogicCfg; } }
    

        public PistolSpreadLogicConfig PistolSpreadLogicCfg       { get { return WeaponConfigAssy.S_PistolSpreadLogicCfg; } }


        public ShotgunSpreadLogicConfig ShotgunSpreadLogicCfg     { get { return WeaponConfigAssy.S_ShotgunSpreadLogicCfg; } }
       

        public RifleSpreadLogicConfig RifleSpreadLogicCfg         { get { return WeaponConfigAssy.S_RifleSpreadLogicCfg; } }

        public SniperSpreadLogicConfig SniperSpreadLogicCfg       { get { return WeaponConfigAssy.S_SniperSpreadLogicCfg; } }
     
        public RifleKickbackLogicConfig RifleKickbackLogicCfg     { get { return WeaponConfigAssy.S_RifleKickbackLogicCfg; } }
     
        public FixedKickbackLogicConfig FixedKickbackLogicCfg     { get { return WeaponConfigAssy.S_FixedKickbackLogicCfg; } }
  

        public DefaultFireModeLogicConfig DefaultFireModeLogicCfg { get { return WeaponConfigAssy.S_DefaultFireModeLogicCfg; } }
          
    
        public WeaponResConfigItem ResConfig                      { get { return WeaponConfigAssy.WeaponResCfg(); } }
        public RifleFireCounterConfig RifleFireCounterCfg         { get { return WeaponConfigAssy.S_RifleFireCounterCfg; } }

        public BulletConfig BulletCfg                             { get { return WeaponConfigAssy.S_BulletCfg; } }
    
        public int MagazineCapacity                               { get { return CommonFireCfg != null ? MagazineCapacity : 0; } }
     
        public float BreathFactor                                 { get { return WeaponConfigAssy != null? WeaponConfigAssy.GetBreathFactor():1; } }
     
        public float ReloadSpeed                                  { get { return WeaponConfigAssy != null ? WeaponConfigAssy.GetReloadSpeed():1; } }

        public float BaseSpeed                                    { get { return WeaponConfigAssy != null ? WeaponConfigAssy.S_Speed : DefaultSpeed; } }


        public float DefaultSpeed
        {
            get
            {
                var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(WeaponUtil.EmptyHandId);
                return config.S_Speed;
            }
        }

        public float BaseFov                                      { get { return DefaultFireLogicCfg != null ? DefaultFireLogicCfg.Fov : 90; } }
     

        public bool CanWeaponSight                                { get { return DefaultFireLogicCfg != null; } }
        

        public float FallbackOffsetFactor                         { get { return FixedKickbackLogicCfg != null? FixedKickbackLogicCfg.FallbackOffsetFactor:0f; } }
        
        public float FocusSpeed                                   { get { return WeaponConfigAssy != null ? WeaponConfigAssy.GetFocusSpeed() : 0f; } }
     
        public bool IsFovModified                                 { get { return DefaultFireLogicCfg!= null && DefaultFireLogicCfg.Fov != WeaponConfigAssy.GetGunSightFov(); } }

        public float GetGameFov(bool InShiftState)
        {
            if (!IsVailed() || IsFovModified) return BaseFov;
            if (InShiftState)
            {

                
                if (ResConfig != null) return ResConfig.ShiftFov;
            }
            return BaseFov;
        }

   
    }
}
