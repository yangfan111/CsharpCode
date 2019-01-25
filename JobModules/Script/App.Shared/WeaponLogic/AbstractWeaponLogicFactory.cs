using System;
using WeaponConfigNs;
using Core.WeaponLogic.Common;
using Core.Configuration;
using Assets.Utils.Configuration;
using Core.WeaponLogic.FireAciton;
using Utils.Singleton;
using Core.WeaponLogic.WeaponLogicInterface;

namespace Core.WeaponLogic
{
    public interface IWeaponLogicComponentsFactory
    {
        IWeaponLogic CreateWeaponLogic(NewWeaponConfigItem newCfg,
            WeaponConfig config,
            IWeaponSoundLogic soundLogic,
            IWeaponEffectLogic effectLogic);
        IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config);
        IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config);
        ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config);
        IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config);

        IThrowingFactory CreateThrowingFactory(NewWeaponConfigItem newWeaponConfig, ThrowingConfig config);
        IFireActionLogic CreateFireActionLogic(NewWeaponConfigItem config);
        IFireCheck CreateFireCheckLogic(FireModeLogicConfig config);
        IFireBulletCounter CreateFireBulletCounterLogic(FireCounterConfig config);
        IFireEffectFactory CreateFireEffectFactory(BulletConfig config);
        IAfterFire CreateAutoFireLogic(FireModeLogicConfig config);
        IFireCheck CreateSpecialReloadCheckLogic(CommonFireConfig config);
        IBulletFire CreateBulletFireLogic(BulletConfig config);
        IFireTriggger CreateFireCommandLogic(FireLogicConfig config);
        IBulletFire CreateShowFireInMap(FireLogicConfig config);
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

        public IWeaponLogic CreateWeaponLogic(int weaponId, IWeaponSoundLogic soundLogic, IWeaponEffectLogic effectLogic)
        {
            foreach (var row in Configs.Weapons)
            {
                if (row.Id == weaponId)
                {
                    var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
                    var rc = _componentsFactory.CreateWeaponLogic(
                        weaponCfg,
                        row, 
                        soundLogic, 
                        effectLogic);
                    if (rc == null)
                    {
                        throw new Exception("unconfiged weapon " + weaponId);
                    }
                    return rc;
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
}