using Assets.XmlConfig;
using Core.Utils;
using System.Collections.Generic;
using Assets.Utils.Configuration;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{


    /// <summary>
    /// Defines the <see cref="WeaponFireScriptsProvider" />
    /// </summary>
    public class WeaponFireScriptsProvider
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponFireScriptsProvider));

        private ShowFireListenerInMap _showFireListenerInMap;

        private WeaponFireScriptsUtil _scriptUtil;

        private Dictionary<int, IWeaponFireController> _cache = new Dictionary<int, IWeaponFireController>();

        private Contexts _contexts;

        public WeaponFireScriptsProvider(Contexts contexts)
        {
            _contexts = contexts;
            _showFireListenerInMap = new ShowFireListenerInMap(contexts.ui);
            _scriptUtil = new WeaponFireScriptsUtil(contexts,contexts.session.commonSession.EntityIdGenerator);
        }
        public IWeaponFireController GetFireController(WeaponAllConfigs configs)
        {
            return GetFireController(configs, configs.InitAbstractLogicConfig);
        }
        public IWeaponFireController GetFireController(WeaponAllConfigs configs,
            DefaultWeaponAbstractFireFireLogicConfig config)
        {
            if (!_cache.ContainsKey(configs.S_Id))
            {
                var defaultCfg = config as DefaultFireLogicConfig;
                if (null != defaultCfg)
                {
                    _cache[configs.S_Id] = CreateCommonFireController(configs, defaultCfg);
                }
                var meleeCfg = config as MeleeFireLogicConfig;
                if (null != meleeCfg)
                {
                    _cache[configs.S_Id] = new MeleeWeaponFireController(meleeCfg);
                }
                var throwingCfg = config as ThrowingFireLogicConfig;
                if (null != throwingCfg)
                {
                    _cache[configs.S_Id] = new ThrowingWeaponFireController(
                        throwingCfg,_scriptUtil.CreateThrowingFactory(configs.NewWeaponCfg,throwingCfg.Throwing));
                }
            }
            if (_cache.ContainsKey(configs.S_Id))
            {
                return _cache[configs.S_Id];
            }
            if (configs.NewWeaponCfg.Type != (int)EWeaponType_Config.TacticWeapon)
            {
                Logger.ErrorFormat("no firelogic for weapon {0}", configs.S_Id);
            }
            return null;
        }

        private IWeaponFireController CreateCommonFireController(WeaponAllConfigs allConfigs,
            DefaultFireLogicConfig config)
        {
            var fireLogic = new CommonWeaponFireController();
            fireLogic.RegisterProcessor(_scriptUtil.CreateAccuracyCalculator(config.AccuracyLogic));
            fireLogic.RegisterProcessor(_scriptUtil.CreateSpreadProcessor(config.SpreadLogic));
            fireLogic.RegisterProcessor(_scriptUtil.CreateShakeProcessor(config.Shake));
            fireLogic.RegisterProcessor(_scriptUtil.CreateEffectManager(allConfigs.S_EffectConfig));
            fireLogic.RegisterProcessor(_scriptUtil.CreateFireChecker(config.FireModeLogic));
            fireLogic.RegisterProcessor(_scriptUtil.CreateFireActionLogic(allConfigs));
            fireLogic.RegisterProcessor(_scriptUtil.CreateFireBulletCounter(config.FireCounter));
            fireLogic.RegisterProcessor(_scriptUtil.CreateAutoFireProcessor(config.FireModeLogic));
            fireLogic.RegisterProcessor(_scriptUtil.CreateBulletFireListener(config.Bullet));
            fireLogic.RegisterProcessor(_scriptUtil.CreateSpecialReloadChecker(config.Basic));
            fireLogic.RegisterProcessor(_scriptUtil.CreateFireTrigger(config));
            fireLogic.RegisterProcessor(_scriptUtil.CreateShowFireInMap(config));
            return fireLogic;
        }
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
