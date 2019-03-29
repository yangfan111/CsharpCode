using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Shared.GameModules.Player;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerDisableAction : AbstractPlayerAction
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

    }
}
