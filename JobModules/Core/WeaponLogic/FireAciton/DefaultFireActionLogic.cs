using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.FireAciton
{
    public class DefaultFireActionLogic : AbstractFireActionLogic<CommonFireConfig, int>
    {
        public DefaultFireActionLogic(CommonFireConfig config, IWeaponSoundLogic weaponSoundLogic) : base(config)
        {

        }

        public override void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
        }

        protected override void OnAfterFire(IPlayerWeaponState playerWeapon, bool needActionDeal)
        {
            playerWeapon.OnDefaultFire(needActionDeal);
        }
        
        public override void Apply(CommonFireConfig baseConfig, CommonFireConfig output, int arg)
        {
            output.MagazineCapacity = baseConfig.MagazineCapacity + arg; 
        }
    }
}
