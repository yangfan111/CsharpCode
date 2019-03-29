using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.ui;

namespace gameplay.gamerule.free.item
{
    [System.Serializable]
    public class PlayerRemoveItemAction : AbstractPlayerAction
    {
        private const long serialVersionUID = 2754682892581756843L;

        private string item;

        private string exp;

        private string count;

        [System.NonSerialized]
        private SelectMethod method;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            IGameUnit player = GetPlayer(args);
            if (player != null)
            {
                if (!StringUtil.IsNullOrEmpty(item))
                {
                    IParable pe = args.GetUnit(item);
                    if (pe != null && pe is ItemPosition)
                    {
                        args.TempUse("current", (FreeData)player);
                        ItemPosition ip = (ItemPosition)pe;
                        ip.GetInventory().RemoveItem(fr, ip);
                        RemoveUI(ip, fr);
                        args.Resume("current");
                    }
                }
                else
                {

                    method = new SelectMethod(FreeUtil.ReplaceVar(exp, args));

                    FreeData fd = (FreeData)player;
                    ItemPosition[] currentItems = fd.freeInventory.Select(method);
                    if (StringUtil.IsNullOrEmpty(count))
                    {
                        count = "1";
                    }
                    int c = FreeUtil.ReplaceInt(count, args);
                    for (int i = 0; i < MyMath.Min(c, currentItems.Length); i++)
                    {
                        args.TempUse("current", (FreeData)player);
                        ItemPosition ip = currentItems[i];
                        ip.GetInventory().RemoveItem(fr, ip);
                        RemoveUI(ip, fr);
                        args.Resume("current");
                    }
                }
            }
        }

        private void RemoveUI(ItemPosition ip, ISkillArgs args)
        {
            //FreeUIDeleteAction del = new FreeUIDeleteAction();
            //del.SetKey(ip.GetUIKey());
            //del.SetScope(1);
            //del.SetPlayer("current");
            //del.Act(args);
        }
    }
}
