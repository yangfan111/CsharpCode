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
using Core.CameraControl.NewMotor;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Singleton;
using XmlConfig;
using Type = Google.Protobuf.WellKnownTypes.Type;

namespace App.Shared.GameModules.Camera
{
    class CameraUpdateSystem : IUserCmdExecuteSystem, ISimpleParallelUserCmdExecuteSystem, IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraUpdateSystem));

        private Motors _motors;

        DummyCameraMotorState _state;
        private VehicleContext _vehicleContext;

        private FreeMoveContext _freeMoveContext;
        private PlayerContext _playerContext;
        private int _cmdSeq = 0;
//        private readonly List<SubCameraMotorType> _subCameraMotorTypeArray = new List<SubCameraMotorType>();
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
        }


        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            _cmdSeq = cmd.Seq;
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;

            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;

            DummyCameraMotorState.Convert(player.cameraStateNew, _state);
            DummyCameraMotorInput _input = (DummyCameraMotorInput) player.cameraStateNew.CameraMotorInput;
            var archotRotation = player.cameraArchor.ArchorEulerAngle;
            _input.Generate(player, cmd, archotRotation.y, archotRotation.x);

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
                player.cameraStateOutputNew);
            DummyCameraMotorState.Convert(_state, player.cameraStateNew);

            CopyStateToUploadComponent(player.cameraStateNew, player.cameraStateUpload);
            player.cameraStateUpload.ArchorType = (Byte) player.cameraArchor.ArchorType;
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
            CameraStateOutputNewComponent finalOutput)
        {
            player.cameraConfigNow.Config = _state.GetMainConfig();
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
                _output.Append(CalcSubFinalCamera(player, input, _state, _motors.GetDict(type), _state.Get(type),
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


            return oldMotor;
        }

        public void OnRender()
        {
            var player = _playerContext.flagSelfEntity;
            if (player == null) return;
            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;
            CalcFinalOutput(player, (DummyCameraMotorInput) player.cameraStateNew.LastCameraMotorInput,
                player.cameraStateOutputNew);
            //DummyCameraMotorState.Convert(_state, player.cameraStateNew);
        }
    }
}