using Core.Utils;
using WeaponConfigNs;
using Core.WeaponLogic.Attachment;

namespace Core.WeaponLogic.Kickback
{
    public class FixedKickbackLogic : AbstractKickbackLogic<FixedKickbackLogicConfig, float>
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(FixedKickbackLogic));
        public FixedKickbackLogic(FixedKickbackLogicConfig config):base(config)
        {
        }

        public override void AfterFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            if(null == _config)
            {
                Logger.Error("config is null");
                return;
            }

            playerWeapon.NegPunchPitch += _config.PunchPitch;
            playerWeapon.WeaponPunchPitch += _config.PunchPitch * _config.PunchOffsetFactor;
        }

        public override void SetVisualConfig(ref VisualConfigGroup config)
        {
            config.KickBack = _config;
        }

        protected override float GetWeaponPunchYawFactor(IPlayerWeaponState playerWeapon)
        {
            return _config.FallbackOffsetFactor;
        }
        
        public override void Apply(FixedKickbackLogicConfig baseConfig, FixedKickbackLogicConfig output, float arg)
        {
            var realFactor = arg > 0 ? arg : 1;
            output.PunchPitch = baseConfig.PunchPitch * realFactor; 
        }
    }
}
