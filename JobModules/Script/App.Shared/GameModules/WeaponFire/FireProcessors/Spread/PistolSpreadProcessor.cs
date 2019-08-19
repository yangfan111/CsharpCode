using App.Shared.Components.Serializer;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolSpreadProcessor" />
    /// </summary>
    public class PistolSpreadProcessor : AbstractSpreadProcessor
    {
        
        protected override void Update(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var config = attackProxy.WeaponConfigAssy.S_PistolSpreadLogicCfg;
            float spreadScaleFactor = FireSpreadProvider.GetSpreadScaleFactor(config, attackProxy);
            FireSpreadFormula.ApplyPistolFinalSpread(spreadScaleFactor,config.SpreadScale,attackProxy.RuntimeComponent);
        }

       
    }
}
