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
using com.wd.free.item;
using com.cpkf.yyjd.tools.util.math;
using com.cpkf.yyjd.tools.util;
using Core.EntityComponent;
using App.Shared;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Shared.GameModules.GamePlay.Free.Map;
using App.Server.GameModules.GamePlay.Free.client;
using Assets.XmlConfig;
using Utils.Configuration;
using XmlConfig;
using App.Server.GameModules.GamePlay.Free.item;
using App.Shared.FreeFramework.framework.util;
using App.Shared.Components;
using App.Server.GameModules.GamePlay.Free.chicken;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeDragImageHandler : ParaConstant, IFreeMessageHandler
    {
        private const string Ground = "ground";

        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.DragImage;
        }

        private StringPara eventKey = new StringPara(PARA_EVENT_KEY, "");

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            FreeData fd = (FreeData)player.freeData.FreeData;

            room.FreeArgs.TempUse(PARA_PLAYER_CURRENT, fd);
            eventKey.SetValue(message.Ss[0]);
            room.FreeArgs.GetDefault().GetParameters().TempUse(eventKey);

            Debug.Log("drag from:" + message.Ss[0] + " to:" + message.Ss[1]);
            string from = message.Ss[0];
            string to = message.Ss[1];

            // 显示拆分UI
            if (message.Bs[0] && from.StartsWith(ChickenConstant.BagDefault))
            {
                PickupItemUtil.ShowSplitUI(room, fd, from);
                return;
            }

            if (!HandleBag(from, to, room, fd))
            {
                return;
            }
            if (player.gamePlay.GameState == GameState.AirPlane)
            {
                return;
            }
            if (from.StartsWith("belt") || to.StartsWith("belt"))
            {
                // 腰包不能做任何操作
            }
            else if (from.StartsWith(Ground) && to != Ground)
            {
                // 地面模糊操作
                HandleAuto(from, to, room, fd);
            }
            else if (from.StartsWith(Ground) || to.StartsWith(Ground))
            {
                handleFromGround(from, to, room, fd);
                handleToGround(from, to, room, fd);
            }
            // 背包物品拖动到武器槽
            else if (from.StartsWith("default") && to.StartsWith("w"))
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, from, fd.freeInventory.GetInventoryManager());
                if (ip != null)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                    if (info.cat == (int)ECategory.WeaponPart)
                    {
                        MovePartToWeapon(room, fd, ip, to, info);
                    }
                    else
                    {
                        FreeItemManager.DragItem(message.Ss[0], fd, room.FreeArgs, message.Ss[1]);
                    }
                }
            }
            // 背包物品拖动到人身上
            else if (from.StartsWith("default") && string.IsNullOrEmpty(to))
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, from, fd.freeInventory.GetInventoryManager());
                if (ip != null)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                    if (info.cat == (int)ECategory.WeaponPart)
                    {
                        MovePartToWeapon(room, fd, ip, to, info);
                    }
                    else
                    {
                        FreeItemManager.UseItem(from, fd, room.FreeArgs);
                    }
                }
            }
            else
            {
                FreeItemManager.DragItem(message.Ss[0], fd, room.FreeArgs, message.Ss[1]);
            }

            room.FreeArgs.Resume(PARA_PLAYER_CURRENT);
            room.FreeArgs.GetDefault().GetParameters().Resume(PARA_EVENT_KEY);
        }

        private void MovePartToWeapon(ServerRoom room, FreeData fd, ItemPosition ip, string to, FreeItemInfo info)
        {
            string inv = PickupItemUtil.AutoPutPart(fd, info, to, room);
            if (inv != null && inv != "default")
            {
                ItemInventory toInv = fd.freeInventory.GetInventoryManager().GetInventory(inv);
                int[] xy = toInv.GetNextEmptyPosition(ip.key);
                ItemInventoryUtil.MovePosition(ip,
                            toInv, xy[0], xy[1], room.FreeArgs);
            }
            else
            {
                FuntionUtil.Call(room.FreeArgs, "showBottomTip", "msg", "{desc:10076}");
            }
        }

        private bool HandleBag(string from, string to, ServerRoom room, FreeData fd)
        {
            FreeItemInfo info = null;
            if (from.StartsWith("ground"))
            {
                SimpleItemInfo sinfo = PickupItemUtil.GetGroundItemInfo(room, fd, from);
                info = FreeItemConfig.GetItemInfo(sinfo.cat, sinfo.id);
            }
            else
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, from, fd.freeInventory.GetInventoryManager());
                if (ip == null)
                {
                    ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(from.Trim());
                    if (ii != null && ii.posList.Count > 0)
                    {
                        ip = ii.posList[0];
                    }
                }
                if (ip != null)
                {

                    info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                }
            }

            if (info != null && info.cat == 9)
            {
                RoleAvatarConfigItem avatar = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(info.id);
                if (avatar.Capacity > 0)
                {
                    if (from.StartsWith("ground") && !to.StartsWith("ground"))
                    {
                        return BagCapacityUtil.CanAddToBag(room.FreeArgs, fd, info.cat, info.id, 1);
                    }

                    if (to.StartsWith("ground") && !from.StartsWith("ground"))
                    {
                        return BagCapacityUtil.CanTakeOff(room.FreeArgs, fd, info.cat, info.id);
                    }
                }
            }

            return true;
        }

        private void HandleAuto(string from, string to, ServerRoom room, FreeData fd)
        {
            SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, from);

            PickupItemUtil.AddItemToPlayer(room, fd.Player, info.entityId, info.cat, info.id, info.count, to);
        }

        private void handleToGround(string from, string to, ServerRoom room, FreeData fd)
        {
            if (!from.StartsWith(Ground) && to.StartsWith(Ground))
            {
                FreeItemManager.DragItem(from, fd, room.FreeArgs, Ground);
            }
        }

        private void handleFromGround(string from, string to, ServerRoom room, FreeData fd)
        {
            if (from.StartsWith(Ground) && !to.StartsWith(Ground))
            {
                SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, from);
                if (info.cat > 0)
                {
                    ItemInventory inv = fd.freeInventory.GetInventoryManager().GetInventory("ground");

                    if (inv != null)
                    {
                        inv.Clear();
                        FreeItem item = FreeItemManager.GetItem(room.FreeArgs, FreeItemConfig.GetItemKey(info.cat, info.id), info.count);
                        item.GetParameters().AddPara(new IntPara("entityId", info.entityId));
                        inv.AddItem(room.FreeArgs, item, false);

                        DragGroundOne(fd, room, to);
                    }
                    else
                    {
                        Debug.LogErrorFormat("inventory {0} not existed.", from);
                    }

                }
            }
        }

        private void DragGroundOne(FreeData fd, ServerRoom room, string to)
        {
            if (to.StartsWith("default"))
            {
                FreeItemManager.DragItem("ground,0,0", fd, room.FreeArgs, "default");
            }
            else
            {
                FreeItemManager.DragItem("ground,0,0", fd, room.FreeArgs, to);
            }
        }

    }
}
