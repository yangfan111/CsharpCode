using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace com.wd.free.action.stage
{
    [Serializable]
    public class AddMultiFrameAction : AbstractGameAction, IRule
    {
        private string key;
        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
           args.FreeContext.MultiFrame.BindKeyAction(FreeUtil.ReplaceVar(key, args), (IGameAction)SerializeUtil.Clone(action));
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.AddMultiFrameAction;
        }
    }
}
