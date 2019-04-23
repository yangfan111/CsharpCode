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

            int totalTime = args.GetInt("{waitFlyTime}");
            int startTime = args.GetInt("{canJumpTime}") / 1000;
            int forceTime = args.GetInt("{forceJumpTime}") / 1000;

            Vector2 from = start + (stop - start) * startTime / totalTime;
            Vector2 to = start + (stop - start) * forceTime / totalTime;

            Debug.LogFormat("{0} to {1}, {2} {3} {4}", from, to, totalTime, startTime, forceTime);

            builder.Fs.Add(from.x);
            builder.Fs.Add(from.y);
            builder.Fs.Add(to.x);
            builder.Fs.Add(to.y);
        }
    }
}
