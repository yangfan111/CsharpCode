using App.Shared.Components;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon.Behavior;
using Core.Event;
using Core.Utils;
using Utils.CharacterState;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="DefaultWeaponEffectLogic" />
    /// </summary>
    public class DefaultWeaponEffectLogic : IWeaponEffectLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultWeaponEffectLogic));

        private ClientEffectContext _context;

        private IEntityIdGenerator _idGenerator;

        private DefaultWeaponEffectConfig _config;

        public DefaultWeaponEffectLogic(ClientEffectContext context, IEntityIdGenerator idGenerator, DefaultWeaponEffectConfig config)
        {
            _config = config;
            _context = context;
            _idGenerator = idGenerator;
        }

        public void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            CreateFireEvent(controller);
        }

        public void CreateFireEvent(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }
            if (controller.RelatedLocalEvents != null)
            {
                var e = EventInfos.Instance.Allocate(EEventType.Fire, false);
                controller.RelatedLocalEvents.Events.AddEvent(e);
            }
        }

        public void CreatePullBoltEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }


            if (controller.RelatedLocalEvents != null)
            {
                var e = EventInfos.Instance.Allocate(EEventType.PullBolt, false);
                controller.RelatedLocalEvents.Events.AddEvent(e);
            }
        }

        public void PlayBulletDropEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }
            var ejectTrans = controller.RelatedBones.GetLocation(SpecialLocation.EjectionLocation, controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != ejectTrans)
            {
                ClientEffectFactory.CreateBulletDrop(_context, _idGenerator, controller.Owner, ejectTrans.position, controller.RelatedOrient.Yaw, controller.RelatedOrient.Pitch, _config.BulletDrop);
            }
            else
            {
                Logger.Error("Get ejectionLocation location failed");
            }
        }

        public void PlayMuzzleSparkEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }
            var muzzleTrans = controller.RelatedBones.GetLocation(SpecialLocation.MuzzleEffectPosition, controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != muzzleTrans)
            {
                ClientEffectFactory.CreateMuzzleSparkEffct(_context, _idGenerator, controller.Owner, muzzleTrans, controller.RelatedOrient.Pitch, controller.RelatedOrient.Yaw, _config.Spark);
            }
            else
            {
                Logger.Error("Get muzzleLocation location failed");
            }
        }

        public void PlayPullBoltEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var effectPos = PlayerEntityUtility.GetThrowingEmitPosition(controller);
            float effectYaw = (controller.RelatedOrient.Yaw + 90) % 360;
            float effectPitch = controller.RelatedOrient.Pitch;
            int effectId = 32;
            int effectTime = 3000;

            ClientEffectFactory.CreateGrenadeExplosionEffect(_context, _idGenerator,
                            controller.Owner, effectPos, effectYaw, effectPitch, effectId, effectTime, EClientEffectType.PullBolt);
        }
    }
}
