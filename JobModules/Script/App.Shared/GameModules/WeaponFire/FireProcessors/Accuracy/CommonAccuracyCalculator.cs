using System;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CommonAccuracyCalculator" />
    /// </summary>
    public class CommonAccuracyCalculator : IAccuracyCalculator
    {

        public void OnBeforeFire(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            UpdateAccurcy(weaponBaseAgent);
        }

 
        public void OnIdle(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            //静止状态accurcy会影响到准星扩散
            UpdateAccurcy(weaponBaseAgent);

        }
        private void UpdateAccurcy(WeaponBaseAgent weaponBaseAgent)
        {
            var config = weaponBaseAgent.BaseAccuracyLogicCfg;
            if (config == null)
                return;
            var runTimeComponent = weaponBaseAgent.RunTimeComponent;
                
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
