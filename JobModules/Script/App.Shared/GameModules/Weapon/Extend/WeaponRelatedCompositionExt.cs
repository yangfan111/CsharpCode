using App.Shared.Components.Player;
using App.Shared.Player;
using Assets.Utils.Configuration;
using Core.Appearance;
using Core.CharacterState;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// 定义与Weapon相关连的Player weapon components原子操作
    /// </summary>
    public static class WeaponRelatedExt
    {
        public static bool IsAiming(this CameraStateNewComponent component)
        {
            return component.ViewNowMode == (short)ECameraViewMode.GunSight;
        }
        public static void Reset(this OrientationComponent component)
        {

            // 更新枪械时，后坐力重置
            component.PunchPitch = 0;
            component.PunchYaw = 0;
            component.WeaponPunchPitch = 0;
            component.WeaponPunchYaw = 0;
        }
    
        /// <summary>
        /// ChangeC4ToBag
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="weaponId"></param>
        public static void UnmoutC4(this ICharacterAppearance Appearance, Utils.CharacterState.Sex sex)
        {
            Appearance.UnmountWeaponInPackage(WeaponInPackage.TacticWeapon);
            var c4ResId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(RoleAvatarConfigManager.C4, sex);
            Appearance.ChangeAvatar(c4ResId);
         
        }

        /// <summary>
        /// ChangeBagToC4
        /// </summary>
        /// <param name="Appearance"></param>
        /// <param name="weaponId"></param>
        public static void MountC4(this ICharacterAppearance Appearance, int weaponId)
        {
            var c4AvatarId = SingletonManager.Get<WeaponResourceConfigManager>().GetAvatarByWeaponId(weaponId);
            Appearance.MountWeaponInPackage(WeaponInPackage.TacticWeapon, c4AvatarId);
            Appearance.ClearAvatar(Wardrobe.Bag);
        }

        public static void RemoveC4(this ICharacterAppearance Appearance)
        {
            Appearance.UnmountWeaponInPackage(WeaponInPackage.TacticWeapon);
            Appearance.ClearAvatar(Wardrobe.Bag);
        }
     
        public static void CharacterUnmount(this ICharacterState State, System.Action unarm, float unarmParam)
        {
            State.InterruptAction();
            State.ForceFinishGrenadeThrow();
            State.Unarm(unarm, unarmParam);
        }
        public static GameObject WeaponHandObject(this ICharacterAppearance Appearance)
        {
            return Appearance.GetWeaponP1InHand(); 
        }
    }
}
