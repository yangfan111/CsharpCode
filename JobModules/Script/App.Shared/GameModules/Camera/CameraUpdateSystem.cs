using App.Shared.Components.Player;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Shared.GameModules.Camera.Utils;
using Core.Configuration;
using Core.EntityComponent;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera
{
    class CameraUpdateSystem : AbstractCameraUpdateSystem, IUserCmdExecuteSystem, ISimpleParallelUserCmdExecuteSystem, IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraUpdateSystem));

        private Motors _motors;

        private DummyCameraMotorState _state;
        private DummyCameraMotorState _dummyState;

        private PlayerContext _playerContext;
        private int _cmdSeq = 0;

        DummyCameraMotorOutput _output = new DummyCameraMotorOutput();
        DummyCameraMotorOutput _tempOutput = new DummyCameraMotorOutput();
        private Contexts _context;

        public CameraUpdateSystem(Contexts context, Motors m) : base(context)
        {
            _playerContext = context.player;
            _motors = m;
            _state = new DummyCameraMotorState();
            _dummyState = new DummyCameraMotorState();
        }


        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var player = getter.OwnerEntity as PlayerEntity;
            if (player == null) return;

            CommonUpdate(player,cmd);
        }
        
        protected override void ExecWhenObserving(PlayerEntity player, IUserCmd cmd)
        {
            var observedPlayer = player.observeCamera.ObservedPlayer;
            
            if (observedPlayer != null)
            {
                DummyCameraMotorInput _input = (DummyCameraMotorInput) observedPlayer.cameraStateNew.CameraMotorInput;
                if (_input == null) 
                    _input = new DummyCameraMotorInput();
                _input.FakeForObserve(observedPlayer);
                
                DummyCameraMotorState.Convert(observedPlayer.cameraStateNew, _state);
                
                observedPlayer.cameraStateNew.CameraMotorInput = observedPlayer.cameraStateNew.LastCameraMotorInput;
                observedPlayer.cameraStateNew.LastCameraMotorInput = _input;
                
                var result = observedPlayer.cameraStateOutputNew;
                CalcFinalOutput(observedPlayer, (DummyCameraMotorInput) observedPlayer.cameraStateNew.LastCameraMotorInput,
                    result,_state);
            
                DummyCameraMotorState.Convert(_state, observedPlayer.cameraStateNew);
                
            }

            RestoreSelfPlayer(player);
        }

        private void RestoreSelfPlayer(PlayerEntity player)
        {
            var dict = _motors.GetDict(SubCameraMotorType.View);
            var subState = _state.Get(SubCameraMotorType.View);
            var oldMotor = dict[subState.NowMode];
            if (oldMotor.ModeId != (short) ECameraViewMode.ThirdPerson)
            {
                subState.NowMode = (byte) ECameraViewMode.ThirdPerson;
                subState.LastMode = (byte) ECameraViewMode.FirstPerson;

                _motors.ActionManager.SetActionCode(CameraActionType.Leave, SubCameraMotorType.View, oldMotor.ModeId);
                _motors.ActionManager.SetActionCode(CameraActionType.Enter, SubCameraMotorType.View,
                    (int) ECameraViewMode.ThirdPerson);
                _motors.ActionManager.OnAction(player, _state);
            }
        }

        protected override void ExecWhenNormal(PlayerEntity player, IUserCmd cmd)
        {
            _cmdSeq = cmd.Seq;
            
            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;

            var result = player.cameraStateOutputNew;
            
            CalcuForNormal(cmd, player, result);

            CopyStateToUploadComponent(player.cameraStateNew, player.cameraStateUpload);
        }

        private void CalcuForNormal(IUserCmd cmd, PlayerEntity player, CameraStateOutputNewComponent result)
        {
            DummyCameraMotorState.Convert(player.cameraStateNew, _state);

            DummyCameraMotorInput _input = (DummyCameraMotorInput) player.cameraStateNew.CameraMotorInput;

            _input.Generate(player, cmd, _state, LockView);

            if (!HasConfigInitialed(_input))
                return;

            for (int i = 0; i < (int) SubCameraMotorType.End; i++)
            {
                var type = (SubCameraMotorType) i;
                SetNextMotor(player, type, _state, _input);
            }

            HandleAction(player);

            _motors.ActionManager.ClearActionCode();

            player.cameraStateNew.CameraMotorInput = player.cameraStateNew.LastCameraMotorInput;
            player.cameraStateNew.LastCameraMotorInput = _input;
            
            player.cameraConfigNow.Config = _input.GetPoseConfig(_state.GetMainMotor().NowMode);
            player.cameraConfigNow.PeekConfig = _input.GetPeekConfig();
            player.cameraConfigNow.DeadConfig = _input.GetDeadConfig();
            
            CalcFinalOutput(player, (DummyCameraMotorInput) player.cameraStateNew.LastCameraMotorInput,
                result,_state);
            
            DummyCameraMotorState.Convert(_state, player.cameraStateNew);
            player.cameraStateNew.CanFire =
                !_state.IsFree() && _input.GetPoseConfig(player.cameraStateNew.MainNowMode).CanFire;
        }

        private void HandleAction(PlayerEntity player)
        {
            player.cameraStateUpload.EnterActionCode = _motors.ActionManager.GetActionCode(CameraActionType.Enter);
            player.cameraStateUpload.LeaveActionCode = _motors.ActionManager.GetActionCode(CameraActionType.Leave);
            _motors.ActionManager.OnAction(player, _state);
        }
        
        private void CopyStateToUploadComponent(CameraStateNewComponent input, CameraStateUploadComponent output)
        {
            output.MainNowMode = input.MainNowMode;
            output.ViewNowMode = input.ViewNowMode;
            output.PeekNowMode = input.PeekNowMode;
            output.FreeNowMode = input.FreeNowMode;
            output.FreeYaw = input.FreeYaw;
            output.FreePitch = input.FreePitch;
            output.CanFire = input.CanFire;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new CameraUpdateSystem(_context, _motors);
        }

        private void CalcFinalOutput(PlayerEntity player, DummyCameraMotorInput input,
            CameraStateOutputNewComponent finalOutput, DummyCameraMotorState state)
        {
            _output.Init();
            _output.ArchorPosition =
                player.cameraArchor.ArchorPosition +
                player.cameraArchor.ArchorTransitionOffsetPosition;
            _output.ArchorEulerAngle = player.cameraArchor.ArchorEulerAngle;

            for (int i = 0; i < (int) SubCameraMotorType.End; i++)
            {
                var type = (SubCameraMotorType) i;
                _output.Append(CalcSubFinalCamera(player, input, state, _motors.GetDict(type), state.Get(type),
                    player.time.ClientTime));
            }

            finalOutput.ArchorPosition = _output.ArchorPosition;
            finalOutput.ArchorEulerAngle = _output.ArchorEulerAngle;
            finalOutput.ArchorOffset = _output.ArchorOffset;
            finalOutput.ArchorPostOffset = _output.ArchorPostOffset;
            finalOutput.EulerAngle = _output.EulerAngle;
            finalOutput.Offset = _output.Offset;
            finalOutput.PostOffset = _output.PostOffset;
            finalOutput.Fov = _output.Fov;
            finalOutput.Far = _output.Far;
            finalOutput.Near = _output.Near;
            finalOutput.NeedDetectDistance = !_output.ForbidDetect;
            
            SmoothFov(player,input.GetViewConfig());
        }

        private DummyCameraMotorOutput CalcSubFinalCamera(PlayerEntity player, ICameraMotorInput input,
            ICameraMotorState state,
            Dictionary<int, ICameraNewMotor> dict, SubCameraMotorState subState, int clientTime)
        {
            _tempOutput.Init();
            if (!dict.ContainsKey(subState.NowMode)) return _tempOutput;
            var oldMotor = dict[subState.LastMode];
            var nowMotor = dict[subState.NowMode];
            nowMotor.CalcOutput(player, input, state, subState, _tempOutput, oldMotor, clientTime);
            Logger.DebugFormat("CalcSubFinalCamera:{0}", nowMotor, subState.NowMode);
            return _tempOutput;
        }

        private ICameraNewMotor SetNextMotor(PlayerEntity player, SubCameraMotorType type,
            ICameraMotorState stat, DummyCameraMotorInput input)
        {
            var dict = _motors.GetDict(type);
            var subState = _state.Get(type);
            if (!dict.ContainsKey(subState.NowMode)) return null;
            var oldMotor = dict[subState.NowMode];

            var excludes = oldMotor.ExcludeNextMotor();
            var nextMotor = oldMotor;
            var orderId = int.MinValue;
            foreach (var motor in dict.Values)
            {
                if (excludes.Contains(motor.ModeId)) continue;
                if (motor.IsActive(input, stat))
                {
                    if (motor.Order > orderId)
                    {
                        nextMotor = motor;
                        orderId = motor.Order;
                    }
                }
            }

            if (nextMotor.ModeId != oldMotor.ModeId || subState.ModeTime == 0)
            {
                Logger.InfoFormat("{0} Leave :{1} To {2} with input{3}", _cmdSeq, oldMotor.ModeId, nextMotor.ModeId,
                    input);
                subState.ModeTime = player.time.ClientTime;
                subState.NowMode = (byte) nextMotor.ModeId;
                subState.LastMode = (byte) oldMotor.ModeId;
                _motors.ActionManager.SetActionCode(CameraActionType.Leave, type, oldMotor.ModeId);
                _motors.ActionManager.SetActionCode(CameraActionType.Enter, type, nextMotor.ModeId);
            }

            if (type == SubCameraMotorType.View)
            {
                if (CanChangeViewMotor(input))
                {
                    UpdateOrderViewMode(player, nextMotor.ModeId);
                }
            }
            
            return oldMotor;
        }

        private bool CanChangeViewMotor(ICameraMotorInput input)
        {
            return input.ChangeCamera ;
        }
        
        private void UpdateOrderViewMode(PlayerEntity player,short modeId)
        {
            player.gamePlay.LastViewModeByCmd = modeId;
        }
        
        public void OnRender()
        {
            var player = _playerContext.flagSelfEntity;
            if (player == null) return;
            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;
            if (player.gamePlay.IsObserving()) return;
            CalcFinalOutput(player, (DummyCameraMotorInput) player.cameraStateNew.LastCameraMotorInput,
                player.cameraStateOutputNew,_state);
        }
        
        protected static float CalcuPercent(int clientTime, int startTime, float period)
        {
            var result = period > 0f ? (clientTime - startTime) / period : 0f;
            result = result > 1f ? 1 : result;
            result = result < 0f ? 0 : result;
            return result;
        }
        
        private float lastAimFov ;
        private float lastFov ;
        private int lastVeryTime ;
        private int lastTransitionTime = 200;
        private void SmoothFov(PlayerEntity player, ViewCameraConfig viewConfig)
        {
            var comp = player.cameraStateOutputNew;
            lastAimFov = lastAimFov == 0 ? comp.Fov : lastAimFov;
            if (comp.Fov != lastFov)
            {
                if (player.time.ClientTime - lastVeryTime < lastTransitionTime)
                {
                    lastAimFov = player.cameraStateOutputNew.Fov;
                }
                lastVeryTime = player.time.ClientTime;
            }
            lastVeryTime = comp.Fov == lastFov ? lastVeryTime : player.time.ClientTime;
            lastFov = comp.Fov;
            var period = UpdateViewModeTransitionTime(player, viewConfig);
            var percent = CalcuPercent(player.time.ClientTime, lastVeryTime, period);
            var result = Mathf.Lerp(lastAimFov, comp.Fov, percent);
            if (result == comp.Fov)
            {
                lastAimFov = result;
            }
            comp.Fov = result;
        }

        private bool HasConfigInitialed(DummyCameraMotorInput input)
        {
            return input.GetPoseConfig(0) != null;
        }
        
        private int UpdateViewModeTransitionTime(PlayerEntity player, ViewCameraConfig config)
        {
            if (!player.hasOxygenEnergyInterface)
                return config.DefaltFovTransitionTime;
            bool isGunSight = _state.Dict[(int) SubCameraMotorType.View].NowMode == (byte) ECameraViewMode.GunSight;
            bool isHoldBreath = player.oxygenEnergyInterface.Oxygen.InShiftState;
            bool isOffGunSight = _state.Dict[(int) SubCameraMotorType.View].LastMode == (byte) ECameraViewMode.GunSight;

            if (isGunSight)
            {
                if (isHoldBreath)
                    return config.OnHoldBreathTransitionTime;
                if(player.oxygenEnergyInterface.Oxygen.ShiftVeryTime> _state.Dict[(int)SubCameraMotorType.View].ModeTime)
                    return config.OffHoldBreathTransitionTime;
                return SingletonManager.Get<CharacterStateConfigManager>()
                    .GetPostureTransitionTime(PostureInConfig.Null, PostureInConfig.Sight);
            }
            
            if (isOffGunSight)
                return SingletonManager.Get<CharacterStateConfigManager>()
                    .GetPostureTransitionTime(PostureInConfig.Sight, PostureInConfig.Null);
            
            return config.DefaltFovTransitionTime;
        }
    }
}