using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components.Player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Player;
using com.wd.free.@event;
using Core.Enums;
using Core.Free;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Shared.Scripts.Effect;
using System.Collections.Generic;
using UnityEngine;


namespace App.Shared.GameModules.Player
{
    public class PlayerWitnessSystem : IUserCmdExecuteSystem
    {
        private bool _witness;
        private Contexts _contexts;
        public PlayerWitnessSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            /*throw new System.NotImplementedException();*/
            var player = owner.OwnerEntity as PlayerEntity;
            if (player.hasGamePlay)
            {
                if (_witness != player.gamePlay.Witness) {
                    PlayerEntityUtility.SetActive(player, !player.gamePlay.Witness, EActiveMask.Witness);
                    _witness = player.gamePlay.Witness;
                }
            }
        }
    }
}
