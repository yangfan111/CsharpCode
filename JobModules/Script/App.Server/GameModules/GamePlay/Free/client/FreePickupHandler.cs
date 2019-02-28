using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using Sharpen;
using Core.Configuration;
using XmlConfig;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.action;
using App.Shared;
using com.wd.free.item;
using App.Server.GameModules.GamePlay.Free.map.position;
using App.Server.GameModules.GamePlay.Free.item.config;
using Assets.XmlConfig;
using com.wd.free.util;
using UnityEngine;
using App.Server.GameModules.GamePlay.Free.client;
using App.Server.GameModules.GamePlay.Free.item;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreePickupHandler : IFreeMessageHandler
    {

        private static MyDictionary<string, string> typeInvDic;

        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.PickUpItem;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            if (BagCapacityUtil.CanAddToBag(room.FreeArgs, (FreeData)player.freeData.FreeData, message.Ins[1], message.Ins[2], message.Ins[3]))
            {
                PickupItemUtil.AddItemToPlayer(room, player, message.Ins[0], message.Ins[1], message.Ins[2], message.Ins[3]);
            }
        }

    }
}
