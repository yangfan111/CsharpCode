using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{

    public class DeleteComponentPatch : AbstractComponentPatch
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(DeleteComponentPatch)){}
            public override object MakeObject()
            {
                return new DeleteComponentPatch();
            }

        }
        public static DeleteComponentPatch Allocate()
        {
            return ObjectAllocatorHolder<DeleteComponentPatch>.Allocate();
        }


        public static DeleteComponentPatch Allocate(IGameComponent comp)
        {
            var rc= ObjectAllocatorHolder<DeleteComponentPatch>.Allocate();
			rc.CreateGameComponent(comp.GetComponentId());
            // ReSharper disable once PossibleNullReferenceException
            (rc.Component as INetworkObject).CopyFrom(comp);
			return rc;
        }
        protected override void OnCleanUp()
        {
            ObjectAllocatorHolder<DeleteComponentPatch>.Free(this);
        }

       

        protected DeleteComponentPatch()
        {
            
        }
        public override void ApplyPatchTo(IGameEntity entity,INetworkObjectSerializerManager serializerManager)
        {
            entity.RemoveComponent(Component.GetComponentId());
        }

        public override void ApplyPatchTo(INetworkObject target, INetworkObjectSerializerManager serializerManager)
        {
            throw new System.NotImplementedException();
        }

        public override void Serialize(MyBinaryWriter writer, INetworkObjectSerializerManager serializerManager)
        {
            writer.Write((byte)ComponentReplicateOperationType.Del);
            writer.Write((short)Component.GetComponentId());
        }

        public override void DeSerialize(BinaryReader reader, INetworkObjectSerializerManager serializerManager)
        {
            int typeId = reader.ReadInt16();
            CreateGameComponent(typeId);
        }
        
    }
}
