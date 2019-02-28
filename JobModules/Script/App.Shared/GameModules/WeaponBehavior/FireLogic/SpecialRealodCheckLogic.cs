using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SpecialReloadCheckLogic" />
    /// </summary>
    public class SpecialReloadCheckLogic : IFireCheck
    {
        public SpecialReloadCheckLogic()
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


            if (controller.RelatedStateInterface.GetActionState() != ActionInConfig.Reload &&
                controller.RelatedStateInterface.GetActionState() != ActionInConfig.SpecialReload)
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
                    controller.RelatedStateInterface.ForceBreakSpecialReload(null);
                    return true;
                }
                else
                {
                    //如果没有上膛，执行上膛，结束后开枪
                    controller.RelatedStateInterface.BreakSpecialReload();
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
