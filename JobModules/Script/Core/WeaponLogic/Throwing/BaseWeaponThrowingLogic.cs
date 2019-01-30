using Core.WeaponLogic.Common;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.Throwing
{
    public class BaseWeaponThrowingLogic : AbstractAttachableWeaponLogic<CommonFireConfig, int>, IThrowingContainer
    {
        private IAttachmentManager _attachmentManager;
        public BaseWeaponThrowingLogic(CommonFireConfig config, IAttachmentManager attachmentManager) : base(config)
        {
            _config = config;
            _attachmentManager = attachmentManager;
        }
        
        public override void Apply(CommonFireConfig baseConfig, CommonFireConfig output, int arg)
        {
            output.MagazineCapacity = baseConfig.MagazineCapacity + arg; 
        }
    }
}
