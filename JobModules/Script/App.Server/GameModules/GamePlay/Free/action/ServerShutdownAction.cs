﻿using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class ServerShutdownAction: AbstractGameAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            args.Rule.GameExit = true;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.ServerShutdownAction;
        }
    }
}
