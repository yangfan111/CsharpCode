using System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using System.Collections.Generic;
using App.Client.GameModules.Ui;
using App.Shared.DebugHandle;
using App.Shared.GameMode;
using App.Client.GameMode;

namespace Assets.Sources
{

    public class UnityUserCmdGenerator : IUserCmdGenerator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UnityUserCmdGenerator));
        private int _seq;
        private UserCmd _userCmd = UserCmd.Allocate();
        KeyReceiver _keyReceiver;
        KeyReceiver _specialKeyReceiver;
        KeyReceiver _uiKeyReceiver;
        PointerReceiver _pointerReceiver;
        IUserInputManager _userInputManager;
        List<IGlobalKeyInputMapper> _inputMapperList = new List<IGlobalKeyInputMapper>();

        private float[] yaws;
        private float[] pitchs;

#if UNITY_EDITOR
        private List<Action<UserCmd>> _mockCmdList = new List<Action<UserCmd>>();
#endif
        public UnityUserCmdGenerator()
        {
            Type = CmdGeneratorType.Hunam;
            _seq = 0;

            this.yaws = new float[4];
            this.pitchs = new float[4];
        }

        public void RegisterGlobalKeyReceiver(IGlobalKeyInputMapper inputMapper)
        {
            _inputMapperList.Add(inputMapper);
        }

        //这里所有的操作层级都设置为Env，如果有特殊遮挡需求请修改层级并修改这段注释
        public void BeginReceiveUserInput(IUserInputManager manager)
        {
            _userInputManager = manager;
            #region Env keyreceiver

            // 有UI打开的情况下，不能开枪, 不能瞄准

            {
                _specialKeyReceiver = new KeyReceiver(UiConstant.specicalCmdKeyLayer, BlockType.None);
                _specialKeyReceiver.AddAction(UserInputKey.Fire, (data) => _userCmd.IsLeftAttack = true);
                _specialKeyReceiver.AddAction(UserInputKey.RightAttack, (data) =>
                {
                    _userCmd.IsRightAttack = true;
                });
                for(int i = 0; i < _inputMapperList.Count; i++)
                {
                    _inputMapperList[i].RegisterSpecialCmdKeyInput(_specialKeyReceiver, _userCmd);
                }
                _specialKeyReceiver.AddAction(UserInputKey.CameraFocus, (data) =>
                {
                    _userCmd.IsCameraFocus = true;
                });
                _userInputManager.RegisterKeyReceiver(_specialKeyReceiver);
            }

            _keyReceiver = new KeyReceiver(UiConstant.userCmdKeyLayer, BlockType.None);
            _keyReceiver.AddAction(UserInputKey.SwitchFireMode, (data) => _userCmd.IsSwitchFireMode = true);
            for(int i = 0; i < _inputMapperList.Count; i++)
            {
                _inputMapperList[i].RegisterEnvKeyInput(_keyReceiver, _userCmd);
            }

            _keyReceiver.AddAction(UserInputKey.DrawWeapon, (data) => _userCmd.IsDrawWeapon = true);
            _keyReceiver.AddAction(UserInputKey.Throwing, (data) => _userCmd.IsThrowing = true);
;
            _keyReceiver.AddAction(UserInputKey.FirstPerson,
                (data) =>
                {
                    _userCmd.ChangeCamera = true;
                });
            _keyReceiver.AddAction(UserInputKey.FreeCamera, (data) =>
            {
                _userCmd.IsCameraFree = true;
            });
            _keyReceiver.AddAction(UserInputKey.Jump, (data) => _userCmd.IsJump = true);
            _keyReceiver.AddAction(UserInputKey.Crouch, (data) => _userCmd.IsCrouch = true);
            _keyReceiver.AddAction(UserInputKey.Prone, (data) => _userCmd.IsProne = true);
            _keyReceiver.AddAction(UserInputKey.Injured, (data) => _userCmd.BeState = 2);
            _keyReceiver.AddAction(UserInputKey.Swim, (data) => _userCmd.BeState = 1);
            _keyReceiver.AddAction(UserInputKey.Reload, (data) => _userCmd.IsReload = true);
            _keyReceiver.AddAction(UserInputKey.PeekLeft, (data) => _userCmd.IsPeekLeft = true);
            _keyReceiver.AddAction(UserInputKey.PeekRight, (data) => _userCmd.IsPeekRight = true);
            _keyReceiver.AddAction(UserInputKey.SwitchWeapon, (data) => _userCmd.IsSwitchWeapon = true);
            _keyReceiver.AddAction(UserInputKey.DropWeapon, (data) => _userCmd.IsDropWeapon = true);
            _keyReceiver.AddAction(UserInputKey.Switch1, data => _userCmd.SwitchNumber = 1);
            _keyReceiver.AddAction(UserInputKey.Switch7, data => _userCmd.SwitchNumber = 7);
            _keyReceiver.AddAction(UserInputKey.IsPDown, data => _userCmd.IsPDown = true);
            _keyReceiver.AddAction(UserInputKey.IsYDown, data => _userCmd.IsYDown = true);
            _keyReceiver.AddAction(UserInputKey.AddMark, data => _userCmd.IsAddMark = true);
            _keyReceiver.AddAction(UserInputKey.BreathHold, data => _userCmd.IsHoldBreath = true);
            _keyReceiver.AddAction(UserInputKey.SwitchAutoRun, data => _userCmd.IsSwitchAutoRun = true);
            _keyReceiver.AddAction(UserInputKey.IsCDown, data => _userCmd.IsCDown = true);
            _keyReceiver.AddAction(UserInputKey.IsSpaceDown, data => _userCmd.IsSpaceDown = true);
            _keyReceiver.AddAction(UserInputKey.HoldF, data => _userCmd.IsF = true);
            _keyReceiver.AddAction(UserInputKey.SprayPaint, data => _userCmd.IsSprayPaint = true);
            _userInputManager.RegisterKeyReceiver(_keyReceiver);

            #endregion

            #region Ui Keyreciever

            _uiKeyReceiver = new KeyReceiver(UiConstant.userCmdUIKeyLayer, BlockType.None);
            _uiKeyReceiver.AddAction(UserInputKey.MoveHorizontal, (data) => _userCmd.MoveHorizontal = data.Axis);
            _uiKeyReceiver.AddAction(UserInputKey.MoveVertical, (data) => _userCmd.MoveVertical = data.Axis);
            _uiKeyReceiver.AddAction(UserInputKey.MoveUpDown, (data) => _userCmd.MoveUpDown = data.Axis);
            _uiKeyReceiver.AddAction(UserInputKey.Run, (data) => _userCmd.IsRun = true);
            _uiKeyReceiver.AddAction(UserInputKey.IsTabDown, data => _userCmd.IsTabDown = true);
            _uiKeyReceiver.AddAction(UserInputKey.SlightWalk, data => _userCmd.IsSlightWalk = true);
            _userInputManager.RegisterKeyReceiver(_uiKeyReceiver);

            #endregion

#if UNITY_EDITOR
            foreach (var mockCmd in _mockCmdList)
            {
                mockCmd(_userCmd);
            }
#endif

            var pointerReceiver = new PointerReceiver(UiConstant.userCmdPointLayer, BlockType.None);
            pointerReceiver.AddAction(UserInputKey.Yaw, (pointerData) =>
            {
                for (int i = 0; i < yaws.Length - 1; i++)
                {
                    yaws[i] = yaws[i + 1];
                }
                yaws[yaws.Length - 1] = pointerData.MouseX;
                float total = 0;
                for (int i = yaws.Length - 1; i >= Math.Max(0, yaws.Length - BigMapDebug.SmoothFactor); i--)
                {
                    total = total + yaws[i];
                }
                total = total / (float) BigMapDebug.SmoothFactor;
                //Debug.LogFormat("deltaYaw is {0}, deltaTime {1}", total, Time.deltaTime);
#if UNITY_EDITOR
                if (!Cursor.visible)
                {
                    _userCmd.DeltaYaw = total;
                }
#else
                _userCmd.DeltaYaw = total;
#endif
            });
            pointerReceiver.AddAction(UserInputKey.Pitch, (pointerData) =>
            {
                for (int i = 0; i < pitchs.Length - 1; i++)
                {
                    pitchs[i] = pitchs[i + 1];
                }
                pitchs[pitchs.Length - 1] = pointerData.MouseY;
                float total = 0;
                for (int i = pitchs.Length - 1; i >= Math.Max(0, pitchs.Length - BigMapDebug.SmoothFactor); i--)
                {
                    total = total + pitchs[i];
                }
                total = total / (float) BigMapDebug.SmoothFactor;
#if UNITY_EDITOR
                if (!Cursor.visible)
                {
                    _userCmd.DeltaPitch = -total;
                }
#else
                _userCmd.DeltaPitch = -total;
#endif
            });

            _userInputManager.RegisterPointerReceiver(pointerReceiver);
        }

        public void StopReceiveUserInput()
        {
            _userInputManager.UnregisterKeyReceiver(_specialKeyReceiver);
            _userInputManager.UnregisterKeyReceiver(_uiKeyReceiver);
            _userInputManager.UnregisterKeyReceiver(_keyReceiver);
            _userInputManager.UnregisterPointerReceiver(_pointerReceiver);
        }

        public CmdGeneratorType Type { get; private set; }

        public UserCmd GenerateUserCmd(IUserCmdOwnAdapter player, int intverval)
        {
            _userCmd.Seq = _seq++;
            _userCmd.FrameInterval = intverval;
            _userCmd.ChangedSeat = ChangeSeat();
            _userCmd.ChangeChannel = ChangeChannel();
            var rc = UserCmd.Allocate();
            _userCmd.CopyTo(rc);
            _userCmd.Reset();
            return rc;
        }

        public void SetLastUserCmd(UserCmd missing_name)
        {

        }

        /// <summary>
        /// 设置UserCmd的值
        /// </summary>
        /// <param name="cb"></param>
        public void SetUserCmd(Action<UserCmd> cb)
        {
            if (null != cb)
            {
                cb(_userCmd);
            }
        }

        public void MockUserCmd(Action<UserCmd> cb)
        {
#if UNITY_EDITOR
            _mockCmdList.Add(cb);
#endif
        }

        private int ChangeSeat()
        {
            int keyCode = (int) KeyCode.Alpha0;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.N))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    keyCode = (int) KeyCode.Alpha1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    keyCode = (int) KeyCode.Alpha2;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    keyCode = (int) KeyCode.Alpha3;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    keyCode = (int) KeyCode.Alpha4;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    keyCode = (int) KeyCode.Alpha5;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    keyCode = (int)KeyCode.Alpha6;
                }
            }

            return keyCode - (int) KeyCode.Alpha0;
        }

        private int ChangeChannel()
        {
         
            int keyCode = (int) KeyCode.Alpha0;
            if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.N))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    keyCode = (int)KeyCode.Alpha1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    keyCode = (int)KeyCode.Alpha2;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    keyCode = (int)KeyCode.Alpha3;
                }
            }
           
            return keyCode - (int) KeyCode.Alpha0;
        }
    }
}