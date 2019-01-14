using App.Shared.GameModules.Common;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.GameModule.Common;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;



namespace App.Shared.ContextInfos
{
    public class BulletGameContext : EntitasGameContext<BulletEntity>
    {
        public BulletGameContext(BulletContext context, Bin2D<IGameEntity> bin) : base(context, BulletComponentsLookup.componentTypes, bin)
        {
            
        }


        protected override BulletEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((BulletContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.Bullet; }
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<BulletEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<BulletEntity>();
            }

            public override int InitPoolSize
            {
                get { return 512; }
            }
        }

      
    }
}