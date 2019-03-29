using com.wd.free.action;
using com.wd.free.@event;
using Core;
using System;

namespace App.Shared.FreeFramework.Free.Weapon
{
    [Serializable]
    public class SetWeaponBulletAction : AbstractPlayerAction
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
                    weaponBaseAgent.BaseComponent.Bullet = front;
                    weaponBaseAgent.BaseComponent.ReservedBullet = back;
                }
            }
        }
    }
}
