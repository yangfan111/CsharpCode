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
using gameplay.gamerule.free.ui.component;
using Free.framework;
using Core.Free;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Server.GameModules.GamePlay.free.client;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class UnityInventoryUi : IInventoryUI
    {
        private IGameAction errorAction;

        private IGameAction moveOutAction;

        private IGameAction canNotMoveAction;

        private IGameAction moveAction;

        private string parent;

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

        public void ReDraw(ISkillArgs args, ItemInventory inventory, bool includeBack)
        {
            SimpleProto sp = FreePool.Allocate();
            sp.Key = FreeMessageConstant.InventoyUI;
            // 0 重绘，1 添加，2 删除, 3 更新
            sp.Ks.Add(0);

            sp.Ins.Add(inventory.posList.Count);

            sp.Ss.Add(parent);

            foreach (ItemPosition ip in inventory.posList)
            {
                sp.Ss.Add(ip.GetKey().GetKey() + "_itemUI");

                FreeItemInfo info  = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());

                sp.Ins.Add(info.cat);
                sp.Ins.Add(info.id);

                sp.Ss.Add(inventory.GetName() + "," + ip.GetX() + "," + ip.GetY());

                if (ip.GetKey().GetItemStack() > 1)
                {
                    sp.Ss.Add(ip.GetCount().ToString());
                }
                else
                {
                    sp.Ss.Add("");
                }
            }

            FreeData fd = (FreeData)args.GetUnit("current");
            sp.Ks.Add((int)BagCapacityUtil.GetCapacity(fd));
            sp.Ks.Add((int)Math.Ceiling(BagCapacityUtil.GetWeight(fd)));

            SendMessageAction.sender.SendMessage(args, sp, 1, "current");

            ChickenFuncUtil.UpdateBagCapacity(args, (int)Math.Ceiling(BagCapacityUtil.GetWeight(fd)), BagCapacityUtil.GetCapacity(fd));
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
