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

        public void OnAfterFire(WeaponBaseAgent heldBaseAgent, WeaponSideCmd cmd)
        {
            if(heldBaseAgent.BaseComponent.RealFireModel != (int)EFireMode.Burst)
                return;

            var runTimeComponent = heldBaseAgent.RunTimeComponent;
            runTimeComponent.NeedAutoBurstShoot = false;
            var config = heldBaseAgent.DefaultFireModeLogicCfg;
            runTimeComponent.BurstShootCount += 1;
            float intervalFactor = 1 - heldBaseAgent.GetAttachedAttributeByType(WeaponAttributeType.AttackInterval) / 100;
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
            if (heldBaseAgent.BaseComponent.Bullet == 0)
                runTimeComponent.BurstShootCount = 0;
        }

    }
}
