using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.para;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.rule;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeMoveImageHandler : ParaConstant, IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.MoveImage;
        }

        private StringPara eventKey = new StringPara(PARA_EVENT_KEY, "");

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            room.GameRule.HandleFreeEvent(room.RoomContexts, player, message);

            FreeData fd = (FreeData)player.freeData.FreeData;

            room.FreeArgs.TempUse(PARA_PLAYER_CURRENT, fd);
            eventKey.SetValue(message.Ss[0]);
            room.FreeArgs.GetDefault().GetParameters().TempUse(eventKey);

            FreeItemManager.MoveItem(message.Ss[0], fd, room.FreeArgs, message.Ins[0], message.Ins[1], message.Ins[2], message.Ins[3],
                message.Ins[4], message.Ins[5], message.Ins[6], message.Ins[7]);

            room.FreeArgs.Resume(PARA_PLAYER_CURRENT);
            room.FreeArgs.GetDefault().GetParameters().Resume(PARA_EVENT_KEY);
        }
    }
}
