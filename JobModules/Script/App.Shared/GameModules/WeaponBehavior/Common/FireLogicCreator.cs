using Assets.XmlConfig;
using Core.Utils;
using System.Collections.Generic;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IFireLogicProvider" />
    /// </summary>
    public interface IFireLogicProvider
    {
        IFireLogic GetFireLogic(WeaponResConfigItem newWeaponConfig, DefaultWeaponAbstractFireFireLogicConfig config);

        void ClearCache();
    }

    /// <summary>
    /// Defines the <see cref="FireLogicProvider" />
    /// </summary>
    public class FireLogicProvider : IFireLogicProvider
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FireLogicProvider));

        private ShowFireInMap _showFireInMap;

        private IWeaponLogicComponentsFactory _componentsFactory;

        private Dictionary<int, IFireLogic> _cache = new Dictionary<int, IFireLogic>();

        private Contexts _contexts;

        public FireLogicProvider(Contexts contexts,
            IWeaponLogicComponentsFactory weaponLogicComponentsFactory)
        {
            _contexts = contexts;
            _componentsFactory = weaponLogicComponentsFactory;
            _showFireInMap = new ShowFireInMap(contexts.ui);
        }

        public IFireLogic GetFireLogic(WeaponResConfigItem newWeaponConfig,
            DefaultWeaponAbstractFireFireLogicConfig config)
        {
            if (!_cache.ContainsKey(newWeaponConfig.Id))
            {
                var defaultCfg = config as DefaultFireLogicConfig;
                if (null != defaultCfg)
                {
                    _cache[newWeaponConfig.Id] = CreateDefaultFireLogic(newWeaponConfig, defaultCfg);
                }
                var meleeCfg = config as MeleeFireLogicConfig;
                if (null != meleeCfg)
                {
                    _cache[newWeaponConfig.Id] = new MeleeFireLogic(meleeCfg);
                }
                var throwingCfg = config as ThrowingFireLogicConfig;
                if (null != throwingCfg)
                {
                    _cache[newWeaponConfig.Id] = new ThrowingFireLogic(
                        newWeaponConfig,
                        throwingCfg,
                        _componentsFactory);
                }
            }
            if (_cache.ContainsKey(newWeaponConfig.Id))
            {
                return _cache[newWeaponConfig.Id];
            }
            if (newWeaponConfig.Type != (int)EWeaponType_Config.TacticWeapon)
            {
                Logger.ErrorFormat("no firelogic for weapon {0}", newWeaponConfig.Id);
            }
            return null;
        }

        private IFireLogic CreateDefaultFireLogic(WeaponResConfigItem newWeaponConfig,
            DefaultFireLogicConfig config)
        {
            var fireLogic = new DefaultFireLogic();
            fireLogic.RegisterLogic(_componentsFactory.CreateAccuracyLogic(config.AccuracyLogic));
            fireLogic.RegisterLogic(_componentsFactory.CreateSpreadLogic(config.SpreadLogic));
            fireLogic.RegisterLogic(_componentsFactory.CreateKickbackLogic(config.KickbackLogic));

            fireLogic.RegisterLogic(_componentsFactory.CreateFireCheckLogic(config.FireModeLogic));
            fireLogic.RegisterLogic(_componentsFactory.CreateFireActionLogic(newWeaponConfig));
            fireLogic.RegisterLogic(_componentsFactory.CreateFireBulletCounterLogic(config.FireCounter));
            fireLogic.RegisterLogic(_componentsFactory.CreateAutoFireLogic(config.FireModeLogic));
            fireLogic.RegisterLogic(_componentsFactory.CreateBulletFireLogic(config.Bullet));
            fireLogic.RegisterLogic(_componentsFactory.CreateSpecialReloadCheckLogic(config.Basic));
            fireLogic.RegisterLogic(_componentsFactory.CreateFireCommandLogic(config));
            fireLogic.RegisterLogic(_componentsFactory.CreateShowFireInMap(config));
            return fireLogic;
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
