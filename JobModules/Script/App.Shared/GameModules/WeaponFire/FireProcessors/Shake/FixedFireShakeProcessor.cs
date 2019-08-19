using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedFireShakeProcessor" />
    /// </summary>
    public class FixedFireShakeProcessor : AbstractFireShakeProcessor
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(FixedFireShakeProcessor));

        public override void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var config = attackProxy.WeaponConfigAssy.SFixedShakeCfg;
            if (null == config)
            {
                return;
            }
            attackProxy.Orientation.AccPunchPitch += config.PunchPitch;
            attackProxy.Orientation.AccPunchPitchValue += config.PunchPitch * config.PunchOffsetFactor;
        }

        public override void OnFrame(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            UpdateOrientationAttenuation(attackProxy, cmd);
            RecoverFireRoll(attackProxy, cmd);
            base.OnFrame(attackProxy,cmd);
        }

        protected override float GePuntchFallbackFactor(PlayerWeaponController controller)
        {
            return controller.HeldWeaponAgent.FallbackOffsetFactor;
        }
    }
}
