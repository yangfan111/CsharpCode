
using System;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class ChangeWeaponAnimationController
    {
        public Action<Action, float> MountWeapon;
        public Action<Action, Action, float> UnMountWeapon;
        public Action<Action, Action, float> ChangeWeapon;
        public Action InterruptChangeWeaponAnimation;
        public Action InterruptPlayerAnimation;

        public void PlayAnimation()
        {
            
        }
    }
}
