using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.util;
using gameplay.gamerule.free.rule;
using UnityEngine;

namespace gameplay.gamerule.free.item
{
    [System.Serializable]
    public class DefineItemAction : AbstractGameAction
    {
        private const long serialVersionUID = 5687056626382340079L;

        private IList<FreeItem> items;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            if (items != null)
            {
                foreach (FreeItem fi in items)
                {
                    if (!FreeItemManager.ContainsItem(args.Rule.FreeType, FreeUtil.ReplaceVar(fi.GetKey(), args)))
                    {
                        FreeGameItem gi = (FreeGameItem)fi.Clone();
                        gi.SetItemStack(FreeUtil.ReplaceVar(gi.GetItemStackVar(), args));
                        gi.SetKey(FreeUtil.ReplaceVar(gi.GetKey(), args));
                        gi.SetImg(FreeUtil.ReplaceVar(gi.GetImg(), args));
                        gi.SetName(FreeUtil.ReplaceVar(gi.GetName(), args));
                        gi.SetDesc(FreeUtil.ReplaceVar(gi.GetDesc(), args));
                        gi.SetCat(FreeUtil.ReplaceVar(gi.GetCat(), args));
                        gi.SetIconSize(FreeUtil.ReplaceVar(gi.GetIconSize(), args));
                        gi.SetWidth(FreeUtil.ReplaceNumber(gi.GetWidth(), args));
                        gi.SetHeight(FreeUtil.ReplaceNumber(gi.GetHeight(), args));
                        gi.Created(fr);
                        FreeItemManager.AddItem(args, args.Rule.FreeType, gi);
                    }
                }
            }
        }
    }
}
