using Core.Bag;
using Core.Room;

namespace App.Shared.WeaponLogic
{
    public static class WeaponInfoEx
    {
        public static WeaponInfo ToWeaponInfo(this Components.SceneObject.WeaponComponent weapon)
        {
            return new WeaponInfo
            {
                Id = weapon.Id,
                AvatarId = weapon.AvatarId,
                UpperRail = weapon.UpperRail,
                LowerRail = weapon.LowerRail,
                Magazine = weapon.Magazine,
                Muzzle = weapon.Muzzle,
                Stock = weapon.Stock,
                Bullet = weapon.Bullet,
                ReservedBullet = weapon.ReservedBullet,
            };
        }

        public static WeaponInfo ToWeaponInfo(this Components.Bag.WeaponComponent weapon)
        {
            return new WeaponInfo
            {
                Id = weapon.Id,
                AvatarId = weapon.AvatarId,
                UpperRail = weapon.UpperRail,
                LowerRail = weapon.LowerRail,
                Magazine = weapon.Magazine,
                Muzzle = weapon.Muzzle,
                Stock = weapon.Stock,
                Bullet = weapon.Bullet,
                ReservedBullet = weapon.ReservedBullet,
            };
        }

        public static void ToPlayerWeaponComponent(this WeaponInfo weaponInfo, Components.Bag.WeaponComponent weaponComp)
        {
            weaponComp.Id = weaponInfo.Id;
            weaponComp.AvatarId = weaponInfo.AvatarId;
            weaponComp.UpperRail = weaponInfo.UpperRail;
            weaponComp.LowerRail = weaponInfo.LowerRail;
            weaponComp.Magazine = weaponInfo.Magazine;
            weaponComp.Muzzle = weaponInfo.Muzzle;
            weaponComp.Stock = weaponInfo.Stock;
            weaponComp.Bullet = weaponInfo.Bullet;
            weaponComp.ReservedBullet = weaponInfo.ReservedBullet;
        }

        public static void ToSceneWeaponComponent(this WeaponInfo weaponInfo, Components.SceneObject.WeaponComponent weaponComp)
        {
            weaponComp.Id = weaponInfo.Id;
            weaponComp.AvatarId = weaponInfo.AvatarId;
            weaponComp.UpperRail = weaponInfo.UpperRail;
            weaponComp.LowerRail = weaponInfo.LowerRail;
            weaponComp.Magazine = weaponInfo.Magazine;
            weaponComp.Muzzle = weaponInfo.Muzzle;
            weaponComp.Stock = weaponInfo.Stock;
            weaponComp.Bullet = weaponInfo.Bullet;
            weaponComp.ReservedBullet = weaponInfo.ReservedBullet;
        }

        public static WeaponInfo ToWeaponInfo(this PlayerWeaponData weaponData)
        {
            return new WeaponInfo
            {
                Id = weaponData.WeaponTplId,
                AvatarId = weaponData.WeaponAvatarTplId,
                Muzzle = weaponData.Muzzle,
                Stock = weaponData.Stock,
                Magazine = weaponData.Magazine,
                UpperRail = weaponData.UpperRail,
                LowerRail = weaponData.LowerRail,
            };
        }
    }
}
