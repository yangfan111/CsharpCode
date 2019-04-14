using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// 最终改变WeaponRuntimeDataComponent.LastSpreadX/LastSpreadY
    /// </summary>
    public class RifleSpreadProcessor : AbstractSpreadProcessor
    {

        protected override void Update(PlayerWeaponController controller, IWeaponCmd cmd)

        {
            RifleSpreadLogicConfig config = controller.HeldWeaponAgent.RifleSpreadLogicCfg;
            var weaponRuntime = controller.HeldWeaponAgent.RunTimeComponent;
            float spreadScaleFactor = FireSpreadProvider.GetSpreadScaleFactor(config, controller);
            FireSpreadFormula.ApplyRifleFinalSpread(spreadScaleFactor, config.SpreadScale,weaponRuntime);
        }

    }
}
