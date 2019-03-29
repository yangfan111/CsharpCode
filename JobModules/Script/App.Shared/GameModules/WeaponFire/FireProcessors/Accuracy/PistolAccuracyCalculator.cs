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
            var weaponState = weaponController.HeldWeaponAgent.RunTimeComponent;
            if (weaponState.LastFireTime == 0)
            {
            }
            else
            {
                var config = weaponController.HeldWeaponAgent.PistolAccuracyLogicCfg;
                if (config == null)
                    return;
                var accuracy = weaponState.Accuracy;
                accuracy -= config.AccuracyFactor * (0.3f - (cmd.RenderTime - weaponState.LastFireTime) / 1000.0f);
                weaponState.Accuracy = CompareUtility.LimitBetween(accuracy,config.MinAccuracy,config.MaxAccuracy);
            }
        }

        public void OnIdle(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
            var weaponState = weaponController.HeldWeaponAgent.RunTimeComponent;
            if (weaponState.ContinuesShootCount == 0)
            {

                var config = weaponController.HeldWeaponAgent.PistolAccuracyLogicCfg;
                if (config == null)
                    return;

                weaponState.Accuracy = config.InitAccuracy;
            }
        }
    }
}
