using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using Utils.Utils.Buildin;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public class ModifyComponentPatch : AbstractComponentPatch
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(ModifyComponentPatch)){}
            public override object MakeObject()
            {
                return new ModifyComponentPatch();
            }

        }
        #region DebugInfo

        public static string PrintDebugInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<p>Serialize Info</p>");
                sb.Append("<table width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
                sb.Append("<thead>");
                sb.Append("<td>name</td>");
                sb.Append("<td>Total</td>");
                sb.Append("<td>Average</td>");
                sb.Append("<td>Current</td>");
                sb.Append("</thead>");

                for (int i =0 ;i<_total.Length;i++)
                {
                    if(_total[i] <=0) continue;
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (i >= (int) ECoreComponentIds.End)
                    {
                        sb.Append((EComponentIds)i);
                    }
                    else
                    {
                        sb.Append((ECoreComponentIds)i);
                    }
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(_total[i]);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(_average[i]);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(_current[i]);
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }

                return sb.ToString();

            }

        }

        
        private static long[] _total = new long[512];
        private static long[] _average = new long[512];
        private static long[] _current = new long[512];
        private static long[]  _count =  new long[512];
        #endregion
        public static ModifyComponentPatch Allocate()
        {
            return ObjectAllocatorHolder<ModifyComponentPatch>.Allocate();
        }

        public static ModifyComponentPatch Allocate(IGameComponent leftComponent, IGameComponent comp, BitArray32 mask)
        {
            var rc = ObjectAllocatorHolder<ModifyComponentPatch>.Allocate();
			rc.CreateGameComponent(comp.GetComponentId());
	        (rc.Component as INetworkObject).CopyFrom(comp);
            rc.CreateLastGameComponent(comp.GetComponentId());
            (rc.LastComponent as INetworkObject).CopyFrom(leftComponent);
			rc._bitMask = mask;
            return rc;
        }

        private IGameComponent _lastComponent;
        public IGameComponent LastComponent
        {
            get { return _lastComponent; }
        }

        public void CreateLastGameComponent(int componentId)
        {
            if (_lastComponent != null)
            {
                GameComponentInfo.Instance.Free(_lastComponent);
            }
            _lastComponent = GameComponentInfo.Instance.Allocate(componentId);
        }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            if (_lastComponent != null)
            {
                GameComponentInfo.Instance.Free(_lastComponent);
            }

            _lastComponent = null;
            ObjectAllocatorHolder<ModifyComponentPatch>.Free(this);
        }
        private BitArray32 _bitMask =new BitArray32();

      
        protected ModifyComponentPatch()
        {
            
        }
        

        
        public override void ApplyPatchTo(IGameEntity entity,INetworkObjectSerializerManager serializerManager)
        {
            var componentTypeId = Component.GetComponentId();
            INetworkObject targetComponent = entity.GetComponent(componentTypeId) as INetworkObject;
            INetworkObjectSerializer componentSerializer = serializerManager.GetSerializer(componentTypeId);
            componentSerializer.Merge(Component as INetworkObject,targetComponent,_bitMask);
        }

        public override void ApplyPatchTo(INetworkObject target, INetworkObjectSerializerManager serializerManager)
        {
            var componentTypeId = Component.GetComponentId();
            INetworkObjectSerializer componentSerializer = serializerManager.GetSerializer(componentTypeId);
            componentSerializer.Merge(Component as INetworkObject,target,_bitMask);
        }

        public override void Serialize(MyBinaryWriter writer, INetworkObjectSerializerManager serializerManager)
        {
            var start = writer.BaseStream.Length;
            writer.Write((byte)ComponentReplicateOperationType.Mod);
            var componentId = Component.GetComponentId();
            AssertUtility.Assert(componentId < 65535);
            writer.Write((short)componentId);
            _bitMask.Serialize(writer);
            
            var componentSerializer = serializerManager.GetSerializer(componentId);
            componentSerializer.Serialize(LastComponent as INetworkObject, Component as INetworkObject,_bitMask, DoCompress, writer);
            var end = writer.BaseStream.Length;
            var length = end - start;
            
            _total[componentId] += length;
            _current[componentId] = length;
            _count[componentId] += 1;
            _average[componentId] = _total[componentId] / _count[componentId];
        }

        public override void DeSerialize(BinaryReader reader,  INetworkObjectSerializerManager serializerManager)
        {
            int typeId = reader.ReadInt16();
            CreateGameComponent(typeId);
            var componentSerializer = serializerManager.GetSerializer(typeId);
            _bitMask.Deserialize(reader);
            var networkObject = Component as INetworkObject;
            componentSerializer.Deserialize(networkObject, _bitMask, DoCompress, reader);
        }

        }
       
    
}
