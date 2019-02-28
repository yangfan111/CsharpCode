using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using gameplay.gamerule.free.rule;
using gameplay.gamerule.free.ui;
using App.Server.GameModules.GamePlay.Free.item;
using UnityEngine;
using App.Shared;
using App.Server.GameModules.GamePlay.Free.item.config;
using Assets.XmlConfig;
using XmlConfig;

namespace gameplay.gamerule.free.item
{
    public class FreeItemManager : ParaConstant
    {
        private static MyDictionary<string, MyDictionary<string, FreeItem>> map;

        static FreeItemManager()
        {
            map = new MyDictionary<string, MyDictionary<string, FreeItem>>();
        }

        public static bool ContainsItem(string rule, string itemKey)
        {
            return map.ContainsKey(rule) && map[rule].ContainsKey(itemKey);
        }

        public static void Clear()
        {
            map.Clear();
        }

        public static void AddItem(IEventArgs args, string rule, FreeItem item)
        {
            item = item.Clone();
            if (!map.ContainsKey(rule))
            {
                map[rule] = new MyDictionary<string, FreeItem>();
            }
            map[rule][item.GetKey()] = item;
        }

        public static FreeItem GetItem(FreeRuleEventArgs args, string key, int count)
        {
            string rule = args.Rule.FreeType;
            if (map.ContainsKey(rule) && map[rule].ContainsKey(key))
            {
                FreeItem fi = map[rule][key];
                fi = fi.Clone();
                fi.SetCount(count);
                if (fi.GetId() == 0)
                {
                    fi.SetId();
                }
                return fi;
            }
            else
            {
                throw new GameConfigExpception("item '" + key + "' is not defined.");
            }
        }

        public static ItemPosition GetItemPosition(ISkillArgs args, string key, InventoryManager ins)
        {
            string[] xy = StringUtil.Split(key, SPLITER_COMMA);
            if (xy.Length == 3)
            {
                string name = xy[0];
                int x = NumberUtil.GetInt(xy[1]);
                int y = NumberUtil.GetInt(xy[2]);
                ItemInventory inventory = ins.GetInventory(name);
                if (inventory != null)
                {
                    return inventory.GetItem(x, y);
                }
            }
            return null;
        }

        public static void DragItem(string key, FreeData fd, ISkillArgs args, string toKey)
        {
            ItemPosition ip = GetItemPosition(args, key, fd.freeInventory.GetInventoryManager());
            if (ip == null)
            {
                ItemInventory inv = fd.freeInventory.GetInventoryManager().GetInventory(key.Trim());
                if (inv != null && inv.GetInventoryUI() is UnityOneInventoryUi)
                {
                    if (inv.posList.Count > 0)
                    {
                        ip = inv.posList[0];
                    }
                }
            }
            ItemPosition toIp = null;
            if (!StringUtil.IsNullOrEmpty(toKey))
            {
                toIp = GetItemPosition(args, toKey, fd.freeInventory.GetInventoryManager());
            }
            if (ip != null)
            {
                InventoryManager invManager = fd.freeInventory.GetInventoryManager();
                ItemInventory fromInv = ip.GetInventory();

                ItemInventory toInv = null;
                if (toIp != null)
                {
                    toInv = toIp.GetInventory();
                }
                else
                {
                    toInv = fd.freeInventory.GetInventoryManager().GetInventory(toKey);
                }

                int x = -1;
                int y = -1;
                if (toIp != null)
                {
                    x = toIp.x;
                    y = toIp.y;
                }
                else if (toInv != null)
                {
                    if (toInv.GetName() == "default")
                    {
                        foreach (ItemPosition old in toInv.GetItems())
                        {
                            if (old.GetKey().GetKey() == ip.GetKey().GetKey() && old.GetCount() < ip.GetKey().GetItemStack())
                            {
                                x = old.GetX();
                                y = old.GetY();
                                break;
                            }
                        }
                    }

                    if (x < 0)
                    {
                        int[] pos = toInv.GetNextEmptyPosition(ip.GetKey());
                        x = pos[0];
                        y = pos[1];
                    }
                }

                if (toInv != null && fromInv != toInv)
                {
                    if (toInv.GetName() == "default")
                    {
                        if (BagCapacityUtil.CanAddToBag(args, fd, ip))
                        {
                            ItemInventoryUtil.Move(fromInv, toInv, ip, x, y, args, (FreeRuleEventArgs)args, fromInv.GetInventoryUI(), toInv.GetInventoryUI());
                        }
                    }
                    else
                    {
                        ItemInventoryUtil.Move(fromInv, toInv, ip, x, y, args, (FreeRuleEventArgs)args, fromInv.GetInventoryUI(), toInv.GetInventoryUI());
                    }
                }
                else
                {
                    if (toInv == null)
                    {
                        //fromInv.GetInventoryUI().ReDraw(args, fromInv, true);
                    }
                    else
                    {
                        //fromInv.GetInventoryUI().ReDraw(args, fromInv, true);
                    }
                }
            }
        }

