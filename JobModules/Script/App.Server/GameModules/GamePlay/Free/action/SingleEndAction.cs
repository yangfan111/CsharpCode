using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class SingleEndAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
