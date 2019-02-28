using App.Shared.Player;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.para.exp;
using com.wd.free.unit;
using com.wd.free.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.Free.condition
{
    [Serializable]
    public class PlayerStateCondition : IParaCondition
    {
        public const int InCar = 101;

        private string player;
        private string state;

        public virtual bool Meet(IEventArgs args)
        {
            int realState = FreeUtil.ReplaceInt(state, args);
            PlayerEntity p = ((FreeRuleEventArgs)args).GetPlayer(player);
            if (p != null)
            {
                if (realState > 100)
                {
                    switch (realState)
                    {
                        case InCar:
                            return p.IsOnVehicle();
                        default:
                            return false;
                    }
                }
                else
                {
                    return PlayerStateUtil.HasPlayerState((EPlayerGameState)realState, p.gamePlay);
                }
            }

            return false;
        }
    }
}
