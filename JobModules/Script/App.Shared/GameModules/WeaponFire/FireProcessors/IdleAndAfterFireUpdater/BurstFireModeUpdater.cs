using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="BurstFireModeUpdater" />
    /// </summary>
    public class BurstFireModeUpdater : IAfterFireProcess
    {

        public void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if(attackProxy.BasicComponent.RealFireModel != (int)EFireMode.Burst)
                return;

            var runTimeComponent = attackProxy.RuntimeComponent;
            runTimeComponent.NeedAutoBurstShoot = false;
            var config = attackProxy.WeaponConfigAssy.S_DefaultFireModeLogicCfg;
            runTimeComponent.BurstShootCount += 1;
            float intervalFactor = 1 - attackProxy.GetAttachedAttributeByType(WeaponAttributeType.AttackInterval) / 100;
            if (runTimeComponent.BurstShootCount < config.BurstCount)
            {
                runTimeComponent.NextAttackTimestamp = cmd.UserCmd.RenderTime + Mathf.CeilToInt(config.BurstAttackInnerInterval * intervalFactor);
                runTimeComponent.NeedAutoBurstShoot = true;
                runTimeComponent.IsPrevCmdFire = true;
            }
            else
            {
                runTimeComponent.NextAttackTimestamp = cmd.UserCmd.RenderTime + Mathf.CeilToInt(config.BurstAttackInterval * intervalFactor);
                runTimeComponent.BurstShootCount = 0;
            }

            if (attackProxy.BasicComponent.Bullet == 0)
            {
                runTimeComponent.BurstShootCount = 0;
                runTimeComponent.NeedAutoBurstShoot = false;
            }
        }

    }
}
