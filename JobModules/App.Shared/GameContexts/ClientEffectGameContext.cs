using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace App.Shared.GameContexts
{
    public class ClientEffectGameContext : EntitasGameContext<ClientEffectEntity>
    {
        public ClientEffectGameContext(ClientEffectContext context, Bin2D<IGameEntity> bin) : base(context, ClientEffectComponentsLookup.componentTypes, bin)
        {
            
        }


        public override short EntityType
        {
            get { return (int)EEntityType.ClientEffect; }
        }


        protected override ClientEffectEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((ClientEffectContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<ClientEffectEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<ClientEffectEntity>();
            }

            public override int InitPoolSize
            {
                get { return 512; }
            }
        }
    }
}