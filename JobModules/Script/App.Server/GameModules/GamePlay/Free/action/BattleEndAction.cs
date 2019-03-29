using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class BattleEndAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {
            args.Rule.GameOver = true;
        }
    }
}
