using System;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using com.wd.free.util;
using gameplay.gamerule.free.ui;
using App.Server.GameModules.GamePlay.Free.item;

namespace gameplay.gamerule.free.item
{
    [Serializable]
    public class SimpleInventoryUI : IInventoryUI
    {
        internal string width;

        internal string height;

        internal int itemWidth;

        internal int itemHeight;

        internal string relative;

        internal string x;

        internal string y;

        internal bool nomouse;

        internal bool alwaysShow;

        internal bool itemFixed;

        internal SimpleBackUI backUI;

        internal IItemUI itemUI;

        internal IHotKey hotkey;

        internal IGameAction errorAction;

        internal IGameAction moveOutAction;

        internal IGameAction canNotMoveAction;

        internal IGameAction moveAction;

        [System.NonSerialized]
        private FreeUIDeleteAction delete;

        [System.NonSerialized]
        private int iX;

        [System.NonSerialized]
        private int iY;

        [System.NonSerialized]
        private int iWidth;

        [System.NonSerialized]
        private int iHeight;

        [System.NonSerialized]
        private long lastErrorTime;

        private void Initial(IEventArgs args)
        {
            if (iWidth == 0)
            {
                iX = FreeUtil.ReplaceInt(x, args);
                iY = FreeUtil.ReplaceInt(y, args);
                iWidth = FreeUtil.ReplaceInt(width, args);
                iHeight = FreeUtil.ReplaceInt(height, args);
            }
        }

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

        public virtual int GetX(IEventArgs args)
        {
            Initial(args);
            return iX;
        }

        public virtual int GetY(IEventArgs args)
        {
            Initial(args);
            return iY;
        }

        public virtual int GetWidth(IEventArgs args)
        {
            Initial(args);
            return iWidth;
        }

        public virtual int GetHeight(IEventArgs args)
        {
            Initial(args);
            return iHeight;
        }

        public virtual FreeUIUtil.Rectangle GetItemRegion(ItemInventory inventory, IEventArgs args, int screenW, int screenH, ItemPosition ip)
        {
            Initial(args);
            FreeUIUtil.Rectangle rec = GetItemRegion(args, screenW, screenH);
            rec.x = rec.x + ip.GetX() * itemWidth + (int)MyMath.Max(0, (ip.GetX() - 1)) * GetWidthMargin(inventory, args);
            rec.y = rec.y + ip.GetY() * itemHeight + (int)MyMath.Max(0, (ip.GetY() - 1)) * GetHeightMargin(inventory, args);
            return rec;
        }

        public virtual FreeUIUtil.Rectangle GetItemRegion(IEventArgs args, int screenW, int screenH)
        {
            Initial(args);
            FreeUIUtil.Rectangle rec = FreeUIUtil.GetXY(0, 0, screenW, screenH, iX, iY, FreeUtil.ReplaceInt(relative, args));
            rec.width = iWidth;
            rec.height = iHeight;
            return rec;
        }

        public virtual void ReDraw(ISkillArgs args, ItemInventory inventory, bool includeBack)
        {
            if (includeBack)
            {
                backUI.Draw(args, inventory);
            }
            foreach (ItemPosition ip in inventory.GetItems())
            {
                itemUI.Draw(this, args, inventory, ip);
            }
        }

        public virtual void UseItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            string cat = ((FreeGameItem)ip.GetKey()).GetCat();
            foreach (ItemPosition i in inventory.GetItems())
            {
                if (((FreeGameItem)i.GetKey()).GetCat().Equals(cat))
                {
                    i.SetUsed(false);
                    UpdateItem(args, inventory, i);
                }
            }
            ip.SetUsed(true);
            UpdateItem(args, inventory, ip);
        }

        public virtual void Error(ISkillArgs args, ItemInventory inventory, string msg)
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

        public virtual void DeleteItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            if (delete == null)
            {
                delete = new FreeUIDeleteAction();
            }
            delete.SetKey(ip.GetUIKey());
            delete.SetScope(1);
            delete.SetPlayer("current");
            delete.Act(args);
        }

        public virtual void AddItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            itemUI.Draw(this, args, inventory, ip);
        }

        public virtual void UpdateItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            itemUI.Draw(this, args, inventory, ip);
        }

        public virtual void MoveItemPosition(IInventoryUI toInventory, int x, int y, ISkillArgs args)
        {
        }

        internal virtual int GetWidthMargin(ItemInventory inventory, IEventArgs args)
        {
            Initial(args);
            if (inventory.GetColumn() == 1)
            {
                return iWidth - itemWidth;
            }
            return (iWidth - itemWidth * inventory.GetColumn()) / (inventory.GetColumn() - 1);
        }

        internal virtual int GetHeightMargin(ItemInventory inventory, IEventArgs args)
        {
            Initial(args);
            if (inventory.GetRow() == 1)
            {
                return iHeight - itemHeight;
            }
            else
            {
                return (iHeight - itemHeight * inventory.GetRow()) / (inventory.GetRow() - 1);
            }
        }
    }
}
