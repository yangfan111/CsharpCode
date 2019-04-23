using CompareUtility = Utils.Compare.CompareUtility;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolAccuracyCalculator" />
    /// </summary>
    public class PistolAccuracyCalculator : IAccuracyCalculator
    {

        public void OnBeforeFire(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
            var runTimeComponent = weaponController.HeldWeaponAgent.RunTimeComponent;
            if (runTimeComponent.LastAttackTimestamp == 0)
            {
            }
            else
            {
                var config = weaponController.HeldWeaponAgent.PistolAccuracyLogicCfg;
                if (config == null)
                    return;
                runTimeComponent.Accuracy = AccuracyFormula.GetPistolAccuracy(cmd.RenderTime - runTimeComponent.LastAttackTimestamp,
                    config.AccuracyFactor, config.MinAccuracy, config.MaxAccuracy);
            }
        }

        public void OnIdle(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
            var runTimeComponent = weaponController.HeldWeaponAgent.RunTimeComponent;
            if (runTimeComponent.ContinuesShootCount == 0)
            {

                var config = weaponController.HeldWeaponAgent.PistolAccuracyLogicCfg;
                if (config == null)
                    return;

                runTimeComponent.Accuracy = config.InitAccuracy;
            }
        }
    }
}
