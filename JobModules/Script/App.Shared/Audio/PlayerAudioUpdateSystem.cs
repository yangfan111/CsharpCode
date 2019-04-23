using App.Shared.Configuration;
using App.Shared.Player.Events;
using App.Shared.Terrains;
using Core.Event;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.Audio
{
    public class PlayerAudioUpdateSystem:IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var audioController = GameModuleManagement.Get<PlayerAudioController>(owner.OwnerEntityKey.EntityId).Value;
            if(audioController != null)
                audioController.Update();
        }
    }
}