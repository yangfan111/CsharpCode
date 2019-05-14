using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="ContinueFireCounter" />
    /// </summary>
    public class ContinueFireCounter : IFireProcessCounter
    {
       
        public void OnIdle(WeaponBaseAgent heldAgent, WeaponSideCmd cmd)
        {
            if (heldAgent.RifleFireCounterCfg == null) return;
            if (heldAgent.RunTimeComponent.ContinuesShootCount < 1)
                return;
            if (heldAgent.RunTimeComponent.ContinuesShootReduceTimestamp < cmd.UserCmd.RenderTime )
            {
                
      //          DebugUtil.MyLog("Count -- ||DecreaseStepInterval:"+heldAgent.RifleFireCounterCfg.DecreaseStepInterval+"||"+heldAgent.RifleFireCounterCfg.DecreaseStepInterval);
                heldAgent.RunTimeComponent.ContinuesShootReduceTimestamp = cmd.UserCmd.RenderTime + 
                                                                           heldAgent.RifleFireCounterCfg.DecreaseStepInterval;
                --heldAgent.RunTimeComponent.ContinuesShootCount;
            }

        }

        public void OnBeforeFire(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            if (weaponBaseAgent.RifleFireCounterCfg == null)
                return;
            
            var runTimeComponent = weaponBaseAgent.RunTimeComponent;
         // runTimeComponent.NeedReduceContinuesShootCD = true;
            runTimeComponent.ContinuesShootCount = Mathf.Min(++runTimeComponent.ContinuesShootCount,
                weaponBaseAgent.RifleFireCounterCfg.MaxCount);
            runTimeComponent.ContinuesShootReduceTimestamp = cmd.UserCmd.RenderTime + weaponBaseAgent.RifleFireCounterCfg.DecreaseInitInterval;
        }

    
    }
}
