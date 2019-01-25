using System.Collections.Generic;
using Core.EntityComponent;

namespace Core.Compensation
{

    // 从Snapshot的EntityMap中，过滤出要被补偿的Entity和Component，比如HitBox相关的信息
    // 然后再插值
    public interface ISnapshotEntityMapFilter
    {
        IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetCompensationEntityList(EntityMap snapshot);
        IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetNonSelfEntityList(EntityKey self, EntityMap snapshot);
        IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetSelfEntityList(EntityKey self, EntityMap snapshot);
        IEnumerable<KeyValuePair<EntityKey, IGameEntity>> GetSyncLatestEntityList(EntityKey self, EntityMap entityMap);
    }

}