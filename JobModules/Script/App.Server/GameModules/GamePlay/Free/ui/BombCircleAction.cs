using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using Core.Free;
using gameplay.gamerule.free.map;
using gameplay.gamerule.free.ui;

namespace App.Server.GameModules.GamePlay.Free.action.ui
{
    [Serializable]
    public class BombCircleAction : SendMessageAction
    {
        private IPosSelector pos;
        private string radius;

        protected override void BuildMessage(IEventArgs args)
        {
            this.scope = "4";

            builder.Key = FreeMessageConstant.PoisonCircle;
            UnitPosition from = pos.Select(args);
            builder.Ks.Add(1);
            builder.Fs.Add(from.GetX());
            builder.Fs.Add(from.GetZ());

            builder.Ins.Add(FreeUtil.ReplaceInt(radius, args));
        }
    }
}
