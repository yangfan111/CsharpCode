using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{

    public class DeleteEntityPatch : AbstractEntityPatch
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(DeleteEntityPatch)){}
            public override object MakeObject()
            {
                return new DeleteEntityPatch();
            }

        }
        public static DeleteEntityPatch Allocate()
        {
            return ObjectAllocatorHolder<DeleteEntityPatch>.Allocate();
        }

        public static DeleteEntityPatch Allocate(EntityKey key)
        {
            var rc= ObjectAllocatorHolder<DeleteEntityPatch>.Allocate();
            rc.Key = key;
            return rc;
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            ObjectAllocatorHolder<DeleteEntityPatch>.Free(this);
        }

        protected DeleteEntityPatch(EntityKey key)
        {
            Key = key;
        }


        protected DeleteEntityPatch()
        {
            
        }

        public override void ApplyPatchTo(ISnapshot snap,INetworkObjectSerializerManager serializerManager)
        {
            snap.RemoveEntity(Key);
        }

        public override void Serialize(MyBinaryWriter writer,INetworkObjectSerializerManager manager)
        {
            writer.Write((byte)EntityReplicateOperationType.Del);
            writer.Write(Key.EntityId);
            writer.Write(Key.EntityType);
        }

        public override void DeSerialize(BinaryReader reader , INetworkObjectSerializerManager manager)
        {
            int entityId = reader.ReadInt32();
            short entityType = reader.ReadInt16();
            Key = new EntityKey(entityId,entityType);
        }

       
    }
}
