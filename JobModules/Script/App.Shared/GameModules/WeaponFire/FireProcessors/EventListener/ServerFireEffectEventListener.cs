using App.Shared.Components;
using App.Shared.Player.Events;
using Core;
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

        protected override void CreateFireRelatedEffect(PlayerWeaponController controller)
        {
            if (null == controller || DefaultCfg.Spark < 1)
                return;
            var muzzleTrans = controller.RelatedBones.GetLocation(SpecialLocation.MuzzleEffectPosition,
            controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            var ejectTrans = controller.RelatedBones.GetLocation(SpecialLocation.EjectionLocation,
            controller.RelatedAppearence.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            bool hasMuzzleEfc  = null != muzzleTrans && DefaultCfg.Spark > 0;
            bool hasBulletDrop = null != ejectTrans && DefaultCfg.BulletDrop > 0;
            if (!hasMuzzleEfc && !hasBulletDrop)
                return;
            FireEvent.FireEffectType fireEffectType = FireEvent.FireEffectType.Both;
            if (!hasMuzzleEfc)
                fireEffectType = FireEvent.FireEffectType.EnjectOnly;
            if(!hasBulletDrop)
                fireEffectType = FireEvent.FireEffectType.MuzzleOnly;

            FireEvent e = (FireEvent) EventInfos.Instance.Allocate(EEventType.Fire, false);
            e.owner = controller.Owner ;
            e.fireEffectType = fireEffectType;
            e.pitch          = controller.RelatedOrientation.Pitch;
            e.yaw            = controller.RelatedOrientation.Yaw;
            e.weaponId       = controller.HeldConfigId;
            controller.RelatedLocalEvents.AddEvent(e);
        }
    }
}