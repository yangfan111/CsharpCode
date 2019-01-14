using System.Collections.Generic;
using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public class AddEntityPatch : AbstractEntityPatch
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(AddEntityPatch)){}
            public override object MakeObject()
            {
                return new AddEntityPatch();
            }

        }
        public static AddEntityPatch Allocate()
        {
            return ObjectAllocatorHolder<AddEntityPatch>.Allocate();
        }

        public static AddEntityPatch Allocate(EntityKey key)
        {
            var rc= ObjectAllocatorHolder<AddEntityPatch>.Allocate();
            rc.Key = key;
            return rc;
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            ObjectAllocatorHolder<AddEntityPatch>.Free(this);
        }

        protected AddEntityPatch()
        {
            ComponentPatchList = new List<AbstractComponentPatch>();
        }


        public override void ApplyPatchTo(ISnapshot snap,INetworkObjectSerializerManager serializerManager)
        {
            IGameEntity entity = snap.GetOrCreate(Key);
            for(int i = 0;i <  ComponentPatchList.Count;i++)
            {
                ComponentPatchList[i].ApplyPatchTo(entity,serializerManager);
            }
        }

        public override void Serialize(MyBinaryWriter writer,INetworkObjectSerializerManager serializerManager)
        {
            writer.Write((byte)EntityReplicateOperationType.Add);
            writer.Write(Key.EntityId);
            writer.Write(Key.EntityType);
            writer.Write(ComponentPatchList.Count);
            for (int i = 0; i < ComponentPatchList.Count; i++)
            {
                ComponentPatchList[i].Serialize(writer, serializerManager);
            }
        }

        public override void DeSerialize(BinaryReader reader, INetworkObjectSerializerManager serializerManager)
        {
            var id = reader.ReadInt32();
            var type = reader.ReadInt16();
            Key = new EntityKey(id,type);
            int count = reader.ReadInt32();
            AssertUtility.Assert(ComponentPatchList.Count == 0);
            for (int i = 0; i < count; i++)
            {
                reader.ReadByte(); // var op
                var patch = CreateEmptyComponentPatch();
                patch.DeSerialize(reader, serializerManager);
                ComponentPatchList.Add(patch);
            }
        }

        private AbstractComponentPatch CreateEmptyComponentPatch()
        {
            return Patch.AddComponentPatch.Allocate();
        }

        
    }
}
