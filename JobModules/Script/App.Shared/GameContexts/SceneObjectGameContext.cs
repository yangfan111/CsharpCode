using App.Shared;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace Assets.App.Shared.GameContexts
{
    public class SceneObjectGameContext : EntitasGameContext<SceneObjectEntity>
    {
        private SceneObjectContext _context;
        private IGroup<SceneObjectEntity> _destroyEntities;

        public SceneObjectGameContext(SceneObjectContext context, Bin2D<IGameEntity> bin) : base(context, SceneObjectComponentsLookup.componentTypes, bin)
        {
            _context = context;
        }

        protected override SceneObjectEntity  GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((SceneObjectContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.SceneObject; }
        }


        public int DestroyComponentId
        {
            get { return SceneObjectComponentsLookup.FlagDestroy; }
        }

        public Entity[] DestoryEntities
        {
            get
            {
                if (_destroyEntities == null)
                {
                    _destroyEntities = _context.GetGroup(SceneObjectMatcher.AnyOf(
                        SceneObjectMatcher.FlagDestroy));

                }
                return _destroyEntities.GetEntities();
            }
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<SceneObjectEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<SceneObjectEntity>();
            }

            public override int InitPoolSize
            {
                get { return 4096; }
            }
        }
    }
}
