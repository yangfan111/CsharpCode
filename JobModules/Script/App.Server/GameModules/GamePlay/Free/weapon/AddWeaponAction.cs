using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.unit;
using App.Server.GameModules.GamePlay.free.player;
using Core;
using com.wd.free.util;
using UnityEngine;
using Free.framework;
using App.Shared;
using Core.CharacterState;
using Core.Free;
using XmlConfig;
using Assets.App.Server.GameModules.GamePlay.Free;
using App.Shared.GameModules.Weapon;
using Core.Utils;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class NewAddWeaponAction : AbstractPlayerAction
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NewAddWeaponAction));

        private string weaponKey;

        private string weaponId;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            IGameUnit unit = GetPlayer(args);

            if (unit != null)
            {
                PlayerEntity p = ((FreeData)unit).Player;


                int itemId = FreeUtil.ReplaceInt(weaponId, args);

                int index = FreeUtil.ReplaceInt(weaponKey, args);

                EWeaponSlotType st = FreeWeaponUtil.GetSlotType(index);

                Debug.LogFormat("add weapon: " + itemId + "," + index);
                Logger.Debug("add weapon to team " + p.playerInfo.Camp + " player " + p.playerInfo.PlayerName);

                SimpleProto message = new SimpleProto();
                if (index == 0)
                {
                    p.WeaponController().PickUpWeapon(WeaponUtil.CreateScan(itemId));
                    //p.bag.Bag.SetWeaponBullet(30);
                    //p.bag.Bag.SetReservedCount(100);
                }
                else
                {
                    p.WeaponController().ReplaceWeaponToSlot(st, WeaponUtil.CreateScan(itemId));

                    if (p.stateInterface.State.CanDraw() && p.WeaponController().HeldSlotType == EWeaponSlotType.None)
                    {
                        p.WeaponController().TryArmWeapon(st);
                    }

                    //SwitchWeaponAction.WeaponToHand(p, st);
                }

                message.Ins.Add(itemId);
                if (index > 0)
                {
                    message.Ins.Add((int)st);
                }
                else
                {
                    message.Ins.Add(-1);
                }

                message.Ks.Add(2);
                message.Key = FreeMessageConstant.ChangeAvatar;
                FreeMessageSender.SendMessage(p, message);
                //p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }
    }
}
