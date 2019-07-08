using System.Collections.Generic;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    ///     Defines the <see cref="WeaponFireScriptsProvider" />
    /// </summary>
    public class WeaponFireScriptsProvider
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponFireScriptsProvider));

        private Dictionary<int, IWeaponFireController> _cache = new Dictionary<int, IWeaponFireController>();

        private Contexts _contexts;

        private WeaponFireScriptsUtil _scriptUtil;

        private ShowFireListener _showFireListener;

        public WeaponFireScriptsProvider(Contexts contexts)
        {
            _contexts         = contexts;
            _showFireListener = new ShowFireListener(contexts.ui);
            _scriptUtil       = new WeaponFireScriptsUtil(contexts, contexts.session.commonSession.EntityIdGenerator);
        }

        public IWeaponFireController GetFireController(WeaponAllConfigs configs)
        {
            return GetFireController(configs, configs.InitAbstractLogicConfig);
        }

        public IWeaponFireController GetFireController(WeaponAllConfigs configs,
                                                       DefaultWeaponAbstractFireFireLogicConfig config)
        {
            IWeaponFireController v = null;
            if (_cache.TryGetValue(configs.S_Id, out v))
            {
                return _cache[configs.S_Id];
            }

            var defaultCfg = config as DefaultFireLogicConfig;
            if (null != defaultCfg)
            {
                v                    = CreateCommonFireController(configs, defaultCfg);
                _cache[configs.S_Id] = v;
                return v;
            }

            var meleeCfg = config as MeleeFireLogicConfig;
            if (null != meleeCfg)
            {
                v                    = new MeleeWeaponFireController(meleeCfg);
                _cache[configs.S_Id] = v;
                return v;
            }

            var throwingCfg = config as ThrowingFireLogicConfig;
            if (null != throwingCfg)
            {
                v                    = new ThrowingWeaponFireController(throwingCfg,configs.NewWeaponCfg);
                _cache[configs.S_Id] = v;
                return v;
            }

            if (configs.NewWeaponCfg.Type != (int) EWeaponType_Config.TacticWeapon)
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
            fireLogic.RegisterProcessor(_scriptUtil.CreateFireModeChecker(allConfigs));
            fireLogic.RegisterProcessor(_scriptUtil.CreateFireModeUpdater(config.FireModeLogic));
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