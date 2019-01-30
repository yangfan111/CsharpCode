using WeaponConfigNs;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Spread;

namespace Core.WeaponLogic
{
    public class PistolSpreadLogic : AbstractSpreadLogic<PistolSpreadLogicConfig,  float>
    {
        public PistolSpreadLogic(PistolSpreadLogicConfig config) : base(config)
        {
        }

        public override void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            float spread = UpdateSpread(playerWeapon, playerWeapon.LastAccuracy);
            playerWeapon.LastSpreadX = spread * _config.SpreadScale.ScaleX;
            playerWeapon.LastSpreadY = spread * _config.SpreadScale.ScaleY;
        }

        protected override float UpdateSpread(IPlayerWeaponState playerWeapon, float accuracy)
        {
            float param;
            if (playerWeapon.IsAir)
                param = _config.AirParam;
            else if (playerWeapon.HorizontalVeocity > 13)
                param = _config.LengthGreater13Param;
            else if (playerWeapon.IsDuckOrProneState)
                param = _config.DuckParam;
            else
                param = _config.DefaultParam;
            return param * (1 - accuracy);
        }
        public override void Apply(PistolSpreadLogicConfig baseConfig, PistolSpreadLogicConfig output, float arg)
        {
            var percent = arg > 0 ? arg : 1;
            output.AirParam = baseConfig.AirParam * percent;
            output.DefaultParam = baseConfig.DefaultParam * percent;
            output.DuckParam = baseConfig.DuckParam * percent;
            output.LengthGreater13Param = baseConfig.LengthGreater13Param * percent;
        }
    }
}