using System;
using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.util;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;
using App.Server.GameModules.GamePlay.Free.item;

namespace gameplay.gamerule.free.item
{
    [Serializable]
    public class SimpleItemUI : IItemUI
    {
        [System.NonSerialized]
        private FreeUICreateAction fui;

        internal IList<IFreeComponent> components;

        internal int back;

        internal int item;

        internal int count;

        internal int hotkey;

        internal string used;

        internal string notused;

        [System.NonSerialized]
        private ItemInventory inventory;

        public SimpleItemUI()
            : base()
        {
        }

        public virtual void Draw(SimpleInventoryUI ui, IEventArgs args, ItemInventory inventory, ItemPosition ip)
        {
            this.inventory = inventory;
            if (fui == null)
            {
                fui = new FreeUICreateAction();
                Sharpen.Collections.AddAll(fui.GetComponents(), components);
            }
            ip.GetParameters().AddPara(new StringPara("inventory", inventory.GetName()));
            StringPara img = new StringPara("img", string.Empty);
            StringPara itemName = new StringPara("name", string.Empty);
            IntPara count = new IntPara("count", 0);
            args.GetDefault().GetParameters().TempUse(img);
            args.GetDefault().GetParameters().TempUse(count);
            args.GetDefault().GetParameters().TempUse(itemName);

            img.SetValue(ip.GetKey().GetImg());
            count.SetValue(ip.GetCount());
            itemName.SetValue(ip.GetKey().GetName());
            FreeImageComponet back = GetBackground();
            if (ui.nomouse)
            {
                back.SetNoMouse("true");
            }
            if (ui.itemFixed)
            {
                back.SetFixed("true");
            }
            int startX = GetX(args, ui, ip, back);
            int startY = GetY(args, ui, ip, back);
            if (!StringUtil.IsNullOrEmpty(notused) && !StringUtil.IsNullOrEmpty(used))
            {
                if (ip.IsUsed())
                {
                    back.SetUrl(used);
                }
                else
                {
                    back.SetUrl(notused);
                }
            }
            back.SetX(startX.ToString());
            back.SetY(startY.ToString());
            back.SetRelative(ui.relative);
            back.SetEvent(inventory.GetName() + "," + ip.GetX() + "," + ip.GetY());
            FreeImageComponet itemImg = back;
            FreeImageComponet secondComponent = GetItemImage();
            if (secondComponent != null)
            {
                back.SetEvent(string.Empty);
                itemImg = secondComponent;
            }
            if (ui.nomouse)
            {
                itemImg.SetNoMouse("true");
            }
            if (ui.itemFixed)
            {
                itemImg.SetFixed("true");
            }
            itemImg.SetUrl(ip.GetKey().GetImg());
            itemImg.SetOriginalSize(ip.GetKey().GetItemWidth() + "," + ip.GetKey().GetItemHeight());
            if (itemImg == back)
            {
                AdjustSize(args, ui, ip, inventory, back, itemImg, startX, startY);
            }
            else
            {
                AdjustSize(args, ui, ip, inventory, back, itemImg, 0, 0);
            }
            SetCount(ip);
            fui.SetKey(ip.GetUIKey());
            fui.SetShow(inventory.IsOpen());
            fui.SetScope(1);
            fui.SetPlayer("current");
            fui.Act(args);
            UpdateHotKey(ui, args, ip);
            args.GetDefault().GetParameters().Resume("img");
            args.GetDefault().GetParameters().Resume("name");
            args.GetDefault().GetParameters().Resume("count");
        }

        private void UpdateHotKey(SimpleInventoryUI ui, IEventArgs args, ItemPosition ip)
        {
            if (ui.hotkey != null && GetHotKey() != null)
            {
                string hotkey = ui.hotkey.GetHotKey(args, ip);
                if (!StringUtil.IsNullOrEmpty(hotkey))
                {
                    FreeUIUpdateAction update = new FreeUIUpdateAction();
                    update.SetPlayer("current");
                    update.SetScope(1);
                    update.SetKey(ip.GetUIKey());
                    FreeUIImageValue value = new FreeUIImageValue();
                    value.SetUrl(hotkey);
                    value.SetSeq(this.hotkey.ToString());
                    update.AddValue(value);
                    update.Act(args);
                }
            }
        }

        private void SetCount(ItemPosition ip)
        {
            FreeNumberComponet countUI = GetCount();
            if (countUI != null)
            {
                if (ip.GetKey().GetItemStack() == 1)
                {
                    countUI.SetNumber(string.Empty);
                }
                else
                {
                    countUI.SetNumber(ip.GetCount().ToString());
                }
            }
        }

        private void AdjustSize(IEventArgs args, SimpleInventoryUI ui, ItemPosition ip, ItemInventory inventory, FreeImageComponet back, FreeImageComponet itemImg, int startX, int startY)
        {
            //back.SetWidth((ip.GetKey().GetGridWidth() * ui.itemWidth + (ip.GetKey().GetGridWidth() - 1) * ui.GetWidthMargin(inventory, args)).ToString());
            //back.SetHeight((ip.GetKey().GetGridHeight() * ui.itemHeight + (ip.GetKey().GetGridHeight() - 1) * ui.GetHeightMargin(inventory, args)).ToString());
            double w = double.Parse(back.GetWidth(args)) / (double)ip.GetKey().GetItemWidth();
            double h = double.Parse(back.GetHeight(args)) / (double)ip.GetKey().GetItemHeight();
            double per = h;
            if (w < h)
            {
                per = w;
            }
            int iw = (int)(ip.GetKey().GetItemWidth() * per * ip.GetKey().GetIconScale());
            //itemImg.SetWidth(iw.ToString());
            int ih = (int)(ip.GetKey().GetItemHeight() * per * ip.GetKey().GetIconScale());
            //itemImg.SetHeight(ih.ToString());
            //itemImg.SetX((startX + (NumberUtil.GetInt(back.GetWidth(args)) - iw) / 2).ToString());
            //itemImg.SetY((startY + (NumberUtil.GetInt(back.GetHeight(args)) - ih) / 2).ToString());
            itemImg.SetEvent(inventory.GetName() + "," + ip.GetX() + "," + ip.GetY());
        }

        private int GetX(IEventArgs args, SimpleInventoryUI ui, ItemPosition ip, FreeImageComponet back)
        {
            int widthMargin = ui.GetWidthMargin(inventory, args);
            return ui.GetX(args) + FreeUIUtil.GetRightX(FreeUtil.ReplaceInt(ui.relative, args), ip.GetX() * (ui.itemWidth + widthMargin));
        }

        private int GetY(IEventArgs args, SimpleInventoryUI ui, ItemPosition ip, FreeImageComponet back)
        {
            int heightMargin = ui.GetHeightMargin(inventory, args);
            return ui.GetY(args) + FreeUIUtil.GetDownY(FreeUtil.ReplaceInt(ui.relative, args), ip.GetY() * (ui.itemHeight + heightMargin));
        }

        private FreeImageComponet GetBackground()
        {
            if (back <= components.Count)
            {
                return (FreeImageComponet)components[back - 1];
            }
            return null;
        }

        private FreeImageComponet GetItemImage()
        {
            if (item <= components.Count)
            {
                return (FreeImageComponet)components[item - 1];
            }
            return null;
        }

        private IFreeComponent GetHotKey()
        {
            if (hotkey > 0 && hotkey <= components.Count)
            {
                return components[hotkey - 1];
            }
            return null;
        }

        private FreeNumberComponet GetCount()
        {
            if (count > 0 && count <= components.Count)
            {
                return (FreeNumberComponet)components[count - 1];
            }
            return null;
        }
    }
}
