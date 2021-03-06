﻿using App.Shared.Components;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Weapon.Behavior;
using Core.CharacterBone;
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

        public void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (DefaultCfg == null)
                return;
            var bones = attackProxy.Owner.RelatedBones;
            CreateFireRelatedEffect(attackProxy,bones);
            CreateBulletEffect(attackProxy,bones);
        }


        private void CreateBulletEffect(WeaponAttackProxy attackProxy, ICharacterBone bones)
        {
            if (DefaultCfg.BulletFly < 1 || attackProxy.IsAiming)
                return;
            var muzzleTrans = bones.GetLocation(SpecialLocation.MuzzleEffectPosition,
                attackProxy.Appearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (!muzzleTrans)
                return;
            var efcCommonCfg = SingletonManager.Get<ClientEffectCommonConfigManager>()
                            .GetConfigByType(EEffectObjectClassify.BulletFly);
            foreach (var bulletData in attackProxy.Owner.BulletList)
            {
                var origin   = bulletData.ViewPosition;
                var velocity = efcCommonCfg.Velocity * bulletData.Dir;

                Vector3 target               = origin + velocity * 1.0f;
                Vector3 bulletEffectVelocity = (target - bulletData.EmitPosition) / 1.0f;
                ClientEffectFactory.CreateBulletFlyEffect(attackProxy.Owner.Owner, muzzleTrans.position,
                    muzzleTrans.rotation, bulletEffectVelocity, DefaultCfg.BulletFly, efcCommonCfg.Delay);
            }
        }


        private void AddFireEvent(PlayerWeaponController controller)
        {
        }

        private void AddPullBoltEvent(WeaponAttackProxy attackProxy)
        {
            var e = EventInfos.Instance.Allocate(EEventType.PullBolt, false);
            attackProxy.Owner.RelatedLocalEvents.AddEvent(e);
        }

        protected virtual void CreateFireRelatedEffect(WeaponAttackProxy attackProxy, ICharacterBone bones)

        {
            if (DefaultCfg.Spark < 1)
                return;
            var muzzleTrans = bones.GetLocation(SpecialLocation.MuzzleEffectPosition,
                attackProxy.Appearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            var ejectTrans = bones.GetLocation(SpecialLocation.EjectionLocation,
                attackProxy.Appearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);

            bool hasMuzzleEfc  = muzzleTrans && DefaultCfg.Spark > 0 && attackProxy.Owner.HeldWeaponAgent.HasSpark;
            bool hasBulletDrop = ejectTrans && DefaultCfg.BulletDrop > 0;
            if (hasMuzzleEfc)
                ClientEffectFactory.CreateMuzzleSparkEffct(muzzleTrans.position, attackProxy.Orientation.Yaw,
                    attackProxy.Orientation.Pitch, DefaultCfg.Spark, muzzleTrans);
            if (hasBulletDrop)
                ClientEffectFactory.CreateBulletDrop(ejectTrans.position, attackProxy.Orientation.Yaw,
                    attackProxy.Orientation.Pitch, DefaultCfg.BulletDrop, attackProxy.WeaponConfigAssy.S_Id,
                    attackProxy.AudioController.GetFootMatType());
        }

        private void CreatePullBoltEffect(PlayerWeaponController controller)
        {
            if (null == controller)
            {
                Logger.Error("player state owner is not player or null !");
                return;
            }

            var   effectPos   = SingletonManager.Get<ThrowAmmunitionCalculator>().GetFireViewPosition(controller);
            float effectYaw   = (controller.RelatedOrientation.Yaw + 90) % 360;
            float effectPitch = controller.RelatedOrientation.Pitch;
            int   effectId    = 32;
            int   effectTime  = 3000;

            ClientEffectFactory.CreateGrenadeExplosionEffect(_context, _idGenerator, controller.Owner, effectPos,
                effectYaw, effectPitch, effectId, effectTime, EClientEffectType.PullBolt);
        }
    }
}