using App.Shared.Components;
using App.Shared.EntityFactory;
using Core.Utils;
using Core.WeaponLogic;
using UnityEngine;
using WeaponConfigNs;
using Core.EntityComponent;
using Core.WeaponLogic.WeaponLogicInterface;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.FireAciton;
using Utils.Singleton;
using Core.WeaponLogic.Accuracy;
using Assets.Utils.Configuration;
using Core.WeaponLogic.Common;
using Core.WeaponLogic.Kickback;
using App.Shared.WeaponLogic.Common;
using App.Shared.WeaponLogic.FireLogic;
using App.Shared.GameModules.Weapon;

namespace App.Shared.WeaponLogic
{
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

        public IWeaponLogic CreateWeaponLogic(NewWeaponConfigItem newCfg,
        WeaponConfig config,
        IWeaponSoundLogic soundLogic,
        IWeaponEffectLogic effectLogic)
        {
            IWeaponLogic rc = null;
            var weaponLogicConfig = config.WeaponLogic;
            if (weaponLogicConfig is DefaultWeaponLogicConfig)
            {
                rc = new DefaultWeaponLogic();
            }
            else if (weaponLogicConfig is DoubleWeaponLogicConfig)
            {
                rc = new DoubleWeaponLogic(null, null);
            }
            return rc;
        }

        public IAccuracyLogic CreateAccuracyLogic(AccuracyLogicConfig config)
        {
            if (config is BaseAccuracyLogicConfig)
            {
                return new BaseAccuracyLogic(_contexts);
            }
            else if (config is PistolAccuracyLogicConfig)
            {
                return new PistolAccuracyLogic(_contexts);
            }

            return null;
        }

        public ISpreadLogic CreateSpreadLogic(SpreadLogicConfig config)
        {
            if (config is FixedSpreadLogicConfig)
            {
                return new FixedSpreadLogic(_contexts);
            }
            else if (config is PistolSpreadLogicConfig)
            {
                return new PistolSpreadLogic(_contexts);
            }
            else if (config is SniperSpreadLogicConfig)
            {
                return new SniperSpreadLogic(_contexts);
            }
            else if (config is RifleSpreadLogicConfig)
            {
                return new RifleSpreadLogic(_contexts);
            }

            return null;
        }

        public IFireCheck CreateSpecialReloadCheckLogic(CommonFireConfig commonFireConfig)
        {
            if (null != commonFireConfig && commonFireConfig.SpecialReloadCount > 0)
            {
                return new SpecialReloadCheckLogic(_contexts);
            }
            return null;
        }

        public IBulletFire CreateShowFireInMap(FireLogicConfig config)
        {
            var defaultFireConfig = config as DefaultFireLogicConfig;
            if(null != defaultFireConfig)
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
                        return new AutoFireLogic(_contexts);
                    }
                }
            }
            return null;
        }

        public IKickbackLogic CreateKickbackLogic(KickbackLogicConfig config)
        {
            if (config is RifleKickbackLogicConfig)
            {
                return new RifleKickbackLogic(_contexts);
            }
            else if (config is FixedKickbackLogicConfig)
            {
                return new FixedKickbackLogic(_contexts);
            }


            return null;
        }

        public IFireCheck CreateFireCheckLogic(FireModeLogicConfig config)
        {
            if (config is DefaultFireModeLogicConfig)
                return new FireBulletModeLogic(_contexts);
            return null;
        }

        public IFireBulletCounter CreateFireBulletCounterLogic(FireCounterConfig config)
        {
            if (config is RifleFireCounterConfig)
            {
                return new RifleFireBulletCounter(_contexts);
            }

            return null;
        }

        public IFireActionLogic CreateFireActionLogic(NewWeaponConfigItem config)
        {
            if (SingletonManager.Get<WeaponConfigManager>().IsSpecialType(config.Id, ESpecialWeaponType.SniperFrie))
            {
                return new SpecialFireActionLogic(_contexts);
            }
            else
            {
                return new DefaultFireActionLogic(_contexts);
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

            public void CreateBulletDropEffect(PlayerEntity playerEntity)
            {
                if(playerEntity.hasWeaponEffect)
                {
                    playerEntity.weaponEffect.PlayList.Add(XmlConfig.EClientEffectType.BulletDrop);
                }
            }

            public void CreateSparkEffect(PlayerEntity playerEntity)
            {
                if(playerEntity.hasWeaponEffect)
                {
                    playerEntity.weaponEffect.PlayList.Add(XmlConfig.EClientEffectType.MuzzleSpark);
                }
            }
        }

        public IThrowingFactory CreateThrowingFactory(NewWeaponConfigItem newWeaponConfig, ThrowingConfig config)
        {
            return new ThrowingFactory(_contexts.throwing, _entityIdGenerator, newWeaponConfig, config);
        }

        public IBulletFire CreateBulletFireLogic(BulletConfig config)
        {
            if(null != config)
            {
                return new DefaultBulletFireLogic(_contexts);
            }
            return null;
        }

        public IFireTriggger CreateFireCommandLogic(FireLogicConfig config)
        {
            var defaultFireLogicConfig = config as DefaultFireLogicConfig;
            if(null != defaultFireLogicConfig)
            {
                return new DefaultFireCmdLogic();
            }
            var meleeFireLogicConfig =  config as MeleeFireLogicConfig;
            if(null != meleeFireLogicConfig)
            {
                //TODO 近战武器实现
                return null;
            }
            var throwingFireLogicConfig = config as ThrowingFireLogicConfig;
            if(null != throwingFireLogicConfig)
            {
                //TODO 投掷武器实现
                return null;
            }
            return null;
        }

        public class ThrowingFactory : IThrowingFactory
        {
            private ThrowingConfig _config;
            public ThrowingFactory(ThrowingContext throwingContext, IEntityIdGenerator entityIdGenerator, NewWeaponConfigItem newWeaponConfig, ThrowingConfig throwingConfig)
            {
                _throwingContext = throwingContext;
                _entityIdGenerator = entityIdGenerator;
                _newWeaponConfig = newWeaponConfig;
                _config = throwingConfig;
            }

            private ThrowingContext _throwingContext;
            private IEntityIdGenerator _entityIdGenerator;
            private NewWeaponConfigItem _newWeaponConfig;

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

            public EntityKey CreateThrowing(PlayerEntity playerEntity, Vector3 direction, int renderTime, float initVel)
            {
                var throwingEntity = ThrowingEntityFactory.CreateThrowingEntity(
                    _throwingContext,
                    _entityIdGenerator,
                    playerEntity,
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