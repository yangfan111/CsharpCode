namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="PistolAccuracyLogic" />
    /// </summary>
    public class PistolAccuracyLogic : IAccuracyLogic
    {
        public PistolAccuracyLogic()
        {
        }

        public void BeforeFireBullet(PlayerWeaponController weaponController, IWeaponCmd cmd)
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
                if (accuracy > config.MaxAccuracy)
                    accuracy = config.MaxAccuracy;
                else if (accuracy < config.MinAccuracy)
                    accuracy = config.MinAccuracy;
                weaponState.Accuracy = accuracy;
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
