using System;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CommonAccuracyCalculator" />
    /// </summary>
    public class CommonAccuracyCalculator : IAccuracyCalculator
    {

        public void OnBeforeFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            UpdateAccurcy(attackProxy);
        }
 
        public void OnIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            //静止状态accurcy会影响到准星扩散
            UpdateAccurcy(attackProxy);

        }
        private void UpdateAccurcy(WeaponAttackProxy attackProxy)
        {
            var config = attackProxy.WeaponConfigAssy.S_BaseAccuracyLogicCfg;
            if (config == null)
                return;
                
            int accuracyDivisor = config.AccuracyDivisor; //除数因子
            if (accuracyDivisor != -1)
            {
                attackProxy.RuntimeComponent.Accuracy = AccuracyFormula.GetCommonAccuracy(config.MaxInaccuracy, attackProxy.RuntimeComponent.ContinuesShootCount,
                    config.AccuracyDivisor, config.AccuracyOffset);
            }
            else
            {
                attackProxy.RuntimeComponent.Accuracy = 0;
            }
        }

     
    }
}
