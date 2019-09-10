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
        private static LoggerAdapter logger = new LoggerAdapter(typeof(UnityUserCmdGenerator));
       
        private UserCmd userCmd = UserCmd.Allocate();
        KeyHandler _keyHandler;
        KeyHandler _specialKeyHandler;
        KeyHandler _uKeyHandler;
        KeyHandler _bioKeyHandler;
        KeyHandler _bioSpecialKeyHandler;
        PointerKeyHandler _pointerKeyHandler;
        UserInputManager.Lib.UserInputManager userInputManager;
        List<IGlobalKeyInputMapper> inputMapperList = new List<IGlobalKeyInputMapper>();

        private float[] yaws;
        private float[] pitchs;

#if UNITYEDITOR
        private List<Action<UserCmd>> mockCmdList = new List<Action<UserCmd>>();
#endif
        public UnityUserCmdGenerator()
        {
            Type = CmdGeneratorType.Hunam;

            this.yaws = new float[4];
            this.pitchs = new float[4];
        }

        public void RegisterGlobalKeyhandler(IGlobalKeyInputMapper inputMapper)
        {
            inputMapperList.Add(inputMapper);
        }

        //这里所有的操作层级都设置为Env，如果有特殊遮挡需求请修改层级并修改这段注释
        public void BeginReceiveUserInput(UserInputManager.Lib.UserInputManager manager)
        {
            userInputManager = manager;
            #region Env keyhandler

            // 有UI打开的情况下，不能开枪, 不能瞄准
            _specialKeyHandler = new KeyHandler(UiConstant.specicalCmdKeyLayer, BlockType.None);
            for(int i = 0; i < inputMapperList.Count; i++)
            {
                inputMapperList[i].RegisterSpecialCmdKeyInput(_specialKeyHandler, userCmd);
            }
            _specialKeyHandler.BindKeyAction(UserInputKey.Fire, (data) => userCmd.IsLeftAttack = true);
            _specialKeyHandler.BindKeyAction(UserInputKey.RightAttack, (data) => userCmd.IsRightAttack = true);
            _specialKeyHandler.BindKeyAction(UserInputKey.CameraFocus, (data) => userCmd.IsCameraFocus = true);
            userInputManager.RegisterKeyhandler(_specialKeyHandler);

            _keyHandler = new KeyHandler(UiConstant.userCmdKeyLayer, BlockType.None);
            for(int i = 0; i < inputMapperList.Count; i++)
            {
                inputMapperList[i].RegisterEnvKeyInput(_keyHandler, userCmd);
            }
            _keyHandler.BindKeyAction(UserInputKey.SwitchFireMode, (data) => userCmd.IsSwitchFireMode = true);
            _keyHandler.BindKeyAction(UserInputKey.DrawWeapon, (data) => userCmd.IsDrawWeapon = true);
            _keyHandler.BindKeyAction(UserInputKey.Throwing, (data) => userCmd.IsThrowing = true);
            _keyHandler.BindKeyAction(UserInputKey.FirstPerson, (data) => userCmd.ChangeCamera = true);
            _keyHandler.BindKeyAction(UserInputKey.FreeCamera, (data) => userCmd.IsCameraFree = true);
            _keyHandler.BindKeyAction(UserInputKey.Jump, (data) => userCmd.IsJump = true);
            _keyHandler.BindKeyAction(UserInputKey.Crouch, (data) => userCmd.IsCrouch = true);
            _keyHandler.BindKeyAction(UserInputKey.Prone, (data) => userCmd.IsProne = true);
            //keyhandler.AddAction(UserInputKey.Injured, (data) => userCmd.BeState = 2);
            //keyhandler.AddAction(UserInputKey.Swim, (data) => userCmd.BeState = 1);
            _keyHandler.BindKeyAction(UserInputKey.Reload, (data) => userCmd.IsReload = true);
            _keyHandler.BindKeyAction(UserInputKey.PeekLeft, (data) => userCmd.IsPeekLeft = true);
            _keyHandler.BindKeyAction(UserInputKey.PeekRight, (data) => userCmd.IsPeekRight = true);
            _keyHandler.BindKeyAction(UserInputKey.SwitchWeapon, (data) => userCmd.IsSwitchWeapon = true);
            _keyHandler.BindKeyAction(UserInputKey.DropWeapon, (data) => userCmd.IsDropWeapon = true);
            _keyHandler.BindKeyAction(UserInputKey.Switch1, data => userCmd.SwitchNumber = 1);
            _keyHandler.BindKeyAction(UserInputKey.Switch7, data => userCmd.SwitchNumber = 7);
            _keyHandler.BindKeyAction(UserInputKey.IsPDown, data => userCmd.IsPDown = true);
            _keyHandler.BindKeyAction(UserInputKey.IsYDown, data => userCmd.IsYDown = true);
            _keyHandler.BindKeyAction(UserInputKey.AddMark, data => userCmd.IsAddMark = true);
            _keyHandler.BindKeyAction(UserInputKey.BreathHold, data => userCmd.IsHoldBreath = true);
            _keyHandler.BindKeyAction(UserInputKey.SwitchAutoRun, data => userCmd.IsSwitchAutoRun = true);
            _keyHandler.BindKeyAction(UserInputKey.IsCDown, data => userCmd.IsCDown = true);
            _keyHandler.BindKeyAction(UserInputKey.IsSpaceDown, data => userCmd.IsSpaceDown = true);
            _keyHandler.BindKeyAction(UserInputKey.HoldF, data => userCmd.IsF = true);
            _keyHandler.BindKeyAction(UserInputKey.SprayPaint, data => userCmd.IsSprayPaint = true);
            _keyHandler.BindKeyAction(UserInputKey.ScopeIn, data => userCmd.IsScopeIn = true);
            _keyHandler.BindKeyAction(UserInputKey.ScopeOut, data => userCmd.IsScopeOut = true);
            userInputManager.RegisterKeyhandler(_keyHandler);

            #endregion
            _bioSpecialKeyHandler = new KeyHandler(UiConstant.specicalCmdKeyLayer, BlockType.None);
            _bioSpecialKeyHandler.BindKeyAction(UserInputKey.Fire, (data) => userCmd.IsLeftAttack = true);
            _bioSpecialKeyHandler.BindKeyAction(UserInputKey.RightAttack, (data) => userCmd.IsRightAttack = true);
            _bioSpecialKeyHandler.BindKeyAction(UserInputKey.CameraFocus, (data) => userCmd.IsCameraFocus = true);

            _bioKeyHandler = new KeyHandler(UiConstant.userCmdKeyLayer, BlockType.None);
            _bioKeyHandler.BindKeyAction(UserInputKey.Throwing, (data) => userCmd.IsThrowing = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.FreeCamera, (data) => userCmd.IsCameraFree = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.Jump, (data) => userCmd.IsJump = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.Crouch, (data) => userCmd.IsCrouch = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.Prone, (data) => userCmd.IsProne = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.IsPDown, data => userCmd.IsPDown = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.IsYDown, data => userCmd.IsYDown = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.AddMark, data => userCmd.IsAddMark = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.BreathHold, data => userCmd.IsHoldBreath = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.SwitchAutoRun, data => userCmd.IsSwitchAutoRun = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.IsCDown, data => userCmd.IsCDown = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.IsSpaceDown, data => userCmd.IsSpaceDown = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.HoldF, data => userCmd.IsF = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.SprayPaint, data => userCmd.IsSprayPaint = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.ScopeIn, data => userCmd.IsScopeIn = true);
            _bioKeyHandler.BindKeyAction(UserInputKey.ScopeOut, data => userCmd.IsScopeOut = true);
            #region bioMain


            #endregion

            #region Ui Keyreciever

            _uKeyHandler = new KeyHandler(UiConstant.userCmdUIKeyLayer, BlockType.None);
            _uKeyHandler.BindKeyAction(UserInputKey.MoveHorizontal, (data) => userCmd.MoveHorizontal = data.Axis);
            _uKeyHandler.BindKeyAction(UserInputKey.MoveVertical, (data) => userCmd.MoveVertical = data.Axis);
            _uKeyHandler.BindKeyAction(UserInputKey.MoveUpDown, (data) => userCmd.MoveUpDown = data.Axis);
            _uKeyHandler.BindKeyAction(UserInputKey.Run, (data) => userCmd.IsRun = true);
            _uKeyHandler.BindKeyAction(UserInputKey.IsTabDown, data => userCmd.IsTabDown = true);
            _uKeyHandler.BindKeyAction(UserInputKey.SlightWalk, data => userCmd.IsSlightWalk = true);
            userInputManager.RegisterKeyhandler(_uKeyHandler);

            #endregion

#if UNITYEDITOR
            foreach (var mockCmd in mockCmdList)
            {
                mockCmd(userCmd);
            }
#endif

            var pointerhandler = new PointerKeyHandler(UiConstant.userCmdPointLayer, BlockType.None);
            pointerhandler.BindPointAction(UserInputKey.Yaw, (keyData) =>
            {
                var pointerData = keyData as PointerData;
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
#if UNITYEDITOR
                if (!Cursor.visible)
                {
                    userCmd.DeltaYaw = total;
                }
#else
                userCmd.DeltaYaw = total;
#endif
            });
            pointerhandler.BindPointAction(UserInputKey.Pitch, (keyData) =>
            {
                var pointerData = keyData as PointerData;
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
#if UNITYEDITOR
                if (!Cursor.visible)
                {
                    userCmd.DeltaPitch = -total;
                }
#else
                userCmd.DeltaPitch = -total;
#endif
            });

            userInputManager.RegisterPointerhandler(pointerhandler);
        }

        public void StopReceiveUserInput()
        {
            userInputManager.UnregisterKeyhandler(_specialKeyHandler);
            userInputManager.UnregisterKeyhandler(_uKeyHandler);
            userInputManager.UnregisterKeyhandler(_keyHandler);
            userInputManager.UnregisterPointerhandler(_pointerKeyHandler);
            userInputManager.UnregisterKeyhandler(_bioKeyHandler);
        }

        public void StartReceiveUserInput()
        {
            userInputManager.RegisterKeyhandler(_specialKeyHandler);
            userInputManager.RegisterKeyhandler(_uKeyHandler);
            userInputManager.RegisterKeyhandler(_keyHandler);
            userInputManager.RegisterPointerhandler(_pointerKeyHandler);
            userInputManager.RegisterKeyhandler(_bioKeyHandler);
        }

        public void SwitchMode(EModeSwitch mode)
        {
            switch (mode) {
                case EModeSwitch.Normal:
                    userInputManager.RegisterKeyhandler(_specialKeyHandler);
                    userInputManager.RegisterKeyhandler(_keyHandler);
                    userInputManager.UnregisterKeyhandler(_bioSpecialKeyHandler);
                    userInputManager.UnregisterKeyhandler(_bioKeyHandler);
                    break;
                case EModeSwitch.Bio:
                    userInputManager.RegisterKeyhandler(_bioSpecialKeyHandler);
                    userInputManager.RegisterKeyhandler(_bioKeyHandler);
                    userInputManager.UnregisterKeyhandler(_specialKeyHandler);
                    userInputManager.UnregisterKeyhandler(_keyHandler);
                    break;
            }
        }

        public CmdGeneratorType Type { get; private set; }

        public UserCmd GenerateUserCmd(IUserCmdOwnAdapter player, int intverval)
        {
            logger.DebugFormat("GenerateUserCmd:{0}", MyGameTime.seq);
            userCmd.Seq = MyGameTime.seq;
            userCmd.FrameInterval = intverval;
            userCmd.ChangedSeat = ChangeSeat();
            userCmd.ChangeChannel = ChangeChannel();
            var rc = UserCmd.Allocate();
            userCmd.CopyTo(rc);
            userCmd.Reset();
            return rc;
        }

        public void SetLastUserCmd(UserCmd missingname)
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
                cb(userCmd);
            }
        }

        public void MockUserCmd(Action<UserCmd> cb)
        {
#if UNITYEDITOR
            mockCmdList.Add(cb);
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
            return userCmd;
        }
    }
}