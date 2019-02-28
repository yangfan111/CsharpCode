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
using UnityEngine;
using App.Server.GameModules.GamePlay.Free.client;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.XmlConfig;
using App.Server.GameModules.GamePlay.Free.item.config;
using com.wd.free.item;
using Utils.Configuration;
using XmlConfig;
using App.Server.GameModules.GamePlay.Free.item;
using Assets.App.Server.GameModules.GamePlay.Free;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeClickImageHandler : ParaConstant, IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.ClickImage;
        }

        private StringPara eventKey = new StringPara(PARA_EVENT_KEY, "");

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            FreeData fd = (FreeData)player.freeData.FreeData;

            room.FreeArgs.TempUse(PARA_PLAYER_CURRENT, fd);
            eventKey.SetValue(message.Ss[0]);
            room.FreeArgs.GetDefault().GetParameters().TempUse(eventKey);

            room.GameRule.HandleFreeEvent(room.RoomContexts, player, message);

            string key = message.Ss[0];
            Debug.LogFormat("click item {0}. ", key);
            if (message.Bs[0])
            {
                // 显示拆分UI
                if (message.Bs[1])
                {
                    PickupItemUtil.ShowSplitUI(room, fd, key);
                    return;
                }
                if (key.StartsWith("ground"))
                {
                    SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, key);
                    if (info.cat > 0)
                    {
                        if (CanChangeBag(room, fd, key))
                        {
                            PickupItemUtil.AddItemToPlayer(room, player, info.entityId, info.cat, info.id, info.count);
                        }

                    }
                }
                else if (key.StartsWith("default"))
                {

                    ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, key, fd.GetFreeInventory().GetInventoryManager());
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                    if (info.cat == (int)ECategory.WeaponPart)
                    {
                        string inv = PickupItemUtil.AutoPutPart(fd, FreeItemConfig.GetItemInfo(info.cat, info.id));
                        if (inv != null && inv != "default")
                        {
                            ItemInventoryUtil.MovePosition(ip,
                                fd.GetFreeInventory().GetInventoryManager().GetInventory(inv), 0, 0, room.FreeArgs);
                        }
                    }
                    else
                    {
                        FreeItemManager.UseItem(key, fd, room.FreeArgs);
                    }
                }
                // 点击装配好的配件，自动进背包
                else if (key.StartsWith("w") && key.Length == 3)
                {
                    ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(key);
                    ItemInventory defaultInventory = fd.GetFreeInventory().GetInventoryManager().GetDefaultInventory();

                    if (ii != null && ii.posList.Count > 0)
                    {
                        ItemPosition ip = ii.posList[0];
                        if (BagCapacityUtil.CanAddToBag(room.FreeArgs, fd, ip))
                        {
                            int[] xy = defaultInventory.GetNextEmptyPosition(ip.GetKey());
                            ItemInventoryUtil.MovePosition(ip,
                                    defaultInventory, xy[0], xy[1], room.FreeArgs);
                        }
                    }
                }
                else
                {
                    FreeItemManager.UseItem(key, fd, room.FreeArgs);
                }
            }

            room.FreeArgs.Resume(PARA_PLAYER_CURRENT);
            room.FreeArgs.GetDefault().GetParameters().Resume(PARA_EVENT_KEY);
        }

        private bool CanChangeBag(ServerRoom room, FreeData fd, string key)
        {
            if (key.StartsWith("ground"))
            {
                SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, key);

                return BagCapacityUtil.CanChangeBag(room.FreeArgs, fd, info.cat, info.id);
            }

            return true;
        }
    }
}
