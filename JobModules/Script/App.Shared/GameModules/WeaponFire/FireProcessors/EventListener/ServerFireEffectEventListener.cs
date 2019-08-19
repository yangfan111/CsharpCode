using App.Shared.Components;
using App.Shared.Player.Events;
using Core;
using Core.CharacterBone;
using Core.EntityComponent;
using Core.Event;
using Utils.CharacterState;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    ///     Defines the <see cref="AfterFireEffectEventListener" />
    /// </summary>
    public class ServerFireEffectEventListener : AfterFireEffectEventListener
    {
        public ServerFireEffectEventListener(ClientEffectContext context, IEntityIdGenerator idGenerator,
                                             WeaponEffectConfig config) : base(context, idGenerator, config)
        {
        }

        protected override void CreateFireRelatedEffect(WeaponAttackProxy attackProxy, ICharacterBone bones)
        {
            if (DefaultCfg.Spark < 1)
                return;
            var muzzleTrans = bones.GetLocation(SpecialLocation.MuzzleEffectPosition,
            attackProxy.Appearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            var ejectTrans = bones.GetLocation(SpecialLocation.EjectionLocation, 
                attackProxy.Appearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            bool hasMuzzleEfc  = null != muzzleTrans && DefaultCfg.Spark > 0 && attackProxy.Owner.HeldWeaponAgent.HasSpark;
            bool hasBulletDrop = null != ejectTrans && DefaultCfg.BulletDrop > 0;
            if (!hasMuzzleEfc && !hasBulletDrop)
                return;
            FireEvent.FireEffectType fireEffectType = FireEvent.FireEffectType.Both;
            if (!hasMuzzleEfc)
                fireEffectType = FireEvent.FireEffectType.EnjectOnly;
            if(!hasBulletDrop)
                fireEffectType = FireEvent.FireEffectType.MuzzleOnly;

            FireEvent e = (FireEvent) EventInfos.Instance.Allocate(EEventType.Fire, false);
            e.owner = attackProxy.Owner.Owner;
            e.fireEffectType = fireEffectType;
            e.pitch          = attackProxy.Orientation.Pitch;
            e.yaw            = attackProxy.Orientation.Yaw;
            e.weaponId       = attackProxy.WeaponConfigAssy.S_Id;
            attackProxy.Owner.RelatedLocalEvents.AddEvent(e);
        }
    }
}