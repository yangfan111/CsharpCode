namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="FixedSpreadProcessor" />
    /// </summary>
    public class FixedSpreadProcessor : AbstractSpreadProcessor
    {

        protected override void Update(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var config           = attackProxy.WeaponConfigAssy.S_FixedSpreadLogicCfg;
            FireSpreadFormula.ApplyFixedFinalSpread(config.Value, config.SpreadScale, attackProxy.RuntimeComponent);
        }
    }
}
