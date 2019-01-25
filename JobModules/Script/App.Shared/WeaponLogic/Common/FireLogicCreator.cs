using App.Shared.WeaponLogic.FireLogic;
using Core.WeaponLogic;
using Core.WeaponLogic.Common;
using System.Collections.Generic;
using WeaponConfigNs;

namespace App.Shared.WeaponLogic.Common
{
    public interface IFireLogicCreator
    {
        IFireLogic GetFireLogic(NewWeaponConfigItem newWeaponConfig, FireLogicConfig config);
        void ClearCache();
    }

    public class FireLogicCreator : IFireLogicCreator
    {
        private ShowFireInMap _showFireInMap;
        private IWeaponLogicComponentsFactory _componentsFactory;

        private Dictionary<int, IFireLogic> _cache = new Dictionary<int, IFireLogic>();
        private Contexts _contexts;

        public FireLogicCreator(Contexts contexts, 
            IWeaponLogicComponentsFactory weaponLogicComponentsFactory)
        {
            _contexts = contexts;
            _componentsFactory = weaponLogicComponentsFactory; 
            _showFireInMap = new ShowFireInMap(contexts.ui);
        }

        public IFireLogic GetFireLogic(NewWeaponConfigItem newWeaponConfig, 
            FireLogicConfig config)
        {
            if(!_cache.ContainsKey(newWeaponConfig.Id))
            {
                var defaultCfg = config as DefaultFireLogicConfig;
                if (null != defaultCfg)
                {
                    _cache[newWeaponConfig.Id] = CreateDefaultFireLogic(newWeaponConfig, defaultCfg);
                }
                var meleeCfg = config as MeleeFireLogicConfig;
                if(null != meleeCfg)
                {
                    _cache[newWeaponConfig.Id] = new MeleeFireLogic(_contexts, meleeCfg);
                }
                var throwingCfg = config as ThrowingFireLogicConfig;
                if (null != throwingCfg)
                {
                    _cache[newWeaponConfig.Id] = new ThrowingFireLogic(
                        _contexts,
                        newWeaponConfig,
                        throwingCfg,
                        _componentsFactory);
                }
            }
            return _cache[newWeaponConfig.Id];
        }

        private IFireLogic CreateDefaultFireLogic(NewWeaponConfigItem newWeaponConfig, 
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