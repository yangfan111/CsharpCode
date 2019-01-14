using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;

namespace com.wd.free.action.stage
{
    [Serializable]
    public class AddMultiFrameAction : AbstractGameAction
    {
        private string key;
        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
           args.FreeContext.MultiFrame.AddAction(FreeUtil.ReplaceVar(key, args), (IGameAction)SerializeUtil.Clone(action));
        }
    }
}
