using Core.WeaponLogic.Common;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.Bullet
{
    public class BaseWeaponBulletLogic : AbstractAttachableWeaponLogic<CommonFireConfig, int>, IBulletContainer 
    {
        private IAttachmentManager _attachmentManager;
        public BaseWeaponBulletLogic(CommonFireConfig config, IAttachmentManager attachmentManager):base(config)
        {
            _attachmentManager = attachmentManager;
        }        

        public int GetBulletLimit()
        {
            return _config.MagazineCapacity;
        }

        public int GetSpecialReloadCount()
        {
            return _config.SpecialReloadCount;
        }
        
        public void SetAttachments(WeaponPartsStruct attachments)
        {
            _attachmentManager.ApplyAttachment(this);
        }
        public override void Apply(CommonFireConfig baseConfig, CommonFireConfig output, int arg)
        {
            output.MagazineCapacity = baseConfig.MagazineCapacity + arg; 
        }
    }
}
