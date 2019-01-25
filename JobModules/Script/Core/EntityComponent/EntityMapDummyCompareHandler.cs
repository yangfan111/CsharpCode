namespace Core.EntityComponent
{
    public class EntityMapDummyCompareHandler : IEntityMapCompareHandler
    {
        public virtual void OnLeftEntityMissing(IGameEntity rightEntity)
        {
        }

        public virtual void OnRightEntityMissing(IGameEntity leftEntity)
        {
        }

        public virtual void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
        }

        public virtual void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
        }

        public virtual bool IsBreak()
        {
            return false;
        }

        public virtual bool IsExcludeComponent(IGameComponent component)
        {
            return false;
        }

        public virtual  void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
        }

        public virtual void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
        }

        public virtual void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity,
            IGameComponent rightComponent)
        {
        }
    }
}