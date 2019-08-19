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
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SnapshotSerializer));
        private MyBinaryWriter _binaryWriter;
        
        private INetworkObjectSerializerManager _serializerManager;
        private readonly string _version;
        public SnapshotSerializer(INetworkObjectSerializerManager manager, string version)
        {
            _serializerManager = manager;
            _version = version;
        }

        public INetworkObjectSerializerManager GetSerializerManager()
        {
            return _serializerManager;
        }
        public int Serialize(ISnapshot baseSnap, ISnapshot snap, Stream stream)
        {
//            _binaryWriter = new MyBinaryWriter(stream);
            _binaryWriter = MyBinaryWriter.Allocate(stream);
            Reset();
            snap.Header.Serialize(_binaryWriter,_version);
            var baseMap = baseSnap.EntityMap;
            var currentMap = snap.EntityMap;
            SnapshotPatchGenerator handler = new SnapshotPatchGenerator(_serializerManager);
            EntityMapCompareExecutor.Diff(baseMap, currentMap, handler, "serialize",null);
            SnapshotPatch patch = handler.Detach();
            
            patch.BaseSnapshotSeq = baseSnap.SnapshotSeq;
            patch.Serialize(_binaryWriter, _serializerManager);
            _binaryWriter.ReleaseReference();
            patch.ReleaseReference();
            return _binaryWriter.WriterLenght;
        }

        public SnapshotPatch DeSerialize(BinaryReader reader, out SnapshotHeader header)
        {
            header = new SnapshotHeader();
            string version = header.DeSerialize(reader);
            if (!version.Equals(_version))
            {
                _logger.ErrorFormat("ComponentSerializer Hash {0} Not Equal{1}",_version,version);
            }
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
