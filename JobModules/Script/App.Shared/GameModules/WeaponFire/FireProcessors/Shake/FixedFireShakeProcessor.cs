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

        public override void OnAfterFire(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            var config = agent.FixedShakeCfg;
            if (null == config)
            {
                return;
            }

            var orientation = agent.Owner.WeaponController().RelatedOrientation;
            orientation.AccPunchPitch += config.PunchPitch;
            orientation.AccPunchPitchValue += config.PunchPitch * config.PunchOffsetFactor;
        }

        public override void OnFrame(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            UpdateOrientationAttenuation(agent, cmd);
            base.OnFrame(agent,cmd);
        }

        protected override float GePuntchFallbackFactor(PlayerWeaponController controller)
        {
            return controller.HeldWeaponAgent.FallbackOffsetFactor;
        }
    }
}
