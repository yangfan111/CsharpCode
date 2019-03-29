using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.unit;
using com.wd.free.util;

namespace gameplay.gamerule.free.item
{
    [System.Serializable]
    public class SelectItemAction : AbstractPlayerAction
    {
        private const long serialVersionUID = 7344720901070127419L;

        private string exp;

        [System.NonSerialized]
        private SelectMethod method;

        private string count;

        private IGameAction action;

        private string item;

        private string id;

        private string usePosition;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            IGameUnit player = GetPlayer(args);
            if (StringUtil.IsNullOrEmpty(count) || count.Equals("0"))
            {
                count = "10000000";
            }
            if (StringUtil.IsNullOrEmpty(item))
            {
                item = "item";
            }
            if (player != null)
            {
                FreeData fd = (FreeData)player;
                if (!StringUtil.IsNullOrEmpty(id))
                {
                    ItemPosition ip = fd.freeInventory.GetItemById(FreeUtil.ReplaceInt(id, args));
                    if (ip != null)
                    {
                        HandleItem(ip, fr);
                    }
                }
                else
                {
                    if (!StringUtil.IsNullOrEmpty(exp))
                    {

                        method = new SelectMethod(FreeUtil.ReplaceVar(exp, args));

                        ItemPosition[] currentItems = fd.freeInventory.Select(method);
                        for (int i = 0; i < MyMath.Min(FreeUtil.ReplaceInt(count, args), currentItems.Length); i++)
                        {
                            ItemPosition ip = currentItems[i];
                            HandleItem(ip, fr);
                        }
                    }
                }
            }
        }

        private void HandleItem(ItemPosition ip, FreeRuleEventArgs fr)
        {
            if (action != null)
            {
                if ("true".Equals(FreeUtil.ReplaceVar(usePosition, fr)))
                {
                    fr.TempUse(item, ip);
                }
                else
                {
                    fr.TempUse(item, ip.GetKey());
                }
                action.Act(fr);
                fr.Resume(item);
            }
        }
    }
}
