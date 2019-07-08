using App.Shared.Components;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon.Behavior;
using Core.Configuration;
using Core.Event;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    ///     Defines the <see cref="AfterFireEffectEventListener" />
    /// </summary>
    public class AfterFireEffectEventListener : IAfterFireProcess
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AfterFireEffectEventListener));

        protected WeaponEffectConfig _config;

        protected ClientEffectContext _context;

        protected IEntityIdGenerator _idGenerator;

        public AfterFireEffectEventListener(ClientEffectContext context, IEntityIdGenerator idGenerator,
                                            WeaponEffectConfig config)
        {
            _config      = config;
            _context     = context;
            _idGenerator = idGenerator;
        }

        protected DefaultWeaponEffectConfig DefaultCfg
        {
            get { return _config as DefaultWeaponEffectConfig; }
        }

        public void OnAfterFire(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            if (DefaultCfg == null)
                return;
            var weaponController = weaponBaseAgent.Owner.WeaponController();
            CreateFireRelatedEffect(weaponController);
            CreateBulletEffect(weaponController);
        }

        private void CreateBulletEffect(PlayerWeaponController controller)
        {
            if (DefaultCfg.BulletFly < 1)
                return;
            if (controller.RelatedCameraSNew.IsAiming())
                return;
            var muzzleTrans = controller.RelatedBones.GetLocation(SpecialLocation.MuzzleEffectPosition,
                controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (!muzzleTrans)
                return;
            var efcCommonCfg = SingletonManager.Get<ClientEffectCommonConfigManager>()
                            .GetConfigByType(EEffectObjectClassify.BulletFly);
            foreach (var bulletData in controller.BulletList)
            {
                var origin = bulletData.ViewPosition;
                var velocity = efcCommonCfg.Velocity * bulletData.Dir;

                Vector3 target               = origin + velocity * 1.0f;
                Vector3 bulletEffectVelocity = (target - bulletData.EmitPosition) / 1.0f;
                ClientEffectFactory.CreateBulletFlyEffect(controller.Owner, muzzleTrans.position, muzzleTrans.rotation,
                    bulletEffectVelocity, DefaultCfg.BulletFly,efcCommonCfg.Delay);
            }
        }


        private void AddFireEvent(PlayerWeaponController controller)
        {
        }

        private void AddPullBoltEvent(PlayerWeaponController controller)
        {
            if (controller.RelatedLocalEvents != null)
            {
                var e = EventInfos.Instance.Allocate(EEventType.PullBolt, false);
                controller.RelatedLocalEvents.AddEvent(e);
            }
        }

        protected virtual void CreateFireRelatedEffect(PlayerWeaponController controller)
        {
            if (null == controller || DefaultCfg.Spark < 1)
                return;
            var muzzleTrans = controller.RelatedBones.GetLocation(SpecialLocation.MuzzleEffectPosition,
                controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            var ejectTrans = controller.RelatedBones.GetLocation(SpecialLocation.EjectionLocation,
                controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            bool hasMuzzleEfc  = null != muzzleTrans && DefaultCfg.Spark > 0;
            bool hasBulletDrop = null != ejectTrans && DefaultCfg.BulletDrop > 0;
            if (hasMuzzleEfc)
                ClientEffectFactory.CreateMuzzleSparkEffct(muzzleTrans.position, controller.RelatedOrientation.Yaw,controller.RelatedOrientation.Pitch,DefaultCfg.Spark,muzzleTrans);
            if (hasBulletDrop)
                ClientEffectFactory.CreateBulletDrop(ejectTrans.position,
                    controller.RelatedOrientation.Yaw, controller.RelatedOrientation.Pitch, DefaultCfg.BulletDrop,
                    controller.HeldConfigId, AudioGrp_FootMatType.Concrete);
        }

        private void CreatePullBoltEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var   effectPos   = PlayerEntityUtility.GetThrowingEmitPosition(controller);
            float effectYaw   = (controller.RelatedOrientation.Yaw + 90) % 360;
            float effectPitch = controller.RelatedOrientation.Pitch;
            int   effectId    = 32;
            int   effectTime  = 3000;

            ClientEffectFactory.CreateGrenadeExplosionEffect(_context, _idGenerator, controller.Owner, effectPos,
                effectYaw, effectPitch, effectId, effectTime, EClientEffectType.PullBolt);
        }
    }
}