using App.Shared.Components.Player;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// 定义entity Weapon相关components原子操作
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
    }
}
