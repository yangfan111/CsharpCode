namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="BaseAccuracyLogic" />
    /// </summary>
    public class BaseAccuracyLogic : IAccuracyLogic
    {
        public BaseAccuracyLogic()
        {
        }

        public void BeforeFireBullet(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
            var config = weaponController.HeldWeaponAgent.BaseAccuracyLogicCfg;
            if (config == null)
                return;
            Components.Weapon.WeaponRuntimeDataComponent weaponState = weaponController.HeldWeaponAgent.RunTimeComponent;
            int accuracyDivisor = config.AccuracyDivisor;
            if (accuracyDivisor != -1)
            {
                int shotsFired = weaponState.ContinuesShootCount;
                float maxInaccuracy = config.MaxInaccuracy;
                float accuracyOffset = config.AccuracyOffset;
                float accuracy = shotsFired * shotsFired * shotsFired / accuracyDivisor + accuracyOffset;
                if (accuracy > maxInaccuracy)
                    accuracy = maxInaccuracy;
                weaponState.Accuracy = accuracy;
            }
            else
            {
                weaponState.Accuracy = 0;
            }
        }

        public void OnIdle(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
        }
    }
}
