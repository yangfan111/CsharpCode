using System;
using Core.EntityComponent;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    public interface INetworkObjectSerializerManager
    {
        INetworkObjectSerializer GetSerializer(int typeId);
        IGameComponent CreateComponentById(int id);
    }
}
