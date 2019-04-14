namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedSpreadProcessor" />
    /// </summary>
    public class FixedSpreadProcessor : AbstractSpreadProcessor
    {

        protected override void Update(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config           = controller.HeldWeaponAgent.FixedSpreadLogicCfg;
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
            FireSpreadFormula.ApplyFixedFinalSpread(config.Value, config.SpreadScale, runTimeComponent);
        }
    }
}
