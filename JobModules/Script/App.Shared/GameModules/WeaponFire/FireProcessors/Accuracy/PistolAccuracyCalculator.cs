using CompareUtility = Utils.Compare.CompareUtility;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolAccuracyCalculator" />
    /// </summary>
    public class PistolAccuracyCalculator : IAccuracyCalculator
    {

        public void OnBeforeFire(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            var runTimeComponent = weaponBaseAgent.RunTimeComponent;
            if (runTimeComponent.LastAttackTimestamp == 0)
            {
            }
            else
            {
                var config = weaponBaseAgent.PistolAccuracyLogicCfg;
                if (config == null)
                    return;
                runTimeComponent.Accuracy = AccuracyFormula.GetPistolAccuracy(cmd.UserCmd.RenderTime - runTimeComponent.LastAttackTimestamp,
                    config.AccuracyFactor, config.MinAccuracy, config.MaxAccuracy);
            }
        }

        public void OnIdle(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            var runTimeComponent = weaponBaseAgent.RunTimeComponent;
            if (runTimeComponent.ContinuesShootCount == 0)
            {

                var config = weaponBaseAgent.PistolAccuracyLogicCfg;
                if (config == null)
                    return;

                runTimeComponent.Accuracy = config.InitAccuracy;
            }
        }
    }
}
