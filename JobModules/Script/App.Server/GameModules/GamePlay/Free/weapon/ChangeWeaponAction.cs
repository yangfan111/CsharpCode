using App.Shared;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Core;
using Core.Free;
using Free.framework;
using System;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class ChangeWeaponAction : AbstractPlayerAction, IRule
    {
        private string weaponKey;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            int index = FreeUtil.ReplaceInt(weaponKey, args);
            EWeaponSlotType st = FreeWeaponUtil.GetSlotType(index);
            SimpleProto message = FreePool.Allocate();
            message.Key = FreeMessageConstant.ChangeWeapon;
            switch (st)
            {
                case EWeaponSlotType.None:
                    playerEntity.WeaponController().UnArmWeapon(false);
                    message.Ins.Add(0);
                    break;
                default:
                    playerEntity.WeaponController().PureSwitchIn(st);
                    message.Ins.Add(index);
                    break;
            }
            FreeMessageSender.SendMessage(playerEntity, message);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.ChangeWeaponAction;
        }
    }
}
