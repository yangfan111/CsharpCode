using System.Collections.Generic;
using Utils.Configuration;
using WeaponConfigNs;
using XmlConfig;

namespace Core.Configuration
{
    public interface IWeaponDataConfigManager
    {
        int ConfigCount { get; }
        WeaponConfig GetConfigById(int id);
    }

    public class ExpandWeaponDataConfig
    {
        public readonly WeaponConfig Weapon;
        public DefaultWeaponLogicConfig defaultWeaponLogicConfig { get; private set; }
        public ExpandWeaponLogicConfig detailDefaultWeaponLogicConfig { get; private set; }

        public ExpandWeaponDataConfig(WeaponConfig weaponConfig)
        {
            Weapon = weaponConfig;
            defaultWeaponLogicConfig = weaponConfig.WeaponLogic as DefaultWeaponLogicConfig;
            if(null != defaultWeaponLogicConfig)
            {
                detailDefaultWeaponLogicConfig = new ExpandWeaponLogicConfig(defaultWeaponLogicConfig);
            }
        }
    }

    /// <summary>
    /// 处理武器配置相关逻辑
    /// </summary>
    public class ExpandWeaponLogicConfig
    {
        public DefaultFireLogicConfig DefaultFireLogicCfg { get; private set; }
        public DefaultWeaponLogicConfig DefaultWeaponLogicCfg { get; private set; }
        public TacticWeaponLogicConfig TacticWeaponLogicCfg { get; private set; }
        public CommonFireConfig CommonFireCfg { get; private set; }

        public PistolAccuracyLogicConfig PistolAccuracyLogicCfg { get; private set; }
        public BaseAccuracyLogicConfig BaseAccuracyLogicCfg { get; private set; }

        public FixedSpreadLogicConfig FixedSpreadLogicCfg { get; private set; }
        public PistolSpreadLogicConfig PistolSpreadLogicCfg { get; private set; }
        public ShotgunSpreadLogicConfig ShotgunSpreadLogicCfg { get; private set; }
        public RifleSpreadLogicConfig RifleSpreadLogicCfg { get; private set; }
        public SniperSpreadLogicConfig SniperSpreadLogicCfg { get; private set; }

        public RifleKickbackLogicConfig RifleKickbackLogicCfg { get; private set; }
        public FixedKickbackLogicConfig FixedKickbackLogicCfg { get; private set; } 

        public DefaultFireModeLogicConfig DefaultFireModeLogicCfg { get; private set; }
        public RifleFireCounterConfig RifleFireCounterCfg { get; private set; }
        
        public BulletConfig BulletCfg { get; private set; }

        public ExpandWeaponLogicConfig(TacticWeaponLogicConfig tacticWeaponLogicConfig)
        {
            if(null == tacticWeaponLogicConfig)
            {
                return;
            }
            TacticWeaponLogicCfg = tacticWeaponLogicConfig;
        }

        public ExpandWeaponLogicConfig (DefaultWeaponLogicConfig defaultWeaponLogicConfig)
        {
            if(null == defaultWeaponLogicConfig)
            {
                return;
            }
            DefaultWeaponLogicCfg = defaultWeaponLogicConfig;
            DefaultFireLogicCfg = DefaultWeaponLogicCfg.FireLogic as DefaultFireLogicConfig;
            if(null != DefaultFireLogicCfg)
            {
                CommonFireCfg = DefaultFireLogicCfg.Basic;
                RifleFireCounterCfg = DefaultFireLogicCfg.FireCounter as RifleFireCounterConfig;
                BulletCfg = DefaultFireLogicCfg.Bullet;

                ProcessAccuracy(DefaultFireLogicCfg);
                ProcessKickback(DefaultFireLogicCfg);
                ProcessSpread(DefaultFireLogicCfg);
                ProcessFireMode(DefaultFireLogicCfg);
                
                return;
            }
            //TODO 近战和投掷配置
        }

        private void ProcessFireMode(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            DefaultFireModeLogicCfg = defaultFireLogicConfig.FireModeLogic as DefaultFireModeLogicConfig;
            if(null != DefaultFireModeLogicCfg)
            {
                return;
            }
        }

        private void ProcessAccuracy(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            var accuracyConfig = defaultFireLogicConfig.AccuracyLogic;
            BaseAccuracyLogicCfg = accuracyConfig as BaseAccuracyLogicConfig;
            if(null != BaseAccuracyLogicCfg)
            {
                return;
            }
            PistolAccuracyLogicCfg = accuracyConfig as PistolAccuracyLogicConfig;
            if(null != PistolAccuracyLogicCfg)
            {
                return;
            }
        }

        private void ProcessSpread(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            var spreadConfig = defaultFireLogicConfig.SpreadLogic;
            RifleSpreadLogicCfg = spreadConfig as RifleSpreadLogicConfig;
            if(null != RifleSpreadLogicCfg)
            {
                return;
            }
            FixedSpreadLogicCfg = spreadConfig as FixedSpreadLogicConfig;
            if(null != FixedSpreadLogicCfg)
            {
                return;
            }
            PistolSpreadLogicCfg = spreadConfig as PistolSpreadLogicConfig;
            if(null != PistolSpreadLogicCfg)
            {
                return;
            }
            SniperSpreadLogicCfg = spreadConfig as SniperSpreadLogicConfig;
            if(null != SniperSpreadLogicCfg)
            {
                return;
            }
        }

        private void ProcessKickback(DefaultFireLogicConfig defaultFireLogicConfig)
        {
            var kickbackConfig = defaultFireLogicConfig.KickbackLogic;
            RifleKickbackLogicCfg = kickbackConfig as RifleKickbackLogicConfig;
            if(null != RifleKickbackLogicCfg)
            {
                return;
            }
            FixedKickbackLogicCfg = kickbackConfig as FixedKickbackLogicConfig;
            if(null != FixedKickbackLogicCfg)
            {
                return;
            }
        }

