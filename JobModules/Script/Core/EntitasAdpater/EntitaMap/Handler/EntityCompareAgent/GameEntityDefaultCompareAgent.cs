namespace Core.EntityComponent
{
    public class GameEntityDefaultCompareAgent : AbstractGameEntityCompareAgent
    {
        public GameEntityDefaultCompareAgent(IEntityMapDiffHandler handler) 
        {
            base.Init(handler);
        }

#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
            diffCacheData.LeftEntity  = leftEntity;
            diffCacheData.RightEntity = rightEntity;
            handler.DoDiffEntityStart(leftEntity, rightEntity);

            int count = EntityCompareHelper.CompareEnumeratableGameComponents(
                leftEntity.SortedComponentList,
                rightEntity.SortedComponentList,
                handler,diffCacheData);

            handler.DoDiffEntityFinish(leftEntity, rightEntity);
            return count;
        }
    }
}