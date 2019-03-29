using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Assets.Sources.Free.UI;
using Core.Prediction.UserPrediction.Cmd;
using UnityEngine;
using com.wd.free.ai;
using App.Shared;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerCmdHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerCmd;
        }

        public void Handle(SimpleProto data)
        {
            int id = data.Ks[1];

            PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(id, (short)EEntityType.Player));
            if (player != null)
            {
                player.playerIntercept.InterceptType = data.Ks[0];

                PlayerActEnum.CmdType type = (PlayerActEnum.CmdType)data.Ks[0];
                switch (type)
                {
                    case PlayerActEnum.CmdType.Walk:
                        player.playerIntercept.MovePos = new Vector3(data.Fs[0], data.Fs[1], data.Fs[2]);
                        break;
                    case PlayerActEnum.CmdType.ClearKeys:
                        player.playerIntercept.InterceptKeys.Clear();
                        player.playerIntercept.PresssKeys.Clear();
                        break;
                    case PlayerActEnum.CmdType.PressKey:
                        player.playerIntercept.PresssKeys.AddKeyTime(data.Ins[1], data.Ins[0]);
                        break;
                    case PlayerActEnum.CmdType.InterceptKey:
                        player.playerIntercept.InterceptKeys.AddKeyTime(data.Ins[1], data.Ins[0]);
                        break;
                    case PlayerActEnum.CmdType.Attack:
                        player.playerIntercept.AttackPlayerId = data.Ks[2];
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
