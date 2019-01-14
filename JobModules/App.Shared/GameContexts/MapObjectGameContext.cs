using System;
using Core.EntitasAdpater;
using Core.EntityComponent;
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
    }
}