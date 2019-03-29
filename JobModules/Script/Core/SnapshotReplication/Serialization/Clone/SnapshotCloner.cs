using Core.Replicaton;

namespace Core.SnapshotReplication.Serialization.Clone
{
    public class SnapshotCloner
    {
        public static ISnapshot Clone(ISnapshot orig)
        {
            var snapshotCopy = CloneSnapshot.Allocate();
            snapshotCopy.Header = orig.Header;
            foreach (var item in orig.EntityMap)
            {
                var entity = EntityCloner.Clone(item.Value);
                snapshotCopy.AddEntity(entity);
                entity.ReleaseReference();
            }
            return snapshotCopy;
        }
    }
}
