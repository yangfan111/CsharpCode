using Core.EntityComponent;
using Core.WeaponLogic.Bullet;
using Entitas;
using WeaponConfigNs;

namespace Core.IFactory
{
    public interface IBulletEntityFactory
    {
        Entity CreateBulletEntity(
            int cmdSeq,
            int weaponId,
            EntityKey entityKey,
            int serverTime, UnityEngine.Vector3 dir,
            IBulletFireInfoProviderDispatcher bulletFireInfoProviderDispatcher,
            BulletConfig bulletConfig,
            EBulletCaliber caliber);
    }
}
