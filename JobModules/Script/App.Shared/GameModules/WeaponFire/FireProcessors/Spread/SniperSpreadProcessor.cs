using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SniperSpreadProcessor" />
    /// </summary>
    public class SniperSpreadProcessor : AbstractSpreadProcessor
    {
        protected override void Update(PlayerWeaponController controller, IWeaponCmd cmd)

        {
            SniperSpreadLogicConfig config           = controller.HeldWeaponAgent.SniperSpreadLogicCfg;
            var                     runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
            float                   spread           = FireSpreadProvider.GetSpreadScaleFactor(config, controller);
            FireSpreadFormula.ApplyFixedFinalSpread(spread, config.SpreadScale, runTimeComponent);
        }
    }
}