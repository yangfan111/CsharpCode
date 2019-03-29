namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedSpreadProcessor" />
    /// </summary>
    public class FixedSpreadProcessor : ISpreadProcessor
    {
        public FixedSpreadProcessor()
        {
        }

        public void OnBeforeFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.FixedSpreadLogicCfg;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            float spread = UpdateSpread(controller, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerWeaponController controller, float accuracy)
        {
            return controller.HeldWeaponAgent.FixedSpreadLogicCfg.Value;
        }
    }
}
