/**
 * create by liu 2018/5/8
 */
namespace Core.Prediction.UserPrediction.Cmd
{
    public class UserMoveCmd
    {
        public const int IsIdle = 1 << (int)UserCmdEnum.IsIdle;

        public const int IsSureWalk = 1 << (int)UserCmdEnum.IsSureWalk;
        public const int IsSureRun = 1 << (int)UserCmdEnum.IsSureRun;
        public const int IsSureSprint = 1 << (int)UserCmdEnum.IsSureSprint;

        public const int IsSwim = 1 << (int)UserCmdEnum.IsSwim;

        public const int IsDying = 1 << (int)UserCmdEnum.IsDying;

        public int Buttons = 0;
    }

    public enum UserCmdEnum
    {
        IsRun,
        IsCrouch,
        IsProne,
        IsSlightWalk,
        IsCameraFree,
        IsLeftAttack,
        IsRightAttack,
        IsJump,
        IsF,
        IsCameraFocus,
        ChangeCamera,
        IsSwitchFireMode,
        IsReload,
        IsPeekLeft,
        IsPeekRight,
        IsSwitchWeapon,
        IsDropWeapon,
        IsYDown,
        IsPDown,
        IsCDown,
        IsSpaceDown,
        IsStopFire,
        IsDrawWeapon,
        IsTabDown,
        IsThrowing,
        IsAddMark,
        IsHoldBreath,
        IsSwitchAutoRun,
        IsPickUp,
        IsHoldF,
        IsForceUnmountWeapon,
        IsInterrupt,

        IsIdle, // 
        IsSureWalk,
        IsSureRun,
        IsSureSprint,
        IsSwim,
        IsDying,
    }
}
