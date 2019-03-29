using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Playback;
using Core.Replicaton;
using Core.Utils;

namespace Core.Compensation
{
    public interface ICompensationWorldFactory
    {
        ICompensationWorld CreateCompensationWorld(int serverTime);
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
        protected abstract EntityMap CreateEntityMap(int serverTime);
        private IHitBoxEntityManager _hitboxHandler;

        protected AbstractCompensationWorldFactory(IHitBoxEntityManager hitboxHandler)
        {
            _hitboxHandler = hitboxHandler;
        }

        public ICompensationWorld CreateCompensationWorld(int serverTime)
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
        private ISnapshotSelectorContainer _snapshotSelector;
        

        public ServerCompensationWorldFactory(
            ISnapshotSelectorContainer snapshotSelectorContainer,
            IHitBoxEntityManager hitboxHandler) : base(hitboxHandler)
        {
            _snapshotSelector = snapshotSelectorContainer;
        }

        protected override EntityMap CreateEntityMap(int serverTime)
        {
            SnapshotPair pair = _snapshotSelector.SnapshotSelector.SelectSnapshot(serverTime);
            if (pair != null)
            {
                CompensationSnapshotHandler handler =
                    new CompensationSnapshotHandler(new InterpolationInfo(pair));
                var left = pair.LeftSnapshot.CompensationEntityMap;
                var right = pair.RightSnapshot.CompensationEntityMap;
                EntityMapComparator.Diff(left, right, handler, "compensation");
                return handler.TheSnapshot.EntityMap;
               
            }
            _logger.ErrorFormat("can't get snapshot at {0}, current range: {1}-{2}", 
                serverTime,
                _snapshotSelector.SnapshotSelector.OldestSnapshot.ServerTime, 
                _snapshotSelector.SnapshotSelector.LatestSnapshot.ServerTime);
            return null;
        }

        
    }
}