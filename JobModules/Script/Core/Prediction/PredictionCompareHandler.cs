using Core.Compare;
using Core.EntityComponent;
using Core.Utils;

namespace Core.Prediction
{
    public class PredictionCompareHandler<TPredictionComponent> : IEntityMapCompareHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PredictionCompareHandler<TPredictionComponent>));
        private int _remoteCmdSeq;
        public PredictionCompareHandler(int remoteCmdSeq)
        {
            _remoteCmdSeq = remoteCmdSeq;
        }

        public bool IsDiff;
        public void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            EntityKey entityKey = rightEntity.EntityKey;
            _logger.InfoFormat("cmd seq {0}, local entity missing {1}", _remoteCmdSeq, entityKey);
            IsDiff = true;
        }

        public void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.InfoFormat("cmd seq {0} local component missing {1} {2}", _remoteCmdSeq, leftEntity.EntityKey, rightComponent.GetType());
            IsDiff = true;
        }

        public void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            
        }

        public void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            bool diff;

            var comp = leftComponent as IComparableComponent;
            // ReSharper disable once PossibleNullReferenceException
            diff = !comp.IsApproximatelyEqual(rightComponent);

            
           
            if (diff)
            {
                _logger.InfoFormat("cmd seq {0} component diff key[{1}], type[{2}],\n local {3}],\n remote[{4}]",
                    _remoteCmdSeq, leftEntity.EntityKey, leftComponent.GetType(), leftComponent, rightComponent);
            }
            IsDiff = IsDiff || diff;
        }

        public void OnRightEntityMissing(IGameEntity leftEntity)
        {
            EntityKey entityKey = leftEntity.EntityKey;
            _logger.InfoFormat("cmd seq {0} remote entity missing {1}", _remoteCmdSeq, entityKey);
            IsDiff = true;
        }

        public bool IsBreak()
        {
#if UNITY_EDITOR
            return false;
#else
            return IsDiff;
#endif

        }

        public void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.InfoFormat("cmd seq {0} remote component missing {1} {2}", _remoteCmdSeq, leftEntity.EntityKey, leftComponent.GetType());
            IsDiff = true;
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