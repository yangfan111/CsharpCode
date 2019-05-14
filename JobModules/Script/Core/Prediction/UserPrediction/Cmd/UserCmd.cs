using Core.ObjectPool;
using System;
using System.Collections.Generic;

namespace Core.Prediction.UserPrediction.Cmd
{
    public class EUserCmdFlags
    {
        public const int IsRun                = 0;
        public const int IsCrouch             = /*1 <<*/ (int) UserCmdEnum.IsCrouch;
        public const int IsProne              = /*1 <<*/ (int) UserCmdEnum.IsProne;
        public const int IsSlightWalk         = /*1 <<*/ (int) UserCmdEnum.IsSlightWalk;
        public const int IsCameraFree         = /*1 <<*/ (int) UserCmdEnum.IsCameraFree;
        public const int IsLeftAttack         = /*1 <<*/ (int) UserCmdEnum.IsLeftAttack;
        public const int IsRightAttack        = /*1 <<*/ (int) UserCmdEnum.IsRightAttack;
        public const int IsJump               = /*1 <<*/ (int) UserCmdEnum.IsJump;
        public const int IsF                  = /*1 <<*/ (int) UserCmdEnum.IsF;
        public const int IsCameraFocus        = /*1 <<*/ (int) UserCmdEnum.IsCameraFocus;
        public const int ChangeCamera         = /*1 <<*/ (int) UserCmdEnum.ChangeCamera;
        public const int IsSwitchFireMode     = /*1 <<*/ (int) UserCmdEnum.IsSwitchFireMode;
        public const int IsReload             = /*1 <<*/ (int) UserCmdEnum.IsReload;
        public const int IsPeekLeft           = /*1 <<*/ (int) UserCmdEnum.IsPeekLeft;
        public const int IsPeekRight          = /*1 <<*/ (int) UserCmdEnum.IsPeekRight;
        public const int IsSwitchWeapon       = /*1 <<*/ (int) UserCmdEnum.IsSwitchWeapon;
        public const int IsDropWeapon         = /*1 <<*/ (int) UserCmdEnum.IsDropWeapon;
        public const int IsYDown              = /*1 <<*/ (int) UserCmdEnum.IsYDown;
        public const int IsPDown              = /*1 <<*/ (int) UserCmdEnum.IsPDown;
        public const int IsCDown              = /*1 <<*/ (int) UserCmdEnum.IsCDown;
        public const int IsSpaceDown          = /*1 <<*/ (int) UserCmdEnum.IsSpaceDown;
        public const int IsStopFire           = /*1 <<*/ (int) UserCmdEnum.IsStopFire;
        public const int IsDrawWeapon         = /*1 <<*/ (int) UserCmdEnum.IsDrawWeapon;
        public const int IsTabDown            = /*1 <<*/ (int) UserCmdEnum.IsTabDown;
        public const int IsThrowing           = /*1 <<*/ (int) UserCmdEnum.IsThrowing;
        public const int IsAddMark            = /*1 <<*/ (int) UserCmdEnum.IsAddMark;
        public const int IsHoldBreath         = /*1 <<*/ (int) UserCmdEnum.IsHoldBreath;
        public const int IsSwitchAutoRun      = /*1 <<*/ (int) UserCmdEnum.IsSwitchAutoRun;
        public const int IsPickUp             = /*1 <<*/ (int) UserCmdEnum.IsPickUp;
        public const int IsUseAction          = /*1 <<*/ (int) UserCmdEnum.IsHoldF;
        public const int IsForceUnmountWeapon = /*1 <<*/ (int) UserCmdEnum.IsForceUnmountWeapon;
        public const int IsInInterruptState   = /*1 <<*/ (int) UserCmdEnum.IsInterrupt;
        public const int IsSprayPaint         = /*1 <<*/ (int) UserCmdEnum.IsSprayPaint;
        public const int IsScopeIn            = (int) UserCmdEnum.IsScopeIn;
        public const int IsScopeOut           = (int) UserCmdEnum.IsScopeOut;
    }

