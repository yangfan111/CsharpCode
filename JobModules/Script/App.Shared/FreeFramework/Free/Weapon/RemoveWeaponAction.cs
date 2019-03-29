using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;
using Core;
using System;
using UnityEngine;

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

                EWeaponSlotType currentSlot = p.WeaponController().HeldSlotType;

                if (index > 0)
                {
                    currentSlot = FreeWeaponUtil.GetSlotType(index);
                }

                Debug.LogFormat("remove weapon: " + index);

                p.WeaponController().DestroyWeapon(currentSlot, -1);

                //SimpleProto message = new SimpleProto();
                //message.Key = FreeMessageConstant.ChangeAvatar;
                //message.Ins.Add((int)currentSlot);
                //message.Ks.Add(6);
                //p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }
    }
}
