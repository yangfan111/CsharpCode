using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="ContinueFireCounter" />
    /// </summary>
    public class ContinueFireCounter : IFireProcessCounter
    {
       
        public void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var heldAgent = controller.HeldWeaponAgent;
            if (heldAgent.RifleFireCounterCfg == null) return;
            if (heldAgent.RunTimeComponent.ContinuesShootCount < 1)
                return;
            if (heldAgent.RunTimeComponent.ContinuesShootReduceTimestamp < controller.RelatedTime)
            {
                
      //          DebugUtil.MyLog("Count -- ||DecreaseStepInterval:"+heldAgent.RifleFireCounterCfg.DecreaseStepInterval+"||"+heldAgent.RifleFireCounterCfg.DecreaseStepInterval);
                heldAgent.RunTimeComponent.ContinuesShootReduceTimestamp = controller.RelatedTime + 
                                                                           heldAgent.RifleFireCounterCfg.DecreaseStepInterval;
                --heldAgent.RunTimeComponent.ContinuesShootCount;
            }

        }

        public void OnBeforeFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.HeldWeaponAgent.RifleFireCounterCfg == null)
                return;
            
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
         // runTimeComponent.NeedReduceContinuesShootCD = true;
            runTimeComponent.ContinuesShootCount = Mathf.Min(++runTimeComponent.ContinuesShootCount,
                controller.HeldWeaponAgent.RifleFireCounterCfg.MaxCount);
            runTimeComponent.ContinuesShootReduceTimestamp = controller.RelatedTime + controller.HeldWeaponAgent.RifleFireCounterCfg.DecreaseInitInterval;
        }

    
    }
}
