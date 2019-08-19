using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    ///     Defines the <see cref="SpecialReloadChecker" />
    /// </summary>
    public class SpecialReloadChecker : IFireChecker
    {
        public bool IsCanFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            return CheckSpecialReload(attackProxy);
        }

        /// <summary>
        ///     判断特殊换弹逻辑
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        /// <returns>是否可以射击</returns>
        private bool CheckSpecialReload(WeaponAttackProxy attackProxy)
        {
            var specialReload = attackProxy.CharacterState.GetActionState();
            // DebugUtil.MyLog("GetActionState:"+controller.RelatedCharState.GetActionState());
            if (specialReload != ActionInConfig.Reload && specialReload != ActionInConfig.SpecialReload)
            {
                return true;
            }

            var config = attackProxy.WeaponConfigAssy.S_CommonFireCfg;
            if (config == null)
                return false;
            if (config.SpecialReloadCount > 0 && attackProxy.BasicComponent.Bullet > 0)
            {
                if (attackProxy.BasicComponent.PullBolt)
                {
                    //如果已经上膛，直接打断并开枪
                    attackProxy.CharacterState.ForceBreakSpecialReload(null);
                    return true;
                }

                //如果没有上膛，执行上膛，结束后开枪
                attackProxy.CharacterState.BreakSpecialReload();
                attackProxy.BasicComponent.PullBolt             = true;
                attackProxy.RuntimeComponent.NeedAutoBurstShoot = false;
            }
            else
            {
                attackProxy.AudioController.PlayEmptyFireAudio();
            }

            return false;
        }
    }
}