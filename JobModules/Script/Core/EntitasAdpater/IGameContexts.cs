using Core.EntityComponent;

namespace Core.EntityComponent
{
    public interface IGameContexts
    {
        
        IGameEntity CreateAndGetGameEntity(EntityKey entityKey);
        IGameContext[] AllContexts { get; }
        EntityMap MyEntityMap { get; }
        EntityMap LatestEntityMap { get; }
        EntityMap SelfEntityMap { get; }
        EntityMap NonSelfEntityMap { get; }
        EntityMap CompensationEntityMap { get; }
        EntityKey Self { get; set; }

        IGameEntity GetGameEntity(EntityKey entityKey);
        bool TryGetGameEntity(EntityKey entityKey, out IGameEntity entity);
    }
}