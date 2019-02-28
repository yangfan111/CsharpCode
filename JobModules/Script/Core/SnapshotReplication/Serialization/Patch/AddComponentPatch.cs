using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public class AddComponentPatch : AbstractComponentPatch
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(AddComponentPatch)){}
            public override object MakeObject()
            {
                return new AddComponentPatch();
            }

        }
        public static AddComponentPatch Allocate(IGameComponent comp)
        {
            var rc = ObjectAllocatorHolder<AddComponentPatch>.Allocate();

	        rc.CreateGameComponent(comp.GetComponentId());
            // ReSharper disable once PossibleNullReferenceException
			(rc.Component as INetworkObject).CopyFrom(comp);
			return rc;
        }

        public static AddComponentPatch Allocate()
        {
            return ObjectAllocatorHolder<AddComponentPatch>.Allocate();
        }

        protected override void OnCleanUp()
        {
            ObjectAllocatorHolder<AddComponentPatch>.Free(this);
        }




        protected AddComponentPatch()
        {
            
        }


        public override void ApplyPatchTo(IGameEntity entity,INetworkObjectSerializerManager serializerManager)
        {
            var copyCopy = entity.AddComponent(Component.GetComponentId(), Component);
           
        }

        public override void ApplyPatchTo(INetworkObject target, INetworkObjectSerializerManager serializerManager)
        {
       
            target.CopyFrom(Component);
        }

        public override void Serialize(MyBinaryWriter writer , INetworkObjectSerializerManager serializerManager)
        {
            writer.Write((byte)ComponentReplicateOperationType.Add);
            var componentId = Component.GetComponentId();
            writer.Write((short)componentId);

            var componentSerializer = serializerManager.GetSerializer(componentId);
            componentSerializer.SerializeAll(Component as INetworkObject, DoCompress, writer);
        }

        public override void DeSerialize(BinaryReader reader, INetworkObjectSerializerManager serializerManager)
        {
            var typeId = reader.ReadInt16();
            CreateGameComponent(typeId); 
            var componentSerializer = serializerManager.GetSerializer(typeId);
            componentSerializer.DeserializeAll(Component as INetworkObject, DoCompress, reader);
        }

        
        
    }
}
