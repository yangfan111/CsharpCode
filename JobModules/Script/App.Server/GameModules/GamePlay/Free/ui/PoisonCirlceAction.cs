using System;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using Core.Free;
using gameplay.gamerule.free.ui;

namespace App.Server.GameModules.GamePlay.Free.action.ui
{
    [Serializable]
    public class PoisonCirlceAction : SendMessageAction
    {
        private IPosSelector fromPos;
        private string fromRadius;
        private string fromWaitTime;
        private string fromMoveTime;

        private IPosSelector toPos;
        private string toRadius;

        protected override void BuildMessage(IEventArgs args)
        {
            this.scope = "4";

            builder.Key = FreeMessageConstant.PoisonCircle;
            UnitPosition from = fromPos.Select(args);
            builder.Ks.Add(0);
            builder.Fs.Add(from.GetX());
            builder.Fs.Add(from.GetY());

            UnitPosition to = toPos.Select(args);
            builder.Fs.Add(to.GetX());
            builder.Fs.Add(to.GetY());

            builder.Ins.Add(FreeUtil.ReplaceInt(fromRadius, args));
            builder.Ins.Add(FreeUtil.ReplaceInt(fromWaitTime, args));
            builder.Ins.Add(FreeUtil.ReplaceInt(fromMoveTime, args));
            builder.Ins.Add(FreeUtil.ReplaceInt(toRadius, args));
            builder.Ins.Add(FreeUtil.ReplaceInt(fromWaitTime, args));
            builder.Ins.Add(FreeUtil.ReplaceInt(fromMoveTime, args));
        }
    }
}