    public class UserCmd : BaseRefCounter, IUserCmd
    {
        protected UserCmd()
        {
        }

        /*private BitVector32 _flags = new BitVector32();*/
        /*暂定8，有越界风险*/

        private byte[] _flags = new byte[8];
   
        public int   FrameInterval       { get; set; }
        public bool  NeedStepPredication { get; set; }
        public int   RenderTime          { get; set; }
        public int   ClientTime          { get; set; }
        public bool  PredicatedOnce      { get; set; }
        public int   Seq                 { get; set; }
        public int   SnapshotId          { get; set; }
        public float MoveHorizontal      { get; set; }
        public float MoveVertical        { get; set; }
        public float MoveUpDown          { get; set; }
        public int   CurWeapon           { get; set; }

        public long Buttons
        {
            get { return BitConverter.ToInt64(_flags, 0); }
            set { _flags = /*new BitVector32(value)*/BitConverter.GetBytes(value); }
        }

        private bool AddEnum(int idx, bool add)
        {
            if (idx < 0 || idx >= this._flags.Length * 8)
            {
                return false;
            }

            /*if (((int)this._flags[idx / 8] & 1 << idx % 8) != 0)
            {
                return false;
            }*/
            byte[] expr_cp_0 = this._flags;
            int    expr_cp_1 = idx / 8;
            if (add)
            {
                expr_cp_0[expr_cp_1] |= (byte) (1 << idx % 8);
            }
            else
            {
                expr_cp_0[expr_cp_1] &= (byte) (~(byte) (1 << idx % 8));
            }

            return true;
        }

        private void ClearEnum()
        {
            for (int i = 0, maxi = _flags.Length; i < maxi; i++)
            {
                _flags[i] = 0;
            }
        }

        private bool HasEnum(int idx)
        {
            return idx >= 0 && idx < this._flags.Length * 8 && ((int) this._flags[idx / 8] & 1 << idx % 8) != 0;
        }

        public bool IsSwitchFireMode
        {
            get { return HasEnum((int) EUserCmdFlags.IsSwitchFireMode); }
            set { AddEnum((int) EUserCmdFlags.IsSwitchFireMode, value); }
        }

        public bool IsRun
        {
            get { return HasEnum((int) EUserCmdFlags.IsRun); }
            set { AddEnum((int) EUserCmdFlags.IsRun, value); }
        }

        public bool IsCrouch
        {
            get { return HasEnum((int) EUserCmdFlags.IsCrouch); }
            set { AddEnum((int) EUserCmdFlags.IsCrouch, value); }
        }

        public bool IsProne
        {
            get { return HasEnum((int) EUserCmdFlags.IsProne); }
            set { AddEnum((int) EUserCmdFlags.IsProne, value); }
        }

        public bool IsSlightWalk
        {
            get { return HasEnum((int) EUserCmdFlags.IsSlightWalk); }
            set { AddEnum((int) EUserCmdFlags.IsSlightWalk, value); }
        }

        public bool IsCameraFree
        {
            get { return HasEnum((int) EUserCmdFlags.IsCameraFree); }
            set { AddEnum((int) EUserCmdFlags.IsCameraFree, value); }
        }

        public bool IsLeftAttack
        {
            get { return HasEnum((int) EUserCmdFlags.IsLeftAttack); }
            set { AddEnum((int) EUserCmdFlags.IsLeftAttack, value); }
        }

        public bool IsRightAttack
        {
            get { return HasEnum((int) EUserCmdFlags.IsRightAttack); }
            set { AddEnum((int) EUserCmdFlags.IsRightAttack, value); }
        }

        public bool IsJump
        {
            get { return HasEnum((int) EUserCmdFlags.IsJump); }
            set { AddEnum((int) EUserCmdFlags.IsJump, value); }
        }

        public bool IsF
        {
            get { return HasEnum((int) EUserCmdFlags.IsF); }
            set { AddEnum((int) EUserCmdFlags.IsF, value); }
        }

