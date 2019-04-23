using App.Shared;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using com.wd.free.ai;
using Core.Free;
using Core.Utils;
using Free.framework;
using UnityEngine;
using Utils.Singleton;
using WindowsInput;
using WindowsInput.Native;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerCmdHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerCmd || key == FreeMessageConstant.PlayerPressKey;
        }

        public void Handle(SimpleProto data)
        {
            if (data.Key == FreeMessageConstant.PlayerCmd)
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
                            player.playerIntercept.PressKeys.Clear();
                            break;
                        case PlayerActEnum.CmdType.PressKey:
                            player.playerIntercept.PressKeys.AddKeyTime(data.Ins[1], data.Ins[0]);
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

            if (data.Key == FreeMessageConstant.PlayerPressKey)
            {
                var simulator = new InputSimulator();
                
                if (data.Ins[1] == 0)
                {
                    if (data.Ins[0] == 1)
                    {
                        simulator.Mouse.LeftButtonClick();
                    }
                    else if(data.Ins[0] == 2)
                    {
                        simulator.Mouse.RightButtonClick();
                    }
                    else
                    {
                        simulator.Keyboard.KeyPress((VirtualKeyCode) data.Ins[0]);
                    }
                }
                else
                {
                    if (data.Ins[0] == 1)
                    {
                        simulator.Mouse.LeftButtonDown();
                    }
                    else if(data.Ins[0] == 2)
                    {
                        simulator.Mouse.RightButtonDown();
                    }
                    else
                    {
                        simulator.Keyboard.KeyDown((VirtualKeyCode) data.Ins[0]);
                    }

                    SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity.playerIntercept.RealPressKeys.AddKeyTime(data.Ins[0], data.Ins[1]);
                }
            }
        }
    }
}
