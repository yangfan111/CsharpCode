using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class CommonFireModeUpdater : IAfterFireProcess, IFrameProcess
    {

        public void OnFrame(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            if (!cmd.IsFire)
            {
                weaponBaseAgent.RunTimeComponent.IsPrevCmdFire = false;
                var audioController = weaponBaseAgent.Owner.AudioController();
                if (audioController != null)
                    audioController.StopFireTrigger();
            }
        }

        public void OnAfterFire(WeaponBaseAgent weaponBaseAgent, WeaponSideCmd cmd)
        {
            if ((EFireMode) weaponBaseAgent.BaseComponent.RealFireModel == EFireMode.Burst)
                return;
            var runtimeDataComponent = weaponBaseAgent.RunTimeComponent;
            runtimeDataComponent.IsPrevCmdFire = true;
            float intervalFactor = 1 - weaponBaseAgent.GetAttachedAttributeByType(WeaponAttributeType.AttackInterval) / 100;
            runtimeDataComponent.NextAttackTimestamp = cmd.UserCmd.RenderTime + Mathf.CeilToInt(weaponBaseAgent.CommonFireCfg.AttackInterval * intervalFactor);
            runtimeDataComponent.NeedAutoBurstShoot       = false;
            runtimeDataComponent.ContinuesShootCount = 0;
            runtimeDataComponent.LastAttackTimestamp = cmd.UserCmd.RenderTime;
        }

    }
}