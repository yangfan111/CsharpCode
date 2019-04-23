using XmlConfig;

namespace Utils.Appearance.Weapon
{
    public static class WeaponTypeConvertUtil
    {
        public static WeaponPartLocation GetLocationByPartType(EWeaponPartType partType)
        {
            switch (partType)
            {
                case EWeaponPartType.Magazine:
                    return WeaponPartLocation.Magazine;
                case EWeaponPartType.LowerRail:
                    return WeaponPartLocation.LowRail;
                case EWeaponPartType.Muzzle:
                    return WeaponPartLocation.Muzzle;
                case EWeaponPartType.SideRail:
                    return WeaponPartLocation.EndOfTheWorld;
                case EWeaponPartType.Stock:
                    return WeaponPartLocation.Buttstock;
                case EWeaponPartType.UpperRail:
                    return WeaponPartLocation.Scope;
                default:
                    return WeaponPartLocation.EndOfTheWorld;
            }
        }
    }
}
