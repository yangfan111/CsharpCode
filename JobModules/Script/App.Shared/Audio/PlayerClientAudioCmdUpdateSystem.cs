using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.Player.Events;
using App.Shared.Terrains;
using Core.Event;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.Audio
{
    public class PlayerClientAudioCmdUpdateSystem:IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var audioController = owner.OwnerEntityKey.AudioController();
            if(audioController != null)
                audioController.Update(cmd);
        }
    }
}