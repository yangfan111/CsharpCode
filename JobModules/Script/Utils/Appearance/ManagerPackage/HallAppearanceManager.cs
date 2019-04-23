using UnityEngine;
using Utils.Appearance.PropItem;
using Utils.Appearance.WardrobePackage;
using Utils.Appearance.Weapon;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance.ManagerPackage
{
    public class HallAppearanceManager : AppearanceManagerBase
    {
        public HallAppearanceManager()
        {
            WeaponControllerBaseImpl = new HallWeaponController();
            WardrobeControllerBaseImpl = new HallWardrobeController(null);
            PropControllerBaseImpl = new HallPropController();
            ReplaceMaterialShaderBaseImpl = new HallReplaceMaterialShader();

        }
        public void TryRewind()
        {
            WeaponControllerBaseImpl.TryRewind();
            WardrobeControllerBaseImpl.TryRewind();
            PropControllerBaseImpl.TryRewind();
        }

        public void CreateCharacter(int roleId)
        {
            var sex = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId).Sex;
            SetRoleModelIdAndInitAvatar(roleId, null);
        }

        public void SetThirdCharacter(GameObject character)
        {
            ClearThirdPersonCharacter();
            SetAnimatorP3(character.GetComponent<Animator>());
            SetThirdPersonCharacter(character);
        }

        public void ClearThirdPersonCharacter()
        {
            WardrobeControllerBaseImpl.ClearThirdPersonCharacter();
            WeaponControllerBaseImpl.ClearThirdPersonCharacter();
        }


        public void CommonReset()
        {
            WardrobeControllerBaseImpl.CommonReset();
            WeaponControllerBaseImpl.CommonReset();
        }
    }
}
