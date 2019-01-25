using Core.Components;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.Utils;

namespace Core.Prediction
{
    public class PredictionRewindHandler<TPredictionComponent> : IEntityMapCompareHandler where TPredictionComponent:IPredictionComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<TPredictionComponent>.LoggerName);
        private IPredictionRewindInfoProvider _handler;

        public PredictionRewindHandler(IPredictionRewindInfoProvider handler)
        {
            _handler = handler;
        }

        public void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            // sync in SyncLatestManager
            //AssertUtility.Assert(false);
            EntityKey entityKey = rightEntity.EntityKey;
            _logger.DebugFormat("create entity {0}", entityKey);
            var localEntity = _handler.CreateAndGetLocalEntity(entityKey);
            foreach (var rightComponent in rightEntity.ComponentList)
            {
                
                if (!IsExcludeComponent(rightComponent) && localEntity.GetComponent(rightComponent.GetComponentId()) == null)
                {
                    _logger.DebugFormat("add component {0}:{1}", entityKey, rightComponent.GetType());
                    var localComponent = localEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);
                    
                    
                }
            }
        }

        public void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("add component {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            var localCompoent = leftEntity.AddComponent(rightComponent.GetComponentId(), rightComponent);
           
        }

        public void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            
        }

        public void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("rewind component field {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());

            var left = leftComponent as IRewindableComponent;
          
            left.RewindTo(rightComponent);
            
        }

        public void OnRightEntityMissing(IGameEntity leftEntity)
        {
            // sync in SyncLatestManager
            //AssertUtility.Assert(false);

            EntityKey entityKey = leftEntity.EntityKey;
            if (!_handler.IsRemoteEntityExists(entityKey))
            {
                _logger.DebugFormat("destroy entity {0}", leftEntity.EntityKey);
                _handler.DestroyLocalEntity(leftEntity);
            }
            else
            {
                leftEntity.RemoveComponent<OwnerIdComponent>();
                _logger.DebugFormat("ignore destroy entity {0}", leftEntity.EntityKey);
            }
        }

        public bool IsBreak()
        {
            return false;
        }

        public void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.DebugFormat("remove component {0}:{1}", leftEntity.EntityKey, leftComponent.GetType());
            leftEntity.RemoveComponent(leftComponent.GetComponentId());
        }

        public bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is TPredictionComponent);
        }

        public void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            
        }
    }
}