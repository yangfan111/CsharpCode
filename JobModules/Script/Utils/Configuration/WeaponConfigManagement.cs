using Assets.XmlConfig;
using System.Collections.Generic;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace Assets.Utils.Configuration
{
    public class WeaponAllConfigs
    {
        /// <summary>
        /// 总配置
        /// </summary>
        public readonly WeaponConfig InitWeaponAllConfig;


        public readonly bool HasLogicConfig;

        public List<AnimatorStateItem> AniStates
        {
            get { return InitWeaponAllConfig.AnimatorStateTimes; }
        }

        /// origin related configs
        public string S_Name
        {
            get { return InitWeaponAllConfig.Name; }
        }

        public int S_Id
        {
            get { return InitWeaponAllConfig.Id; }
        }

        public bool S_IsSilence
        {
            get { return InitAbstractBehavior.IsSilence == 1; }
        }

        public WeaponEffectConfig S_EffectConfig
        {
            get
            {
                if (InitAbstractBehavior != null)
                    return InitAbstractBehavior.EffectConfig;
                return null;
            }
        }

        public float S_Speed
        {
            get
            {
                var val = InitAbstractBehavior != null ? InitAbstractBehavior.MaxSpeed : 0;
                return Mathf.Max(val, 1);
            }
        }

        public bool S_CantRun
        {
            get { return InitAbstractBehavior != null ? InitAbstractBehavior.CantRun : false; }
        }

        public EFireMode[] S_FireModelArr
        {
            get
            {
                if (S_DefaultFireModeLogicCfg != null)
                    return S_DefaultFireModeLogicCfg.AvaliableModes;
                return null;
            }
        }

        public int[] S_AttachmentConfig
        {
            get
            {
                if (InitAbstractBehavior != null)
                    return InitAbstractBehavior.AttachmentConfig;
                return null;
            }
        }

        public Ragdoll S_Ragdoll
        {
            get
            {
                if (InitAbstractBehavior != null)
                    return InitAbstractBehavior.Ragdoll;
                return null;
            }
        }

        /// <summary>
        /// behavior layer
        /// base:WeaponAbstractBehavior,ext :DoubleWeaponBehaviorConfig ,ext :TacticWeaponBehaviorConfig ,ext:DefaultWeaponBehaviorConfig 
        /// </summary>
        public readonly WeaponAbstractBehavior InitAbstractBehavior;

        public DoubleWeaponBehaviorConfig  S_DoubleBehavior  { get; private set; }
        public TacticWeaponBehaviorConfig  S_TacticBehvior   { get; private set; }
        public DefaultWeaponBehaviorConfig S_DefualtBehavior { get; private set; }


        /// <summary>
        /// logic layer
        /// base:InitAbstractLogicConfig,ext :ThrowingFireLogicConfig ,ext :DefaultFireLogicConfig ,ext:MeleeFireLogicConfig 
        /// </summary>
        public DefaultWeaponAbstractFireFireLogicConfig InitAbstractLogicConfig { get; private set; }

        public DefaultFireLogicConfig S_DefaultFireLogicCfg { get; private set; }

        /// <summary>
        /// sub logic layer
        /// DefaultFireLogicConfig sub config
        /// </summary>
        public RifleFireCounterConfig S_RifleFireCounterCfg { get; private set; }

        public BulletConfig               S_BulletCfg               { get; private set; }
        public CommonFireConfig           S_CommonFireCfg           { get; private set; }
        public PistolAccuracyLogicConfig  S_PistolAccuracyLogicCfg  { get; private set; }
        public BaseAccuracyLogicConfig    S_BaseAccuracyLogicCfg    { get; private set; }
        public FixedSpreadLogicConfig     S_FixedSpreadLogicCfg     { get; private set; }
        public PistolSpreadLogicConfig    S_PistolSpreadLogicCfg    { get; private set; }
        public ShotgunSpreadLogicConfig   S_ShotgunSpreadLogicCfg   { get; private set; }
        public RifleSpreadLogicConfig     S_RifleSpreadLogicCfg     { get; private set; }
        public SniperSpreadLogicConfig    S_SniperSpreadLogicCfg    { get; private set; }
        public RifleShakeConfig           SRifleShakeCfg            { get; private set; }
        public FixedShakeConfig           SFixedShakeCfg            { get; private set; }
        public DefaultFireModeLogicConfig S_DefaultFireModeLogicCfg { get; private set; }
        public FireRollConfig S_FireRollCfg { get; private set; }

        public List<int> ApplyParts
        {
            get { return NewWeaponCfg.ApplyParts; }
        }

        private List<EWeaponPartType> applyPartsSlotCache;

        public List<EWeaponPartType> ApplyPartsSlot
        {
            get
            {
                if (applyPartsSlotCache == null)
                {
                    applyPartsSlotCache = new List<EWeaponPartType>();
                    if (ApplyParts != null)
                    {
                        ApplyParts.ForEach(partId =>
                        {
                            var partCfg = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(partId);
                            if (partCfg != null)
                                applyPartsSlotCache.Add((EWeaponPartType) partCfg.Type);
                        });
                    }
                }


                return applyPartsSlotCache;
            }
        }

        public bool IsPartMatchWeapon(int partId)
        {
            return ApplyParts != null && ApplyParts.Contains(partId);
        }

        public WeaponResConfigItem NewWeaponCfg
        {
            get
            {
                if (newWeaponConfigCache == null)
                    newWeaponConfigCache = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(S_Id);
                return newWeaponConfigCache;
            }
        }

        private WeaponResConfigItem    newWeaponConfigCache;
        private WeaponAvatarConfigItem weaponAvatarConfigCache;

        public WeaponAvatarConfigItem WeaponDefaultAvartar
        {
            get
            {
                if (weaponAvatarConfigCache == null)
                {
                    weaponAvatarConfigCache = SingletonManager
                                              .Get<WeaponAvatarConfigManager>().GetConfigById(NewWeaponCfg.AvatorId);
                }

                return weaponAvatarConfigCache;
            }
        }

        private WeaponPropertyConfigItem propertyCfgCache;

        public WeaponPropertyConfigItem PropertyCfg
        {
            get
            {
                if (propertyCfgCache == null)
                    propertyCfgCache = SingletonManager.Get<WeaponPropertyConfigManager>().FindByWeaponId(S_Id);
                propertyCfgCache = propertyCfgCache ?? WeaponPropertyConfigItem.DefaultProperty;
                return propertyCfgCache;
            }
        }

        private WeaponPartsAchive defaultParts;

        public WeaponPartsAchive DefaultParts
        {
            get
            {
                if (defaultParts == null)
                {
                    defaultParts = new WeaponPartsAchive();
                    if (NewWeaponCfg == null || NewWeaponCfg.Parts == null || NewWeaponCfg.Parts.Length == 0)
                        return defaultParts;
                    foreach (var part in NewWeaponCfg.Parts)
                    {
                        if (part < 1)
                        {
                            continue;
                        }

                        var partCfg = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(part);
                        switch ((EWeaponPartType) partCfg.Type)
                        {
                            case EWeaponPartType.LowerRail:
                                defaultParts.LowerRail = part;
                                break;
                            case EWeaponPartType.UpperRail:
                                defaultParts.UpperRail = part;
                                break;
                            case EWeaponPartType.Muzzle:
                                defaultParts.Muzzle = part;
                                break;
                            case EWeaponPartType.Magazine:
                                defaultParts.Magazine = part;
                                break;
                            case EWeaponPartType.Stock:
                                defaultParts.Stock = part;
                                break;
                            case EWeaponPartType.SideRail:
                                defaultParts.SideRail = part;
                                break;
                            case EWeaponPartType.Bore:
                                defaultParts.Bore = part;
                                break;
                            case EWeaponPartType.Interlock:
                                defaultParts.Interlock = part;
                                break;
                            case EWeaponPartType.Feed:
                                defaultParts.Feed = part;
                                break;
                            case EWeaponPartType.Brake:
                                defaultParts.Brake = part;
                                break;
                            case EWeaponPartType.Trigger:
                                defaultParts.Trigger = part;
                                break;
                        }
                    }
                }

                return defaultParts;
            }
        }

        public bool CanotReloadBullet
        {
            get
            {
                return NewWeaponCfg.Type == (int) EWeaponType_Config.ThrowWeapon ||
                       NewWeaponCfg.Type == (int) EWeaponType_Config.MeleeWeapon;
            }
            //config.Type == (int) EWeaponType_Config.           MeleeWeapon;       
        }

        ///shortcut

        public int FireModeCount
        {
            get { return S_FireModelArr != null ? S_FireModelArr.Length : 0; }
        }

        public bool CanAutoFireMode()
        {
            if (null == S_FireModelArr) return false;

            foreach (var mode in S_FireModelArr)
            {
                if (mode == EFireMode.Auto)
                {
                    return true;
                }
            }

            return false;
        }

        public EFireMode GetDefaultFireModel()
        {
            if (null == S_FireModelArr || S_FireModelArr.Length < 1)
            {
                return EFireMode.Manual;
            }

            return S_FireModelArr[0];
        }

        public bool IsAttachmentMatch(int attachId)
        {
            if (S_AttachmentConfig == null)
                return false;
            for (int i = 0; i < S_AttachmentConfig.Length; i++)
            {
                if (S_AttachmentConfig[i] == attachId)
                {
                    return true;
                }
            }

            return false;
        }

        public WeaponAllConfigs(WeaponConfig weaponConfig)
        {
            InitWeaponAllConfig  = weaponConfig;
            InitAbstractBehavior = weaponConfig.WeaponBehavior;
            S_DefualtBehavior    = weaponConfig.WeaponBehavior as DefaultWeaponBehaviorConfig;
            if (null != S_DefualtBehavior)
            {
                InitAbstractLogicConfig = S_DefualtBehavior.FireLogic;
                S_DefaultFireLogicCfg   = InitAbstractLogicConfig as DefaultFireLogicConfig;
                if (null != S_DefaultFireLogicCfg)
                {
                    HasLogicConfig        = true;
                    S_CommonFireCfg       = S_DefaultFireLogicCfg.Basic;
                    S_RifleFireCounterCfg = S_DefaultFireLogicCfg.FireCounter as RifleFireCounterConfig;
                    S_BulletCfg           = S_DefaultFireLogicCfg.Bullet;
                    S_FireRollCfg = S_DefaultFireLogicCfg.FireRoll;

                    ProcessAccuracy(S_DefaultFireLogicCfg);
                    ProcessShake(S_DefaultFireLogicCfg);
                    ProcessSpread(S_DefaultFireLogicCfg);
                    ProcessFireMode(S_DefaultFireLogicCfg);
                }

                return;
                //    detailDefaultWeaponLogicConfig = new ExpandWeaponLogicConfig(OriginWeaponLogicConfig);
            }

            S_TacticBehvior = weaponConfig.WeaponBehavior as TacticWeaponBehaviorConfig;
            if (S_TacticBehvior == null)
            {
                S_DoubleBehavior = weaponConfig.WeaponBehavior as DoubleWeaponBehaviorConfig;
            }
        }

        private void ProcessFireMode(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            S_DefaultFireModeLogicCfg = defaultFireLogicConfig.FireModeLogic as DefaultFireModeLogicConfig;
            if (null != S_DefaultFireModeLogicCfg)
            {
                return;
            }
        }

        private void ProcessAccuracy(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            var accuracyConfig = defaultFireLogicConfig.AccuracyLogic;
            S_BaseAccuracyLogicCfg = accuracyConfig as BaseAccuracyLogicConfig;
            if (null != S_BaseAccuracyLogicCfg)
            {
                return;
            }

            S_PistolAccuracyLogicCfg = accuracyConfig as PistolAccuracyLogicConfig;
            if (null != S_PistolAccuracyLogicCfg)
            {
                return;
            }
        }

        private void ProcessSpread(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            var spreadConfig = defaultFireLogicConfig.SpreadLogic;
            S_RifleSpreadLogicCfg = spreadConfig as RifleSpreadLogicConfig;
            if (null != S_RifleSpreadLogicCfg)
            {
                return;
            }

            S_FixedSpreadLogicCfg = spreadConfig as FixedSpreadLogicConfig;
            if (null != S_FixedSpreadLogicCfg)
            {
                return;
            }

            S_PistolSpreadLogicCfg = spreadConfig as PistolSpreadLogicConfig;
            if (null != S_PistolSpreadLogicCfg)
            {
                return;
            }

            S_SniperSpreadLogicCfg = spreadConfig as SniperSpreadLogicConfig;
            if (null != S_SniperSpreadLogicCfg)
            {
                return;
            }
        }

        private void ProcessShake(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            var kickbackConfig = defaultFireLogicConfig.Shake;
            SRifleShakeCfg = kickbackConfig as RifleShakeConfig;
            if (null != SRifleShakeCfg)
            {
                return;
            }

            SFixedShakeCfg = kickbackConfig as FixedShakeConfig;
            if (null != SFixedShakeCfg)
            {
                return;
            }
        }

        public float GetReloadSpeed()
        {
            if (null != S_DefaultFireLogicCfg)
            {
                return ReplaceZeroWithOne(S_DefaultFireLogicCfg.ReloadSpeed);
            }

            return 1;
        }

        public float GetGunSightFov()
        {
            if (null != S_DefaultFireLogicCfg)
            {
                return S_DefaultFireLogicCfg.Fov;
            }

            return 1;
        }

        public float GetFocusSpeed()
        {
            if (null != S_DefaultFireLogicCfg)
            {
                return ReplaceZeroWithOne(S_DefaultFireLogicCfg.FocusSpeed);
            }

            return 1;
        }


        public float GetBreathFactor()
        {
            if (null != S_DefaultFireLogicCfg)
            {
                return ReplaceZeroWithOne(S_DefaultFireLogicCfg.BreathFactor);
            }

            return 1;
        }

        public int GetBulletLimit()
        {
            if (null != S_CommonFireCfg)
            {
                return S_CommonFireCfg.MagazineCapacity;
            }

            return 0;
        }

        public int GetSpecialReloadCount()
        {
            if (null != S_CommonFireCfg)
            {
                return S_CommonFireCfg.SpecialReloadCount;
            }

            return 0;
        }

        private float ReplaceZeroWithOne(float val)
        {
            return val <= 0 ? 1 : val;
        }

        public bool IsSnipperType
        {
            get { return NewWeaponCfg.IsSnipperType; }
        }
    }


    public class WeaponConfigManagement : AbstractConfigManager<WeaponConfigManagement>
    {
        private Dictionary<int, WeaponAllConfigs> _configCache = new Dictionary<int, WeaponAllConfigs>();

        private WeaponConfigs _configs       = null;
        private int[]         _emptyIntArray = new int[0];
        public  string        ConfigName;

        public WeaponConfig Configs
        {
            get
            {
                for (int i = 0; i < _configs.Weapons.Length; i++)
                {
                    if (_configs.Weapons[i].Name == ConfigName)
                    {
                        return _configs.Weapons[i];
                    }
                }

                return _configs.Weapons[0];
            }
            set
            {
                for (int i = 0; i < _configs.Weapons.Length; i++)
                {
                    if (_configs.Weapons[i].Name == ConfigName)
                    {
                        _configs.Weapons[i] = value;
                        break;
                    }
                }
            }
        }

        public int ConfigCount
        {
            get { return _configs.Weapons.Length; }
        }

        public override void ParseConfig(string xml)
        {
            _configs = XmlConfigParser<WeaponConfigs>.Load(xml);
            foreach (var item in _configs.Weapons)
            {
                _configCache[item.Id] = new WeaponAllConfigs(item);
            }
        }

        public WeaponAllConfigs FindConfigById(int id)
        {
            WeaponAllConfigs configs;
            _configCache.TryGetValue(id, out configs);
            if (configs == null)
                Logger.WarnFormat("{0} does not exist in weapon config ", id);
            return configs;
        }


        public WeaponConfigs GetConfigs()
        {
            return _configs;
        }
    }
}