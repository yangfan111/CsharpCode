using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedFireShakeProcessor" />
    /// </summary>
    public class FixedFireShakeProcessor : AbstractFireShakeProcessor
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(FixedFireShakeProcessor));

        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.FixedShakeCfg;
            if (null == config)
            {
                return;
            }
            controller.RelatedOrientation.AccPunchPitch += config.PunchPitch;
            controller.RelatedOrientation.AccPunchPitchValue += config.PunchPitch * config.PunchOffsetFactor;
        }

        public override void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            UpdateOrientationAttenuation(controller, cmd);
            base.OnFrame(controller,cmd);
        }

        protected override float GePuntchFallbackFactor(PlayerWeaponController controller)
        {
            return controller.HeldWeaponAgent.FallbackOffsetFactor;
        }
    }
}
