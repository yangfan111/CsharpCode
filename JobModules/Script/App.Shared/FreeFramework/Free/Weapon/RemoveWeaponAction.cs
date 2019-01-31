using com.wd.free.action;
using System;
using com.wd.free.@event;
using com.wd.free.unit;
using App.Server.GameModules.GamePlay.free.player;
using Core;
using com.wd.free.util;
using UnityEngine;
using App.Shared;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class NewRemoveWeaponAction : AbstractPlayerAction
    {
        private string weaponKey;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            IGameUnit unit = GetPlayer(args);

            if (unit != null)
            {
                PlayerEntity p = ((FreeData)unit).Player;

                int index = FreeUtil.ReplaceInt(weaponKey, args);

                EWeaponSlotType currentSlot = p.GetController<PlayerWeaponController>().CurrSlotType;

                if (index > 0)
                {
                    currentSlot = FreeWeaponUtil.GetSlotType(index);
                }

                Debug.LogFormat("remove weapon: " + index);

                p.GetController<PlayerWeaponController>().DropSlotWeapon(args.GameContext, currentSlot);

                //SimpleProto message = new SimpleProto();
                //message.Key = FreeMessageConstant.ChangeAvatar;
                //message.Ins.Add((int)currentSlot);
                //message.Ks.Add(6);
                //p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }
    }
}
