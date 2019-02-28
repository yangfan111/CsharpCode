using App.Shared.Components;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Weapon.Behavior;
using Assets.Utils.Configuration;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponLogicComponentsFactory" />
    /// </summary>
    public class WeaponLogicComponentsFactory : IWeaponLogicComponentsFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WeaponLogicComponentsFactory));

        private IEntityIdGenerator _entityIdGenerator;

        private Contexts _contexts;

        public WeaponLogicComponentsFactory(
            Contexts contexts,
            IEntityIdGenerator entityIdGenerator)
        {
            _contexts = contexts;
            _entityIdGenerator = entityIdGenerator;
        }

        public IWeaponLogic CreateWeaponLogic(WeaponResConfigItem newCfg,
        WeaponConfig config,
        IWeaponSoundLogic soundLogic,
        IWeaponEffectLogic effectLogic)
        {
            IWeaponLogic rc = null;
            var weaponLogicConfig = config.WeaponLogic;
            if (weaponLogicConfig is WeaponConfigNs.DefaultWeaponBehaviorConfig)
            {
                rc = new Behavior.DefaultWeaponLogic();
            }
            else if (weaponLogicConfig is DoubleWeaponBehaviorConfig)
            {
                rc = new DoubleWeaponLogic(null, null);
            }
            return rc;
        }

        public IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config)
        {
            if (config is BaseAccuracyLogicConfig)
            {
                return new BaseAccuracyLogic();
            }
            else if (config is PistolAccuracyLogicConfig)
            {
                return new PistolAccuracyLogic();
            }

            return null;
        }

        public ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config)
        {
            if (config is FixedSpreadLogicConfig)
            {
                return new FixedSpreadLogic();
            }
            else if (config is PistolSpreadLogicConfig)
            {
                return new PistolSpreadLogic();
            }
            else if (config is SniperSpreadLogicConfig)
            {
                return new SniperSpreadLogic(_contexts);
            }
            else if (config is RifleSpreadLogicConfig)
            {
                return new RifleSpreadLogic();
            }

            return null;
        }

        public IFireCheck CreateSpecialReloadCheckLogic(CommonFireConfig commonFireConfig)
        {
            if (null != commonFireConfig && commonFireConfig.SpecialReloadCount > 0)
            {
                return new SpecialReloadCheckLogic();
            }
            return null;
        }

        public IBulletFire CreateShowFireInMap(DefaultWeaponAbstractFireFireLogicConfig config)
        {
            var defaultFireConfig = config as DefaultFireLogicConfig;
            if (null != defaultFireConfig)
            {
                return new ShowFireInMap(_contexts.ui);
            }
            return null;
        }

        public IAfterFire CreateAutoFireLogic(FireModeLogicConfig config)
        {
            var modeConfig = config as DefaultFireModeLogicConfig;
            if (null != modeConfig)
            {
                foreach (var mode in modeConfig.AvaliableModes)
                {
                    if (mode == EFireMode.Burst)
                    {
                        return new AutoFireLogic();
                    }
                }
            }
            return null;
        }

        public IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config)
        {
            if (config is RifleKickbackLogicConfig)
            {
                return new RifleKickbackLogic();
            }
            else if (config is FixedKickbackLogicConfig)
            {
                return new FixedKickbackLogic();
            }


            return null;
        }

        public IFireCheck CreateFireCheckLogic(FireModeLogicConfig config)
        {
            if (config is DefaultFireModeLogicConfig)
                return new FireBulletModeLogic();
            return null;
        }

        public IFireBulletCounter CreateFireBulletCounterLogic(FireCounterConfig config)
        {
            if (config is RifleFireCounterConfig)
            {
                return new RifleFireBulletCounter();
            }

            return null;
        }

        public IFireActionLogic CreateFireActionLogic(WeaponResConfigItem config)
        {
            if (SingletonManager.Get<WeaponResourceConfigManager>().IsSpecialType(config.Id, ESpecialWeaponType.SniperFrie))
            {
                return new SpecialFireActionLogic();
            }
            else
            {
                return new DefaultFireActionLogic();
            }
        }

        public IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config)
        {
            //TODO 近战特效
            return new DefaultWeaponEffectLogic(_contexts.clientEffect, _entityIdGenerator, config);
        }

        public IFireEffectFactory CreateFireEffectFactory(BulletConfig config)
        {
            return new FireEffectFactory(_contexts.clientEffect, _entityIdGenerator, config);
        }

        /// <summary>
        /// Defines the <see cref="FireEffectFactory" />
        /// </summary>
        public class FireEffectFactory : IFireEffectFactory
        {
            private ClientEffectContext _clientEffectContext;

            private IEntityIdGenerator _idGenerator;

            private BulletConfig _bulletConfig;

            public FireEffectFactory(ClientEffectContext context, IEntityIdGenerator idGenerator, BulletConfig config)
            {
                _clientEffectContext = context;
                _idGenerator = idGenerator;
                _bulletConfig = config;
            }

            public void CreateBulletDropEffect(PlayerWeaponController controller)
            {
                controller.AddAuxEffect(XmlConfig.EClientEffectType.BulletDrop);
            }

            public void CreateSparkEffect(PlayerWeaponController controller)
            {
                controller.AddAuxEffect(XmlConfig.EClientEffectType.MuzzleSpark);
            }
        }

        public IThrowingFactory CreateThrowingFactory(WeaponResConfigItem newWeaponConfig, ThrowingConfig config)
        {
            return new ThrowingFactory(_contexts.throwing, _entityIdGenerator, newWeaponConfig, config);
        }

        public IBulletFire CreateBulletFireLogic(BulletConfig config)
        {
            if (null != config)
            {
                return new DefaultBulletFireLogic();
            }
            return null;
        }

        public IFireTriggger CreateFireCommandLogic(DefaultWeaponAbstractFireFireLogicConfig config)
        {
            var defaultFireLogicConfig = config as DefaultFireLogicConfig;
            if (null != defaultFireLogicConfig)
            {
                return new DefaultFireCmdLogic();
            }
            var meleeFireLogicConfig = config as MeleeFireLogicConfig;
            if (null != meleeFireLogicConfig)
            {
                //TODO 近战武器实现
                return null;
            }
            var throwingFireLogicConfig = config as ThrowingFireLogicConfig;
            if (null != throwingFireLogicConfig)
            {
                //TODO 投掷武器实现
                return null;
            }
            return null;
        }

        /// <summary>
        /// Defines the <see cref="ThrowingFactory" />
        /// </summary>
        public class ThrowingFactory : IThrowingFactory
        {
            private ThrowingConfig _config;

            public ThrowingFactory(ThrowingContext throwingContext, IEntityIdGenerator entityIdGenerator, WeaponResConfigItem newWeaponConfig, ThrowingConfig throwingConfig)
            {
                _throwingContext = throwingContext;
                _entityIdGenerator = entityIdGenerator;
                _newWeaponConfig = newWeaponConfig;
                _config = throwingConfig;
            }

            private ThrowingContext _throwingContext;

            private IEntityIdGenerator _entityIdGenerator;

            private WeaponResConfigItem _newWeaponConfig;

            public int ThrowingHitCount
            {
                get
                {
                    return 0;
                }
            }

            public float ThrowingInitSpeed(bool isNear)
            {
                if (isNear)
                {
                    return _config.NearInitSpeed;
                }
                else
                {
                    return _config.FarInitSpeed;
                }
            }

            public int BombCountdownTime
            {
                get { return _config.CountdownTime; }
            }

            public ThrowingConfig ThrowingConfig
            {
                get { return _config; }
            }

            public EntityKey CreateThrowing(PlayerWeaponController controller, Vector3 direction, int renderTime, float initVel)
            {
                var throwingEntity = ThrowingEntityFactory.CreateThrowingEntity(
                    _throwingContext,
                    _entityIdGenerator,
                    controller,
                    renderTime,
                    direction,
                    initVel,
                    _newWeaponConfig,
                    _config);

                _logger.InfoFormat("CreateThrowing from {0} with velocity {1}, entity key {2}",
                    throwingEntity.position.Value,
                    throwingEntity.throwingData.Velocity,
                    throwingEntity.entityKey);

                return throwingEntity.entityKey.Value;
            }

            public void UpdateThrowing(EntityKey entityKey, bool isThrow, float initVel)
            {
                ThrowingEntity throwing = _throwingContext.GetEntityWithEntityKey(entityKey);
                if (null != throwing)
                {
                    throwing.throwingData.IsThrow = isThrow;
                    throwing.throwingData.InitVelocity = initVel;
                }
            }

            public void DestroyThrowing(EntityKey entityKey)
            {
                ThrowingEntity throwing = _throwingContext.GetEntityWithEntityKey(entityKey);
                if (null != throwing)
                {
                    throwing.isFlagDestroy = true;
                }
            }
        }
    }
}
