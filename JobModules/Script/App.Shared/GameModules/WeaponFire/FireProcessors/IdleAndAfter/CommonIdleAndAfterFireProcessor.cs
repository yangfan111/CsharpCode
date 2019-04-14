using Core.Utils;
using Entitas.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CommonIdleAndAfterFireProcessor" />
    /// </summary>
    public class CommonIdleAndAfterFireProcessor : IIdleAndAfterFireProcess
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonIdleAndAfterFireProcessor));
        public virtual void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
        }

        public virtual void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
           
            if (controller.RelatedCameraSNew != null && controller.RelatedCameraSNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                if (controller.RelatedCharState != null)
                {
                    controller.RelatedCharState.SightsFire();
                }
                else
                {
                    Logger.Error("player has no stateInterface");
                }
            }
            else
            {
                if (controller.RelatedCharState != null)
                {
                    controller.RelatedCharState.Fire();
                }
                else
                {
                    Logger.Error("player has no stateInterface");
                }
            }
        }
       
    }
}
