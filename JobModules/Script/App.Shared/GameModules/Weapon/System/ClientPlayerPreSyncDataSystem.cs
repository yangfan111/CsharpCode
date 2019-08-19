using App.Shared.EntityFactory;
using App.Shared.GameModules.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using Core.Components;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;

namespace App.Shared.GameModules.Weapon
{
    public class ClientPlayerPreSyncDataSystem:IUserCmdExecuteSystem
    {
       
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            PrecisionsVector3 locatorDelta = new PrecisionsVector3(player.characterBoneInterface.CharacterBone.BaseLocatorDelta,4);
            var clientUpdateComp = player.playerClientUpdate;
            var gamePlay = player.gamePlay;
            clientUpdateComp.LastSpreadOffsetX = locatorDelta.x;
            clientUpdateComp.LastSpreadOffsetY = locatorDelta.y;
            clientUpdateComp.DestoryPreparedThrowingEntity = PlayerStateUtil.HasUIState(gamePlay);
            if (gamePlay.UIStateUpdate)
            {
                gamePlay.UIStateUpdate = false;
                FreeMessageSender.SendOpenSpecifyUIMessageC(player);
            }
           
            // if ( && )
            // {
            //     FreeMessageSender.SendOpenSpecifyUIMessageC(player);
            // }
        }
    }
}