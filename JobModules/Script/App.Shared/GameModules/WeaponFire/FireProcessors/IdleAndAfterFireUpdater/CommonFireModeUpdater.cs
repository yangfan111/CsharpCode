using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class CommonFireModeUpdater : IAfterFireProcess, IFrameProcess
    {

        public void OnFrame(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (!cmd.IsFire)
            {
                attackProxy.RuntimeComponent.IsPrevCmdFire = false;
                attackProxy.AudioController.StopFireTrigger();
            }
        }

        public void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if ((EFireMode) attackProxy.BasicComponent.RealFireModel == EFireMode.Burst)
                return;
            var runtimeDataComponent = attackProxy.RuntimeComponent;
            runtimeDataComponent.IsPrevCmdFire = true;
            float intervalFactor = 1 - attackProxy.GetAttachedAttributeByType(WeaponAttributeType.AttackInterval) / 100;
            runtimeDataComponent.NextAttackTimestamp = cmd.UserCmd.RenderTime + Mathf.CeilToInt(attackProxy.WeaponConfigAssy.S_CommonFireCfg.AttackInterval * intervalFactor);
            runtimeDataComponent.NeedAutoBurstShoot       = false;
          //  runtimeDataComponent.ContinuesShootCount = 0;
            runtimeDataComponent.LastAttackTimestamp = cmd.UserCmd.RenderTime;
        }

        
    }
}