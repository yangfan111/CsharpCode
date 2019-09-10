using System.Collections;
using System.IO;
using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    public interface INetworkObjectSerializer
    {
        void Serialize(INetworkObject lastNetworkObject, INetworkObject networkObject, BitArray32 fieldsMask, bool DoCompress, MyBinaryWriter writer);
        void SerializeAll(INetworkObject networkObject, bool DoCompress, MyBinaryWriter writer);
        void Deserialize(INetworkObject networkObject, BitArray32 fieldsMask, bool DoCompress, BinaryReader reader);
        void DeserializeAll(INetworkObject networkObject, bool DoCompress, BinaryReader reader);
        void Merge(INetworkObject from, INetworkObject to, BitArray32 fieldsMask);
        BitArray32 DiffNetworkObject(INetworkObject baseObject, INetworkObject newObject);
        
    }

    public interface IComponentFactory
    {
        IGameComponent Create();
    }
}
