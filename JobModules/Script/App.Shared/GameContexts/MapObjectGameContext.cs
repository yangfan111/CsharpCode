using System;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace App.Shared.GameContexts
{
    public class MapObjectGameContext:EntitasGameContext<MapObjectEntity>
    {
        public MapObjectGameContext(Context<MapObjectEntity> context, Bin2D<IGameEntity> bin) : base(context, MapObjectComponentsLookup.componentTypes, bin)
        {
            
        }

        protected override MapObjectEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((MapObjectContext) EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.MapObject; }
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<MapObjectEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<MapObjectEntity>();
            }

            public override int InitPoolSize
            {
                get { return 4096; }
            }
        }
    }
}