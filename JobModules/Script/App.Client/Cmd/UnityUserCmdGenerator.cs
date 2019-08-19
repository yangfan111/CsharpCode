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
using Common;

namespace Assets.Sources
{

    public class UnityUserCmdGenerator : IUserCmdGenerator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UnityUserCmdGenerator));
       
        private UserCmd _userCmd = UserCmd.Allocate();
        KeyReceiver _keyReceiver;
        KeyReceiver _specialKeyReceiver;
        KeyReceiver _uiKeyReceiver;
        KeyReceiver _bioKeyReceiver;
        KeyReceiver _bioSpecialKeyReceiver;
        PointerReceiver _pointerReceiver;
        GameInputManager _userInputManager;
        List<IGlobalKeyInputMapper> _inputMapperList = new List<IGlobalKeyInputMapper>();

        private float[] yaws;
        private float[] pitchs;

#if UNITY_EDITOR
        private List<Action<UserCmd>> _mockCmdList = new List<Action<UserCmd>>();
#endif
        public UnityUserCmdGenerator()
        {
            Type = CmdGeneratorType.Hunam;

            this.yaws = new float[4];
            this.pitchs = new float[4];
        }

        public void RegisterGlobalKeyReceiver(IGlobalKeyInputMapper inputMapper)
        {
            _inputMapperList.Add(inputMapper);
        }

        //这里所有的操作层级都设置为Env，如果有特殊遮挡需求请修改层级并修改这段注释
        public void BeginReceiveUserInput(GameInputManager manager)
        {
            _userInputManager = manager;
            #region Env keyreceiver

            // 有UI打开的情况下，不能开枪, 不能瞄准
            _specialKeyReceiver = new KeyReceiver(UiConstant.specicalCmdKeyLayer, BlockType.None,"specicalCmd");
            for(int i = 0; i < _inputMapperList.Count; i++)
            {
                _inputMapperList[i].RegisterSpecialCmdKeyInput(_specialKeyReceiver, _userCmd);
            }
            _specialKeyReceiver.BindKeyAction(UserInputKey.Fire, (data) => _userCmd.IsLeftAttack = true);
            _specialKeyReceiver.BindKeyAction(UserInputKey.RightAttack, (data) => _userCmd.IsRightAttack = true);
            _specialKeyReceiver.BindKeyAction(UserInputKey.CameraFocus, (data) => _userCmd.IsCameraFocus = true);
            _userInputManager.RegisterKeyReceiver(_specialKeyReceiver);

            _keyReceiver = new KeyReceiver(UiConstant.userCmdKeyLayer, BlockType.None,"userCmdLower");
            for(int i = 0; i < _inputMapperList.Count; i++)
            {
                _inputMapperList[i].RegisterEnvKeyInput(_keyReceiver, _userCmd);
            }
            _keyReceiver.BindKeyAction(UserInputKey.SwitchFireMode, (data) => _userCmd.IsSwitchFireMode = true);
            _keyReceiver.BindKeyAction(UserInputKey.DrawWeapon, (data) => _userCmd.IsDrawWeapon = true);
            _keyReceiver.BindKeyAction(UserInputKey.Throwing, (data) => _userCmd.IsThrowing = true);
            _keyReceiver.BindKeyAction(UserInputKey.FirstPerson, (data) => _userCmd.ChangeCamera = true);
            _keyReceiver.BindKeyAction(UserInputKey.FreeCamera, (data) => _userCmd.IsCameraFree = true);
            _keyReceiver.BindKeyAction(UserInputKey.Jump, (data) => _userCmd.IsJump = true);
            _keyReceiver.BindKeyAction(UserInputKey.Crouch, (data) => _userCmd.IsCrouch = true);
            _keyReceiver.BindKeyAction(UserInputKey.Prone, (data) => _userCmd.IsProne = true);
            //_keyReceiver.BindKeyAction(UserInputKey.Injured, (data) => _userCmd.BeState = 2);
            //_keyReceiver.BindKeyAction(UserInputKey.Swim, (data) => _userCmd.BeState = 1);
            _keyReceiver.BindKeyAction(UserInputKey.Reload, (data) => _userCmd.IsReload = true);
            _keyReceiver.BindKeyAction(UserInputKey.PeekLeft, (data) => _userCmd.IsPeekLeft = true);
            _keyReceiver.BindKeyAction(UserInputKey.PeekRight, (data) => _userCmd.IsPeekRight = true);
            _keyReceiver.BindKeyAction(UserInputKey.SwitchWeapon, (data) => _userCmd.IsSwitchWeapon = true);
            _keyReceiver.BindKeyAction(UserInputKey.DropWeapon, (data) => _userCmd.IsDropWeapon = true);
            _keyReceiver.BindKeyAction(UserInputKey.Switch1, data => _userCmd.SwitchNumber = 1);
            _keyReceiver.BindKeyAction(UserInputKey.Switch7, data => _userCmd.SwitchNumber = 7);
            _keyReceiver.BindKeyAction(UserInputKey.IsPDown, data => _userCmd.IsPDown = true);
            _keyReceiver.BindKeyAction(UserInputKey.IsYDown, data => _userCmd.IsYDown = true);
            _keyReceiver.BindKeyAction(UserInputKey.AddMark, data => _userCmd.IsAddMark = true);
            _keyReceiver.BindKeyAction(UserInputKey.BreathHold, data => _userCmd.IsHoldBreath = true);
            _keyReceiver.BindKeyAction(UserInputKey.SwitchAutoRun, data => _userCmd.IsSwitchAutoRun = true);
            _keyReceiver.BindKeyAction(UserInputKey.IsCDown, data => _userCmd.IsCDown = true);
            _keyReceiver.BindKeyAction(UserInputKey.IsSpaceDown, data => _userCmd.IsSpaceDown = true);
            _keyReceiver.BindKeyAction(UserInputKey.HoldF, data => _userCmd.IsF = true);
            _keyReceiver.BindKeyAction(UserInputKey.SprayPaint, data => _userCmd.IsSprayPaint = true);
            _keyReceiver.BindKeyAction(UserInputKey.ScopeIn, data => _userCmd.IsScopeIn = true);
            _keyReceiver.BindKeyAction(UserInputKey.ScopeOut, data => _userCmd.IsScopeOut = true);
            _userInputManager.RegisterKeyReceiver(_keyReceiver);

            #endregion
            _bioSpecialKeyReceiver = new KeyReceiver(UiConstant.specicalCmdKeyLayer, BlockType.None);
            _bioSpecialKeyReceiver.BindKeyAction(UserInputKey.Fire, (data) => _userCmd.IsLeftAttack = true);
            _bioSpecialKeyReceiver.BindKeyAction(UserInputKey.RightAttack, (data) => _userCmd.IsRightAttack = true);
            _bioSpecialKeyReceiver.BindKeyAction(UserInputKey.CameraFocus, (data) => _userCmd.IsCameraFocus = true);

            _bioKeyReceiver = new KeyReceiver(UiConstant.userCmdKeyLayer, BlockType.None);
            _bioKeyReceiver.BindKeyAction(UserInputKey.Throwing, (data) => _userCmd.IsThrowing = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.FreeCamera, (data) => _userCmd.IsCameraFree = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.Jump, (data) => _userCmd.IsJump = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.Crouch, (data) => _userCmd.IsCrouch = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.Prone, (data) => _userCmd.IsProne = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.IsPDown, data => _userCmd.IsPDown = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.IsYDown, data => _userCmd.IsYDown = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.AddMark, data => _userCmd.IsAddMark = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.BreathHold, data => _userCmd.IsHoldBreath = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.SwitchAutoRun, data => _userCmd.IsSwitchAutoRun = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.IsCDown, data => _userCmd.IsCDown = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.IsSpaceDown, data => _userCmd.IsSpaceDown = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.HoldF, data => _userCmd.IsF = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.SprayPaint, data => _userCmd.IsSprayPaint = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.ScopeIn, data => _userCmd.IsScopeIn = true);
            _bioKeyReceiver.BindKeyAction(UserInputKey.ScopeOut, data => _userCmd.IsScopeOut = true);
            #region bioMain


            #endregion

            #region Ui Keyreciever

            _uiKeyReceiver = new KeyReceiver(UiConstant.userCmdUIKeyLayer, BlockType.None,"UserCmdAUpper");
            _uiKeyReceiver.BindKeyAction(UserInputKey.MoveHorizontal, (data) => _userCmd.MoveHorizontal = data.Axis);
            _uiKeyReceiver.BindKeyAction(UserInputKey.MoveVertical, (data) => _userCmd.MoveVertical = data.Axis);
            _uiKeyReceiver.BindKeyAction(UserInputKey.MoveUpDown, (data) => _userCmd.MoveUpDown = data.Axis);
            _uiKeyReceiver.BindKeyAction(UserInputKey.Run, (data) => _userCmd.IsRun = true);
            _uiKeyReceiver.BindKeyAction(UserInputKey.IsTabDown, data => _userCmd.IsTabDown = true);
            _uiKeyReceiver.BindKeyAction(UserInputKey.SlightWalk, data => _userCmd.IsSlightWalk = true);
            _userInputManager.RegisterKeyReceiver(_uiKeyReceiver);

            #endregion

#if UNITY_EDITOR
            foreach (var mockCmd in _mockCmdList)
            {
                mockCmd(_userCmd);
            }
#endif

            var pointerReceiver = new PointerReceiver(UiConstant.userCmdPointLayer, BlockType.None);
            pointerReceiver.BindPointAction(UserInputKey.Yaw, (data) =>
            {
                var pointerData = data as PointerData;
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
            pointerReceiver.BindPointAction(UserInputKey.Pitch, (data) =>
            {
                var pointerData = data as PointerData;
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
            _userInputManager.UnregisterKeyReceiver(_bioKeyReceiver);
        }

        public void StartReceiveUserInput()
        {
            _userInputManager.RegisterKeyReceiver(_specialKeyReceiver);
            _userInputManager.RegisterKeyReceiver(_uiKeyReceiver);
            _userInputManager.RegisterKeyReceiver(_keyReceiver);
            _userInputManager.RegisterPointerReceiver(_pointerReceiver);
            _userInputManager.RegisterKeyReceiver(_bioKeyReceiver);
        }

        public void SwitchMode(EModeSwitch mode)
        {
            switch (mode) {
                case EModeSwitch.Normal:
                    _userInputManager.RegisterKeyReceiver(_specialKeyReceiver);
                    _userInputManager.RegisterKeyReceiver(_keyReceiver);
                    _userInputManager.UnregisterKeyReceiver(_bioSpecialKeyReceiver);
                    _userInputManager.UnregisterKeyReceiver(_bioKeyReceiver);
                    break;
                case EModeSwitch.Bio:
                    _userInputManager.RegisterKeyReceiver(_bioSpecialKeyReceiver);
                    _userInputManager.RegisterKeyReceiver(_bioKeyReceiver);
                    _userInputManager.UnregisterKeyReceiver(_specialKeyReceiver);
                    _userInputManager.UnregisterKeyReceiver(_keyReceiver);
                    break;
            }
        }

        public CmdGeneratorType Type { get; private set; }

        public UserCmd GenerateUserCmd(IUserCmdOwnAdapter player, int intverval)
        {
            _logger.DebugFormat("GenerateUserCmd:{0}", MyGameTime.seq);
            _userCmd.Seq = MyGameTime.seq;
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

        public UserCmd GetUserCmd()
        {
            return _userCmd;
        }
    }
}