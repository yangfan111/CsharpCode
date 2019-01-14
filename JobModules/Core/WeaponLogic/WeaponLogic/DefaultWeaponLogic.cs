using System;
using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Bullet;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class DefaultWeaponLogic : AbstractWeaponLogic<DefaultWeaponLogicConfig, int>
    {
        private IFireLogic _fireLogic;
        private LeftWeaponCmd _cmd = new LeftWeaponCmd();
        private IAttachmentManager _attachmentManager;
        public DefaultWeaponLogic(
            NewWeaponConfigItem newCfg,
            DefaultWeaponLogicConfig config, 
            IWeaponLogicComponentsFactory componentsFactory, 
            IWeaponSoundLogic soundLogic, 
            IWeaponEffectLogic effectLogic, 
            IAttachmentManager attachmentManager,
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher) : base(config, componentsFactory)
        {
            _fireLogic = componentsFactory.CreateFireLogic(newCfg, config.FireLogic, soundLogic, effectLogic, bulletFireInfoProviderDispatcher);
            _attachmentManager = attachmentManager; 
        }
        
        public override void Apply(DefaultWeaponLogicConfig baseConfig, DefaultWeaponLogicConfig output, int arg)
        {
            
        }

        public override float GetBaseSpeed()
        {
            var moveConfig = _config.MoveSpeedLogic as DefaultMoveSpeedLogicConfig;
            if(null == moveConfig)
            {
                return moveConfig.MaxSpeed;
            }
            return base.GetBaseSpeed();
        }

        public override float GetFocusSpeed()
        {
            var fireConfig = _config.FireLogic as DefaultFireLogicConfig;
            if(null == fireConfig)
            {
                return 1;
            }
            return fireConfig.FocusSpeed == 0 ? 1 : fireConfig.FocusSpeed;
        }

        public override float GetReloadSpeed()
        {
            var fireConfig = _config.FireLogic as DefaultFireLogicConfig;
            if(null == fireConfig)
            {
                return 1;
            }
            return fireConfig.ReloadSpeed == 0 ? 1 : fireConfig.ReloadSpeed;
        }

        public override float GetBreathFactor()
        {
            var factor = _fireLogic.GetBreathFactor();
            return factor == 0 ? 1 : factor;
        }

        public override float GetFov()
        {
            var fov = _fireLogic.GetFocusFov();
            return fov;
        }

        public override bool IsFovModified()
        {
            return _fireLogic.IsFovModified();
        }

        public override void Update(IPlayerWeaponState playerWeapon, IUserCmd cmd)
        {
            _cmd.SetCurrentCmd(cmd);
            _fireLogic.OnFrame(playerWeapon, _cmd);
        }

        public override void SetAttachment(WeaponPartsStruct attachments)
        {
            if(null != _fireLogic)
            {
                _fireLogic.SetAttachment(attachments);
            }
            _attachmentManager.ApplyAttachment(this);
        }

        public override int GetBulletLimit()
        {
            return _fireLogic.GetBulletLimit();
        }

        public override int GetSpecialReloadCount()
        {
            return _fireLogic.GetSpecialReloadCount();
        }

        public override void Reset()
        {
            _fireLogic.Reset();
        }

        public override void SetVisualConfig(ref VisualConfigGroup config)
        {
            _fireLogic.SetVisualConfig(ref config);
        }
    }
}