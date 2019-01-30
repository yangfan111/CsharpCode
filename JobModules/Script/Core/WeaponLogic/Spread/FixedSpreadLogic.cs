using WeaponConfigNs;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Spread;
using System;

namespace Core.WeaponLogic
{

    public class FixedSpreadLogic : AbstractSpreadLogic<FixedSpreadLogicConfig, float>
    {
        public override void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            float spread = UpdateSpread(playerWeapon, playerWeapon.LastAccuracy);
            playerWeapon.LastSpreadX = spread * _config.SpreadScale.ScaleX;
            playerWeapon.LastSpreadY = spread * _config.SpreadScale.ScaleY;
        }

        public FixedSpreadLogic(FixedSpreadLogicConfig config): base(config)
        {
        }

        protected override float UpdateSpread(IPlayerWeaponState playerWeapon, float accuracy)
        {
            return _config.Value;
        }
        public override void Apply(FixedSpreadLogicConfig baseConfig, FixedSpreadLogicConfig output, float arg)
        {
            var _spreadPercent = arg > 0 ? arg : 1;
            output.Value = baseConfig.Value * _spreadPercent;
        }
    }
}