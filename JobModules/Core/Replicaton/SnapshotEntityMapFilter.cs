using System.Collections.Generic;
using System.Linq;
using Core.Compensation;
using Core.EntityComponent;

namespace Core.Replicaton
{
    public class SnapshotEntityMapFilter: ISnapshotEntityMapFilter
    {
        ReplicationFilter _filter = new ReplicationFilter();

       
        public IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetCompensationEntityList(EntityMap entityMap)
        {
            return entityMap.Where(entry => _filter.IsCompensation(entry.Value));
        }

        public IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetNonSelfEntityList(EntityKey self, EntityMap entityMap)
        {
            return entityMap.Where(entry =>
                _filter.IsSyncNonSelf(entry.Value, self)
            );

          
        }
        
        public IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetSelfEntityList(EntityKey self, EntityMap entityMap)
        {
            return entityMap.Where(entry => _filter.IsSyncSelf(entry.Value, self)
            );
        }

        public IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetSyncLatestEntityList(EntityKey self, EntityMap entityMap)
        {
            return entityMap.Where(entry => _filter.IsSyncSelfOrThird(entry.Value, self));
        }
        
    }
}