using App.Shared.Components.Serializer;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolSpreadProcessor" />
    /// </summary>
    public class PistolSpreadProcessor : AbstractSpreadProcessor
    {
        
        protected override void Update(WeaponBaseAgent heldBaseAgent, WeaponSideCmd cmd)
        {
            var config = heldBaseAgent.PistolSpreadLogicCfg;
            var weaponRuntime = heldBaseAgent.RunTimeComponent;
            float spreadScaleFactor = FireSpreadProvider.GetSpreadScaleFactor(config, heldBaseAgent.Owner.WeaponController());
            FireSpreadFormula.ApplyPistolFinalSpread(spreadScaleFactor,config.SpreadScale,weaponRuntime);
        }

       
    }
}
