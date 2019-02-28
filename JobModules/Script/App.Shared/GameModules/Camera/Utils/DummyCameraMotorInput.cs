using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Core.CameraControl;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;
using ICameraMotorInput = Core.CameraControl.NewMotor.ICameraMotorInput;

namespace App.Shared.GameModules.Camera
{
    class DummyCameraMotorInput : ICameraMotorInput
    {
        public void Generate(Contexts contexts, PlayerEntity player, IUserCmd usercmd,float archorYaw,float archorPitch)
        {
            DeltaYaw = usercmd.DeltaYaw;
            DeltaPitch = usercmd.DeltaPitch;
            if (usercmd.FilteredInput != null)
            {
                IsCameraFree = usercmd.FilteredInput.IsInput(EPlayerInput.IsCameraFree);
                FilteredChangeCamera = usercmd.FilteredInput.IsInput(EPlayerInput.ChangeCamera);
                FilteredCameraFocus = usercmd.FilteredInput.IsInput(EPlayerInput.IsCameraFocus);
            }
            var controller = player.WeaponController();
            FrameInterval = usercmd.FrameInterval;
            ChangeCamera = usercmd.ChangeCamera;
          
            IsCameraFocus = usercmd.IsCameraFocus;
         
            IsCmdRun = usercmd.IsRun;
            IsCmdMoveVertical = usercmd.MoveVertical > 0;
            CurrentPostureState = player.stateInterface.State.GetCurrentPostureState();
            NextPostureState = player.stateInterface.State.GetNextPostureState();
            LeanState = player.stateInterface.State.GetNextLeanState();
            IsAirPlane = player.gamePlay.GameState == GameState.AirPlane;      
            ActionState = player.stateInterface.State.GetActionState();
            ActionKeepState = player.stateInterface.State.GetActionKeepState();
            IsDriveCar =  player.IsOnVehicle();            
            IsDead = player.gamePlay.IsLifeState(EPlayerLifeState.Dead);
            CanWeaponGunSight = controller.HeldWeaponAgent.CanWeaponSight;
            ArchorPitch = YawPitchUtility.Normalize(archorPitch);
            ArchorYaw = YawPitchUtility.Normalize(archorYaw);
            IsParachuteAttached = player.hasPlayerSkyMove && player.playerSkyMove.IsParachuteAttached;
            if(!controller.IsHeldSlotEmpty)
            {
                ForceChangeGunSight = controller.HeldWeaponAgent.RunTimeComponent.ForceChangeGunSight;
            }
            ForceInterruptGunSight = UseActionOrIsStateNoGunSight(usercmd, controller);
            controller.HeldWeaponAgent.RunTimeComponent.ForceChangeGunSight = false;
        }

        private bool UseActionOrIsStateNoGunSight(IUserCmd userCmd, PlayerWeaponController controller)
        {
            var interrupt = userCmd.IsUseAction || userCmd.IsTabDown || userCmd.FilteredInput.IsInputBlocked(EPlayerInput.IsCameraFocus);
            if(!interrupt)
            {
                interrupt |= controller.ForceInterruptGunSight> 0;
            }
            return interrupt;
        }

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

        public bool FilteredChangeCamera
        {
            get;
            set;
        }

        public bool FilteredCameraFocus { get; set; }
        public bool ForceChangeGunSight { get; set; }
        public bool ForceInterruptGunSight { get; set; }

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
                   || CanCameraFocus!= r.ChangeCamera
                   || NextPostureState!=r.NextPostureState
                   || CurrentPostureState!=r.CurrentPostureState
                   || LeanState!=r.LeanState
                   || ActionState!=r.ActionState
                   || IsDriveCar!=r.IsDriveCar
                   || IsAirPlane!=r.IsAirPlane
                   || IsDead!=r.IsDead
                   || CanWeaponGunSight!=r.CanWeaponGunSight
                   || IsCmdRun!=r.IsCmdRun
                   || IsCmdMoveVertical!=r.IsCmdMoveVertical
                   || IsParachuteAttached!=r.IsParachuteAttached
                   || FilteredChangeCamera!=r.FilteredChangeCamera
                   || FilteredCameraFocus!=r.FilteredCameraFocus
                   || ForceChangeGunSight!=r.ForceChangeGunSight
                   || ForceInterruptGunSight!=r.ForceInterruptGunSight
                   || ActionKeepState!=r.ActionKeepState;
                  
        }
    }
}