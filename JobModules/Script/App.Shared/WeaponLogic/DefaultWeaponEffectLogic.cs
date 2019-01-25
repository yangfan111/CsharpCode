using App.Shared.Components;
using App.Shared.EntityFactory;
using Core.Utils;
using Core.WeaponLogic;
using WeaponConfigNs;
using Utils.CharacterState;
using XmlConfig;
using App.Shared.GameModules.Player;
using Core.Event;

namespace App.Shared.GameModules.Weapon
{
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

        public void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            CreateFireEvent(playerEntity);
        }

        public void CreateFireEvent(PlayerEntity playerEntity)
        {
            if (null == playerEntity)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }
            if (playerEntity.hasLocalEvents)
            {
                var e = EventInfos.Instance.Allocate(EEventType.Fire, false);
                playerEntity.localEvents.Events.AddEvent(e);
            }
        }

        public void CreatePullBoltEffect(PlayerEntity playerEntity)
        {
            if (null == playerEntity)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

         
            if (playerEntity.hasLocalEvents)
            {
                var e = EventInfos.Instance.Allocate(EEventType.PullBolt, false);
                playerEntity.localEvents.Events.AddEvent(e);
            }
        }

        public void PlayBulletDropEffect(PlayerEntity playerEntity)
        {
            if (null == playerEntity)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var appearance = playerEntity.appearanceInterface.Appearance;
            var characterBone = playerEntity.characterBoneInterface.CharacterBone;
            var owner = playerEntity.entityKey.Value;

            var ejectTrans = characterBone.GetLocation(SpecialLocation.EjectionLocation, appearance.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != ejectTrans)
            {
                ClientEffectFactory.CreateBulletDrop(_context, _idGenerator, owner, ejectTrans.position, playerEntity.orientation.Yaw, playerEntity.orientation.Pitch, _config.BulletDrop);
            }
            else
            {
                Logger.Error("Get ejectionLocation location failed");
            }
        }

        public void PlayMuzzleSparkEffect(PlayerEntity playerEntity)
        {
            if (null == playerEntity)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var appearance = playerEntity.appearanceInterface.Appearance;
            var characterBone = playerEntity.characterBoneInterface.CharacterBone;
            var owner = playerEntity.entityKey.Value;
            var muzzleTrans = characterBone.GetLocation(SpecialLocation.MuzzleEffectPosition, appearance.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != muzzleTrans)
            {
                ClientEffectFactory.CreateMuzzleSparkEffct(_context, _idGenerator, owner, muzzleTrans, playerEntity.orientation.Pitch, playerEntity.orientation.Yaw, _config.Spark);
            }
            else
            {
                Logger.Error("Get muzzleLocation location failed");
            }
        }

        public void PlayPullBoltEffect(PlayerEntity playerEntity)
        {
            if (null == playerEntity)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var owner = playerEntity.entityKey.Value;
            var effectPos = PlayerEntityUtility.GetThrowingEmitPosition(playerEntity);
            float effectYaw = (playerEntity.orientation.Yaw + 90)%360;
            float effectPitch = playerEntity.orientation.Pitch;
            int effectId = 32;
            int effectTime = 3000;

            ClientEffectFactory.CreateGrenadeExplosionEffect(_context, _idGenerator,
                            owner, effectPos, effectYaw, effectPitch, effectId, effectTime, EClientEffectType.PullBolt);
        }
    }
}
