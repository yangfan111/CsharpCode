using System.Collections.Specialized;
using Core.GameInputFilter;
using Core.ObjectPool;

namespace Core.Prediction.UserPrediction.Cmd
{
     
    public class EUserCmdFlags
    {
        public const int IsRun = 1;
        public const int IsCrouch = 1 << (int)UserCmdEnum.IsCrouch;
        public const int IsProne = 1 << (int)UserCmdEnum.IsProne;
        public const int IsSlightWalk = 1 << (int)UserCmdEnum.IsSlightWalk;
        public const int IsCameraFree = 1 << (int)UserCmdEnum.IsCameraFree;
        public const int IsLeftAttack = 1 << (int)UserCmdEnum.IsLeftAttack;
        public const int IsRightAttack = 1 << (int)UserCmdEnum.IsRightAttack;
        public const int IsJump = 1 << (int)UserCmdEnum.IsJump;
        public const int IsF = 1 << (int)UserCmdEnum.IsF;
        public const int IsCameraFocus = 1 << (int)UserCmdEnum.IsCameraFocus;
        public const int ChangeCamera = 1 << (int)UserCmdEnum.ChangeCamera;
        public const int IsSwitchFireMode = 1 << (int)UserCmdEnum.IsSwitchFireMode;
        public const int IsReload = 1 << (int)UserCmdEnum.IsReload;
        public const int IsPeekLeft = 1 << (int)UserCmdEnum.IsPeekLeft;
        public const int IsPeekRight = 1 << (int)UserCmdEnum.IsPeekRight;
        public const int IsSwitchWeapon = 1 << (int)UserCmdEnum.IsSwitchWeapon;
        public const int IsDropWeapon = 1 << (int)UserCmdEnum.IsDropWeapon;
        public const int IsYDown = 1 << (int)UserCmdEnum.IsYDown;
        public const int IsPDown = 1 << (int)UserCmdEnum.IsPDown;
        public const int IsCDown = 1 << (int)UserCmdEnum.IsCDown;
        public const int IsSpaceDown = 1 << (int)UserCmdEnum.IsSpaceDown;
        public const int IsStopFire = 1 << (int)UserCmdEnum.IsStopFire;
        public const int IsDrawWeapon = 1 << (int)UserCmdEnum.IsDrawWeapon;
        public const int IsTabDown = 1 << (int)UserCmdEnum.IsTabDown;
        public const int IsThrowing = 1 << (int)UserCmdEnum.IsThrowing;
        public const int IsAddMark = 1 << (int)UserCmdEnum.IsAddMark;
        public const int IsHoldBreath = 1 << (int)UserCmdEnum.IsHoldBreath;
        public const int IsSwitchAutoRun = 1 << (int)UserCmdEnum.IsSwitchAutoRun;
        public const int IsPickUp = 1 << (int)UserCmdEnum.IsPickUp;
        public const int IsUseAction = 1 << (int)UserCmdEnum.IsHoldF;
        public const int IsForceUnmountWeapon = 1 << (int)UserCmdEnum.IsForceUnmountWeapon;
        /// <summary>
        /// 客户端发起的打断状态，比如打开某些界面时需要终止当前动作
        /// </summary>
        public const int IsInInterruptState = 1 << (int)UserCmdEnum.IsInterrupt;
    }

    public class UserCmd : BaseRefCounter, IUserCmd
    {
        protected UserCmd()
        {
        }

        private BitVector32 _flags = new BitVector32();
        
        public int FrameInterval { get; set; }
        public bool NeedStepPredication { get; set; }
        public int RenderTime { get; set; }
        public int ClientTime { get; set; }
        public bool PredicatedOnce { get; set; }
        public int Seq { get; set; }
        public int SnapshotId { get; set; }
        public float MoveHorizontal { get; set; }
        public float MoveVertical { get; set; }
        public float MoveUpDown { get; set; }
        public int CurWeapon { get; set; }
        public int Buttons
        {
            get { return _flags.Data; }
            set { _flags =new BitVector32(value); }
        }

