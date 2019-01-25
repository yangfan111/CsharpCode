using Core.CameraControl;
using Core.GameInputFilter;
using Core.ObjectPool;

namespace Core.Prediction.UserPrediction.Cmd
{

    public interface IUserCmd: IVariableCameraInput, ICmd, IRefCounter
    {
        bool PredicatedOnce { get; set; }
        bool NeedStepPredication { get; set; }

        int RenderTime { get; set; }

        int ClientTime { get; set; }
        
        float MoveHorizontal { get; set; }

        float MoveVertical { get; set; }

        float MoveUpDown { get; set; }



        float Roll { get; set; }


        bool IsJump { get; set; }

        bool IsLeftAttack { get; set; }

        bool IsRun { get; set; }

        /// <summary>
        /// 轻走
        /// </summary>
        bool IsSlightWalk { get; set; }

        bool IsCrouch { get; set; }

        bool IsProne { get; set; }

        /// <summary>
        /// 进入受伤和游泳状态,这个状态需要服务器发送过来，而不是cmd
        /// 状态被设置
        /// </summary>
        int BeState { get; set; }

        int SwitchNumber { get; set; }

        bool IsReload { get; set; }

        bool IsPeekLeft { get; set; }

        bool IsPeekRight { get; set; }

        bool IsSwitchWeapon { get; set; }

        /// <summary>
        /// 按住F键
        /// </summary>
        bool IsF { get; set; }

        bool IsPDown { get; set; }

        bool IsYDown { get; set; }
        
        bool IsCDown { get; set; }
        
        bool IsSpaceDown { get; set; }

        bool IsTabDown { get; set; }

        int ChangedSeat { get; set; }

        int ChangeChannel { get; set; }

        int UseType { get; set; }
        int UseEntityId { get; set; }
        int UseVehicleSeat { get; set; }
        
        bool IsCameraFocus { get; set; }

        bool ChangeCamera { get; set; }

        int SnapshotId { get; set; }
        
        bool IsRightAttack { get; set; }
        int Buttons { get; set; }
        bool IsSwitchFireMode { get; set; }

        int CurWeapon { get; set; }
        bool IsDropWeapon { get; set; }
        bool IsDrawWeapon { get; set; }
        int PickUpEquip { get; set; }
        int PickUpEquipItemId { get; set; }
        int PickUpEquipItemCount { get; set; }

        bool IsThrowing { get; set; }

        bool IsAddMark { get; set; }
        bool IsHoldBreath { get; set; }

        bool IsSwitchAutoRun { get; set; }
        bool IsManualPickUp { get; set; }
        int BagIndex { get; set; }
        bool IsInterrupt { get; set; }

        IFilteredInput FilteredInput { get; set; }

        /// <summary>
        /// 触发提示操作
        /// </summary>
        bool IsUseAction { get; set; }

        /// <summary>
        /// 卸载武器，无动作
        /// </summary>
        bool IsForceUnmountWeapon{ get; set; }
    }
}