using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SniperSpreadLogic" />
    /// </summary>
    public class SniperSpreadLogic : ISpreadLogic
    {
        protected int Length2D1 = 350;

        protected int Length2D2 = 25;

        public SniperSpreadLogic(Contexts contexts)
        {
        }

        public void BeforeFireBullet(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            SniperSpreadLogicConfig config = controller.HeldWeaponAgent.SniperSpreadLogicCfg;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            float spread = UpdateSpread(controller, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerWeaponController controller, float accuracy)
        {
            SniperSpreadLogicConfig config = controller.HeldWeaponAgent.SniperSpreadLogicCfg;
            float param;
            var posture = controller.RelatedStateInterface.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                param = config.AirParam;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > Length2D1)
            {
                param = config.LengthParam1;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > Length2D2)
            {
                param = config.LengthParam2;
            }
            else if (posture == XmlConfig.PostureInConfig.Crouch || posture == XmlConfig.PostureInConfig.Prone)
            {
                param = config.DuckParam;
            }
            else
            {
                param = config.DefaultParam;
            }
            if (!controller.RelatedCameraSNew.IsAiming())
            {
                param += config.FovAddParam;
            }
            return param;
        }
    }
}
