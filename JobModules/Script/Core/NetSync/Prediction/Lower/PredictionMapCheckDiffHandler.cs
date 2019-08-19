using System;
using Core.Compare;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.SyncLatest;
using Core.Utils;

namespace Core.Prediction
{
    /// <summary>
    /// rediction设置IsDiff = true
    /// 不同比较：IsApproximatelyEqual
    /// </summary>
    /// <typeparam name="TPredictionComponent"></typeparam>
    public class PredictionMapCheckDiffHandler: EntityMapDiffHandlerAdapter
    {
        private static LoggerAdapter _logger = new LoggerAdapter("Core.Prediction.PredictionCompareHandler." );
        private int remoteCmdSeq;
        public void SetRemoteCmdSeq(int remoteCmdSeq)
        {
            this.remoteCmdSeq = remoteCmdSeq;
        }

        public bool IsDiff;
        public override void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            _logger.InfoFormat("cmd seq {0}, local entity missing {1}", remoteCmdSeq,  rightEntity.EntityKey);
            IsDiff = true;
        }

        public override void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.InfoFormat("cmd seq {0} local component missing {1} {2}", remoteCmdSeq, leftEntity.EntityKey, rightComponent.GetType());
            IsDiff = true;
        }

    
        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            bool diff;

            var comp = leftComponent as IComparableComponent;
            // ReSharper disable once PossibleNullReferenceException
            diff = !comp.IsApproximatelyEqual(rightComponent);
           
            if (diff)
            {
                _logger.InfoFormat("cmd seq {0} component diff key[{1}], type[{2}],\n local {3}],\n remote[{4}]",
                    remoteCmdSeq, leftEntity.EntityKey, leftComponent.GetType(), leftComponent, rightComponent);
                IsDiff = true;
            }
        }

        public override void OnRightEntityMissing(IGameEntity leftEntity)
        {
            _logger.InfoFormat("cmd seq {0} remote entity missing {1}", remoteCmdSeq, leftEntity.EntityKey);
            IsDiff = true;
        }

        public override bool IsBreak()
        {
#if UNITY_EDITOR
            return false;
#else
            return IsDiff;
#endif

        }

        public override void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.InfoFormat("cmd seq {0} remote component missing {1} {2}", remoteCmdSeq, leftEntity.EntityKey, leftComponent.GetType());
            IsDiff = true;
        }

        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is IUserPredictionComponent);
        }

     
    }
}