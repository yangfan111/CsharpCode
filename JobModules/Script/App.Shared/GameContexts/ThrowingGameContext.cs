using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;

namespace App.Shared.ContextInfos
{
    public class ThrowingGameContext : EntitasGameContext<ThrowingEntity>
    {
        public ThrowingGameContext(ThrowingContext context, Bin2D<IGameEntity> bin) : base(context, ThrowingComponentsLookup.componentTypes, bin)
        {

        }

        protected override ThrowingEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((ThrowingContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.Throwing; }
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<ThrowingEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<ThrowingEntity>();
            }

            public override int InitPoolSize
            {
                get { return 128; }
            }
        }
    }
}