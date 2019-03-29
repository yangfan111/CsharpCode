using com.wd.free.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.skill;
using com.wd.free.action;
using Sharpen;
using com.wd.free.para;
using gameplay.gamerule.free.ui;
using com.wd.free.util;
using gameplay.gamerule.free.ui.component;
using App.Server.GameModules.GamePlay.Free.ui;
using WeaponConfigNs;
using Core.Configuration;
using com.cpkf.yyjd.tools.util;
using UnityEngine;
using App.Server.GameModules.GamePlay.free.player;
using Utils.Configuration;
using Assets.Utils.Configuration;
using App.Server.GameModules.GamePlay.Free.weapon;
using Free.framework;
using Core.Free;
using App.Server.GameModules.GamePlay.Free.item.config;
using Assets.App.Server.GameModules.GamePlay.Free;
using App.Server.GameModules.GamePlay.Free.chicken;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class UnityOneInventoryUi : IInventoryUI
    {
        private IGameAction errorAction;

        private IGameAction moveOutAction;

        private IGameAction canNotMoveAction;

        private IGameAction moveAction;

        private string uiKey;
        private string image;
        private string name;
        private string count;

        [System.NonSerialized]
        private long lastErrorTime;

        public IGameAction MoveAction
        {
            get { return moveAction; }
        }

        public IGameAction CanNotMoveAction
        {
            get { return canNotMoveAction; }
        }

        public IGameAction MoveOutAction
        {
            get { return MoveOutAction; }
        }

        public IGameAction ErrorAction
        {
            get { return errorAction; }
        }

        public void AddItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            ReDraw(args, inventory, true);
        }

        public void DeleteItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            ReDraw(args, inventory, true);
        }

        public void Error(ISkillArgs args, ItemInventory inventory, string msg)
        {
            if (errorAction != null)
            {
                if (Runtime.CurrentTimeMillis() - lastErrorTime > 2000)
                {
                    if (args != null)
                    {
                        args.GetDefault().GetParameters().TempUse(new StringPara("message", msg));
                        errorAction.Act(args);
                        args.GetDefault().GetParameters().Resume("message");
                    }
                    lastErrorTime = Runtime.CurrentTimeMillis();
                }
            }
        }

        private FreeUIUpdateAction update;

        public void ReDraw(ISkillArgs args, ItemInventory inventory, bool includeBack)
        {
            if (update == null)
            {
                update = new FreeUIUpdateAction();
            }

            update.SetKey(FreeUtil.ReplaceVar(uiKey, args));
            update.ClearValue();

            FreeData fd = (FreeData)args.GetUnit("current");

            FreePrefabValue v = new FreePrefabValue();
            v.SetSeq("1");
            update.AddValue(v);
            v.AddOneValue(new OnePrefabValue(image, ""));
            if (!string.IsNullOrEmpty(name))
            {
                v.AddOneValue(new OnePrefabValue(name, ""));
            }
            if (!string.IsNullOrEmpty(count))
            {
                v.AddOneValue(new OnePrefabValue(count, ""));
            }
            update.SetScope(1);
            update.SetPlayer("current");
            update.Act(args);

            if (inventory.posList.Count > 0)
            {
                ItemPosition ip = inventory.posList[0];
                v.Clear();
                v.AddOneValue(new OnePrefabValue(image, ip.key.GetImg()));
                if (!string.IsNullOrEmpty(name))
                {
                    v.AddOneValue(new OnePrefabValue(name, ip.key.GetName()));
                }
                if (!string.IsNullOrEmpty(count))
                {
                    if(ip.count != 1)
                    {
                        v.AddOneValue(new OnePrefabValue(count, ip.GetCount().ToString()));
                    }
                }
                else
                {
                    v.AddOneValue(new OnePrefabValue(count, ""));
                }
                update.Act(args);

                redrawPart(inventory, args, ip, fd);

                SimpleProto itemInfo = FreePool.Allocate();
                itemInfo.Key = FreeMessageConstant.ItemInfo;
                itemInfo.Ss.Add(inventory.name);
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                itemInfo.Ins.Add(info.cat);
                itemInfo.Ins.Add(info.id);
                itemInfo.Ins.Add(ip.GetCount());
                FreeMessageSender.SendMessage(args, "current", itemInfo);
            }
            else
            {
                clearPart(inventory, args, fd);
            }
        }

        private void clearPart(ItemInventory inventory, ISkillArgs args, FreeData fd)
        {
            if (inventory.name == "w1" || inventory.name == "w2" || inventory.name == "w3")
            {
                ShowPartAction show = new ShowPartAction();
                show.show = false;
                show.SetPlayer("current");
                show.SetScope(1);

                if (inventory.name == "w1")
                {
                    show.weaponKey = "1";
                    ClearPart(args, fd, 1);
                }
                else if (inventory.name == "w2")
                {
                    show.weaponKey = "2";
                    ClearPart(args, fd, 2);
                }
                else if (inventory.name == "w3")
                {
                    show.weaponKey = "3";
                    ClearPart(args, fd, 3);
                }
                show.parts = "1,2,3,4,5";
                show.Act(args);
            }
        }

        private void redrawPart(ItemInventory inventory, ISkillArgs args, ItemPosition ip, FreeData fd)
        {
            if (inventory.name == "w1" || inventory.name == "w2" || inventory.name == "w3")
            {
                int id = (int)((IntPara)ip.GetParameters().Get("itemId")).GetValue();

                WeaponResConfigItem config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(id);
                if (config != null)
                {
                    HashSet<int> list = new HashSet<int>();

                    foreach (XmlConfig.EWeaponPartType part in SingletonManager.Get<WeaponPartsConfigManager>().GetAvaliablePartTypes(id))
                    {
                        list.Add(FreeWeaponUtil.GetWeaponPart(part));
                    }

                    List<string> showP = new List<string>();
                    List<string> hideP = new List<string>();

                    for (int i = 1; i <= 5; i++)
                    {
                        if (list.Contains(i))
                        {
                            showP.Add(i.ToString());
                        }
                        else
                        {
                            hideP.Add(i.ToString());
                        }
                    }

                    ShowPartAction show = new ShowPartAction();
                    show.show = true;
                    show.SetPlayer("current");
                    show.SetScope(1);
                    if (inventory.name == "w1")
                    {
                        show.weaponKey = "1";
                    }
                    else if (inventory.name == "w2")
                    {
                        show.weaponKey = "2";
                    }
                    else
                    {
                        show.weaponKey = "3";
                    }

                    show.parts = StringUtil.GetStringFromStrings(showP, ",");
                    show.Act(args);

                    show.show = false;
                    show.parts = StringUtil.GetStringFromStrings(hideP, ",");
                    show.Act(args);
                }
                else
                {
                    Debug.LogError(ip.key.GetName() + " 没有定义配件.");
                }
            }
        }

        private void ClearPart(ISkillArgs fr, FreeData fd, int key)
        {
            for (int i = 1; i <= 5; i++)
            {
                ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory("w" + key + "" + i);
                if (ii != null)
                {
                    ii.Clear();
                    ii.GetInventoryUI().ReDraw(fr, ii, true);
                }
            }
        }

        public void UpdateItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            ReDraw(args, inventory, true);
        }

        public void UseItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            ReDraw(args, inventory, true);
        }
    }
}
