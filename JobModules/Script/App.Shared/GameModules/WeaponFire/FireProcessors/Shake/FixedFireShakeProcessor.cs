using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedFireShakeProcessor" />
    /// </summary>
    public class FixedFireShakeProcessor : AbstractFireShakeProcessor<FixedShakeConfig>
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(FixedFireShakeProcessor));

        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.FixedShakeCfg;
            if (null == config)
            {
                return;
            }

            controller.RelatedOrient.NegPunchPitch += config.PunchPitch;
            controller.RelatedOrient.WeaponPunchPitch += config.PunchPitch * config.PunchOffsetFactor;
        }

        protected override float GetWeaponPunchYawFactor(PlayerWeaponController controller)
        {
            return controller.HeldWeaponAgent.FallbackOffsetFactor;
        }
    }
}
