using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Player;

using Core.CameraControl;
using Core.CameraControl.NewMotor;
using Core.Configuration;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;
using ICameraMotorInput = Core.CameraControl.NewMotor.ICameraMotorInput;

namespace App.Shared.GameModules.Camera
{
    class DummyCameraMotorInput : ICameraMotorInput
    {
        public void Generate(PlayerEntity player, IUserCmd usercmd, ICameraMotorState state, bool lockView)
        {
            var speedRatio = CameraUtility.GetGunSightSpeed(player, state);
            DeltaYaw = usercmd.DeltaYaw * speedRatio;
            DeltaPitch = usercmd.DeltaPitch * speedRatio;
            if (usercmd.FilteredInput != null)
            {
                IsCameraFree = usercmd.FilteredInput.IsInput(EPlayerInput.IsCameraFree);
                FilteredChangeCamera = lockView ? false : usercmd.FilteredInput.IsInput(EPlayerInput.ChangeCamera);
                FilteredCameraFocus = usercmd.FilteredInput.IsInput(EPlayerInput.IsCameraFocus);
            }
            FrameInterval = usercmd.FrameInterval;
            ChangeCamera = lockView ? false :usercmd.ChangeCamera;
            IsCameraFocus = usercmd.IsCameraFocus;
            IsCmdRun = usercmd.IsRun;
            IsCmdMoveVertical = usercmd.MoveVertical > 0;
            InterruptCameraFocus = usercmd.IsUseAction || usercmd.IsTabDown;

            if (player.hasStateInterface)
            {
                CurrentPostureState = player.stateInterface.State.GetCurrentPostureState();
                NextPostureState = player.stateInterface.State.GetNextPostureState();
                LeanState = player.stateInterface.State.GetNextLeanState(); 
                ActionState = player.stateInterface.State.GetActionState();
                ActionKeepState = player.stateInterface.State.GetActionKeepState();
            }
            else if(player.hasThirdPersonAppearance)
            {
                CurrentPostureState = (PostureInConfig) player.thirdPersonAppearance.PostureValue;
                NextPostureState = (PostureInConfig) player.thirdPersonAppearance.NextPostureValue;
                ActionState = (ActionInConfig) player.thirdPersonAppearance.ActionValue;
            }

            IsAirPlane = player.gamePlay.GameState == GameState.AirPlane;     
            IsDriveCar =  player.IsOnVehicle();            
            IsDead = player.gamePlay.IsLifeState(EPlayerLifeState.Dead);
            CanWeaponGunSight =  player.WeaponController().HeldWeaponAgent.CanWeaponSight;
            ArchorPitch = YawPitchUtility.Normalize(player.cameraArchor.ArchorEulerAngle.x);
            ArchorYaw = YawPitchUtility.Normalize(player.cameraArchor.ArchorEulerAngle.y);
            IsParachuteAttached = player.hasPlayerSkyMove && player.playerSkyMove.IsParachuteAttached;
            LastViewByOrder = player.gamePlay.LastViewModeByCmd;
            RoleId = player.playerInfo.RoleModelId;
            LockViewByRoom = lockView;
            ModelLoaded = player.hasFirstPersonModel && player.hasThirdPersonModel;
            IsVariant = JudgeVariant(player);
        }

        public void FakeForObserve(PlayerEntity player)
        {          
            IsDead = player.gamePlay.IsLifeState(EPlayerLifeState.Dead);
            ArchorPitch = YawPitchUtility.Normalize(player.cameraArchor.ArchorEulerAngle.x);
            ArchorYaw = YawPitchUtility.Normalize(player.cameraArchor.ArchorEulerAngle.y);
            RoleId = player.playerInfo.RoleModelId;
        }
        
        private bool JudgeVariant(PlayerEntity player)
        {
            if (null == player || !player.hasPlayerInfo) return false;

            var roleId = player.playerInfo.RoleModelId;
            return SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId).Unique;
        }
        
