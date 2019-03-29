using App.Shared;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace Assets.App.Shared.GameContexts
{
    public class FreeMoveGameContext : EntitasGameContext<FreeMoveEntity>
    {
        private FreeMoveContext _context;
        private IGroup<FreeMoveEntity> _destroyEntities;

        public FreeMoveGameContext(FreeMoveContext context, Bin2D<IGameEntity> bin) : base(context, FreeMoveComponentsLookup.componentTypes, bin)
        {
            _context = context;
        }

        protected override FreeMoveEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((FreeMoveContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.FreeMove; }
        }


        public int DestroyComponentId
        {
            get { return FreeMoveComponentsLookup.FlagDestroy; }
        }

        public Entity[] DestoryEntities
        {
            get
            {
                if (_destroyEntities == null)
                {
                    _destroyEntities = _context.GetGroup(FreeMoveMatcher.AnyOf(
                        FreeMoveMatcher.FlagDestroy));

                }
                return _destroyEntities.GetEntities();
            }
        }
        
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<FreeMoveEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<FreeMoveEntity>();
            }

            public override int InitPoolSize
            {
                get { return 256; }
            }
        }
    }
}
