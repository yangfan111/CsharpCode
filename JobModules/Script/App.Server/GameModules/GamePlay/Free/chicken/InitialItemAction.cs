using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.Free.item;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    class InitialItemAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {
            FreeItemDrop.Initial();
        }
    }
}
