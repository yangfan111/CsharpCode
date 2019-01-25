using System.IO;
using Core.EntityComponent;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Patch;
using Core.Utils;
using UnityEngine.Profiling;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    
    public class SnapshotSerializer : ISnapshotSerializer
    {
        private MyBinaryWriter _binaryWriter;
        
        private INetworkObjectSerializerManager _serializerManager;
        
        public SnapshotSerializer(INetworkObjectSerializerManager manager)
        {
            
            
            _serializerManager = manager;
        }

        public INetworkObjectSerializerManager GetSerializerManager()
        {
            return _serializerManager;
        }
        public void Serialize(ISnapshot baseSnap, ISnapshot snap, Stream stream)
        {
//            _binaryWriter = new MyBinaryWriter(stream);
            _binaryWriter = MyBinaryWriter.Allocate(stream);
            Reset();
            snap.Header.Serialize(_binaryWriter);
            var baseMap = baseSnap.EntityMap;
            var currentMap = snap.EntityMap;
            SnapshotPatchGenerator handler = new SnapshotPatchGenerator(_serializerManager);
            EntityMapComparator.Diff(baseMap, currentMap, handler, "serialize");
            SnapshotPatch patch = handler.Detach();
            
            patch.BaseSnapshotSeq = baseSnap.SnapshotSeq;
            patch.Serialize(_binaryWriter, _serializerManager);
            _binaryWriter.ReleaseReference();
            patch.ReleaseReference();
        }

        public SnapshotPatch DeSerialize(BinaryReader reader, out SnapshotHeader header)
        {
            header = new SnapshotHeader();
            header.DeSerialize(reader);
            SnapshotPatch patch = SnapshotPatch.Allocate();
            patch.DeSerialize(reader, _serializerManager);
            return patch;
        }

        private void Reset()
        {
           
        }

        private void SerializeBaseSnapshotId(int baseId,MyBinaryWriter writer)
        {
            writer.Write(baseId);
        }

        private int ReadBaseSnapshotId(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

	    public void Dispose()
	    {
	    }
    }
}
