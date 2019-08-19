namespace Core.EntityComponent
{
    public interface IEntityMapDiffHandler
    {
        void OnLeftEntityMissing(IGameEntity rightEntity);
        void OnRightEntityMissing(IGameEntity leftEntity);

        void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent);
        void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent);

        bool IsBreak();
        bool IsExcludeComponent(IGameComponent component);
        
        void DoDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity);
        void DoDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity);

        void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent);


        int OnDiffEntity(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle);
    }
}