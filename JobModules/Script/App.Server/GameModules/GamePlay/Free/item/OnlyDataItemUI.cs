using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.item;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class OnlyDataItemUi : IItemUI
    {
        public void Draw(SimpleInventoryUI ui, IEventArgs args, ItemInventory inventory, ItemPosition ip)
        {
            FreeUICreateAction fui = new FreeUICreateAction();

            FreeImageComponet img = new FreeImageComponet();
            int startX = GetX(inventory, args, ui, ip, img);
            int startY = GetY(inventory, args, ui, ip, img);

            img.SetX(startX.ToString());
            img.SetY(startY.ToString());
            img.SetRelative(ui.relative);
            img.SetWidth("64");
            img.SetHeight("64");
            img.SetEvent(inventory.GetName() + "," + ip.GetX() + "," + ip.GetY());
            if (ui.nomouse)
            {
                img.SetNoMouse("true");
            }
            if (ui.itemFixed)
            {
                img.SetFixed("true");
            }

            img.SetUrl(ip.GetKey().GetImg());
            img.SetOriginalSize(ip.GetKey().GetItemWidth() + "," + ip.GetKey().GetItemHeight());

            fui.GetComponents().Add(img);

            fui.SetKey(ip.GetUIKey());
            fui.SetShow(inventory.IsOpen());
            fui.SetScope(1);
            fui.SetPlayer("current");
            fui.Act(args);
        }

        private int GetX(ItemInventory inventory, IEventArgs args, SimpleInventoryUI ui, ItemPosition ip, FreeImageComponet back)
        {
            int widthMargin = ui.GetWidthMargin(inventory, args);
            return ui.GetX(args) + FreeUIUtil.GetRightX(FreeUtil.ReplaceInt(ui.relative, args), ip.GetX() * (ui.itemWidth + widthMargin));
        }

        private int GetY(ItemInventory inventory, IEventArgs args, SimpleInventoryUI ui, ItemPosition ip, FreeImageComponet back)
        {
            int heightMargin = ui.GetHeightMargin(inventory, args);
            return ui.GetY(args) + FreeUIUtil.GetDownY(FreeUtil.ReplaceInt(ui.relative, args), ip.GetY() * (ui.itemHeight + heightMargin));
        }
    }
}
