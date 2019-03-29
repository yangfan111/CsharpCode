using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="RifleSpreadProcessor" />
    /// </summary>
    public class RifleSpreadProcessor : ISpreadProcessor
    {
        public RifleSpreadProcessor()
        {
        }

        public void OnBeforeFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            RifleSpreadLogicConfig config = controller.HeldWeaponAgent.RifleSpreadLogicCfg;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            float spread = UpdateSpread(controller, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerWeaponController controller, float accuracy)
        {
            RifleSpreadLogicConfig config = controller.HeldWeaponAgent.RifleSpreadLogicCfg;
            float spread = 0;
            var spreadCfg = controller.RelatedCameraSNew.IsAiming() ? config.Aiming : config.Default;
            var posture = controller.RelatedStateInterface.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                spread = spreadCfg.Base * spreadCfg.Air;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > config.FastMoveSpeed)
            {
                spread = spreadCfg.Base * spreadCfg.FastMove;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone)
            {
                spread = spreadCfg.Base * spreadCfg.Prone;
            }
            else
            {
                spread = spreadCfg.Base;
            }
            return spread * accuracy;
        }
    }
}
