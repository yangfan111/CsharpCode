using System;
using System.Collections.Generic;
using System.IO;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    

    public class ModifyEntityPatch : AbstractEntityPatch
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(ModifyEntityPatch)){}
            public override object MakeObject()
            {
                return new ModifyEntityPatch();
            }



        }
        public static ModifyEntityPatch Allocate()
        {
            return ObjectAllocatorHolder<ModifyEntityPatch>.Allocate();
        }

        public static ModifyEntityPatch Allocate(IGameEntity entity)
        {
            var rc =  ObjectAllocatorHolder<ModifyEntityPatch>.Allocate();
            rc.Key = entity.EntityKey;
            return rc;
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            ObjectAllocatorHolder<ModifyEntityPatch>.Free(this);
        }

        protected ModifyEntityPatch(EntityKey entity, List<AbstractComponentPatch> patchList) :base(entity, patchList)
        {
        }

        protected ModifyEntityPatch()
        {
            
        }

        protected ModifyEntityPatch(EntityKey entity) :base(entity)
        {

        }






        public override void ApplyPatchTo(ISnapshot snap,INetworkObjectSerializerManager serializerManager)
        {
            var baseEntity  = snap.GetEntity(Key);
            foreach (var compPatch in ComponentPatchList)
            {
                compPatch.ApplyPatchTo(baseEntity,serializerManager);
            }
            
        }

        public override void Serialize(MyBinaryWriter writer ,INetworkObjectSerializerManager serializerManager)
        {
            writer.Write((byte)EntityReplicateOperationType.Mod);
            writer.Write(Key.EntityId);
            writer.Write(Key.EntityType);
            writer.Write((short)ComponentPatchList.Count);
            for (int i = 0; i < ComponentPatchList.Count; i++)
            {
                ComponentPatchList[i].Serialize(writer, serializerManager);
            }
        }

        public override void DeSerialize(BinaryReader reader , INetworkObjectSerializerManager serializerManager)
        {
            var id = reader.ReadInt32();
            short type = reader.ReadInt16();
            Key = new EntityKey(id, type);
            int count = reader.ReadInt16();
            AssertUtility.Assert(ComponentPatchList.Count == 0);
            for (int i = 0; i < count; i++)
            {
                var opType = (ComponentReplicateOperationType)reader.ReadByte();
                
                var patch = CreateEmptyComponentPatch(opType);
                patch.DeSerialize(reader, serializerManager);
                ComponentPatchList.Add(patch);
            }
        }

        private AbstractComponentPatch CreateEmptyComponentPatch(ComponentReplicateOperationType opType)
        {
            switch (opType)
            {
                case ComponentReplicateOperationType.Add:
                    return Patch.AddComponentPatch.Allocate();
                case ComponentReplicateOperationType.Del:
                    return DeleteComponentPatch.Allocate(); 
                case ComponentReplicateOperationType.Mod:
                    return ModifyComponentPatch.Allocate();
                default:
                    throw new Exception(string.Format("invalid ComponentReplicateOperationType {0}",opType ));
            }
        }
    }
}
