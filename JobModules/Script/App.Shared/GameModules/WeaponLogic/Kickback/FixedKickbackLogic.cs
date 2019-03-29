using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedKickbackLogic" />
    /// </summary>
    public class FixedKickbackLogic : AbstractKickbackLogic<FixedKickbackLogicConfig>
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(FixedKickbackLogic));

        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.FixedKickbackLogicCfg;
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
