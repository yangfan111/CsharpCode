using System.Collections.Generic;
using System.Xml.Serialization;

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
        IsF,
        IsActionInterrupt,
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
        GrenadeThrow,
        //运行时手动生成
        Move,
        OpenUI,
        WeaponRotState,
        PullBoltInterrupt,
        PaintDisc,
        Ladder,
        FinalPosing,
        OpenDoor,
        Length
    }
    public class StateTransitionConfig
    {
        public StateTransitionConfigItem[] Items;
    }
    [XmlRoot("root")]
    public class StateInterruptConfig
    {
        public StateInterruptItem[] Items;
    }
    public enum InterruptConfigType
    {
        [XmlEnum(Name = "PullboltInterrupt")]
        PullboltInterrupt,
        [XmlEnum(Name = "ThrowInterrupt")]
        ThrowInterrupt,
        Length,
    }
    public enum Transition
    {
        [XmlEnum(Name = "IsLeftAttack")]
        IsLeftAttack,
        [XmlEnum(Name = "IsRightAttack")]
        IsRightAttack,
        [XmlEnum(Name = "IsRun")]
        IsRun,
        [XmlEnum(Name = "IsSprint")]
        IsSprint,
        [XmlEnum(Name = "IsCrouch")]
        IsCrouch,
        [XmlEnum(Name = "IsProne")]
        IsProne,
        [XmlEnum(Name = "IsReload")]
        IsReload,
        [XmlEnum(Name = "IsPeekLeft")]
        IsPeekLeft,
        [XmlEnum(Name = "IsPeekRight")]
        IsPeekRight,
        [XmlEnum(Name = "IsSwitchWeapon")]
        IsSwitchWeapon,
        [XmlEnum(Name = "IsSlightWalk")]
        IsSlightWalk,
        [XmlEnum(Name = "IsJump")]
        IsJump,
        [XmlEnum(Name = "IsCameraFocus")]
        IsCameraFocus,
        [XmlEnum(Name = "IsCameraFree")]
        IsCameraFree,
        [XmlEnum(Name = "ChangeCamera")]
        ChangeCamera,
        [XmlEnum(Name = "IsSwitchFireMode")]
        IsSwitchFireMode,
        [XmlEnum(Name = "IsDropWeapon")]
        IsDropWeapon,
        [XmlEnum(Name = "IsDrawWeapon")]
        IsDrawWeapon,
        [XmlEnum(Name = "IsThrowing")]
        IsThrowing,
        [XmlEnum(Name = "MeleeAttack")]
        MeleeAttack,
        [XmlEnum(Name = "IsUseAction")]
        IsUseAction,
        [XmlEnum(Name = "IsPullboltInterrupt")]
        IsPullboltInterrupt,
        [XmlEnum(Name = "IsUseItem")]
        IsUseItem,
        // Length
        Length,
    }

    [XmlType("Item")]
    public class StateInterruptItem
    {
        public EPlayerState State;
        public InterruptConfigType[] Interrupts;
    }

    public class StateTransitionConfigItem
    {
        public EPlayerState State;
        public Transition[] Transitions;

        public bool[] vsTransition;

        public bool GetTransition(Transition transition)
        {
            if (null == vsTransition)
                return false;
            return vsTransition[(int)transition];
        }

        //public bool IsLeftAttack;
        //public bool IsRightAttack;
        //public bool IsRun;
        //public bool IsSprint;
        //public bool IsCrouch;
        //public bool IsProne;
        //public bool IsReload;
        //public bool IsPeekLeft;
        //public bool IsPeekRight;
        //public bool IsSwitchWeapon;
        //public bool IsSlightWalk;
        //public bool IsJump;
        //public bool IsCameraFocus;
        //public bool IsCameraFree;
        //public bool ChangeCamera;
        //public bool IsSwitchFireMode;
        //public bool IsDropWeapon;
        //public bool IsDrawWeapon;
        //public bool IsThrowing;
        //public bool MeleeAttack;
        //public bool IsUseAction;
        //public bool IsPullboltInterrupt;
        //public bool IsUseItem;
    }
}
