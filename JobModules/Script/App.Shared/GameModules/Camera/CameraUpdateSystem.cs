using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Motor.Pose;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Vehicle;
using Assets.App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Camera.Motor.Free;
using BehaviorDesigner.Runtime.Tasks;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.CameraControl.NewMotor;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;
using Type = Google.Protobuf.WellKnownTypes.Type;

namespace App.Shared.GameModules.Camera
{
    class CameraUpdateSystem : IUserCmdExecuteSystem, ISimpleParallelUserCmdExecuteSystem, IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraUpdateSystem));

        private Motors _motors;

        private DummyCameraMotorState _state;
        private DummyCameraMotorState _dummyState;

        private VehicleContext _vehicleContext;

        private FreeMoveContext _freeMoveContext;
        private PlayerContext _playerContext;
        private int _cmdSeq = 0;

        DummyCameraMotorOutput _output = new DummyCameraMotorOutput();
        DummyCameraMotorOutput _tempOutput = new DummyCameraMotorOutput();
        private Contexts _context;

        public CameraUpdateSystem(Contexts context, Motors m)
        {
            _context = context;
            _vehicleContext = context.vehicle;
            _freeMoveContext = context.freeMove;
            _playerContext = context.player;
            _motors = m;
            _state = new DummyCameraMotorState(_motors);
            _dummyState = new DummyCameraMotorState(_motors);
        }


        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            _cmdSeq = cmd.Seq;
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;

            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;
            if (player.gamePlay.IsObserving()) return;

            var archotRotation = player.cameraArchor.ArchorEulerAngle;
            var result = player.cameraStateOutputNew;
            
            CalcuForNormal(cmd, player, archotRotation, result);

            DummyCameraMotorState.Convert(_state, player.cameraStateNew);

            CopyStateToUploadComponent(player.cameraStateNew, player.cameraStateUpload);
            player.cameraStateUpload.ArchorType = (Byte) player.cameraArchor.ArchorType;

            if (player.appearanceInterface.Appearance.IsFirstPerson && player.gamePlay.BeingObserved)
            {
                CalcuForObserving(cmd, player, archotRotation, player.thirdPersonDataForObserving.ThirdPersonData);
            }
        }

        private void CalcuForNormal(IUserCmd cmd, PlayerEntity player, Vector3 archotRotation,
            CameraStateOutputNewComponent result)
        {
            DummyCameraMotorState.Convert(player.cameraStateNew, _state);
            
            DummyCameraMotorInput _input = (DummyCameraMotorInput) player.cameraStateNew.CameraMotorInput;
            _input.Generate(_context, player, cmd, archotRotation.y, archotRotation.x);
            
            for (int i = 0; i < (int) SubCameraMotorType.End; i++)
            {
                var type = (SubCameraMotorType) i;
                SetNextMotor(player, type, _state, _input);
            }

            CameraActionManager.OnAction(player, _state);

            player.cameraStateUpload.EnterActionCode = CameraActionManager.GetActionCode(CameraActionType.Enter);
            player.cameraStateUpload.LeaveActionCode = CameraActionManager.GetActionCode(CameraActionType.Leave);
            CameraActionManager.ClearActionCode();

            player.cameraStateNew.CameraMotorInput = player.cameraStateNew.LastCameraMotorInput;
            player.cameraStateNew.LastCameraMotorInput = _input;

            CalcFinalOutput(player, (DummyCameraMotorInput) player.cameraStateNew.LastCameraMotorInput,
                result,_state);
        }

        private void CalcuForObserving(IUserCmd cmd, PlayerEntity player, Vector3 archotRotation,
            CameraStateOutputNewComponent result)
        {
            DummyCameraMotorState.Convert(player.cameraStateNew, _dummyState);
            _dummyState.Dict[(int) SubCameraMotorType.View].NowMode = (byte)ECameraViewMode.ThirdPerson;
            _dummyState.Dict[(int) SubCameraMotorType.View].LastMode = (byte)ECameraViewMode.FirstPerson;
            DummyCameraMotorInput _input = (DummyCameraMotorInput) player.cameraStateNew.CameraMotorInput;
            _input.Generate(_context, player, cmd, archotRotation.y, archotRotation.x);
            CalcFinalOutput(player, _input, result, _dummyState);
            result.ArchorPosition = player.thirdPersonDataForObserving.ThirdPersonArchorPosition;
        }
        
        private void CopyStateToUploadComponent(CameraStateNewComponent input, CameraStateUploadComponent output)
        {
            output.MainNowMode = input.MainNowMode;
            output.MainLastMode = input.MainLastMode;
            output.MainModeTime = input.MainModeTime;
            output.ViewNowMode = input.ViewNowMode;
            output.ViewLastMode = input.ViewLastMode;
            output.ViewModeTime = input.ViewModeTime;
            output.PeekNowMode = input.PeekNowMode;
            output.PeekLastMode = input.PeekLastMode;
            output.PeekModeTime = input.PeekModeTime;
            output.FreeNowMode = input.FreeNowMode;
            output.FreeLastMode = input.FreeLastMode;
            output.FreeModeTime = input.FreeModeTime;
            output.FreeYaw = input.FreeYaw;
            output.FreePitch = input.FreePitch;
            output.LastFreeYaw = input.LastFreeYaw;
            output.LastFreePitch = input.LastFreePitch;
            output.LastPeekPercent = input.LastPeekPercent;
            output.CanFire = input.CanFire;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new CameraUpdateSystem(_context, _motors);
        }

        private void CalcFinalOutput(PlayerEntity player, DummyCameraMotorInput input,
            CameraStateOutputNewComponent finalOutput, DummyCameraMotorState state)
        {
            player.cameraConfigNow.Config = state.GetMainConfig();
            player.cameraConfigNow.PeekConfig = SingletonManager.Get<CameraConfigManager>().Config.PeekConfig;
            player.cameraConfigNow.DeadConfig = SingletonManager.Get<CameraConfigManager>().Config.DeadConfig;
            _output.Init();
            _output.ArchorPosition =
                player.cameraArchor.ArchorPosition+
                player.cameraArchor.ArchorTransitionOffsetPosition;
            _output.ArchorEulerAngle = player.cameraArchor.ArchorEulerAngle;

            for(int i=0;i<(int)SubCameraMotorType.End;i++)
            {
                var type = (SubCameraMotorType)i;
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
            
            SmoothFov(player);
            
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
            ICameraMotorState stat, DummyCameraMotorInput input
        )
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
                Logger.DebugFormat("{0} Levae :{1} To {2} with input{3}", _cmdSeq, oldMotor.ModeId, nextMotor.ModeId,
                    input);

                CameraActionManager.SetActionCode(CameraActionType.Leave, type, oldMotor.ModeId);
                CameraActionManager.SetActionCode(CameraActionType.Enter, type, nextMotor.ModeId);

                subState.NowMode = (byte) nextMotor.ModeId;
                subState.ModeTime = player.time.ClientTime;
                subState.LastMode = (byte) oldMotor.ModeId;
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
        private void SmoothFov(PlayerEntity player)
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
            var period = UpdateTransitionTime(player);
            var percent = CalcuPercent(player.time.ClientTime, lastVeryTime, period);
            var result = Mathf.Lerp(lastAimFov, comp.Fov, percent);
            if (result == comp.Fov)
            {
                lastAimFov = result;
            }
            comp.Fov = result;
        }


        private readonly int OnGunSightTransitionTime = SingletonManager.Get<CharacterStateConfigManager>()
            .GetPostureTransitionTime(PostureInConfig.Null, PostureInConfig.Sight);
        private readonly int OffGunSightTranstitionTime = SingletonManager.Get<CharacterStateConfigManager>()
            .GetPostureTransitionTime(PostureInConfig.Sight, PostureInConfig.Null);
        private readonly int OnHoldBreathTransitionTime = CameraUtility._manager.Config.ViewConfig.OnHoldBreathTransitionTime;
        private readonly int OffHoldBreathTransitionTime = CameraUtility._manager.Config.ViewConfig.OffHoldBreathTransitionTime;
        private readonly int DefaultFovTransitionTime = CameraUtility._manager.Config.ViewConfig.DefaltFovTransitionTime;
        private int UpdateTransitionTime(PlayerEntity player)
        {
            bool isGunSight = _state.Dict[(int) SubCameraMotorType.View].NowMode == (byte) ECameraViewMode.GunSight;
            bool isHoldBreath = player.oxygenEnergyInterface.Oxygen.InShiftState;
            bool isOffGunSight = _state.Dict[(int) SubCameraMotorType.View].LastMode == (byte) ECameraViewMode.GunSight;

            if (isGunSight)
            {
                if (isHoldBreath)
                    return OnHoldBreathTransitionTime;
                if(player.oxygenEnergyInterface.Oxygen.ShiftVeryTime> _state.Dict[(int)SubCameraMotorType.View].ModeTime)
                    return OffHoldBreathTransitionTime;
                return OnGunSightTransitionTime;
            }
            if (isOffGunSight)
                return OffGunSightTranstitionTime;
            
            return DefaultFovTransitionTime;
        }
    }
}