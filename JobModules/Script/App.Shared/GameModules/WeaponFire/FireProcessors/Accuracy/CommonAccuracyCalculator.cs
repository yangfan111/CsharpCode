using System;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CommonAccuracyCalculator" />
    /// </summary>
    public class CommonAccuracyCalculator : IAccuracyCalculator
    {

        public void OnBeforeFire(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
            UpdateAccurcy(weaponController);
        }

 
        public void OnIdle(PlayerWeaponController weaponController, IWeaponCmd cmd)
        {
            //静止状态accurcy会影响到准星扩散
            UpdateAccurcy(weaponController);

        }
        private void UpdateAccurcy(PlayerWeaponController weaponController)
        {
            var config = weaponController.HeldWeaponAgent.BaseAccuracyLogicCfg;
            if (config == null)
                return;
            var runTimeComponent = weaponController.HeldWeaponAgent.RunTimeComponent;
                
            int accuracyDivisor = config.AccuracyDivisor; //除数因子
            if (accuracyDivisor != -1)
            {
                runTimeComponent.Accuracy = AccuracyFormula.GetCommonAccuracy(config.MaxInaccuracy, runTimeComponent.ContinuesShootCount,
                    config.AccuracyDivisor, config.AccuracyOffset);
            }
            else
            {
                runTimeComponent.Accuracy = 0;
            }
        }
    }
}
