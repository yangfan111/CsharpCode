using App.Shared.Components;
using App.Shared.EntityFactory;
using Core.Utils;
using Core.WeaponLogic;
using Core.WeaponLogic.Attachment;
using UnityEngine;
using WeaponConfigNs;
using Core.GameTime;
using Assets.App.Shared.WeaponLogic;
using Core.WeaponLogic.Common;
using Core.EntityComponent;
using Core.IFactory;
using Core.WeaponLogic.Bullet;

namespace App.Shared.WeaponLogic
{
    public class WeaponLogicComponentsFactory : AbstractWeaponLogicComponentsFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WeaponLogicComponentsFactory));
        private BulletContext _bulletContext;
        private ThrowingContext _throwingContext;
        private ClientEffectContext _clientEffectContext;
        private IEntityIdGenerator _entityIdGenerator;
        private SoundContext _soundContext;
        private ICurrentTime _currentTime;
        private ISoundEntityFactory _soundEntityFactory;
        private IBulletEntityFactory _bulletEntityFactory;

        public WeaponLogicComponentsFactory(
            Contexts contexts,
            IEntityIdGenerator entityIdGenerator,
            ICurrentTime currentTime,
            IAttachmentManager attachmentManager,
            ISoundEntityFactory soundEntityFactory,
            IBulletEntityFactory bulletEntityFactory):base(attachmentManager)
        {
            _bulletContext = contexts.bullet;
            _throwingContext = contexts.throwing;
            _clientEffectContext = contexts.clientEffect;
            _soundContext = contexts.sound;
            _entityIdGenerator = entityIdGenerator;
            _currentTime = currentTime;
            _attachmentManager = attachmentManager;
            _soundEntityFactory = soundEntityFactory;
            _bulletEntityFactory = bulletEntityFactory;
        }

        public override IWeaponEffectLogic CreateEffectLogic(DefaultWeaponEffectConfig config)
        {
            //TODO 近战特效
            return new DefaultWeaponEffectLogic(_clientEffectContext, _entityIdGenerator, config);
        }

        public override IWeaponSoundLogic CreateSoundLogic(IPlayerWeaponState weaponState, WeaponLogicConfig config)
        {
            if(null == config)
            {
                return null;
            }
            var soundCfg = config.SoundConfig;
            var defaultCfg = soundCfg as DefaultWeaponSoundConfig;
            if(null != defaultCfg)
            {
                return new DefaultWeaponSoundLogic(_soundEntityFactory, weaponState, defaultCfg);
            }
            var meleeCfg = soundCfg as MeleeWeaponSoundConfig;
            if(null != meleeCfg)
            {
                return new MeleeWeaponSoundLogic(weaponState, _soundEntityFactory, meleeCfg);
            }
            return null;
        }

        public override IBulletFactory CreateBulletFactory(BulletConfig config, CommonFireConfig common)
        {
            return new BulletFactory(_bulletContext, _entityIdGenerator, _soundEntityFactory, _bulletEntityFactory, config);
        }

        public class BulletFactory : AbstractAttachableWeaponLogic<BulletConfig, float>, IBulletFactory
        {
            public BulletFactory(BulletContext bulletContext, 
                IEntityIdGenerator entityIdGenerator, 
                ISoundEntityFactory soundEntityFactory,
                IBulletEntityFactory bulletEntityFactory,
                BulletConfig bulletConfig) : base(bulletConfig)
            {
                _bulletContext = bulletContext;
                _entityIdGenerator = entityIdGenerator;
                _soundEntityFacotry = soundEntityFactory;
                _bulletEntityFactory = bulletEntityFactory;
            }

            public override void Apply(BulletConfig baseConfig, BulletConfig output, float arg)
            {
                output.EmitVelocity = baseConfig.EmitVelocity * (arg> 0 ? arg : 1);
            }
            private BulletContext _bulletContext;
            private IEntityIdGenerator _entityIdGenerator;
            private ISoundEntityFactory _soundEntityFacotry;
            private IBulletEntityFactory _bulletEntityFactory;
            public int BulletHitCount
            {
                get
                {
                    return _config.HitCount;
                }
            }

            public void CreateBullet(IPlayerWeaponState playerWeapon, 
                Vector3 direction, 
                IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher,
                int cmdSeq, 
                int renderTime)
            {
                PlayerEntity playerEntity = (PlayerEntity)playerWeapon.Owner;
                var bulletEntity = _bulletEntityFactory.CreateBulletEntity(
                    cmdSeq,
                    playerEntity.weaponLogicInfo.WeaponId,
                    playerEntity.entityKey.Value,
                    renderTime,
                    direction, 
                    bulletFireInfoProviderDispatcher,
                    _config,
                    playerWeapon.Caliber) as BulletEntity;

                if(null != bulletEntity)
                {
                    _logger.DebugFormat("Fire from {0} with velocity {1}, entity key {2}, cmd {3}",
                        bulletEntity.position.Value,
                        bulletEntity.bulletData.Velocity,
                        bulletEntity.entityKey,
                        cmdSeq);
                }
            }
        }

        public override IFireEffectFactory CreateFireEffectFactory(BulletConfig config)
        {
           return new FireEffectFactory(_clientEffectContext, _entityIdGenerator, config); 
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

            public void CreateBulletDropEffect(IPlayerWeaponState playerWeapon)
            {
                 var player = playerWeapon.Owner as PlayerEntity;
                player.weaponLogic.WeaponEffect.PlayBulletDropEffect(playerWeapon);
            }

            public void CreateSparkEffect(IPlayerWeaponState playerWeapon)
            {
                 var player = playerWeapon.Owner as PlayerEntity;
                player.weaponLogic.WeaponEffect.PlayMuzzleSparkEffect(playerWeapon);
            }
        }

        public override IThrowingFactory CreateThrowingFactory(NewWeaponConfigItem newWeaponConfig, ThrowingConfig config)
        {
            return new ThrowingFactory(_throwingContext, _entityIdGenerator, newWeaponConfig, config);
        }

        public class ThrowingFactory : AbstractAttachableWeaponLogic<ThrowingConfig, ThrowingModifierArg>, IThrowingFactory
        {
            public ThrowingFactory(ThrowingContext throwingContext, IEntityIdGenerator entityIdGenerator, NewWeaponConfigItem newWeaponConfig, ThrowingConfig throwingConfig) : base(throwingConfig)
            {
                _throwingContext = throwingContext;
                _entityIdGenerator = entityIdGenerator;
                _newWeaponConfig = newWeaponConfig;
            }

            public override void Apply(ThrowingConfig baseConfig, ThrowingConfig output, ThrowingModifierArg arg)
            {
            
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

            public EntityKey CreateThrowing(IPlayerWeaponState playerWeapon, Vector3 direction, int renderTime, float initVel)
            {
                PlayerEntity playerEntity = (PlayerEntity)playerWeapon.Owner;
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