namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedSpreadProcessor" />
    /// </summary>
    public class FixedSpreadProcessor : AbstractSpreadProcessor
    {

        protected override void Update(WeaponBaseAgent heldBaseAgent, WeaponSideCmd cmd)
        {
            var config           = heldBaseAgent.FixedSpreadLogicCfg;
            var runTimeComponent = heldBaseAgent.RunTimeComponent;
            FireSpreadFormula.ApplyFixedFinalSpread(config.Value, config.SpreadScale, runTimeComponent);
        }
    }
}
