using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SniperSpreadProcessor" />
    /// </summary>
    public class SniperSpreadProcessor : AbstractSpreadProcessor
    {
   
        protected override void Update(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            SniperSpreadLogicConfig config           = attackProxy.WeaponConfigAssy.S_SniperSpreadLogicCfg;
            float                   spread           = FireSpreadProvider.GetSpreadScaleFactor(config, attackProxy);
            FireSpreadFormula.ApplyFixedFinalSpread(spread, config.SpreadScale, attackProxy.RuntimeComponent);
        }
    }
}