using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map;
using com.wd.free.para;
using com.wd.free.unit;
using com.wd.free.util;
using Sharpen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class ResetPosAction : AbstractGameAction
    {
        private const int TYPE = 103;
        private int resetpos;
        
        public override void DoAction(IEventArgs args)
        {
            ParaList list = args.GetDefault().GetParameters();
            if (list.HasPara("resetpos"))
            {
                resetpos = (int)list.Get("resetpos").GetValue();
                var poss = args.FreeContext.Poss;
                if (!poss.ExsitIndex(TYPE, resetpos))
                {
                    poss.Remove(TYPE, resetpos);
                }
            }
        }
    }
}
