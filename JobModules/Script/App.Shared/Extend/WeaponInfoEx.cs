using Core;
using App.Shared.Components.Weapon;
using Core.WeaponLogic.Attachment;
using Core.Room;

namespace App.Shared
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
                weaponKey = weapon.WeaponKey,
            };
        }

        public static WeaponInfo ToWeaponInfo(this WeaponEntity weaponEntity)
        {
            if(null == weaponEntity || !weaponEntity.hasWeaponData)
            {
                return new WeaponInfo();
            }
            var weaponInfo = weaponEntity.weaponData.ToWeaponInfo();
            weaponInfo.weaponKey = weaponEntity.entityKey.Value.EntityId;
            return weaponInfo;
        }

        public static WeaponInfo ToWeaponInfo(this WeaponDataComponent weapon)
        {
            if (null == weapon)
            {
                return new WeaponInfo();
            }
            return new WeaponInfo
            {
                Id = weapon.WeaponId,
                AvatarId = weapon.WeaponAvatarId,
                UpperRail = weapon.UpperRail,
                LowerRail = weapon.LowerRail,
                Magazine = weapon.Magazine,
                Muzzle = weapon.Muzzle,
                Stock = weapon.Stock,
                Bullet = weapon.Bullet,
                ReservedBullet = weapon.ReservedBullet,
            };
        }

        public static void ToWeaponComponent(this WeaponInfo weaponInfo, WeaponDataComponent weaponComp)
        {
            weaponComp.WeaponId = weaponInfo.Id;
            weaponComp.WeaponAvatarId = weaponInfo.AvatarId;
            weaponComp.Bullet = weaponInfo.Bullet;
            weaponComp.ReservedBullet = weaponInfo.ReservedBullet;
            weaponComp.UpperRail = weaponInfo.UpperRail;
            weaponComp.LowerRail = weaponInfo.LowerRail;
            weaponComp.Magazine = weaponInfo.Magazine;
            weaponComp.Muzzle = weaponInfo.Muzzle;
            weaponComp.Stock = weaponInfo.Stock;
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
            weaponComp.WeaponKey = weaponInfo.weaponKey;
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

        public static void ClearParts(this WeaponDataComponent weaponDataComponent)
        {
            weaponDataComponent.LowerRail = 0;
            weaponDataComponent.UpperRail = 0;
            weaponDataComponent.Stock = 0;
            weaponDataComponent.Magazine = 0;
            weaponDataComponent.Muzzle = 0;
        }
    }
}
