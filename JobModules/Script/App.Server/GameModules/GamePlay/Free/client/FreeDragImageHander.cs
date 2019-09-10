using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.client;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Shared;
using App.Shared.Components;
using App.Shared.GameModules.GamePlay.Free.Map;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.XmlConfig;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using Core;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.rule;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeDragImageHandler : ParaConstant, IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.DragImage;
        }

        private StringPara eventKey = new StringPara(PARA_EVENT_KEY, "");

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            FreeData fd = (FreeData)player.freeData.FreeData;
            var freeArgs = room.ContextsWrapper.FreeArgs as FreeRuleEventArgs;
            freeArgs.TempUse(PARA_PLAYER_CURRENT, fd);
            eventKey.SetValue(message.Ss[0]);
            freeArgs.GetDefault().GetParameters().TempUse(eventKey);

            Debug.Log("drag from:" + message.Ss[0] + " to:" + message.Ss[1]);
            string from = message.Ss[0];
            string to = message.Ss[1];

            ItemPosition fromIp = FreeItemManager.GetItemPosition(freeArgs, from, fd.freeInventory.GetInventoryManager());
            ItemPosition toIp = FreeItemManager.GetItemPosition(freeArgs, to, fd.freeInventory.GetInventoryManager());

            // 显示拆分UI
            if (message.Bs[0] && from.StartsWith(ChickenConstant.BagDefault))
            {
                PickupItemUtil.ShowSplitUI(room, fd, from);
            }
            else if (!HandleBag(from, to, room, fd))
            {

            }
            else if (player.gamePlay.GameState == GameState.AirPlane || player.gamePlay.GameState == GameState.Gliding || player.gamePlay.GameState == GameState.JumpPlane)
            {

            }
            /*else if (from.StartsWith(ChickenConstant.BagBelt) || to.StartsWith(ChickenConstant.BagBelt))
            {
                // 腰包不能做任何操作
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ChickenTip;
                msg.Ss.Add("word63");
                FreeMessageSender.SendMessage(fd.Player, msg);
            }*/
            else if (from.StartsWith(ChickenConstant.BagGround) && !to.StartsWith(ChickenConstant.BagGround))
            {
                // 地面模糊操作
                HandleAuto(from, to, room, fd);
            }
            else if (from.StartsWith(ChickenConstant.BagGround) || to.StartsWith(ChickenConstant.BagGround))
            {
                /*if (from.StartsWith(Ground) && !to.StartsWith(Ground))
                {
                    handleFromGround(from, to, room, fd);
                }*/
                if(!from.StartsWith(ChickenConstant.BagGround) && to.StartsWith(ChickenConstant.BagGround))
                {
                    FreeItemInfo fromInfo = FreeItemConfig.GetItemInfo(fromIp.key.GetKey());
                    if (fromInfo.cat == (int) ECategory.WeaponPart && from.StartsWith("w"))
                    {
                        if (BagCapacityUtil.CanDemountAttachment(room, fd, fromInfo, from, true))
                        {
                            handleToGround(from, to, room, fd);
                        }
                    }
                    else
                    {
                        handleToGround(from, to, room, fd);
                    }
                }
            }
            else if (from.StartsWith(ChickenConstant.BagDefault) && to.StartsWith("w"))
            {
                // 背包物品拖动到武器槽
                if (fromIp != null)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(fromIp.key.GetKey());
                    if (info.cat == (int)ECategory.WeaponPart)
                    {
                        MovePartToWeapon(room, fd, fromIp, to, info);
                    }
                    else
                    {
                        FreeItemManager.DragItem(from, fd, room.ContextsWrapper.FreeArgs, to);
                    }
                }
            }
            else if (PickupItemUtil.IsDefault(from) && string.IsNullOrEmpty(to))
            {
                // 背包物品拖动到人身上
                if (fromIp != null)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(fromIp.key.GetKey());
                    if (info.cat == (int)ECategory.WeaponPart)
                    {
                        MovePartToWeapon(room, fd, fromIp, to, info);
                    }
                    else
                    {
                        FreeItemManager.UseItem(from, fd, room.ContextsWrapper.FreeArgs);
                    }
                }
            }
            else if ((from.StartsWith("w1,") && to.StartsWith("w2,")) || (from.StartsWith("w2,") && to.StartsWith("w1,")))
            {
                ExchangeWeapon(room.ContextsWrapper.FreeArgs, fd, from, to);
            }
            else if (from.StartsWith("w") && to.StartsWith("w") && from.IndexOf(",") == 3 && to.IndexOf(",") == 2)
            {
                if (fromIp != null && toIp != null)
                {
                    WeaponBaseAgent toAgent = fd.Player.WeaponController().GetWeaponAgent((EWeaponSlotType) short.Parse(to.Substring(1, 1)));
                    FreeItemInfo fromInfo = FreeItemConfig.GetItemInfo(fromIp.key.GetKey());
                    if (!toAgent.WeaponConfigAssy.IsPartMatchWeapon(WeaponPartUtil.GetWeaponFstMatchedPartId(fromInfo.id, toAgent.ConfigId)))
                    {
                        SimpleProto msg = FreePool.Allocate();
                        msg.Key = FreeMessageConstant.ChickenTip;
                        msg.Ss.Add("word77," + toAgent.WeaponConfigAssy.S_Name + "," + fromInfo.name);
                        FreeMessageSender.SendMessage(fd.Player, msg);
                    }
                    else
                    {
                        string toPosition = to.Substring(0, 2) + from.Substring(2, 1) + ",0,0";
                        ItemPosition toPart = FreeItemManager.GetItemPosition(room.ContextsWrapper.FreeArgs, toPosition, fd.freeInventory.GetInventoryManager());
                        WeaponBaseAgent fromAgent = fd.Player.WeaponController().GetWeaponAgent((EWeaponSlotType) short.Parse(from.Substring(1, 1)));
                        if (toPart != null)
                        {
                            FreeItemInfo toInfo = FreeItemConfig.GetItemInfo(toPart.key.GetKey());
                            if (!fromAgent.WeaponConfigAssy.IsPartMatchWeapon(WeaponPartUtil.GetWeaponFstMatchedPartId(toInfo.id, fromAgent.ConfigId)))
                            {
                                SimpleProto msg = FreePool.Allocate();
                                msg.Key = FreeMessageConstant.ChickenTip;
                                msg.Ss.Add("word77," + fromAgent.WeaponConfigAssy.S_Name + "," + toInfo.name);
                                FreeMessageSender.SendMessage(fd.Player, msg);
                            }
                            else
                            {
                                if (BagCapacityUtil.CanExchangeAttachment(room, fd, fromInfo, toInfo, fromAgent, toAgent))
                                {
                                    FreeItemManager.DragItem(from, fd, room.ContextsWrapper.FreeArgs, toPosition);
                                }
                            }
                        }
                        else
                        {
                            if (BagCapacityUtil.CanDemountAttachment(room, fd, fromInfo, from, false))
                            {
                                FreeItemManager.DragItem(from, fd, room.ContextsWrapper.FreeArgs, toPosition);
                            }
                        }
                    }
                }
            }
            else if(from.StartsWith("w") && from.IndexOf(",") == 3 && PickupItemUtil.IsDefault(to))
            {
                FreeItemInfo fromInfo = FreeItemConfig.GetItemInfo(fromIp.key.GetKey());
                if (BagCapacityUtil.CanDemountAttachment(room, fd, fromInfo, from, false))
                {
                    ItemInventory fromInv = fromIp.GetInventory();
                    ItemInventory toInv = toIp == null ? fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagDefault) : toIp.GetInventory();
                    int[] pos = toIp == null ? new int[] {0, 0} : toInv.GetNextEmptyPosition(toIp.GetKey());
                    ItemInventoryUtil.MoveItem(pos[0], pos[1], fromInv, toInv, fromInv.GetInventoryUI(), toInv.GetInventoryUI(), fromIp, null, room.ContextsWrapper.FreeArgs);
                }
            }
            else
            {
                FreeItemManager.DragItem(from, fd, room.ContextsWrapper.FreeArgs, to);
            }
            
            room.ContextsWrapper.FreeArgs.Resume(PARA_PLAYER_CURRENT);
            room.ContextsWrapper.FreeArgs.GetDefault().GetParameters().Resume(PARA_EVENT_KEY);
        }

        private void ExchangeWeapon(ISkillArgs args, FreeData fd, string from, string to)
        {
            string fromWeapon = from.Split(',')[0].Trim();
            string toWeapon = to.Split(',')[0].Trim();

            if (fromWeapon != toWeapon)
            {
                ItemInventory fromInv = fd.freeInventory.GetInventoryManager().GetInventory(fromWeapon);
                ItemInventory toInv = fd.freeInventory.GetInventoryManager().GetInventory(toWeapon);

                ExchangeInv(args, fromInv, toInv);

                string fromKey = fromWeapon.Substring(1, 1);
                string toKey = toWeapon.Substring(1, 1);

                for (int i = 1; i <= 5; i++)
                {
                    ExchangeInv(args, fd.freeInventory.GetInventoryManager().GetInventory("w" + fromKey + i),
                        fd.freeInventory.GetInventoryManager().GetInventory("w" + toKey + i));
                }

                fd.Player.ModeController().ExchangePlayerWeapon(fd.Player);
            }
        }

        private void ExchangeInv(ISkillArgs args, ItemInventory fromInv, ItemInventory toInv)
        {
            ItemPosition temp = null;
            if (toInv.posList.Count > 0)
            {
                temp = toInv.posList[0];
            }
            toInv.posList.Clear();

            ItemPosition fromIp = null;
            if (fromInv.posList.Count > 0)
            {
                fromIp = fromInv.posList[0];
            }
            fromInv.posList.Clear();

            if (temp != null)
            {
                temp.inventory = fromInv;
                fromInv.posList.Add(temp);
            }

            if (fromIp != null)
            {
                fromIp.inventory = toInv;
                toInv.posList.Add(fromIp);
            }

            fromInv.ReDraw(args);
            toInv.ReDraw(args);
        }

        private void MovePartToWeapon(ServerRoom room, FreeData fd, ItemPosition ip, string to, FreeItemInfo info)
        {
            string inv = PickupItemUtil.AutoPutPart(fd, info, to, room);
            if (inv != null && inv != ChickenConstant.BagDefault)
            {
                ItemInventory toInv = fd.freeInventory.GetInventoryManager().GetInventory(inv);
                int[] xy = toInv.GetNextEmptyPosition(ip.key);
                ItemInventoryUtil.MovePosition(ip, toInv, xy[0], xy[1], room.ContextsWrapper.FreeArgs);
            }
            else
            {
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ChickenTip;
                msg.Ss.Add("word76");
                FreeMessageSender.SendMessage(fd.Player, msg);
            }
        }

        private bool HandleBag(string from, string to, ServerRoom room, FreeData fd)
        {
            FreeItemInfo info = null;
            if (from.StartsWith(ChickenConstant.BagGround))
            {
                SimpleItemInfo sinfo = PickupItemUtil.GetGroundItemInfo(room, fd, from);
                info = FreeItemConfig.GetItemInfo(sinfo.cat, sinfo.id);
            }
            else
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.ContextsWrapper.FreeArgs, from, fd.freeInventory.GetInventoryManager());
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
                    if (from.StartsWith(ChickenConstant.BagGround) && !to.StartsWith(ChickenConstant.BagGround))
                    {
                        return BagCapacityUtil.CanAddToBag(room.ContextsWrapper.FreeArgs, fd, info.cat, info.id, 1);
                    }

                    if (to.StartsWith(ChickenConstant.BagGround) && !from.StartsWith(ChickenConstant.BagGround))
                    {
                        return BagCapacityUtil.CanTakeOff(room.ContextsWrapper.FreeArgs, fd, info.cat, info.id);
                    }
                }
            }

            return true;
        }

        private void HandleAuto(string from, string to, ServerRoom room, FreeData fd)
        {
            SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, from);
            if (PickupItemUtil.AddItemToPlayer(room, fd.Player, info.entityId, info.cat, info.id, info.count, to))
            {
                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.PlaySound;
                sp.Ks.Add(2);
                sp.Ins.Add(5018);
                sp.Bs.Add(false);
                FreeMessageSender.SendMessage(fd.Player, sp);
            }
        }

        private void handleToGround(string from, string to, ServerRoom room, FreeData fd)
        {
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, fd.Player.gamePlay);
            FreeItemManager.DragItem(from, fd, room.ContextsWrapper.FreeArgs, ChickenConstant.BagGround);

            SimpleProto sp = FreePool.Allocate();
            sp.Key = FreeMessageConstant.PlaySound;
            sp.Ks.Add(2);
            sp.Ins.Add(5017);
            sp.Bs.Add(false);
            FreeMessageSender.SendMessage(fd.Player, sp);
        }

        private void handleFromGround(string from, string to, ServerRoom room, FreeData fd)
        {
            SimpleItemInfo info = PickupItemUtil.GetGroundItemInfo(room, fd, from);
            if (info.cat > 0)
            {
                ItemInventory inv = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagGround);

                if (inv != null)
                {
                    inv.Clear();
                    FreeItem item = FreeItemManager.GetItem(room.ContextsWrapper.FreeArgs, FreeItemConfig.GetItemKey(info.cat, info.id), info.count);
                    item.GetParameters().AddPara(new IntPara("entityId", info.entityId));
                    inv.AddItem(room.ContextsWrapper.FreeArgs, item, false);

                    DragGroundOne(fd, room, to);
                }
                else
                {
                    Debug.LogErrorFormat("inventory {0} not existed.", from);
                }
            }
        }

        private void DragGroundOne(FreeData fd, ServerRoom room, string to)
        {
            if (to.StartsWith(ChickenConstant.BagDefault))
            {
                FreeItemManager.DragItem("ground,0,0", fd, room.ContextsWrapper.FreeArgs, ChickenConstant.BagDefault);
            }
            else
            {
                FreeItemManager.DragItem("ground,0,0", fd, room.ContextsWrapper.FreeArgs, to);
            }
        }

    }
}
