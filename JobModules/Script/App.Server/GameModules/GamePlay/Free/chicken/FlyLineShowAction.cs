using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    public class FlyLineShowAction : SendMessageAction
    {
        protected override void BuildMessage(IEventArgs args)
        {
            this.scope = "4";

            builder.Key = FreeMessageConstant.AirLineData;
            builder.Bs.Add(false);

            Vector2 start = ChickenRuleVars.GetAirLineStartPos(args);
            Vector2 stop = ChickenRuleVars.GetAirLineStopPos(args);

            builder.Fs.Add(start.x);
            builder.Fs.Add(start.y);
            builder.Fs.Add(stop.x);
            builder.Fs.Add(stop.y);
        }
    }
}