        public bool IsSwitchFireMode
        {
            get { return _flags[EUserCmdFlags.IsSwitchFireMode]; }
            set { _flags[EUserCmdFlags.IsSwitchFireMode] = value; }

        }
        public bool IsRun
        {
            get { return _flags[ EUserCmdFlags.IsRun]; }
            set { _flags[ EUserCmdFlags.IsRun] = value; }
        }

        public bool IsCrouch
        {
            get { return _flags[EUserCmdFlags.IsCrouch]; }
            set { _flags[EUserCmdFlags.IsCrouch] = value; }
        }
        public bool IsProne
        {
            get { return _flags[EUserCmdFlags.IsProne]; }
            set { _flags[EUserCmdFlags.IsProne] = value; }
        }
        public bool IsSlightWalk
        {
            get { return _flags[EUserCmdFlags.IsSlightWalk]; }
            set { _flags[EUserCmdFlags.IsSlightWalk] = value; }
        }
        public bool IsCameraFree
        {
            get { return _flags[EUserCmdFlags.IsCameraFree]; }
            set { _flags[EUserCmdFlags.IsCameraFree] = value; }
        }
        public bool IsLeftAttack
        {
            get { return _flags[EUserCmdFlags.IsLeftAttack]; }
            set { _flags[EUserCmdFlags.IsLeftAttack] = value; }
        }
        public bool IsRightAttack
        {
            get { return _flags[EUserCmdFlags.IsRightAttack]; }
            set { _flags[EUserCmdFlags.IsRightAttack] = value; }
        }
        public bool IsJump
        {
            get { return _flags[EUserCmdFlags.IsJump]; }
            set { _flags[EUserCmdFlags.IsJump] = value; }
        }
        public bool IsF
        {
            get { return _flags[EUserCmdFlags.IsF]; }
            set { _flags[EUserCmdFlags.IsF] = value; }
        }

        public bool IsCameraFocus
        {
            get { return _flags[EUserCmdFlags.IsCameraFocus]; }
            set { _flags[EUserCmdFlags.IsCameraFocus] = value; }
        }

        public bool IsDrawWeapon
        {
            get { return _flags[EUserCmdFlags.IsDrawWeapon]; }
            set { _flags[EUserCmdFlags.IsDrawWeapon] = value; }
        }

        public bool IsForceUnmountWeapon
        {
            get { return _flags[EUserCmdFlags.IsForceUnmountWeapon]; }
            set { _flags[EUserCmdFlags.IsForceUnmountWeapon] = value; }
        }

        public bool ChangeCamera
        {
            get { return _flags[EUserCmdFlags.ChangeCamera]; }
            set { _flags[EUserCmdFlags.ChangeCamera] = value; }
        }

        public int BeState { get; set; }
        public int SwitchNumber { get; set; }
        public int BagIndex { get; set; }
        public bool IsReload
		{
			get { return _flags[EUserCmdFlags.IsReload]; }
			set { _flags[EUserCmdFlags.IsReload] = value; }
		}
        public bool IsPeekLeft
		{
			get { return _flags[EUserCmdFlags.IsPeekLeft]; }
			set { _flags[EUserCmdFlags.IsPeekLeft] = value; }
		}	
        public bool IsPeekRight
		{
			get { return _flags[EUserCmdFlags.IsPeekRight]; }
			set { _flags[EUserCmdFlags.IsPeekRight] = value; }
		}
        public bool IsSwitchWeapon
        {
            get { return _flags[EUserCmdFlags.IsSwitchWeapon]; }
            set { _flags[EUserCmdFlags.IsSwitchWeapon] = value; }
        }

        public bool IsDropWeapon
        {
            get { return _flags[EUserCmdFlags.IsDropWeapon]; }
            set { _flags[EUserCmdFlags.IsDropWeapon] = value; }
        }

