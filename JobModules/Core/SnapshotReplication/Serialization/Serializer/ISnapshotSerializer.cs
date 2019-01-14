using System;
using System.IO;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Patch;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    public interface ISnapshotSerializer : IDisposable
    {
        /// <summary>
        /// 序列化要发送的snapshot
        /// </summary>
        /// <param name="baseSnap">用来做deltaCompress的基础Snapshot</param>
        /// <param name="snap">要发送的Snapshot</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        void Serialize(ISnapshot baseSnap, ISnapshot snap, Stream stream);

        SnapshotPatch DeSerialize(BinaryReader readert, out SnapshotHeader header);

        INetworkObjectSerializerManager GetSerializerManager();
	    
    }
}
