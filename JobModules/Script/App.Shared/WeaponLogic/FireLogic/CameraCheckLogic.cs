using App.Shared.GameModules.Camera.Utils;
using Core.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;

namespace App.Shared.WeaponLogic.FireCheck
{
    public class CameraCheckLogic : IFireCheck
    {
        public bool IsCanFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            return playerEntity.IsCameraCanFire();
        }
    }
}