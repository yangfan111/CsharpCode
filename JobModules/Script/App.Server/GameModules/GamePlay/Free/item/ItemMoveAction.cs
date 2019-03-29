using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.action;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.rule;

namespace gameplay.gamerule.free.item
{
    [System.Serializable]
    public class ItemMoveAction : AbstractPlayerAction
    {
        private const long serialVersionUID = 883839350426129267L;

        private string item;

        private string toInventory;

        private string x;

        private string y;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            IGameUnit player = GetPlayer(args);
            if (player != null)
            {
                IParable pa = fr.GetUnit(item);
                if (pa is ItemPosition)
                {
                    ItemPosition ip = (ItemPosition)pa;
                    FreeData fd = (FreeData)player;
                    ItemInventory toInv = fd.freeInventory.GetInventoryManager().GetInventory(FreeUtil.ReplaceVar(toInventory, args));
                    if (toInv != null)
                    {
                        int toX = 0;
                        int toY = 0;
                        if(string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                        {
                            int[] next = toInv.GetNextEmptyPosition(ip.GetKey());
                            toX = next[0];
                            toY = next[1];
                        }
                        else
                        {
                            toX = FreeUtil.ReplaceInt(x, args);
                            toY = FreeUtil.ReplaceInt(y, args);
                        }
                        ItemInventoryUtil.MovePosition(ip, toInv, toX, toY, fr);
                    }
                }
            }
        }
    }
}
