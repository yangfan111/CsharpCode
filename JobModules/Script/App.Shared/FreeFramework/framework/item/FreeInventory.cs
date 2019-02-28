using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.data.sort;
using com.wd.free.item;
using com.wd.free.skill;
using Sharpen;
using com.wd.free.action;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.para.exp;
using com.wd.free.action.function;
using Free.framework;
using Core.Free;
using App.Shared;
using App.Shared.FreeFramework.Free;
using App.Shared.FreeFramework.Free.Chicken;
using App.Shared.Player;
using App.Server.GameModules.GamePlay.Free.player;

namespace com.wd.free.item
{
    public class FreeInventory
    {
        private InventoryManager inventoryManager;
        private ItemPosition currentItem;
        private MyDictionary<String, ItemPosition> lastUseItem;

        private PlayerActionSkill itemSkill;

        private bool startUse;

        public FreeInventory()
        {
            this.inventoryManager = new InventoryManager();
            this.lastUseItem = new MyDictionary<String, ItemPosition>();

            IniSkill();
        }

        public void StopUseItem(IEventArgs args, FreeData fd)
        {
            startUse = false;

            StartCounter(args, 0, fd, false);
        }

        public void UsingItem(ISkillArgs args)
        {
            if (startUse)
            {
                itemSkill.Frame(args);
            }
        }

        private void IniSkill()
        {
            this.itemSkill = new PlayerActionSkill();
            SkillTimeTrigger trigger = new SkillTimeTrigger();
            this.itemSkill.SetTrigger(trigger);
            SkillMultiInterrupter inter = new SkillMultiInterrupter();
            inter.inters = new List<ISkillInterrupter>();
            inter.inters.Add(new SkillKeyInterrupter("5,0,1,2,3,18,23"));

            SkillConditionInterrupter condition = new SkillConditionInterrupter();
            condition.condition = new ExpParaCondition("current.CurHp <= 0");
            inter.inters.Add(condition);

            inter.inters.Add(new PlayerStateInterrupter());

            trigger.SetInterrupter(inter);

            UsingItemAction interAction = new UsingItemAction(true);
            trigger.interAction = interAction;

            UsingItemAction action = new UsingItemAction(false);
            this.itemSkill.SetEffect(action);
        }

        public void StartUseItem(IEventArgs args, FreeData fd, ItemPosition ip, int sing, int sound)
        {
            PlayerStateUtil.RemoveGameState(EPlayerGameState.InterruptItem, fd.Player.gamePlay);
            fd.Player.autoMoveInterface.PlayerAutoMove.StopAutoMove();

            SkillTimeTrigger trigger = (SkillTimeTrigger)itemSkill.trigger;
            trigger.SetTime(sing * 1000);
            trigger.Reset();

            UsingItemAction interAction = (UsingItemAction)trigger.interAction;
            interAction.fd = fd;
            interAction.ip = ip;

            UsingItemAction action = (UsingItemAction)itemSkill.GetEffect();
            action.fd = fd;
            action.ip = ip;

            StartCounter(args, sing, fd, true);

            startUse = true;

            FreeSoundUtil.Stop("use", args, fd);

            if(sound > 0)
            {
                FreeSoundUtil.PlayOnce("use", sound, args, fd);
            }
        }

        private void StartCounter(IEventArgs args, int time, FreeData fd, bool start)
        {
            //UseCommonAction use = new UseCommonAction();
            //use.key = "startUseCounter";
            //use.values = new List<ArgValue>();
            //use.values.Add(new ArgValue("time", time.ToString()));
            //use.values.Add(new ArgValue("start", start.ToString().ToLower()));

            //args.TempUse("current", fd);
            //use.Act(args);
            //args.Resume("current");
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.CountDown;
            data.Bs.Add(start);
            data.Fs.Add(time);

            fd.Player.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, data);
        }


        public InventoryManager GetInventoryManager()
        {
            return inventoryManager;
        }

        public ItemPosition GetCurrentItem()
        {
            return currentItem;
        }

        public ItemPosition GetItemPosition(FreeItem item)
        {
            foreach (string invName in inventoryManager.GetInventoryNames())
            {
                ItemInventory ii = inventoryManager.GetInventory(invName);
                foreach (ItemPosition ip in ii.GetItems())
                {
                    if (ip.GetKey().GetId() == item.GetId())
                    {
                        return ip;
                    }
                }
            }

            return null;
        }

        public ItemPosition GetCurrentItem(String cat)
        {
            return lastUseItem.Get(cat);
        }

        public ItemPosition GetItemById(int id)
        {
            foreach (string inv in inventoryManager.GetInventoryNames())
            {
                ItemInventory ii = inventoryManager.GetInventory(inv);
                foreach (ItemPosition ip in ii.GetItems())
                {
                    if (ip.GetKey().GetId() == id)
                    {
                        return ip;
                    }
                }
            }

            return null;
        }

        public ItemPosition[] Select(SelectMethod sm)
        {

            List<ItemPosition> items = new List<ItemPosition>();

            DataBlock bl = new DataBlock();
            foreach (string inv in inventoryManager.GetInventoryNames())
            {
                ItemInventory ii = inventoryManager.GetInventory(inv);
                foreach (ItemPosition ip in ii.GetItems())
                {
                    bl.AddData(ip);
                }
            }

            if (sm != null)
            {
                foreach (IFeaturable fe in sm.Select(bl).GetAllDatas())
                {
                    items.Add((ItemPosition)fe);
                }
            }
            else
            {
                foreach (IFeaturable fe in bl.GetDataList())
                {
                    items.Add((ItemPosition)fe);
                }
            }

            return items.ToArray();

        }

        public void SetCurrentItem(ItemPosition currentItem, ISkillArgs args)
        {
            this.currentItem = currentItem;
            String cat = currentItem.GetKey().Cat;
            if (cat != null)
            {
                this.lastUseItem.Add(cat, currentItem);
            }
        }
    }

    class UsingItemAction : AbstractGameAction
    {
        public ItemPosition ip;
        public FreeData fd;
        private bool inter;

        public UsingItemAction(bool inter)
        {
            this.inter = inter;
        }

        public override void DoAction(IEventArgs args)
        {
            if (inter)
            {
                if (fd != null)
                {
                    fd.freeInventory.StopUseItem(args, fd);
                    UseCommonAction use = new UseCommonAction();
                    use.key = "showBottomTip";
                    use.values = new List<ArgValue>();
                    use.values.Add(new ArgValue("msg", "{desc:10072,{item.name}}"));

                    args.TempUse("current", fd);
                    args.TempUse("item", ip);
                    use.Act(args);
                    args.Resume("current");
                    args.Resume("item");
                    PlayerAnimationAction.DoAnimation(args.GameContext, PlayerAnimationAction.Stop, fd.Player);
                    FreeSoundUtil.Stop("use", args, fd);

                    PlayerStateUtil.RemoveGameState(EPlayerGameState.InterruptItem, fd.Player.gamePlay);
                }
            }
            else
            {
                if (ip != null && fd != null)
                {
                    UseItem(ip, fd, (ISkillArgs)args);
                    fd.freeInventory.StopUseItem(args, fd);
                }
            }
        }

        private void UseItem(ItemPosition ip, FreeData fd, ISkillArgs fr)
        {
            if (ip != null)
            {
                fr.TempUse("item", ip.GetKey());
                if (ip.GetKey().Effect(fr))
                {
                    ip.GetInventory().UseItem(fr, ip);
                }
                fr.Resume("item");
                if (!ip.GetKey().IsGoods())
                {
                    fd.freeInventory.SetCurrentItem(ip, fr);
                }
            }
        }
    }
}
