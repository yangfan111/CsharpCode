using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolSpreadLogic" />
    /// </summary>
    public class PistolSpreadLogic : ISpreadLogic
    {
        public PistolSpreadLogic()
        {
        }

        public void BeforeFireBullet(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            PistolSpreadLogicConfig config = controller.HeldWeaponAgent.PistolSpreadLogicCfg;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            float spread = UpdateSpread(controller, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerWeaponController controller, float accuracy)
        {
            PistolSpreadLogicConfig config = controller.HeldWeaponAgent.PistolSpreadLogicCfg;
            float param;
            var posture = controller.RelatedStateInterface.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                param = config.AirParam;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > 13)
            {
                param = config.LengthGreater13Param;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone || posture == XmlConfig.PostureInConfig.Crouch)
            {
                param = config.DuckParam;
            }
            else
            {
                param = config.DefaultParam;
            }
            return param * (1 - accuracy);
        }
    }
}
