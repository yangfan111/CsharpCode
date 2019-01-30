using System;
using Core.Utils;
using WeaponConfigNs;
using Core.WeaponLogic.Common;
using Core.WeaponLogic.Kickback;
using Core.WeaponLogic.Accuracy;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Bullet;
using Core.WeaponLogic.Throwing;
using Core.Configuration;
using Assets.Utils.Configuration;
using Core.WeaponLogic.FireAciton;
using Utils.Singleton;

namespace Core.WeaponLogic
{
    public interface IWeaponLogicComponentsFactory
    {
        IWeaponLogic CreateWeaponLogic(NewWeaponConfigItem newCfg, 
            WeaponLogicConfig config, 
            IWeaponSoundLogic soundLogic, 
            IWeaponEffectLogic effectLogic, 
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher);
        IWeaponSoundLogic CreateSoundLogic(IPlayerWeaponState weaponState, WeaponLogicConfig config);
        IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config);
        IFireLogic CreateFireLogic(NewWeaponConfigItem newCfg, 
            FireLogicConfig config, 
            IWeaponSoundLogic soundLogic, 
            IWeaponEffectLogic effectLogic,
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher);
        IMoveSpeedLogic CreateMoveSpeedLogic(MoveSpeedLogicConfig config);

        IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config, CommonFireConfig conmmon);
        ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config, CommonFireConfig conmmon);
        IBulletFactory CreateBulletFactory(BulletConfig config, CommonFireConfig conmmon);
        IThrowingFactory CreateThrowingFactory(NewWeaponConfigItem newWeaponConfig, ThrowingConfig config);
        IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config, CommonFireConfig conmmon);
        IFireActionLogic CreateFireActionLogic(NewWeaponConfigItem config, CommonFireConfig common, IWeaponSoundLogic weaponLoigc);
        IFireBulletModeLogic CreateFireReadyLogic(FireModeLogicConfig config, CommonFireConfig conmmon);
        IFireBulletCounter CreateContinuesShootLogic(FireCounterConfig config, CommonFireConfig conmmon);
        IFireEffectFactory CreateFireEffectFactory(BulletConfig config);
        IAutoFireLogic CreateAutoFireLogic(FireModeLogicConfig config, CommonFireConfig common);
        IBulletContainer CreateBulletLogic(CommonFireConfig common);
        IThrowingContainer CreateThrowingLogic(CommonFireConfig common);
    }

    public interface IWeaponLogicFactory
    {
        IWeaponLogic CreateWeaponLogic(int weaponId, IWeaponSoundLogic soundLogic, IWeaponEffectLogic effectLogic, IBulletFireInfoProviderDispatcher bulletFireInfoProvider);
        IWeaponSoundLogic CreateWeaponSoundLogic(int weaponId, IPlayerWeaponState weaponState);
        IWeaponEffectLogic CreateWeaponEffectLogic(int weaponId);
    }

    public class WeaponLogicFactory : IWeaponLogicFactory
    {
        private IWeaponLogicComponentsFactory _componentsFactory;

        private WeaponConfigs Configs
        {
            get
            {
                if(null == _configs)
                {
                    return SingletonManager.Get<WeaponDataConfigManager>().GetConfigs();
                }
                return _configs;
            }
        }
        private WeaponConfigs _configs;

        public WeaponLogicFactory(IWeaponLogicComponentsFactory factory)
        {
            _componentsFactory = factory;
        }

        public WeaponLogicFactory(WeaponConfigs configs, IWeaponLogicComponentsFactory factory)
        {
            _componentsFactory = factory;
            _configs = configs;
        }

        public IWeaponLogic CreateWeaponLogic(int weaponId, IWeaponSoundLogic soundLogic, IWeaponEffectLogic effectLogic, IBulletFireInfoProviderDispatcher bulletFireInfoProvider)
        {
            foreach (var row in Configs.Weapons)
            {
                if (row.Id == weaponId)
                {
                    var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
                    var rc = _componentsFactory.CreateWeaponLogic(
                        weaponCfg,
                        row.WeaponLogic, 
                        soundLogic, 
                        effectLogic,
                        bulletFireInfoProvider);
                    if (rc == null)
                    {
                        throw new Exception("unconfiged weapon " + weaponId);
                    }
                    rc.Reset();
                    return rc;
                }
            }

            return null;
        }

        public IWeaponSoundLogic CreateWeaponSoundLogic(int weaponId, IPlayerWeaponState weaponState)
        {
            foreach (var row in Configs.Weapons)
            {
                if (row.Id == weaponId)
                {
                    var soundLogic = _componentsFactory.CreateSoundLogic(weaponState, row.WeaponLogic);
                    if (soundLogic == null)
                    {
                        throw new Exception("unconfiged weapon " + weaponId);
                    }
                    return soundLogic;
                }
            }
            return null;
        }

        public IWeaponEffectLogic CreateWeaponEffectLogic(int weaponId)
        {
            foreach (var row in Configs.Weapons)
            {
                if (row.Id == weaponId)
                {
                    var effectLogic = _componentsFactory.CreateEffectLogic(row.WeaponLogic.EffectConfig as DefaultWeaponEffectConfig);
                    if (effectLogic == null)
                    {
                        throw new Exception("unconfiged weapon " + weaponId);
                    }
                    return effectLogic;
                }
            }
            return null; ;
        }
    }

    public abstract class AbstractWeaponLogicComponentsFactory : IWeaponLogicComponentsFactory
    {
        protected IAttachmentManager _attachmentManager;
        public AbstractWeaponLogicComponentsFactory(IAttachmentManager attachManager)
        {
            _attachmentManager = attachManager;
        }

        public IWeaponLogic CreateWeaponLogic(NewWeaponConfigItem newCfg, 
            WeaponLogicConfig config, 
            IWeaponSoundLogic soundLogic, 
            IWeaponEffectLogic effectLogic, 
            IBulletFireInfoProviderDispatcher bulletFireInfoProvider)
        {
            IWeaponLogic rc = null;
            if (config is DefaultWeaponLogicConfig)
            {
                rc = new DefaultWeaponLogic(newCfg, 
                    config as DefaultWeaponLogicConfig, 
                    this, 
                    soundLogic, 
                    effectLogic, 
                    _attachmentManager, 
                    bulletFireInfoProvider);
            }
            else if (config is DoubleWeaponLogicConfig)
            {
                rc = new DoubleWeaponLogic(newCfg, 
                    config as DoubleWeaponLogicConfig, 
                    this, soundLogic, 
                    effectLogic, 
                    _attachmentManager,
                    bulletFireInfoProvider);
            }
            
            return rc;
        }

        public abstract IWeaponSoundLogic CreateSoundLogic(IPlayerWeaponState weaponState, WeaponLogicConfig config);

        public abstract IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config);

        public IFireLogic CreateFireLogic(NewWeaponConfigItem newWeaponConfig, 
            FireLogicConfig config, 
            IWeaponSoundLogic soundLogic, 
            IWeaponEffectLogic effectLogic,
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher)
        {
            var defaultCfg = config as DefaultFireLogicConfig;
            if (null != defaultCfg)
            {
                return new DefaultFireLogic(
                    newWeaponConfig,
                    defaultCfg,
                    this,
                    _attachmentManager,
                    soundLogic,
                    effectLogic,
                    bulletFireInfoProviderDispatcher);
            }
            var meleeCfg = config as MeleeFireLogicConfig;
            if(null != meleeCfg)
            {
                return new MeleeFireLogic(meleeCfg, soundLogic, effectLogic);
            }
            var throwingCfg = config as ThrowingFireLogicConfig;
            if (null != throwingCfg)
            {
                return new ThrowingFireAction(
                    newWeaponConfig,
                    throwingCfg,
                    this,
                    _attachmentManager,
                    soundLogic,
                    effectLogic);
            }
            return null;
        }

        public IBulletContainer CreateBulletLogic(CommonFireConfig config)
        {
            return new BaseWeaponBulletLogic(config, _attachmentManager);
        }

        public IThrowingContainer CreateThrowingLogic(CommonFireConfig config)
        {
            return new BaseWeaponThrowingLogic(config, _attachmentManager);
        }

        public IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config, CommonFireConfig common)
        {
            if (config is BaseAccuracyLogicConfig)
            {
                return new BaseAccuracyLogic(config as BaseAccuracyLogicConfig);
            }
            else if (config is PistolAccuracyLogicConfig)
            {
                return  new PistolAccuracyLogic(config as PistolAccuracyLogicConfig);
            }

            return null;
        }

        public ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config, CommonFireConfig common)
        {
            if (config is FixedSpreadLogicConfig)
            {
                return new FixedSpreadLogic(config as FixedSpreadLogicConfig);
            }
            else if (config is PistolSpreadLogicConfig)
            {
                return new PistolSpreadLogic(config as PistolSpreadLogicConfig);
            }
            else if (config is SniperSpreadLogicConfig)
            {
                return new SniperSpreadLogic(config as SniperSpreadLogicConfig);
            }
            else if (config is RifleSpreadLogicConfig)
            {
                return new RifleSpreadLogic(config as RifleSpreadLogicConfig);
            }

            return null;
        }

        public abstract IBulletFactory CreateBulletFactory(BulletConfig config, CommonFireConfig common);
        public abstract IFireEffectFactory CreateFireEffectFactory(BulletConfig config);
        public abstract IThrowingFactory CreateThrowingFactory(NewWeaponConfigItem newWeaponConfig, ThrowingConfig config);

        public IAutoFireLogic CreateAutoFireLogic(FireModeLogicConfig config, CommonFireConfig common)
        {
            var modeConfig = config as DefaultFireModeLogicConfig; 
            if(null != modeConfig)
            {
                foreach(var mode in modeConfig.AvaiableModes)
                {
                    if(mode == EFireMode.Burst)
                    {
                        return new AutoFireLogic(modeConfig.BurstCount, modeConfig.BurstAttackInnerInterval, modeConfig.BurstAttackInterval);
                    }
                }
            }
            return new DummyAutoFireLogic();
        }

        public IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config, CommonFireConfig common)
        {
            if (config is RifleKickbackLogicConfig)
            {
                return new RifleKickbackLogic(config as RifleKickbackLogicConfig, common);
            }
            else if (config is FixedKickbackLogicConfig)
            {
                return new FixedKickbackLogic(config as FixedKickbackLogicConfig);
            }


            return null;
        }

        public IFireBulletModeLogic CreateFireReadyLogic(FireModeLogicConfig config, CommonFireConfig common)
        {
            if (config is DefaultFireModeLogicConfig)
                return new FireBulletModeLogic(config as DefaultFireModeLogicConfig, common);
            return null;
        }

        public IFireBulletCounter CreateContinuesShootLogic(FireCounterConfig config, CommonFireConfig common)
        {
            if (config is RifleFireCounterConfig)
            {
                return new RifleFireBulletCounter(config as RifleFireCounterConfig);
            }

            return null;
        }

        public IMoveSpeedLogic CreateMoveSpeedLogic(MoveSpeedLogicConfig config)
        {
            if (config is DefaultMoveSpeedLogicConfig)
            {
                return new DefaultMaxMoveSpeedLogic(config as DefaultMoveSpeedLogicConfig);
            }

            return null;
        }

        public IFireActionLogic CreateFireActionLogic(NewWeaponConfigItem config, CommonFireConfig commonConfig, IWeaponSoundLogic weaponSoundLogic)
        {
            if(SingletonManager.Get<WeaponConfigManager>().IsSpecialType(config.Id, ESpecialWeaponType.SniperFrie))
            {
                return new SpecialFireActionLogic(commonConfig, weaponSoundLogic);
            }
            else
            {
                return new DefaultFireActionLogic(commonConfig, weaponSoundLogic);
            }
        }
    }
}