
namespace App.Shared.GameModules.Weapon.Behavior.FireCheck
{
    /// <summary>
    /// Defines the <see cref="CameraChecker" />
    /// </summary>
    public class CameraChecker : IFireChecker
    {
        public bool IsCanFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            return controller.RelatedCameraSNew.CanFire;
        }
    }
}
