using Core.Components;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.Utils;

namespace Core.Prediction
{
 
    public class PredictionMapRewindDiffHandler : EntityMapDiffHandlerAdapter
    {
        private static LoggerAdapter _logger = new LoggerAdapter("PredictionMapRewindDiffHandler");
        private AbstractPredictionProvider rewindProvider;

        public PredictionMapRewindDiffHandler(AbstractPredictionProvider rewindProvider)
        {
            this.rewindProvider = rewindProvider;
        }

        public override void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            // sync in SyncLatestManager
            //AssertUtility.Assert(false);
            EntityKey entityKey = rightEntity.EntityKey;
            _logger.DebugFormat("create entity {0}", entityKey);
            var localEntity = rewindProvider.CreateAndGetLocalEntity(entityKey);
            foreach (var rightComponent in rightEntity.ComponentList)
            {
                if (!IsExcludeComponent(rightComponent) &&
                    localEntity.GetComponent(rightComponent.GetComponentId()) == null)
                {
                    OnLeftComponentMissing(localEntity, rightEntity, rightComponent);
                }
            }
        }

        public override void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity,
                                                    IGameComponent rightComponent)
        {
            _logger.DebugFormat("add component {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            leftEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);
        }


        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent,
                                             IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("rewind component field {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            var left = leftComponent as IRewindableComponent;
            left.RewindTo(rightComponent);
        }

        public override void OnRightEntityMissing(IGameEntity leftEntity)
        {
            // sync in SyncLatestManager
            //AssertUtility.Assert(false);

            EntityKey entityKey = leftEntity.EntityKey;
            if (!rewindProvider.IsRemoteEntityExists(entityKey))
            {
                _logger.DebugFormat("destroy entity {0}", leftEntity.EntityKey);
                rewindProvider.DestroyLocalEntity(leftEntity);
            }
            else
            {
                leftEntity.RemoveComponent<OwnerIdComponent>();
                _logger.DebugFormat("ignore destroy entity {0}", leftEntity.EntityKey);
            }
        }


        public override void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity,
                                                     IGameComponent leftComponent)
        {
            _logger.DebugFormat("remove component {0}:{1}", leftEntity.EntityKey, leftComponent.GetType());
            leftEntity.RemoveComponent(leftComponent.GetComponentId());
        }

        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is IUserPredictionComponent);
        }
    }
}