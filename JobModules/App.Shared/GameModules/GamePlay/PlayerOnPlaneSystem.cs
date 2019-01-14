using Core.GameModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.UserPrediction.Cmd;
using App.Shared.Components;
using App.Shared.Player;
using UnityEngine;

namespace App.Shared.GameModules.GamePlay
{
    public class PlayerOnPlaneSystem : IUserCmdExecuteSystem
    {
        private Contexts contexts;
 
        public PlayerOnPlaneSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (player.gamePlay.GameState == GameState.AirPlane)
            {
                FreeMoveEntity[] entities = contexts.freeMove.GetEntities();
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[0].isAirPlane)
                    {
                        Vector3 v = entities[0].position.Value;
                        v.y = v.y + 20;
                        player.position.Value = v;
                        player.RootGo().transform.parent = null;
                    }
                }
            }
        }
    }
}
