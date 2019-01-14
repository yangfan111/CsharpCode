using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas;

namespace Core.Components
{
    
    public class OwnerIdComponent : IGameComponent, ISelfLatestComponent, INonSelfLatestComponent,IResetableComponent
    {
        [NetworkProperty]
        public EntityKey Value;

        public OwnerIdComponent()
        {
            
        }

        public OwnerIdComponent(EntityKey data)
        {
            Value = data;
        }
        public override string ToString()
        {
            return string.Format("{0}", Value);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void Reset()
        {
            Value = EntityKey.Default;
        }


        public void CopyFrom(object rightComponent)
        {
            // ReSharper disable once PossibleNullReferenceException
            var r = rightComponent as OwnerIdComponent;
            Value = r.Value;
        }

      

        public int GetComponentId() { { return (int)ECoreComponentIds.OwnerId; } }
    }
}
