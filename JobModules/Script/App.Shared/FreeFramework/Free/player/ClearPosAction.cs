using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map;
using Core.Free;
using com.wd.free.unit;
using com.wd.free.util;
using Sharpen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class ClearPosAction : AbstractGameAction, IRule
    {
        private const int TYPE = 103;
        private int resetpos;

        public override void DoAction(IEventArgs args)
        {
            var poss = args.FreeContext.Poss;
            poss.Clear(TYPE);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.ClearPosAction;
        }
    }
}
