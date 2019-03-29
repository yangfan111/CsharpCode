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
            var config = weaponController.HeldWeaponAgent.BaseAccuracyLogicCfg;
            if (config == null)
                return;
            Components.Weapon.WeaponRuntimeDataComponent weaponState = weaponController.HeldWeaponAgent.RunTimeComponent;
                
            int accuracyDivisor = config.AccuracyDivisor;//除数因子
            if (accuracyDivisor != -1)
            {
                int shootCount = weaponState.ContinuesShootCount;
                float maxInaccuracy = config.MaxInaccuracy;//最高精确度
                float accuracyOffset = config.AccuracyOffset;
                float accuracy = shootCount * shootCount * shootCount / accuracyDivisor + accuracyOffset;
                weaponState.Accuracy = Math.Min(accuracy,maxInaccuracy) ;
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
