using System;
using Core.EntityComponent;

namespace Core.EntitasAdpater
{
    public interface IComponentTypeLookup
    {
        int[] IndexByComponentId { get; }
        int MaxIndex { get; }
        int GetComponentIndex<T>() where T : IGameComponent;
        int GetComponentIndex(int componentId);
        Type GetComponentType(int componentId);
        
        int EntityAdapterComponentIndex { get; }
        int EntityKeyComponentIndex { get; }
        int FlagCompensationComponentIndex { get; }
        int FlagDestroyComponentIndex { get; }
        int FlagSelfComponentIndex { get; }
        int FlagSyncNonSelfComponentIndex { get; }
        int FlagSyncSelfComponentIndex { get; }
        int PositionComponentIndex { get; }
        int FlagPositionFilterComponentIndex { get; }
        int OwnerIdComponentIndex { get; }
        int FlagImmutabilityComponentIndex { get; }
        int LifeTimeComponentIndex { get; }
        int[] AssetComponentIndexs { get; }
        int[] SelfIndexs { get;  }
        int[] NoSelfIndexs { get;  }
        int[] UpdateLatestIndexs { get;  }
        int[] SyncLatestIndexs { get;  }
        int[] PlaybackIndexs { get;  }
        int[] CompensationIndexs { get;  }
       
    }


}