        public bool IsCameraFocus
        {
            get { return HasEnum((int) EUserCmdFlags.IsCameraFocus); }
            set { AddEnum((int) EUserCmdFlags.IsCameraFocus, value); }
        }

        public bool IsDrawWeapon
        {
            get { return HasEnum((int) EUserCmdFlags.IsDrawWeapon); }
            set { AddEnum((int) EUserCmdFlags.IsDrawWeapon, value); }
        }

        public bool IsForceUnmountWeapon
        {
            get { return HasEnum((int) EUserCmdFlags.IsForceUnmountWeapon); }
            set { AddEnum((int) EUserCmdFlags.IsForceUnmountWeapon, value); }
        }

        public bool ChangeCamera
        {
            get { return HasEnum((int) EUserCmdFlags.ChangeCamera); }
            set { AddEnum((int) EUserCmdFlags.ChangeCamera, value); }
        }

        public int BeState      { get; set; }
        public int SwitchNumber { get; set; }
        public int BagIndex     { get; set; }

        public bool IsReload
        {
            get { return HasEnum((int) EUserCmdFlags.IsReload); }
            set { AddEnum((int) EUserCmdFlags.IsReload, value); }
        }

        public bool IsPeekLeft
        {
            get { return HasEnum((int) EUserCmdFlags.IsPeekLeft); }
            set { AddEnum((int) EUserCmdFlags.IsPeekLeft, value); }
        }

        public bool IsPeekRight
        {
            get { return HasEnum((int) EUserCmdFlags.IsPeekRight); }
            set { AddEnum((int) EUserCmdFlags.IsPeekRight, value); }
        }

        public bool IsSwitchWeapon
        {
            get { return HasEnum((int) EUserCmdFlags.IsSwitchWeapon); }
            set { AddEnum((int) EUserCmdFlags.IsSwitchWeapon, value); }
        }

        public bool IsDropWeapon
        {
            get { return HasEnum((int) EUserCmdFlags.IsDropWeapon); }
            set { AddEnum((int) EUserCmdFlags.IsDropWeapon, value); }
        }

        public bool IsPDown
        {
            get { return HasEnum((int) EUserCmdFlags.IsPDown); }
            set { AddEnum((int) EUserCmdFlags.IsPDown, value); }
        }

        public bool IsYDown
        {
            get { return HasEnum((int) EUserCmdFlags.IsYDown); }
            set { AddEnum((int) EUserCmdFlags.IsYDown, value); }
        }

        public bool IsCDown
        {
            get { return HasEnum((int) EUserCmdFlags.IsCDown); }
            set { AddEnum((int) EUserCmdFlags.IsCDown, value); }
        }

        public bool IsSpaceDown
        {
            get { return HasEnum((int) EUserCmdFlags.IsSpaceDown); }
            set { AddEnum((int) EUserCmdFlags.IsSpaceDown, value); }
        }

        public bool IsTabDown
        {
            get { return HasEnum((int) EUserCmdFlags.IsTabDown); }
            set { AddEnum((int) EUserCmdFlags.IsTabDown, value); }
        }

        public bool IsThrowing
        {
            get { return HasEnum((int) EUserCmdFlags.IsThrowing); }
            set { AddEnum((int) EUserCmdFlags.IsThrowing, value); }
        }

        public bool IsAddMark
        {
            get { return HasEnum((int) EUserCmdFlags.IsAddMark); }
            set { AddEnum((int) EUserCmdFlags.IsAddMark, value); }
        }

        public bool IsHoldBreath
        {
            get { return HasEnum((int) EUserCmdFlags.IsHoldBreath); }
            set { AddEnum((int) EUserCmdFlags.IsHoldBreath, value); }
        }

        public bool IsSprayPaint
        {
            get { return HasEnum((int) EUserCmdFlags.IsSprayPaint); }
            set { AddEnum((int) EUserCmdFlags.IsSprayPaint, value); }
        }

