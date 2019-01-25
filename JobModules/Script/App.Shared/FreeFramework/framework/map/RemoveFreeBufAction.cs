using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.cpkf.yyjd.tools.util;
using com.wd.free.util;

namespace com.wd.free.map
{
    [Serializable]
    public class RemoveFreeBufAction : AbstractGameAction
    {
        private String key;

        public override void DoAction(IEventArgs args)
        {
            if (key != null)
            {
                foreach (String k in StringUtil.Split(FreeUtil.ReplaceVar(key, args),
                        new String[] { ",", "，" }))
                {
                    args.FreeContext.Bufs.RemoveBuf(args, k);
                }
            }
        }
    }
}
