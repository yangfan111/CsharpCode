
namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CameraChecker" />
    /// </summary>
    public class CameraChecker : IFireChecker
    {
        public bool IsCanFire(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            return controller.RelatedCameraSNew.CanFire;
        }
    }
}
