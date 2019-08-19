using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    ///     Defines the <see cref="CommonFireAppearanceUpdater" />
    /// </summary>
    public class CommonFireAppearanceUpdater : IIdleAndAfterFireProcess
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonFireAppearanceUpdater));


        public virtual void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (attackProxy.CharacterState != null)
            {
                if (attackProxy.IsAiming)
                    attackProxy.CharacterState.SightsFire();
                else
                    attackProxy.CharacterState.Fire();
            }
        }

        public void OnIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (!cmd.IsFire)
            {
                attackProxy.AudioController.StopFireTrigger();
                DoIdle(attackProxy, cmd);
            }
        }

        protected virtual void DoIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
        }
    }
}