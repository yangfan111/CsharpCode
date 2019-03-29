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
using Core.Configuration;
using XmlConfig;
using App.Server.GameModules.GamePlay.Free.weapon;
using Core;
using com.wd.free.para;
using Utils.Configuration;
using Assets.App.Server.GameModules.GamePlay.Free;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerItemPartAction : AbstractPlayerAction
    {
        private bool delete;
        private string weaponKey;

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
                EWeaponSlotType type = FreeWeaponUtil.GetSlotType(FreeUtil.ReplaceInt(weaponKey, args));

                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ChangeAvatar;

                int itemId = FreeUtil.ReplaceInt("{item.itemId}", args);

                if (!delete)
                {
                    playerEntity.WeaponController().SetWeaponPart(type, itemId);

                    message.Ins.Add((int)type);
                    message.Ins.Add(itemId);
                    message.Ks.Add(3);

                    FreeMessageSender.SendMessage(playerEntity, message);
                    //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
                }
                else
                {
                    var part = SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(itemId));

                    playerEntity.WeaponController().DeleteWeaponPart(type, part);

                    message.Ins.Add((int)type);
                    message.Ins.Add((int)part);
                    message.Ks.Add(4);

                    FreeMessageSender.SendMessage(playerEntity, message);
                    //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
                }

            }

        }
    }
}
