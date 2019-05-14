using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SniperSpreadProcessor" />
    /// </summary>
    public class SniperSpreadProcessor : AbstractSpreadProcessor
    {
        protected override void Update(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)

        {
            SniperSpreadLogicConfig config           = weaponBaseAgent.SniperSpreadLogicCfg;
            var                     runTimeComponent = weaponBaseAgent.RunTimeComponent;
            float                   spread           = FireSpreadProvider.GetSpreadScaleFactor(config, weaponBaseAgent.Owner.WeaponController());
            FireSpreadFormula.ApplyFixedFinalSpread(spread, config.SpreadScale, runTimeComponent);
        }
    }
}