using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.client;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.Free.item.config;
using Assets.App.Server.GameModules.GamePlay.Free;
using Core;
using Core.Free;
using Free.framework;
using Sharpen;

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
            if (BagCapacityUtil.CanAddToBagCount(room.ContextsWrapper.FreeArgs, (FreeData)player.freeData.FreeData, message.Ins[1], message.Ins[2], message.Ins[3]) > 0)
            {
                if (PickupItemUtil.AddItemToPlayer(room, player, message.Ins[0], message.Ins[1], message.Ins[2], message.Ins[3]))
                {
                    FreeItemInfo item = FreeItemConfig.GetItemInfo(message.Ins[1], message.Ins[2]);
                    if (item.subType == "w4" || item.subType == "w6" || item.subType == "w7")
                    {
                        SimpleProto sp = FreePool.Allocate();
                        sp.Key = FreeMessageConstant.PlaySound;
                        sp.Ks.Add(2);
                        sp.Ins.Add((int) EAudioUniqueId.PickupWeapon);
                        sp.Bs.Add(true);
                        FreeMessageSender.SendMessage(player, sp);
                    }
                }
            }
        }

    }
}
