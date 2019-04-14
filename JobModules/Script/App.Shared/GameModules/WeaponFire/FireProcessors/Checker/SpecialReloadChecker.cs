using Core.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SpecialReloadChecker" />
    /// </summary>
    public class SpecialReloadChecker : IFireChecker
    {
        public SpecialReloadChecker()
        {
        }

        public bool IsCanFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            return CheckSpecialReload(controller);
        }

        /// <summary>
        /// 判断特殊换弹逻辑 
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        /// <returns>是否可以射击</returns>
        private bool CheckSpecialReload(PlayerWeaponController controller)
        {

           // DebugUtil.MyLog("GetActionState:"+controller.RelatedCharState.GetActionState());
            if (controller.RelatedCharState.GetActionState() != ActionInConfig.Reload &&
                controller.RelatedCharState.GetActionState() != ActionInConfig.SpecialReload)
            {
                return true;
            }
            var config = controller.HeldWeaponAgent.CommonFireCfg;
            if (config == null)
                return false;
            var weaponRunTIme = controller.HeldWeaponAgent.RunTimeComponent;
            var weaponBase = controller.HeldWeaponAgent.BaseComponent;


            if (config.SpecialReloadCount > 0 && weaponBase.Bullet > 0)
            {
                //TODO 特殊换弹打断逻辑
                if (weaponBase.PullBolt)
                {
                    //如果已经上膛，直接打断并开枪
                    controller.RelatedCharState.ForceBreakSpecialReload(null);
                    return true;
                }
                else
                {
                    //如果没有上膛，执行上膛，结束后开枪
                    controller.RelatedCharState.BreakSpecialReload();
                    weaponBase.PullBolt = true;
                 
                    if (controller.AutoFire.HasValue )
                    {
                        controller.AutoFire =(int)EAutoFireState.ReloadBreak;
                    }
                }
            }
            return false;
        }
    }
}
