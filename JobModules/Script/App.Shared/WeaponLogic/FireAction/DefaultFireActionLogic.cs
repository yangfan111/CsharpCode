using WeaponConfigNs;

namespace Core.WeaponLogic.FireAciton
{
    public class DefaultFireActionLogic : AbstractFireActionLogic<CommonFireConfig>
    {
        public DefaultFireActionLogic(Contexts contexts):base(contexts)
        {

        }

        public override void OnIdle(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
        }

        protected override void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, bool needActionDeal)
        {
            DefaultFire(playerEntity);
        }
    }
}
