namespace Core.EntityComponent
{
    public interface IEntityMapCompareHandler
    {
        void OnLeftEntityMissing(IGameEntity rightEntity);
        void OnRightEntityMissing(IGameEntity leftEntity);

        void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent);
        void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent);

        bool IsBreak();
        bool IsExcludeComponent(IGameComponent component);
        
        void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity);
        void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity);

        void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent);
    }
}