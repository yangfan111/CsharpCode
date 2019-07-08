using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Core;
using System;

namespace App.Shared.FreeFramework.Free.Weapon
{
    [Serializable]
    public class SetWeaponBulletAction : AbstractPlayerAction, IRule
    {
        public int weaponKey;
        public int front;
        public int back;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (null != playerEntity)
            {
                var weaponBaseAgent = playerEntity.WeaponController().GetWeaponAgent((EWeaponSlotType) weaponKey);
                if (weaponBaseAgent.IsValid())
                {
                    if (front > 0) {
                        weaponBaseAgent.BaseComponent.Bullet = front;
                    } else {
                        weaponBaseAgent.BaseComponent.Bullet = weaponBaseAgent.WeaponConfigAssy.PropertyCfg.Bullet;
                    }
                    if (back > 0) {
                        weaponBaseAgent.BaseComponent.ReservedBullet = back;
                    } else {
                        weaponBaseAgent.BaseComponent.ReservedBullet = weaponBaseAgent.WeaponConfigAssy.PropertyCfg.Bulletmax;

                    }
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SetWeaponBulletAction;
        }
    }
}