        public static void MoveItem(string key, FreeData fd, ISkillArgs args, int x, int y, int toGlobalX, int toGlobalY, int stageWidth, int stageHeigth, int fromGlobalX, int fromGlobalY)
        {
            toGlobalY = stageHeigth - toGlobalY;
            fromGlobalY = stageHeigth - fromGlobalY;
            ItemPosition ip = GetItemPosition(args, key, fd.freeInventory.GetInventoryManager());
            if (ip != null)
            {
                InventoryManager invManager = fd.freeInventory.GetInventoryManager();
                ItemInventory fromInv = ip.GetInventory();
                ItemInventory toInv = null;
                FreeUIUtil.Rectangle rec = null;
                foreach (string inv in invManager.GetInventoryNames())
                {
                    ItemInventory ii = invManager.GetInventory(inv);
                    if (ii != null && ii.IsOpen())
                    {
                        rec = ((SimpleInventoryUI)ii.GetInventoryUI()).GetItemRegion(args, stageWidth, stageHeigth);
                        if (rec.In(toGlobalX, toGlobalY))
                        {
                            toInv = ii;
                            break;
                        }
                    }
                }
                if (toInv == fromInv || toInv == null)
                {
                    ItemInventoryUtil.MovePosition(ip.GetInventory(), ip.GetInventory(), (SimpleInventoryUI)fromInv.GetInventoryUI(), (SimpleInventoryUI)fromInv.GetInventoryUI(), ip, x, y, args);
                }
                else
                {
                    FreeUIUtil.Rectangle itemRec = ((SimpleInventoryUI)fromInv.GetInventoryUI()).GetItemRegion(fromInv, args, stageWidth, stageHeigth, ip);
                    ItemInventoryUtil.MovePosition(ip.GetInventory(), invManager.GetInventory(toInv.GetName()), (SimpleInventoryUI)fromInv.GetInventoryUI(), (SimpleInventoryUI)toInv.GetInventoryUI(), ip, toGlobalX - rec.x - (fromGlobalX - itemRec.x), toGlobalY
                        - rec.y - (fromGlobalY - itemRec.y), args);
                }
            }
        }

        public static void UseItem(ItemPosition ip, FreeData fd, FreeRuleEventArgs fr)
        {
            if (ip != null)
            {
                fr.TempUse(PARA_ITEM, ip.GetKey());
                if (ip.GetKey().Effect(fr))
                {
                    if (!ip.GetKey().IsGoods())
                    {
                        if (ip.GetKey().IsUseClose())
                        {
                            CloseInventory(ip.GetInventory(), fr);
                        }
                        ip.GetInventory().UseItem(fr, ip);
                        if (ip.GetCount() <= 0 && ip.GetKey().IsConsume())
                        {
                            RemoveUI(ip, fr);
                        }
                    }
                }
                fr.Resume(PARA_ITEM);
                if (!ip.GetKey().IsGoods())
                {
                    fd.freeInventory.SetCurrentItem(ip, fr);
                }
                ip.GetKey().GetParameters().AddPara(new IntPara(PARA_ITEM_USE_TIME, fr.GameContext.session.currentTimeObject.CurrentTime));
            }
        }

        public static void UseItem(string key, FreeData fd, ISkillArgs args)
        {
            if(fd.Player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Climb)
            {
                return;
            }

            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            ItemPosition ip = GetItemPosition(args, key, fd.freeInventory.GetInventoryManager());

            if (ip != null && ip.key.IsConsume() && FreeItemConfig.GetSing(ip.key) > 0)
            {
                args.TempUse("current", fd);
                args.TempUse("item", ip);
                if (ip.key.CanUse(args))
                {
                    FreeItemConfig.UseAnimation(args.GameContext, fd, ip.key.GetKey());
                    fd.freeInventory.StartUseItem(args, fd, ip, FreeItemConfig.GetSing(ip.key), FreeItemConfig.GetSound(ip.key.GetKey()));
                }
                args.Resume("current");
                args.Resume("item");
            }
            else
            {
                UseItem(ip, fd, fr);
            }
        }

        private static void RemoveUI(ItemPosition ip, ISkillArgs args)
        {
            //FreeUIDeleteAction del = new FreeUIDeleteAction();
            //del.SetKey(ip.GetUIKey());
            //del.SetScope(1);
            //del.SetPlayer(PARA_PLAYER_CURRENT);
            //del.Act(args);
            //ip.GetInventory().GetInventoryUI().DeleteItem(args, ip.GetInventory(), ip);
        }

        public static void CloseInventory(ItemInventory inventory, ISkillArgs args)
        {
            if (((SimpleInventoryUI)inventory.GetInventoryUI()).alwaysShow)
            {
                return;
            }
            OpenInventoryAction oia = new OpenInventoryAction();
            oia.SetAlwaysClose(true);
            oia.SetPlayer(PARA_PLAYER_CURRENT);
            oia.SetName(inventory.GetName());
            oia.Act(args);
        }
    }
}
