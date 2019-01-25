using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;

namespace App.Shared.GameContexts
{
    public class WeaponGameContext : EntitasGameContext<WeaponEntity>
    {
        public WeaponGameContext(WeaponContext context, Bin2D<IGameEntity> bin) : base(context, WeaponComponentsLookup.componentTypes, bin)
        {
            
        }

        protected override WeaponEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return null;
        }

        public override short EntityType
        {
            get { return (int)EEntityType.Weapon; }
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<WeaponEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<WeaponEntity>();
            }

            public override int InitPoolSize
            {
                get { return 128; }
            }
        }
    }
}
