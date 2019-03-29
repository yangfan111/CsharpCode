using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class PlayerRaycastCondition : IParaCondition
    {
        public int key;
        public string player;

        public bool Meet(IEventArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit(player);
            if (fd != null)
            {
                return fd.Player.hasRaycastTarget && fd.Player.raycastTarget.Key == key;
            }

            return false;
        }
    }
}
