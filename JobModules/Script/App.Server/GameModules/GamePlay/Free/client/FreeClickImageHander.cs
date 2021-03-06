﻿using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.client;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.XmlConfig;
using com.wd.free.item;
using com.wd.free.para;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.rule;
using UnityEngine;

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

            room.ContextsWrapper.FreeArgs.TempUse(PARA_PLAYER_CURRENT, fd);
            eventKey.SetValue(message.Ss[0]);
            room.ContextsWrapper.FreeArgs.GetDefault().GetParameters().TempUse(eventKey);

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
                if (key.StartsWith(ChickenConstant.BagGround))
                {
                    SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, key);
                    if (info.cat > 0)
                    {
                        if (CanChangeBag(room, fd, key))
                        {
                            if (PickupItemUtil.AddItemToPlayer(room, player, info.entityId, info.cat, info.id, info.count))
                            {
                                SimpleProto sp = FreePool.Allocate();
                                sp.Key = FreeMessageConstant.PlaySound;
                                sp.Ks.Add(2);
                                sp.Ins.Add(5018);
                                sp.Bs.Add(false);
                                FreeMessageSender.SendMessage(player, sp);
                            }
                        }

                    }
                }
                else if (key.StartsWith(ChickenConstant.BagDefault))
                {
                    ItemPosition ip = FreeItemManager.GetItemPosition(room.ContextsWrapper.FreeArgs, key, fd.GetFreeInventory().GetInventoryManager());
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                    if (info.cat == (int)ECategory.WeaponPart)
                    {
                        string inv = PickupItemUtil.AutoPutPart(fd, FreeItemConfig.GetItemInfo(info.cat, info.id));
                        if (inv != null && inv != ChickenConstant.BagDefault)
                        {
                            ItemInventoryUtil.MovePosition(ip,
                                fd.GetFreeInventory().GetInventoryManager().GetInventory(inv), 0, 0, room.ContextsWrapper.FreeArgs);
                        }
                    }
                    else
                    {
                        FreeItemManager.UseItem(key, fd, room.ContextsWrapper.FreeArgs);
                    }
                }
                // 点击装配好的配件，自动进背包
                else if (key.StartsWith("w") && key.IndexOf(",") == 3)
                {
                    ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(key.Substring(0, 3));
                    ItemInventory defaultInventory = fd.GetFreeInventory().GetInventoryManager().GetDefaultInventory();
                    
                    if (ii != null && ii.posList.Count > 0)
                    {
                        ItemPosition ip = ii.posList[0];
                        if (BagCapacityUtil.CanDemountAttachment(room, fd, FreeItemConfig.GetItemInfo(ip.key.GetKey()), key, false))
                        {
                            int[] xy = defaultInventory.GetNextEmptyPosition(ip.GetKey());
                            ItemInventoryUtil.MovePosition(ip,
                                    defaultInventory, xy[0], xy[1], room.ContextsWrapper.FreeArgs);
                        }
                    }
                }
                else
                {
                    FreeItemManager.UseItem(key, fd, room.ContextsWrapper.FreeArgs);
                }
            }

            room.ContextsWrapper.FreeArgs.Resume(PARA_PLAYER_CURRENT);
            room.ContextsWrapper.FreeArgs.GetDefault().GetParameters().Resume(PARA_EVENT_KEY);
        }

        private bool CanChangeBag(ServerRoom room, FreeData fd, string key)
        {
            if (key.StartsWith(ChickenConstant.BagGround))
            {
                SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, key);

                return BagCapacityUtil.CanChangeBag(room.ContextsWrapper.FreeArgs, fd, info.cat, info.id);
            }
            return true;
        }
    }
}
