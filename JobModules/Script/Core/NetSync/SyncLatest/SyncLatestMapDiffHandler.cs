using Core.EntityComponent;
using Core.Prediction;
using Core.SyncLatest;
using Core.Utils;

namespace Core.EntityComponent
{
    /// <summary>
    /// 与PredictionRewind除了provider一模一样
    /// 
    /// </summary>
    public class SyncLatestMapDiffHandler : EntityMapDiffHandlerAdapter
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SyncLatestMapDiffHandler));
        public SyncLatestMapDiffHandler(INetSyncProvider netSyncProvider) : base(netSyncProvider)
        {
        }

        public override void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            var entityKey = rightEntity.EntityKey;
            IGameEntity localEntity = netSyncProvider.CreateAndGetLocalEntity(entityKey);
            _logger.DebugFormat("create entity {0}", entityKey);
            foreach (var rightComponent in rightEntity.ComponentList)
            {
                if (!IsExcludeComponent(rightComponent) && localEntity.GetComponent(rightComponent.GetComponentId()) == null)
                {
                    OnLeftComponentMissing(localEntity, rightEntity, rightComponent);
                    // _logger.DebugFormat("{2}add component {0}:{1}", entityKey, rightComponent.GetType(), rightComponent);
                    // var leftComponent = (ILatestComponent) localEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);

                }
            }
        }

        public override void OnRightEntityMissing(IGameEntity leftEntity)
        {
            _logger.DebugFormat("destroy entity {0}", leftEntity.EntityKey);
            netSyncProvider.DestroyLocalEntity(leftEntity);
        }

        public override void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("add component {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            leftEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);
            
        }

        public override void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.DebugFormat("remove component {0}:{1}", leftEntity.EntityKey, leftComponent.GetType());
            leftEntity.RemoveComponent(leftComponent.GetComponentId());
        }


        public override bool IsExcludeComponent(IGameComponent component)
        {
            if (!(component is ILatestComponent))
                return true;
            //不是自己
            if (_isSelfEntity && component is INonSelfLatestComponent && !(component is ISelfLatestComponent))
                return true;
            if ((component is IVehicleLatestComponent) && !((IVehicleLatestComponent) component).IsSyncLatest)
                return true;
           
            return false;
        }

        private bool _isSelfEntity; 
        public override void DoDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            _isSelfEntity= netSyncProvider.IsSelf(leftEntity.EntityKey);
        }


        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity,
            IGameComponent rightComponent)
        {
            (leftComponent as ILatestComponent).SyncLatestFrom(rightComponent);
        }
    }
}