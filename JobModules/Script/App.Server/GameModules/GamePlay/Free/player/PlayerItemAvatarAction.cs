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
using com.wd.free.para;
using Utils.Configuration;
using App.Shared.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using Shared.Scripts;
using Utils.Singleton;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerItemAvatarAction : AbstractPlayerAction
    {
        private bool takeoff;

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

                if (takeoff)
                {
                    TakeOff(playerEntity, itemId);
                }
                else
                {
                    PutOn(playerEntity, itemId);
                }

                ReduceDamageUtil.UpdateArmorAndHelmet((FreeData)playerEntity.freeData.FreeData);
            }

        }

        public static void PutOn(PlayerEntity playerEntity, int id)
        {
            var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(id, playerEntity.GetSex());
            var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
            if (avatar != null)
            {

                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ChangeAvatar;

                playerEntity.appearanceInterface.Appearance.ChangeAvatar(resId);

                message.Ins.Add(id);
                message.Ks.Add(1);

                //FreeMessageSender.SendMessage(playerEntity, message);
                //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }

        public static void TakeOff(PlayerEntity playerEntity, int id)
        {
            var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(id, playerEntity.GetSex());
            var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
            if (avatar != null)
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ChangeAvatar;

                playerEntity.appearanceInterface.Appearance.ClearAvatar((Wardrobe)avatar.AvatarType);

                message.Ins.Add(avatar.AvatarType);
                message.Ks.Add(5);

                //FreeMessageSender.SendMessage(playerEntity, message);
                //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }
    }
}
