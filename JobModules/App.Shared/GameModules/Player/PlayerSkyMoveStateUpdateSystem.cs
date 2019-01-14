using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.HitBox;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using XmlConfig;
using Object = UnityEngine.Object;

namespace App.Shared.GameModules.Player
{


    public class PlayerSkyMoveStateUpdateSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerSkyMoveStateUpdateSystem));
        private Contexts _contexts;
        public PlayerSkyMoveStateUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            
            var playerSkyMove = player.playerSkyMove;
            SynToInterVar(player);
            playerSkyMove.IsMoveEnabled = PlayerSkyMoveState.IsSkyMoveEnabled(player);
            //Logger.InfoFormat("client move enabled!!!playerSkyMove stage:{0}, game state:{1}, seq:{2}",playerSkyMove.MoveStage, player.gamePlay.GameState, cmd.Seq);

            if (playerSkyMove.IsMoveEnabled)
            {
                var stage = playerSkyMove.MoveStage;
                PlayerSkyMoveStateMachine.GetStates()[(int)stage].Update(_contexts, player, cmd); 
            }

            PlayerSkyMoveState.ValidateStateAfterUpdate(_contexts, player);
        }

        private void SynToInterVar(PlayerEntity player)
        {
            player.playerSkyMoveInterVar.SyncFrom(player.playerSkyMove);
        }

      
    }
}
