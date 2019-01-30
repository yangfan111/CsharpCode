using Core.Utils;
using WeaponConfigNs;
using Core.WeaponLogic.Attachment;
using UnityEngine;
using XmlConfig;

namespace Core.WeaponLogic.Common
{
    public class ThrowingFireAction : AbstractFireLogic<ThrowingFireLogicConfig, object>, IFireLogic
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingFireAction));

        private IWeaponEffectLogic _weaponEffectLogic;
        private IAttachmentManager _attachmentManager;
        private IWeaponSoundLogic _soundLogic;

        private IThrowingFactory _throwingFactory;
        private IThrowingContainer _throwingLogic;

        private bool _initialized;

        private static int _switchCdTime = 300;

        public ThrowingFireAction(
            NewWeaponConfigItem newWeaponConfig,
            ThrowingFireLogicConfig config,
            IWeaponLogicComponentsFactory componentsFactory,
            IAttachmentManager attachmentManager,
            IWeaponSoundLogic soundLogic,
            IWeaponEffectLogic effectLogic) : base(config)
        {
            _attachmentManager = attachmentManager;
            _soundLogic = soundLogic;
            _weaponEffectLogic = effectLogic;

            _throwingLogic = componentsFactory.CreateThrowingLogic(config.Basic);
            _throwingFactory = componentsFactory.CreateThrowingFactory(newWeaponConfig, config.Throwing);
        }
        
        public override void Apply(ThrowingFireLogicConfig baseConfig, ThrowingFireLogicConfig output, object arg)
        {
            
        }

        public void SetAttachment(WeaponPartsStruct attachments)
        {
            _attachmentManager.Prepare(attachments);
            _attachmentManager.ApplyAttachment(_soundLogic);
            _attachmentManager.ApplyAttachment(_weaponEffectLogic);
        }

        public int GetBulletLimit()
        {
            return 1;
        }

        public int GetSpecialReloadCount()
        {
            return 0;
        }

        public void Reset()
        {

        }

        public void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (!_initialized)
            {
                _initialized = true;
            }

            if (cmd.IsFire && cmd.FilteredInput.IsInput(EPlayerInput.IsLeftAttack))
            {
                DoReady(playerWeapon, cmd);
            }

            if (cmd.SwitchThrowMode)
            {
                DoSwitchMode(playerWeapon, cmd);
            }

            if (cmd.IsReload && cmd.FilteredInput.IsInput(EPlayerInput.IsReload))
            {
                DoPull(playerWeapon, cmd);
            }

            if (cmd.IsThrowing && cmd.FilteredInput.IsInput(EPlayerInput.IsThrowing))
            {
                DoThrowing(playerWeapon, cmd);
            }

            //判断打断
            CheckBrokeThrow(playerWeapon, cmd);
        }

        //准备
        private void DoReady(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (!playerWeapon.ThrowingActionInfo.IsReady)
            {
                playerWeapon.ThrowingActionInfo.IsReady = true;
                playerWeapon.ThrowingActionInfo.ReadyTime = playerWeapon.ClientTime;
                playerWeapon.ThrowingActionInfo.Config = _throwingFactory.ThrowingConfig;
                //准备动作
                playerWeapon.ReadyThrowing();
            }
        }

        //投掷/抛投切换
        private void DoSwitchMode(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (playerWeapon.ThrowingActionInfo.IsReady && !playerWeapon.ThrowingActionInfo.IsThrow)
            {
                if ((playerWeapon.ClientTime - playerWeapon.ThrowingActionInfo.LastSwitchTime) < _switchCdTime)
                {
                    return;
                }
                playerWeapon.ThrowingActionInfo.LastSwitchTime = playerWeapon.ClientTime;
                playerWeapon.ThrowingActionInfo.IsNearThrow = !playerWeapon.ThrowingActionInfo.IsNearThrow;
                playerWeapon.SwitchThrowingMode(playerWeapon.ThrowingActionInfo.IsNearThrow);
            }
        }

        //拉栓
        private void DoPull(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (playerWeapon.ThrowingActionInfo.IsReady && !playerWeapon.ThrowingActionInfo.IsPull)
            {
                playerWeapon.ThrowingActionInfo.IsPull = true;
                playerWeapon.ThrowingActionInfo.LastPullTime = playerWeapon.ClientTime;
                playerWeapon.ThrowingActionInfo.ShowCountdownUI = true;
                playerWeapon.ThrowingActionInfo.IsInterrupt = false;
                //生成Entity
                int renderTime = cmd.RenderTime;
                var dir = BulletDirUtility.GetThrowingDir(playerWeapon);
                playerWeapon.ThrowingActionInfo.ThrowingEntityKey = _throwingFactory.CreateThrowing(playerWeapon, dir, renderTime, GetInitVel(playerWeapon));
                playerWeapon.LastBulletDir = dir;
                //弹片特效
                if (cmd.IsReload)
                {
                    _weaponEffectLogic.CreatePullBoltEffect(playerWeapon);
                }
                playerWeapon.OnWeaponStateChanged();
            }
        }

        //投掷
        private void DoThrowing(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (playerWeapon.ThrowingActionInfo.IsReady && !playerWeapon.ThrowingActionInfo.IsThrow)
            {
                if (!playerWeapon.ThrowingActionInfo.IsPull)
                {
                    DoPull(playerWeapon, cmd);
                }
                playerWeapon.ThrowingActionInfo.IsThrow = true;
                playerWeapon.ThrowingActionInfo.ShowCountdownUI = false;
                //投掷时间
                playerWeapon.ThrowingActionInfo.LastFireTime = playerWeapon.ClientTime;
                _throwingFactory.UpdateThrowing(playerWeapon.ThrowingActionInfo.ThrowingEntityKey, true, GetInitVel(playerWeapon));
                //投掷动作
                playerWeapon.StartThrowing();
                //状态重置
                playerWeapon.IsThrowingStartFly = false;
                //投掷型物品使用数量
                playerWeapon.OnGrenadeThrowingOnce();
            }
        }

        private void CheckBrokeThrow(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (playerWeapon.ThrowingActionInfo.IsReady && cmd.FilteredInput.IsInputBlocked(EPlayerInput.IsThrowing))
            {
                //收回手雷
                playerWeapon.UnmountWeaponByAction();
                if (playerWeapon.ThrowingActionInfo.IsPull)
                {
                    //若已拉栓，销毁ThrowingEntity
                    _throwingFactory.DestroyThrowing(playerWeapon.ThrowingActionInfo.ThrowingEntityKey);
                }
                //拉栓未投掷，打断投掷动作
                playerWeapon.ForceStopThrowing();
                ClearState(playerWeapon);
            }
        }

        private void ClearState(IPlayerWeaponState playerWeapon)
        {
            playerWeapon.ThrowingActionInfo.ClearState();
        }

        private float GetInitVel(IPlayerWeaponState playerWeapon)
        {
            return _throwingFactory.ThrowingInitSpeed(playerWeapon.ThrowingActionInfo.IsNearThrow);
        }

        public void SetVisualConfig(ref VisualConfigGroup config)
        {
        }
    }
}