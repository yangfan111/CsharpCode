using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using Assets.App.Server.GameModules.GamePlay.Free;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Shared.FreeFramework.framework.ai.attack
{
    class SimpleAttackAiAction : AbstractPlayerAction
    {
        private string target;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity source = GetPlayerEntity(args);

            FreeData fd = (FreeData)args.GetUnit(target);
            if(source != null && fd != null)
            {
                
            }
        }
    }
}
