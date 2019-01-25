using System;
using App.Shared.GameModules.Common;


using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.GameModule.Common;
using Core.ObjectPool;
using Core.SpatialPartition;
using Entitas;

namespace App.Shared.ContextInfos
{
    public class PlayerGameContext : EntitasGameContext<PlayerEntity>
    {
        public PlayerGameContext(PlayerContext context, Bin2D<IGameEntity> bin) : base(context, PlayerComponentsLookup.componentTypes, bin)
        {

        }

        protected override PlayerEntity GetEntityWithEntityKey(EntityKey entitykey)
        {
            return ((PlayerContext)EntitasContext).GetEntityWithEntityKey(entitykey);
        }

        public override short EntityType
        {
            get { return (int)EEntityType.Player; }
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof( EntitasGameEntity<PlayerEntity>))
            {
            }

            public override object MakeObject()
            {
                return new  EntitasGameEntity<PlayerEntity>();
            }

            public override int InitPoolSize
            {
                get { return 64; }
            }
        }
      
      
    }
}