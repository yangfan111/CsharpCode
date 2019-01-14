using System.Collections.Generic;
using Core.EntitasAdpater;

namespace Core.EntityComponent
{
    public delegate void EntityRemoved(IGameEntity entity);
    public delegate void EntityChanged(IGameEntity entity, int index);
    
    public interface IGameContext
    {
        event EntityRemoved EntityAdded;
        event EntityRemoved EntityRemoved;
        event EntityChanged EntityComponentChanged;
        List<IGameEntity> GetEntities();
        short EntityType { get; }
        IGameEntity CreateAndGetEntity(EntityKey entityKey);
        IGameEntity GetEntity(EntityKey entityKey);
        bool CanContainComponent<TComponent>() where TComponent: IGameComponent;
        
        IGameGroup CreateGameGroup<TComponent>() where TComponent : IGameComponent;
    }
}