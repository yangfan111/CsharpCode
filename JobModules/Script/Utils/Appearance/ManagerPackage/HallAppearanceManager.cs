using UnityEngine;
using Utils.Appearance.PropItem;
using Utils.Appearance.WardrobePackage;
using Utils.Appearance.Weapon;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.ManagerPackage
{
    public class HallAppearanceManager : AppearanceManagerBase
    {
        public HallAppearanceManager()
        {
            WeaponControllerBaseImpl = new NewHallWeaponController(); 
            WeaponDataBaseImpl = new HallWeaponController();
            WardrobeControllerBaseImpl = new HallWardrobeController(null);
            PropControllerBaseImpl = new HallPropController();
            ReplaceMaterialShaderBaseImpl = new HallReplaceMaterialShader();
        }
        
        public void TryRewind()
        {
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
            //WeaponControllerBaseImpl.ClearThirdPersonCharacter();
        }

        public void CommonResetWardrobe()
        {
            WardrobeControllerBaseImpl.CommonReset();
        }

        public void CommonReset()
        {
            WardrobeControllerBaseImpl.CommonReset();
            //WeaponControllerBaseImpl.CommonReset();
        }

        public override void MountWeaponInPackage(WeaponInPackage pos, int id)
        {
            if ((int)pos < (int)WeaponInPackage.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.SetWeaponIdByType(pos,id);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public override void UnmountWeaponInPackage(WeaponInPackage pos)
        {
            if ((int)pos < (int)WeaponInPackage.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.SetWeaponIdByType(pos, UniversalConsts.InvalidIntId);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public override void MountWeaponToHand(WeaponInPackage pos)
        {
            if ((int)pos < (int)WeaponInPackage.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.SetHandWeaponType(pos);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public override void UnmountWeaponFromHand()
        {
            WeaponControllerBaseImpl.SetHandWeaponType(WeaponInPackage.EmptyHanded);
        }

        public override void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if ((int)pos <= (int)WeaponInPackage.EndOfTheWorld &&
                (int)location <= (int)WeaponPartLocation.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.SetAttachmentIdByType(pos, location, id);
            }
        }

        public override void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location)
        {
            if ((int)pos <= (int)WeaponInPackage.EndOfTheWorld &&
                (int)location <= (int)WeaponPartLocation.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.SetAttachmentIdByType(pos, location, UniversalConsts.InvalidIntId);
            }
        }
    }
}
