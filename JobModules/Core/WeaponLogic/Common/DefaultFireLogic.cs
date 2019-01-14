using System.Collections.Generic;
using Core.Utils;
using WeaponConfigNs;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.FireAciton;
using Core.WeaponLogic.Bullet;
using Core.WeaponLogic.Common;
using App.Shared.Components;
namespace Core.WeaponLogic
{
    public class DefaultFireLogic : AbstractFireLogic<DefaultFireLogicConfig,  DefaultFireModifierArg>, IFireLogic
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultFireLogic));

        private List<IAfterFireBullet> _afterFires = new List<IAfterFireBullet>();
        private List<IAfterFireCmd> _afterFireCmds = new List<IAfterFireCmd>();
        private List<IFrame> _frames = new List<IFrame>();
        private List<IBeforeFireBullet> _beforeFires = new List<IBeforeFireBullet>();
        private List<IIdle> _idles = new List<IIdle>();

        private IAccuracyLogic _accuracyLogic;
        private ISpreadLogic _spreadLogic;
        private IBulletFactory _bulletFactory;
        private IWeaponEffectLogic _weaponEffectLogic;
        private IKickbackLogic _kickbackLogic;
        private IFireBulletModeLogic _fireBulletModeLogic;
        private IFireBulletCounter _fireBulletCounter;
        private IAttachmentManager _attachmentManager;
        private IBulletContainer _bulletLogic;
        private IWeaponSoundLogic _soundLogic;
        private IFireActionLogic _fireActionLogic;
        private bool _fireAfterBreakReload;

        private IAutoFireLogic _autoFireLogic;
        private bool _startSpecialFire;
        private bool _initialized;
        IBulletFireInfoProviderDispatcher _bulletFireInfoProvider;

        public DefaultFireLogic(
            NewWeaponConfigItem newWeaponConfig,
            DefaultFireLogicConfig config,
            IWeaponLogicComponentsFactory componentsFactory,
            IAttachmentManager attachmentManager,
            IWeaponSoundLogic soundLogic,
            IWeaponEffectLogic effectLogic,
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher):base(config)
        {
            _attachmentManager = attachmentManager;
            _accuracyLogic = componentsFactory.CreateAccuracyLogic(config.AccuracyLogic, config.Basic);
            _spreadLogic = componentsFactory.CreateSpreadLogic(config.SpreadLogic, config.Basic);
            _autoFireLogic = componentsFactory.CreateAutoFireLogic(config.FireModeLogic, config.Basic);
            _bulletLogic = componentsFactory.CreateBulletLogic(config.Basic);
            _soundLogic = soundLogic;
            _weaponEffectLogic = effectLogic;
            _bulletFireInfoProvider = bulletFireInfoProviderDispatcher;
            
            _bulletFactory = componentsFactory.CreateBulletFactory(config.Bullet, config.Basic);
            _kickbackLogic = componentsFactory.CreateKickbackLogic(config.KickbackLogic, config.Basic);
            _fireBulletModeLogic = componentsFactory.CreateFireReadyLogic(config.FireModeLogic, config.Basic);
            _fireBulletCounter = componentsFactory.CreateContinuesShootLogic(config.FireCounter, config.Basic);
            _fireActionLogic = componentsFactory.CreateFireActionLogic(newWeaponConfig, config.Basic, _soundLogic);

            AddLogic(_accuracyLogic);
            AddLogic(_spreadLogic);
            AddLogic(_kickbackLogic);
            AddLogic(_autoFireLogic);
            AddLogic(_fireBulletModeLogic);
            AddLogic(_fireActionLogic);
            AddLogic(_fireBulletCounter);
        }

        public override void Apply(DefaultFireLogicConfig baseConfig, DefaultFireLogicConfig output, DefaultFireModifierArg arg)
        {
            output.Fov = arg.Fov > 0 ? arg.Fov : baseConfig.Fov;
            output.FocusSpeed = arg.FocusSpeed > 0 ? arg.FocusSpeed : baseConfig.FocusSpeed;
            output.ReloadSpeed = arg.ReloadSpeed > 0 ? arg.ReloadSpeed : baseConfig.ReloadSpeed;
            output.BreathFactor = arg.BreathFactor > 0 ? arg.BreathFactor : baseConfig.BreathFactor;
        }
        
        public void AddLogic<T>(T logic)
        {
            var beforeLogic = logic as IBeforeFireBullet;
            if(null != beforeLogic)
            {
                _beforeFires.Add(beforeLogic);
            }
            var afterLogic = logic as IAfterFireBullet;
            if(null != afterLogic)
            {
                _afterFires.Add(afterLogic);
            }
            var afterCmdLogic = logic as IAfterFireCmd;
            if(null != afterCmdLogic)
            {
                _afterFireCmds.Add(afterCmdLogic);
            }
            var idleLogic = logic as IIdle;
            if(null != idleLogic)
            {
                _idles.Add(idleLogic);
            }
            var frameLogic = logic as IFrame;
            if(null != frameLogic)
            {
                _frames.Add(frameLogic);
            }
        }

        public void SetAttachment(WeaponPartsStruct attachments)
        {
            _attachmentManager.Prepare(attachments);
            _attachmentManager.ApplyAttachment(_kickbackLogic);
            _attachmentManager.ApplyAttachment(_spreadLogic);
            _attachmentManager.ApplyAttachment(_accuracyLogic);
            _attachmentManager.ApplyAttachment(_bulletLogic);
            _attachmentManager.ApplyAttachment(_soundLogic);
            _attachmentManager.ApplyAttachment(_weaponEffectLogic);
            _attachmentManager.ApplyAttachment(_bulletFactory);
            _attachmentManager.ApplyAttachment(this);
        }

        public int GetBulletLimit()
        {
            return _bulletLogic.GetBulletLimit();
        }

        public int GetSpecialReloadCount()
        {
            return _bulletLogic.GetSpecialReloadCount();
        }

        public void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (!_initialized)
            {
                _initialized = true;

            }
            if (cmd.IsFire)
            {
                if(null != cmd.FilteredInput && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsLeftAttack))
                {
                    if (BreakSpecialReload(playerWeapon))
                    {
                        if (playerWeapon.CanFire())
                        {
                            Fire(playerWeapon, cmd);   
                        }
                    }
                }
                else
                {
                    if(null == cmd.FilteredInput)
                    {
                        Logger.Error("FilteredInput in cmd should never be null !");
                    }
                }
            }
            else if (_autoFireLogic.Running || _fireAfterBreakReload)
            {
                if (playerWeapon.CanFire())
                {
                    Fire(playerWeapon, cmd);
                }
            }
            else
            {
                CallOnIdle(playerWeapon, cmd);
            }

            CallOnFrame(playerWeapon, cmd);
        }

        private void Fire(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            _bulletFireInfoProvider.Prepare();
            int cmdSeq = cmd.CmdSeq;
            int renderTime = cmd.RenderTime;
            playerWeapon.RangeAttacking = true;
            int fireTimes = _fireBulletModeLogic.GetFireTimes(playerWeapon);
            for(int i = 0; i < fireTimes; i++)
            {
                CallBeforeFire(playerWeapon, cmd, i);

                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("spread {0} {1}, YawPitch {2} {3}, index {4}",
                        playerWeapon.LastSpreadX, playerWeapon.LastSpreadY, playerWeapon.NegPunchYaw,
                        playerWeapon.NegPunchPitch, i);
                }
               
               //播放弹壳掉落特效
                //_weaponEffectLogic.PlayBulletDropEffect(playerWeapon);
                _weaponEffectLogic.CreateFireEvent(playerWeapon);
                //Audio.GameAudioMedium.PerformAudioOnGunFire(playerWeapon);
                //Audio.AKAudioEntry.PostEvent(11,);

                // 射出子弹
                for(int j = 0; j < _bulletFactory.BulletHitCount; j ++)
                {
                    var dir = _bulletFireInfoProvider.GetFireDir(cmdSeq + i + j ); 
                    _bulletFactory.CreateBullet(playerWeapon, 
                        dir,
                        _bulletFireInfoProvider,
                        cmdSeq, 
                        renderTime);
                    playerWeapon.LastBulletDir = dir;
                }
                //开枪总数
                playerWeapon.OnFireOnce();
            //    playerWeapon.CurrentWeapon
                CallAfterFires(playerWeapon, cmd, i);
                playerWeapon.OnWeaponCost();
            }

            CallAfterFireCmds(playerWeapon, cmd, fireTimes);
            if (fireTimes > 0)
            Logger.DebugFormat("fire {0}", fireTimes);
        }

        private void CallAfterFires(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            foreach (var afterfire in _afterFires)
            {
                afterfire.AfterFireBullet(playerWeapon, cmd, bullet);
            }
        }

        private void CallAfterFireCmds(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bulletCount)
        {
            foreach (var afterfire in _afterFireCmds)
            {
                afterfire.AfterFireCmd(playerWeapon, cmd, bulletCount);
            }
        }

        private void CallBeforeFire(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            foreach (var beforeFire in _beforeFires)
            {
                beforeFire.BeforeFireBullet(playerWeapon, cmd, bullet);
            }
        }

        private void CallOnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            foreach (var fireIdle in _idles)
            {
                fireIdle.OnIdle(playerWeapon, cmd);
            }
        }

        private void CallOnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            foreach (var beforeFire in _frames)
            {
                beforeFire.OnFrame(playerWeapon, cmd);
            }
        }

        /// <summary>
        /// 判断特殊换弹逻辑 
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        /// <returns>是否可以射击</returns>
        private bool BreakSpecialReload(IPlayerWeaponState playerWeapon)
        {
            if (!playerWeapon.IsReload)
            {
                _fireAfterBreakReload = false;
                return true;
            }
            if (!_fireAfterBreakReload && playerWeapon.SpecialReloadCount > 0 && playerWeapon.LoadedBulletCount > 0)
            {
                if (playerWeapon.IsBolted)
                {
                    //如果已经上膛，直接打断并开枪
                    playerWeapon.ForceBreakReloading(null);
                    return true;
                }
                else
                {
                    //如果没有上膛，执行上膛，结束后开枪
                    playerWeapon.BreakReloading();
                    playerWeapon.IsBolted = true;
                    _fireAfterBreakReload = true;
                }
            }
            return false;
        }

        /// <summary>
        /// 切枪时清理状态
        /// </summary>
        public void Reset()
        {
            _fireAfterBreakReload = false;
            _startSpecialFire = false;
            _autoFireLogic.Reset();
        }

        public void SetVisualConfig(ref VisualConfigGroup config)
        {
            _kickbackLogic.SetVisualConfig(ref config);
        }

        public override float GetBreathFactor()
        {
            if(_config.BreathFactor == 0)
            {
                return base.GetBreathFactor();
            }
            return _config.BreathFactor;
        }

        public override float GetFocusFov()
        {
            if(_config.Fov == 0)
            {
                return base.GetFocusFov();
            }
            return _config.Fov;
        }

        public override float GetFocusSpeed()
        {
            if(_config.FocusSpeed == 0)
            {
                return base.GetFocusSpeed();
            }
            return _config.FocusSpeed;
        }

        public override bool IsFovModified()
        {
            return _config.Fov != _baseConfig.Fov;
        }

        public override float GetReloadSpeed()
        {
            if(_config.ReloadSpeed == 0)
            {
                return base.GetReloadSpeed();
            }
            return _config.ReloadSpeed;
        }
    }
}