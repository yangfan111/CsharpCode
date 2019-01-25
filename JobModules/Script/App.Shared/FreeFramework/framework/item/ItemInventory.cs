using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.util;
using com.wd.free.para;
using System;

namespace com.wd.free.item
{
    public class ItemInventory
    {
        [System.NonSerialized]
        public string name;

        internal int column;

        internal int row;

        internal bool open;

        internal string canDrop;

        [System.NonSerialized]
        internal bool[][] ins;

        [System.NonSerialized]
        internal IParaCondition canDropCondition;

        [System.NonSerialized]
        public IList<ItemPosition> posList;

        private IInventoryUI inventoryUI;

        private IGameAction closeAction;

        private IGameAction openAction;

        private IGameAction clickAction;

        private IGameAction dropAction;

        public ItemInventory(int column, int row)
        {
            this.column = column;
            this.row = row;
            ins = Arrays.Create<bool>(row, column);
            posList = new List<ItemPosition>();
        }

        public virtual void UseItem(ISkillArgs args, ItemPosition ip)
        {
            if (ip.key.IsUseAll())
            {
                UseItem(args, ip, ip.count);
            }
            else
            {
                UseItem(args, ip, 1);
            }
        }

        public virtual void UseItem(ISkillArgs args, ItemPosition ipos, int count)
        {
            ItemPosition remove = null;
            ItemPosition useIp = null;
            foreach (ItemPosition ip in posList)
            {
                if (ip.GetX() == ipos.x && ip.GetY() == ipos.y)
                {
                    useIp = ip;
                    if (ip.GetKey().IsConsume())
                    {
                        ip.count = ip.count - count;
                    }
                    if (ip.count <= 0 && ipos.key.IsConsume())
                    {
                        remove = ip;
                    }
                }
            }
            if (remove != null)
            {
                RemoveItemPosition(remove);
            }
            if (inventoryUI != null && ipos.key.IsConsume())
            {
                if (remove != null)
                {
                    inventoryUI.DeleteItem(args, this, remove);
                }
                else
                {
                    if (useIp != null)
                    {
                        inventoryUI.UpdateItem(args, this, useIp);
                    }
                }
            }
            if (inventoryUI != null && useIp != null)
            {
                inventoryUI.UseItem(args, this, useIp);
            }
        }

        public virtual ItemPosition GetItem(string key)
        {
            foreach (ItemPosition ip in posList)
            {
                if (ip.key.GetKey().Equals(key))
                {
                    return ip;
                }
            }
            return null;
        }

        public virtual ItemPosition[] GetItem(int x, int y, int w, int h)
        {
            IList<ItemPosition> items = new List<ItemPosition>();
            foreach (ItemPosition ip in posList)
            {
                if ((ip.x >= x && ip.x < x + w || x >= ip.x && x < ip.x + ip.GetKey().GetGridWidth()) && (ip.y >= y && ip.y < y + h || y >= ip.y && y < ip.y + ip.GetKey().GetGridHeight()))
                {
                    items.Add(ip);
                }
            }
            return Sharpen.Collections.ToArray(items, new ItemPosition[0]);
        }

        public virtual ItemPosition GetItem(int x, int y)
        {
            foreach (ItemPosition ip in posList)
            {
                if (ip.GetX() == x && ip.GetY() == y)
                {
                    return ip;
                }
            }
            return null;
        }

        private void RemoveItemPosition(ItemPosition ip)
        {
            posList.Remove(ip);
            ip.Remove();
        }

        public virtual void ChangePosition(ItemPosition ip, com.wd.free.item.ItemInventory toIn, int toX, int toY)
        {
            for (int i = ip.GetX(); i < ip.GetX() + ip.GetKey().GetGridWidth(); i++)
            {
                for (int j = ip.GetY(); j < ip.GetY() + ip.GetKey().GetGridHeight(); j++)
                {
                    ins[j][i] = false;
                }
            }
            for (int i_1 = toX; i_1 < toX + ip.GetKey().GetGridWidth(); i_1++)
            {
                for (int j = toY; j < toY + ip.GetKey().GetGridHeight(); j++)
                {
                    toIn.ins[j][i_1] = true;
                }
            }
            // System.err.println(ip.getKey().getKey() + " " + ip.getX() + ","
            // + ip.getY() + "->" + toX + "," + toY);
            ip.SetX(toX);
            ip.SetY(toY);
            if (ip.inventory != toIn)
            {
                posList.Remove(ip);
                toIn.posList.Add(ip);
            }
            ip.inventory = toIn;
        }

