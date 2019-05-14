using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// 最终改变WeaponRuntimeDataComponent.LastSpreadX/LastSpreadY
    /// </summary>
    public class RifleSpreadProcessor : AbstractSpreadProcessor
    {

        protected override void Update(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)

        {
            RifleSpreadLogicConfig config = weaponBaseAgent.RifleSpreadLogicCfg;
            var weaponRuntime = weaponBaseAgent.RunTimeComponent;
            float spreadScaleFactor = FireSpreadProvider.GetSpreadScaleFactor(config, weaponBaseAgent.Owner.WeaponController());
            FireSpreadFormula.ApplyRifleFinalSpread(spreadScaleFactor, config.SpreadScale,weaponRuntime);
        }

    }
}
