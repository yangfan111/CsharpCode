using System;
using System.Collections.Generic;
using System.IO;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    
    public class SnapshotPatch: BaseRefCounter
    {
        public static SnapshotPatch Allocate()
        {
            return ObjectAllocatorHolder<SnapshotPatch>.Allocate();
        }
        private List<AbstractEntityPatch> _entityPatchList;
        public int BaseSnapshotSeq { get; set; }

        protected SnapshotPatch()
        {
            _entityPatchList = new List<AbstractEntityPatch>();
           
        }

        protected override void OnCleanUp()
        {
            foreach (var entityPath in _entityPatchList)
            {
                entityPath.ReleaseReference();
            }
            _entityPatchList.Clear();
            ObjectAllocatorHolder<SnapshotPatch>.Free(this);
        }

        public void AddEntityPatch(AbstractEntityPatch entityPatch)
        {
            _entityPatchList.Add(entityPatch);
            entityPatch.AcquireReference();
        }
 
        public void ApplyPatchTo(ISnapshot baseSnap,INetworkObjectSerializerManager serializerManager)
        {
            foreach (var patch in _entityPatchList)
            {
               
                patch.ApplyPatchTo(baseSnap, serializerManager);
            }
        }

        public AbstractEntityPatch GetEntityPatch(int index)
        {
            return _entityPatchList[index];
        }

        public int GetEntityPatchCount()
        {
            return _entityPatchList.Count;
        }

       
        public void Serialize(MyBinaryWriter writer,INetworkObjectSerializerManager serializerManager)
        {
            writer.Write(BaseSnapshotSeq);
            writer.Write(_entityPatchList.Count);
            foreach (var item in _entityPatchList)
            {
                item.Serialize(writer, serializerManager);
            }
        }

        public void DeSerialize(BinaryReader reader , INetworkObjectSerializerManager serializerManager)
        {
            BaseSnapshotSeq = reader.ReadInt32();
            AssertUtility.Assert(_entityPatchList.Count == 0);
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                AbstractEntityPatch patch = CreateEntityPatch((EntityReplicateOperationType)reader.ReadByte());
                patch.DeSerialize(reader, serializerManager);
                _entityPatchList.Add(patch);
            }
        }

        private AbstractEntityPatch CreateEntityPatch(EntityReplicateOperationType type)
        {
            switch (type)
            {
                case EntityReplicateOperationType.Add:
                    return Patch.AddEntityPatch.Allocate();
                case EntityReplicateOperationType.Del:
                    return DeleteEntityPatch.Allocate();
                case EntityReplicateOperationType.Mod:
                    return ModifyEntityPatch.Allocate();
                default:
                    throw new Exception(string.Format("type {0} not exist",type));
            }
        }

       
    }
}