        private bool IsEmpty(com.wd.free.item.ItemInventory toIn, int x, int y, int w, int h)
        {
            for (int i = x; i < x + w; i++)
            {
                for (int j = y; j < y + h; j++)
                {
                    if (i >= toIn.column || j >= toIn.row || toIn.ins[j][i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>清空两边的格子，先尝试把p1放入p2起始位置,如果可以，则先占有此格子，再判断p2是否可以放入p1开始位置</summary>
        /// <param name="ipFrom"/>
        /// <param name="ipTo"/>
        /// <returns/>
        public virtual bool CanExchange(ItemPosition ipFrom, ItemPosition ipTo)
        {
            bool can = false;
            TempSetPosition(ipFrom, false);
            TempSetPosition(ipTo, false);
            if (IsEmpty(ipFrom.inventory, ipFrom.GetX(), ipFrom.GetY(), ipTo.GetKey().GetGridWidth(), ipTo.GetKey().GetGridHeight()))
            {
                TempSetPosition(ipFrom.inventory, ipFrom.GetX(), ipFrom.GetY(), ipTo.GetKey().GetGridWidth(), ipTo.GetKey().GetGridHeight(), true);
                if (IsEmpty(ipTo.inventory, ipTo.GetX(), ipTo.GetY(), ipFrom.GetKey().GetGridWidth(), ipFrom.GetKey().GetGridHeight()))
                {
                    can = true;
                }
                TempSetPosition(ipFrom.inventory, ipFrom.GetX(), ipFrom.GetY(), ipTo.GetKey().GetGridWidth(), ipTo.GetKey().GetGridHeight(), false);
            }
            TempSetPosition(ipFrom, true);
            TempSetPosition(ipTo, true);
            return can;
        }

        private void TempSetPosition(com.wd.free.item.ItemInventory inventory, int x, int y, int w, int h, bool value)
        {
            for (int i = x; i < x + w; i++)
            {
                for (int j = y; j < y + h; j++)
                {
                    inventory.ins[j][i] = value;
                }
            }
        }

        private void TempSetPosition(ItemPosition ip, bool value)
        {
            for (int i = ip.GetX(); i < ip.GetX() + ip.GetKey().GetGridWidth(); i++)
            {
                for (int j = ip.GetY(); j < ip.GetY() + ip.GetKey().GetGridHeight(); j++)
                {
                    ip.inventory.ins[j][i] = value;
                }
            }
        }

        public virtual bool CanMoveTo(ItemPosition ip, com.wd.free.item.ItemInventory toIn, int toX, int toY)
        {
            bool can = true;
            TempSetPosition(ip, false);
            can = IsEmpty(toIn, toX, toY, ip.GetKey().GetGridWidth(), ip.GetKey().GetGridHeight());
            TempSetPosition(ip, true);
            return can;
        }

        public virtual void ExchangeItemPosition(ItemPosition p1, ItemPosition p2)
        {
            for (int i = p1.GetX(); i < p1.GetX() + p1.GetKey().GetGridWidth(); i++)
            {
                for (int j = p1.GetY(); j < p1.GetY() + p1.GetKey().GetGridHeight(); j++)
                {
                    p1.inventory.ins[j][i] = false;
                }
            }
            for (int i_1 = p2.GetX(); i_1 < p2.GetX() + p2.GetKey().GetGridWidth(); i_1++)
            {
                for (int j = p2.GetY(); j < p2.GetY() + p2.GetKey().GetGridHeight(); j++)
                {
                    p2.inventory.ins[j][i_1] = false;
                }
            }
            for (int i_2 = p1.GetX(); i_2 < p1.GetX() + p2.GetKey().GetGridWidth(); i_2++)
            {
                for (int j = p1.GetY(); j < p1.GetY() + p2.GetKey().GetGridHeight(); j++)
                {
                    p1.inventory.ins[j][i_2] = true;
                }
            }
            for (int i_3 = p2.GetX(); i_3 < p2.GetX() + p1.GetKey().GetGridWidth(); i_3++)
            {
                for (int j = p2.GetY(); j < p2.GetY() + p1.GetKey().GetGridHeight(); j++)
                {
                    p2.inventory.ins[j][i_3] = true;
                }
            }
            int oldX = p1.GetX();
            int oldY = p1.GetY();
            p1.SetX(p2.GetX());
            p1.SetY(p2.GetY());
            p2.SetX(oldX);
            p2.SetY(oldY);
            if (p1.inventory != p2.inventory)
            {
                p1.inventory.posList.Remove(p1);
                p2.inventory.posList.Remove(p2);
                p1.inventory.posList.Add(p2);
                p2.inventory.posList.Add(p1);
                com.wd.free.item.ItemInventory temp = p1.inventory;
                p1.inventory = p2.inventory;
                p2.inventory = temp;
            }
        }

        // System.err.println(p1.getKey().getKey() + " " + p2.getX() + ","
        // + p2.getY() + "->" + p1.getX() + "," + p1.getY());
        // System.err.println(p2.getKey().getKey() + " " + p1.getX() + ","
        // + p1.getY() + "->" + p2.getX() + "," + p2.getY());
        public virtual void ReDraw(ISkillArgs args)
        {
            if (inventoryUI != null)
            {
                inventoryUI.ReDraw(args, this, true);
            }
        }

        public virtual void RemoveItem(ISkillArgs args, ItemPosition ip)
        {
            ip.key.Removed(args);
            if (inventoryUI != null && inventoryUI.MoveAction != null)
            {
                args.TempUsePara(new StringPara("from", this.name));
                args.TempUse("item", ip);

                inventoryUI.MoveAction.Act(args);

                args.ResumePara("from");
                args.Resume("item");
            }
            RemoveItemPosition(ip);
            if (inventoryUI != null)
            {
                inventoryUI.DeleteItem(args, this, ip);
            }
        }

        public int[] GetNextEmptyPosition(FreeItem item)
        {
            for (int i = 0; i <= row - item.GetGridHeight(); i++)
            {
                for (int j = 0; j <= column - item.GetGridWidth(); j++)
                {
                    bool filled = false;
                    for (int ii = i; ii < i + item.GetGridHeight(); ii++)
                    {
                        for (int jj = j; jj < j + item.GetGridWidth(); jj++)
                        {
                            if (ins[ii][jj])
                            {
                                filled = true;
                            }
                        }
                    }
                    if (!filled)
                    {
                        return new int[] { j, i };
                    }
                }
            }

            return new int[] { 0, 0 };
        }

        public virtual bool AddItem(ISkillArgs args, FreeItem item, bool useMove)
        {
            bool existed = (GetExisted(item) != null);
            if (existed)
            {
                if (item.IsUnique())
                {
                    if (inventoryUI != null)
                    {
                        inventoryUI.Error(args, this, "已存在唯一物品'" + item.GetName() + "'");
                    }
                    return false;
                }
            }
            ItemPosition old = GetNotFullExisted(item);
            if (old != null)
            {
                int addCount = item.GetCount();
                old.SetKey(item);
                int removed = Math.Min(item.GetItemStack() - old.GetCount(), item.GetCount());
                old.SetCount(old.GetCount() + removed);
                if (inventoryUI != null && args != null)
                {
                    inventoryUI.UpdateItem(args, this, old);
                }

                if (removed < addCount)
                {
                    FreeItem clone = item.Clone();
                    clone.SetCount(addCount - removed);
                    AddNewItem(args, clone, false);
                }

                args.TempUsePara(new StringPara("inventory", name));
                item.Added(args);
                if (inventoryUI != null && inventoryUI.MoveAction != null && useMove)
                {
                    args.TempUsePara(new StringPara("to", this.name));
                    inventoryUI.MoveAction.Act(args);
                    args.ResumePara("to");
                }
                args.ResumePara("inventory");



                return true;
            }
            else
            {
                return AddNewItem(args, item, useMove);   
            }
        }

        private bool AddNewItem(ISkillArgs args, FreeItem item, bool useMove)
        {
            for (int i = 0; i <= row - item.GetGridHeight(); i++)
            {
                for (int j = 0; j <= column - item.GetGridWidth(); j++)
                {
                    bool filled = false;
                    for (int ii = i; ii < i + item.GetGridHeight(); ii++)
                    {
                        for (int jj = j; jj < j + item.GetGridWidth(); jj++)
                        {
                            if (ins[ii][jj])
                            {
                                filled = true;
                            }
                        }
                    }
                    if (!filled)
                    {
                        ItemPosition ip = new ItemPosition(item, j, i);
                        ip.inventory = this;
                        posList.Add(ip);
                        for (int ii_1 = i; ii_1 < i + item.GetGridHeight(); ii_1++)
                        {
                            for (int jj = j; jj < j + item.GetGridWidth(); jj++)
                            {
                                ins[ii_1][jj] = true;
                            }
                        }
                        if (inventoryUI != null && args != null)
                        {
                            inventoryUI.AddItem(args, this, ip);
                        }
                        //if ("default".Equals(name))
                        //{
                        args.TempUsePara(new StringPara("inventory", name));
                        item.Added(args);
                        if (inventoryUI != null && inventoryUI.MoveAction != null && useMove)
                        {
                            args.TempUsePara(new StringPara("to", this.name));
                            args.TempUse("item", ip);

                            inventoryUI.MoveAction.Act(args);

                            args.Resume("item");
                            args.ResumePara("to");
                        }
                        args.ResumePara("inventory");
                        //}
                        return true;
                    }
                }
            }
            if (inventoryUI != null)
            {
                inventoryUI.Error(args, this, "物品栏已满");
            }
            return false;
        }

        private ItemPosition GetExisted(FreeItem item)
        {
            foreach (ItemPosition ip in posList)
            {
                if (ip.key.GetKey().Equals(item.GetKey()))
                {
                    return ip;
                }
            }
            return null;
        }

        private ItemPosition GetNotFullExisted(FreeItem item)
        {
            foreach (ItemPosition ip in posList)
            {
                if (ip.key.GetKey().Equals(item.GetKey()))
                {
                    if (ip.GetCount() < item.GetItemStack())
                    {
                        return ip;
                    }
                }
            }
            return null;
        }

        public virtual void Clear()
        {
            this.ins = Arrays.Create<bool>(row, column);
            this.posList = new List<ItemPosition>();
        }

        public virtual void Resort(ISkillArgs args, string order)
        {
            ItemPosition[] ips = GetItems();
            this.Clear();
            ItemPosition[] newOrder = ips;
            if (!StringUtil.IsNullOrEmpty(order))
            {
                IList<ItemPosition> ipList = new List<ItemPosition>();
                DataBlock bl = new DataBlock();
                foreach (ItemPosition ip in ips)
                {
                    bl.AddData(ip);
                }
                IList<IFeaturable> list = new SelectMethod(order).Select(bl).GetAllDatas();
                foreach (IFeaturable fe in list)
                {
                    ipList.Add((ItemPosition)fe);
                }
                newOrder = Sharpen.Collections.ToArray(ipList, new ItemPosition[0]);
            }
            bool canReorder = true;
            foreach (ItemPosition ip_1 in newOrder)
            {
                if (inventoryUI != null)
                {
                    inventoryUI.DeleteItem(args, this, ip_1);
                }
                ip_1.key.SetCount(ip_1.count);
                if (!AddItem(null, ip_1.key, false))
                {
                    canReorder = false;
                }
            }
            if (!canReorder)
            {
                this.Clear();
                foreach (ItemPosition ip in ips)
                {
                    AddItem(null, ip.key,false);
                }
            }
            if (inventoryUI != null)
            {
                inventoryUI.ReDraw(args, this, false);
            }
        }

        public virtual ItemPosition[] GetItems()
        {
            return Sharpen.Collections.ToArray(posList, new ItemPosition[0]);
        }

        public virtual int GetColumn()
        {
            return column;
        }

        public virtual void SetColumn(int column)
        {
            this.column = column;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual int GetRow()
        {
            return row;
        }

        public virtual void SetRow(int row)
        {
            this.row = row;
        }

        public virtual IInventoryUI GetInventoryUI()
        {
            return inventoryUI;
        }

        public virtual void SetInventoryUI(IInventoryUI redraw)
        {
            this.inventoryUI = redraw;
        }

        public virtual bool IsOpen()
        {
            return open;
        }

        public virtual void SetOpen(bool open)
        {
            this.open = open;
        }

        public virtual string GetUIKey()
        {
            return "inventory_" + name + "_ui";
        }

        public virtual IGameAction GetCloseAction()
        {
            return closeAction;
        }

        public virtual void SetCloseAction(IGameAction closeAction)
        {
            this.closeAction = closeAction;
        }

        public virtual IGameAction GetOpenAction()
        {
            return openAction;
        }

        public virtual void SetOpenAction(IGameAction openAction)
        {
            this.openAction = openAction;
        }

        public virtual IGameAction GetClickAction()
        {
            return clickAction;
        }

        public virtual void SetClickAction(IGameAction clickAction)
        {
            this.clickAction = clickAction;
        }

        public virtual bool IsCanDrop(ItemPosition ip, IEventArgs args)
        {
            if (StringUtil.IsNullOrEmpty(canDrop))
            {
                return true;
            }
            if (canDropCondition == null || (canDrop.Contains(FreeUtil.VAR_START) && canDrop.Contains(FreeUtil.VAR_END)))
            {
                canDropCondition = new ExpParaCondition(canDrop);
            }
            args.TempUse("item", ip);
            bool can = canDropCondition.Meet(args);
            args.Resume("item");
            return can;
        }

        public virtual void SetCanDrop(string canDrop)
        {
            this.canDrop = canDrop;
        }

        public virtual IGameAction GetDropAction()
        {
            return dropAction;
        }

        public virtual void SetDropAction(IGameAction dropAction)
        {
            this.dropAction = dropAction;
        }
    }
}
