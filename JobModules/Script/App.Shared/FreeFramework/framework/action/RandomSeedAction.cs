using com.cpkf.yyjd.tools.util.math;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.framework.action
{
    [Serializable]
    public class RandomSeedAction : AbstractGameAction
    {
        private string seed;

        public override void DoAction(IEventArgs args)
        {
            RandomUtil.SetSeed(FreeUtil.ReplaceInt(seed,args));
        }
    }
}
