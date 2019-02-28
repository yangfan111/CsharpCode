using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="AutoFireLogic" />
    /// </summary>
    public class AutoFireLogic : IAfterFire
    {
        public AutoFireLogic()
        {
        }

        public void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {

            var heldAgent = controller.HeldWeaponAgent;
            if (heldAgent.BaseComponent.RealFireModel != (int)EFireMode.Burst)
            {
                heldAgent.RunTimeComponent.BurstShootCount = 0;
                EnableAutoFire(controller, false);
                return;
            }
            var config = heldAgent.DefaultFireModeLogicCfg;
            if (config == null)
                return;
            heldAgent.RunTimeComponent.BurstShootCount += 1;
            if (heldAgent.RunTimeComponent.BurstShootCount < config.BurstCount)
            {
                heldAgent.RunTimeComponent.NextAttackTimer = (cmd.RenderTime + config.BurstAttackInnerInterval);
                EnableAutoFire(controller, true);
            }
            else
            {
                heldAgent.RunTimeComponent.NextAttackTimer = (cmd.RenderTime + config.BurstAttackInterval);
                heldAgent.RunTimeComponent.BurstShootCount = 0;
                EnableAutoFire(controller, false);
            }
            if (IsTheLastBullet(controller))
            {
                controller.HeldWeaponAgent.RunTimeComponent.BurstShootCount = 0;
                EnableAutoFire(controller, false);
            }
        }

        private bool IsTheLastBullet(PlayerWeaponController controller)
        {
            return controller.HeldWeaponAgent.BaseComponent.Bullet == 0;
        }

        private void EnableAutoFire(PlayerWeaponController controller, bool autoFire)
        {
            if (!controller.AutoFire.HasValue )
            {
                return;
            }
            if (autoFire)
            {
                controller.AutoFire |= (int)EAutoFireState.Burst;
            }
            else
            {
                controller.AutoFire &= ~(int)EAutoFireState.Burst;
          
            }
        }
    }
}
