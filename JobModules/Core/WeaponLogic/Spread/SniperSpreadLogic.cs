using WeaponConfigNs;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Spread;

namespace Core.WeaponLogic
{
    public class SniperSpreadLogic : AbstractSpreadLogic<SniperSpreadLogicConfig, float>
    {
        protected int Length2D1 = 350;
        protected int Length2D2 = 25;

        public SniperSpreadLogic(SniperSpreadLogicConfig config):base(config)
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
            else if (playerWeapon.HorizontalVeocity > Length2D1)
                param = _config.LengthParam1;
            else if (playerWeapon.HorizontalVeocity > Length2D2)
                param = _config.LengthParam2;
            else if (playerWeapon.IsDuckOrProneState)
                param = _config.DuckParam;
            else
                param = _config.DefaultParam;
            if(!playerWeapon.IsAiming)
            {
                param += _config.FovAddParam;
            }
            return param;
        }
        
        public override void Apply(SniperSpreadLogicConfig baseConfig, SniperSpreadLogicConfig output, float arg)
        {
        }
    }
}