        public bool IsPDown
        {
            get { return _flags[EUserCmdFlags.IsPDown]; }
            set { _flags[EUserCmdFlags.IsPDown] = value; }
        }

        public bool IsYDown
        {
            get { return _flags[EUserCmdFlags.IsYDown]; }
            set { _flags[EUserCmdFlags.IsYDown] = value; }
        }
        
        public bool IsCDown
        {
            get { return _flags[EUserCmdFlags.IsCDown]; }
            set { _flags[EUserCmdFlags.IsCDown] = value; }
        }
        
        public bool IsSpaceDown
        {
            get { return _flags[EUserCmdFlags.IsSpaceDown]; }
            set { _flags[EUserCmdFlags.IsSpaceDown] = value; }
        }

        public bool IsTabDown
        {
            get { return _flags[EUserCmdFlags.IsTabDown]; }
            set { _flags[EUserCmdFlags.IsTabDown] = value; }
        }

        public bool IsThrowing
        {
            get { return _flags[EUserCmdFlags.IsThrowing]; }
            set { _flags[EUserCmdFlags.IsThrowing] = value; }
        }

        public bool IsAddMark
        {
            get { return _flags[EUserCmdFlags.IsAddMark]; }
            set { _flags[EUserCmdFlags.IsAddMark] = value; }
        }

        public bool IsHoldBreath
        {
            get { return _flags[EUserCmdFlags.IsHoldBreath]; }
            set { _flags[EUserCmdFlags.IsHoldBreath] = value; }
        }

        public bool IsSwitchAutoRun { get { return _flags[EUserCmdFlags.IsSwitchAutoRun]; } set { _flags[EUserCmdFlags.IsSwitchAutoRun] = value; } } 
        public bool IsManualPickUp { get { return _flags[EUserCmdFlags.IsPickUp]; } set { _flags[EUserCmdFlags.IsPickUp] = value; } } 
        public bool IsInterrupt { get { return _flags[EUserCmdFlags.IsInInterruptState]; } set { _flags[EUserCmdFlags.IsInInterruptState] = value; } } 
        public bool IsUseAction { get { return _flags[EUserCmdFlags.IsUseAction];} set { _flags[EUserCmdFlags.IsUseAction] = value; } }

        public float DeltaYaw { get; set; }
        public float DeltaPitch { get; set; }
        public float Roll { get; set; }
        public int ChangedSeat { get; set; }  
        public int ChangeChannel { get; set; }
        public int PickUpEquip { get; set; }
        public int PickUpEquipItemId { get; set; }
        public int PickUpEquipItemCount { get; set; }

        public IFilteredInput FilteredInput { get; set; }
        public int UseEntityId { get; set; }
        public int UseVehicleSeat { get; set; }
        public int UseType { get; set; }

