using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.item
{
    public class UnityInventoryUtil
    {
        public static void Drag(ISkillArgs args, PlayerEntity player, ItemInventory fromInv, ItemInventory toInv, ItemPosition ip, ItemPosition toIp)
        {
            if (toInv != null && fromInv != toInv)
            {
                
            }
            else
            {
                if (toInv == null)
                {
                    fromInv.RemoveItem(args, ip);
                }
                else
                {
                    fromInv.GetInventoryUI().ReDraw(args, fromInv, true);
                }
            }
        }
    }
}
