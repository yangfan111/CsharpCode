using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="DefaultFireActionLogic" />
    /// </summary>
    public class DefaultFireActionLogic : AbstractFireActionLogic<CommonFireConfig>
    {
        public override void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
        }

        protected override void OnAfterFire(PlayerWeaponController controller, bool needActionDeal)
        {
            DefaultFire(controller);
        }
    }
}
