using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Common;
using WeaponConfigNs;

namespace Core.WeaponLogic.Spread
{
    public abstract class AbstractSpreadLogic<T1, T3> : AbstractAttachableWeaponLogic<T1,  T3>, ISpreadLogic where T1 : ICopyableConfig<T1>, new()
    {
        protected abstract float UpdateSpread(IPlayerWeaponState playerWeapon, float accuracy);
        public abstract void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet);

        protected AbstractSpreadLogic(T1 config) : base(config)
        {
        }
    }
}