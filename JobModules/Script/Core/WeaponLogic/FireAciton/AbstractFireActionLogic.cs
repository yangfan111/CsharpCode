using Assets.Utils.Configuration;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Common;
using Utils.Singleton;
using WeaponConfigNs;

namespace Core.WeaponLogic.FireAciton
{
    public abstract class AbstractFireActionLogic<T1,  T3> : AbstractAttachableWeaponLogic<T1, T3>, IFireActionLogic where T1 : ICopyableConfig<T1>, new()
    {
        public AbstractFireActionLogic(T1 config):base(config)
        {
             
        }
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractFireActionLogic<T1, T3>));
        public virtual void AfterFireCmd(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bulletCount)
        {
            if(bulletCount > 0)
            {
                var needActionDeal = CheckNeedActionDeal(playerWeapon.CurrentWeapon, ActionDealEnum.Fire);
                OnAfterFire(playerWeapon, needActionDeal);
            }
            else
            {
                Logger.Debug("bullet count is zero !!");
            }
       }

        protected abstract void OnAfterFire(IPlayerWeaponState playerWeapon, bool needActionDeal);
        public abstract void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd);

        protected bool CheckNeedActionDeal(int weaponId, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponConfigManager>().NeedActionDeal(weaponId, ActionDealEnum.Reload);
        }       
    }
}
