namespace Core.EntityComponent
{
    //比对ILastestComponent
      public class GameEntitySelfLatestCompareAgent : AbstractGameEntityCompareAgent
    {
        private int serverTime;
    
        public void Init(IEntityMapDiffHandler handler, int serverTime)
        {
            base.Init(handler);
            this.serverTime = serverTime;
        }
#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
            //YF TODO:
           
            bool needSkip = false;
            if (leftEntity.HasFlagImmutabilityComponent && rightEntity.HasFlagImmutabilityComponent)
            {
                var local = leftEntity.FlagImmutabilityComponent;
                var remote = rightEntity.FlagImmutabilityComponent;
                if (local.LastModifyServerTime != remote.LastModifyServerTime)
                {
                    local.Reset();
                }else  if (local.JudgeNeedSkipSyncLatest(serverTime))
                {
                    needSkip = true;
                }
            }

            if (needSkip)
            {
                return 0;
            }

            diffCacheData.LeftEntity = leftEntity;
            diffCacheData.RightEntity = rightEntity;
            handler.DoDiffEntityStart(leftEntity, rightEntity);
            int count = EntityCompareHelper.CompareEnumeratableGameComponents(leftEntity.SyncLatestComponentDict,
                rightEntity.SyncLatestComponentDict, handler, diffCacheData);
            handler.DoDiffEntityFinish(leftEntity, rightEntity);
            return count;
        }
    }
}