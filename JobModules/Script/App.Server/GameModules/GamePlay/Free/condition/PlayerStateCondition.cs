using App.Shared;
using App.Shared.GameModules.Player;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.util;
using Core.Free;
using System;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.condition
{
    [Serializable]
    public class PlayerStateCondition : IParaCondition, IRule
    {
        public const int InCar = 101;
        public const int Jump = 102;
        public const int Stand = 103;

        public const int SwitchWeapon = 201;
        public const int Climb = 202;

        private string player;
        private string state;

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerStateCondition;
        }

        public virtual bool Meet(IEventArgs args)
        {
            int realState = FreeUtil.ReplaceInt(state, args);
            PlayerEntity p = ((FreeRuleEventArgs)args).GetPlayer(player);

            if (p != null)
            {
                if(realState > 200)
                {
                    switch (realState)
                    {
                        case SwitchWeapon:
                            return p.StateInteractController().GetCurrStates().Contains(EPlayerState.SwitchWeapon);
                        case Climb:
                            return p.StateInteractController().GetCurrStates().Contains(EPlayerState.Climb);
                        default:
                            return false;
                    }
                }
                else if (realState > 100)
                {
                    switch (realState)
                    {
                        case InCar:
                            return p.IsOnVehicle();
                        case Jump:
                            return p.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Jump;
                        case Stand:
                            return p.stateInterface.State.GetCurrentMovementState() == MovementInConfig.Idle;
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
