using App.Shared.Player;
using Assets.Utils.Configuration;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="PlayerEntityC4BagEx" />
    /// </summary>
    public static class PlayerEntityC4BagEx
    {
        public static void AddC4ToBag(this PlayerEntity playerEntity, int weaponId)
        {
            playerEntity.overrideBag.TacticWeapon = weaponId;
            playerEntity.ChangeC4ToBag();
        }

        public static void MountC4(this PlayerEntity playerEntity, int weaponId)
        {
            playerEntity.ChangeBagToC4(weaponId);
        }

        public static void UnmoutC4(this PlayerEntity playerEntity)
        {
            playerEntity.ChangeC4ToBag();
        }

        public static void RemoveC4(this PlayerEntity playerEntity)
        {
            playerEntity.appearanceInterface.Appearance.UnmountWeaponInPackage(WeaponInPackage.TacticWeapon);
            playerEntity.appearanceInterface.Appearance.ClearAvatar(Utils.CharacterState.Wardrobe.Bag);
        }

        private static void ChangeC4ToBag(this PlayerEntity playerEntity)
        {
            playerEntity.appearanceInterface.Appearance.UnmountWeaponInPackage(WeaponInPackage.TacticWeapon);
            var c4ResId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(RoleAvatarConfigManager.C4, playerEntity.GetSex());
            playerEntity.appearanceInterface.Appearance.ChangeAvatar(c4ResId);
        }

        private static void ChangeBagToC4(this PlayerEntity playerEntity, int weaponId)
        {
            var c4AvatarId = SingletonManager.Get<WeaponConfigManager>().GetAvatarByWeaponId(weaponId);
            playerEntity.appearanceInterface.Appearance.MountWeaponInPackage(WeaponInPackage.TacticWeapon, c4AvatarId);
            playerEntity.appearanceInterface.Appearance.ClearAvatar(Utils.CharacterState.Wardrobe.Bag);
        }
    }
}
