using System;
using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Bullet;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class DoubleWeaponLogic : AbstractWeaponLogic<DoubleWeaponLogicConfig, object>
    {
        private IFireLogic _leftFireLogic;
        private IFireLogic _rightFireLogic;
        private LeftWeaponCmd _leftcmd = new LeftWeaponCmd();
        private RightWeaponCmd _rightCmd = new RightWeaponCmd();
        private IAttachmentManager _attachmentManager;
        public DoubleWeaponLogic(NewWeaponConfigItem newCfg, 
            DoubleWeaponLogicConfig config, 
            IWeaponLogicComponentsFactory componentsFactory, 
            IWeaponSoundLogic soundLogic, 
            IWeaponEffectLogic effectLogic, 
            IAttachmentManager attachmentManager,
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher) : base(config, componentsFactory)
        {
            _leftFireLogic = componentsFactory.CreateFireLogic(newCfg, config.LeftFireLogic, soundLogic, effectLogic, bulletFireInfoProviderDispatcher);
            _rightFireLogic = componentsFactory.CreateFireLogic(newCfg, config.RightFireLogic, soundLogic, effectLogic, bulletFireInfoProviderDispatcher);
            _attachmentManager = attachmentManager;
        }

        public override void Apply(DoubleWeaponLogicConfig baseConfig, DoubleWeaponLogicConfig output, object arg)
        {
           
        }
        public override void SetAttachment(WeaponPartsStruct attachments)
        {

        }

        public override void Update(IPlayerWeaponState playerWeapon, IUserCmd cmd)
        {
            _leftcmd.SetCurrentCmd(cmd);
            _rightCmd.SetCurrentCmd(cmd);
            _leftFireLogic.OnFrame(playerWeapon, _leftcmd);
            _rightFireLogic.OnFrame(playerWeapon, _rightCmd);
        }

        public override int GetBulletLimit()
        {
            return 0;
        }

        public override int GetSpecialReloadCount()
        {
            return 0;
        }

        public override void Reset()
        {
            _leftFireLogic.Reset();
            _rightFireLogic.Reset();
        }

        public override void SetVisualConfig(ref VisualConfigGroup config)
        {

        }
    }
}