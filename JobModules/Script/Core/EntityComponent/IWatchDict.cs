using Core.EntityComponent;
using Entitas;
using System.Collections.Generic;

namespace Core.EntityComponent
{
    public interface IWatchDict
    {
        HashSet<EntityKey> preEntitys_HashSet { get; }
        List<EntityKey> removeList { get;  }
        object dict { get;  }

        void Add(IGameEntity localEntity, IContexts contexts);
        void BuildEntitiesHashSet(List<IGameEntity> preEntitys);
        void ClearDictByRemoveList();
    }
}