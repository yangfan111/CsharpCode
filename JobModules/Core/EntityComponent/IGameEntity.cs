using System.Collections.Generic;
using Core.Components;
using Core.ObjectPool;
using Core.Utils.System46;

namespace Core.EntityComponent
{
    
    public interface IGameEntity: IRefCounter
    {
        int EntityId { get; }
        int EntityType { get; }
        EntityKey EntityKey { get; }

        // high performance
        T AddComponent<T>() where T : IGameComponent, new(); 
        IGameComponent AddComponent(int componentId);
        IGameComponent AddComponent(int componentId, IGameComponent copyValue);

        bool HasComponent<T>() where T : IGameComponent;

        // high performance
        T GetComponent<T>() where T : IGameComponent;
        IGameComponent GetComponent(int componentId);

        // high performance
        void RemoveComponent<T>() where T : IGameComponent;
        void RemoveComponent(int componentId);
        ICollection<IGameComponent> ComponentList { get; }
        
        List<IGameComponent> SortedComponentList { get; }
        MyDictionary<int, IGameComponent> SyncLatestComponentDictionary { get; }
        MyDictionary<int, IGameComponent> PlayBackComponentDictionary { get; }
        MyDictionary<int, IGameComponent> SortedCompensationComponentList { get; }
        void Destroy();
        void MarkDestroy();
        object RealEntity { get; }

        PositionComponent Position { get; }
        bool HasPositionFilter { get; }
        PositionFilterComponent PositionFilter { get; }

        bool HasOwnerIdComponent { get; }
        OwnerIdComponent OwnerIdComponent { get; }
        
        bool HasFlagImmutabilityComponent { get;  }
        
        FlagImmutabilityComponent FlagImmutabilityComponent { get; }
        LifeTimeComponent LifeTimeComponent { get; }
        bool IsCompensation { get; }
        bool IsDestroy{ get; }
        bool IsSelf{ get; }
        bool IsSyncNonSelf{ get; }
        bool IsSyncSelf{ get; }
        List<IAssetComponent> AssetComponents { get; }
       


        IGameEntity GetNonSelfEntityCopy(int snapShotSeq);
        IGameEntity GetSelfEntityCopy(int snapShotSeq);

        List<IGameComponent> GetUpdateLatestComponents();
       
    }
}