        public bool IsSwitchAutoRun
        {
            get { return HasEnum((int) EUserCmdFlags.IsSwitchAutoRun); }
            set { AddEnum((int) EUserCmdFlags.IsSwitchAutoRun, value); }
        }

        public bool IsManualPickUp
        {
            get { return HasEnum((int) EUserCmdFlags.IsPickUp); }
            set { AddEnum((int) EUserCmdFlags.IsPickUp, value); }
        }

        public bool IsInterrupt
        {
            get { return HasEnum((int) EUserCmdFlags.IsInInterruptState); }
            set { AddEnum((int) EUserCmdFlags.IsInInterruptState, value); }
        }

        public bool IsUseAction
        {
            get { return HasEnum((int) EUserCmdFlags.IsUseAction); }
            set { AddEnum((int) EUserCmdFlags.IsUseAction, value); }
        }

        public bool IsScopeIn
        {
            get { return HasEnum((int) EUserCmdFlags.IsScopeIn); }
            set { AddEnum((int) EUserCmdFlags.IsScopeIn, value); }
        }

        public bool IsScopeOut
        {
            get { return HasEnum((int) EUserCmdFlags.IsScopeOut); }
            set { AddEnum((int) EUserCmdFlags.IsScopeOut, value); }
        }

        public float DeltaYaw      { get; set; }
        public float DeltaPitch    { get; set; }
        public float Roll          { get; set; }
        public int   ChangedSeat   { get; set; }
        public int   ChangeChannel { get; set; }

        public int       ManualPickUpEquip { get; set; }
        public List<int> AutoPickUpEquip   { get; set; }

        public IFilteredInput FilteredInput  { get; set; }
        public int            UseEntityId    { get; set; }
        public int            UseVehicleSeat { get; set; }
        public int            UseType        { get; set; }

        //Generate in runtime
        public bool IsAutoFire   { get; set; }
        public bool IsAutoReload { get; set; }


        public static UserCmd Allocate()
        {
            return ObjectAllocatorHolder<UserCmd>.Allocate();
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(UserCmd))
            {
            }

            public override object MakeObject()
            {
                return new UserCmd();
            }
        }

        public void CopyTo(UserCmd cmd)
        {
            cmd.Reset();
            cmd.FrameInterval        = FrameInterval;
            cmd.RenderTime           = RenderTime;
            cmd.ClientTime           = ClientTime;
            cmd.PredicatedOnce       = PredicatedOnce;
            cmd.Seq                  = Seq;
            cmd.MoveHorizontal       = MoveHorizontal;
            cmd.IsLeftAttack         = IsLeftAttack;
            cmd.IsRightAttack        = IsRightAttack;
            cmd.IsRun                = IsRun;
            cmd.IsCrouch             = IsCrouch;
            cmd.IsProne              = IsProne;
            cmd.BeState              = BeState;
            cmd.SwitchNumber         = SwitchNumber;
            cmd.IsReload             = IsReload;
            cmd.IsPeekLeft           = IsPeekLeft;
            cmd.IsPeekRight          = IsPeekRight;
            cmd.IsSwitchWeapon       = IsSwitchWeapon;
            cmd.IsSlightWalk         = IsSlightWalk;
            cmd.MoveVertical         = MoveVertical;
            cmd.MoveUpDown           = MoveUpDown;
            cmd.DeltaYaw             = DeltaYaw;
            cmd.DeltaPitch           = DeltaPitch;
            cmd.Roll                 = Roll;
            cmd.IsJump               = IsJump;
            cmd.IsF                  = IsF;
            cmd.ChangedSeat          = ChangedSeat;
            cmd.ChangeChannel        = ChangeChannel;
            cmd.IsCameraFocus        = IsCameraFocus;
            cmd.IsCameraFree         = IsCameraFree;
            cmd.ChangeCamera         = ChangeCamera;
            cmd.SnapshotId           = SnapshotId;
            cmd.IsSwitchFireMode     = IsSwitchFireMode;
            cmd.CurWeapon            = CurWeapon;
            cmd.IsDropWeapon         = IsDropWeapon;
            cmd.ManualPickUpEquip    = ManualPickUpEquip;
            cmd.AutoPickUpEquip      = AutoPickUpEquip;
            cmd.IsPDown              = IsPDown;
            cmd.IsYDown              = IsYDown;
            cmd.IsCDown              = IsCDown;
            cmd.IsSpaceDown          = IsSpaceDown;
            cmd.IsDrawWeapon         = IsDrawWeapon;
            cmd.IsTabDown            = IsTabDown;
            cmd.IsThrowing           = IsThrowing;
            cmd.IsAddMark            = IsAddMark;
            cmd.IsHoldBreath         = IsHoldBreath;
            cmd.IsSwitchAutoRun      = IsSwitchAutoRun;
            cmd.IsManualPickUp       = IsManualPickUp;
            cmd.UseEntityId          = UseEntityId;
            cmd.UseVehicleSeat       = UseVehicleSeat;
            cmd.UseType              = UseType;
            cmd.BagIndex             = BagIndex;
            cmd.IsInterrupt          = IsInterrupt;
            cmd.IsSprayPaint         = IsSprayPaint; /*喷漆*/
            cmd.IsUseAction          = IsUseAction;
            cmd.IsForceUnmountWeapon = IsForceUnmountWeapon;
            cmd.IsScopeIn            = IsScopeIn;
            cmd.IsScopeOut           = IsScopeOut;
            cmd.IsAutoFire           = IsAutoFire;
            cmd.IsAutoReload         = IsAutoReload;
        }

