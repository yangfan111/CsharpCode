using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;

namespace Core.SnapshotReplication.Serialization.Clone
{
    public class EntityCloner
    {
      
        public static IGameEntity Clone(IGameEntity entity)
        {
            var entityCopy = GameEntity.Allocate(entity.EntityKey);
            foreach (var comp in entity.ComponentList)
            {
                
                var compCopy = entityCopy.AddComponent(comp.GetComponentId());
                (compCopy as INetworkObject).CopyFrom(comp);
            }
            return entityCopy;
        }
    }
}
