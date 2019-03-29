using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Free.framework;
using Core.Free;
using App.Shared;
using com.wd.free.item;
using com.wd.free.util;
using com.wd.free.para;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerItemWeaponAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            if (string.IsNullOrEmpty(player))
            {
                player = "current";
            }

            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            PlayerEntity playerEntity = (PlayerEntity)fr.GetEntity(player);

            IParable item = args.GetUnit("item");

            if (playerEntity != null && item != null)
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ChangeAvatar;

                int itemId = FreeUtil.ReplaceInt("{item.itemId}", args);

                playerEntity.WeaponController().PickUpWeapon(WeaponUtil.CreateScan(itemId));
                //playerEntity.bag.Bag.SetWeaponBullet(30);
                //playerEntity.bag.Bag.SetReservedCount(100);

                message.Ins.Add(itemId);

                message.Ks.Add(2);

                playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }

        }
    }
}
