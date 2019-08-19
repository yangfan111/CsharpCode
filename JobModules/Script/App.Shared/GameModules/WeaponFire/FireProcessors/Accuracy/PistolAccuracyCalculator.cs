using CompareUtility = Utils.Compare.CompareUtility;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolAccuracyCalculator" />
    /// </summary>
    public class PistolAccuracyCalculator : IAccuracyCalculator
    {
      
        public void OnBeforeFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var runTimeComponent = attackProxy.RuntimeComponent;
            if (runTimeComponent.LastAttackTimestamp == 0)
            {
            }
            else
            {
                var config = attackProxy.WeaponConfigAssy.S_PistolAccuracyLogicCfg;
                if (config == null)
                    return;
                    runTimeComponent.Accuracy = AccuracyFormula.GetPistolAccuracy(cmd.UserCmd.RenderTime - runTimeComponent.LastAttackTimestamp,
                    config.AccuracyFactor, config.MinAccuracy, config.MaxAccuracy);
            }
        }

        public void OnIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (attackProxy.RuntimeComponent.ContinuesShootCount == 0)
            {

                var config = attackProxy.WeaponConfigAssy.S_PistolAccuracyLogicCfg;
                if(config != null)
                    attackProxy.RuntimeComponent.Accuracy = config.InitAccuracy;
            }
        }

      
    }
}
