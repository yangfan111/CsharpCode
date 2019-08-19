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

        private void HandlePlayerCmd(SimpleProto data)
        {
            int          id     = data.Ks[1];
            PlayerEntity player =  SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(id, (short)EEntityType.Player));
            if (player != null)
            {
                var pintercept = player.playerIntercept;
                pintercept.InterceptType = data.Ks[0];
                PlayerActEnum.CmdType type = (PlayerActEnum.CmdType)data.Ks[0];
                switch (type)
                {
                    case PlayerActEnum.CmdType.Walk:
                        pintercept.MovePos = new Vector3(data.Fs[0], data.Fs[1], data.Fs[2]);
                        break;
                    case PlayerActEnum.CmdType.ClearKeys:
                        pintercept.InterceptKeys.Clear();
                        pintercept.PressKeys.Clear();
                        break;
                    case PlayerActEnum.CmdType.PressKey:
                        pintercept.PressKeys.AddKeyTime(data.Ins[1], data.Ins[0]);
                        break;
                    case PlayerActEnum.CmdType.InterceptKey:
                        pintercept.InterceptKeys.AddKeyTime(data.Ins[1], data.Ins[0]);
                        break;
                    case PlayerActEnum.CmdType.Attack:
                        pintercept.AttackPlayerId = data.Ks[2];
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandlePlayerPress(SimpleProto data)
        {
                KeyCode code = KeyCode.None;
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Enum.GetName(typeof(KeyCode), kc).ToLower().Equals(data.Ss[0].ToLower()))
                    {
                        code = kc;
                        break;
                    }
                }

                if (code == KeyCode.None) return;

                GameInputManager manager =  SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Mgr;
                Dictionary<UserInputKey, InputConvertItem> inputMap = SingletonManager.Get<InputConfigManager>().itemsMap;

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
                    foreach ( InputConvertItem covertItem in inputMap.Values)
                    {
                        foreach (KeyConvertItem item in covertItem.Items)
                        {
                            if (item.Key == code && item.State == UserInputState.KeyDown)
                            {
                                PosDict.Add(covertItem.Key, 0);
                                HoldDict.Add(covertItem.Key, false);
                            }

                            if (item.Key == code && item.State == UserInputState.KeyHold)
                            {
                                PosDict.Add(covertItem.Key, 0);
                                HoldDict.Add(covertItem.Key, true);
                            }

                            if (keyName.Equals(item.InputKey))
                            {
                                PosDict.Add(covertItem.Key, positive ? 1 : -1);
                                HoldDict.Add(covertItem.Key, hold);
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
                                SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity.playerIntercept.
                                                RealPressKeys.AddKeyTime((int) uik, data.Ins[0], PosDict[uik]);
                            }
                        }
                    }
                }
                else
                {
                    Dictionary<UserInputKey, int> UpDict = new Dictionary<UserInputKey, int>();
                    Dictionary<UserInputKey, int> RealseDict = new Dictionary<UserInputKey, int>();

                    foreach (InputConvertItem inputValue in inputMap.Values)
                    {
                        foreach (KeyConvertItem item in inputValue.Items)
                        {
                            if (item.Key == code)
                            {
                                if (item.State == UserInputState.KeyUp)
                                {
                                    UpDict.Add(inputValue.Key, 0);
                                }
                                else
                                {
                                    RealseDict.Add(inputValue.Key, 0);
                                }
                            }

                            if (keyName.Equals(item.InputKey))
                            {
                                RealseDict.Add(inputValue.Key, 0);
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
                            SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity.playerIntercept.RealPressKeys.Release((int) uik);
                        }
                    }
            }
        }
        public void Handle(SimpleProto data)
        {
            switch (data.Key)
            {
                case FreeMessageConstant.PlayerCmd:
                    HandlePlayerCmd(data);
                    break;
                case FreeMessageConstant.PlayerPressKey:
                    HandlePlayerPress(data);
                    break;
                    
            }
         

          
        }
    }
}
