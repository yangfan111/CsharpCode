
namespace App.Shared.GameModules.Weapon.Behavior.FireCheck
{
    /// <summary>
    /// Defines the <see cref="CameraCheckLogic" />
    /// </summary>
    public class CameraCheckLogic : IFireCheck
    {
        public bool IsCanFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            return controller.RelatedCameraSNew.CanFire;
        }
    }
}
