using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.FireAciton
{
    public class SpecialFireActionLogic : AbstractFireActionLogic<CommonFireConfig, int>
    {
        private IWeaponSoundLogic _weaponSoundLogic;
        public SpecialFireActionLogic(CommonFireConfig config, IWeaponSoundLogic weaponSoundLogic):base(config)
        {
            _weaponSoundLogic = weaponSoundLogic; 
        }

        public override void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if(playerWeapon.PullBolting && !playerWeapon.IsFireEnd && !playerWeapon.IsFireHold)
            {
                playerWeapon.PullBolting = false;
            }
            if(playerWeapon.IsFireHold)
            {
                playerWeapon.EndSpecialFire();
                playerWeapon.PullBolting = true;
            }
        }
        public override void Apply(CommonFireConfig baseConfig, CommonFireConfig output, int arg)
        {
            output.MagazineCapacity = baseConfig.MagazineCapacity + arg; 
        }

        protected override void OnAfterFire(IPlayerWeaponState playerWeapon, bool needActionDeal)
        {
            if(playerWeapon.LoadedBulletCount > 0)
            {
                playerWeapon.OnSpecialFire(needActionDeal);
            }
            else
            {
                playerWeapon.OnDefaultFire(needActionDeal);
            }
        }
    }
}
