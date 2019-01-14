using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Common;
using WeaponConfigNs;
using XmlConfig;
using Core.GameInputFilter;

namespace Core.WeaponLogic
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

    public abstract class AbstractWeaponLogic<T1, T3> : AbstractAttachableWeaponLogic<T1, T3>, IWeaponLogic where T1 : ICopyableConfig<T1>, new()
    {
        public bool EmptyHand
        {
            get;set;
        } 

        public AbstractWeaponLogic(T1 config, IWeaponLogicComponentsFactory componentsFactory):base(config)
        {
        }
        public virtual float GetBaseSpeed()
        {
            return 6;
        }

        public virtual float GetFov()
        {
            return 75;
        }

        public virtual bool IsFovModified()
        {
            return false;
        }

        public virtual float GetBreathFactor()
        {
            return 1f;
        }

        public virtual float GetFocusSpeed()
        {
            return 1;
        }

        public virtual float GetReloadSpeed()
        {
            return 1f;
        }

        public virtual bool CanCameraFocus()
        {
            //如果有两种形态，不能开镜
            var defCfg = _config as DefaultWeaponLogicConfig;
            if(null == defCfg)
            {
                return false;
            }
            var defFireCfg = defCfg.FireLogic as DefaultFireLogicConfig;
            //如果不是默认远程，不能开镜
            if(null == defFireCfg)
            {
                return false;
            }
            return true;
        } 

        public abstract void SetAttachment(WeaponPartsStruct attachments);

        public void PlaySound(EWeaponSoundType sound)
        {
        }

        public abstract int GetBulletLimit();
        public abstract int GetSpecialReloadCount();

        public abstract void Update(IPlayerWeaponState playerWeapon, IUserCmd cmd);

        public abstract void Reset();

        public abstract void SetVisualConfig(ref VisualConfigGroup config);
    }
}