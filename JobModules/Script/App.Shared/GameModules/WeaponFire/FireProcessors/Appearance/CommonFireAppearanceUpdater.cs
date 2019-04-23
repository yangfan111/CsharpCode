using Core.Utils;
using Entitas.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CommonFireAppearanceUpdater" />
    /// </summary>
    public class CommonFireAppearanceUpdater : IIdleAndAfterFireProcess
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonFireAppearanceUpdater));
        public virtual void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
           
        }

        public virtual void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var relatedCharState = controller.RelatedCharState;
            if (relatedCharState != null)
            {
                if (controller.RelatedCameraSNew.IsAiming())
                    relatedCharState.SightsFire();
                else
                    relatedCharState.Fire();
            }
       
        }
       
    }
}
