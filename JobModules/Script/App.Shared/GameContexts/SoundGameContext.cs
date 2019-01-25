using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace App.Shared.GameContexts
{
    public class SoundGameContext : EntitasGameContext<SoundEntity>
    {
        private SoundContext _context;
        private IGroup<SoundEntity> _destroyEntities;

        public SoundGameContext(SoundContext context, Bin2D<IGameEntity> bin) : base(context, SoundComponentsLookup.componentTypes, bin)
        {
            _context = context;
        }

        protected override SoundEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((SoundContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.Sound; }
        }


        public int DestroyComponentId
        {
            get { return SoundComponentsLookup.FlagDestroy; }
        }

        public Entity[] DestoryEntities
        {
            get
            {
                if (_destroyEntities == null)
                {
                    _destroyEntities = _context.GetGroup(SoundMatcher.AnyOf(
                        SoundMatcher.FlagDestroy));

                }
                return _destroyEntities.GetEntities();
            }
        }
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<SoundEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<SoundEntity>();
            }

            public override int InitPoolSize
            {
                get { return 128; }
            }
        }
    }
}
