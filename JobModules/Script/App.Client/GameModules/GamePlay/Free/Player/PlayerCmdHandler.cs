using App.Shared;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using com.wd.free.ai;
using Core.Free;
using Free.framework;
using Luminosity.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Singleton;

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
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;

            if (data.Key == FreeMessageConstant.PlayerCmd)
            {
                int id = data.Ks[1];
                PlayerEntity player = contexts.player.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(id, (short)EEntityType.Player));
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
                KeyCode code = KeyCode.None;
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Enum.GetName(typeof(KeyCode), kc).ToLower().Equals(data.Ss[0].ToLower()))
                    {
                        code = kc;
                    }
                }

                if (code == KeyCode.None) return;

                IUserInputManager manager = contexts.userInput.userInputManager.Instance;
                Dictionary<UserInputKey, List<KeyConvertItem>> config = manager.GetInputDic();
                PlayerEntity player = contexts.player.flagSelfEntity;

                string keyName = "None";
                bool positive = true, hold = false;
                foreach (InputAction ia in Luminosity.IO.InputManager.GetControlScheme("normal").Actions)
                {
                    foreach (InputBinding ib in ia.Bindings)
                    {
                        if (ib.Positive == code || ib.Negative == code)
                        {
                            keyName = ia.Name;
                            positive = ib.Positive == code;
                            hold = ib.Type == InputType.DigitalAxis;
                        }
                    }
                }

                if (data.Bs[0])
                {
                    Dictionary<UserInputKey, int> PosDict = new Dictionary<UserInputKey, int>();
                    Dictionary<UserInputKey, bool> HoldDict = new Dictionary<UserInputKey, bool>();
                    foreach (UserInputKey uik in config.Keys)
                    {
                        foreach (KeyConvertItem item in config[uik])
                        {
                            if (item.Key == code && item.State == UserInputState.KeyDown)
                            {
                                PosDict.Add(uik, 0);
                                HoldDict.Add(uik, false);
                            }

                            if (item.Key == code && item.State == UserInputState.KeyHold)
                            {
                                PosDict.Add(uik, 0);
                                HoldDict.Add(uik, true);
                            }

                            if (keyName.Equals(item.InputKey))
                            {
                                PosDict.Add(uik, positive ? 1 : -1);
                                HoldDict.Add(uik, hold);
                            }
                        }
                    }

                    if (PosDict.Count > 0)
                    {
                        foreach (UserInputKey uik in PosDict.Keys)
                        {
                            if (data.Ins[0] == 0 || !HoldDict[uik])
                            {
                                manager.InsertKey(new KeyData(uik, PosDict[uik]));
                            }

                            if (data.Ins[0] > 0 && HoldDict[uik])
                            {
                                player.playerIntercept.RealPressKeys.AddKeyTime((int) uik, data.Ins[0], PosDict[uik]);
                            }
                        }
                    }
                }
                else
                {
                    Dictionary<UserInputKey, int> UpDict = new Dictionary<UserInputKey, int>();
                    Dictionary<UserInputKey, int> RealseDict = new Dictionary<UserInputKey, int>();

                    foreach (UserInputKey uik in config.Keys)
                    {
                        foreach (KeyConvertItem item in config[uik])
                        {
                            if (item.Key == code)
                            {
                                if (item.State == UserInputState.KeyUp)
                                {
                                    UpDict.Add(uik, 0);
                                }
                                else
                                {
                                    RealseDict.Add(uik, 0);
                                }
                            }

                            if (keyName.Equals(item.InputKey))
                            {
                                RealseDict.Add(uik, 0);
                            }
                        }
                    }

                    if (UpDict.Count > 0)
                    {
                        foreach (UserInputKey uik in UpDict.Keys)
                        {
                            manager.InsertKey(new KeyData(uik));
                        }
                    }

                    if (RealseDict.Count > 0)
                    {
                        foreach (UserInputKey uik in RealseDict.Keys)
                        {
                            player.playerIntercept.RealPressKeys.Release((int) uik);
                        }
                    }
                }
            }
        }
    }
}
