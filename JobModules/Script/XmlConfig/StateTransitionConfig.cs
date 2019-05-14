using System.Collections.Generic;

namespace XmlConfig
{
    public enum EPlayerInputGroup
    {
        SlightWalk,
        Length,
        
    }
    public enum EPlayerInput
    {
        None,
        IsLeftAttack,
        IsRightAttack,
        IsRun,
        IsSprint,
        IsCrouch,
        IsProne,
        IsReload,
        IsPeekLeft,
        IsPeekRight,
        IsSwitchWeapon,
        IsSlightWalk,
        IsJump,
        IsCameraFocus,
        IsCameraFree,
        ChangeCamera,
        IsSwitchFireMode,
        IsDropWeapon,
        IsDrawWeapon,
        IsThrowing,
        IsThrowingInterrupt,

        IsPullboltInterrupt,
        MeleeAttack,
        IsUseAction,
        IsAutoReload,
        IsAutoFire,
        Length,
    }

    public class EPlayerInputComparer : IEqualityComparer<EPlayerInput>
    {
        public bool Equals(EPlayerInput x, EPlayerInput y)
        {
            return x == y;
        }

        public int GetHashCode(EPlayerInput obj)
        {
            return (int)obj;
        }

        private static EPlayerInputComparer _instance = new EPlayerInputComparer();
        public static EPlayerInputComparer Instance
        {
            get
            {
                return _instance;
            }
        }



    }
    public class EPlayerStateComparer : IEqualityComparer<EPlayerState>
    {
        public bool Equals(EPlayerState x, EPlayerState y)
        {
            return x == y;
        }

        public int GetHashCode(EPlayerState obj)
        {
            return (int)obj;
        }

        private static EPlayerStateComparer _instance = new EPlayerStateComparer();
        public static EPlayerStateComparer Instance
        {
            get
            {
                return _instance;
            }
        }



    }

    public enum EPlayerState
    {
        None,
        Stand,
        Crouch,
        Climb,
        Prone,
        Run,
        Injured,
        Jump,
        Sprint,
        Swim,
        Drive,
        Grenade,
        Gliding,
        Parachuting,
        Dying,
        Dead,
        Curing,
        OnAir,
        Dive,
        Firing,
        Sight,
        Reload,
        SpecialReload,
        PullBolt,
        Pickup,
        Rescue,
        Droping,
        SwitchWeapon,
        DrawWeapon,
        UndrawWeapon,
        MeleeAttacking,
        PeekLeft,
        PeekRight,
        Land,
        ProneMove,
        Walk,
        PostureTrans,
        RunDebuff,
        CameraFree,
        Props,
        Length,
        //运行时手动生成
        Move,
        OpenUI,
        WeaponRotState,
        PullBoltInterrupt,
        PaintDisc,
        Ladder
    }

    public class StateTransitionConfig
    {
        public StateTransitionConfigItem[] Items;
    }
    
    public class StateTransitionConfigItem
    {
        public EPlayerState State;
        public bool IsLeftAttack;
        public bool IsRightAttack;
        public bool IsRun;
        public bool IsSprint;
        public bool IsCrouch;
        public bool IsProne;
        public bool IsReload;
        public bool IsPeekLeft;
        public bool IsPeekRight;
        public bool IsSwitchWeapon;
        public bool IsSlightWalk;
        public bool IsJump;
        public bool IsCameraFocus;
        public bool IsCameraFree;
        public bool ChangeCamera;
        public bool IsSwitchFireMode;
        public bool IsDropWeapon;
        public bool IsDrawWeapon;
        public bool IsThrowing;
        public bool MeleeAttack;
        public bool IsUseAction;
        public bool IsPullboltInterrupt;
    }
}
