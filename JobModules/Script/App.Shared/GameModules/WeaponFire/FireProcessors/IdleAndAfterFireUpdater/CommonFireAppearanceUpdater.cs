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

        protected virtual void DoIdle(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
          
        }

      
        public virtual void OnAfterFire(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            var weaponController = agent.Owner.WeaponController();
            var relatedCharState = weaponController.RelatedCharState;
            if (relatedCharState != null)
            {
                if (weaponController.RelatedCameraSNew.IsAiming())
                    relatedCharState.SightsFire();
                else
                    relatedCharState.Fire();
            }
          
        }

        public void OnIdle(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            if (!cmd.IsFire)
            {
                var audioController = agent.Owner.AudioController();
                if (audioController != null)
                    audioController.StopFireTrigger();
                DoIdle(agent.Owner.WeaponController(),cmd);
            }
        }
    }
}