        public void Reset()
        {
            ClearEnum();
            FrameInterval     = 0;
            RenderTime        = 0;
            ClientTime        = 0;
            PredicatedOnce    = false;
            Seq               = 0;
            MoveHorizontal    = 0;
            IsLeftAttack      = false;
            IsRightAttack     = false;
            IsRun             = false;
            IsCrouch          = false;
            IsProne           = false;
            BeState           = 0;
            SwitchNumber      = -1;
            IsReload          = false;
            IsPeekLeft        = false;
            IsPeekRight       = false;
            IsSwitchWeapon    = false;
            IsSlightWalk      = false;
            MoveVertical      = 0;
            MoveUpDown        = 0;
            DeltaYaw          = 0;
            DeltaPitch        = 0;
            Roll              = 0;
            IsJump            = false;
            IsF               = false;
            ChangedSeat       = 0;
            IsCameraFocus     = false;
            ChangeCamera      = false;
            IsCameraFree      = false;
            SnapshotId        = 0;
            IsSwitchFireMode  = false;
            IsDropWeapon      = false;
            CurWeapon         = 0;
            ManualPickUpEquip = 0;
            if (null == AutoPickUpEquip) AutoPickUpEquip = new List<int>();
            AutoPickUpEquip.Clear();
            IsPDown              = false;
            IsYDown              = false;
            IsCDown              = false;
            IsSpaceDown          = false;
            IsDrawWeapon         = false;
            IsTabDown            = false;
            IsThrowing           = false;
            IsAddMark            = false;
            IsHoldBreath         = false;
            IsSwitchAutoRun      = false;
            IsManualPickUp       = false;
            UseEntityId          = 0;
            UseVehicleSeat       = 0;
            UseType              = 0;
            BagIndex             = 0;
            IsInterrupt          = false;
            IsSprayPaint         = false;
            IsUseAction          = false;
            IsForceUnmountWeapon = false;
            IsScopeIn            = false;
            IsScopeOut           = false;
            IsAutoFire           = false;
            IsAutoReload         = false;
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

        public static List<int> CopyList(List<int> l, List<int> r)
        {
            if (l == null)
            {
                l = new List<int>();
            }
            else
            {
                l.Clear();
            }

            if (r != null)
            {
                l.AddRange(r);
            }

            return l;
        }
    }
}