        public int RoleId { get; set; }
        public bool ModelLoaded { get; set; }
        public bool LockViewByRoom { get; set; }
        public float DeltaYaw { get; set; }
        public float DeltaPitch { get; set; }
        public bool IsCameraFree { get; set; }
        public int FrameInterval { get; set; }
        public bool ChangeCamera { get; set; }
        public bool IsCameraFocus { get; set; }
        public bool CanCameraFocus { get; set; }
        public PostureInConfig NextPostureState { get; set; }
        public PostureInConfig CurrentPostureState { get; set; }
        public LeanInConfig LeanState { get; set; }
        public ActionInConfig ActionState { get; set; }
        public ActionKeepInConfig ActionKeepState { get; set; }
        public bool IsDriveCar { get; set; }
        public bool IsAirPlane { get; set; }
        public bool IsDead { get; set; }
        public bool CanWeaponGunSight { get; set; }
        public bool IsCmdRun { get; set; }
        public bool IsCmdMoveVertical { get; set; }
        public float ArchorYaw { get; set; }
        public float ArchorPitch { get; set; }
        public bool IsParachuteAttached { get; set; }
        public bool IsVariant { get; set; }
        
        public CameraConfigItem Config
        {
            get { return SingletonManager.Get<CameraConfigManager>().GetRoleConfig(RoleId); }
        }

        public PoseCameraConfig GetPoseConfig(int modeId)
        {
            return Config.PoseConfigs[modeId];
        }

        public PeekCameraConfig GetPeekConfig()
        {
            return Config.PeekConfig;
        }

        public FreeCameraConfig GetFreeConfig()
        {
            return Config.FreeConfig;
        }

        public ViewCameraConfig GetViewConfig()
        {
            return Config.ViewConfig;
        }

        public DeadCameraConfig GetDeadConfig()
        {
            return Config.DeadConfig;
        }
        
        public bool FilteredChangeCamera
        {
            get;
            set;
        }

        public bool FilteredCameraFocus { get; set; }
        public bool InterruptCameraFocus { get; set; }
        public short LastViewByOrder { get; set; }
        public override string ToString()
        {
            return string.Format("DeltaYaw: {0}, DeltaPitch: {1}, IsCameraFree: {2}, FrameInterval: {3}, ChangeCamera: {4}, IsCameraFocus: {5}, CanCameraFocus: {6}, NextPostureState: {7}, CurrentPostureState: {8}, LeanState: {9}, ActionState: {10}, IsDriveCar: {11}, IsAirPlane: {12}, IsDead: {13}, CanWeaponGunSight: {14}, IsCmdRun: {15}, IsCmdMoveVertical: {16}, ArchorYaw: {17}, ArchorPitch: {18}", 
                DeltaYaw, DeltaPitch, IsCameraFree, FrameInterval, ChangeCamera, IsCameraFocus, CanCameraFocus, NextPostureState, CurrentPostureState, LeanState, ActionState, IsDriveCar, IsAirPlane, IsDead, CanWeaponGunSight, IsCmdRun, IsCmdMoveVertical, ArchorYaw, ArchorPitch);
        }

        public bool IsChange(ICameraMotorInput r)
        {
            return r == null
                   || IsCameraFree != r.IsCameraFree
                   || ChangeCamera != r.ChangeCamera
                   || IsCameraFocus != r.IsCameraFocus
                   || CanCameraFocus != r.ChangeCamera
                   || NextPostureState != r.NextPostureState
                   || CurrentPostureState != r.CurrentPostureState
                   || LeanState != r.LeanState
                   || ActionState != r.ActionState
                   || IsDriveCar != r.IsDriveCar
                   || IsAirPlane != r.IsAirPlane
                   || IsDead != r.IsDead
                   || CanWeaponGunSight != r.CanWeaponGunSight
                   || IsCmdRun != r.IsCmdRun
                   || IsCmdMoveVertical != r.IsCmdMoveVertical
                   || IsParachuteAttached != r.IsParachuteAttached
                   || FilteredChangeCamera != r.FilteredChangeCamera
                   || FilteredCameraFocus != r.FilteredCameraFocus
                   || InterruptCameraFocus != r.InterruptCameraFocus
                   || ActionKeepState != r.ActionKeepState
                   || LastViewByOrder != r.LastViewByOrder;
        }
    }
}