        public float GetReloadSpeed()
        {
            if(null != DefaultFireLogicCfg)
            {
                return ReplaceZeroWithOne(DefaultFireLogicCfg.ReloadSpeed);
            }
            return 1;
        }

        public float GetGunSightFov()
        {
            if(null != DefaultFireLogicCfg)
            {
                return DefaultFireLogicCfg.Fov;
            }
            return 1;
        }

        public float GetFocusSpeed()
        {
            if(null != DefaultFireLogicCfg)
            {
                return ReplaceZeroWithOne(DefaultFireLogicCfg.FocusSpeed);
            }
            return 1;
        }

        public float GetSpeed()
        {
            if(null != DefaultWeaponLogicCfg)
            {
                return ReplaceZeroWithOne(DefaultWeaponLogicCfg.MaxSpeed);
            }
            if(null != TacticWeaponLogicCfg)
            {
                return ReplaceZeroWithOne(TacticWeaponLogicCfg.MaxSpeed);
            }
            return 1;
        }

        public float GetBreathFactor()
        {
            if(null != DefaultFireLogicCfg)
            {
                return ReplaceZeroWithOne(DefaultFireLogicCfg.BreathFactor);
            }
            return 1;
        }

        public int GetBulletLimit()
        {
            if(null != CommonFireCfg)
            {
                return CommonFireCfg.MagazineCapacity;
            }
            return 0;
        }

        public bool GetRunable()
        {
            if(null != DefaultWeaponLogicCfg)
            {
                return !DefaultWeaponLogicCfg.CantRun;
            }
            if(null != TacticWeaponLogicCfg)
            {
                return !TacticWeaponLogicCfg.CantRun;
            }
            return true;
        }

        public int GetSpecialReloadCount()
        {
            if(null != CommonFireCfg)
            {
                return CommonFireCfg.SpecialReloadCount;
            }
            return 0;
        }

        private float ReplaceZeroWithOne(float val)
        {
            return val == 0 ? 1 : val;
        }
    }


    public class WeaponDataConfigManager : AbstractConfigManager<WeaponDataConfigManager>, IWeaponDataConfigManager
    {
        private Dictionary<int, ExpandWeaponDataConfig> _configCache = new Dictionary<int, ExpandWeaponDataConfig>();
        private Dictionary<int, int> _fakeConfigCache = new Dictionary<int, int>();

        private WeaponConfigs _configs = null;
        private int[] _emptyIntArray = new int[0];
        public string ConfigName;

        public WeaponConfig Configs
        {
            get
            {
                for(int i = 0; i < _configs.Weapons.Length; i++)
                {
                    if(_configs.Weapons[i].Name == ConfigName)
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
            get
            {
                return _configs.Weapons.Length;
            }
        }

        public override void ParseConfig(string xml)
        {
            _configs = XmlConfigParser<WeaponConfigs>.Load(xml);
            foreach (var item in _configs.Weapons)
            {
                _configCache[item.Id] = new ExpandWeaponDataConfig(item);
            }
        }

        private ExpandWeaponDataConfig GetDetailWeaponDataConfigItem(int id)
        {
            if(_configCache.ContainsKey(id))
            {
                return _configCache[id];
            }
            else
            {
                Logger.ErrorFormat("{0} does not exist in weapon config ", id);
                return null;
            }
        }

        public WeaponConfig GetConfigById(int id)
        {
            var config = GetDetailWeaponDataConfigItem(id);
            if(null != config)
            {
                return config.Weapon;
            }
            return null;
        }

        public bool HasAutoFireMode(int id)
        {
            var cfg = GetFireModeConfig(id);
            if (null == cfg) return false;
            foreach(var mode in cfg.AvaliableModes)
            {
                if(mode == EFireMode.Auto)
                {
                    return true;
                }
            }
            return false;
        } 

        public EFireMode GetFirstAvaliableFireMode(int id)
        {
            var cfg = GetFireModeConfig(id);
            if(null == cfg || cfg.AvaliableModes.Length < 1)
            {
                return EFireMode.Manual;
            }
            return cfg.AvaliableModes[0]; 
        }

        public int GetFireModeCountById(int id)
        {
            var cfg = GetFireModeConfig(id);
            if(null == cfg)
            {
                return 1;
            }
            return cfg.AvaliableModes.Length;
        }

        public DefaultFireLogicConfig GetFireLogicConfig(int id)
        {
            var config = GetDetailWeaponDataConfigItem(id);
            if(null == config)
            {
                return null;
            }
            var fireLogicConfig = config.detailDefaultWeaponLogicConfig.DefaultFireLogicCfg;
            return fireLogicConfig;
        }

        public DefaultFireModeLogicConfig GetFireModeConfig(int id)
        {
            var config = GetDetailWeaponDataConfigItem(id);
            if(null == config)
            {
                return null;
            }
            if(null == config.detailDefaultWeaponLogicConfig)
            {
                return null;
            }
            var fireModeCfg = config.detailDefaultWeaponLogicConfig.DefaultFireModeLogicCfg;
            return fireModeCfg;
        }

        public WeaponConfigs GetConfigs()
        {
            return _configs;   
        }

        public bool IsAttachmentMatch(int id, int attachId )
        {
            if(!_configCache.ContainsKey(id))
            {
                Logger.ErrorFormat("{0} does not exist in weapon config ", id);
                return false;
            }
            var config = _configCache[id];
            var attachs = config.Weapon.WeaponLogic.AttachmentConfig;
            for(int i = 0; i < attachs.Length; i++)
            {
                if(attachs[i] == attachId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
