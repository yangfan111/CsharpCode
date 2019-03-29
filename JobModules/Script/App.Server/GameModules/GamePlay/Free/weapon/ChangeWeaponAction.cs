using com.wd.free.action;
using System;
using App.Shared;
using App.Shared.GameModules.Weapon;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.@event;
using Core;
using com.wd.free.util;
using Core.Free;
using Free.framework;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class ChangeWeaponAction : AbstractPlayerAction
    {
        private string weaponKey;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            int index = FreeUtil.ReplaceInt(weaponKey, args);
            EWeaponSlotType st = FreeWeaponUtil.GetSlotType(index);

            playerEntity.WeaponController().PureSwitchIn(st);

            SimpleProto message = FreePool.Allocate();
            message.Key = FreeMessageConstant.ChangeWeapon;
            message.Ins.Add(index);
            FreeMessageSender.SendMessage(playerEntity, message);
        }
    }
}
