using Core.Prediction.UserPrediction.Cmd;
using App.Shared.GameModules.Weapon.Behavior;
using WeaponConfigNs;
using XmlConfig;
using Core.GameInputFilter;
using System;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public abstract class AbstractWeaponCmd : IWeaponCmd
    {
        protected IUserCmd _cmd;

#pragma warning disable RefCounter001 // possible reference counter error
        public void SetCurrentCmd(IUserCmd cmd)
#pragma warning restore RefCounter001 // possible reference counter error
        {
            _cmd = cmd;
        }
        public abstract bool IsFire { get; }
        public bool IsSwitchFireMode
        {
            get { return _cmd.IsSwitchFireMode; }
        }
        public int FrameInterval
        {
            get { return _cmd.FrameInterval; }
        }
        public int CmdSeq
        {
            get { return _cmd.Seq; }
        }

        public int RenderTime
        {
            get { return _cmd.RenderTime; }
        }

        public bool IsCanFire
        {
            get { return true; }
        }

        public bool IsSpecialFire
        {
            get
            {
                return _cmd.IsRightAttack;
            }
        }

        public bool IsThrowing
        {
            get { return _cmd.IsThrowing; }
        }

        public bool IsReload
        {
            get { return _cmd.IsReload; }
        }

        public bool SwitchThrowMode
        {
            get { return _cmd.IsRightAttack; }
        }

        public IFilteredInput FilteredInput
        {
            get { return _cmd.FilteredInput; }
        }
    }

    public class LeftWeaponCmd : AbstractWeaponCmd
    {
        public override bool IsFire
        {
            get { return _cmd.IsLeftAttack && IsCanFire; }
        }
    }

    public class RightWeaponCmd : AbstractWeaponCmd
    {
        public override bool IsFire
        {
            get { return _cmd.IsRightAttack && IsCanFire; }
        }
    }
}