        public static UserCmd Allocate()
        {
            return ObjectAllocatorHolder<UserCmd>.Allocate();
        }
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(UserCmd)){}
            public override object MakeObject()
            {
                return new UserCmd();
            }
        }
        public void CopyTo(UserCmd cmd)
        {
            cmd.FrameInterval = FrameInterval;
            cmd.RenderTime = RenderTime;
            cmd.ClientTime = ClientTime;
            cmd.PredicatedOnce = PredicatedOnce;
            cmd.Seq = Seq;
            cmd.MoveHorizontal = MoveHorizontal;
            cmd.IsLeftAttack = IsLeftAttack;
            cmd.IsRightAttack = IsRightAttack;
            cmd.IsRun = IsRun;
            cmd.IsCrouch = IsCrouch;
            cmd.IsProne = IsProne;
            cmd.BeState = BeState;
            cmd.SwitchNumber = SwitchNumber;
            cmd.IsReload = IsReload;
            cmd.IsPeekLeft = IsPeekLeft;
            cmd.IsPeekRight = IsPeekRight;
            cmd.IsSwitchWeapon = IsSwitchWeapon;
            cmd.IsSlightWalk = IsSlightWalk;
            cmd.MoveVertical = MoveVertical;
            cmd.MoveUpDown = MoveUpDown;
            cmd.DeltaYaw = DeltaYaw;
            cmd.DeltaPitch = DeltaPitch;
            cmd.Roll = Roll;
            cmd.IsJump = IsJump;
            cmd.IsF = IsF;
            cmd.ChangedSeat = ChangedSeat;
            cmd.ChangeChannel = ChangeChannel;
            cmd.IsCameraFocus = IsCameraFocus;
            cmd.IsCameraFree = IsCameraFree;
            cmd.ChangeCamera = ChangeCamera;
            cmd.SnapshotId = SnapshotId;
            cmd.IsSwitchFireMode = IsSwitchFireMode;
            cmd.CurWeapon = CurWeapon;
            cmd.IsDropWeapon = IsDropWeapon;
            cmd.PickUpEquip = PickUpEquip;
            cmd.PickUpEquipItemId = PickUpEquipItemId;
            cmd.PickUpEquipItemCount = PickUpEquipItemCount;
            cmd.IsPDown = IsPDown;
            cmd.IsYDown = IsYDown;
            cmd.IsCDown = IsCDown;
            cmd.IsSpaceDown = IsSpaceDown;
            cmd.IsDrawWeapon = IsDrawWeapon;
            cmd.IsTabDown = IsTabDown;
            cmd.IsThrowing = IsThrowing;
            cmd.IsAddMark = IsAddMark;
            cmd.IsHoldBreath = IsHoldBreath;
            cmd.IsSwitchAutoRun = IsSwitchAutoRun;
            cmd.IsManualPickUp = IsManualPickUp;
            cmd.UseEntityId = UseEntityId;
            cmd.UseVehicleSeat = UseVehicleSeat;
            cmd.UseType = UseType;
            cmd.BagIndex = BagIndex;
            cmd.IsInterrupt = IsInterrupt;
            cmd.IsUseAction = IsUseAction;
            cmd.IsForceUnmountWeapon = IsForceUnmountWeapon;
        }

        public void Reset()
        {
            FrameInterval = 0;
            RenderTime = 0;
            ClientTime = 0;
            PredicatedOnce = false;
            Seq = 0;
            MoveHorizontal = 0;
            IsLeftAttack = false;
            IsRightAttack = false;
            IsRun = false;
            IsCrouch = false;
            IsProne = false;
            BeState = 0;
            SwitchNumber = -1;
            IsReload = false;
            IsPeekLeft = false;
            IsPeekRight = false;
            IsSwitchWeapon = false;
            IsSlightWalk = false;
            MoveVertical = 0;
            MoveUpDown = 0;
            DeltaYaw = 0;
            DeltaPitch = 0;
            Roll = 0;
            IsJump = false;
            IsF = false;
            ChangedSeat = 0;
            IsCameraFocus = false;
            ChangeCamera = false;
            IsCameraFree = false;
            SnapshotId = 0;
            IsSwitchFireMode = false;
            IsDropWeapon = false;
            CurWeapon = 0;
            PickUpEquip = 0;
            PickUpEquipItemId = 0;
            PickUpEquipItemCount = 0;
            IsPDown = false;
            IsYDown = false;
            IsCDown = false;
            IsSpaceDown = false;
            IsDrawWeapon = false;
            IsTabDown = false;
            IsThrowing = false;
            IsAddMark = false;
            IsHoldBreath = false;
            IsSwitchAutoRun = false;
            IsManualPickUp = false;
            UseEntityId = 0;
            UseVehicleSeat = 0;
            UseType = 0;
            BagIndex = 0;
            IsInterrupt = false;
            IsUseAction = false;
            IsForceUnmountWeapon = false;
        }

        protected override void OnCleanUp()
        {
            Reset();
            ObjectAllocatorHolder<UserCmd>.Free(this);
        }

        public override string ToString()
        {
            return string.Format("{0}, Seq: {1}, SnapshotId: {2}", "UserCmd", Seq, SnapshotId);
        }
    }
}