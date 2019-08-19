using Core.SyncLatest;

namespace Core.EntityComponent
{
    public class EntityDiffData
    {
        public IGameEntity LeftEntity;
        public IGameEntity RightEntity;
    }
    public class EntityMapDiffHandlerAdapter : IEntityMapDiffHandler
    {
        protected INetSyncProvider netSyncProvider;
        public EntityMapDiffHandlerAdapter(INetSyncProvider netSyncProvider)
        {
            this.netSyncProvider = netSyncProvider;
        }
        public EntityMapDiffHandlerAdapter(){}

        protected IGameEntityCompareAgent EntityCompareAgent;

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

        public virtual  void DoDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
        }

        public virtual void DoDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
        }

        public virtual void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity,
            IGameComponent rightComponent)
        {
          
        }

      

        public int OnDiffEntity(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
        {
            return EntityCompareAgent.Diff(leftEntity, rightEntity, skipMissHandle);
            // diffAgent.LeftEntity  = leftEntity;
            // diffAgent.RightEntity = rightEntity;
            // DoDiffEntityStart(leftEntity,rightEntity);
            // int count = EntityCompareHelper.CompareSortedComponents(
            //     leftEntity.SortedComponentList,
            //     rightEntity.SortedComponentList,
            //     this,diffAgent);
            //
            // DoDiffEntityFinish(leftEntity, rightEntity);
            // return count;
        }
    }
}