using WeaponConfigNs;
using Core.WeaponLogic.Spread;
using Core.WeaponLogic.Attachment;

namespace Core.WeaponLogic
{
    public class RifleSpreadLogic : AbstractSpreadLogic<RifleSpreadLogicConfig,  float>
    {

        public RifleSpreadLogic(RifleSpreadLogicConfig config):base(config)
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
            float spread = 0;
            var spreadCfg = playerWeapon.IsAiming ? _config.Aiming : _config.Default; 
            if (playerWeapon.IsAir)
                spread = spreadCfg.Base * spreadCfg.Air;
            else if (playerWeapon.HorizontalVeocity > _config.FastMoveSpeed)
                spread = spreadCfg.Base * spreadCfg.FastMove;
            else if (playerWeapon.IsProne)
                spread = spreadCfg.Base * spreadCfg.Prone;
            else
                spread = spreadCfg.Base;
            return spread * accuracy;
        }
        
        public override void Apply(RifleSpreadLogicConfig baseConfig, RifleSpreadLogicConfig output, float arg)
        {
        }

    }
}