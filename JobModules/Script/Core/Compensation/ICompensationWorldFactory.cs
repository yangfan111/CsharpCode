using Core.EntityComponent;
using Core.EntityComponent;
using Core.Playback;
using Core.Replicaton;
using Core.Utils;

namespace Core.Compensation
{
    public interface ICompensationWorldFactory
    {
        CompensationWorld CreateCompensationWorld(int serverTime);
    }

    public class ClientCompensationWorldFactory : AbstractCompensationWorldFactory
    {
        private IGameContexts _gameContexts;
        public ClientCompensationWorldFactory(IGameContexts gameContexts, IHitBoxEntityManager hitboxHandler)
            : base(hitboxHandler)
        {

            _gameContexts = gameContexts;
        }

        
        protected override EntityMap CreateEntityMap(int serverTime)
        {
            var rc = _gameContexts.CompensationEntityMap; 
            rc.AcquireReference();
            return rc;
        }
    }

    public abstract class AbstractCompensationWorldFactory : ICompensationWorldFactory
    {
        /// <summary>
        /// 客户端取唯一CompensationWorld
        /// 服务端取当前World时间片
        /// </summary>
        /// <param name="serverTime"></param>
        /// <returns></returns>
        protected abstract EntityMap CreateEntityMap(int serverTime);
        /// <summary>
        /// IHitBoxEntityManager hitBoxEntityManager = new HitBoxEntityManager(contexts, isServer);
        /// </summary>
        private IHitBoxEntityManager _hitboxHandler;

        protected AbstractCompensationWorldFactory(IHitBoxEntityManager hitboxHandler)
        {
            _hitboxHandler = hitboxHandler;
        }

        public CompensationWorld CreateCompensationWorld(int serverTime)
        {
            EntityMap entityMap = CreateEntityMap(serverTime);
            if (entityMap != null)
            {
                var rc = CompensationWorld.Allocate();
                rc.Init(serverTime, entityMap, _hitboxHandler);
                entityMap.ReleaseReference();
                return rc;
            }
            return null;
        }
    }
    public class ServerCompensationWorldFactory : AbstractCompensationWorldFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerCompensationWorldFactory));
      
        private ISnapshotSelector _snapshotSelector;
        

        public ServerCompensationWorldFactory(
            ISnapshotSelector snapshotSelectorContainer,
            IHitBoxEntityManager hitboxHandler) : base(hitboxHandler)
        {
            _snapshotSelector = snapshotSelectorContainer;
        }

        protected override EntityMap CreateEntityMap(int serverTime)
        {
            SnapshotPair pair = _snapshotSelector.SelectSnapshot(serverTime);
            if (pair != null)
            {
                CompensationMapDiffHandler diffHandler = new CompensationMapDiffHandler(new InterpolationInfo(pair));
                EntityMap left = pair.LeftSnapshot.CompensationEntityMap;
                EntityMap right = pair.RightSnapshot.CompensationEntityMap;
                EntityMapCompareExecutor.Diff(left, right, diffHandler, "compensation",null);
                return diffHandler.TheSnapshot.EntityMap;
               
            }
            _logger.ErrorFormat("can't get snapshot at {0}, current range: {1}-{2}", 
                serverTime,
                _snapshotSelector.OldestSnapshot.ServerTime, 
                _snapshotSelector.LatestSnapshot.ServerTime);
            return null;
        }

        
    }
}