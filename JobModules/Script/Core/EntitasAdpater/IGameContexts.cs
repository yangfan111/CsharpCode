using Core.EntityComponent;

namespace Core.EntitasAdpater
{
    public interface IGameContexts
    {
        
        IGameEntity CreateAndGetGameEntity(EntityKey entityKey);
        IGameContext[] AllContexts { get; }
        EntityMap EntityMap { get; }
        EntityMap LatestEntityMap { get; }
        EntityMap SelfEntityMap { get; }
        EntityMap NonSelfEntityMap { get; }
        EntityMap CompensationEntityMap { get; }
        EntityKey Self { get; set; }

        IGameEntity GetGameEntity(EntityKey entityKey);
    }
}