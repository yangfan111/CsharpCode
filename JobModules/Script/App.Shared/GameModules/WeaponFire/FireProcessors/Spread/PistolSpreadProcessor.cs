using App.Shared.Components.Serializer;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolSpreadProcessor" />
    /// </summary>
    public class PistolSpreadProcessor : AbstractSpreadProcessor
    {
        
        protected override void Update(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.PistolSpreadLogicCfg;
            var weaponRuntime = controller.HeldWeaponAgent.RunTimeComponent;
            float spreadScaleFactor = FireSpreadProvider.GetSpreadScaleFactor(config, controller);
            FireSpreadFormula.ApplyPistolFinalSpread(spreadScaleFactor,config.SpreadScale,weaponRuntime);
        }

       
    }
}
