using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Shared.GameModules.Player;
using Core.Free;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerDisableAction : AbstractPlayerAction, IRule
    {
        private string enable;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity p = GetPlayerEntity(args);
            if (p != null)
            {
                PlayerEntityUtility.SetActive(p, args.GetBool(enable));
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerDisableAction;
        }
    }
}
