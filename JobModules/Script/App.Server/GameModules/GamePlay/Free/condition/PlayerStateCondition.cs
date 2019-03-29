using App.Shared.GameModules.Player;
using App.Shared.Player;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.util;
using System;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.condition
{
    [Serializable]
    public class PlayerStateCondition : IParaCondition
    {
        public const int InCar = 101;
        public const int Jump = 102;

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
                        case Jump:
                            return p.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Jump;
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
