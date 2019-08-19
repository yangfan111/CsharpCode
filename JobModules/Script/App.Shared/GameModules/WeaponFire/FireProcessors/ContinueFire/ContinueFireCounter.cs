using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    ///     Defines the <see cref="ContinueFireCounter" />
    /// </summary>
    public class ContinueFireCounter : IFireProcessCounter
    {
        public void OnBeforeFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (attackProxy.WeaponConfigAssy.S_RifleFireCounterCfg == null)
                return;
            var runTimeComponent = attackProxy.RuntimeComponent;
            // runTimeComponent.NeedReduceContinuesShootCD = true;
            runTimeComponent.ContinuesShootCount = Mathf.Min(++runTimeComponent.ContinuesShootCount,
                attackProxy.WeaponConfigAssy.S_RifleFireCounterCfg.MaxCount);
            runTimeComponent.ContinuesShootReduceTimestamp =
                            cmd.UserCmd.RenderTime + attackProxy.WeaponConfigAssy.S_RifleFireCounterCfg.DecreaseInitInterval;
        }

        public void OnIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (attackProxy.WeaponConfigAssy.S_RifleFireCounterCfg == null) return;

            if (attackProxy.RuntimeComponent.ContinuesShootCount < 1)
                return;
            if (attackProxy.RuntimeComponent.ContinuesShootReduceTimestamp < cmd.UserCmd.RenderTime)
            {
                //          DebugUtil.MyLog("Count -- ||DecreaseStepInterval:"+heldAgent.RifleFireCounterCfg.DecreaseStepInterval+"||"+heldAgent.RifleFireCounterCfg.DecreaseStepInterval);
                attackProxy.RuntimeComponent.ContinuesShootReduceTimestamp = cmd.UserCmd.RenderTime +
                                attackProxy.WeaponConfigAssy.S_RifleFireCounterCfg.DecreaseStepInterval;
                --attackProxy.RuntimeComponent.ContinuesShootCount;
            }
        }
    }
}