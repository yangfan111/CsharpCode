using System.Reflection;
using App.Shared.Components;
using App.Shared.EntityFactory;
using Core.Utils;
using Core.WeaponLogic;
using UnityEngine;
using WeaponConfigNs;
using Core.WeaponLogic.Common;
using Utils.CharacterState;
using XmlConfig;
using App.Shared.GameModules.Player;
using Core.Event;

namespace App.Shared.WeaponLogic
{
    public class DefaultWeaponEffectLogic : AbstractAttachableWeaponLogic<DefaultWeaponEffectConfig, int>, IWeaponEffectLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultWeaponEffectLogic));
        private ClientEffectContext _context;
        private IEntityIdGenerator _idGenerator;
        private DefaultWeaponLogicConfig _defaultWeaponLogicConfig;
        private DoubleWeaponLogicConfig _doubleweapnLogicConfig;

        public DefaultWeaponEffectLogic(ClientEffectContext context, IEntityIdGenerator idGenerator, DefaultWeaponEffectConfig config): base(config)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public override void Apply(DefaultWeaponEffectConfig baseConfig, DefaultWeaponEffectConfig output, int arg)
        {
            output.Spark = arg != 0 ? arg : baseConfig.Spark;
        }

        public void CreateFireEvent(IPlayerWeaponState playerState)
        {
            var player = playerState.Owner as PlayerEntity;
            if (null == player)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }
            if (player.hasLocalEvents)
            {
                var e = EventInfos.Instance.Allocate(EEventType.Fire, false);
                player.localEvents.Events.AddEvent(e);
            }
        }

        public void CreatePullBoltEffect(IPlayerWeaponState playerState)
        {
            var player = playerState.Owner as PlayerEntity;
            if (null == player)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

         
            if (player.hasLocalEvents)
            {
                var e = EventInfos.Instance.Allocate(EEventType.PullBolt, false);
                player.localEvents.Events.AddEvent(e);
            }
        }

        public void PlayBulletDropEffect(IPlayerWeaponState playerState)
        {
            var player = playerState.Owner as PlayerEntity;
            if (null == player)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var appearance = player.appearanceInterface.Appearance;
            var characterBone = player.characterBoneInterface.CharacterBone;
            var owner = player.entityKey.Value;

            var ejectTrans = characterBone.GetLocation(SpecialLocation.EjectionLocation, appearance.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != ejectTrans)
            {
                ClientEffectFactory.CreateBulletDrop(_context, _idGenerator, owner, ejectTrans.position, playerState.ViewYaw, playerState.ViewPitch, _config.BulletDrop);
            }
            else
            {
                Logger.Error("Get ejectionLocation location failed");
            }
        }

        public void PlayMuzzleSparkEffect(IPlayerWeaponState playerState)
        {
            var player = playerState.Owner as PlayerEntity;
            if (null == player)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var appearance = player.appearanceInterface.Appearance;
            var characterBone = player.characterBoneInterface.CharacterBone;
            var owner = player.entityKey.Value;
            var muzzleTrans = characterBone.GetLocation(SpecialLocation.MuzzleEffectPosition, appearance.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != muzzleTrans)
            {
                ClientEffectFactory.CreateMuzzleSparkEffct(_context, _idGenerator, owner, muzzleTrans, playerState.ViewPitch, playerState.ViewYaw, _config.Spark);
            }
            else
            {
                Logger.Error("Get muzzleLocation location failed");
            }
        }

        public void PlayPullBoltEffect(IPlayerWeaponState playerState)
        {
            var player = playerState.Owner as PlayerEntity;
            if (null == player)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var owner = player.entityKey.Value;
            var effectPos = PlayerEntityUtility.GetThrowingEmitPosition(player);
            float effectYaw = (playerState.ViewYaw + 90)%360;
            float effectPitch = playerState.ViewPitch;
            int effectId = 32;
            int effectTime = 3000;

            ClientEffectFactory.CreateGrenadeExplosionEffect(_context, _idGenerator,
                            owner, effectPos, effectYaw, effectPitch, effectId, effectTime, EClientEffectType.PullBolt);
        }
    }
}
