using com.wd.free.@event;
using com.wd.free.item;
using gameplay.gamerule.free.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.item
{
    public interface IItemUI
    {
        void Draw(SimpleInventoryUI ui, IEventArgs args, ItemInventory inventory, ItemPosition ip);
    }
}
